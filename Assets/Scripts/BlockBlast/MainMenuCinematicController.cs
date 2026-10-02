using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace BlockBlast
{
    public class MainMenuCinematicController : MonoBehaviour, IPointerClickHandler
    {
        [Header("Camera & Viewport")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RectTransform cinematicRoot;

        [Header("UI RectTransforms")]
        [SerializeField] private RectTransform leftMascot;
        [SerializeField] private RectTransform rightMascot;
        [SerializeField] private RectTransform logoTitle;
        [SerializeField] private CanvasGroup touchPromptGroup;
        [SerializeField] private TMP_Text touchPromptText;
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private CanvasGroup menuCanvasGroup;

        [Header("Language Logos")]
        [SerializeField] private Image logoImage;
        [SerializeField] private Sprite[] languageLogos;

        [Header("Effects & Managers")]
        [SerializeField] private JellyFireworksEffect fireworksEffect;
        [SerializeField] private BlockBlastUIManager uiManager;

        [Header("Cinematic Audio Clips")]
        [SerializeField] private AudioClip sfxPinkMascot;
        [SerializeField] private AudioClip sfxMintMascot;
        [SerializeField] private AudioClip sfxLogoTitle;
        [SerializeField] private AudioSource audioSource;

        private bool _canTouchToStart = false;
        private bool _hasStarted = false;
        private float _inputAllowedTime = 0f;
        private Coroutine _introCoroutine;
        private Coroutine _idleCoroutine;
        private Coroutine _continuousFireworksCoroutine;

        private void Awake()
        {
            // ─── Enforce Screen Resolution Immediately on Boot ───────────────────────
            // LobbyManager.ApplyScreenSettings() is called later (after the title screen),
            // but the Windows Standalone player may restore a cached registry window size.
            // Reading saved prefs here ensures the first visible frame is already 9:16.
            const string kAspectKey = "Mallang_AspectRatio_Idx";
            const string kWindowKey = "Mallang_WindowMode_Idx";
            int aspectIdx = PlayerPrefs.GetInt(kAspectKey, 3);
            int windowIdx = PlayerPrefs.GetInt(kWindowKey, 1); // Default: 1 (Borderless / FullScreenWindow)
            if (aspectIdx < 0 || aspectIdx > 3) aspectIdx = 3;
            if (windowIdx < 0 || windowIdx > 2) windowIdx = 1;

            FullScreenMode mode = windowIdx == 2 ? FullScreenMode.ExclusiveFullScreen
                                : windowIdx == 1 ? FullScreenMode.FullScreenWindow
                                                 : FullScreenMode.Windowed;
            int w, h;
            if (mode == FullScreenMode.Windowed)
            {
                switch (aspectIdx)
                {
                    case 0: w = 1600; h =  900; break;  // 16:9
                    case 1: w = 1440; h =  900; break;  // 16:10
                    case 2: w = 1200; h =  900; break;  // 4:3
                    default: w = 720; h = 1280; break;  // 9:16
                }
            }
            else if (mode == FullScreenMode.ExclusiveFullScreen)
            {
                switch (aspectIdx)
                {
                    case 0: w = 1920; h = 1080; break;
                    case 1: w = 1920; h = 1200; break;
                    case 2: w = 1440; h = 1080; break;
                    default: w = 1080; h = 1920; break;
                }
            }
            else
            {
                w = Display.main.systemWidth  > 0 ? Display.main.systemWidth  : 1920;
                h = Display.main.systemHeight > 0 ? Display.main.systemHeight : 1080;
            }
            Screen.SetResolution(w, h, mode);
            // ────────────────────────────────────────────────────────────────────────

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            if (menuCanvasGroup == null)
            {
                menuCanvasGroup = GetComponent<CanvasGroup>();
                if (menuCanvasGroup == null) menuCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            ResetToPreIntroState();
            EnsureAudioClips();
            LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
            UpdateLocalizedPrompt();
            UpdateLogo(LocalizationManager.CurrentLanguage);
        }

        public bool CanAcceptStartInput()
        {
            if (_hasStarted) return false;
            if (!_canTouchToStart) return false;
            if (Time.time < _inputAllowedTime) return false;
            if (SplashScreenController.Instance != null && SplashScreenController.Instance.IsActive) return false;
            return true;
        }

        public void ResetToPreIntroState()
        {
            _canTouchToStart = false;
            if (cinematicRoot != null)
            {
                cinematicRoot.localScale = Vector3.one * 1.55f;
                cinematicRoot.anchoredPosition = new Vector2(360f, 60f);
            }
            if (leftMascot != null)
            {
                leftMascot.anchoredPosition = new Vector2(-260f, -80f);
                leftMascot.localScale = Vector3.zero;
            }
            if (rightMascot != null)
            {
                rightMascot.anchoredPosition = new Vector2(260f, -80f);
                rightMascot.localScale = Vector3.zero;
            }
            if (logoTitle != null)
            {
                logoTitle.localScale = Vector3.zero;
            }
            if (touchPromptGroup != null)
            {
                touchPromptGroup.alpha = 0f;
            }
        }

        public void EnsureAudioClips()
        {
            var pickupClip = Resources.Load<AudioClip>("Audio/item_pick_up_04");
            if (pickupClip != null)
            {
                sfxPinkMascot = pickupClip;
                sfxMintMascot = pickupClip;
            }
            else
            {
                if (sfxPinkMascot == null) sfxPinkMascot = Resources.Load<AudioClip>("Audio/item_pick_up_04");
                if (sfxMintMascot == null) sfxMintMascot = Resources.Load<AudioClip>("Audio/item_pick_up_04");
            }
            if (sfxLogoTitle == null) sfxLogoTitle = Resources.Load<AudioClip>("Audio/item_acquired_04");
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
            if (_continuousFireworksCoroutine != null)
            {
                StopCoroutine(_continuousFireworksCoroutine);
                _continuousFireworksCoroutine = null;
            }
        }

        private void OnDisable()
        {
            if (_continuousFireworksCoroutine != null)
            {
                StopCoroutine(_continuousFireworksCoroutine);
                _continuousFireworksCoroutine = null;
            }
        }

        private void OnEnable()
        {
            ResetToPreIntroState();
            UpdateLocalizedPrompt();
            UpdateLogo(LocalizationManager.CurrentLanguage);
        }

        private void HandleLanguageChanged(GameLanguage lang)
        {
            UpdateLocalizedPrompt();
            UpdateLogo(lang);
        }

        private void UpdateLocalizedPrompt()
        {
            if (touchPromptText == null)
            {
                var promptObj = transform.Find("TouchPromptGroup/TouchText");
                if (promptObj != null) touchPromptText = promptObj.GetComponent<TMP_Text>();
                if (touchPromptText == null)
                {
                    var found = GameObject.Find("TouchText");
                    if (found != null) touchPromptText = found.GetComponent<TMP_Text>();
                }
            }
            if (touchPromptText != null)
            {
                touchPromptText.text = LocalizationManager.Get("intro_touch");
            }
        }

        public void SetupLanguageLogos(Image img, Sprite[] logos)
        {
            logoImage = img;
            languageLogos = logos;
            UpdateLogo(LocalizationManager.CurrentLanguage);
        }

        private void UpdateLogo(GameLanguage lang)
        {
            if (logoImage != null && languageLogos != null && (int)lang >= 0 && (int)lang < languageLogos.Length)
            {
                if (languageLogos[(int)lang] != null)
                {
                    logoImage.sprite = languageLogos[(int)lang];
                }
            }
        }

        public void SetupCinematicAudio(AudioClip pink, AudioClip mint, AudioClip logo)
        {
            sfxPinkMascot = pink;
            sfxMintMascot = mint;
            sfxLogoTitle = logo;
        }

        private void PlayCinematicSfx(AudioClip clip)
        {
            if (clip == null)
            {
                EnsureAudioClips();
            }
            if (clip == null) return;

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayCustomSFX(clip, 1.0f);
                return;
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
            audioSource.spatialBlend = 0f;
            audioSource.PlayOneShot(clip, BlockAudioManager.DEFAULT_SFX_VOLUME);
        }

        private bool _introStarted = false;

        private void Start()
        {
            ResetToPreIntroState();
            EnsureAudioClips();
            UpdateLocalizedPrompt();
            UpdateLogo(LocalizationManager.CurrentLanguage);
            int best = PlayerPrefs.GetInt("BlockBlast_Best", 0);
            if (bestScoreText != null)
            {
                bestScoreText.text = $"최고 점수: {best}";
            }

            // If a splash screen is active, wait for it to complete.
            // Otherwise (or if splash is disabled/missing), start the cinematic intro immediately.
            if (SplashScreenController.Instance != null && SplashScreenController.Instance.IsActive)
            {
                return;
            }

            PlayIntro();
        }

        public void PlayIntro()
        {
            if (_introStarted) return;
            _introStarted = true;

            ResetToPreIntroState();
            EnsureAudioClips();
            _inputAllowedTime = Time.time + 0.4f;

            if (_introCoroutine != null) StopCoroutine(_introCoroutine);
            _introCoroutine = StartCoroutine(CinematicIntroRoutine());

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayIntroBGM();
            }
        }

        private void Update()
        {
            // Do not accept any start input while splash screen is active or before intro is ready
            if (!CanAcceptStartInput()) return;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TriggerStartGame(Vector2.zero);
                return;
            }
#else
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TriggerStartGame(Vector2.zero);
                return;
            }
#endif

#if ENABLE_INPUT_SYSTEM
            bool clicked = false;
            Vector2 pos = Vector2.zero;
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                clicked = true;
                pos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
            else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                clicked = true;
                pos = UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
            }

            if (clicked)
            {
                TriggerStartGame(pos);
            }
#else
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Vector2 pos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                TriggerStartGame(pos);
            }
