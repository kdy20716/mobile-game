using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    /// <summary>
    /// Ensures that on any PC aspect ratio (16:9, 16:10, 4:3, or tall vertical),
    /// the CanvasScaler automatically adapts so that NO board cells, HUD elements,
    /// or buttons are ever cropped!
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public class AspectRatioAdapter : MonoBehaviour
    {
        public static AspectRatioAdapter Instance { get; private set; }

        private CanvasScaler _scaler;
        private int _lastWidth = -1;
        private int _lastHeight = -1;

        private const float TargetAspect = 1080f / 1920f; // 0.5625f

        private void Awake()
        {
            Instance = this;
            _scaler = GetComponent<CanvasScaler>();
            UpdateScaler();
        }

        private void Update()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                UpdateScaler();
            }
        }

        public void UpdateScaler()
        {
            if (_scaler == null) _scaler = GetComponent<CanvasScaler>();
            if (_scaler == null) return;

            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            if (_lastHeight <= 0) return;

            float currentAspect = (float)_lastWidth / _lastHeight;

            // If screen is wider than 9:16 (e.g. PC widescreen 16:9, 16:10, ultrawide),
            // match HEIGHT (1.0f) so the entire vertical game fits in the center without top/bottom cropping!
            // If screen is taller than 9:16 (e.g. tall phone), match WIDTH (0.0f) so sides don't crop.
            if (currentAspect > TargetAspect)
            {
                _scaler.matchWidthOrHeight = 1.0f;
            }
            else
            {
                _scaler.matchWidthOrHeight = 0.0f;
            }

            if (SideWingsDecorator.Instance != null)
            {
                SideWingsDecorator.Instance.UpdateVisibility();
            }
        }
    }
}
