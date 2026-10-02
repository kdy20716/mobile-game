using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockBlast
{
    public class BlockAudioManager : MonoBehaviour
    {
        public static BlockAudioManager Instance { get; private set; }

        public const float BASE_SFX_VOLUME = 0.22f; // Comfortable, gentle default (at 50% slider)
        public const float BASE_BGM_VOLUME = 0.12f; // Soothing, comfortable background default (at 50% slider)
        public const float DEFAULT_SLIDER_PERCENT = 0.50f; // Slider bar default: 50%
        public const float MAX_SFX_VOLUME = BASE_SFX_VOLUME * 2f; // 0.44f
        public const float MAX_BGM_VOLUME = BASE_BGM_VOLUME * 2f; // 0.24f

        public const float DEFAULT_SFX_VOLUME = BASE_SFX_VOLUME;
        public const float DEFAULT_BGM_VOLUME = BASE_BGM_VOLUME;

        public enum BGMState
        {
            None,
            Intro,
            Lobby,
            InGame
        }

        [Header("SFX Audio Clips")]
        [Tooltip("상점에서 구매할 때 (buy.wav)")]
        public AudioClip sfxBuy;

        [Tooltip("화면 터치/클릭 (click.wav)")]
        public AudioClip sfxClick;

        [Tooltip("블록을 집을 때 (grab.wav)")]
        public AudioClip sfxGrab;

        [Tooltip("블록을 판에 놓을 때 (grab-down.wav)")]
        public AudioClip sfxGrabDown;

        [Tooltip("3초 남을 때 경고 (warning.wav)")]
        public AudioClip sfxWarning;

        [Tooltip("창 열기/닫기 (window.wav)")]
        public AudioClip sfxWindow;

        [Header("BGM Audio Clips")]
        [Tooltip("첫 화면 인트로 음악 (Intro)")]
        public AudioClip bgmIntro;

        [Tooltip("로비 기본 배경 음악 (Lobby fallback)")]
        public AudioClip bgmLobby;

        [Tooltip("로비 배경 음악 트랙 리스트 (Lobby - 1, 2, 3 테마별 매핑)")]
        public List<AudioClip> bgmLobbyTracks = new List<AudioClip>();

        [Tooltip("인게임 배경 음악 플레이리스트 (InGame - 번호순 정렬 및 무한 루프)")]
        public List<AudioClip> bgmInGamePlaylist = new List<AudioClip>();

        [Header("Volume Settings (50% UI Slider Calibrated)")]
        [Range(0f, 1f)] public float sfxSliderLevel = DEFAULT_SLIDER_PERCENT;
        [Range(0f, 1f)] public float bgmSliderLevel = DEFAULT_SLIDER_PERCENT;

        public float sfxVolume => Mathf.Clamp01(sfxSliderLevel * MAX_SFX_VOLUME);
        public float bgmVolume => Mathf.Clamp01(bgmSliderLevel * MAX_BGM_VOLUME);

        private AudioSource _sfxSource;
        private AudioSource _bgmSource;

        private BGMState _currentBgmState = BGMState.None;
        private int _currentInGameTrackIndex = 0;
        private Coroutine _fadeCoroutine;

        public BGMState CurrentBgmState => _currentBgmState;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;

            // Enforce 50% default slider level on settings bar while keeping current audio volume intact
            if (!PlayerPrefs.HasKey("Sound_Volume_Calibrated_50Percent_V8"))
            {
                sfxSliderLevel = DEFAULT_SLIDER_PERCENT; // 0.5f (50%)
                bgmSliderLevel = DEFAULT_SLIDER_PERCENT; // 0.5f (50%)
                PlayerPrefs.SetFloat("SFX_Slider_Level", sfxSliderLevel);
                PlayerPrefs.SetFloat("BGM_Slider_Level", bgmSliderLevel);
                PlayerPrefs.SetInt("Sound_Volume_Calibrated_50Percent_V8", 1);
                PlayerPrefs.Save();
            }
            else
            {
                sfxSliderLevel = PlayerPrefs.GetFloat("SFX_Slider_Level", DEFAULT_SLIDER_PERCENT);
                bgmSliderLevel = PlayerPrefs.GetFloat("BGM_Slider_Level", DEFAULT_SLIDER_PERCENT);
            }

            ReloadClipsFromEditorAssets();
        }

        private void Start()
        {
            ReloadClipsFromEditorAssets();

            // Do not play Intro BGM while splash screen is active!
            // MainMenuCinematicController will call PlayIntroBGM() when the splash screen finishes or is skipped.
            bool isSplashActive = SplashScreenController.Instance != null && SplashScreenController.Instance.IsActive;
            if (!isSplashActive)
            {
                PlayIntroBGM();
            }
        }

        private void Update()
        {
            HandleGlobalScreenTouch();
            HandleInGamePlaylistProgression();
        }

        private void HandleGlobalScreenTouch()
        {
            bool isTouchBegan = false;
            Vector2 touchPos = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                isTouchBegan = true;
                touchPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
            else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                isTouchBegan = true;
                touchPos = UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                isTouchBegan = true;
                touchPos = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isTouchBegan = true;
                touchPos = Input.GetTouch(0).position;
            }
#endif

            if (isTouchBegan)
            {
                // Mobile WebGL audio recovery: ensure BGM starts playing upon first user touch
                if (_currentBgmState == BGMState.Lobby && (!_bgmSource.isPlaying || _bgmSource.time <= 0f))
                {
                    PlayLobbyBGM();
                }
                else if (_currentBgmState == BGMState.Intro && (!_bgmSource.isPlaying || _bgmSource.time <= 0f))
                {
                    PlayIntroBGM();
                }

                // Check if the pointer is touching a DraggableBlockUI in hand slots
                bool isOverDraggableBlock = false;
                if (EventSystem.current != null)
                {
                    PointerEventData peData = new PointerEventData(EventSystem.current) { position = touchPos };
                    List<RaycastResult> hits = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(peData, hits);

                    for (int i = 0; i < hits.Count; i++)
                    {
                        if (hits[i].gameObject != null && hits[i].gameObject.GetComponentInParent<DraggableBlockUI>() != null)
                        {
                            isOverDraggableBlock = true;
                            break;
                        }
                    }
                }

                // If not grabbing a block, play screen touch click sound (50% volume scaled)
                if (!isOverDraggableBlock)
                {
                    PlayClick();
                }
            }
        }

        private void HandleInGamePlaylistProgression()
        {
            if (_currentBgmState == BGMState.InGame && bgmInGamePlaylist != null && bgmInGamePlaylist.Count > 0)
            {
                if (!_bgmSource.isPlaying && _bgmSource.clip != null)
                {
                    // Current track finished; advance to next track in playlist (loops back to 0 at end)
                    _currentInGameTrackIndex = (_currentInGameTrackIndex + 1) % bgmInGamePlaylist.Count;
                    AudioClip nextClip = bgmInGamePlaylist[_currentInGameTrackIndex];
                    if (nextClip != null)
                    {
                        _bgmSource.clip = nextClip;
                        _bgmSource.loop = (bgmInGamePlaylist.Count == 1);
                        _bgmSource.volume = bgmVolume;
                        _bgmSource.Play();
                    }
                }
            }
        }

        // ==========================================
        // SFX PLAYBACK METHODS (50% Default Volume)
        // ==========================================

        public void PlayClick()
        {
            if (sfxClick != null)
            {
                _sfxSource.PlayOneShot(sfxClick, sfxVolume);
            }
        }

        public void PlayGrab()
        {
            if (sfxGrab != null)
            {
                _sfxSource.PlayOneShot(sfxGrab, sfxVolume);
            }
        }

        public void PlayGrabDown()
        {
            if (sfxGrabDown != null)
            {
                _sfxSource.PlayOneShot(sfxGrabDown, sfxVolume);
            }
        }

        public void PlayWarning()
        {
            if (sfxWarning != null)
            {
                _sfxSource.PlayOneShot(sfxWarning, sfxVolume);
            }
        }

        public void PlayWindow()
        {
            if (sfxWindow != null)
            {
                _sfxSource.PlayOneShot(sfxWindow, sfxVolume);
            }
        }

        public void PlayBuy()
        {
            if (sfxBuy != null)
            {
                _sfxSource.PlayOneShot(sfxBuy, sfxVolume);
            }
        }

        public void PlayCustomSFX(AudioClip clip, float volumeMultiplier = 1.0f)
        {
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeMultiplier));
            }
        }

        private AudioClip _fairyChimeClip;

        public void PlayFairyChime()
        {
            if (_fairyChimeClip == null)
            {
                _fairyChimeClip = CreateFairyChimeClip();
            }
            if (_fairyChimeClip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(_fairyChimeClip, Mathf.Clamp01(sfxVolume * 1.35f));
            }
        }

        private AudioClip CreateFairyChimeClip()
        {
            int sampleRate = 44100;
            float duration = 0.85f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            // 5 sparkling notes arpeggio: C6, E6, G6, C7, E7 ("뾰로롱~~")
            float[] freqs = new float[] { 1046.5f, 1318.5f, 1568.0f, 2093.0f, 2637.0f };
            float noteDelay = 0.08f;

            for (int n = 0; n < freqs.Length; n++)
            {
                int startSample = (int)(n * noteDelay * sampleRate);
                float f = freqs[n];
                for (int i = startSample; i < totalSamples; i++)
                {
                    float t = (i - startSample) / (float)sampleRate;
                    float env = Mathf.Exp(-t * 9.5f);
                    float wave = Mathf.Sin(2f * Mathf.PI * f * t) + 0.35f * Mathf.Sin(4f * Mathf.PI * f * t);
                    samples[i] += wave * env * 0.16f;
                }
            }

            AudioClip clip = AudioClip.Create("FairyChime", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // Backward compatibility wrappers
        public void PlayUIClick()
        {
            // Empty by design: Screen touch click handler already triggers PlayClick()
        }

        public void PlayPickup()
        {
            PlayGrab();
        }

        public void PlayPlace()
        {
            PlayGrabDown();
        }

        // Removed old procedural synth sounds
        public void PlayClear(int combo = 1) { }
        public void PlayBomb() { }
        public void PlayRotate() { }
        public void PlaySkip() { }
        public void PlayFever() { }
        public void PlayGameOver() { }

        // ==========================================
        // BGM PLAYBACK & TRANSITIONS
        // ==========================================

        public void PlayIntroBGM()
        {
            if (bgmIntro == null) return;
            if (_currentBgmState == BGMState.Intro && _bgmSource.isPlaying) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _currentBgmState = BGMState.Intro;
            _bgmSource.loop = true;
            _bgmSource.clip = bgmIntro;
            _bgmSource.volume = bgmVolume;
            _bgmSource.Play();
        }

        public void StopIntroBGM(float fadeDuration = 0.2f)
        {
            if (_currentBgmState != BGMState.Intro) return;
            StopBGM(fadeDuration);
            _currentBgmState = BGMState.None;
        }

        public void PlayLobbyBGM(int themeIndex = 0)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _currentBgmState = BGMState.Lobby;

            AudioClip targetTrack = null;
            if (bgmLobbyTracks != null && bgmLobbyTracks.Count > 0)
            {
                int idx = Mathf.Clamp(themeIndex, 0, bgmLobbyTracks.Count - 1);
                targetTrack = bgmLobbyTracks[idx];
            }
            else if (bgmLobby != null)
            {
                targetTrack = bgmLobby;
            }

            if (targetTrack != null)
            {
                _bgmSource.clip = targetTrack;
                _bgmSource.loop = true;
                _bgmSource.volume = bgmVolume;
                if (!_bgmSource.isPlaying || _bgmSource.time <= 0f)
                {
                    _bgmSource.Play();
                }
            }
            else
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }
        }

        public void PlayInGameBGM()
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _currentBgmState = BGMState.InGame;

            if (bgmInGamePlaylist != null && bgmInGamePlaylist.Count > 0)
            {
                _currentInGameTrackIndex = 0;
                AudioClip firstTrack = bgmInGamePlaylist[_currentInGameTrackIndex];
                if (firstTrack != null)
                {
                    _bgmSource.clip = firstTrack;
                    _bgmSource.loop = (bgmInGamePlaylist.Count == 1);
                    _bgmSource.volume = bgmVolume;
                    _bgmSource.Play();
                }
            }
            else
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }
        }

        public void StopBGM(float fadeDuration = 0.2f)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            if (fadeDuration <= 0f)
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeOutBGMRoutine(fadeDuration));
            }
        }

        private IEnumerator FadeOutBGMRoutine(float dur)
        {
            float startVol = _bgmSource.volume;
            float elapsed = 0f;
            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(startVol, 0f, elapsed / dur);
                yield return null;
            }
            _bgmSource.Stop();
            _bgmSource.clip = null;
            _bgmSource.volume = bgmVolume;
        }

        public void SetBGMVolume(float sliderValue)
        {
            bgmSliderLevel = Mathf.Clamp01(sliderValue);
            if (_bgmSource != null) _bgmSource.volume = bgmVolume;
            PlayerPrefs.SetFloat("BGM_Slider_Level", bgmSliderLevel);
            PlayerPrefs.Save();
        }

        public void SetSFXVolume(float sliderValue)
        {
            sfxSliderLevel = Mathf.Clamp01(sliderValue);
            if (_sfxSource != null) _sfxSource.volume = sfxVolume;
            PlayerPrefs.SetFloat("SFX_Slider_Level", sfxSliderLevel);
            PlayerPrefs.Save();
        }

        public void ReloadClipsFromEditorAssets()
        {
#if UNITY_EDITOR
            if (sfxBuy == null) sfxBuy = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/buy.wav");
            if (sfxClick == null) sfxClick = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/click.wav");
            if (sfxGrab == null) sfxGrab = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/grab.wav");
            if (sfxGrabDown == null) sfxGrabDown = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/grab-down.wav");
            if (sfxWarning == null) sfxWarning = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/warning.wav");
            if (sfxWindow == null) sfxWindow = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/window.wav");

            // Scan Intro BGM folder
            if (bgmIntro == null && UnityEditor.AssetDatabase.IsValidFolder("Assets/Sounds/BGM/Intro"))
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new string[] { "Assets/Sounds/BGM/Intro" });
                if (guids != null && guids.Length > 0)
                {
                    bgmIntro = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
                }
            }

            // Scan Lobby BGM folder (sorted 1, 2, 3)
            if (UnityEditor.AssetDatabase.IsValidFolder("Assets/Sounds/BGM/Lobby"))
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new string[] { "Assets/Sounds/BGM/Lobby" });
                if (guids != null && guids.Length > 0)
                {
                    bgmLobbyTracks.Clear();
                    foreach (var g in guids)
                    {
                        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(UnityEditor.AssetDatabase.GUIDToAssetPath(g));
                        if (clip != null) bgmLobbyTracks.Add(clip);
                    }

                    bgmLobbyTracks.Sort((a, b) =>
                    {
                        int numA = ExtractTrackNumber(a.name);
                        int numB = ExtractTrackNumber(b.name);
                        if (numA != numB) return numA.CompareTo(numB);
                        return string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase);
                    });

                    if (bgmLobbyTracks.Count > 0) bgmLobby = bgmLobbyTracks[0];
                }
            }

            // Scan InGame BGM folder (auto-sort by number: Stage 1, Stage 2, ..., Stage 9)
            if (UnityEditor.AssetDatabase.IsValidFolder("Assets/Sounds/BGM/InGame"))
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new string[] { "Assets/Sounds/BGM/InGame" });
                if (guids != null && guids.Length > 0)
                {
                    bgmInGamePlaylist.Clear();
                    foreach (var g in guids)
                    {
                        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(UnityEditor.AssetDatabase.GUIDToAssetPath(g));
                        if (clip != null) bgmInGamePlaylist.Add(clip);
                    }

                    bgmInGamePlaylist.Sort((a, b) =>
                    {
                        int numA = ExtractTrackNumber(a.name);
                        int numB = ExtractTrackNumber(b.name);
                        if (numA != numB) return numA.CompareTo(numB);
                        return string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase);
                    });
                }
            }
#endif
        }

        private static int ExtractTrackNumber(string name)
        {
            var match = System.Text.RegularExpressions.Regex.Match(name, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int result))
            {
                return result;
            }
            return int.MaxValue;
        }
    }
}
