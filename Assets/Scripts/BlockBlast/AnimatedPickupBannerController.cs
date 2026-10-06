using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BlockBlast
{
    /// <summary>
    /// Controls the dynamic live animation for the Shop's Pickup Gacha Banner.
    /// Features:
    /// 1. Floating & breathing Special Mascot live cutout.
    /// 2. Continuously rotating golden celestial sunburst rays & pulsing prismatic aura.
    /// 3. Shooting stars streaking diagonally across the cosmic sky with luminous glowing trails.
    /// 4. Drifting, twinkling fairy sparkle particles.
    /// 5. Seamless fallback/plug-and-play support for Google Labs AI video (MP4) playback via VideoPlayer.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class AnimatedPickupBannerController : MonoBehaviour
    {
        [Header("Banner Background")]
        [SerializeField] private Image backgroundImage;

        [Header("Live Mascot Layer")]
        [SerializeField] private RectTransform mascotTransform;
        [SerializeField] private Image mascotImage;
        [SerializeField] private Vector2 mascotBaseAnchoredPos = new Vector2(0f, -10f);
        [SerializeField] private float floatAmplitude = 10f;
        [SerializeField] private float floatSpeed = 2.2f;
        [SerializeField] private float breatheSpeed = 2.8f;
        [SerializeField] private float breatheScale = 0.035f;
        [SerializeField] private float tiltAngle = 2.5f;

        [Header("Sacred Rays & Aura")]
        [SerializeField] private RectTransform sunburstTransform;
        [SerializeField] private Image sunburstImage;
        [SerializeField] private float sunburstRotationSpeed = 15f;
        [SerializeField] private RectTransform rainbowAuraTransform;
        [SerializeField] private Image rainbowAuraImage;
        [SerializeField] private float auraPulseSpeed = 2.0f;

        [Header("Shooting Stars FX")]
        [SerializeField] private RectTransform shootingStarContainer;
        [SerializeField] private Sprite shootingStarSprite;
        [SerializeField] private int maxShootingStars = 3;
        [SerializeField] private float minStarInterval = 1.6f;
        [SerializeField] private float maxStarInterval = 3.0f;

        [Header("Sparkle Particles")]
        [SerializeField] private RectTransform sparkleContainer;
        [SerializeField] private Sprite sparkleSprite;
        [SerializeField] private int sparkleCount = 14;

        [Header("External Video Player (Google Labs Support)")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RawImage videoDisplay;
        [SerializeField] private VideoClip videoClip;
        [SerializeField] private CanvasGroup videoCanvasGroup;
        [SerializeField] private Button skipButton;
        [SerializeField] private bool useVideoIfAvailable = true;
        [SerializeField] private string videoFileName = "mallang_pick1.mp4";
        [SerializeField] private bool playVideoOnTabSelected = true;

        private List<SparkleItem> _sparkles = new List<SparkleItem>();
        private List<ShootingStarItem> _shootingStars = new List<ShootingStarItem>();
        private Coroutine _starSpawnerCoroutine;
        private Coroutine _fadeCoroutine;
        private bool _isPlayingVideo = false;
        private RenderTexture _videoRenderTexture;

        private class SparkleItem
        {
            public RectTransform rect;
            public Image image;
            public Vector2 basePos;
            public float phase;
            public float speed;
            public float driftRadius;
            public float scaleMin;
            public float scaleMax;
        }

        private class ShootingStarItem
        {
            public GameObject gameObject;
            public RectTransform rect;
            public Image image;
            public CanvasGroup group;
            public bool isBusy;
        }

        private void Awake()
        {
            InitializeSparkles();
            InitializeShootingStars();
            if (videoDisplay != null && videoCanvasGroup == null)
            {
                videoCanvasGroup = videoDisplay.GetComponent<CanvasGroup>();
                if (videoCanvasGroup == null) videoCanvasGroup = videoDisplay.gameObject.AddComponent<CanvasGroup>();
            }
            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(SkipVideo);
            }
        }

        private void OnEnable()
        {
            InitializeSparkles();
            InitializeShootingStars();

            if (Application.isPlaying)
            {
                if (playVideoOnTabSelected && useVideoIfAvailable && (videoClip != null || HasVideoFile()))
                {
                    PlayIntroVideo();
                }
                else
                {
                    ShowLiveElements(true);
                }
            }
            else
            {
                ShowLiveElements(true);
            }
        }

        private void OnDisable()
        {
            if (_starSpawnerCoroutine != null)
            {
                StopCoroutine(_starSpawnerCoroutine);
                _starSpawnerCoroutine = null;
            }
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            _isPlayingVideo = false;
            BlockAudioManager.Instance?.UnduckBGM(0.5f);
        }

        private void OnDestroy()
        {
            if (_videoRenderTexture != null)
            {
                if (videoPlayer != null) videoPlayer.targetTexture = null;
                _videoRenderTexture.Release();
                Destroy(_videoRenderTexture);
                _videoRenderTexture = null;
            }
        }

        private void Update()
        {
            if (_isPlayingVideo) return;

            float time = Application.isPlaying ? Time.unscaledTime : (float)Time.realtimeSinceStartup;
            float dt = Application.isPlaying ? Time.unscaledDeltaTime : 0.016f;

            // 1. Sunburst Continuous Rotation & Alpha Breathing
            if (sunburstTransform != null)
            {
                sunburstTransform.Rotate(0f, 0f, -sunburstRotationSpeed * dt);
                if (sunburstImage != null)
                {
                    float alpha = Mathf.Lerp(0.40f, 0.75f, (Mathf.Sin(time * 2.0f) + 1f) * 0.5f);
                    Color col = sunburstImage.color;
                    col.a = alpha;
                    sunburstImage.color = col;
                }
            }

            // 2. Rainbow Aura Soft Pulsing Scale & Alpha
            if (rainbowAuraTransform != null)
            {
                float auraScale = 1.0f + Mathf.Sin(time * auraPulseSpeed) * 0.08f;
                rainbowAuraTransform.localScale = new Vector3(auraScale, auraScale, 1f);

                if (rainbowAuraImage != null)
                {
                    float a = Mathf.Lerp(0.55f, 0.85f, (Mathf.Sin(time * auraPulseSpeed + 1.2f) + 1f) * 0.5f);
                    Color c = rainbowAuraImage.color;
                    c.a = a;
                    rainbowAuraImage.color = c;
                }
            }

            // 3. Special Mascot Floating, Breathing Squash-and-Stretch & Tilt
            if (mascotTransform != null)
            {
                float yOffset = Mathf.Sin(time * floatSpeed) * floatAmplitude;
                mascotTransform.anchoredPosition = mascotBaseAnchoredPos + new Vector2(0f, yOffset);

                float breatheY = 1.0f + Mathf.Sin(time * breatheSpeed) * breatheScale;
                float breatheX = 1.0f - Mathf.Sin(time * breatheSpeed) * (breatheScale * 0.6f);
                mascotTransform.localScale = new Vector3(breatheX, breatheY, 1f);

                float tilt = Mathf.Sin(time * (floatSpeed * 0.7f)) * tiltAngle;
                mascotTransform.localRotation = Quaternion.Euler(0f, 0f, tilt);
            }

            // 4. Update Fairy Sparkles
            UpdateSparkles(time);
        }

        #region Sparkles

        private void InitializeSparkles()
        {
            if (sparkleContainer == null || sparkleSprite == null) return;
            if (_sparkles.Count > 0) return;

            // Preset anchor spots distributed across the banner
            Vector2[] presetPositions = new Vector2[]
            {
                new Vector2(-280f, 120f), new Vector2(-190f, 170f), new Vector2(-120f, 80f),
                new Vector2(130f, 160f),  new Vector2(220f, 110f),  new Vector2(300f, 150f),
                new Vector2(-220f, -60f), new Vector2(-310f, 20f),  new Vector2(250f, -40f),
                new Vector2(320f, 30f),   new Vector2(-80f, -120f), new Vector2(90f, -110f),
                new Vector2(0f, 190f),    new Vector2(-50f, 150f)
            };

            for (int i = 0; i < sparkleCount; i++)
            {
                Vector2 basePos = presetPositions[i % presetPositions.Length];
                // add subtle random jitter
                basePos += new Vector2(Random.Range(-25f, 25f), Random.Range(-20f, 20f));

                GameObject spObj = new GameObject($"Sparkle_{i}", typeof(RectTransform), typeof(Image));
                spObj.transform.SetParent(sparkleContainer, false);

                RectTransform rt = spObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = basePos;
                rt.sizeDelta = new Vector2(36f, 36f);

                Image img = spObj.GetComponent<Image>();
                img.sprite = sparkleSprite;
                img.raycastTarget = false;
                img.preserveAspect = true;

                // Color variation: pastel gold, celestial cyan, soft magenta
                Color[] colors = new Color[]
                {
                    new Color(1f, 0.95f, 0.70f, 1f),
                    new Color(0.70f, 0.90f, 1f, 1f),
                    new Color(1f, 0.80f, 0.95f, 1f)
                };
                img.color = colors[i % colors.Length];

                _sparkles.Add(new SparkleItem
                {
                    rect = rt,
                    image = img,
                    basePos = basePos,
                    phase = Random.Range(0f, Mathf.PI * 2f),
                    speed = Random.Range(1.8f, 3.2f),
                    driftRadius = Random.Range(4f, 10f),
                    scaleMin = Random.Range(0.4f, 0.6f),
                    scaleMax = Random.Range(0.9f, 1.35f)
                });
            }
        }

        private void UpdateSparkles(float time)
        {
            for (int i = 0; i < _sparkles.Count; i++)
            {
                var sp = _sparkles[i];
                if (sp.rect == null || sp.image == null) continue;

                float t = time * sp.speed + sp.phase;

                // Subtle orbital drift
                Vector2 drift = new Vector2(Mathf.Sin(t) * sp.driftRadius, Mathf.Cos(t * 1.3f) * sp.driftRadius);
                sp.rect.anchoredPosition = sp.basePos + drift;

                // Pulsing scale & alpha
                float norm = (Mathf.Sin(t * 1.5f) + 1f) * 0.5f;
                float currentScale = Mathf.Lerp(sp.scaleMin, sp.scaleMax, norm);
                sp.rect.localScale = new Vector3(currentScale, currentScale, 1f);

                Color c = sp.image.color;
                c.a = Mathf.Lerp(0.20f, 0.95f, norm);
                sp.image.color = c;
            }
        }

        #endregion

        #region Shooting Stars

        private void InitializeShootingStars()
        {
            if (shootingStarContainer == null || shootingStarSprite == null) return;
            if (_shootingStars.Count > 0) return;

            for (int i = 0; i < maxShootingStars; i++)
            {
                GameObject starObj = new GameObject($"ShootingStar_{i}", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
                starObj.transform.SetParent(shootingStarContainer, false);
                starObj.SetActive(false);

                RectTransform rt = starObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(240f, 60f);

                Image img = starObj.GetComponent<Image>();
                img.sprite = shootingStarSprite;
                img.raycastTarget = false;
                img.preserveAspect = true;

                CanvasGroup cg = starObj.GetComponent<CanvasGroup>();
                cg.alpha = 0f;

                _shootingStars.Add(new ShootingStarItem
                {
                    gameObject = starObj,
                    rect = rt,
                    image = img,
                    group = cg,
                    isBusy = false
                });
            }
        }

        private IEnumerator ShootingStarLoop()
        {
            while (true)
            {
                float wait = Random.Range(minStarInterval, maxStarInterval);
                yield return new WaitForSecondsRealtime(wait);

                // Find an idle shooting star
                ShootingStarItem idleStar = null;
                for (int i = 0; i < _shootingStars.Count; i++)
                {
                    if (!_shootingStars[i].isBusy)
                    {
                        idleStar = _shootingStars[i];
                        break;
                    }
                }

                if (idleStar != null)
                {
                    StartCoroutine(LaunchShootingStar(idleStar));
                }
            }
        }

        private IEnumerator LaunchShootingStar(ShootingStarItem star)
        {
            star.isBusy = true;
            star.gameObject.SetActive(true);

            // Trajectory: from upper right to lower left across the cosmic sky
            float startX = Random.Range(80f, 380f);
            float startY = Random.Range(130f, 220f);
            Vector2 startPos = new Vector2(startX, startY);

            float travelDist = Random.Range(550f, 750f);
            float angleDeg = Random.Range(-32f, -26f); // Heading down-left
            float angleRad = (angleDeg + 180f) * Mathf.Deg2Rad; // Tail faces backwards
            Vector2 dir = new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
            Vector2 endPos = startPos + dir * travelDist;

            star.rect.anchoredPosition = startPos;
            star.rect.localRotation = Quaternion.Euler(0f, 0f, angleDeg);

            float duration = Random.Range(0.65f, 0.90f);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Smooth ease out trajectory
                float easeT = 1f - Mathf.Pow(1f - t, 2.2f);
                star.rect.anchoredPosition = Vector2.Lerp(startPos, endPos, easeT);

                // Fade in quickly, then fade out smoothly
                float alpha;
                if (t < 0.25f)
                {
                    alpha = t / 0.25f;
                }
                else
                {
                    alpha = 1f - ((t - 0.25f) / 0.75f);
                }

                star.group.alpha = Mathf.Clamp01(alpha);
                yield return null;
            }

            star.group.alpha = 0f;
            star.gameObject.SetActive(false);
            star.isBusy = false;
        }

        #endregion

        #region Video Player Support (mallang_pick1 Video Playback & Transition)

        private bool HasVideoFile()
        {
            string p1 = Path.Combine(Application.dataPath, "movies", videoFileName);
            string p2 = Path.Combine(Application.streamingAssetsPath, videoFileName);
            return File.Exists(p1) || File.Exists(p2);
        }

        private string GetVideoFilePath()
        {
            string p1 = Path.Combine(Application.dataPath, "movies", videoFileName);
            if (File.Exists(p1)) return p1;
            string p2 = Path.Combine(Application.streamingAssetsPath, videoFileName);
            if (File.Exists(p2)) return p2;
            return null;
        }

        /// <summary>
        /// Plays mallang_pick1 video animation first upon selecting the Pickup tab,
        /// then seamlessly transitions to the live floating mascot and cosmic background.
        /// </summary>
        public void PlayIntroVideo()
        {
            if (videoPlayer == null)
            {
                ShowLiveElements(true);
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (_videoRenderTexture == null)
            {
                _videoRenderTexture = new RenderTexture(1024, 576, 0, RenderTextureFormat.ARGB32);
                _videoRenderTexture.Create();
            }

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = _videoRenderTexture;
            videoPlayer.isLooping = false;

            if (videoClip != null)
            {
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = videoClip;
            }
            else
            {
                string targetPath = GetVideoFilePath();
                if (!string.IsNullOrEmpty(targetPath))
                {
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = targetPath;
                }
                else
                {
                    ShowLiveElements(true);
                    return;
                }
            }

            if (videoDisplay != null)
            {
                videoDisplay.texture = _videoRenderTexture;
                videoDisplay.gameObject.SetActive(true);
                if (videoCanvasGroup != null)
                {
                    videoCanvasGroup.alpha = 1f;
                    videoCanvasGroup.interactable = true;
                    videoCanvasGroup.blocksRaycasts = true;
                }
            }

            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
            }

            // Hide live procedural layers during video playback
            SetLiveLayersActive(false);

            _isPlayingVideo = true;

            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.time = 0;
            videoPlayer.Play();
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.DuckBGM(0.25f, 0.4f);
            Debug.Log("<color=cyan><b>[AnimatedPickupBanner]</b> Playing mallang_pick1 intro animation with BGM ducking...</color>");
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            if (_isPlayingVideo)
            {
                TransitionToLiveElements();
            }
        }

        public void SkipVideo()
        {
            if (_isPlayingVideo)
            {
                TransitionToLiveElements();
            }
        }

        public void StopVideo()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.UnduckBGM(0.5f);
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            ShowLiveElements(true);
        }

        private void TransitionToLiveElements()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.UnduckBGM(0.8f);
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(CrossFadeToLiveRoutine());
        }

        private IEnumerator CrossFadeToLiveRoutine()
        {
            // Activate live procedural layers immediately underneath
            SetLiveLayersActive(true);

            if (videoCanvasGroup != null)
            {
                float elapsed = 0f;
                float duration = 0.35f;
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    videoCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                    yield return null;
                }
                videoCanvasGroup.alpha = 0f;
                videoCanvasGroup.interactable = false;
                videoCanvasGroup.blocksRaycasts = false;
            }

            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(false);
            }

            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }

            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            _isPlayingVideo = false;
            _fadeCoroutine = null;

            if (Application.isPlaying && _starSpawnerCoroutine == null)
            {
                _starSpawnerCoroutine = StartCoroutine(ShootingStarLoop());
            }
        }

        public void ShowLiveElements(bool immediate)
        {
            _isPlayingVideo = false;

            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }

            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(false);
                if (videoCanvasGroup != null)
                {
                    videoCanvasGroup.alpha = 0f;
                    videoCanvasGroup.interactable = false;
                    videoCanvasGroup.blocksRaycasts = false;
                }
            }

            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }

            SetLiveLayersActive(true);

            if (Application.isPlaying && _starSpawnerCoroutine == null)
            {
                _starSpawnerCoroutine = StartCoroutine(ShootingStarLoop());
            }
        }

        private void SetLiveLayersActive(bool active)
        {
            if (sunburstTransform != null) sunburstTransform.gameObject.SetActive(active);
            if (rainbowAuraTransform != null) rainbowAuraTransform.gameObject.SetActive(active);
            if (mascotTransform != null) mascotTransform.gameObject.SetActive(active);
            if (shootingStarContainer != null) shootingStarContainer.gameObject.SetActive(active);
            if (sparkleContainer != null) sparkleContainer.gameObject.SetActive(active);
        }

        public void EnableVideoMode(string videoUrl = null)
        {
            PlayIntroVideo();
        }

        #endregion

        #region Editor Setup Helper

        public void SetupReferences(
            Image bgImg,
            RectTransform mascotRt, Image mascotImg,
            RectTransform sunburstRt, Image sunburstImg,
            RectTransform auraRt, Image auraImg,
            RectTransform starContainer, Sprite starSp,
            RectTransform spContainer, Sprite spSprite,
            VideoPlayer vp, RawImage vDisplay,
            VideoClip clip = null, Button skipBtn = null)
        {
            backgroundImage = bgImg;
            mascotTransform = mascotRt;
            mascotImage = mascotImg;
            sunburstTransform = sunburstRt;
            sunburstImage = sunburstImg;
            rainbowAuraTransform = auraRt;
            rainbowAuraImage = auraImg;
            shootingStarContainer = starContainer;
            shootingStarSprite = starSp;
            sparkleContainer = spContainer;
            sparkleSprite = spSprite;
            videoPlayer = vp;
            videoDisplay = vDisplay;
            videoClip = clip;
            skipButton = skipBtn;

            if (videoDisplay != null)
            {
                videoCanvasGroup = videoDisplay.GetComponent<CanvasGroup>();
                if (videoCanvasGroup == null)
                {
                    videoCanvasGroup = videoDisplay.gameObject.AddComponent<CanvasGroup>();
                }

                Button vBtn = videoDisplay.GetComponent<Button>();
                if (vBtn == null) vBtn = videoDisplay.gameObject.AddComponent<Button>();
                vBtn.transition = Selectable.Transition.None;
                vBtn.onClick.RemoveAllListeners();
                vBtn.onClick.AddListener(SkipVideo);
            }

            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(SkipVideo);
            }

            if (mascotTransform != null)
            {
                mascotBaseAnchoredPos = mascotTransform.anchoredPosition;
            }
        }

        #endregion
    }
}
