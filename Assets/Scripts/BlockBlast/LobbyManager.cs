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

        [Header("Language Logos")]
        [SerializeField] private Image lobbyLogoImage;
        [SerializeField] private Sprite[] languageLogos;

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

        [Header("NanoBanana Custom Shop Tab Sprites")]
        public bool useNanoBananaTabButtons = true;
        [SerializeField] private Sprite tabGameActiveSprite;
        [SerializeField] private Sprite tabLobbyActiveSprite;
        [SerializeField] private Sprite tabInactiveSprite;
        [SerializeField] private Sprite tabOriginalPillSprite;

        private int _currentShopTab = 0;

        [Header("Settings Modal")]
        [SerializeField] private GameObject settingsModal;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button btnCloseSettings;

        [Header("Language Selection (Settings Modal)")]
        [SerializeField] private Button[] languageButtons;
        [SerializeField] private Image[] languageButtonBgs;
        [SerializeField] private TMP_Text[] languageButtonTexts;
        [SerializeField] private TMP_Text settingsTitleText;
        [SerializeField] private TMP_Text settingsBgmText;
        [SerializeField] private TMP_Text settingsSfxText;
        [SerializeField] private TMP_Text settingsLangText;
        [SerializeField] private TMP_Text shopTitleText;
        [SerializeField] private TMP_Text helpTitleText;
        [SerializeField] private TMP_Text helpConfirmText;
        [SerializeField] private TMP_Text profileTitleText;
        [SerializeField] private TMP_Text settingsVersionText;

        [Header("Help Modal")]
        [SerializeField] private GameObject helpModal;
        [SerializeField] private Button btnCloseHelp;
        [SerializeField] private Button btnConfirmHelp;

        [Header("Party Stage Tip")]
        [SerializeField] private TMP_Text partyTipText;

        [Header("Quit Modal (PC)")]
        [SerializeField] private GameObject quitModal;
        [SerializeField] private Button btnQuitConfirmYes;
        [SerializeField] private Button btnQuitConfirmNo;
        [SerializeField] private Button btnQuitDarkBg;
        [SerializeField] private Button btnOpenQuitModal;
        [SerializeField] private TMP_Text quitModalTitleText;
        [SerializeField] private TMP_Text quitModalDescText;
        [SerializeField] private TMP_Text quitModalYesText;
        [SerializeField] private TMP_Text quitModalNoText;
        [SerializeField] private TMP_Text settingsQuitButtonText;

        [Header("Screen Settings (Aspect Ratio & Window Mode)")]
        [SerializeField] private TMP_Text settingsAspectTitleText;
        [SerializeField] private Button[] aspectButtons; // 0: 16:9, 1: 16:10, 2: 4:3, 3: 9:16
        [SerializeField] private Image[] aspectBgs;
        [SerializeField] private TMP_Text[] aspectTexts;

        [SerializeField] private TMP_Text settingsWindowModeTitleText;
        [SerializeField] private Button[] windowModeButtons; // 0: 창모드, 1: 테두리없는 창모드, 2: 전체화면
        [SerializeField] private Image[] windowModeBgs;
        [SerializeField] private TMP_Text[] windowModeTexts;

        [SerializeField] private Sprite screenActiveSprite;
        [SerializeField] private Sprite screenInactiveSprite;

        private const string KEY_ASPECT_RATIO_INDEX = "Mallang_AspectRatio_Idx";
        private const string KEY_WINDOW_MODE_INDEX = "Mallang_WindowMode_Idx";
        private int _currentAspectIdx = 3; // 0: 16:9, 1: 16:10, 2: 4:3, 3: 9:16 (Default: 9:16)
        private int _currentWindowModeIdx = 0; // 0: 창모드, 1: 테두리없는 창모드, 2: 전체화면 (Default: 창모드)

        [Header("Mascot Avatars (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private Sprite[] mascotAvatars;

        [Header("Shop Action Button Sprites")]
        [SerializeField] private Sprite shopEquipBtnSprite;
        [SerializeField] private Sprite shopEquippedBtnSprite;

        private int _currentAvatarIdx = 0;
        private string _currentNickname = "";
        private string _currentBio = "";
        private int _currentCoins = 0; // Steam default: 0 Gold
        private bool _isLoggedIn = true;

        private int _selectedMenuIdx = 0;
        private Coroutine _mascotBounceCoroutine;

        // Button Colors & Texts matching mascots
        private static readonly Color ColorPink = new Color(1f, 0.33f, 0.53f, 1f);     // #FF5588
        private static readonly Color ColorMint = new Color(0.17f, 0.83f, 0.64f, 1f);   // #2CD4A4
        private static readonly Color ColorGold = new Color(1f, 0.70f, 0.0f, 1f);       // #FFB300
        private static readonly Color ColorPurple = new Color(0.66f, 0.33f, 0.97f, 1f);  // #A855F7

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
            EnsurePartyTipReference();
        }

        private void EnsurePartyTipReference()
        {
            if (partyTipText == null)
            {
                var tipObj = GameObject.Find("PartyTip");
                if (tipObj != null)
                {
                    partyTipText = tipObj.GetComponent<TMP_Text>();
                }
            }
        }

        private void Start()
        {
            LocalizationManager.Init();
            LocalizationManager.OnLanguageChanged += UpdateLanguageUI;

            SetupEventListeners();
            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
            RefreshProfileUI();
            SelectMenu(0, false); // Default: Game Start (Pink Mascot)

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);
            ApplyTheme(equippedTheme);

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);
            ApplyLobbyTheme(equippedLobby, playMusic: false);

            if (profileModal != null) profileModal.SetActive(false);
            if (shopModal != null) shopModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
            if (quitModal != null) quitModal.SetActive(false);

            _currentAspectIdx = PlayerPrefs.GetInt(KEY_ASPECT_RATIO_INDEX, 3);
            if (_currentAspectIdx < 0 || _currentAspectIdx > 3) _currentAspectIdx = 3;
            _currentWindowModeIdx = PlayerPrefs.GetInt(KEY_WINDOW_MODE_INDEX, 0);
            if (_currentWindowModeIdx < 0 || _currentWindowModeIdx > 2) _currentWindowModeIdx = 0;
            ApplyScreenSettings();
            UpdateScreenSettingsUI();

            if (_mascotBounceCoroutine != null) StopCoroutine(_mascotBounceCoroutine);
            _mascotBounceCoroutine = StartCoroutine(MascotIdleBounceRoutine());

            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= UpdateLanguageUI;
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
                _currentCoins = 0;     // Steam default: 0 Gold
                _isLoggedIn = true;

                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.SetString(KEY_BIO, _currentBio);
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            }
            else
            {
                _currentNickname = PlayerPrefs.GetString(KEY_NICKNAME, $"말랑이#{Random.Range(1000, 9999)}");
                _currentBio = PlayerPrefs.GetString(KEY_BIO, "말랑블라스트에 오신 걸 환영해요!");
                _currentNickname = System.Text.RegularExpressions.Regex.Replace(_currentNickname, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentBio = System.Text.RegularExpressions.Regex.Replace(_currentBio, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentAvatarIdx = PlayerPrefs.GetInt(KEY_AVATAR, 0);
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 0);
                _isLoggedIn = PlayerPrefs.GetInt(KEY_LOGGED_IN, 1) == 1;
            }

            // Steam Launch Configuration: 0 Gold & all shop themes unlocked by default
            if (PlayerPrefs.GetInt("Steam_Init_Unlocked_All_Themes_v1", 0) == 0)
            {
                PlayerPrefs.SetInt("Steam_Init_Unlocked_All_Themes_v1", 1);
                _currentCoins = 0;
                PlayerPrefs.SetInt(KEY_COINS, 0);
                for (int i = 0; i < 10; i++)
                {
                    PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + i, 1);
                    PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 1);
                }
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.Save();
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

            // Language Selection Buttons (Runtime binding)
            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    int langIdx = i;
                    if (languageButtons[i] != null)
                    {
                        languageButtons[i].onClick.RemoveAllListeners();
                        languageButtons[i].onClick.AddListener(() =>
                        {
                            PlayClickSound();
                            SelectLanguage((GameLanguage)langIdx);
                        });
                    }
                }
            }

            // Resolution & Screen Mode Events (Runtime binding)
            if (aspectButtons != null)
            {
                for (int i = 0; i < aspectButtons.Length; i++)
                {
                    int idx = i;
                    if (aspectButtons[i] != null)
                    {
                        aspectButtons[i].onClick.RemoveAllListeners();
                        aspectButtons[i].onClick.AddListener(() => SetAspectRatio(idx));
                    }
                }
            }
            if (windowModeButtons != null)
            {
                for (int j = 0; j < windowModeButtons.Length; j++)
                {
                    int idx = j;
                    if (windowModeButtons[j] != null)
                    {
                        windowModeButtons[j].onClick.RemoveAllListeners();
                        windowModeButtons[j].onClick.AddListener(() => SetWindowMode(idx));
                    }
                }
            }

            // Quit Confirm Modal Events (Runtime binding)
            if (btnQuitConfirmYes != null)
            {
                btnQuitConfirmYes.onClick.RemoveAllListeners();
                btnQuitConfirmYes.onClick.AddListener(QuitGame);
            }
            if (btnQuitConfirmNo != null)
            {
                btnQuitConfirmNo.onClick.RemoveAllListeners();
                btnQuitConfirmNo.onClick.AddListener(CloseQuitModal);
            }
            if (btnQuitDarkBg != null)
            {
                btnQuitDarkBg.onClick.RemoveAllListeners();
                btnQuitDarkBg.onClick.AddListener(CloseQuitModal);
            }
            if (btnOpenQuitModal != null)
            {
                btnOpenQuitModal.onClick.RemoveAllListeners();
                btnOpenQuitModal.onClick.AddListener(OpenQuitModal);
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

        public string GetMenuActionText(int index)
        {
            switch (index)
            {
                case 0: return LocalizationManager.Get("lobby_start") + "!";
                case 1: return LocalizationManager.Get("lobby_shop") + "!";
                case 2: return LocalizationManager.Get("lobby_settings") + "!";
                case 3: return LocalizationManager.Get("lobby_help") + "!";
                default: return "";
            }
        }

        public void SelectMenu(int index, bool triggerActionIfAlreadySelected = false)
        {
            if (index < 0 || index >= MenuColors.Length) return;

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
                btnPlayGameText.text = GetMenuActionText(index);
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
                profileModalNicknameInput.text = _isLoggedIn ? _currentNickname : LocalizationManager.Get("profile_login_required");
                profileModalNicknameInput.interactable = _isLoggedIn;
            }

            if (profileModalNickname != null)
            {
                profileModalNickname.text = _isLoggedIn ? _currentNickname : LocalizationManager.Get("profile_login_required");
            }

            if (profileModalBioInput != null)
            {
                profileModalBioInput.text = _currentBio;
                profileModalBioInput.interactable = _isLoggedIn;
                if (profileModalBioInput.placeholder is TMP_Text phText)
                {
                    phText.text = LocalizationManager.Get("profile_bio_placeholder");
                }
            }

            if (profileModalBestScore != null)
            {
                int best = PlayerPrefs.GetInt("BlockBlast_Best", 0);
                profileModalBestScore.text = $"{LocalizationManager.Get("profile_best_score_prefix")}: {best:N0}{LocalizationManager.Get("profile_best_score_suffix")}";
            }

            if (profileModalCoins != null)
            {
                profileModalCoins.text = $"{LocalizationManager.Get("profile_coins_label")}: {_currentCoins:N0} C";
            }

            if (btnLogout != null)
            {
                TMP_Text btnText = btnLogout.GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    btnText.text = _isLoggedIn ? LocalizationManager.Get("profile_btn_logout") : LocalizationManager.Get("profile_btn_login");
                }
            }

            if (profileModal != null)
            {
                var card = profileModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var bioLbl = card.Find("BioLbl")?.GetComponent<TMP_Text>();
                    if (bioLbl != null) bioLbl.text = LocalizationManager.Get("profile_bio_label");
                    var pickLbl = card.Find("PickLbl")?.GetComponent<TMP_Text>();
                    if (pickLbl != null) pickLbl.text = LocalizationManager.Get("profile_pick_label");
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

            bool isOwned = (themeIdx == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + themeIdx, 1) == 1);

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

            if (SideWingsDecorator.Instance != null)
            {
                SideWingsDecorator.Instance.SyncWithTheme(themeIdx);
            }
        }

        public void RefreshThemeShopUI()
        {
            if (shopCoinsText != null)
            {
                shopCoinsText.text = $"{LocalizationManager.Get("my_coins")}: {_currentCoins:N0} C";
            }

            int equippedTheme = PlayerPrefs.GetInt(KEY_EQUIPPED_THEME, 0);

            if (themeActionButtons != null)
            {
                for (int i = 0; i < themeActionButtons.Length; i++)
                {
                    if (themeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedTheme == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_THEME_OWNED_PREFIX + i, 1) == 1);

                    Image btnImg = themeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (themeActionTexts != null && i < themeActionTexts.Length && themeActionTexts[i] != null)
                        ? themeActionTexts[i]
                        : themeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equipped");
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f); // Dark Forest Teal
                        }
                        if (btnImg != null)
                        {
                            if (shopEquippedBtnSprite != null) btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equip");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f); // Dark Berry Magenta
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < ThemePrices.Length) ? ThemePrices[i] : 0;
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        themeActionButtons[i].interactable = (_currentCoins >= price);
                    }
                }
            }

            if (shopInGameThemesPanel != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var itemCard = shopInGameThemesPanel.transform.Find($"ThemeItem_{i}");
                    if (itemCard != null)
                    {
                        var nameTxt = itemCard.Find("Name")?.GetComponent<TMP_Text>();
                        if (nameTxt != null) nameTxt.text = LocalizationManager.Get($"theme_game_{i}_name");
                        var descTxt = itemCard.Find("Desc")?.GetComponent<TMP_Text>();
                        if (descTxt != null) descTxt.text = LocalizationManager.Get($"theme_game_{i}_desc");
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

        public void SetupShopButtonSprites(Sprite equipSp, Sprite equippedSp)
        {
            shopEquipBtnSprite = equipSp;
            shopEquippedBtnSprite = equippedSp;
            RefreshThemeShopUI();
            RefreshLobbyThemeShopUI();
        }

        // ==========================================
        // SHOP TABS & LOBBY THEME STORE
        // ==========================================

        public void SelectShopTab(int tabIndex)
        {
            _currentShopTab = Mathf.Clamp(tabIndex, 0, 1);
            if (shopInGameThemesPanel != null) shopInGameThemesPanel.SetActive(_currentShopTab == 0);
            if (shopLobbyThemesPanel != null) shopLobbyThemesPanel.SetActive(_currentShopTab == 1);

            if (useNanoBananaTabButtons && tabGameActiveSprite != null && tabLobbyActiveSprite != null && tabInactiveSprite != null)
            {
                Color activeTextColor = Color.white;
                Color inactiveTextColor = new Color(0.32f, 0.22f, 0.48f, 1f);

                if (btnShopTabInGameBg != null)
                {
                    btnShopTabInGameBg.sprite = (_currentShopTab == 0) ? tabGameActiveSprite : tabInactiveSprite;
                    btnShopTabInGameBg.color = Color.white;
                }
                if (btnShopTabInGameText != null)
                {
                    btnShopTabInGameText.text = LocalizationManager.Get("shop_tab_game");
                    btnShopTabInGameText.color = (_currentShopTab == 0) ? activeTextColor : inactiveTextColor;
                }

                if (btnShopTabLobbyBg != null)
                {
                    btnShopTabLobbyBg.sprite = (_currentShopTab == 1) ? tabLobbyActiveSprite : tabInactiveSprite;
                    btnShopTabLobbyBg.color = Color.white;
                }
                if (btnShopTabLobbyText != null)
                {
                    btnShopTabLobbyText.text = LocalizationManager.Get("shop_tab_lobby");
                    btnShopTabLobbyText.color = (_currentShopTab == 1) ? activeTextColor : inactiveTextColor;
                }
            }
            else
            {
                Color activeTabBg = new Color(1.0f, 0.40f, 0.62f, 1f); // Vibrant Candy Pink
                Color activeTabText = Color.white;
                Color inactiveTabBg = new Color(0.92f, 0.90f, 0.97f, 0.85f); // Soft lavender
                Color inactiveTabText = new Color(0.40f, 0.30f, 0.55f, 0.9f);

                if (tabOriginalPillSprite != null)
                {
                    if (btnShopTabInGameBg != null) btnShopTabInGameBg.sprite = tabOriginalPillSprite;
                    if (btnShopTabLobbyBg != null) btnShopTabLobbyBg.sprite = tabOriginalPillSprite;
                }

                if (btnShopTabInGameBg != null) btnShopTabInGameBg.color = (_currentShopTab == 0) ? activeTabBg : inactiveTabBg;
                if (btnShopTabInGameText != null)
                {
                    btnShopTabInGameText.text = LocalizationManager.Get("shop_tab_game");
                    btnShopTabInGameText.color = (_currentShopTab == 0) ? activeTabText : inactiveTabText;
                }

                if (btnShopTabLobbyBg != null) btnShopTabLobbyBg.color = (_currentShopTab == 1) ? new Color(0.55f, 0.38f, 0.95f, 1f) : inactiveTabBg;
                if (btnShopTabLobbyText != null)
                {
                    btnShopTabLobbyText.text = LocalizationManager.Get("shop_tab_lobby");
                    btnShopTabLobbyText.color = (_currentShopTab == 1) ? activeTabText : inactiveTabText;
                }
            }

            if (_currentShopTab == 0) RefreshThemeShopUI();
            else RefreshLobbyThemeShopUI();
        }

        public void BuyOrEquipLobbyTheme(int lobbyIdx)
        {
            if (lobbyIdx < 0 || lobbyIdx >= LobbyThemePrices.Length) return;

            bool isOwned = (lobbyIdx == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + lobbyIdx, 1) == 1);

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

        public void ApplyLobbyTheme(int lobbyIdx, bool playMusic = true)
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
            if (playMusic && BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayLobbyBGM(lobbyIdx);
            }
        }

        public void RefreshLobbyThemeShopUI()
        {
            if (shopCoinsText != null)
            {
                shopCoinsText.text = $"{LocalizationManager.Get("my_coins")}: {_currentCoins:N0} C";
            }

            int equippedLobby = PlayerPrefs.GetInt(KEY_EQUIPPED_LOBBY_THEME, 0);

            if (lobbyThemeActionButtons != null)
            {
                for (int i = 0; i < lobbyThemeActionButtons.Length; i++)
                {
                    if (lobbyThemeActionButtons[i] == null) continue;

                    bool isEquipped = (equippedLobby == i);
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 1) == 1);

                    Image btnImg = lobbyThemeActionButtons[i].GetComponent<Image>();
                    TMP_Text txt = (lobbyThemeActionTexts != null && i < lobbyThemeActionTexts.Length && lobbyThemeActionTexts[i] != null)
                        ? lobbyThemeActionTexts[i]
                        : lobbyThemeActionButtons[i].GetComponentInChildren<TMP_Text>();

                    if (isEquipped)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equipped");
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f); // Dark Forest Teal
                        }
                        if (btnImg != null)
                        {
                            if (shopEquippedBtnSprite != null) btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equip");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f); // Dark Berry Magenta
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = (i < LobbyThemePrices.Length) ? LobbyThemePrices[i] : 0;
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} C " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null)
                        {
                            if (shopEquipBtnSprite != null) btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        lobbyThemeActionButtons[i].interactable = (_currentCoins >= price);
                    }
                }
            }

            if (shopLobbyThemesPanel != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var itemCard = shopLobbyThemesPanel.transform.Find($"LobbyThemeItem_{i}");
                    if (itemCard != null)
                    {
                        var nameTxt = itemCard.Find("Name")?.GetComponent<TMP_Text>();
                        if (nameTxt != null) nameTxt.text = LocalizationManager.Get($"theme_lobby_{i}_name");
                        var descTxt = itemCard.Find("Desc")?.GetComponent<TMP_Text>();
                        if (descTxt != null) descTxt.text = LocalizationManager.Get($"theme_lobby_{i}_desc");
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

        public void SetupShopTabSprites(Sprite gameActive, Sprite lobbyActive, Sprite inactive, Sprite originalPill)
        {
            tabGameActiveSprite = gameActive;
            tabLobbyActiveSprite = lobbyActive;
            tabInactiveSprite = inactive;
            tabOriginalPillSprite = originalPill;
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

        private void Update()
        {
            bool isEsc = false;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
                isEsc = UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
#else
            isEsc = Input.GetKeyDown(KeyCode.Escape);
#endif
            if (isEsc)
            {
                HandleEscapeKey();
            }
        }

        private void HandleEscapeKey()
        {
            if (quitModal != null && quitModal.activeSelf)
            {
                CloseQuitModal();
                return;
            }
            if (settingsModal != null && settingsModal.activeSelf)
            {
                CloseSettingsModal();
                return;
            }
            if (shopModal != null && shopModal.activeSelf)
            {
                CloseShopModal();
                return;
            }
            if (helpModal != null && helpModal.activeSelf)
            {
                CloseHelpModal();
                return;
            }
            if (profileModal != null && profileModal.activeSelf)
            {
                CloseProfileModal();
                return;
            }

            // If no modal is open in Lobby, prompt quit confirmation
            OpenQuitModal();
        }

        // ==========================================
        // QUIT MODAL & MULTI-RESOLUTION SETTINGS
        // ==========================================

        public void OpenQuitModal()
        {
            if (quitModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                quitModal.transform.SetAsLastSibling();
                quitModal.SetActive(true);
            }
        }

        public void CloseQuitModal()
        {
            if (quitModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                quitModal.SetActive(false);
            }
        }

        public void QuitGame()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            Debug.Log("<color=red>[LobbyManager] Quitting application...</color>");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetAspectRatio(int aspectIdx)
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            _currentAspectIdx = Mathf.Clamp(aspectIdx, 0, 3);
            PlayerPrefs.SetInt(KEY_ASPECT_RATIO_INDEX, _currentAspectIdx);
            PlayerPrefs.Save();
            ApplyScreenSettings();
            UpdateScreenSettingsUI();
        }

        public void SetWindowMode(int modeIdx)
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
            _currentWindowModeIdx = Mathf.Clamp(modeIdx, 0, 2);
            PlayerPrefs.SetInt(KEY_WINDOW_MODE_INDEX, _currentWindowModeIdx);
            PlayerPrefs.Save();
            ApplyScreenSettings();
            UpdateScreenSettingsUI();
        }

        public void ApplyScreenSettings()
        {
            FullScreenMode mode = FullScreenMode.FullScreenWindow;
            if (_currentWindowModeIdx == 0) mode = FullScreenMode.Windowed;
            else if (_currentWindowModeIdx == 1) mode = FullScreenMode.FullScreenWindow;
            else if (_currentWindowModeIdx == 2) mode = FullScreenMode.ExclusiveFullScreen;

            int screenW = Display.main.systemWidth > 0 ? Display.main.systemWidth : Screen.currentResolution.width;
            int screenH = Display.main.systemHeight > 0 ? Display.main.systemHeight : Screen.currentResolution.height;
            if (screenW <= 0) screenW = 1920;
            if (screenH <= 0) screenH = 1080;

            int targetW = screenW;
            int targetH = screenH;

            if (mode == FullScreenMode.Windowed)
            {
                switch (_currentAspectIdx)
                {
                    case 0: targetW = 1600; targetH = 900; break;  // 16:9
                    case 1: targetW = 1440; targetH = 900; break;  // 16:10
                    case 2: targetW = 1200; targetH = 900; break;  // 4:3
                    case 3: targetW = 720; targetH = 1280; break;  // 9:16
                    default: targetW = 1600; targetH = 900; break;
                }
            }
            else if (mode == FullScreenMode.ExclusiveFullScreen)
            {
                switch (_currentAspectIdx)
                {
                    case 0: targetW = 1920; targetH = 1080; break; // 16:9
                    case 1: targetW = 1920; targetH = 1200; break; // 16:10
                    case 2: targetW = 1440; targetH = 1080; break; // 4:3
                    case 3: targetW = 1080; targetH = 1920; break; // 9:16
                    default: targetW = 1920; targetH = 1080; break;
                }
            }
            else // FullScreenWindow (Borderless)
            {
                targetW = screenW;
                targetH = screenH;
            }

            Screen.SetResolution(targetW, targetH, mode);
            Debug.Log($"<color=green>[LobbyManager] Applied Screen Settings: Aspect={_currentAspectIdx}, Mode={mode} -> {targetW}x{targetH}</color>");

            if (AspectRatioAdapter.Instance != null)
            {
                AspectRatioAdapter.Instance.UpdateScaler();
            }
            if (SideWingsDecorator.Instance != null)
            {
                SideWingsDecorator.Instance.UpdateVisibility();
            }
        }

        public void UpdateScreenSettingsUI()
        {
            if (settingsAspectTitleText != null)
            {
                settingsAspectTitleText.text = LocalizationManager.Get("settings_aspect_ratio_title");
            }
            if (settingsWindowModeTitleText != null)
            {
                settingsWindowModeTitleText.text = LocalizationManager.Get("settings_window_mode_title");
            }

            string[] aspectKeys = new string[] { "aspect_16_9", "aspect_16_10", "aspect_4_3", "aspect_9_16" };
            if (aspectButtons != null && aspectBgs != null && aspectTexts != null)
            {
                for (int i = 0; i < aspectButtons.Length && i < aspectKeys.Length; i++)
                {
                    bool isActive = (_currentAspectIdx == i);
                    if (aspectBgs[i] != null)
                    {
                        if (screenActiveSprite != null && screenInactiveSprite != null)
                        {
                            aspectBgs[i].sprite = isActive ? screenActiveSprite : screenInactiveSprite;
                            aspectBgs[i].color = Color.white;
                        }
                        else
                        {
                            aspectBgs[i].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.98f, 1f);
                        }
                    }
                    if (aspectTexts[i] != null)
                    {
                        aspectTexts[i].text = LocalizationManager.Get(aspectKeys[i]);
                        aspectTexts[i].color = isActive ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                        aspectTexts[i].fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                    }
                }
            }

            string[] winModeKeys = new string[] { "window_mode_windowed", "window_mode_borderless", "window_mode_fullscreen" };
            if (windowModeButtons != null && windowModeBgs != null && windowModeTexts != null)
            {
                for (int j = 0; j < windowModeButtons.Length && j < winModeKeys.Length; j++)
                {
                    bool isActive = (_currentWindowModeIdx == j);
                    if (windowModeBgs[j] != null)
                    {
                        if (screenActiveSprite != null && screenInactiveSprite != null)
                        {
                            windowModeBgs[j].sprite = isActive ? screenActiveSprite : screenInactiveSprite;
                            windowModeBgs[j].color = Color.white;
                        }
                        else
                        {
                            windowModeBgs[j].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.98f, 1f);
                        }
                    }
                    if (windowModeTexts[j] != null)
                    {
                        windowModeTexts[j].text = LocalizationManager.Get(winModeKeys[j]);
                        windowModeTexts[j].color = isActive ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                        windowModeTexts[j].fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                    }
                }
            }
        }

        public void UpdateScreenModeUI(bool isFullscreen)
        {
            UpdateScreenSettingsUI();
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
            Image playBtnGlow = null,
            TMP_Text partyTip = null)
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
            partyTipText = partyTip;

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

            if (partyTipText != null)
            {
                partyTipText.text = LocalizationManager.Get("lobby_party_tip");
            }
        }

        public void SetupScreenSettingsAndQuitReferences(
            GameObject qModal, Button qYes, Button qNo, Button qDarkBg,
            TMP_Text qTitle, TMP_Text qDesc, TMP_Text qYesTxt, TMP_Text qNoTxt,
            TMP_Text aspectTitle, Button[] aBtns, Image[] aBgs, TMP_Text[] aTexts,
            TMP_Text winModeTitle, Button[] wBtns, Image[] wBgs, TMP_Text[] wTexts,
            Sprite activeSprite, Sprite inactiveSprite)
        {
            quitModal = qModal;
            btnQuitConfirmYes = qYes;
            btnQuitConfirmNo = qNo;
            btnQuitDarkBg = qDarkBg;
            quitModalTitleText = qTitle;
            quitModalDescText = qDesc;
            quitModalYesText = qYesTxt;
            quitModalNoText = qNoTxt;

            settingsAspectTitleText = aspectTitle;
            aspectButtons = aBtns;
            aspectBgs = aBgs;
            aspectTexts = aTexts;

            settingsWindowModeTitleText = winModeTitle;
            windowModeButtons = wBtns;
            windowModeBgs = wBgs;
            windowModeTexts = wTexts;

            screenActiveSprite = activeSprite;
            screenInactiveSprite = inactiveSprite;

            if (quitModal != null) quitModal.SetActive(false);

            if (btnQuitConfirmYes != null)
            {
                btnQuitConfirmYes.onClick.RemoveAllListeners();
                btnQuitConfirmYes.onClick.AddListener(QuitGame);
            }
            if (btnQuitConfirmNo != null)
            {
                btnQuitConfirmNo.onClick.RemoveAllListeners();
                btnQuitConfirmNo.onClick.AddListener(CloseQuitModal);
            }
            if (btnQuitDarkBg != null)
            {
                btnQuitDarkBg.onClick.RemoveAllListeners();
                btnQuitDarkBg.onClick.AddListener(CloseQuitModal);
            }

            if (aspectButtons != null)
            {
                for (int i = 0; i < aspectButtons.Length; i++)
                {
                    int idx = i;
                    if (aspectButtons[i] != null)
                    {
                        aspectButtons[i].onClick.RemoveAllListeners();
                        aspectButtons[i].onClick.AddListener(() => SetAspectRatio(idx));
                    }
                }
            }

            if (windowModeButtons != null)
            {
                for (int j = 0; j < windowModeButtons.Length; j++)
                {
                    int idx = j;
                    if (windowModeButtons[j] != null)
                    {
                        windowModeButtons[j].onClick.RemoveAllListeners();
                        windowModeButtons[j].onClick.AddListener(() => SetWindowMode(idx));
                    }
                }
            }

            UpdateScreenSettingsUI();
        }

        public void SetupLanguageButtons(
            Button[] langBtns, Image[] langBgs, TMP_Text[] langTexts,
            TMP_Text setT, TMP_Text setB, TMP_Text setS, TMP_Text setL,
            TMP_Text shpT = null, TMP_Text hlT = null, TMP_Text hlC = null, TMP_Text prT = null,
            TMP_Text verT = null)
        {
            languageButtons = langBtns;
            languageButtonBgs = langBgs;
            languageButtonTexts = langTexts;
            settingsTitleText = setT;
            settingsBgmText = setB;
            settingsSfxText = setS;
            settingsLangText = setL;
            shopTitleText = shpT;
            helpTitleText = hlT;
            helpConfirmText = hlC;
            profileTitleText = prT;
            settingsVersionText = verT;

            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    int langIdx = i;
                    if (languageButtons[i] != null)
                    {
                        languageButtons[i].onClick.RemoveAllListeners();
                        languageButtons[i].onClick.AddListener(() =>
                        {
                            PlayClickSound();
                            SelectLanguage((GameLanguage)langIdx);
                        });
                    }
                }
            }

            UpdateLanguageUI(LocalizationManager.CurrentLanguage);
        }

        public void SelectLanguage(GameLanguage lang)
        {
            LocalizationManager.CurrentLanguage = lang;
            UpdateLanguageUI(lang);
        }

        public void UpdateLanguageUI(GameLanguage lang)
        {
            // Update 4 Language Button visual states
            if (languageButtons != null)
            {
                for (int i = 0; i < languageButtons.Length; i++)
                {
                    bool isSelected = ((int)lang == i);
                    if (languageButtonBgs != null && i < languageButtonBgs.Length && languageButtonBgs[i] != null)
                    {
                        // Active: Vibrant Candy Pink / Inactive: Soft Lavender Cream
                        languageButtonBgs[i].color = isSelected ? new Color(1f, 0.35f, 0.55f, 1f) : new Color(0.92f, 0.90f, 0.96f, 1f);
                    }
                    if (languageButtonTexts != null && i < languageButtonTexts.Length && languageButtonTexts[i] != null)
                    {
                        languageButtonTexts[i].color = isSelected ? Color.white : new Color(0.40f, 0.30f, 0.55f, 1f);
                        languageButtonTexts[i].fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
                    }
                }
            }

            // Update Party Stage Tip
            EnsurePartyTipReference();
            if (partyTipText != null) partyTipText.text = LocalizationManager.Get("lobby_party_tip");

            // Update Settings Modal Texts
            if (settingsTitleText != null) settingsTitleText.text = LocalizationManager.Get("settings_title");
            if (settingsBgmText != null) settingsBgmText.text = LocalizationManager.Get("settings_bgm");
            if (settingsSfxText != null) settingsSfxText.text = LocalizationManager.Get("settings_sfx");
            if (settingsLangText != null) settingsLangText.text = LocalizationManager.Get("settings_language");
            if (settingsVersionText != null) settingsVersionText.text = LocalizationManager.Get("settings_version");
            if (settingsQuitButtonText != null) settingsQuitButtonText.text = LocalizationManager.Get("settings_btn_quit");
            UpdateScreenSettingsUI();

            // Update Quit Modal Texts
            if (quitModalTitleText != null) quitModalTitleText.text = LocalizationManager.Get("quit_modal_title");
            if (quitModalDescText != null) quitModalDescText.text = LocalizationManager.Get("quit_modal_desc");
            if (quitModalYesText != null) quitModalYesText.text = LocalizationManager.Get("quit_modal_yes");
            if (quitModalNoText != null) quitModalNoText.text = LocalizationManager.Get("quit_modal_no");

            // Update Shop Texts
            if (shopTitleText != null) shopTitleText.text = LocalizationManager.Get("shop_title");
            if (btnShopTabInGameText != null) btnShopTabInGameText.text = LocalizationManager.Get("shop_tab_game");
            if (btnShopTabLobbyText != null) btnShopTabLobbyText.text = LocalizationManager.Get("shop_tab_lobby");

            // Update Help Modal Texts
            if (helpTitleText != null) helpTitleText.text = LocalizationManager.Get("help_title");
            if (helpConfirmText != null) helpConfirmText.text = LocalizationManager.Get("help_confirm");
            if (btnConfirmHelp != null)
            {
                var txt = btnConfirmHelp.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = LocalizationManager.Get("help_confirm");
            }
            if (helpModal != null)
            {
                var card = helpModal.transform.Find("DialogCard");
                if (card != null)
                {
                    var sub = card.Find("Subtitle")?.GetComponent<TMP_Text>();
                    if (sub != null) sub.text = LocalizationManager.Get("help_subtitle");

                    for (int i = 0; i < 4; i++)
                    {
                        var row = card.Find($"HelpRow_{i}");
                        if (row != null)
                        {
                            var stepT = row.Find("StepTitle")?.GetComponent<TMP_Text>();
                            if (stepT != null) stepT.text = LocalizationManager.Get($"help_step_{i + 1}_title");
                            var stepD = row.Find("StepDesc")?.GetComponent<TMP_Text>();
                            if (stepD != null) stepD.text = LocalizationManager.Get($"help_step_{i + 1}_desc");
                        }
                    }
                }
            }

            // Update Profile Modal Texts
            if (profileTitleText != null) profileTitleText.text = LocalizationManager.Get("profile_title");
            RefreshProfileUI();

            // Update Floating Mascot Labels
            if (partyLabelTexts != null && partyLabelTexts.Length >= 4)
            {
                if (partyLabelTexts[0] != null) partyLabelTexts[0].text = LocalizationManager.Get("lobby_start");
                if (partyLabelTexts[1] != null) partyLabelTexts[1].text = LocalizationManager.Get("lobby_shop");
                if (partyLabelTexts[2] != null) partyLabelTexts[2].text = LocalizationManager.Get("lobby_settings");
                if (partyLabelTexts[3] != null) partyLabelTexts[3].text = LocalizationManager.Get("lobby_help");
            }

            // Refresh Bottom Action Button Text
            if (btnPlayGameText != null)
            {
                btnPlayGameText.text = GetMenuActionText(_selectedMenuIdx);
            }

            // Refresh Shop Buttons ("적용 중", "장착하기", "내 코인")
            RefreshThemeShopUI();
            RefreshLobbyThemeShopUI();

            // Update Lobby Logo based on language
            if (lobbyLogoImage != null && languageLogos != null && (int)lang >= 0 && (int)lang < languageLogos.Length)
            {
                if (languageLogos[(int)lang] != null)
                {
                    lobbyLogoImage.sprite = languageLogos[(int)lang];
                }
            }
        }

        public void SetupLanguageLogos(Image img, Sprite[] logos)
        {
            lobbyLogoImage = img;
            languageLogos = logos;
            if (lobbyLogoImage != null && languageLogos != null && (int)LocalizationManager.CurrentLanguage >= 0 && (int)LocalizationManager.CurrentLanguage < languageLogos.Length)
            {
                if (languageLogos[(int)LocalizationManager.CurrentLanguage] != null)
                {
                    lobbyLogoImage.sprite = languageLogos[(int)LocalizationManager.CurrentLanguage];
                }
            }
        }
    }
}
