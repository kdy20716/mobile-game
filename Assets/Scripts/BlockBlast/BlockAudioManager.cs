using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockBlast
{
    public class BlockAudioManager : MonoBehaviour
    {
        public static BlockAudioManager Instance { get; private set; }

        [Header("SFX Audio Clips (Optional - falls back to procedural ASMR if empty)")]
        [Tooltip("블록 집을 때 (sfx_block_pickup.wav)")]
        public AudioClip sfxPickup;
        [Tooltip("블록 놓을 때 (sfx_block_place.wav)")]
        public AudioClip sfxPlace;
        [Tooltip("블록 터질 때 (sfx_block_clear.wav)")]
        public AudioClip sfxClear;
        [Tooltip("UI 버튼 클릭 (sfx_ui_click.wav)")]
        public AudioClip sfxUIClick;
        [Tooltip("화면 터치/탭 (sfx_screen_touch.wav)")]
        public AudioClip sfxScreenTouch;
        [Tooltip("폭탄 블록 폭발 (sfx_block_bomb.wav)")]
        public AudioClip sfxBomb;
        [Tooltip("블록 회전 (sfx_block_rotate.wav)")]
        public AudioClip sfxRotate;
        [Tooltip("블록 스킵 (sfx_block_skip.wav)")]
        public AudioClip sfxSkip;
        [Tooltip("피버 모드 돌입 (sfx_fever_start.wav)")]
        public AudioClip sfxFever;
        [Tooltip("게임 오버 (sfx_game_over.wav)")]
        public AudioClip sfxGameOver;

        [Header("BGM Audio Clip")]
        [Tooltip("인게임 배경음악 (bgm_main.mp3)")]
        public AudioClip bgmMain;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float sfxVolume = 0.8f;
        [Range(0f, 1f)] public float bgmVolume = 0.45f;

        private AudioSource _sfxSource;
        private AudioSource _bgmSource;

        // Sweet Kalimba / Music Box Pentatonic Frequencies (C4 to C6)
        private readonly float[] _kalimbaFreqs = new float[]
        {
            523.25f, // C5 (Do)
            587.33f, // D5 (Re)
            659.25f, // E5 (Mi)
            783.99f, // G5 (Sol)
            880.00f, // A5 (La)
            1046.50f,// C6 (High Do)
            1174.66f,// D6 (High Re)
            1318.51f // E6 (High Mi)
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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
            _bgmSource.loop = true;
        }

        private void Start()
        {
            if (bgmMain != null)
            {
                PlayBGM(bgmMain);
            }
        }

        private void Update()
        {
            // 화면 터치 효과음 (UI 터치가 아닐 때 빈 공간 탭 사운드)
            if (sfxScreenTouch != null)
            {
                bool isTouchBegan = false;
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                {
                    isTouchBegan = true;
                }
                else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                {
                    isTouchBegan = true;
                }
#else
                if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    isTouchBegan = true;
                }
#endif

                if (isTouchBegan)
                {
                    if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
                    {
                        PlayScreenTouch();
                    }
                }
            }
        }

        // 🫧 1. 블록 집을 때 (Pick Up)
        public void PlayPickup()
        {
            if (sfxPickup != null)
            {
                _sfxSource.PlayOneShot(sfxPickup, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateBubblePop(420f, 680f, 0.09f);
                _sfxSource.PlayOneShot(clip, 0.45f * sfxVolume);
            }
        }

        // 🪵 2. 블록 놓을 때 (Place)
        public void PlayPlace()
        {
            if (sfxPlace != null)
            {
                _sfxSource.PlayOneShot(sfxPlace, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateSoftTap(260f, 0.08f);
                _sfxSource.PlayOneShot(clip, 0.55f * sfxVolume);
            }
        }

        // 🔔 3. 블록 터질 때 (Clear / Blast with Combo Pitch)
        public void PlayClear(int combo)
        {
            if (sfxClear != null)
            {
                // 콤보마다 피치가 반음씩 경쾌하게 상승 (최대 1.5배)
                float pitch = Mathf.Clamp(1.0f + (combo - 1) * 0.07f, 1.0f, 1.6f);
                _sfxSource.pitch = pitch;
                _sfxSource.PlayOneShot(sfxClear, sfxVolume);
                _sfxSource.pitch = 1.0f;
            }
            else
            {
                int noteIdx = Mathf.Clamp(combo - 1, 0, _kalimbaFreqs.Length - 1);
                float freq = _kalimbaFreqs[noteIdx];
                AudioClip clip = GenerateKalimbaNote(freq, 0.35f);
                _sfxSource.PlayOneShot(clip, 0.7f * sfxVolume);
            }
        }

        // 🔘 4. UI 버튼 클릭 (UI Click)
        public void PlayUIClick()
        {
            if (sfxUIClick != null)
            {
                _sfxSource.PlayOneShot(sfxUIClick, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateSoftTap(450f, 0.05f);
                _sfxSource.PlayOneShot(clip, 0.5f * sfxVolume);
            }
        }

        // 👆 5. 화면 터치 (Screen Touch)
        public void PlayScreenTouch()
        {
            if (sfxScreenTouch != null)
            {
                _sfxSource.PlayOneShot(sfxScreenTouch, sfxVolume * 0.7f);
            }
        }

        // 🎉 6. 폭탄 블록 폭발 (Bomb)
        public void PlayBomb()
        {
            if (sfxBomb != null)
            {
                _sfxSource.PlayOneShot(sfxBomb, sfxVolume);
            }
            else
            {
                AudioClip clip = GeneratePartyPopper(0.3f);
                _sfxSource.PlayOneShot(clip, 0.8f * sfxVolume);
            }
        }

        // 🔄 7. 블록 회전 (Rotate)
        public void PlayRotate()
        {
            if (sfxRotate != null)
            {
                _sfxSource.PlayOneShot(sfxRotate, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateBubblePop(520f, 780f, 0.08f);
                _sfxSource.PlayOneShot(clip, 0.5f * sfxVolume);
            }
        }

        // ⏭️ 8. 블록 스킵 (Skip)
        public void PlaySkip()
        {
            if (sfxSkip != null)
            {
                _sfxSource.PlayOneShot(sfxSkip, sfxVolume);
            }
            else
            {
                PlayUIClick();
            }
        }

        // 🔥 9. 피버 모드 돌입 (Fever)
        public void PlayFever()
        {
            if (sfxFever != null)
            {
                _sfxSource.PlayOneShot(sfxFever, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateKalimbaNote(1046.5f, 0.6f);
                _sfxSource.PlayOneShot(clip, 0.8f * sfxVolume);
            }
        }

        // 💀 10. 게임 오버 (Game Over)
        public void PlayGameOver()
        {
            if (sfxGameOver != null)
            {
                _sfxSource.PlayOneShot(sfxGameOver, sfxVolume);
            }
            else
            {
                AudioClip clip = GenerateSoftTap(160f, 0.4f);
                _sfxSource.PlayOneShot(clip, 0.7f * sfxVolume);
            }
        }

        // 🎶 BGM 제어
        public void PlayBGM(AudioClip clip)
        {
            if (clip == null) return;
            _bgmSource.clip = clip;
            _bgmSource.volume = bgmVolume;
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        // --- Procedural ASMR Sound Synthesizers (안전 대체용) ---

        private AudioClip GenerateBubblePop(float startFreq, float endFreq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, Mathf.Pow(t, 0.5f));
                float phase = 2f * Mathf.PI * currentFreq * ((float)i / sampleRate);
                float env = Mathf.Sin(t * Mathf.PI);
                samples[i] = Mathf.Sin(phase) * env;
            }

            AudioClip clip = AudioClip.Create("BubblePop", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateSoftTap(float baseFreq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 28f);
                float wave = Mathf.Sin(2f * Mathf.PI * baseFreq * t) +
                             0.25f * Mathf.Sin(2f * Mathf.PI * (baseFreq * 2.2f) * t);

                samples[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create("SoftTap", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateKalimbaNote(float freq, float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 6.5f);
                float fundamental = Mathf.Sin(2f * Mathf.PI * freq * t);
                float harmonic = 0.35f * Mathf.Sin(2f * Mathf.PI * (freq * 2.76f) * t);
                float shimmer = 0.15f * Mathf.Sin(2f * Mathf.PI * (freq * 4.02f) * t);

                samples[i] = (fundamental + harmonic + shimmer) * env;
            }

            AudioClip clip = AudioClip.Create($"Kalimba_{freq}", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GeneratePartyPopper(float duration)
        {
            int sampleRate = 44100;
            int count = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[count];

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 12f);
                float popFreq = Mathf.Lerp(380f, 90f, t * 8f);
                float pop = Mathf.Sin(2f * Mathf.PI * popFreq * t);
                float sparkle = (Random.value * 2f - 1f) * 0.3f * Mathf.Exp(-t * 20f);

                samples[i] = (pop * 0.7f + sparkle) * env;
            }

            AudioClip clip = AudioClip.Create("PartyPopper", count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
