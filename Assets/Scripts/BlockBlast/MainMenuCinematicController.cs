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

        [Header("Effects & Managers")]
        [SerializeField] private JellyFireworksEffect fireworksEffect;
        [SerializeField] private BlockBlastUIManager uiManager;

        private bool _canTouchToStart = true;
        private bool _hasStarted = false;
        private Coroutine _introCoroutine;
        private Coroutine _idleCoroutine;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            if (menuCanvasGroup == null)
            {
                menuCanvasGroup = GetComponent<CanvasGroup>();
                if (menuCanvasGroup == null) menuCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            int best = PlayerPrefs.GetInt("BlockBlast_Best", 0);
            if (bestScoreText != null)
            {
                bestScoreText.text = $"최고 점수: {best}";
            }

            _introCoroutine = StartCoroutine(CinematicIntroRoutine());
        }

        private void Update()
        {
            // Allow instant tap/click anywhere at any time (including during intro) to start the game immediately
            if (_hasStarted) return;

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

            yield return new WaitForSeconds(0.25f);

            // 2. Step 1: Left Mascot Pop ("뿅!") suddenly + Fireworks at Top-Left Corner!
            if (fireworksEffect != null)
            {
                fireworksEffect.TriggerFireworks(new Vector2(-380f, 680f), 35, 1.35f);
            }
            if (leftMascot != null)
            {
                yield return StartCoroutine(PopIn(leftMascot, 0.28f));
                yield return StartCoroutine(PunchViewport(new Vector2(0f, 15f), 0.16f));
            }

            yield return new WaitForSeconds(0.45f);

            // 3. Step 2: Camera Pan Right to Right Mascot Spawn Area
            // Smoothly pan viewport from (360, 60) to (-360, 60), centering the right mascot in close-up view!
            if (cinematicRoot != null)
            {
                float panElapsed = 0f;
                float panDur = 0.55f;
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

            // Right Mascot Pop ("뿅!") suddenly + Fireworks at Top-Right Corner!
            if (fireworksEffect != null)
            {
                fireworksEffect.TriggerFireworks(new Vector2(380f, 680f), 35, 1.35f);
            }
            if (rightMascot != null)
            {
                yield return StartCoroutine(PopIn(rightMascot, 0.28f));
                yield return StartCoroutine(PunchViewport(new Vector2(0f, 15f), 0.16f));
            }

            yield return new WaitForSeconds(0.45f);

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
                fireworksEffect.TriggerFireworks(new Vector2(-380f, 680f), 38, 1.35f);
                fireworksEffect.TriggerFireworks(new Vector2(380f, 680f), 38, 1.35f);
            }
            if (logoTitle != null)
            {
                yield return StartCoroutine(PopIn(logoTitle, 0.35f));
            }

            yield return new WaitForSeconds(0.3f);

            // 5. Step 4: Start Idle Breathing Animation
            _idleCoroutine = StartCoroutine(IdleBreathingRoutine());
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

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerStartGame(eventData.position);
        }

        public void TriggerStartGame(Vector2 screenPos)
        {
            if (_hasStarted) return;
            _hasStarted = true;

            // Immediately cancel cinematic intro or idle coroutine on click
            if (_introCoroutine != null) StopCoroutine(_introCoroutine);
            if (_idleCoroutine != null) StopCoroutine(_idleCoroutine);

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
        }
    }
}
