using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    /// <summary>
    /// Game Boot Splash Screen Controller (Mallang Games Studio Logo).
    /// Plays an adorable jelly bounce animation on startup.
    /// Can be easily enabled/disabled via 'enableSplashScreen' toggle in inspector.
    /// Any click/touch/ESC immediately skips to the title cinematic!
    /// </summary>
    public class SplashScreenController : MonoBehaviour
    {
        public static SplashScreenController Instance { get; private set; }

        [Header("Master Switch (마음에 안 들면 언제든 체크 해제!)")]
        [Tooltip("체크를 끄면 스플래시 화면을 거치지 않고 바로 타이틀 화면으로 진입합니다.")]
        [SerializeField] private bool enableSplashScreen = true;

        [Header("Skip Settings")]
        [Tooltip("스플래시 화면 중 클릭으로 스킵 허용 여부 (기본 false: 스플래시 중 클릭해도 스킵되거나 로비로 넘어가지 않습니다)")]
        [SerializeField] private bool allowClickToSkip = false;

        [Header("UI References")]
        [SerializeField] private CanvasGroup splashCanvasGroup;
        [SerializeField] private RectTransform logoTransform;
        [SerializeField] private Image logoImage;
        [SerializeField] private Image bgImage;

        [Header("Animation Timings")]
        [SerializeField] private float logoHoldDuration = 1.4f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Audio")]
        [SerializeField] private AudioClip splashSfx;
        [SerializeField] private AudioSource audioSource;

        [Header("Optional Links")]
        [SerializeField] private MainMenuCinematicController cinematicController;

        public void SetupReferences(CanvasGroup cg, RectTransform logoRt, Image logo, Image bg, MainMenuCinematicController ctrl)
        {
            splashCanvasGroup = cg;
            logoTransform = logoRt;
            logoImage = logo;
            bgImage = bg;
            cinematicController = ctrl;
        }

        public void SetupAudio(AudioClip clip)
        {
            splashSfx = clip;
        }

        private bool _isFinished = false;
        private bool _isSkipping = false;
        private Coroutine _splashCoroutine;

        public bool IsActive => enableSplashScreen && !_isFinished && gameObject.activeInHierarchy;

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

            if (splashCanvasGroup == null)
            {
                splashCanvasGroup = GetComponent<CanvasGroup>();
                if (splashCanvasGroup == null) splashCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (!enableSplashScreen)
            {
                FinishSplashImmediate();
            }
        }

        private void Start()
        {
            if (!enableSplashScreen)
            {
                FinishSplashImmediate();
                return;
            }

            if (cinematicController == null)
            {
                cinematicController = FindFirstObjectByType<MainMenuCinematicController>();
            }
            if (cinematicController != null)
            {
                cinematicController.ResetToPreIntroState();
            }

            // Guarantee complete silence during splash screen until transition
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.StopBGM(0f);
            }

            _splashCoroutine = StartCoroutine(PlaySplashRoutine());
        }

        private void Update()
        {
            if (_isFinished || _isSkipping) return;

            // Allow instant tap/click/keyboard skip
            if (DetectUserInput())
            {
                SkipSplash();
            }
        }

        private bool DetectUserInput()
        {
            if (!allowClickToSkip) return false;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) return true;
            if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
            if (UnityEngine.InputSystem.Keyboard.current != null && (
                UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame ||
                UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame ||
                UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)) return true;
