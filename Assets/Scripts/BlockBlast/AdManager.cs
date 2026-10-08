using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GoogleMobileAds.Api;

namespace BlockBlast
{
    /// <summary>
    /// Manages Google Mobile Ads (AdMob) initialization, loading, and display.
    /// Supports Rewarded Ads with real unit IDs, Google test unit IDs, and Editor simulation.
    /// Features in-Editor visual mock ad simulation and universal Reward Claim Popup.
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        private static AdManager _instance;
        public static AdManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<AdManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AdManager");
                        _instance = go.AddComponent<AdManager>();
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        public const string ANDROID_APP_ID = "ca-app-pub-6447676826673071~4554049525";
        public const string REWARDED_AD_UNIT_ID = "ca-app-pub-6447676826673071/5945268224";
        public const string TEST_REWARDED_AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917";

        [Header("AdMob Configuration")]
        [Tooltip("When true on mobile devices, requests Google's official sample ad unit to prevent invalid traffic penalties during development.")]
        [SerializeField] private bool useTestAdOnDevice = false;

        private RewardedAd _rewardedAd;
        private bool _isInitialized = false;
        private bool _isLoadingAd = false;

        private Action _onRewardEarnedCallback;
        private Action _onAdClosedCallback;
        private Action<string> _onAdFailedCallback;

        // Visual UI Overlay Components
        private Canvas _uiCanvas;
        private GameObject _mockAdRoot;
        private TMP_Text _mockCountdownText;
        private Image _mockProgressBar;
        private GameObject _rewardModalRoot;
        private TMP_Text _rewardModalTitle;
        private TMP_Text _rewardModalAmount;
        private TMP_Text _rewardModalDesc;
        private Image _rewardModalIcon;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                InitializeAdMob();
                BuildVisualUI();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAdMob()
        {
            try
            {
                MobileAds.Initialize(initStatus =>
                {
                    _isInitialized = true;
                    Debug.Log("<color=green><b>[AdManager] Google Mobile Ads (AdMob) initialized successfully.</b></color>");
                    LoadRewardedAd();
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AdManager] MobileAds.Initialize failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Pre-loads a rewarded ad into memory.
        /// </summary>
        public void LoadRewardedAd()
        {
#if UNITY_EDITOR
            return;
#else
            if (_isLoadingAd) return;
            _isLoadingAd = true;

            string adUnit = useTestAdOnDevice ? TEST_REWARDED_AD_UNIT_ID : REWARDED_AD_UNIT_ID;

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(adUnit, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                _isLoadingAd = false;
                if (error != null || ad == null)
                {
                    Debug.LogWarning($"[AdManager] Rewarded ad failed to load: {error?.GetMessage()}");
                    return;
                }

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
                Debug.Log("<color=cyan>[AdManager] Rewarded ad loaded and ready to display.</color>");
            });
#endif
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("[AdManager] Rewarded ad closed.");
                _onAdClosedCallback?.Invoke();
                _onAdClosedCallback = null;
                LoadRewardedAd();
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError($"[AdManager] Rewarded ad failed to display: {error.GetMessage()}");
                _onAdFailedCallback?.Invoke(error.GetMessage());
                _onAdFailedCallback = null;
                LoadRewardedAd();
            };

            ad.OnAdPaid += (AdValue value) =>
            {
                Debug.Log($"[AdManager] Rewarded ad revenue: {value.Value} {value.CurrencyCode}");
            };
        }

        /// <summary>
        /// Displays the rewarded ad. In Editor, plays a realistic visual mock ad.
        /// When completed, automatically triggers reward and displays the reward popup!
        /// </summary>
        public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null, Action<string> onAdFailed = null)
        {
            _onRewardEarnedCallback = onRewardEarned;
            _onAdClosedCallback = onAdClosed;
            _onAdFailedCallback = onAdFailed;

#if UNITY_EDITOR
            StartCoroutine(EditorSimulateRewardRoutine());
#else
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show(reward =>
                {
                    Debug.Log($"<color=green>[AdManager] User rewarded: {reward.Type} ({reward.Amount})</color>");
                    _onRewardEarnedCallback?.Invoke();
                    _onRewardEarnedCallback = null;
                    ShowRewardPopup(
                        LocalizationManager.Get("reward_claim_title"),
                        "+30 다이아",
                        GetDefaultDiamondSprite(),
                        LocalizationManager.Get("shop_ad_diamond_toast")
                    );
                });
            }
            else
            {
                Debug.LogWarning("[AdManager] Rewarded ad is not ready. Attempting reload...");
                LoadRewardedAd();
                onAdFailed?.Invoke("Ad is still loading. Please try again in a moment.");
            }
#endif
        }

