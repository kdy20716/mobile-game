using UnityEngine;

namespace BlockBlast
{
    /// <summary>
    /// Automatically adjusts RectTransform anchors based on Screen.safeArea,
    /// protecting HUD and UI elements from top notches, camera punch-holes,
    /// rounded device corners, and bottom home gesture indicators on Android & iOS.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class SafeAreaFitter : MonoBehaviour
    {
        [Tooltip("If true, safe area padding is applied to horizontal margins.")]
        [SerializeField] private bool applyHorizontal = true;

        [Tooltip("If true, safe area padding is applied to vertical margins (top notch & bottom gesture bar).")]
        [SerializeField] private bool applyVertical = true;

        private RectTransform _rectTransform;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new Vector2Int(0, 0);
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea ||
                _lastScreenSize.x != Screen.width ||
                _lastScreenSize.y != Screen.height ||
                _lastOrientation != Screen.orientation)
            {
                ApplySafeArea();
            }
        }

        public void ApplySafeArea()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null) return;

            Rect safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;

            if (Screen.width <= 0 || Screen.height <= 0) return;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (!applyHorizontal)
            {
                anchorMin.x = 0f;
                anchorMax.x = 1f;
            }

            if (!applyVertical)
            {
                anchorMin.y = 0f;
                anchorMax.y = 1f;
            }

            // Only apply if values are valid normalized range [0, 1]
            if (anchorMin.x >= 0f && anchorMin.y >= 0f && anchorMax.x <= 1f && anchorMax.y <= 1f)
            {
                _rectTransform.anchorMin = anchorMin;
                _rectTransform.anchorMax = anchorMax;
                _rectTransform.offsetMin = Vector2.zero;
                _rectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}
