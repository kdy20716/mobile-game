using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockBlast
{
    /// <summary>
    /// Tactile pudding / jelly physics for mascot character images.
    /// Short tap: fluid ripple bounce shockwave (톡 누르면 물결/푸딩 탄성 진동).
    /// Long press: pinch deformation (누른 부위는 찌그러지고 반대편은 볼록하게 늘어남).
    /// Drag while holding: follows pointer with dynamic jelly wobble (마우스 움직임에 따라 찰랑찰랑 젤리 워블).
    /// Release: damped spring snap-back relaxation.
    /// </summary>
    public class PuddingJellyTouchPhysics : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
    {
        private RectTransform _rectTransform;
        private Vector3 _originalScale = Vector3.one;
        private Vector2 _originalPosition;

        private bool _isPressed = false;
        private float _pressTime = 0f;
        private Vector2 _pressParentPos;
        private Vector2 _currentDragParentPos;
        private Vector2 _initialAnchoredPos;

        [Header("Physics Tuning")]
        public float shortClickThreshold = 0.20f;
        public Vector2 clampOffset = new Vector2(240f, 110f);

        private Coroutine _wobbleCoroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform != null)
            {
                _originalScale = _rectTransform.localScale;
                _originalPosition = _rectTransform.anchoredPosition;
            }
        }

        private void OnEnable()
        {
            ResetToRested();
        }

        public void SetOriginalAnchor(Vector2 pos)
        {
            _originalPosition = pos;
            if (_rectTransform != null) _rectTransform.anchoredPosition = pos;
        }

        public void CaptureCurrentAsOriginal()
        {
            if (_rectTransform != null)
            {
                _originalPosition = _rectTransform.anchoredPosition;
                _originalScale = _rectTransform.localScale;
            }
        }

        public void ResetToRested()
        {
            if (_rectTransform != null)
            {
                _rectTransform.localScale = _originalScale;
                _rectTransform.anchoredPosition = _originalPosition;
                _rectTransform.localRotation = Quaternion.identity;
            }
            _isPressed = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_wobbleCoroutine != null) StopCoroutine(_wobbleCoroutine);
            _isPressed = true;
            _pressTime = Time.unscaledTime;

            if (_rectTransform != null && _rectTransform.parent is RectTransform parentRT)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, eventData.position, eventData.pressEventCamera, out _pressParentPos);
                _initialAnchoredPos = _rectTransform.anchoredPosition;
                _currentDragParentPos = _pressParentPos;
            }

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isPressed) return;

            if (_rectTransform != null && _rectTransform.parent is RectTransform parentRT)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, eventData.position, eventData.pressEventCamera, out _currentDragParentPos);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float duration = Time.unscaledTime - _pressTime;
            _isPressed = false;

            if (duration < shortClickThreshold && !eventData.dragging)
            {
                TriggerPuddingPoke();
            }
            else
            {
                TriggerReleaseSpringWobble();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Handled in OnPointerUp for accurate duration measurement
        }

        private void Update()
        {
            if (_rectTransform == null) return;

            if (_isPressed)
            {
                // Displacement from click start in parent space
                Vector2 dragOffset = _currentDragParentPos - _pressParentPos;
                float distX = Mathf.Abs(dragOffset.x);
                float distY = Mathf.Abs(dragOffset.y);
                float totalDist = distX + distY;

                // Follow mouse clamped inside the window
                Vector2 targetPos = _initialAnchoredPos + dragOffset;
                targetPos.x = Mathf.Clamp(targetPos.x, _originalPosition.x - clampOffset.x, _originalPosition.x + clampOffset.x);
                targetPos.y = Mathf.Clamp(targetPos.y, _originalPosition.y - clampOffset.y, _originalPosition.y + clampOffset.y);

                _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, targetPos, Time.unscaledDeltaTime * 26f);

                // Normalized deformation intensity (up to 120px)
                float dragIntensity = Mathf.Clamp01(totalDist / 120f);
                float maxCompress = 0.35f;

                // Axis distribution: ratioX is 1.0 when purely horizontal, 0.0 when purely vertical
                float ratioX = totalDist > 0.001f ? (distX / totalDist) : 0.5f;
                float ratioY = 1f - ratioX;

                // User request: 움직인 부분은 줄어들고 다른 부위는 늘어남 (회전 없이 부위만 변형)
                // Dragging along X: X compresses (줄어듦), Y expands (늘어남)
                // Dragging along Y: Y compresses (줄어듦), X expands (늘어남)
                float axisDelta = (ratioX - ratioY) * dragIntensity * maxCompress;
                float scaleXFactor = 1f - axisDelta;
                float scaleYFactor = 1f + axisDelta;

                // Gentle static hold squish if barely dragged
                if (totalDist < 10f)
                {
                    float holdDuration = Time.unscaledTime - _pressTime;
                    float holdT = Mathf.Clamp01(holdDuration / 0.12f);
                    scaleXFactor = 1f + 0.08f * holdT;
                    scaleYFactor = 1f - 0.08f * holdT;
                }

                Vector3 targetScale = new Vector3(
                    _originalScale.x * scaleXFactor,
                    _originalScale.y * scaleYFactor,
                    _originalScale.z
                );

                // Zero rotation at all times
                _rectTransform.localRotation = Quaternion.identity;
                _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, targetScale, Time.unscaledDeltaTime * 22f);
            }
        }

        public void TriggerPuddingPoke()
        {
            if (_wobbleCoroutine != null) StopCoroutine(_wobbleCoroutine);
            _wobbleCoroutine = StartCoroutine(PuddingPokeRoutine());
        }

        private IEnumerator PuddingPokeRoutine()
        {
            float elapsed = 0f;
            float duration = 0.55f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Damped sine ripple (톡 건드렸을 때)
                float decay = Mathf.Exp(-t * 6.0f);
                float wave = Mathf.Sin(t * 26f) * decay;

                float scaleX = _originalScale.x * (1f + wave * 0.28f);
                float scaleY = _originalScale.y * (1f - wave * 0.28f);

                _rectTransform.localScale = new Vector3(scaleX, scaleY, _originalScale.z);
                _rectTransform.anchoredPosition = _originalPosition;
                _rectTransform.localRotation = Quaternion.identity;

                yield return null;
            }

            _rectTransform.localScale = _originalScale;
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.localRotation = Quaternion.identity;
        }

        private void TriggerReleaseSpringWobble()
        {
            if (_wobbleCoroutine != null) StopCoroutine(_wobbleCoroutine);
            _wobbleCoroutine = StartCoroutine(ReleaseSpringRoutine());
        }

        private IEnumerator ReleaseSpringRoutine()
        {
            float elapsed = 0f;
            float duration = 0.55f;
            Vector2 startPos = _rectTransform.anchoredPosition;
            Vector3 startScale = _rectTransform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Smooth elastic spring return to original position & original scale (원상복구 & 제자리 복귀)
                float decay = Mathf.Exp(-t * 6.2f);
                float spring = Mathf.Cos(t * 20f) * decay;

                Vector2 posOffset = (startPos - _originalPosition) * spring;
                _rectTransform.anchoredPosition = _originalPosition + posOffset;

                float currDiffX = (startScale.x - _originalScale.x) * spring;
                float currDiffY = (startScale.y - _originalScale.y) * spring;
                _rectTransform.localScale = new Vector3(
                    _originalScale.x + currDiffX,
                    _originalScale.y + currDiffY,
                    _originalScale.z
                );
                _rectTransform.localRotation = Quaternion.identity;

                yield return null;
            }

            _rectTransform.localScale = _originalScale;
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.localRotation = Quaternion.identity;
        }
    }
}
