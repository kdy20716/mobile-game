using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    /// <summary>
    /// Manages the layered pastel side background panels (Left Wing and Right Wing).
    /// Automatically reveals and animates the dreamy pastel side wings when the screen
    /// aspect ratio widens beyond 9:16 (PC 16:9, 16:10, 4:3, or Fullscreen),
    /// keeping the vertical mobile puzzle game beautifully framed!
    /// </summary>
    public class SideWingsDecorator : MonoBehaviour
    {
        public static SideWingsDecorator Instance { get; private set; }

        [Header("Wing Containers")]
        [SerializeField] private GameObject leftWing;
        [SerializeField] private GameObject rightWing;
        [SerializeField] private GameObject leftDivider;
        [SerializeField] private GameObject rightDivider;

        [Header("Wing Images")]
        [SerializeField] private Image leftWingSkyImg;
        [SerializeField] private Image rightWingSkyImg;
        [SerializeField] private Image leftWingArtImg;
        [SerializeField] private Image rightWingArtImg;

        [Header("Floating Decorations & Sparkles")]
        [SerializeField] private RectTransform[] floatingDecorations;
        [SerializeField] private Image[] twinkleStars;

        private Vector2[] _origPositions;
        private float[] _floatSpeeds;
        private float[] _floatAmplitudes;
        private float[] _twinklePhases;

        private const float TargetAspect = 1080f / 1920f; // 0.5625f
        private int _lastWidth = -1;
        private int _lastHeight = -1;
        private bool _isWide = false;

        private void Awake()
        {
            Instance = this;
            InitDecorations();
            UpdateVisibility();
        }

        public void SetupReferences(
            GameObject lWing, GameObject rWing,
            GameObject lDiv, GameObject rDiv,
            Image lSky, Image rSky,
            Image lArt, Image rArt,
            RectTransform[] floatDecos,
            Image[] stars)
        {
            leftWing = lWing;
            rightWing = rWing;
            leftDivider = lDiv;
            rightDivider = rDiv;
            leftWingSkyImg = lSky;
            rightWingSkyImg = rSky;
            leftWingArtImg = lArt;
            rightWingArtImg = rArt;
            floatingDecorations = floatDecos;
            twinkleStars = stars;

            InitDecorations();
            UpdateVisibility();
        }

        private void InitDecorations()
        {
            if (floatingDecorations != null && floatingDecorations.Length > 0)
            {
                _origPositions = new Vector2[floatingDecorations.Length];
                _floatSpeeds = new float[floatingDecorations.Length];
                _floatAmplitudes = new float[floatingDecorations.Length];

                for (int i = 0; i < floatingDecorations.Length; i++)
                {
                    if (floatingDecorations[i] != null)
                    {
                        _origPositions[i] = floatingDecorations[i].anchoredPosition;
                        _floatSpeeds[i] = 1.0f + (i * 0.35f) % 1.5f;
                        _floatAmplitudes[i] = 8f + (i * 2.5f) % 10f;
                    }
                }
            }

            if (twinkleStars != null && twinkleStars.Length > 0)
            {
                _twinklePhases = new float[twinkleStars.Length];
                for (int i = 0; i < twinkleStars.Length; i++)
                {
                    _twinklePhases[i] = (i * 1.35f) % (Mathf.PI * 2f);
                }
            }
        }

        private void Update()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                UpdateVisibility();
            }

            if (!_isWide) return;

            float time = Time.time;

            // Gentle floating bob for cloud layers & hanging lanterns
            if (floatingDecorations != null && _origPositions != null)
            {
                for (int i = 0; i < floatingDecorations.Length; i++)
                {
                    if (floatingDecorations[i] != null && i < _origPositions.Length)
                    {
                        float yOff = Mathf.Sin(time * _floatSpeeds[i] + i * 0.8f) * _floatAmplitudes[i];
                        floatingDecorations[i].anchoredPosition = _origPositions[i] + new Vector2(0f, yOff);
                    }
                }
            }

            // Twinkling starlight
            if (twinkleStars != null && _twinklePhases != null)
            {
                for (int i = 0; i < twinkleStars.Length; i++)
                {
                    if (twinkleStars[i] != null && i < _twinklePhases.Length)
                    {
                        float alpha = 0.45f + 0.55f * Mathf.Abs(Mathf.Sin(time * 2.0f + _twinklePhases[i]));
                        Color c = twinkleStars[i].color;
                        c.a = alpha;
                        twinkleStars[i].color = c;
                    }
                }
            }
        }

        public void UpdateVisibility()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            if (_lastHeight <= 0) return;

            float currentAspect = (float)_lastWidth / _lastHeight;
            _isWide = currentAspect > (TargetAspect + 0.005f);

            if (leftWing != null && leftWing.activeSelf != _isWide) leftWing.SetActive(_isWide);
            if (rightWing != null && rightWing.activeSelf != _isWide) rightWing.SetActive(_isWide);
            if (leftDivider != null && leftDivider.activeSelf != _isWide) leftDivider.SetActive(_isWide);
            if (rightDivider != null && rightDivider.activeSelf != _isWide) rightDivider.SetActive(_isWide);
        }

        /// <summary>
        /// Harmonizes the side wing pastel ambient tints when theme changes in the shop.
        /// </summary>
        public void SyncWithTheme(int themeIndex)
        {
            Color skyTint = Color.white;
            switch (themeIndex)
            {
                case 1: // Candy Wonderland (Sweet Strawberry & Peach Pastel)
                    skyTint = new Color(1.0f, 0.94f, 0.97f, 1f);
                    break;
                case 2: // Crystal Mermaid (Dreamy Pastel Ocean & Mint)
                    skyTint = new Color(0.92f, 0.98f, 1.0f, 1f);
                    break;
                case 3: // Starry Nebula (Deep Cosmic Violet & Starlight)
                    skyTint = new Color(0.95f, 0.92f, 1.0f, 1f);
                    break;
                default: // Sweet Night (Default Dreamy Lavender)
                    skyTint = Color.white;
                    break;
            }

            if (leftWingArtImg != null) leftWingArtImg.color = skyTint;
            if (rightWingArtImg != null) rightWingArtImg.color = skyTint;
        }
    }
}
