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
        public const string KEY_THEME_OWNED_PREFIX = "Mallang_Theme_Owned_";
        public const string KEY_EQUIPPED_THEME = "Mallang_EquippedTheme";
        public const string KEY_LOBBY_THEME_OWNED_PREFIX = "Mallang_LobbyTheme_Owned_";
        public const string KEY_EQUIPPED_LOBBY_THEME = "Mallang_EquippedLobbyTheme";

        public static readonly int[] ThemePrices = new int[] { 0, 300, 500, 700 };
        public static readonly string[] ThemeNames = new string[] { "몽환의 밤", "캔디 랜드", "크리스탈 바다", "별빛 우주" };

        public static readonly int[] LobbyThemePrices = new int[] { 0, 400, 600 };
        public static readonly string[] LobbyThemeNames = new string[] { "몽환의 방", "달콤 캔디룸", "신비 바다룸" };
        public static readonly string[] LobbyThemeDescs = new string[] { "기본 로비 - 아늑한 파스텔 방", "달콤한 디저트와 와플 무대", "신비로운 바다 궁전 무대" };

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
        [SerializeField] private Image btnPlayGameGlow;

        [Header("Profile Modal")]
        [SerializeField] private GameObject profileModal;
        [SerializeField] private Image profileModalAvatar;
        [SerializeField] private TMP_Text profileModalNickname;
        [SerializeField] private TMP_InputField profileModalNicknameInput;
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

        [Header("Theme Shop")]
        [SerializeField] private Image inGameBackgroundImg;
        [SerializeField] private Button[] themeActionButtons;
        [SerializeField] private TMP_Text[] themeActionTexts;
        [SerializeField] private TMP_Text[] themePriceTexts;
        [SerializeField] private Sprite[] themeSprites;

        [Header("Lobby Background & Themes")]
        [SerializeField] private Image lobbyBackgroundImg;
        [SerializeField] private Sprite[] lobbyThemeSprites;
        [SerializeField] private Button[] lobbyThemeActionButtons;
        [SerializeField] private TMP_Text[] lobbyThemeActionTexts;
        [SerializeField] private TMP_Text[] lobbyThemePriceTexts;

        [Header("Shop Tabs")]
        [SerializeField] private Button btnShopTabInGame;
        [SerializeField] private Button btnShopTabLobby;
        [SerializeField] private Image btnShopTabInGameBg;
        [SerializeField] private Image btnShopTabLobbyBg;
        [SerializeField] private TMP_Text btnShopTabInGameText;
        [SerializeField] private TMP_Text btnShopTabLobbyText;
        [SerializeField] private GameObject shopInGameThemesPanel;
        [SerializeField] private GameObject shopLobbyThemesPanel;

        private int _currentShopTab = 0;

        [Header("Settings Modal")]
        [SerializeField] private GameObject settingsModal;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button btnCloseSettings;

        [Header("Help Modal")]
        [SerializeField] private GameObject helpModal;
        [SerializeField] private Button btnCloseHelp;
        [SerializeField] private Button btnConfirmHelp;

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

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);
            ApplyTheme(equippedTheme);

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby);

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
                _currentNickname = System.Text.RegularExpressions.Regex.Replace(_currentNickname, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentBio = System.Text.RegularExpressions.Regex.Replace(_currentBio, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
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
            if (profileModalNicknameInput != null)
            {
                profileModalNicknameInput.onEndEdit.AddListener(OnNicknameEndEdit);
            }
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
            
            // Shop Tabs (In-Game Theme vs Lobby Theme)
            if (btnShopTabInGame != null)
            {
                btnShopTabInGame.onClick.RemoveAllListeners();
                btnShopTabInGame.onClick.AddListener(() => { PlayClickSound(); SelectShopTab(0); });
            }
            if (btnShopTabLobby != null)
            {
                btnShopTabLobby.onClick.RemoveAllListeners();
                btnShopTabLobby.onClick.AddListener(() => { PlayClickSound(); SelectShopTab(1); });
            }

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (themeActionButtons[i] != null)
                    {
                        themeActionButtons[i].onClick.RemoveAllListeners();
                        themeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipTheme(idx);
                        });
                    }
                }
            }

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (lobbyThemeActionButtons[i] != null)
                    {
                        lobbyThemeActionButtons[i].onClick.RemoveAllListeners();
                        lobbyThemeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipLobbyTheme(idx);
                        });
                    }
                }
            }

            // Settings Modal
            if (btnCloseSettings != null) btnCloseSettings.onClick.AddListener(() => { PlayClickSound(); CloseSettingsModal(); });
            if (bgmSlider != null)
            {
                bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", BlockAudioManager.DEFAULT_BGM_VOLUME);
                bgmSlider.onValueChanged.AddListener((v) =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.SetBGMVolume(v);
                });
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", BlockAudioManager.DEFAULT_SFX_VOLUME);
                sfxSlider.onValueChanged.AddListener((v) =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.SetSFXVolume(v);
                });
            }

            // Help Modal
            if (btnCloseHelp != null) btnCloseHelp.onClick.AddListener(() => { PlayClickSound(); CloseHelpModal(); });
            if (btnConfirmHelp != null) btnConfirmHelp.onClick.AddListener(() => { PlayClickSound(); CloseHelpModal(); });

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

            if (btnPlayGameGlow != null)
            {
                btnPlayGameGlow.color = new Color(MenuColors[index].r, MenuColors[index].g, MenuColors[index].b, 0.50f);
            }

            if (triggerActionIfAlreadySelected && btnPlayGame != null)
            {
                StartCoroutine(PunchMascot(btnPlayGame.GetComponent<RectTransform>()));
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
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshProfileUI();
                profileModal.SetActive(true);
            }
        }

        public void CloseProfileModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (profileModal != null) profileModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
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

        private void OnNicknameEndEdit(string newNick)
        {
            newNick = System.Text.RegularExpressions.Regex.Replace(newNick ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
            if (!string.IsNullOrWhiteSpace(newNick))
            {
                _currentNickname = newNick;
                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.Save();
                RefreshProfileUI();
            }
        }

        private void OnBioEndEdit(string newBio)
        {
            _currentBio = System.Text.RegularExpressions.Regex.Replace(newBio ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
            PlayerPrefs.SetString(KEY_BIO, _currentBio);
            PlayerPrefs.Save();
            RefreshProfileUI();
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

            if (profileModalNicknameInput != null)
            {
                profileModalNicknameInput.text = _isLoggedIn ? _currentNickname : "로그인이 필요합니다";
                profileModalNicknameInput.interactable = _isLoggedIn;
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
        // SHOP MODAL & THEME STORE
        // ==========================================

        public void OpenShopModal()
        {
            if (shopModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                SelectShopTab(_currentShopTab);
                RefreshThemeShopUI();
                RefreshLobbyThemeShopUI();
                shopModal.SetActive(true);
            }
        }

        public void CloseShopModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (shopModal != null) shopModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        public void BuyOrEquipTheme(int themeIdx)
        {
            if (themeIdx < 0 || themeIdx >= ThemePrices.Length) return;

            bool isOwned = (themeIdx == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + themeIdx, 0) == 1);

            if (isOwned)
            {
                // Equip Theme
                PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, themeIdx);
                PlayerPrefs.Save();
                ApplyTheme(themeIdx);
                RefreshThemeShopUI();
                PlayClickSound();
            }
            else
            {
                int price = ThemePrices[themeIdx];
                if (_currentCoins >= price)
                {
                    _currentCoins -= price;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                    PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + themeIdx, 1);
                    PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, themeIdx);
                    PlayerPrefs.Save();

                    ApplyTheme(themeIdx);
                    RefreshProfileUI();
                    RefreshThemeShopUI();

                    if (FairyScreenTransition.Instance != null)
                    {
                        FairyScreenTransition.Instance.EmitCornerSparkles();
                    }
                    if (BlockAudioManager.Instance != null)
                    {
                        BlockAudioManager.Instance.PlayBuy();
                    }
                }
                else
                {
                    // Insufficient Coins
                    PlayClickSound();
                    if (shopCoinsText != null)
                    {
                        StartCoroutine(FlashCoinsTextRed());
                    }
                }
            }
        }

        private IEnumerator FlashCoinsTextRed()
        {
            if (shopCoinsText == null) yield break;
            Color orig = shopCoinsText.color;
            shopCoinsText.color = new Color(1f, 0.25f, 0.35f);
            yield return new WaitForSeconds(0.4f);
            shopCoinsText.color = orig;
        }

        public void ApplyTheme(int themeIdx)
        {
            if (inGameBackgroundImg == null)
            {
                var bgObj = GameObject.Find("BackgroundImage");
                if (bgObj != null) inGameBackgroundImg = bgObj.GetComponent<Image>();
            }

            if (inGameBackgroundImg != null && themeSprites != null && themeIdx >= 0 && themeIdx < themeSprites.Length)
            {
                if (themeSprites[themeIdx] != null)
                {
                    inGameBackgroundImg.sprite = themeSprites[themeIdx];
                    inGameBackgroundImg.color = Color.white;
                }
            }
        }

        public void RefreshThemeShopUI()
        {
            if (shopCoinsText != null)
            {
                shopCoinsText.text = $"내 코인: {_currentCoins:N0} C";
            }

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    if (themeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedTheme == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + i, 0) == 1);

                    Image btnImg = themeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (themeActionTexts != null && i < themeActionTexts.Length && themeActionTexts[i] != null)
                        ? themeActionTexts[i]
                        : themeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = "적용 중";
                            txt.color = Color.white;
                        }
                        if (btnImg != null) btnImg.color = new Color(0.20f, 0.82f, 0.65f, 1f); // Vibrant mint
                        themeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = "장착하기";
                            txt.color = Color.white;
                        }
                        if (btnImg != null) btnImg.color = new Color(1.0f, 0.40f, 0.62f, 1f); // Vibrant strawberry pink
                        themeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < ThemePrices.Length) ? ThemePrices[i] : 0;
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C 구매";
                            txt.color = Color.white;
                        }
                        if (btnImg != null)
                        {
                            btnImg.color = (_currentCoins >= price)
                                ? new Color(0.68f, 0.42f, 0.98f, 1f) // Vibrant lilac
                                : new Color(0.65f, 0.60f, 0.72f, 0.7f); // Muted lavender
                        }
                        themeActionButtons[i].interactable = (_currentCoins >= price);
                    }
                }
            }
        }

        public void SetupShopThemes(Image inGameBg, Sprite[] bgSprites, Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] priceTexts = null)
        {
            inGameBackgroundImg = inGameBg;
            themeSprites = bgSprites;
            themeActionButtons = actionBtns;
            themeActionTexts = actionTexts;
            themePriceTexts = priceTexts;

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (themeActionButtons[i] != null)
                    {
                        themeActionButtons[i].onClick.RemoveAllListeners();
                        themeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipTheme(idx);
                        });
                    }
                }
            }

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);
            ApplyTheme(equippedTheme);
            RefreshThemeShopUI();
        }

        // ==========================================
        // SHOP TABS & LOBBY THEME STORE
        // ==========================================

        public void SelectShopTab(int tabIndex)
        {
            _currentShopTab = Mathf.Clamp(tabIndex, 0, 1);
            if (shopInGameThemesPanel != null) shopInGameThemesPanel.SetActive(_currentShopTab == 0);
            if (shopLobbyThemesPanel != null) shopLobbyThemesPanel.SetActive(_currentShopTab == 1);

            Color activeTabBg = new Color(1.0f, 0.40f, 0.62f, 1f); // Vibrant Candy Pink
            Color activeTabText = Color.white;
            Color inactiveTabBg = new Color(0.92f, 0.90f, 0.97f, 0.85f); // Soft lavender
            Color inactiveTabText = new Color(0.40f, 0.30f, 0.55f, 0.9f);

            if (btnShopTabInGameBg != null) btnShopTabInGameBg.color = (_currentShopTab == 0) ? activeTabBg : inactiveTabBg;
            if (btnShopTabInGameText != null)
            {
                btnShopTabInGameText.text = "게임 테마";
                btnShopTabInGameText.color = (_currentShopTab == 0) ? activeTabText : inactiveTabText;
            }

            if (btnShopTabLobbyBg != null) btnShopTabLobbyBg.color = (_currentShopTab == 1) ? new Color(0.55f, 0.38f, 0.95f, 1f) : inactiveTabBg;
            if (btnShopTabLobbyText != null)
            {
                btnShopTabLobbyText.text = "로비 테마";
                btnShopTabLobbyText.color = (_currentShopTab == 1) ? activeTabText : inactiveTabText;
            }

            if (_currentShopTab == 0) RefreshThemeShopUI();
            else RefreshLobbyThemeShopUI();
        }

        public void BuyOrEquipLobbyTheme(int lobbyIdx)
        {
            if (lobbyIdx < 0 || lobbyIdx >= LobbyThemePrices.Length) return;

            bool isOwned = (lobbyIdx == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + lobbyIdx, 0) == 1);

            if (isOwned)
            {
                PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, lobbyIdx);
                PlayerPrefs.Save();
                ApplyLobbyTheme(lobbyIdx);
                RefreshLobbyThemeShopUI();
                PlayClickSound();
            }
            else
            {
                int price = LobbyThemePrices[lobbyIdx];
                if (_currentCoins >= price)
                {
                    _currentCoins -= price;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                    PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + lobbyIdx, 1);
                    PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, lobbyIdx);
                    PlayerPrefs.Save();

                    ApplyLobbyTheme(lobbyIdx);
                    RefreshProfileUI();
                    RefreshThemeShopUI();
                    RefreshLobbyThemeShopUI();

                    if (FairyScreenTransition.Instance != null)
                    {
                        FairyScreenTransition.Instance.EmitCornerSparkles();
                    }
                    if (BlockAudioManager.Instance != null)
                    {
                        BlockAudioManager.Instance.PlayBuy();
                    }
                }
                else
                {
                    // Insufficient Coins
                    PlayClickSound();
                    if (shopCoinsText != null)
                    {
                        StartCoroutine(FlashCoinsTextRed());
                    }
                }
            }
        }

        public void ApplyLobbyTheme(int lobbyIdx)
        {
            if (lobbyBackgroundImg == null)
            {
                var bgObj = GameObject.Find("LobbyBg");
                if (bgObj != null) lobbyBackgroundImg = bgObj.GetComponent<Image>();
            }

            if (lobbyBackgroundImg != null && lobbyThemeSprites != null && lobbyIdx >= 0 && lobbyIdx < lobbyThemeSprites.Length)
            {
                if (lobbyThemeSprites[lobbyIdx] != null)
                {
                    lobbyBackgroundImg.sprite = lobbyThemeSprites[lobbyIdx];
                    lobbyBackgroundImg.color = Color.white;
                }
            }

            // Play matching Lobby BGM track (Theme 0 -> robby 1, Theme 1 -> robby 2, Theme 2 -> robby 3)
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayLobbyBGM(lobbyIdx);
            }
        }

        public void RefreshLobbyThemeShopUI()
        {
            if (shopCoinsText != null)
            {
                shopCoinsText.text = $"내 코인: {_currentCoins:N0} C";
            }

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    if (lobbyThemeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedLobby == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 0) == 1);

                    Image btnImg = lobbyThemeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (lobbyThemeActionTexts != null && i < lobbyThemeActionTexts.Length && lobbyThemeActionTexts[i] != null)
                        ? lobbyThemeActionTexts[i]
                        : lobbyThemeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = "적용 중";
                            txt.color = Color.white;
                        }
                        if (btnImg != null) btnImg.color = new Color(0.20f, 0.82f, 0.65f, 1f); // Mint
                        lobbyThemeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = "장착하기";
                            txt.color = Color.white;
                        }
                        if (btnImg != null) btnImg.color = new Color(1.0f, 0.40f, 0.62f, 1f); // Pink
                        lobbyThemeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < LobbyThemePrices.Length) ? LobbyThemePrices[i] : 0;
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C 구매";
                            txt.color = Color.white;
                        }
                        if (btnImg != null)
                        {
                            btnImg.color = (_currentCoins >= price)
                                ? new Color(0.55f, 0.38f, 0.95f, 1f) // Purple
                                : new Color(0.65f, 0.60f, 0.72f, 0.7f); // Muted
                        }
                        lobbyThemeActionButtons[i].interactable = (_currentCoins >= price);
                    }
                }
            }
        }

        public void SetupShopTabs(
            Button tabInGame, Button tabLobby,
            Image tabInGameBg, Image tabLobbyBg,
            TMP_Text tabInGameTxt, TMP_Text tabLobbyTxt,
            GameObject inGamePanel, GameObject lobbyPanel)
        {
            btnShopTabInGame = tabInGame;
            btnShopTabLobby = tabLobby;
            btnShopTabInGameBg = tabInGameBg;
            btnShopTabLobbyBg = tabLobbyBg;
            btnShopTabInGameText = tabInGameTxt;
            btnShopTabLobbyText = tabLobbyTxt;
            shopInGameThemesPanel = inGamePanel;
            shopLobbyThemesPanel = lobbyPanel;

            if (btnShopTabInGame != null)
            {
                btnShopTabInGame.onClick.RemoveAllListeners();
                btnShopTabInGame.onClick.AddListener(() => { PlayClickSound(); SelectShopTab(0); });
            }
            if (btnShopTabLobby != null)
            {
                btnShopTabLobby.onClick.RemoveAllListeners();
                btnShopTabLobby.onClick.AddListener(() => { PlayClickSound(); SelectShopTab(1); });
            }

            SelectShopTab(0);
        }

        public void SetupLobbyThemes(
            Image lobbyBg, Sprite[] bgSprites,
            Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] priceTexts = null)
        {
            lobbyBackgroundImg = lobbyBg;
            lobbyThemeSprites = bgSprites;
            lobbyThemeActionButtons = actionBtns;
            lobbyThemeActionTexts = actionTexts;
            lobbyThemePriceTexts = priceTexts;

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    int idx = i;
                    if (lobbyThemeActionButtons[i] != null)
                    {
                        lobbyThemeActionButtons[i].onClick.RemoveAllListeners();
                        lobbyThemeActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipLobbyTheme(idx);
                        });
                    }
                }
            }

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby);
            RefreshLobbyThemeShopUI();
        }

        // ==========================================
        // SETTINGS MODAL
        // ==========================================

        public void OpenSettingsModal()
        {
            if (settingsModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                settingsModal.SetActive(true);
            }
        }

        public void CloseSettingsModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (settingsModal != null) settingsModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        // ==========================================
        // HELP MODAL
        // ==========================================

        public void OpenHelpModal()
        {
            if (helpModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                helpModal.SetActive(true);
            }
        }

        public void CloseHelpModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (helpModal != null) helpModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
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
            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby);
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowInGameUI(false);
            }
        }

        public void StartGameFromLobby()
        {
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayInGameBGM();
            }

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
            GameObject[] glowAuras = null,
            TMP_InputField pNickInput = null,
            Button cfmHBtn = null,
            Image playBtnGlow = null)
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
            btnPlayGameGlow = playBtnGlow;

            profileModal = pModal;
            profileModalAvatar = pModalAv;
            profileModalNickname = pNick;
            profileModalNicknameInput = pNickInput;
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
            btnConfirmHelp = cfmHBtn;

            mascotAvatars = avatars;
        }
    }
}