#else
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) return true;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) return true;
#endif
            return false;
        }

        public void SkipSplash()
        {
            if (_isFinished || _isSkipping) return;
            _isSkipping = true;

            if (_splashCoroutine != null)
            {
                StopCoroutine(_splashCoroutine);
            }
            StartCoroutine(FastFadeOutRoutine());
        }

        private IEnumerator PlaySplashRoutine()
        {
            // Initial state: fully opaque white/pastel background, logo small and transparent
            if (splashCanvasGroup != null)
            {
                splashCanvasGroup.alpha = 1f;
                splashCanvasGroup.blocksRaycasts = true;
                splashCanvasGroup.interactable = true;
            }

            if (logoTransform != null)
            {
                logoTransform.localScale = new Vector3(0.45f, 0.45f, 1f);
                logoTransform.anchoredPosition = new Vector2(0f, 30f);
            }

            Color logoInitialColor = logoImage != null ? logoImage.color : Color.white;
            if (logoImage != null)
            {
                logoImage.color = new Color(logoInitialColor.r, logoInitialColor.g, logoInitialColor.b, 0f);
            }

            yield return new WaitForSeconds(0.12f);

            // Play item_acquired_06 or pop sound
            if (splashSfx == null)
            {
                splashSfx = Resources.Load<AudioClip>("Audio/item_acquired_06");
            }

            if (splashSfx != null)
            {
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayCustomSFX(splashSfx, 1.0f);
                }
                else
                {
                    if (audioSource == null)
                    {
                        audioSource = gameObject.AddComponent<AudioSource>();
                        audioSource.playOnAwake = false;
                    }
                    audioSource.spatialBlend = 0f;
                    audioSource.PlayOneShot(splashSfx, BlockAudioManager.DEFAULT_SFX_VOLUME);
                }
            }
            else if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayClick();
            }

            // Phase 1: Jelly Bounce In (Squash & Stretch)
            float bounceDur = 0.55f;
            float elapsed = 0f;
            Vector2 startPos = new Vector2(0f, 35f);
            Vector2 targetPos = Vector2.zero;

            while (elapsed < bounceDur)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / bounceDur);

                // Alpha fade in
                if (logoImage != null)
                {
                    float a = Mathf.Clamp01(t * 2.2f);
                    logoImage.color = new Color(logoInitialColor.r, logoInitialColor.g, logoInitialColor.b, a);
                }

                // Elastic jelly bounce curve
                float scaleProgress = EaseOutBack(t);
                if (logoTransform != null)
                {
                    logoTransform.localScale = Vector3.one * Mathf.LerpUnclamped(0.45f, 1f, scaleProgress);
                    logoTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                }

                yield return null;
            }

            if (logoTransform != null)
            {
                logoTransform.localScale = Vector3.one;
                logoTransform.anchoredPosition = Vector2.zero;
            }
            if (logoImage != null)
            {
                logoImage.color = new Color(logoInitialColor.r, logoInitialColor.g, logoInitialColor.b, 1f);
            }

            // Phase 2: Gentle breathing pulse while holding
            float holdElapsed = 0f;
            while (holdElapsed < logoHoldDuration)
            {
                holdElapsed += Time.deltaTime;
                float pulse = 1f + Mathf.Sin(holdElapsed * 5f) * 0.025f;
                if (logoTransform != null)
                {
                    logoTransform.localScale = new Vector3(pulse, 2f - pulse, 1f); // subtle jelly squash
                }
                yield return null;
            }

            // Phase 3: Smooth Cross-fade Out to Main Menu
            yield return StartCoroutine(FadeOutAndFinish(fadeOutDuration));
        }

        private IEnumerator FastFadeOutRoutine()
        {
            yield return StartCoroutine(FadeOutAndFinish(0.18f));
        }

        private IEnumerator FadeOutAndFinish(float duration)
        {
            // Start the cinematic intro 0.5s earlier right as fade-out begins!
            if (cinematicController != null)
            {
                cinematicController.PlayIntro();
            }

            float elapsed = 0f;
            float startAlpha = splashCanvasGroup != null ? splashCanvasGroup.alpha : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                if (splashCanvasGroup != null)
                {
                    splashCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                }
                yield return null;
            }

            FinishSplashImmediate();
        }

        private void FinishSplashImmediate()
        {
            _isFinished = true;
            if (splashCanvasGroup != null)
            {
                splashCanvasGroup.alpha = 0f;
                splashCanvasGroup.blocksRaycasts = false;
                splashCanvasGroup.interactable = false;
            }
            gameObject.SetActive(false);

            // Notify Cinematic Controller to begin intro
            if (cinematicController != null)
            {
                cinematicController.PlayIntro();
            }
        }

        private float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}
