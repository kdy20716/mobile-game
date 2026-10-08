using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    [Serializable]
    public class GachaDropItem
    {
        public bool isSpecial;
        public int mascotIndex;
        public int shardCount;
        public bool isDuplicateSpecial;
    }

    public class GachaPresentationController : MonoBehaviour
    {
        private static GachaPresentationController _instance;
        public static GachaPresentationController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GachaPresentationController>();
                }
                return _instance;
            }
            private set => _instance = value;
        }

        [Header("Root & Containers")]
        [SerializeField] private GameObject modalRoot;
        [SerializeField] private RectTransform ballsContainer;
        [SerializeField] private TMP_Text instructionText;
        [SerializeField] private Button btnOpenAll;
        [SerializeField] private Button btnConfirm;

        [Header("Sprites")]
        [SerializeField] private Sprite greyBallSprite;
        [SerializeField] private Sprite goldBallSprite;
        [SerializeField] private Sprite rainbowBallSprite;
        [SerializeField] private Sprite rainbowAuraSprite;
        [SerializeField] private Sprite sunburstSprite;
        [SerializeField] private Sprite sparkleStarSprite;
        [SerializeField] private Sprite gachaMachineSprite;
        [SerializeField] private Sprite silverCoinSprite;
        [SerializeField] private Sprite goldCoinSprite;
        [SerializeField] private Sprite[] mascotAvatars;
        [SerializeField] private Sprite specialMascotCutout;

        [Header("Full Screen Climax Presentation")]
        [SerializeField] private GameObject climaxOverlay;
        [SerializeField] private Image climaxSunburst;
        [SerializeField] private Image climaxMascotImage;
        [SerializeField] private TMP_Text climaxTitleText;
        [SerializeField] private TMP_Text climaxSubText;
        [SerializeField] private Button btnClimaxDismiss;

        // Dynamic Gacha Machine & Sequential Reveal Elements
        private GameObject _machineRoot;
        private Image _machineImg;
        private GameObject _coinObj;
        private Image _coinImg;
        private GameObject _trayRoot;

        private GameObject _singleRevealRoot;
        private RectTransform _singleBallRect;
        private Image _singleBallImg;
        private Image _singleAuraImg;
        private GameObject _singleRewardRoot;
        private Image _singleAvatarImg;
        private TMP_Text _singleNameText;
        private TMP_Text _singleRarityText;
        private TMP_Text _singleShardsText;
        private Button _fullScreenTapBtn;

        private List<GachaDropItem> _currentDrops = new List<GachaDropItem>();
        private List<GachaBallSlot> _slots = new List<GachaBallSlot>();
        private Action _onCompleteCallback;
        private bool _isOpeningAll = false;
        private bool _stepAdvanceRequested = false;
        private Coroutine _climaxSunburstCoroutine;
        private Coroutine _currentSequenceCoroutine;

        private static readonly string[] RarityNames = new string[]
        {
            "일반", "일반", "일반", "일반",
            "희귀", "희귀", "희귀", "희귀",
            "스페셜"
        };

        private static readonly Color[] RarityColors = new Color[]
        {
            new Color(0.35f, 0.45f, 0.65f), // Common: Cool slate blue
            new Color(0.35f, 0.45f, 0.65f),
            new Color(0.35f, 0.45f, 0.65f),
            new Color(0.35f, 0.45f, 0.65f),
            new Color(1.0f, 0.72f, 0.10f),  // Rare: Radiant Gold
            new Color(1.0f, 0.72f, 0.10f),
            new Color(1.0f, 0.72f, 0.10f),
            new Color(1.0f, 0.72f, 0.10f),
            new Color(1.0f, 0.40f, 0.85f)   // Special: Iridescent Pink
        };

        private class GachaBallSlot
        {
            public GameObject rootObj;
            public RectTransform rootRect;
            public Button button;
            public Image ballImage;
            public Image auraImage;
            public GameObject rewardRoot;
            public Image rewardAvatar;
            public TMP_Text rewardText;
            public bool isOpened;
            public GachaDropItem dropData;
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (modalRoot != null) modalRoot.SetActive(false);
            if (climaxOverlay != null) climaxOverlay.SetActive(false);

            if (btnOpenAll != null)
            {
                btnOpenAll.onClick.RemoveAllListeners();
                btnOpenAll.onClick.AddListener(OnOpenAllClicked);
            }

            if (btnConfirm != null)
            {
                btnConfirm.onClick.RemoveAllListeners();
                btnConfirm.onClick.AddListener(OnConfirmClicked);
            }

            if (btnClimaxDismiss != null)
            {
                btnClimaxDismiss.onClick.RemoveAllListeners();
                btnClimaxDismiss.onClick.AddListener(DismissClimax);
            }
        }

        public void SetupSprites(
            Sprite greyBall, Sprite rainbowBall, Sprite rainbowAura,
            Sprite sunburst, Sprite sparkle, Sprite[] avatars, Sprite specialCutout,
            Sprite goldBall = null, Sprite gachaMachine = null, Sprite silverCoin = null, Sprite goldCoin = null)
        {
            greyBallSprite = greyBall;
            rainbowBallSprite = rainbowBall;
            rainbowAuraSprite = rainbowAura;
            sunburstSprite = sunburst;
            sparkleStarSprite = sparkle;
            mascotAvatars = avatars;
            specialMascotCutout = specialCutout;

            if (goldBall != null) goldBallSprite = goldBall;
            if (gachaMachine != null) gachaMachineSprite = gachaMachine;
            if (silverCoin != null) silverCoinSprite = silverCoin;
            if (goldCoin != null) goldCoinSprite = goldCoin;
        }

        public void SetupReferences(
            GameObject root, RectTransform container, TMP_Text instrTxt,
            Button openAll, Button confirm,
            GameObject climax, Image sunburst, Image mascotImg,
            TMP_Text cTitle, TMP_Text cSub, Button cDismiss)
        {
            modalRoot = root;
            ballsContainer = container;
            instructionText = instrTxt;
            btnOpenAll = openAll;
            btnConfirm = confirm;
            climaxOverlay = climax;
            climaxSunburst = sunburst;
            climaxMascotImage = mascotImg;
            climaxTitleText = cTitle;
            climaxSubText = cSub;
            btnClimaxDismiss = cDismiss;

            if (modalRoot != null) modalRoot.SetActive(false);
            if (climaxOverlay != null) climaxOverlay.SetActive(false);
        }

        public Sprite GetBallSpriteForDrop(GachaDropItem drop)
        {
            if (drop.isSpecial || drop.mascotIndex >= 8)
            {
                return rainbowBallSprite;
            }
            if (drop.mascotIndex >= 4) // Rare (Mascots 4~7: Blue, Berry, Lemon, Cloud)
            {
                return (goldBallSprite != null) ? goldBallSprite : rainbowBallSprite;
            }
            // Common (Mascots 0~3)
            return greyBallSprite;
        }

        private void EnsureDynamicUI()
        {
            Transform contentRoot = (modalRoot != null) ? modalRoot.transform.Find("ContentRoot") : transform;
            if (contentRoot == null) contentRoot = (modalRoot != null) ? modalRoot.transform : transform;

            // 1. Gacha Machine Root
            if (_machineRoot == null)
            {
                _machineRoot = new GameObject("GachaMachineRoot", typeof(RectTransform));
                _machineRoot.transform.SetParent(contentRoot, false);
                RectTransform mrt = _machineRoot.GetComponent<RectTransform>();
                mrt.anchorMin = new Vector2(0.5f, 0.5f);
                mrt.anchorMax = new Vector2(0.5f, 0.5f);
                mrt.anchoredPosition = new Vector2(0f, 30f);
                mrt.sizeDelta = new Vector2(560f, 560f);

                GameObject mImgObj = new GameObject("MachineImage", typeof(RectTransform), typeof(Image));
                mImgObj.transform.SetParent(_machineRoot.transform, false);
                RectTransform mirt = mImgObj.GetComponent<RectTransform>();
                mirt.anchorMin = Vector2.zero;
                mirt.anchorMax = Vector2.one;
                mirt.sizeDelta = Vector2.zero;
                _machineImg = mImgObj.GetComponent<Image>();
                _machineImg.sprite = gachaMachineSprite;
                _machineImg.preserveAspect = true;
                _machineImg.raycastTarget = false;

                // Coin Image (animated)
                _coinObj = new GameObject("AnimatedCoin", typeof(RectTransform), typeof(Image));
                _coinObj.transform.SetParent(_machineRoot.transform, false);
                RectTransform crt = _coinObj.GetComponent<RectTransform>();
                crt.anchorMin = new Vector2(0.5f, 0.5f);
                crt.anchorMax = new Vector2(0.5f, 0.5f);
                crt.anchoredPosition = new Vector2(0f, 280f);
                crt.sizeDelta = new Vector2(100f, 100f);
                _coinImg = _coinObj.GetComponent<Image>();
                _coinImg.preserveAspect = true;
                _coinImg.raycastTarget = false;
                _coinObj.SetActive(false);

                // Tray Balls Container (for dropping balls)
                _trayRoot = new GameObject("TrayBalls", typeof(RectTransform));
                _trayRoot.transform.SetParent(_machineRoot.transform, false);
                RectTransform trt = _trayRoot.GetComponent<RectTransform>();
                trt.anchorMin = new Vector2(0.5f, 0.5f);
                trt.anchorMax = new Vector2(0.5f, 0.5f);
                trt.anchoredPosition = new Vector2(100f, -170f);
                trt.sizeDelta = new Vector2(200f, 120f);
            }

            // 2. Single Reveal Stage Root
            if (_singleRevealRoot == null)
            {
                _singleRevealRoot = new GameObject("SingleRevealStage", typeof(RectTransform));
                _singleRevealRoot.transform.SetParent(contentRoot, false);
                RectTransform srt = _singleRevealRoot.GetComponent<RectTransform>();
                srt.anchorMin = Vector2.zero;
                srt.anchorMax = Vector2.one;
                srt.sizeDelta = Vector2.zero;

                // Full screen transparent tap button to advance
                GameObject tapBtnObj = new GameObject("FullScreenTapBtn", typeof(RectTransform), typeof(Image), typeof(Button));
                tapBtnObj.transform.SetParent(_singleRevealRoot.transform, false);
                RectTransform tprt = tapBtnObj.GetComponent<RectTransform>();
                tprt.anchorMin = Vector2.zero;
                tprt.anchorMax = Vector2.one;
                tprt.sizeDelta = Vector2.zero;
                Image tpImg = tapBtnObj.GetComponent<Image>();
                tpImg.color = Color.clear;
                tpImg.raycastTarget = true;
                _fullScreenTapBtn = tapBtnObj.GetComponent<Button>();
                _fullScreenTapBtn.onClick.AddListener(OnScreenTappedToAdvance);

                // Single Ball Object
                GameObject sBallObj = new GameObject("SingleBall", typeof(RectTransform), typeof(Image), typeof(Button));
                sBallObj.transform.SetParent(_singleRevealRoot.transform, false);
                _singleBallRect = sBallObj.GetComponent<RectTransform>();
                _singleBallRect.anchorMin = new Vector2(0.5f, 0.5f);
                _singleBallRect.anchorMax = new Vector2(0.5f, 0.5f);
                _singleBallRect.anchoredPosition = new Vector2(0, 40);
                _singleBallRect.sizeDelta = new Vector2(250, 250);
                _singleBallImg = sBallObj.GetComponent<Image>();
                _singleBallImg.preserveAspect = true;
                Button sBallBtn = sBallObj.GetComponent<Button>();
                sBallBtn.onClick.AddListener(OnScreenTappedToAdvance);

                // Aura Glow
                GameObject sAuraObj = new GameObject("SingleAura", typeof(RectTransform), typeof(Image));
                sAuraObj.transform.SetParent(sBallObj.transform, false);
                RectTransform sart = sAuraObj.GetComponent<RectTransform>();
                sart.anchorMin = Vector2.zero;
                sart.anchorMax = Vector2.one;
                sart.sizeDelta = new Vector2(100, 100);
                _singleAuraImg = sAuraObj.GetComponent<Image>();
                _singleAuraImg.sprite = rainbowAuraSprite;
                _singleAuraImg.raycastTarget = false;

                // Reward Container
                _singleRewardRoot = new GameObject("RewardRoot", typeof(RectTransform));
                _singleRewardRoot.transform.SetParent(_singleRevealRoot.transform, false);
                RectTransform rwrt = _singleRewardRoot.GetComponent<RectTransform>();
                rwrt.anchorMin = new Vector2(0.5f, 0.5f);
                rwrt.anchorMax = new Vector2(0.5f, 0.5f);
                rwrt.anchoredPosition = new Vector2(0, 40);
                rwrt.sizeDelta = new Vector2(400, 400);

                // Avatar
                GameObject avObj = new GameObject("RewardAvatar", typeof(RectTransform), typeof(Image));
                avObj.transform.SetParent(_singleRewardRoot.transform, false);
                RectTransform avrt = avObj.GetComponent<RectTransform>();
                avrt.anchorMin = new Vector2(0.5f, 0.5f);
                avrt.anchorMax = new Vector2(0.5f, 0.5f);
                avrt.anchoredPosition = new Vector2(0, 30);
                avrt.sizeDelta = new Vector2(230, 230);
                _singleAvatarImg = avObj.GetComponent<Image>();
                _singleAvatarImg.preserveAspect = true;
                _singleAvatarImg.raycastTarget = false;

                // Rarity Tag
                GameObject rarObj = new GameObject("RarityTag", typeof(RectTransform), typeof(TextMeshProUGUI));
                rarObj.transform.SetParent(_singleRewardRoot.transform, false);
                RectTransform rart = rarObj.GetComponent<RectTransform>();
                rart.anchorMin = new Vector2(0.5f, 0.5f);
                rart.anchorMax = new Vector2(0.5f, 0.5f);
                rart.anchoredPosition = new Vector2(0, -115);
                rart.sizeDelta = new Vector2(300, 36);
                _singleRarityText = rarObj.GetComponent<TMP_Text>();
                _singleRarityText.fontSize = 24;
                _singleRarityText.alignment = TextAlignmentOptions.Center;
                _singleRarityText.fontStyle = FontStyles.Bold;

                // Mascot Name
                GameObject nameObj = new GameObject("MascotName", typeof(RectTransform), typeof(TextMeshProUGUI));
                nameObj.transform.SetParent(_singleRewardRoot.transform, false);
                RectTransform nrt = nameObj.GetComponent<RectTransform>();
                nrt.anchorMin = new Vector2(0.5f, 0.5f);
                nrt.anchorMax = new Vector2(0.5f, 0.5f);
                nrt.anchoredPosition = new Vector2(0, -155);
                nrt.sizeDelta = new Vector2(400, 44);
                _singleNameText = nameObj.GetComponent<TMP_Text>();
                _singleNameText.fontSize = 32;
                _singleNameText.alignment = TextAlignmentOptions.Center;
                _singleNameText.fontStyle = FontStyles.Bold;
                _singleNameText.color = Color.white;

                // Shards Count
                GameObject shObj = new GameObject("ShardsText", typeof(RectTransform), typeof(TextMeshProUGUI));
                shObj.transform.SetParent(_singleRewardRoot.transform, false);
                RectTransform shrt = shObj.GetComponent<RectTransform>();
                shrt.anchorMin = new Vector2(0.5f, 0.5f);
                shrt.anchorMax = new Vector2(0.5f, 0.5f);
                shrt.anchoredPosition = new Vector2(0, -200);
                shrt.sizeDelta = new Vector2(400, 40);
                _singleShardsText = shObj.GetComponent<TMP_Text>();
                _singleShardsText.fontSize = 26;
                _singleShardsText.alignment = TextAlignmentOptions.Center;
                _singleShardsText.fontStyle = FontStyles.Bold;

                if (instructionText != null && instructionText.font != null)
                {
                    _singleRarityText.font = instructionText.font;
                    _singleNameText.font = instructionText.font;
                    _singleShardsText.font = instructionText.font;
                }
            }

            _machineRoot.SetActive(false);
            _singleRevealRoot.SetActive(false);
        }

        public void StartGachaSequence(List<GachaDropItem> drops, Action onComplete)
        {
            if (drops == null || drops.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            _currentDrops = drops;
            _onCompleteCallback = onComplete;
            _isOpeningAll = false;
            _stepAdvanceRequested = false;

            EnsureDynamicUI();

            if (modalRoot == null) modalRoot = gameObject;
            gameObject.SetActive(true);
            modalRoot.SetActive(true);

            if (climaxOverlay != null) climaxOverlay.SetActive(false);
            if (btnConfirm != null) btnConfirm.gameObject.SetActive(false);
            if (btnOpenAll != null)
            {
                btnOpenAll.interactable = true;
                btnOpenAll.gameObject.SetActive(drops.Count > 1);
            }

            if (ballsContainer != null) ballsContainer.gameObject.SetActive(false);

            if (_currentSequenceCoroutine != null) StopCoroutine(_currentSequenceCoroutine);
            _currentSequenceCoroutine = StartCoroutine(PlayGachaFlowRoutine());
        }

        private IEnumerator PlayGachaFlowRoutine()
        {
            int count = _currentDrops.Count;
            bool isSingle = (count == 1);

            // -------------------------------------------------------------
            // STEP 1: Gacha Machine Presentation
            // -------------------------------------------------------------
            _machineRoot.SetActive(true);
            _machineRoot.transform.localScale = Vector3.one;
            _machineImg.sprite = gachaMachineSprite;

            // Clear tray
            foreach (Transform child in _trayRoot.transform)
            {
                Destroy(child.gameObject);
            }

            if (instructionText != null)
            {
                instructionText.text = isSingle ? "말랑이 1회 소환 시작!" : "말랑이 10회 연속 소환 시작!";
            }

            // Coin animation
            _coinObj.SetActive(true);
            _coinImg.sprite = isSingle ? silverCoinSprite : goldCoinSprite;
            RectTransform crt = _coinObj.GetComponent<RectTransform>();
            crt.anchoredPosition = new Vector2(0f, 320f);
            crt.localScale = Vector3.one * 1.5f;

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            // Coin drop into slot
            float coinDur = 0.5f;
            float el = 0f;
            Vector2 coinStart = new Vector2(0f, 320f);
            Vector2 coinSlot = new Vector2(25f, 50f);
            while (el < coinDur)
            {
                el += Time.deltaTime;
                float t = el / coinDur;
                float easedT = Mathf.SmoothStep(0f, 1f, t);
                crt.anchoredPosition = Vector2.Lerp(coinStart, coinSlot, easedT);
                crt.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one * 0.7f, t);
                crt.localRotation = Quaternion.Euler(0, 0, t * 360f);
                yield return null;
            }
            _coinObj.SetActive(false);

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            // Machine Dial Turn & Shake
            RectTransform mrt = _machineRoot.GetComponent<RectTransform>();
            float shakeDur = 0.35f;
            el = 0f;
            while (el < shakeDur)
            {
                el += Time.deltaTime;
                float sx = Mathf.Sin(el * 40f) * 6f;
                mrt.anchoredPosition = new Vector2(sx, 30f);
                yield return null;
            }
            mrt.anchoredPosition = new Vector2(0f, 30f);

            // Ball(s) drop down into tray
            if (isSingle)
            {
                yield return StartCoroutine(DropBallToTrayRoutine(_currentDrops[0], 0, 1));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    StartCoroutine(DropBallToTrayRoutine(_currentDrops[i], i, count));
                    yield return new WaitForSeconds(0.045f);
                }
                yield return new WaitForSeconds(0.45f);
            }

            // Dispenser sparkles & sound
            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            yield return new WaitForSeconds(0.25f);

            // Smoothly fade out machine
            float fadeDur = 0.25f;
            el = 0f;
            while (el < fadeDur)
            {
                el += Time.deltaTime;
                float scale = Mathf.Lerp(1.0f, 0.85f, el / fadeDur);
                _machineRoot.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            _machineRoot.SetActive(false);

            // -------------------------------------------------------------
            // STEP 2: One-By-One Reveal Phase
            // -------------------------------------------------------------
            _singleRevealRoot.SetActive(true);

            for (int i = 0; i < count; i++)
            {
                if (_isOpeningAll) break;

                var drop = _currentDrops[i];
                yield return StartCoroutine(PlaySingleBallRevealRoutine(drop, i, count));
            }

            // -------------------------------------------------------------
            // STEP 3: Summary Grid (10-Pull 5x2 or 1-Pull Centered)
            // -------------------------------------------------------------
            _singleRevealRoot.SetActive(false);
            if (ballsContainer != null)
            {
                ballsContainer.gameObject.SetActive(true);
                BuildSummaryGrid();
            }

            if (btnOpenAll != null) btnOpenAll.gameObject.SetActive(false);
            if (btnConfirm != null)
            {
                btnConfirm.gameObject.SetActive(true);
                btnConfirm.transform.localScale = Vector3.one;
            }

            if (instructionText != null)
            {
                instructionText.text = "소환이 완료되었습니다!";
            }
        }

        private IEnumerator DropBallToTrayRoutine(GachaDropItem drop, int index, int total)
        {
            GameObject ballObj = new GameObject($"TrayBall_{index}", typeof(RectTransform), typeof(Image));
            ballObj.transform.SetParent(_trayRoot.transform, false);
            RectTransform brt = ballObj.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);

            Image bimg = ballObj.GetComponent<Image>();
            bimg.sprite = GetBallSpriteForDrop(drop);
            bimg.preserveAspect = true;
            bimg.raycastTarget = false;

            float ballSize = (total == 1) ? 90f : 55f;
            brt.sizeDelta = new Vector2(ballSize, ballSize);

            Vector2 startPos = new Vector2(0f, 150f);
            float targetX = (total == 1) ? 0f : UnityEngine.Random.Range(-55f, 55f);
            float targetY = (total == 1) ? 0f : UnityEngine.Random.Range(-25f, 25f);
            Vector2 targetPos = new Vector2(targetX, targetY);

            float dur = 0.35f;
            float el = 0f;
            while (el < dur)
            {
                el += Time.deltaTime;
                float t = el / dur;
                // Bounce drop
                float y = Mathf.Lerp(startPos.y, targetPos.y, t);
                if (t < 0.7f)
                {
                    y = Mathf.Lerp(startPos.y, targetPos.y, (t / 0.7f) * (t / 0.7f));
                }
                else
                {
                    float bt = (t - 0.7f) / 0.3f;
                    y = targetPos.y + Mathf.Sin(bt * Mathf.PI) * 20f;
                }
                brt.anchoredPosition = new Vector2(Mathf.Lerp(startPos.x, targetPos.x, t), y);
                yield return null;
            }
            brt.anchoredPosition = targetPos;
        }

        private IEnumerator PlaySingleBallRevealRoutine(GachaDropItem drop, int index, int total)
        {
            _stepAdvanceRequested = false;

            // Setup unopened ball
            _singleRewardRoot.SetActive(false);
            _singleBallImg.gameObject.SetActive(true);
            _singleBallImg.sprite = GetBallSpriteForDrop(drop);

            bool isRareOrSpecial = drop.isSpecial || drop.mascotIndex >= 4;
            _singleAuraImg.gameObject.SetActive(isRareOrSpecial);
            if (isRareOrSpecial)
            {
                _singleAuraImg.color = drop.isSpecial ? Color.white : new Color(1f, 0.85f, 0.3f, 0.85f);
            }

            if (instructionText != null)
            {
                instructionText.text = (total > 1)
                    ? string.Format(LocalizationManager.Get("gacha_touch_to_open_fmt", "가챠볼을 터치하여 열어보세요! ({0}/{1})"), index + 1, total)
                    : LocalizationManager.Get("gacha_touch_to_open", "가챠볼을 터치하여 열어보세요!");
            }

            // Pop in ball
            _singleBallRect.localScale = Vector3.zero;
            float el = 0f;
            float popDur = 0.28f;
            while (el < popDur)
            {
                el += Time.deltaTime;
                float t = el / popDur;
                float s = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.18f;
                if (t > 0.8f) s = Mathf.Lerp(1.18f, 1f, (t - 0.8f) / 0.2f);
                _singleBallRect.localScale = Vector3.one * s;
                yield return null;
            }
            _singleBallRect.localScale = Vector3.one;

            // Wait for click to crack open
            while (!_stepAdvanceRequested && !_isOpeningAll)
            {
                yield return null;
            }
            _stepAdvanceRequested = false;

            if (_isOpeningAll) yield break;

            // Crack open animation
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            _singleBallImg.gameObject.SetActive(false);
            _singleAuraImg.gameObject.SetActive(false);
            _singleRewardRoot.SetActive(true);

            // Populate reward data
            int mIdx = drop.mascotIndex;
            Sprite avSp = (mascotAvatars != null && mIdx < mascotAvatars.Length) ? mascotAvatars[mIdx] : null;
            _singleAvatarImg.sprite = avSp;

            string mName = LocalizationManager.Get($"mascot_{mIdx}_name", (mIdx < LobbyManager.MascotNames.Length) ? LobbyManager.MascotNames[mIdx] : "말랑이");
            _singleNameText.text = mName;

            string badgeKey = drop.isSpecial ? "codex_badge_special" : (mIdx >= 4 ? "codex_badge_rare" : "codex_badge_common");
            string rarName = LocalizationManager.Get(badgeKey, (mIdx < RarityNames.Length) ? RarityNames[mIdx] : "일반");
            Color rarCol = (mIdx < RarityColors.Length) ? RarityColors[mIdx] : Color.white;
            _singleRarityText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(rarCol)}>【 {rarName} 】</color>";

            _singleShardsText.text = drop.isSpecial
                ? $"<color=#FFDF00>{LocalizationManager.Get("gacha_special_descended", "★ 스페셜 강림! ★")}</color>"
                : $"<color=#00E5FF>{string.Format(LocalizationManager.Get("gacha_shards_gained_fmt", "+{0} 조각 획득!"), drop.shardCount)}</color>";

            // Reward spring pop
            RectTransform rwrt = _singleRewardRoot.GetComponent<RectTransform>();
            rwrt.localScale = Vector3.zero;
            el = 0f;
            float rwDur = 0.26f;
            while (el < rwDur)
            {
                el += Time.deltaTime;
                float t = el / rwDur;
                float s = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.22f;
                if (t > 0.8f) s = Mathf.Lerp(1.22f, 1f, (t - 0.8f) / 0.2f);
                rwrt.localScale = Vector3.one * s;
                yield return null;
            }
            rwrt.localScale = Vector3.one;

            // Climax presentation if Special Mascot!
            if (drop.isSpecial)
            {
                yield return StartCoroutine(PlaySpecialMascotClimaxRoutine(drop));
            }

            if (instructionText != null)
            {
                instructionText.text = (total > 1 && index < total - 1)
                    ? string.Format(LocalizationManager.Get("gacha_touch_for_next_fmt", "화면을 터치하여 다음으로 ({0}/{1})"), index + 1, total)
                    : LocalizationManager.Get("gacha_touch_to_continue", "화면을 터치하여 계속하기");
            }

            // Wait for user click to advance to next ball
            while (!_stepAdvanceRequested && !_isOpeningAll)
            {
                yield return null;
            }
            _stepAdvanceRequested = false;
        }

        private void OnScreenTappedToAdvance()
        {
            _stepAdvanceRequested = true;
        }

        private void BuildSummaryGrid()
        {
            foreach (var s in _slots)
            {
                if (s.rootObj != null) Destroy(s.rootObj);
            }
            _slots.Clear();

            int count = _currentDrops.Count;
            if (count == 1)
            {
                CreateSummarySlot(0, _currentDrops[0], Vector2.zero, new Vector2(200, 200));
            }
            else
            {
                // 10-Pull: 5 columns x 2 rows
                float[] colX = new float[] { -320f, -160f, 0f, 160f, 320f };
                float[] rowY = new float[] { 110f, -110f };

                for (int i = 0; i < count && i < 10; i++)
                {
                    int col = i % 5;
                    int row = i / 5;
                    Vector2 pos = new Vector2(colX[col], rowY[row]);
                    CreateSummarySlot(i, _currentDrops[i], pos, new Vector2(135, 135));
                }
            }
        }

        private void CreateSummarySlot(int index, GachaDropItem drop, Vector2 anchoredPos, Vector2 size)
        {
            GameObject slotObj = new GameObject($"SummarySlot_{index}", typeof(RectTransform));
            slotObj.transform.SetParent(ballsContainer, false);
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            // Reward avatar icon
            Sprite avSprite = (mascotAvatars != null && drop.mascotIndex < mascotAvatars.Length)
                ? mascotAvatars[drop.mascotIndex] : null;
            GameObject avObj = new GameObject("RewardAvatar", typeof(RectTransform), typeof(Image));
            avObj.transform.SetParent(slotObj.transform, false);
            RectTransform avrt = avObj.GetComponent<RectTransform>();
            avrt.anchorMin = new Vector2(0.5f, 0.5f);
            avrt.anchorMax = new Vector2(0.5f, 0.5f);
            avrt.anchoredPosition = new Vector2(0, 10);
            avrt.sizeDelta = new Vector2(size.x * 0.85f, size.y * 0.85f);
            Image avImg = avObj.GetComponent<Image>();
            avImg.sprite = avSprite;
            avImg.preserveAspect = true;
            avImg.raycastTarget = false;

            // Rarity / Shard badge text
            GameObject txtObj = new GameObject("RewardText", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtObj.transform.SetParent(slotObj.transform, false);
            RectTransform trt = txtObj.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0.5f, 0f);
            trt.anchorMax = new Vector2(0.5f, 0f);
            trt.anchoredPosition = new Vector2(0, -14);
            trt.sizeDelta = new Vector2(size.x * 1.3f, 30);
            TMP_Text rText = txtObj.GetComponent<TMP_Text>();
            if (instructionText != null && instructionText.font != null) rText.font = instructionText.font;
            rText.fontSize = (_currentDrops.Count == 1) ? 24 : 18;
            rText.alignment = TextAlignmentOptions.Center;
            string badgeKey = drop.isSpecial ? "codex_badge_special" : (drop.mascotIndex >= 4 ? "codex_badge_rare" : "codex_badge_common");
            string badgeText = LocalizationManager.Get(badgeKey, "스페셜");
            string shardSuffix = LocalizationManager.Get("mascot_detail_shards", "조각");
            rText.text = drop.isSpecial ? $"<color=#FFDF00>★{badgeText}!★</color>" : $"<color=#00E5FF>+{drop.shardCount} {shardSuffix}</color>";

            var slot = new GachaBallSlot
            {
                rootObj = slotObj,
                rootRect = rt,
                rewardRoot = slotObj,
                rewardAvatar = avImg,
                rewardText = rText,
                isOpened = true,
                dropData = drop
            };
            _slots.Add(slot);
        }

        private IEnumerator PlaySpecialMascotClimaxRoutine(GachaDropItem drop)
        {
            if (climaxOverlay == null) yield break;

            climaxOverlay.SetActive(true);

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            if (climaxMascotImage != null && specialMascotCutout != null)
            {
                climaxMascotImage.sprite = specialMascotCutout;
                climaxMascotImage.preserveAspect = true;
            }

            if (climaxTitleText != null)
            {
                climaxTitleText.text = LocalizationManager.Get("gacha_climax_title", "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>스페셜 말랑이 강림!</color></size>");
            }

            if (climaxSubText != null)
            {
                string touchCont = LocalizationManager.Get("gacha_touch_to_continue", "화면을 터치하여 계속하기");
                if (drop.isDuplicateSpecial)
                {
                    climaxSubText.text = $"<color=#00FFAA><b>{LocalizationManager.Get("summon_result_duplicate", "중복 획득!")} (+60)</b></color>\n<size=20><color=#E0D0FF>{touchCont}</color></size>";
                }
                else
                {
                    climaxSubText.text = $"<color=#FFF070><b>{LocalizationManager.Get("summon_result_unlocked", "획득 완료!")}</b></color>\n<size=20><color=#E0D0FF>{touchCont}</color></size>";
                }
            }

            RectTransform mrt = (climaxMascotImage != null) ? climaxMascotImage.GetComponent<RectTransform>() : null;
            if (mrt != null)
            {
                mrt.localScale = Vector3.zero;
                float el = 0f;
                float dur = 0.45f;
                while (el < dur)
                {
                    el += Time.deltaTime;
                    float t = el / dur;
                    float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.2f;
                    if (t > 0.8f) scale = Mathf.Lerp(1.2f, 1.0f, (t - 0.8f) / 0.2f);
                    mrt.localScale = Vector3.one * scale;
                    yield return null;
                }
                mrt.localScale = Vector3.one;
            }

            if (_climaxSunburstCoroutine != null) StopCoroutine(_climaxSunburstCoroutine);
            _climaxSunburstCoroutine = StartCoroutine(SpinSunburstRoutine());

            bool dismissed = false;
            if (btnClimaxDismiss != null)
            {
                btnClimaxDismiss.onClick.RemoveAllListeners();
                btnClimaxDismiss.onClick.AddListener(() => dismissed = true);
            }

            float timer = 0f;
            while (!dismissed && timer < 30f)
            {
                timer += Time.deltaTime;
                if (mrt != null)
                {
                    float bob = Mathf.Sin(Time.time * 5f) * 0.05f;
                    mrt.localScale = Vector3.one * (1.0f + bob);
                }
                yield return null;
            }

            DismissClimax();
        }

        private IEnumerator SpinSunburstRoutine()
        {
            if (climaxSunburst == null) yield break;
            while (climaxOverlay.activeSelf)
            {
                climaxSunburst.transform.Rotate(0, 0, -30f * Time.deltaTime);
                yield return null;
            }
        }

        private void DismissClimax()
        {
            if (climaxOverlay != null) climaxOverlay.SetActive(false);
        }

        private void OnOpenAllClicked()
        {
            if (_isOpeningAll) return;
            _isOpeningAll = true;
            _stepAdvanceRequested = true;
            if (btnOpenAll != null) btnOpenAll.gameObject.SetActive(false);
        }

        public void OpenSlot(int slotIndex)
        {
            _stepAdvanceRequested = true;
        }

        public void ShowClimaxDirect(GachaDropItem drop)
        {
            if (climaxOverlay != null)
            {
                StartCoroutine(PlaySpecialMascotClimaxRoutine(drop));
            }
        }

        public bool IsActive => (modalRoot != null && modalRoot.activeSelf) || (climaxOverlay != null && climaxOverlay.activeSelf);

        public void CloseModal()
        {
            if (modalRoot != null) modalRoot.SetActive(false);
            if (climaxOverlay != null) climaxOverlay.SetActive(false);
        }

        private void OnConfirmClicked()
        {
            CloseModal();
            _onCompleteCallback?.Invoke();
        }
    }
}