#if UNITY_EDITOR
        private IEnumerator EditorSimulateRewardRoutine()
        {
            EnsureUI();
            if (_mockAdRoot != null) _mockAdRoot.SetActive(true);

            float duration = 2.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                int remainSec = Mathf.CeilToInt(duration - elapsed);

                if (_mockCountdownText != null)
                {
                    _mockCountdownText.text = $"광고 완료까지 {remainSec}초...";
                }
                if (_mockProgressBar != null)
                {
                    _mockProgressBar.fillAmount = progress;
                }
                yield return null;
            }

            if (_mockAdRoot != null) _mockAdRoot.SetActive(false);

            // Execute Reward Callback
            _onRewardEarnedCallback?.Invoke();
            _onRewardEarnedCallback = null;
            _onAdClosedCallback?.Invoke();
            _onAdClosedCallback = null;

            // Show Reward Popup
            ShowRewardPopup(
                LocalizationManager.Get("reward_claim_title"),
                "+30 다이아",
                GetDefaultDiamondSprite(),
                LocalizationManager.Get("shop_ad_diamond_toast")
            );
        }
#endif

        /// <summary>
        /// Displays the universal Reward Claim Modal with congratulations, icon, and amount.
        /// </summary>
        public void ShowRewardPopup(string title, string amountText, Sprite icon = null, string desc = null)
        {
            EnsureUI();
            if (_rewardModalRoot == null) return;

            if (_rewardModalTitle != null)
            {
                _rewardModalTitle.text = string.IsNullOrEmpty(title) ? LocalizationManager.Get("reward_claim_title") : title;
            }
            if (_rewardModalAmount != null)
            {
                _rewardModalAmount.text = amountText;
            }
            if (_rewardModalDesc != null)
            {
                _rewardModalDesc.text = string.IsNullOrEmpty(desc) ? LocalizationManager.Get("shop_ad_diamond_toast") : desc;
            }
            if (_rewardModalIcon != null)
            {
                Sprite targetIcon = (icon != null) ? icon : GetDefaultDiamondSprite();
                if (targetIcon != null)
                {
                    _rewardModalIcon.sprite = targetIcon;
                    _rewardModalIcon.gameObject.SetActive(true);
                }
            }

            _rewardModalRoot.SetActive(true);

            // Audio & Sparkle Celebrations
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
        }

        public void CloseRewardPopup()
        {
            if (_rewardModalRoot != null)
            {
                _rewardModalRoot.SetActive(false);
            }
        }

        private Sprite GetDefaultDiamondSprite()
        {
            var diaImg = GameObject.Find("DiaBadge/Icon");
            if (diaImg != null)
            {
                var img = diaImg.GetComponent<Image>();
                if (img != null && img.sprite != null) return img.sprite;
            }
            var shopDia = GameObject.Find("ShopModal/DialogCard/DiaBadge/Icon");
            if (shopDia != null)
            {
                var img = shopDia.GetComponent<Image>();
                if (img != null && img.sprite != null) return img.sprite;
            }
            return null;
        }

        private void EnsureUI()
        {
            if (_uiCanvas == null) BuildVisualUI();
        }

        private void BuildVisualUI()
        {
            if (_uiCanvas != null) return;

            // 1. Root Overlay Canvas
            GameObject canvasObj = new GameObject("AdManagerOverlayCanvas");
            canvasObj.transform.SetParent(transform, false);
            _uiCanvas = canvasObj.AddComponent<Canvas>();
            _uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _uiCanvas.sortingOrder = 9999; // Topmost priority over all game modals

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Font lookup
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts/Jua-Regular SDF");
            if (font == null)
            {
                var anyTmp = FindAnyObjectByType<TMP_Text>();
                if (anyTmp != null) font = anyTmp.font;
            }

            // ==========================================
            // 2. MOCK AD OVERLAY (In-Editor Visuals)
            // ==========================================
            _mockAdRoot = new GameObject("MockAdOverlay", typeof(RectTransform), typeof(Image));
            _mockAdRoot.transform.SetParent(canvasObj.transform, false);
            RectTransform mockRt = _mockAdRoot.GetComponent<RectTransform>();
            mockRt.anchorMin = Vector2.zero;
            mockRt.anchorMax = Vector2.one;
            mockRt.sizeDelta = Vector2.zero;
            mockRt.anchoredPosition = Vector2.zero;
            Image mockBg = _mockAdRoot.GetComponent<Image>();
            mockBg.color = new Color(0.06f, 0.05f, 0.12f, 0.96f);

            // Mock Dialog Card
            GameObject adCard = new GameObject("AdCard", typeof(RectTransform), typeof(Image));
            adCard.transform.SetParent(_mockAdRoot.transform, false);
            RectTransform cardRt = adCard.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(650, 600);
            cardRt.anchoredPosition = Vector2.zero;
            Image cardBg = adCard.GetComponent<Image>();
            cardBg.color = new Color(0.16f, 0.12f, 0.28f, 0.98f);

            // Mock Card Title
            GameObject adTitle = new GameObject("AdTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
            adTitle.transform.SetParent(adCard.transform, false);
            TextMeshProUGUI titleTmp = adTitle.GetComponent<TextMeshProUGUI>();
            titleTmp.text = "🎬 광고 시뮬레이션 (테스트)";
            titleTmp.fontSize = 32;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(0.40f, 0.95f, 1f);
            if (font != null) titleTmp.font = font;
            RectTransform titleRt = adTitle.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 1f);
            titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0, -60);
            titleRt.sizeDelta = new Vector2(600, 50);

            // Clapperboard / Video Icon
            GameObject adIcon = new GameObject("AdIcon", typeof(RectTransform), typeof(TextMeshProUGUI));
            adIcon.transform.SetParent(adCard.transform, false);
            TextMeshProUGUI iconTmp = adIcon.GetComponent<TextMeshProUGUI>();
            iconTmp.text = "🎥";
            iconTmp.fontSize = 80;
            iconTmp.alignment = TextAlignmentOptions.Center;
            RectTransform iconRt = adIcon.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0.5f, 0.5f);
            iconRt.anchorMax = new Vector2(0.5f, 0.5f);
            iconRt.anchoredPosition = new Vector2(0, 70);
            iconRt.sizeDelta = new Vector2(150, 150);

            // Countdown Status Text
            GameObject cdObj = new GameObject("CountdownText", typeof(RectTransform), typeof(TextMeshProUGUI));
            cdObj.transform.SetParent(adCard.transform, false);
            _mockCountdownText = cdObj.GetComponent<TextMeshProUGUI>();
            _mockCountdownText.text = "광고 완료까지 3초...";
            _mockCountdownText.fontSize = 30;
            _mockCountdownText.fontStyle = FontStyles.Bold;
            _mockCountdownText.alignment = TextAlignmentOptions.Center;
            _mockCountdownText.color = Color.white;
            if (font != null) _mockCountdownText.font = font;
            RectTransform cdRt = cdObj.GetComponent<RectTransform>();
            cdRt.anchorMin = new Vector2(0.5f, 0.5f);
            cdRt.anchorMax = new Vector2(0.5f, 0.5f);
            cdRt.anchoredPosition = new Vector2(0, -40);
            cdRt.sizeDelta = new Vector2(600, 45);

            // Progress Bar Track
            GameObject pbTrack = new GameObject("ProgressBarTrack", typeof(RectTransform), typeof(Image));
            pbTrack.transform.SetParent(adCard.transform, false);
            RectTransform trackRt = pbTrack.GetComponent<RectTransform>();
            trackRt.anchorMin = new Vector2(0.5f, 0.5f);
            trackRt.anchorMax = new Vector2(0.5f, 0.5f);
            trackRt.anchoredPosition = new Vector2(0, -100);
            trackRt.sizeDelta = new Vector2(480, 24);
            Image trackBg = pbTrack.GetComponent<Image>();
            trackBg.color = new Color(0.25f, 0.20f, 0.38f, 1f);

            // Progress Bar Fill
            GameObject pbFill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            pbFill.transform.SetParent(pbTrack.transform, false);
            _mockProgressBar = pbFill.GetComponent<Image>();
            _mockProgressBar.type = Image.Type.Filled;
            _mockProgressBar.fillMethod = Image.FillMethod.Horizontal;
            _mockProgressBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            _mockProgressBar.fillAmount = 0f;
            _mockProgressBar.color = new Color(0.20f, 0.85f, 0.65f); // Vibrant mint green
            RectTransform fillRt = pbFill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;

            // Note at bottom
            GameObject adNote = new GameObject("AdNote", typeof(RectTransform), typeof(TextMeshProUGUI));
            adNote.transform.SetParent(adCard.transform, false);
            TextMeshProUGUI noteTmp = adNote.GetComponent<TextMeshProUGUI>();
            noteTmp.text = "※ 실제 모바일 빌드에서는 실제 동영상 광고가 재생됩니다.";
            noteTmp.fontSize = 20;
            noteTmp.alignment = TextAlignmentOptions.Center;
            noteTmp.color = new Color(0.70f, 0.65f, 0.85f);
            if (font != null) noteTmp.font = font;
            RectTransform noteRt = adNote.GetComponent<RectTransform>();
            noteRt.anchorMin = new Vector2(0.5f, 0f);
            noteRt.anchorMax = new Vector2(0.5f, 0f);
            noteRt.anchoredPosition = new Vector2(0, 45);
            noteRt.sizeDelta = new Vector2(580, 40);

            _mockAdRoot.SetActive(false);

            // ==========================================
            // 3. REWARD CLAIM MODAL (Both Editor & Mobile)
            // ==========================================
            _rewardModalRoot = new GameObject("RewardClaimModal", typeof(RectTransform), typeof(Image));
            _rewardModalRoot.transform.SetParent(canvasObj.transform, false);
            RectTransform rewModalRt = _rewardModalRoot.GetComponent<RectTransform>();
            rewModalRt.anchorMin = Vector2.zero;
            rewModalRt.anchorMax = Vector2.one;
            rewModalRt.sizeDelta = Vector2.zero;
            rewModalRt.anchoredPosition = Vector2.zero;
            Image rewModalBg = _rewardModalRoot.GetComponent<Image>();
            rewModalBg.color = new Color(0.04f, 0.02f, 0.10f, 0.75f);
            Button rewModalDimBtn = _rewardModalRoot.AddComponent<Button>();
            rewModalDimBtn.onClick.AddListener(CloseRewardPopup);

            // Dialog Card
            GameObject rewCard = new GameObject("DialogCard", typeof(RectTransform), typeof(Image));
            rewCard.transform.SetParent(_rewardModalRoot.transform, false);
            RectTransform rcRt = rewCard.GetComponent<RectTransform>();
            rcRt.anchorMin = new Vector2(0.5f, 0.5f);
            rcRt.anchorMax = new Vector2(0.5f, 0.5f);
            rcRt.sizeDelta = new Vector2(580, 560);
            rcRt.anchoredPosition = Vector2.zero;
            Image rcBg = rewCard.GetComponent<Image>();
            rcBg.color = new Color(0.98f, 0.96f, 1f, 1f); // Pristine luxury white cream card

            // Card Header Title: "🎉 보상 획득 완료!"
            GameObject rewTitleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            rewTitleObj.transform.SetParent(rewCard.transform, false);
            _rewardModalTitle = rewTitleObj.GetComponent<TextMeshProUGUI>();
            _rewardModalTitle.text = "🎉 보상 획득 완료!";
            _rewardModalTitle.fontSize = 34;
            _rewardModalTitle.fontStyle = FontStyles.Bold;
            _rewardModalTitle.alignment = TextAlignmentOptions.Center;
            _rewardModalTitle.color = new Color(0.25f, 0.12f, 0.40f);
            if (font != null) _rewardModalTitle.font = font;
            RectTransform rTitleRt = rewTitleObj.GetComponent<RectTransform>();
            rTitleRt.anchorMin = new Vector2(0.5f, 1f);
            rTitleRt.anchorMax = new Vector2(0.5f, 1f);
            rTitleRt.anchoredPosition = new Vector2(0, -60);
            rTitleRt.sizeDelta = new Vector2(520, 50);

            // Icon Frame (Circle Backing)
            GameObject rewIconFrame = new GameObject("IconFrame", typeof(RectTransform), typeof(Image));
            rewIconFrame.transform.SetParent(rewCard.transform, false);
            RectTransform iconFrameRt = rewIconFrame.GetComponent<RectTransform>();
            iconFrameRt.anchorMin = new Vector2(0.5f, 0.5f);
            iconFrameRt.anchorMax = new Vector2(0.5f, 0.5f);
            iconFrameRt.anchoredPosition = new Vector2(0, 80);
            iconFrameRt.sizeDelta = new Vector2(160, 160);
            Image ifImg = rewIconFrame.GetComponent<Image>();
            ifImg.color = new Color(0.92f, 0.88f, 0.98f, 1f);

            // Reward Icon
            GameObject rewIconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            rewIconObj.transform.SetParent(rewIconFrame.transform, false);
            _rewardModalIcon = rewIconObj.GetComponent<Image>();
            _rewardModalIcon.preserveAspect = true;
            RectTransform riRt = rewIconObj.GetComponent<RectTransform>();
            riRt.anchorMin = new Vector2(0.5f, 0.5f);
            riRt.anchorMax = new Vector2(0.5f, 0.5f);
            riRt.anchoredPosition = Vector2.zero;
            riRt.sizeDelta = new Vector2(120, 120);

            // Big Amount Text: "+30 다이아"
            GameObject rewAmtObj = new GameObject("Amount", typeof(RectTransform), typeof(TextMeshProUGUI));
            rewAmtObj.transform.SetParent(rewCard.transform, false);
            _rewardModalAmount = rewAmtObj.GetComponent<TextMeshProUGUI>();
            _rewardModalAmount.text = "+30 다이아";
            _rewardModalAmount.fontSize = 42;
            _rewardModalAmount.fontStyle = FontStyles.Bold;
            _rewardModalAmount.alignment = TextAlignmentOptions.Center;
            _rewardModalAmount.color = new Color(0.12f, 0.55f, 0.85f); // Sparkling cyan blue
            if (font != null) _rewardModalAmount.font = font;
            RectTransform rAmtRt = rewAmtObj.GetComponent<RectTransform>();
            rAmtRt.anchorMin = new Vector2(0.5f, 0.5f);
            rAmtRt.anchorMax = new Vector2(0.5f, 0.5f);
            rAmtRt.anchoredPosition = new Vector2(0, -40);
            rAmtRt.sizeDelta = new Vector2(520, 56);

            // Subtitle Description Text
            GameObject rewDescObj = new GameObject("Desc", typeof(RectTransform), typeof(TextMeshProUGUI));
            rewDescObj.transform.SetParent(rewCard.transform, false);
            _rewardModalDesc = rewDescObj.GetComponent<TextMeshProUGUI>();
            _rewardModalDesc.text = "광고 시청 보상이 지급되었습니다!";
            _rewardModalDesc.fontSize = 22;
            _rewardModalDesc.alignment = TextAlignmentOptions.Center;
            _rewardModalDesc.color = new Color(0.45f, 0.35f, 0.55f);
            if (font != null) _rewardModalDesc.font = font;
            RectTransform rDescRt = rewDescObj.GetComponent<RectTransform>();
            rDescRt.anchorMin = new Vector2(0.5f, 0.5f);
            rDescRt.anchorMax = new Vector2(0.5f, 0.5f);
            rDescRt.anchoredPosition = new Vector2(0, -95);
            rDescRt.sizeDelta = new Vector2(520, 40);

            // Confirm Button: [ 확인 ]
            GameObject okBtnObj = new GameObject("BtnConfirm", typeof(RectTransform), typeof(Image), typeof(Button));
            okBtnObj.transform.SetParent(rewCard.transform, false);
            RectTransform okRt = okBtnObj.GetComponent<RectTransform>();
            okRt.anchorMin = new Vector2(0.5f, 0f);
            okRt.anchorMax = new Vector2(0.5f, 0f);
            okRt.anchoredPosition = new Vector2(0, 52);
            okRt.sizeDelta = new Vector2(320, 72);
            Image okImg = okBtnObj.GetComponent<Image>();
            okImg.color = new Color(0.20f, 0.82f, 0.65f, 1f); // Vibrant mint jelly
            Button okBtn = okBtnObj.GetComponent<Button>();
            okBtn.onClick.AddListener(CloseRewardPopup);

            GameObject okTxtObj = new GameObject("Txt", typeof(RectTransform), typeof(TextMeshProUGUI));
            okTxtObj.transform.SetParent(okBtnObj.transform, false);
            TextMeshProUGUI okTmp = okTxtObj.GetComponent<TextMeshProUGUI>();
            okTmp.text = LocalizationManager.Get("reward_claim_btn");
            okTmp.fontSize = 28;
            okTmp.fontStyle = FontStyles.Bold;
            okTmp.alignment = TextAlignmentOptions.Center;
            okTmp.color = Color.white;
            if (font != null) okTmp.font = font;
            RectTransform okTxtRt = okTxtObj.GetComponent<RectTransform>();
            okTxtRt.anchorMin = Vector2.zero;
            okTxtRt.anchorMax = Vector2.one;
            okTxtRt.sizeDelta = Vector2.zero;

            _rewardModalRoot.SetActive(false);
        }
    }
}