#endif
        }

        private IEnumerator CinematicIntroRoutine()
        {
            _canTouchToStart = true;
            _hasStarted = false;

            // 1. Initial State Setup:
            // Focus and Zoom in (1.55x) on the Left Mascot area!
            if (cinematicRoot != null)
            {
                cinematicRoot.localScale = Vector3.one * 1.55f;
                cinematicRoot.anchoredPosition = new Vector2(360f, 60f);
            }

            // Mascots and logo start hidden at scale zero right at their final positions
            if (leftMascot != null)
            {
                leftMascot.anchoredPosition = new Vector2(-260f, -80f);
                leftMascot.localScale = Vector3.zero;
            }
            if (rightMascot != null)
            {
                rightMascot.anchoredPosition = new Vector2(260f, -80f);
                rightMascot.localScale = Vector3.zero;
            }
            if (logoTitle != null)
            {
                logoTitle.localScale = Vector3.zero;
            }
            if (touchPromptGroup != null) touchPromptGroup.alpha = 0f;

            yield return new WaitForSeconds(0.2f);

            // 2. Step 1: Left Mascot Dashes In ("슈우웅~ 퐁!") with Sparkle Trails & Fireworks!
            if (leftMascot != null)
            {
                Color pinkTrail = new Color(1.00f, 0.45f, 0.72f);
                yield return StartCoroutine(FlyInMascot(leftMascot, new Vector2(-650f, -480f), new Vector2(-260f, -80f), pinkTrail, isLeft: true, duration: 0.44f));
                yield return StartCoroutine(PunchViewport(new Vector2(0f, 15f), 0.16f));
            }

            yield return new WaitForSeconds(0.35f);

            // 3. Step 2: Camera Pan Right to Right Mascot Spawn Area
            // Smoothly pan viewport from (360, 60) to (-360, 60), centering the right mascot in close-up view!
            if (cinematicRoot != null)
            {
                float panElapsed = 0f;
                float panDur = 0.52f;
                Vector2 fromPos = cinematicRoot.anchoredPosition;
                Vector2 toPos = new Vector2(-360f, 60f);
                while (panElapsed < panDur)
                {
                    panElapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, panElapsed / panDur);
                    cinematicRoot.anchoredPosition = Vector2.Lerp(fromPos, toPos, t);
                    yield return null;
                }
                cinematicRoot.anchoredPosition = toPos;
            }

            // Right Mascot Dashes In ("슈우웅~ 퐁!") with Mint Sparkle Trails & Fireworks!
            if (rightMascot != null)
            {
                Color mintTrail = new Color(0.28f, 0.95f, 0.80f);
                yield return StartCoroutine(FlyInMascot(rightMascot, new Vector2(650f, -480f), new Vector2(260f, -80f), mintTrail, isLeft: false, duration: 0.44f));
                yield return StartCoroutine(PunchViewport(new Vector2(0f, 15f), 0.16f));
            }

            yield return new WaitForSeconds(0.35f);

            // 4. Step 3: Camera Zooms Out to Center
            // Smoothly zoom out viewport from 1.55x to 1.0x and position to (0, 0)!
            float zoomElapsed = 0f;
            float zoomDur = 0.65f;
            Vector2 zoomFromPos = cinematicRoot != null ? cinematicRoot.anchoredPosition : Vector2.zero;
            float zoomFromScale = cinematicRoot != null ? cinematicRoot.localScale.x : 1.55f;

            while (zoomElapsed < zoomDur)
            {
                zoomElapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, zoomElapsed / zoomDur);

                if (cinematicRoot != null)
                {
                    cinematicRoot.anchoredPosition = Vector2.Lerp(zoomFromPos, Vector2.zero, t);
                    float s = Mathf.Lerp(zoomFromScale, 1.0f, t);
                    cinematicRoot.localScale = new Vector3(s, s, 1f);
                }

                yield return null;
            }

            if (cinematicRoot != null)
            {
                cinematicRoot.anchoredPosition = Vector2.zero;
                cinematicRoot.localScale = Vector3.one;
            }

            // 5. Step 4: Center Logo pops in ("뿅!") in the center + Fireworks burst simultaneously from BOTH top-left and top-right!
            if (fireworksEffect != null)
            {
                fireworksEffect.TriggerFireworks(new Vector2(-380f, 680f), 45, 1.35f);
                fireworksEffect.TriggerFireworks(new Vector2(380f, 680f), 45, 1.35f);
            }
            PlayCinematicSfx(sfxLogoTitle);
            if (logoTitle != null)
            {
                yield return StartCoroutine(PopIn(logoTitle, 0.35f));
            }

            yield return new WaitForSeconds(0.3f);

            // 6. Step 5: Start Idle Breathing Animation & 2-Second Periodic Fireworks Loop
            _idleCoroutine = StartCoroutine(IdleBreathingRoutine());
            if (_continuousFireworksCoroutine != null) StopCoroutine(_continuousFireworksCoroutine);
            _continuousFireworksCoroutine = StartCoroutine(ContinuousFireworksRoutine());
        }

        private IEnumerator FlyInMascot(RectTransform mascot, Vector2 fromPos, Vector2 toPos, Color trailColor, bool isLeft, float duration = 0.44f)
        {
            if (mascot == null) yield break;

            mascot.anchoredPosition = fromPos;
            mascot.localScale = Vector3.one * 0.35f;
            mascot.localRotation = Quaternion.identity;

            float elapsed = 0f;
            float trailTimer = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Smooth dash fly-in with playful upward curve
                float easeT = EaseOutCubic(t);
                float arc = Mathf.Sin(t * Mathf.PI) * 95f;
                Vector2 curPos = Vector2.Lerp(fromPos, toPos, easeT) + new Vector2(0f, arc);
                mascot.anchoredPosition = curPos;

                // Scale smoothly up to slightly oversized then settle
                float scale = Mathf.Lerp(0.35f, 1.12f, easeT);
                mascot.localScale = Vector3.one * scale;

                // Playful tilt in flying direction
                float tilt = Mathf.Sin(t * Mathf.PI) * (isLeft ? -22f : 22f);
                mascot.localRotation = Quaternion.Euler(0f, 0f, tilt);

                // Spawn light sparkle star trail along trajectory
                trailTimer += Time.deltaTime;
                if (trailTimer >= 0.045f)
                {
                    trailTimer = 0f;
                    if (fireworksEffect != null)
                    {
                        fireworksEffect.SpawnTrailParticle(curPos, trailColor, 55f);
                    }
                }

                yield return null;
            }

            mascot.anchoredPosition = toPos;
            mascot.localRotation = Quaternion.identity;

            // Touchdown: Jelly Elastic Squash & Stretch bounce + Fireworks pop from top!
            if (fireworksEffect != null)
            {
                Vector2 topBurstPos = isLeft ? new Vector2(-380f, 680f) : new Vector2(380f, 680f);
                fireworksEffect.TriggerFireworks(topBurstPos, 45, 1.35f);
            }
            PlayCinematicSfx(isLeft ? sfxPinkMascot : sfxMintMascot);

            // Squash & stretch elastic landing bounce
            float bounceElapsed = 0f;
            float bounceDuration = 0.32f;
            while (bounceElapsed < bounceDuration)
            {
                bounceElapsed += Time.deltaTime;
                float bt = Mathf.Clamp01(bounceElapsed / bounceDuration);
                float bounce = ElasticOut(bt);
                mascot.localScale = Vector3.one * bounce;
                yield return null;
            }

            mascot.localScale = Vector3.one;
        }

        private float EaseOutCubic(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3f);
        }

        private IEnumerator PopIn(RectTransform rt, float duration = 0.28f)
        {
            if (rt == null) yield break;
            rt.localScale = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float s = ElasticOut(t);
                rt.localScale = Vector3.one * s;
                yield return null;
            }
            rt.localScale = Vector3.one;
        }

        private IEnumerator BounceIn(RectTransform rt, Vector2 from, Vector2 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float bounce = ElasticOut(t);
                rt.anchoredPosition = Vector2.LerpUnclamped(from, to, bounce);
                yield return null;
            }
            rt.anchoredPosition = to;
        }

        private IEnumerator PunchViewport(Vector2 offset, float duration)
        {
            if (cinematicRoot == null) yield break;
            Vector2 origPos = cinematicRoot.anchoredPosition;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float p = Mathf.Sin(t * Mathf.PI);
                cinematicRoot.anchoredPosition = origPos + offset * p;
                yield return null;
            }
            cinematicRoot.anchoredPosition = origPos;
        }

        private float ElasticOut(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return Mathf.Sin(-13f * (t + 1f) * Mathf.PI * 0.5f) * Mathf.Pow(2f, -10f * t) + 1f;
        }

        private IEnumerator IdleBreathingRoutine()
        {
            Vector2 leftBase = leftMascot != null ? leftMascot.anchoredPosition : Vector2.zero;
            Vector2 rightBase = rightMascot != null ? rightMascot.anchoredPosition : Vector2.zero;
            Vector2 rootBase = cinematicRoot != null ? cinematicRoot.anchoredPosition : Vector2.zero;

            while (!_hasStarted)
            {
                float time = Time.time;

                // Gentle camera breathing floating motion (up & down)
                if (cinematicRoot != null)
                {
                    float breathY = Mathf.Sin(time * 2.0f) * 6f;
                    cinematicRoot.anchoredPosition = rootBase + new Vector2(0f, breathY);
                }

                // Mascot subtle floating
                if (leftMascot != null)
                {
                    leftMascot.anchoredPosition = leftBase + new Vector2(0f, Mathf.Sin(time * 2.6f) * 10f);
                }
                if (rightMascot != null)
                {
                    rightMascot.anchoredPosition = rightBase + new Vector2(0f, Mathf.Cos(time * 2.6f) * 10f);
                }

                // "화면을 터치해주세요" Blinking Pulse
                if (touchPromptGroup != null)
                {
                    touchPromptGroup.alpha = 0.35f + 0.65f * Mathf.PingPong(time * 1.5f, 1f);
                }

                yield return null;
            }
        }

        private IEnumerator ContinuousFireworksRoutine()
        {
            // Initial 2-second delay after logo intro burst
            yield return new WaitForSeconds(2.0f);

            while (!_hasStarted)
            {
                if (fireworksEffect != null)
                {
                    // Staggered burst: Left top fireworks then Right top fireworks
                    fireworksEffect.TriggerFireworks(new Vector2(-380f, 680f), 38, 1.3f);
                    yield return new WaitForSeconds(0.2f);
                    if (_hasStarted) yield break;
                    fireworksEffect.TriggerFireworks(new Vector2(380f, 680f), 38, 1.3f);
                }

                yield return new WaitForSeconds(1.8f); // 0.2s + 1.8s = exactly 2.0s period
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanAcceptStartInput()) return;
            TriggerStartGame(eventData.position);
        }

        public void TriggerStartGame(Vector2 screenPos)
        {
            if (!CanAcceptStartInput()) return;
            _hasStarted = true;

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.StopIntroBGM(0.2f);
            }

            // Immediately cancel cinematic intro, idle coroutine, and continuous fireworks on click
            if (_introCoroutine != null) StopCoroutine(_introCoroutine);
            if (_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            if (_continuousFireworksCoroutine != null)
            {
                StopCoroutine(_continuousFireworksCoroutine);
                _continuousFireworksCoroutine = null;
            }

            StartCoroutine(TransitionToGameRoutine(screenPos));
        }

        private IEnumerator TransitionToGameRoutine(Vector2 screenPos)
        {
            // Fireworks at touch position
            if (fireworksEffect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    fireworksEffect.GetComponent<RectTransform>(),
                    screenPos,
                    null,
                    out Vector2 localPos
                );
                fireworksEffect.TriggerFireworks(localPos, 35, 1.4f);
            }

            // Smoothly and quickly reset viewport to center
            Vector2 rootFromPos = cinematicRoot != null ? cinematicRoot.anchoredPosition : Vector2.zero;
            float rootFromScale = cinematicRoot != null ? cinematicRoot.localScale.x : 1f;

            float elapsed = 0f;
            float dur = 0.25f; // Snappy, responsive transition directly into the game

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                if (menuCanvasGroup != null)
                {
                    menuCanvasGroup.alpha = Mathf.Clamp01(1f - t);
                }
                if (cinematicRoot != null)
                {
                    cinematicRoot.anchoredPosition = Vector2.Lerp(rootFromPos, Vector2.zero, t);
                    float s = Mathf.Lerp(rootFromScale, 1.0f, t);
                    cinematicRoot.localScale = new Vector3(s, s, 1f);
                }
                yield return null;
            }

            if (cinematicRoot != null)
            {
                cinematicRoot.anchoredPosition = Vector2.zero;
                cinematicRoot.localScale = Vector3.one;
            }

            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.DoTransition(() =>
                {
                    if (LobbyManager.Instance != null)
                    {
                        LobbyManager.Instance.ShowLobby();
                    }
                    else if (uiManager != null)
                    {
                        uiManager.StartGameFromMenu();
                    }
                    gameObject.SetActive(false);
                });
            }
            else
            {
                if (LobbyManager.Instance != null)
                {
                    LobbyManager.Instance.ShowLobby();
                }
                else if (uiManager != null)
                {
                    uiManager.StartGameFromMenu();
                }
                gameObject.SetActive(false);
            }
        }

        public void SetupReferences(Camera cam, RectTransform root, RectTransform left, RectTransform right, RectTransform logo, CanvasGroup touchGroup, TMP_Text touchText, TMP_Text bestText, JellyFireworksEffect fx, CanvasGroup canvasGroup, BlockBlastUIManager ui)
        {
            mainCamera = cam;
            cinematicRoot = root;
            leftMascot = left;
            rightMascot = right;
            logoTitle = logo;
            touchPromptGroup = touchGroup;
            touchPromptText = touchText;
            bestScoreText = bestText;
            fireworksEffect = fx;
            menuCanvasGroup = canvasGroup;
            uiManager = ui;
            UpdateLocalizedPrompt();
            UpdateLogo(LocalizationManager.CurrentLanguage);
        }
    }
}
