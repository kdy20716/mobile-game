using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlockBlast
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        private const string KEY_NICKNAME = "Mallang_Nickname";
        private const string KEY_BIO = "Mallang_Bio";
        private const string KEY_AVATAR = "Mallang_Avatar";
        private const string KEY_COINS = "Mallang_Coins";
        private const string KEY_LOGGED_IN = "Mallang_LoggedIn";

        [Header("Lobby Root & Groups")]
        [SerializeField] private GameObject lobbyRoot;
        [SerializeField] private CanvasGroup lobbyCanvasGroup;

        [Header("Top Right Profile & Coins")]
        [SerializeField] private Button profileBtn;
        [SerializeField] private Image profileBtnAvatar;
        [SerializeField] private TMP_Text lobbyCoinsText;

        [Header("Lobby Mascots & Floating Labels (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private RectTransform[] partyMascots;
        [SerializeField] private GameObject[] partyLabels;
        [SerializeField] private Image[] partyLabelBgs;
        [SerializeField] private TMP_Text[] partyLabelTexts;
        [SerializeField] private GameObject[] partyGlowAuras;

        [Header("Bottom Action Button")]
        [SerializeField] private Button btnPlayGame;
        [SerializeField] private Image btnPlayGameBg;
        [SerializeField] private TMP_Text btnPlayGameText;

        [Header("Profile Modal")]
        [SerializeField] private GameObject profileModal;
        [SerializeField] private Image profileModalAvatar;
        [SerializeField] private TMP_Text profileModalNickname;
        [SerializeField] private TMP_InputField profileModalBioInput;
        [SerializeField] private TMP_Text profileModalBestScore;
        [SerializeField] private TMP_Text profileModalCoins;
        [SerializeField] private Button[] avatarSelectButtons;
        [SerializeField] private Button btnLogout;
        [SerializeField] private Button btnCloseProfile;

        [Header("Shop Modal")]
        [SerializeField] private GameObject shopModal;
        [SerializeField] private TMP_Text shopCoinsText;
        [SerializeField] private Button btnCloseShop;

        [Header("Settings Modal")]
        [SerializeField] private GameObject settingsModal;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button btnCloseSettings;

        [Header("Help Modal")]
        [SerializeField] private GameObject helpModal;
        [SerializeField] private Button btnCloseHelp;

        [Header("Mascot Avatars (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private Sprite[] mascotAvatars;

        private int _currentAvatarIdx = 0;
        private string _currentNickname = "";
        private string _currentBio = "";
        private int _currentCoins = 1000;
        private bool _isLoggedIn = true;

        private int _selectedMenuIdx = 0;
        private Coroutine _mascotBounceCoroutine;

        // Button Colors & Texts matching mascots
        private static readonly Color ColorPink = new Color(1f, 0.33f, 0.53f, 1f);     // #FF5588
        private static readonly Color ColorMint = new Color(0.17f, 0.83f, 0.64f, 1f);   // #2CD4A4
        private static readonly Color ColorGold = new Color(1f, 0.70f, 0.0f, 1f);       // #FFB300
        private static readonly Color ColorPurple = new Color(0.66f, 0.33f, 0.97f, 1f);  // #A855F7

        private static readonly string[] MenuActionTexts = new string[]
        {
            "게임 시작!",
            "상점 가기!",
            "설정 열기!",
            "도움말 보기!"
        };

        private static readonly Color[] MenuColors = new Color[]
        {
            ColorPink,
            ColorMint,
            ColorGold,
            ColorPurple
        };

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            PerformAutoLogin();
        }

        private void Start()
        {
            SetupEventListeners();
            RefreshProfileUI();
            SelectMenu(0, false); // Default: Game Start (Pink Mascot)

            if (profileModal != null) profileModal.SetActive(false);
            if (shopModal != null) shopModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);

            if (_mascotBounceCoroutine != null) StopCoroutine(_mascotBounceCoroutine);
            _mascotBounceCoroutine = StartCoroutine(MascotIdleBounceRoutine());
        }

        // ==========================================
        // AUTO-LOGIN & ACCOUNT DATA
        // ==========================================

        private void PerformAutoLogin()
        {
            if (!PlayerPrefs.HasKey(KEY_LOGGED_IN) || PlayerPrefs.GetInt(KEY_LOGGED_IN, 0) == 0)
            {
                _currentNickname = $"말랑이#{Random.Range(1000, 9999)}";
                _currentBio = "말랑블라스트에 오신 걸 환영해요!";
                _currentAvatarIdx = 0; // Pink Mascot default
                _currentCoins = 1000;  // 1000 welcome coins
                _isLoggedIn = true;

                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.SetString(KEY_BIO, _currentBio);
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
                PlayerPrefs.Save();
            }
            else
            {
                _currentNickname = PlayerPrefs.GetString(KEY_NICKNAME, $"말랑이#{Random.Range(1000, 9999)}");
                _currentBio = PlayerPrefs.GetString(KEY_BIO, "말랑블라스트에 오신 걸 환영해요!");
                _currentAvatarIdx = PlayerPrefs.GetInt(KEY_AVATAR, 0);
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 1000);
                _isLoggedIn = PlayerPrefs.GetInt(KEY_LOGGED_IN, 1) == 1;
            }
        }

        private void SetupEventListeners()
        {
            // Profile Button
            if (profileBtn != null) profileBtn.onClick.AddListener(() => { PlayClickSound(); OpenProfileModal(); });

            // Bottom Action Button
            if (btnPlayGame != null) btnPlayGame.onClick.AddListener(() => { PlayClickSound(); ExecuteSelectedMenuAction(); });

            // Profile Modal
            if (btnCloseProfile != null) btnCloseProfile.onClick.AddListener(() => { PlayClickSound(); CloseProfileModal(); });
            if (profileModalBioInput != null)
            {
                profileModalBioInput.onEndEdit.AddListener(OnBioEndEdit);
            }
            if (btnLogout != null) btnLogout.onClick.AddListener(OnLogoutClicked);

            // Avatar Select Buttons
            if (avatarSelectButtons != null)
            {
                for (int i = 0; i < avatarSelectButtons.Length; i++)
                {
                    int idx = i;
                    if (avatarSelectButtons[i] != null)
                    {
                        avatarSelectButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectAvatar(idx); });
                    }
                }
            }

            // Shop Modal
            if (btnCloseShop != null) btnCloseShop.onClick.AddListener(() => { PlayClickSound(); CloseShopModal(); });

            // Settings Modal
            if (btnCloseSettings != null) btnCloseSettings.onClick.AddListener(() => { PlayClickSound(); CloseSettingsModal(); });
            if (bgmSlider != null)
            {
                bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 0.7f);
                bgmSlider.onValueChanged.AddListener((v) =>
                {
                    PlayerPrefs.SetFloat("BGM_Volume", v);
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.bgmVolume = v;
                });
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 0.85f);
                sfxSlider.onValueChanged.AddListener((v) =>
                {
                    PlayerPrefs.SetFloat("SFX_Volume", v);
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.sfxVolume = v;
                });
            }

            // Help Modal
            if (btnCloseHelp != null) btnCloseHelp.onClick.AddListener(() => { PlayClickSound(); CloseHelpModal(); });

            // Party Mascot & Floating Label Clicks
            if (partyMascots != null)
            {
                for (int i = 0; i < partyMascots.Length; i++)
                {
                    int idx = i;
                    Button btn = partyMascots[i].GetComponent<Button>();
                    if (btn == null) btn = partyMascots[i].gameObject.AddComponent<Button>();
                    btn.transition = Selectable.Transition.None;
                    btn.onClick.AddListener(() =>
                    {
                        SelectMenu(idx, true);
                    });

                    // Also wire up any button on children (like mascot image)
                    Button[] childBtns = partyMascots[i].GetComponentsInChildren<Button>(true);
                    foreach (var cBtn in childBtns)
                    {
                        if (cBtn != btn)
                        {
                            cBtn.transition = Selectable.Transition.None;
                            cBtn.onClick.AddListener(() =>
                            {
                                SelectMenu(idx, true);
                            });
                        }
                    }
                }
            }

            if (partyLabels != null)
            {
                for (int i = 0; i < partyLabels.Length; i++)
                {
                    int idx = i;
                    if (partyLabels[i] != null)
                    {
                        Button lBtn = partyLabels[i].GetComponent<Button>();
                        if (lBtn == null) lBtn = partyLabels[i].AddComponent<Button>();
                        lBtn.transition = Selectable.Transition.None;
                        lBtn.onClick.AddListener(() =>
                        {
                            SelectMenu(idx, true);
                        });
                    }
                }
            }
        }

        // ==========================================
        // MENU SELECTION & BOTTOM BUTTON SYNC
        // ==========================================

        public void SelectMenu(int index, bool triggerActionIfAlreadySelected = false)
        {
            if (index < 0 || index >= MenuActionTexts.Length) return;

            // If already selected and user clicked again, trigger action immediately!
            if (_selectedMenuIdx == index && triggerActionIfAlreadySelected)
            {
                ExecuteSelectedMenuAction();
                return;
            }

            _selectedMenuIdx = index;

            // Update Bottom Button Text & Color
            if (btnPlayGameText != null)
            {
                btnPlayGameText.text = MenuActionTexts[index];
            }

            if (btnPlayGameBg != null)
            {
                btnPlayGameBg.color = MenuColors[index];
            }
            else if (btnPlayGame != null)
            {
                var img = btnPlayGame.GetComponent<Image>();
                if (img != null) img.color = MenuColors[index];
            }

            // Update Glow Auras behind characters (light radiates from behind selected mascot!)
            if (partyGlowAuras != null)
            {
                for (int i = 0; i < partyGlowAuras.Length; i++)
                {
                    if (partyGlowAuras[i] != null)
                    {
                        partyGlowAuras[i].SetActive(i == index);
                    }
                }
            }

            // Update Floating Labels Highlight
            if (partyLabels != null)
            {
                for (int i = 0; i < partyLabels.Length; i++)
                {
                    bool isSel = (i == index);
                    if (partyLabelBgs != null && i < partyLabelBgs.Length && partyLabelBgs[i] != null)
                    {
                        partyLabelBgs[i].color = isSel ? Color.white : new Color(1f, 1f, 1f, 0.7f);
                    }
                    if (partyLabelTexts != null && i < partyLabelTexts.Length && partyLabelTexts[i] != null)
                    {
                        partyLabelTexts[i].color = isSel ? MenuColors[i] : new Color(0.35f, 0.25f, 0.45f, 0.9f);
                    }
                }
            }

            // Punch animation on chosen mascot
            if (partyMascots != null && index < partyMascots.Length && partyMascots[index] != null)
            {
                StartCoroutine(PunchMascot(partyMascots[index]));
            }

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayPickup();
            }
        }

        private void ExecuteSelectedMenuAction()
        {
            switch (_selectedMenuIdx)
            {
                case 0:
                    StartGameFromLobby();
                    break;
                case 1:
                    OpenShopModal();
                    break;
                case 2:
                    OpenSettingsModal();
                    break;
                case 3:
                    OpenHelpModal();
                    break;
            }
        }

        // ==========================================
        // PROFILE MODAL
        // ==========================================

        public void OpenProfileModal()
        {
            if (profileModal != null)
            {
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshProfileUI();
                profileModal.SetActive(true);
            }
        }

        public void CloseProfileModal()
        {
            if (profileModal != null) profileModal.SetActive(false);
        }

        private void SelectAvatar(int idx)
        {
            if (mascotAvatars != null && idx >= 0 && idx < mascotAvatars.Length)
            {
                _currentAvatarIdx = idx;
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
        }

        private void OnBioEndEdit(string newBio)
        {
            _currentBio = newBio.Trim();
            PlayerPrefs.SetString(KEY_BIO, _currentBio);
            PlayerPrefs.Save();
        }

        private void OnLogoutClicked()
        {
            PlayClickSound();
            if (_isLoggedIn)
            {
                _isLoggedIn = false;
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 0);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
            else
            {
                _isLoggedIn = true;
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
        }

        private void RefreshProfileUI()
        {
            Sprite avatarSprite = (mascotAvatars != null && _currentAvatarIdx < mascotAvatars.Length)
                ? mascotAvatars[_currentAvatarIdx]
                : null;

            if (profileBtnAvatar != null && avatarSprite != null)
            {
                profileBtnAvatar.sprite = avatarSprite;
                profileBtnAvatar.color = _isLoggedIn ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.7f);
            }

            if (lobbyCoinsText != null)
            {
                lobbyCoinsText.text = $"{_currentCoins:N0} C";
            }

            if (profileModalAvatar != null && avatarSprite != null)
            {
                profileModalAvatar.sprite = avatarSprite;
            }

            if (profileModalNickname != null)
            {
                profileModalNickname.text = _isLoggedIn ? _currentNickname : "로그인이 필요합니다";
            }

            if (profileModalBioInput != null)
            {
                profileModalBioInput.text = _currentBio;
                profileModalBioInput.interactable = _isLoggedIn;
            }

            if (profileModalBestScore != null)
            {
                int best = PlayerPrefs.GetInt("BlockBlast_Best", 0);
                profileModalBestScore.text = $"최고 점수: {best:N0}점";
            }

            if (profileModalCoins != null)
            {
                profileModalCoins.text = $"보유 코인: {_currentCoins:N0} C";
            }

            if (btnLogout != null)
            {
                TMP_Text btnText = btnLogout.GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    btnText.text = _isLoggedIn ? "로그아웃" : "게스트로 로그인";
                }
            }
        }

        // ==========================================
        // SHOP MODAL
        // ==========================================

        public void OpenShopModal()
        {
            if (shopModal != null)
            {
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (shopCoinsText != null) shopCoinsText.text = $"내 코인: {_currentCoins:N0} C";
                shopModal.SetActive(true);
            }
        }

        public void CloseShopModal()
        {
            if (shopModal != null) shopModal.SetActive(false);
        }

        // ==========================================
        // SETTINGS MODAL
        // ==========================================

        public void OpenSettingsModal()
        {
            if (settingsModal != null)
            {
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                settingsModal.SetActive(true);
            }
        }

        public void CloseSettingsModal()
        {
            if (settingsModal != null) settingsModal.SetActive(false);
        }

        // ==========================================
        // HELP MODAL
        // ==========================================

        public void OpenHelpModal()
        {
            if (helpModal != null)
            {
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                helpModal.SetActive(true);
            }
        }

        public void CloseHelpModal()
        {
            if (helpModal != null) helpModal.SetActive(false);
        }

        // ==========================================
        // SCENE TRANSITIONS
        // ==========================================

        public void ShowLobby()
        {
            if (lobbyRoot != null) lobbyRoot.SetActive(true);
            if (lobbyCanvasGroup != null) lobbyCanvasGroup.alpha = 1f;
            SelectMenu(0, false);
            RefreshProfileUI();
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowInGameUI(false);
            }
        }

        public void StartGameFromLobby()
        {
            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.DoTransition(() =>
                {
                    if (lobbyRoot != null) lobbyRoot.SetActive(false);
                    if (BlockBlastUIManager.Instance != null)
                    {
                        BlockBlastUIManager.Instance.ShowInGameUI(true);
                        BlockBlastUIManager.Instance.StartGameFromMenu();
                    }
                });
            }
            else
            {
                StartCoroutine(TransitionToGameRoutine());
            }
        }

        private IEnumerator TransitionToGameRoutine()
        {
            yield return StartCoroutine(FadeCanvasGroup(lobbyCanvasGroup, 1f, 0f, 0.25f));
            if (lobbyRoot != null) lobbyRoot.SetActive(false);

            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowInGameUI(true);
                BlockBlastUIManager.Instance.StartGameFromMenu();
            }
        }

        public void ReturnToLobby()
        {
            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.DoTransition(() =>
                {
                    if (BlockBlastUIManager.Instance != null)
                    {
                        BlockBlastUIManager.Instance.RestartGame();
                    }
                    ShowLobby();
                });
            }
            else
            {
                if (BlockBlastUIManager.Instance != null)
                {
                    BlockBlastUIManager.Instance.RestartGame();
                }
                ShowLobby();
            }
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
        {
            if (cg == null) yield break;
            float elapsed = 0f;
            cg.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            cg.alpha = to;
        }

        // ==========================================
        // PARTY MASCOT IDLE ANIMATION
        // ==========================================

        private IEnumerator MascotIdleBounceRoutine()
        {
            if (partyMascots == null || partyMascots.Length == 0) yield break;

            Vector2[] origPositions = new Vector2[partyMascots.Length];
            for (int i = 0; i < partyMascots.Length; i++)
            {
                if (partyMascots[i] != null) origPositions[i] = partyMascots[i].anchoredPosition;
            }

            while (true)
            {
                float time = Time.time;
                for (int i = 0; i < partyMascots.Length; i++)
                {
                    if (partyMascots[i] != null)
                    {
                        float offset = i * 0.72f;
                        float bounceY = Mathf.Abs(Mathf.Sin(time * 3.2f + offset)) * 14f;
                        float squashX = 1f + Mathf.Sin(time * 3.2f + offset) * 0.04f;
                        float squashY = 1f - Mathf.Sin(time * 3.2f + offset) * 0.04f;

                        partyMascots[i].anchoredPosition = origPositions[i] + new Vector2(0f, bounceY);
                        partyMascots[i].localScale = new Vector3(squashX, squashY, 1f);
                    }
                }
                yield return null;
            }
        }

        private IEnumerator PunchMascot(RectTransform mascotRT)
        {
            if (mascotRT == null) yield break;
            float elapsed = 0f;
            float duration = 0.28f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
                mascotRT.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            mascotRT.localScale = Vector3.one;
        }

        private void PlayClickSound()
        {
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayUIClick();
            }
        }

        // ==========================================
        // SETUP HELPER (Called by SceneBuilder)
        // ==========================================

        public void SetupReferences(
            GameObject root, CanvasGroup cg,
            Button pBtn, Image pAvatar, TMP_Text cText,
            RectTransform[] mascots, GameObject[] labels, Image[] labelBgs, TMP_Text[] labelTexts,
            Button playBtn, Image playBtnBg, TMP_Text playBtnText,
            GameObject pModal, Image pModalAv, TMP_Text pNick, TMP_InputField pBio, TMP_Text pBest, TMP_Text pCoin, Button[] avBtns, Button lBtn, Button clPBtn,
            GameObject sModal, TMP_Text sCoins, Button clSBtn,
            GameObject setModal, Slider bSlider, Slider sSlider, Button clSetBtn,
            GameObject hModal, Button clHBtn,
            Sprite[] avatars,
            GameObject[] glowAuras = null)
        {
            lobbyRoot = root;
            lobbyCanvasGroup = cg;

            profileBtn = pBtn;
            profileBtnAvatar = pAvatar;
            lobbyCoinsText = cText;

            partyMascots = mascots;
            partyLabels = labels;
            partyLabelBgs = labelBgs;
            partyLabelTexts = labelTexts;
            partyGlowAuras = glowAuras;

            btnPlayGame = playBtn;
            btnPlayGameBg = playBtnBg;
            btnPlayGameText = playBtnText;

            profileModal = pModal;
            profileModalAvatar = pModalAv;
            profileModalNickname = pNick;
            profileModalBioInput = pBio;
            profileModalBestScore = pBest;
            profileModalCoins = pCoin;
            avatarSelectButtons = avBtns;
            btnLogout = lBtn;
            btnCloseProfile = clPBtn;

            shopModal = sModal;
            shopCoinsText = sCoins;
            btnCloseShop = clSBtn;

            settingsModal = setModal;
            bgmSlider = bSlider;
            sfxSlider = sSlider;
            btnCloseSettings = clSetBtn;

            helpModal = hModal;
            btnCloseHelp = clHBtn;

            mascotAvatars = avatars;
        }
    }
}
