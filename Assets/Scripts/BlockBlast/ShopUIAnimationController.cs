using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BlockBlast
{
    /// <summary>
    /// Blue Archive / AAA Mobile Style UI Animations for Shop Modal.
    /// Handles:
    /// 1. Modal pop-in with smooth elastic damping
    /// 2. Active tab indicator smooth gliding with squish/stretch
    /// 3. Tab content fade-and-rise transition
    /// 4. Tactile spring bounce on luxury buttons
    /// </summary>
    public class ShopUIAnimationController : MonoBehaviour
    {
        [Header("Modal Animation Targets")]
        [SerializeField] private RectTransform dialogCardRect;
        [SerializeField] private CanvasGroup modalCanvasGroup;
        [SerializeField] private Image darkBgImage;

        [Header("Tab Indicator Glide")]
        [SerializeField] private RectTransform tabIndicatorRect;
        [SerializeField] private RectTransform[] tabButtonRects;

        [Header("Content Panels")]
        [SerializeField] private RectTransform contentContainerRect;

        private Coroutine _modalAnimCoroutine;
        private Coroutine _indicatorGlideCoroutine;
        private Coroutine _panelTransitionCoroutine;

        private Vector3 _originalCardScale = Vector3.one;
        private int _currentTabIndex = 0;

        private void Awake()
        {
            if (dialogCardRect != null)
            {
                _originalCardScale = dialogCardRect.localScale;
            }
            if (modalCanvasGroup == null && dialogCardRect != null)
            {
                modalCanvasGroup = dialogCardRect.GetComponent<CanvasGroup>();
                if (modalCanvasGroup == null) modalCanvasGroup = dialogCardRect.gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void SetupReferences(
            RectTransform cardRect,
            CanvasGroup cg,
            Image darkBg,
            RectTransform indicatorRect,
            RectTransform[] tabRects,
            RectTransform contentRect)
        {
            dialogCardRect = cardRect;
            modalCanvasGroup = cg;
            darkBgImage = darkBg;
            tabIndicatorRect = indicatorRect;
            tabButtonRects = tabRects;
            contentContainerRect = contentRect;

            if (dialogCardRect != null) _originalCardScale = Vector3.one;
        }

        // ==========================================
        // 1. MODAL OPEN / CLOSE ANIMATIONS
        // ==========================================

        public void AnimateOpen()
        {
            if (!gameObject.activeInHierarchy) return;
            if (_modalAnimCoroutine != null) StopCoroutine(_modalAnimCoroutine);
            _modalAnimCoroutine = StartCoroutine(CoAnimateOpen());
        }

        private IEnumerator CoAnimateOpen()
        {
            float duration = 0.28f;
            float elapsed = 0f;

            if (dialogCardRect != null) dialogCardRect.localScale = _originalCardScale * 0.92f;
            if (modalCanvasGroup != null) modalCanvasGroup.alpha = 0f;
            if (darkBgImage != null)
            {
                Color c = darkBgImage.color;
                c.a = 0f;
                darkBgImage.color = c;
            }

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Ease Out Back curve for luxury spring pop
                float s = 1.70158f;
                float tNorm = t - 1f;
                float easeBack = (tNorm * tNorm * ((s + 1f) * tNorm + s) + 1f);

                if (dialogCardRect != null)
                {
                    dialogCardRect.localScale = Vector3.LerpUnclamped(_originalCardScale * 0.92f, _originalCardScale, easeBack);
                }

                if (modalCanvasGroup != null)
                {
                    modalCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t * 1.5f);
                }

                if (darkBgImage != null)
                {
                    Color c = darkBgImage.color;
                    c.a = Mathf.Lerp(0f, 0.88f, t);
                    darkBgImage.color = c;
                }

                yield return null;
            }

            if (dialogCardRect != null) dialogCardRect.localScale = _originalCardScale;
            if (modalCanvasGroup != null) modalCanvasGroup.alpha = 1f;
            if (darkBgImage != null)
            {
                Color c = darkBgImage.color;
                c.a = 0.88f;
                darkBgImage.color = c;
            }

            _modalAnimCoroutine = null;
        }

        public void AnimateClose(Action onComplete)
        {
            if (!gameObject.activeInHierarchy)
            {
                onComplete?.Invoke();
                return;
            }
            if (_modalAnimCoroutine != null) StopCoroutine(_modalAnimCoroutine);
            _modalAnimCoroutine = StartCoroutine(CoAnimateClose(onComplete));
        }

        private IEnumerator CoAnimateClose(Action onComplete)
        {
            float duration = 0.16f;
            float elapsed = 0f;

            Vector3 startScale = dialogCardRect != null ? dialogCardRect.localScale : _originalCardScale;
            float startAlpha = modalCanvasGroup != null ? modalCanvasGroup.alpha : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                if (dialogCardRect != null)
                {
                    dialogCardRect.localScale = Vector3.Lerp(startScale, _originalCardScale * 0.94f, t);
                }

                if (modalCanvasGroup != null)
                {
                    modalCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                }

                if (darkBgImage != null)
                {
                    Color c = darkBgImage.color;
                    c.a = Mathf.Lerp(0.88f, 0f, t);
                    darkBgImage.color = c;
                }

                yield return null;
            }

            if (modalCanvasGroup != null) modalCanvasGroup.alpha = 0f;
            _modalAnimCoroutine = null;
            onComplete?.Invoke();
        }

        // ==========================================
        // 2. TAB INDICATOR GLIDE ANIMATION
        // ==========================================

        public void SnapTabIndicator(int tabIdx)
        {
            _currentTabIndex = tabIdx;
            if (tabIndicatorRect != null && tabButtonRects != null && tabIdx >= 0 && tabIdx < tabButtonRects.Length)
            {
                if (tabButtonRects[tabIdx] != null)
                {
                    Vector2 pos = tabIndicatorRect.anchoredPosition;
                    pos.x = tabButtonRects[tabIdx].anchoredPosition.x;
                    tabIndicatorRect.anchoredPosition = pos;
                    tabIndicatorRect.localScale = Vector3.one;
                }
            }
        }

        public void SnapTab(int tabIdx, RectTransform activePanelRect)
        {
            SnapTabIndicator(tabIdx);

            if (_panelTransitionCoroutine != null)
            {
                StopCoroutine(_panelTransitionCoroutine);
                _panelTransitionCoroutine = null;
            }
            if (_indicatorGlideCoroutine != null)
            {
                StopCoroutine(_indicatorGlideCoroutine);
                _indicatorGlideCoroutine = null;
            }

            if (activePanelRect != null)
            {
                activePanelRect.anchoredPosition = Vector2.zero;
                CanvasGroup cg = activePanelRect.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 1f;
            }
        }

        public void AnimateTabGlide(int targetIdx, RectTransform activePanelRect)
        {
            if (!gameObject.activeInHierarchy)
            {
                SnapTabIndicator(targetIdx);
                return;
            }
            if (tabIndicatorRect == null || tabButtonRects == null) return;
            if (targetIdx < 0 || targetIdx >= tabButtonRects.Length) return;
            if (tabButtonRects[targetIdx] == null) return;

            float targetX = tabButtonRects[targetIdx].anchoredPosition.x;
            _currentTabIndex = targetIdx;

            if (_indicatorGlideCoroutine != null) StopCoroutine(_indicatorGlideCoroutine);
            _indicatorGlideCoroutine = StartCoroutine(CoGlideIndicator(targetX));

            if (activePanelRect != null && activePanelRect.gameObject.activeInHierarchy)
            {
                if (_panelTransitionCoroutine != null) StopCoroutine(_panelTransitionCoroutine);
                _panelTransitionCoroutine = StartCoroutine(CoAnimatePanel(activePanelRect));
            }
        }

        private IEnumerator CoGlideIndicator(float targetX)
        {
            float duration = 0.22f;
            float elapsed = 0f;
            float startX = tabIndicatorRect.anchoredPosition.x;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Smooth cubic ease out
                float ease = 1f - Mathf.Pow(1f - t, 3f);

                Vector2 pos = tabIndicatorRect.anchoredPosition;
                pos.x = Mathf.Lerp(startX, targetX, ease);
                tabIndicatorRect.anchoredPosition = pos;

                // Subtle squash & stretch during fast travel
                float stretch = Mathf.Sin(t * Mathf.PI) * 0.10f;
                tabIndicatorRect.localScale = new Vector3(1f + stretch, 1f - stretch * 0.5f, 1f);

                yield return null;
            }

            Vector2 finalPos = tabIndicatorRect.anchoredPosition;
            finalPos.x = targetX;
            tabIndicatorRect.anchoredPosition = finalPos;
            tabIndicatorRect.localScale = Vector3.one;
            _indicatorGlideCoroutine = null;
        }

        private IEnumerator CoAnimatePanel(RectTransform panelRect)
        {
            CanvasGroup cg = panelRect.GetComponent<CanvasGroup>();
            if (cg == null) cg = panelRect.gameObject.AddComponent<CanvasGroup>();

            float duration = 0.18f;
            float elapsed = 0f;
            Vector2 origPos = panelRect.anchoredPosition;
            Vector2 startPos = origPos + new Vector2(0f, -14f);

            panelRect.anchoredPosition = startPos;
            cg.alpha = 0.2f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float ease = 1f - Mathf.Pow(1f - t, 2f);

                panelRect.anchoredPosition = Vector2.Lerp(startPos, origPos, ease);
                cg.alpha = Mathf.Lerp(0.2f, 1f, ease);

                yield return null;
            }

            panelRect.anchoredPosition = origPos;
            cg.alpha = 1f;
            _panelTransitionCoroutine = null;
        }

        // ==========================================
        // 3. TACTILE SPRING BUTTON BOUNCE
        // ==========================================

        public static void AttachTactileBounce(Button button)
        {
            if (button == null) return;
            var trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

            RectTransform targetRt = button.GetComponent<RectTransform>();

            // PointerDown: Squash slightly
            EventTrigger.Entry entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entryDown.callback.AddListener((_) =>
            {
                if (targetRt != null) targetRt.localScale = new Vector3(0.93f, 0.93f, 1f);
            });
            trigger.triggers.Add(entryDown);

            // PointerUp: Spring recover
            EventTrigger.Entry entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entryUp.callback.AddListener((_) =>
            {
                if (targetRt != null)
                {
                    var runner = button.GetComponent<MonoBehaviour>();
                    if (runner != null && runner.gameObject.activeInHierarchy)
                    {
                        runner.StartCoroutine(CoSpringButton(targetRt));
                    }
                    else
                    {
                        targetRt.localScale = Vector3.one;
                    }
                }
            });
            trigger.triggers.Add(entryUp);
        }

        private static IEnumerator CoSpringButton(RectTransform rt)
        {
            float duration = 0.18f;
            float elapsed = 0f;
            Vector3 startScale = rt.localScale;
            Vector3 targetScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Slight overshoot
                float bounce = 1f + Mathf.Sin(t * Mathf.PI) * 0.05f;
                rt.localScale = Vector3.Lerp(startScale, targetScale, t) * bounce;

                yield return null;
            }

            rt.localScale = Vector3.one;
        }
    }
}
