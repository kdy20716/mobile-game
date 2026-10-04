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
                    _instance = FindObjectOfType<GachaPresentationController>(true);
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
        [SerializeField] private Sprite rainbowBallSprite;
        [SerializeField] private Sprite rainbowAuraSprite;
        [SerializeField] private Sprite sunburstSprite;
        [SerializeField] private Sprite sparkleStarSprite;
        [SerializeField] private Sprite[] mascotAvatars;
        [SerializeField] private Sprite specialMascotCutout;

        [Header("Full Screen Climax Presentation")]
        [SerializeField] private GameObject climaxOverlay;
        [SerializeField] private Image climaxSunburst;
        [SerializeField] private Image climaxMascotImage;
        [SerializeField] private TMP_Text climaxTitleText;
        [SerializeField] private TMP_Text climaxSubText;
        [SerializeField] private Button btnClimaxDismiss;

        private List<GachaDropItem> _currentDrops = new List<GachaDropItem>();
        private List<GachaBallSlot> _slots = new List<GachaBallSlot>();
        private Action _onCompleteCallback;
        private bool _isOpeningAll = false;
        private Coroutine _climaxSunburstCoroutine;

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
            Sprite sunburst, Sprite sparkle, Sprite[] avatars, Sprite specialCutout)
        {
            greyBallSprite = greyBall;
            rainbowBallSprite = rainbowBall;
            rainbowAuraSprite = rainbowAura;
            sunburstSprite = sunburst;
            sparkleStarSprite = sparkle;
            mascotAvatars = avatars;
            specialMascotCutout = specialCutout;
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

            if (instructionText != null)
            {
                instructionText.text = (drops.Count == 1)
                    ? "가챠볼을 터치하여 열어보세요!"
                    : "10회 소환! 가챠볼을 터치해 하나씩 열어보세요!";
            }

            BuildBallSlots();
        }

        private void BuildBallSlots()
        {
            foreach (var slot in _slots)
            {
                if (slot.rootObj != null) Destroy(slot.rootObj);
            }
            _slots.Clear();

            int count = _currentDrops.Count;
            if (count == 1)
            {
                // Single Ball Centered
                CreateBallSlot(0, _currentDrops[0], Vector2.zero, new Vector2(180, 180));
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
                    CreateBallSlot(i, _currentDrops[i], pos, new Vector2(135, 135));
                }
            }
        }

        private void CreateBallSlot(int index, GachaDropItem drop, Vector2 anchoredPos, Vector2 size)
        {
            GameObject slotObj = new GameObject($"BallSlot_{index}", typeof(RectTransform));
            slotObj.transform.SetParent(ballsContainer, false);
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            // Aura glow for rainbow balls
            Image auraImg = null;
            if (drop.isSpecial && rainbowAuraSprite != null)
            {
                GameObject auraObj = new GameObject("AuraGlow", typeof(RectTransform), typeof(Image));
                auraObj.transform.SetParent(slotObj.transform, false);
                RectTransform art = auraObj.GetComponent<RectTransform>();
                art.anchorMin = Vector2.zero;
                art.anchorMax = Vector2.one;
                art.sizeDelta = new Vector2(size.x * 0.4f, size.y * 0.4f);
                auraImg = auraObj.GetComponent<Image>();
                auraImg.sprite = rainbowAuraSprite;
                auraImg.raycastTarget = false;
            }

            // Ball button & image
            GameObject ballImgObj = new GameObject("BallImage", typeof(RectTransform), typeof(Image), typeof(Button));
            ballImgObj.transform.SetParent(slotObj.transform, false);
            RectTransform brt = ballImgObj.GetComponent<RectTransform>();
            brt.anchorMin = Vector2.zero;
            brt.anchorMax = Vector2.one;
            brt.sizeDelta = Vector2.zero;
            Image ballImg = ballImgObj.GetComponent<Image>();
            ballImg.sprite = drop.isSpecial ? rainbowBallSprite : greyBallSprite;
            ballImg.preserveAspect = true;
            Button ballBtn = ballImgObj.GetComponent<Button>();

            // Reward container (revealed on open)
            GameObject rewardObj = new GameObject("RewardContainer", typeof(RectTransform));
            rewardObj.transform.SetParent(slotObj.transform, false);
            RectTransform rrt = rewardObj.GetComponent<RectTransform>();
            rrt.anchorMin = Vector2.zero;
            rrt.anchorMax = Vector2.one;
            rrt.sizeDelta = Vector2.zero;
            rewardObj.SetActive(false);

            // Reward avatar icon
            Sprite avSprite = (mascotAvatars != null && drop.mascotIndex < mascotAvatars.Length)
                ? mascotAvatars[drop.mascotIndex] : null;
            GameObject avObj = new GameObject("RewardAvatar", typeof(RectTransform), typeof(Image));
            avObj.transform.SetParent(rewardObj.transform, false);
            RectTransform avrt = avObj.GetComponent<RectTransform>();
            avrt.anchorMin = new Vector2(0.5f, 0.5f);
            avrt.anchorMax = new Vector2(0.5f, 0.5f);
            avrt.anchoredPosition = new Vector2(0, 10);
            avrt.sizeDelta = new Vector2(size.x * 0.85f, size.y * 0.85f);
            Image avImg = avObj.GetComponent<Image>();
            avImg.sprite = avSprite;
            avImg.preserveAspect = true;
            avImg.raycastTarget = false;

            // Reward badge text
            GameObject txtObj = new GameObject("RewardText", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtObj.transform.SetParent(rewardObj.transform, false);
            RectTransform trt = txtObj.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0.5f, 0f);
            trt.anchorMax = new Vector2(0.5f, 0f);
            trt.anchoredPosition = new Vector2(0, -14);
            trt.sizeDelta = new Vector2(size.x * 1.2f, 30);
            TMP_Text rText = txtObj.GetComponent<TMP_Text>();
            rText.fontSize = (_currentDrops != null && _currentDrops.Count == 1) ? 24 : 18;
            rText.alignment = TextAlignmentOptions.Center;
            rText.text = drop.isSpecial ? "<color=#FFDF00>★스페셜!★</color>" : $"<color=#00E5FF>+{drop.shardCount}조각</color>";

            var slot = new GachaBallSlot
            {
                rootObj = slotObj,
                rootRect = rt,
                button = ballBtn,
                ballImage = ballImg,
                auraImage = auraImg,
                rewardRoot = rewardObj,
                rewardAvatar = avImg,
                rewardText = rText,
                isOpened = false,
                dropData = drop
            };

            int slotIdx = index;
            ballBtn.onClick.AddListener(() => OpenSlot(slotIdx));

            _slots.Add(slot);

            // Pop in slot with scale animation
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PopInSlotRoutine(rt, index * 0.04f));
            }
            else
            {
                rt.localScale = Vector3.one;
            }
        }

        private IEnumerator PopInSlotRoutine(RectTransform rt, float delay)
        {
            rt.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);

            float el = 0f;
            float dur = 0.25f;
            while (el < dur)
            {
                el += Time.deltaTime;
                float t = el / dur;
                // Elastic bounce in
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.15f;
                if (t > 0.8f) scale = Mathf.Lerp(1.15f, 1f, (t - 0.8f) / 0.2f);
                rt.localScale = Vector3.one * scale;
                yield return null;
            }
            rt.localScale = Vector3.one;
        }

        private void Update()
        {
            // Subtle floating / pulsing for unopened rainbow aura
            if (_slots != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.12f;
                for (int i = 0; i < _slots.Count; i++)
                {
                    var s = _slots[i];
                    if (s != null && !s.isOpened && s.auraImage != null)
                    {
                        s.auraImage.transform.localScale = Vector3.one * pulse;
                        s.auraImage.transform.Rotate(0, 0, 45f * Time.deltaTime);
                    }
                }
            }
        }

        public void OpenSlot(int index)
        {
            if (index < 0 || index >= _slots.Count) return;
            var slot = _slots[index];
            if (slot.isOpened) return;

            slot.isOpened = true;
            slot.button.interactable = false;

            StartCoroutine(AnimateCrackBallRoutine(slot));
        }

        private IEnumerator AnimateCrackBallRoutine(GachaBallSlot slot)
        {
            // Shake ball
            Vector2 origPos = slot.rootRect.anchoredPosition;
            float shakeTime = 0.18f;
            float el = 0f;
            while (el < shakeTime)
            {
                el += Time.deltaTime;
                float ox = UnityEngine.Random.Range(-5f, 5f);
                float oy = UnityEngine.Random.Range(-5f, 5f);
                slot.rootRect.anchoredPosition = origPos + new Vector2(ox, oy);
                yield return null;
            }
            slot.rootRect.anchoredPosition = origPos;

            // Audio: crack / pop
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayBuy();
            }

            // Ball vanishes, reward reveals with spring bounce
            if (slot.auraImage != null) slot.auraImage.gameObject.SetActive(false);
            slot.ballImage.gameObject.SetActive(false);
            slot.rewardRoot.SetActive(true);

            RectTransform rrt = slot.rewardRoot.GetComponent<RectTransform>();
            rrt.localScale = Vector3.zero;
            el = 0f;
            float popDur = 0.22f;
            while (el < popDur)
            {
                el += Time.deltaTime;
                float t = el / popDur;
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.25f;
                if (t > 0.8f) scale = Mathf.Lerp(1.25f, 1.0f, (t - 0.8f) / 0.2f);
                rrt.localScale = Vector3.one * scale;
                yield return null;
            }
            rrt.localScale = Vector3.one;

            // Check if this was a SPECIAL MASCOT!
            if (slot.dropData.isSpecial)
            {
                yield return StartCoroutine(PlaySpecialMascotClimaxRoutine(slot.dropData));
            }

            CheckAllOpened();
        }

        public void ShowClimaxDirect(GachaDropItem drop)
        {
            if (modalRoot != null) modalRoot.SetActive(true);
            StartCoroutine(PlaySpecialMascotClimaxRoutine(drop));
        }

        private IEnumerator PlaySpecialMascotClimaxRoutine(GachaDropItem drop)
        {
            if (climaxOverlay == null) yield break;

            climaxOverlay.SetActive(true);

            // Audio: celebration
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayBuy();
            }

            if (climaxMascotImage != null && specialMascotCutout != null)
            {
                climaxMascotImage.sprite = specialMascotCutout;
                climaxMascotImage.preserveAspect = true;
            }

            if (climaxTitleText != null)
            {
                climaxTitleText.text = "<size=46><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=32><color=#FFFFFF>스페셜 말랑이 강림!</color></size>";
            }

            if (climaxSubText != null)
            {
                if (drop.isDuplicateSpecial)
                {
                    climaxSubText.text = "<color=#00FFAA><b>[중복 획득] 60조각 즉시 변환!</b></color>\n<size=20><color=#E0D0FF>화면을 터치하여 계속하기</color></size>";
                }
                else
                {
                    climaxSubText.text = "<color=#FFF070><b>신규 스페셜 말랑이 획득!</b></color>\n<size=20><color=#E0D0FF>화면을 터치하여 계속하기</color></size>";
                }
            }

            // Animate Giant Mascot Scaling In
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

            // Spin sunburst rays
            if (_climaxSunburstCoroutine != null) StopCoroutine(_climaxSunburstCoroutine);
            _climaxSunburstCoroutine = StartCoroutine(SpinSunburstRoutine());

            // Wait for user to dismiss or auto wait
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
                // Mascot cute gentle breathing/floating
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
            if (btnOpenAll != null) btnOpenAll.interactable = false;

            StartCoroutine(OpenAllRoutine());
        }

        private IEnumerator OpenAllRoutine()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (!_slots[i].isOpened)
                {
                    OpenSlot(i);
                    yield return new WaitForSeconds(0.08f);
                }
            }
            CheckAllOpened();
        }

        private void CheckAllOpened()
        {
            bool allDone = true;
            foreach (var slot in _slots)
            {
                if (!slot.isOpened)
                {
                    allDone = false;
                    break;
                }
            }

            if (allDone)
            {
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
