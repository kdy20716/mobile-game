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
        private const string KEY_DIAMONDS = "Mallang_Diamonds";
        private const string KEY_LOGGED_IN = "Mallang_LoggedIn";
        public const string KEY_THEME_OWNED_PREFIX = "Mallang_Theme_Owned_";
        public const string KEY_EQUIPPED_THEME = "Mallang_EquippedTheme";
        public const string KEY_LOBBY_THEME_OWNED_PREFIX = "Mallang_LobbyTheme_Owned_";
        public const string KEY_EQUIPPED_LOBBY_THEME = "Mallang_EquippedLobbyTheme";
        public const string KEY_SELECTED_MASCOT = "Selected_Mascot_Idx";
        public const string KEY_MASCOT_OWNED_PREFIX = "Mallang_Mascot_Owned_";
        private const string KEY_MOBILE_INIT_ECONOMY = "Mallang_Mobile_Init_v2";
        public const string KEY_MASCOT_SHARDS_PREFIX = "Mallang_Mascot_Shards_";
        public const string KEY_MASCOT_LEVEL_PREFIX = "Mallang_Mascot_Level_";

        public static readonly int[] ThemePrices = new int[] { 0, 1000, 2000, 3000 };
        public static readonly string[] ThemeNames = new string[] { "몽환의 밤", "캔디 랜드", "크리스탈 바다", "별빛 우주" };

        public static readonly int[] LobbyThemePrices = new int[] { 0, 1000, 2000 };
        public static readonly string[] LobbyThemeNames = new string[] { "몽환의 방", "달콤 캔디룸", "신비 바다룸" };
        public static readonly string[] LobbyThemeDescs = new string[] { "기본 로비 - 아늑한 파스텔 방", "달콤한 디저트와 와플 무대", "신비로운 바다 궁전 무대" };

        public static readonly string[] MascotNames = new string[] { "핑크 말랑이", "민트 말랑이", "골드 말랑이", "퍼플 말랑이", "스페셜 말랑이" };
        public static readonly string[] MascotTitles = new string[] { "기본 말랑이", "스킵 마스터", "보물 사냥꾼", "매직 큐브", "올 클리어 엔젤" };
        public static readonly string[] MascotAbilities = new string[]
        {
            "고유능력: 추가 시간 +1초\n(첫 턴 3분 1초 / 최소 6초)",
            "고유능력: 추가 스킵 스택\n(시작 2개 / 최대 4개)",
            "고유능력: 라인 추가 점수\n(클리어 줄마다 +100점 추가)",
            "고유능력: 2×2 매직 블록\n(5% 확률로 2×2 보라 블록 3개 소환)",
            "고유능력: 보드 올 클리어\n(스킬 터치 시 모든 블록 폭파!\n30줄 클리어마다 쿨타임 충전)"
        };
        public static readonly int[] MascotPricesCoins = new int[] { 0, 1000, 2000, 3000, 0 };
        public static readonly int[] MascotPricesDiamonds = new int[] { 0, 50, 100, 150, 0 };

        public static int GetMascotShards(int idx)
        {
            return PlayerPrefs.GetInt(KEY_MASCOT_SHARDS_PREFIX + idx, 0);
        }

        public static void SetMascotShards(int idx, int count)
        {
            PlayerPrefs.SetInt(KEY_MASCOT_SHARDS_PREFIX + idx, Mathf.Max(0, count));
        }

        public static void AddMascotShards(int idx, int count)
        {
            SetMascotShards(idx, GetMascotShards(idx) + count);
        }

        public static int GetMascotLevel(int idx)
        {
            return PlayerPrefs.GetInt(KEY_MASCOT_LEVEL_PREFIX + idx, 1);
        }

        public static void SetMascotLevel(int idx, int level)
        {
            PlayerPrefs.SetInt(KEY_MASCOT_LEVEL_PREFIX + idx, Mathf.Max(1, level));
        }

        public static string GetMascotAbilityDescription(int idx, int level)
        {
            level = Mathf.Max(1, level);
            switch (idx)
            {
                case 0:
                    float sec = 1.0f + (level - 1) * 0.2f;
                    return $"고유능력: 추가 시간 +{sec:F1}초\n(첫 턴 {180 + sec:F1}초 / 최소 {5 + sec:F1}초)";
                case 1:
                    int startStock = 1 + Mathf.Min(3, level);
                    int maxStock = 3 + level;
                    return $"고유능력: 스킵 스택 강화\n(시작 {startStock}개 / 최대 {maxStock}개 보유)";
                case 2:
                    int bonus = 100 + (level - 1) * 25;
                    return $"고유능력: 라인 추가 점수\n(줄당 +{bonus}점 추가 보너스)";
                case 3:
                    int rate = 5 + (level - 1);
                    return $"고유능력: 2×2 매직 블록\n({rate}% 확률로 2×2 보라 블록 3개 소환)";
                case 4:
                    int lines = Mathf.Max(15, 30 - (level - 1) * 2);
                    return $"고유능력: 보드 올 클리어\n(스킬 터치 시 모든 블록 폭파!\n{lines}줄 클리어마다 쿨타임 충전)";
                default:
                    return "";
            }
        }

        [Header("Lobby Root & Groups")]
        [SerializeField] private GameObject lobbyRoot;
        [SerializeField] private CanvasGroup lobbyCanvasGroup;

        [Header("Top Right Profile, Coins & Diamonds")]
        [SerializeField] private Button profileBtn;
        [SerializeField] private Image profileBtnAvatar;
        [SerializeField] private TMP_Text lobbyCoinsText;
        [SerializeField] private TMP_Text lobbyDiamondsText;

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
        [SerializeField] private Image profileModalAvatarPlate;
        [SerializeField] private Image profileBtnAvatarPlate;
        [SerializeField] private TMP_Text profileModalNickname;
        [SerializeField] private TMP_InputField profileModalNicknameInput;
        [SerializeField] private TMP_InputField profileModalTagInput;
        [SerializeField] private Button btnStartEditNick;
        [SerializeField] private Button btnCheckTag;
        [SerializeField] private Button btnConfirmNick;
        [SerializeField] private TMP_Text profileModalNickStatus;
        [SerializeField] private TMP_InputField profileModalBioInput;
        [SerializeField] private TMP_Text profileModalBestScore;
        [SerializeField] private TMP_Text profileModalCoins;
        [SerializeField] private Button[] avatarSelectButtons;
        [SerializeField] private Button btnLogout;
        [SerializeField] private Button btnCloseProfile;

        [Header("Login Modal")]
        [SerializeField] private GameObject loginModal;
        [SerializeField] private TMP_InputField loginIdInput;
        [SerializeField] private TMP_InputField loginPwInput;
        [SerializeField] private Button btnSubmitLogin;
        [SerializeField] private Button btnGoogleLogin;
        [SerializeField] private Button btnCloseLogin;
        [SerializeField] private TMP_Text loginStatusText;

        [Header("Shop Modal & Currency")]
        [SerializeField] private GameObject shopModal;
        [SerializeField] private TMP_Text shopCoinsText;
        [SerializeField] private TMP_Text shopDiamondsText;
        [SerializeField] private Button btnCloseShop;

        [Header("Mascot Management Modal")]
        [SerializeField] private GameObject mascotModal;
        [SerializeField] private Button btnCloseMascotModal;
        [SerializeField] private Button[] mascotActionButtons;
        [SerializeField] private TMP_Text[] mascotActionTexts;
        [SerializeField] private TMP_Text[] mascotStatusTexts;
        [SerializeField] private Button[] mascotUpgradeButtons;
        [SerializeField] private TMP_Text[] mascotUpgradeTexts;
        [SerializeField] private TMP_Text[] mascotLevelTexts;
        [SerializeField] private TMP_Text[] mascotShardTexts;
        [SerializeField] private TMP_Text[] mascotAbilityTexts;

        [Header("Summon Result Modal")]
        [SerializeField] private GameObject summonResultModal;
        [SerializeField] private Button btnCloseSummonResult;
        [SerializeField] private TMP_Text summonResultTitleText;
        [SerializeField] private TMP_Text summonResultHighlightText;
        [SerializeField] private TMP_Text summonResultShardsText;
        [SerializeField] private Image summonResultMascotIcon;

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

        [Header("Shop 5 Vertical Tabs & Panels")]
        [SerializeField] private Button[] shopTabButtons;
        [SerializeField] private Image[] shopTabBgs;
        [SerializeField] private TMP_Text[] shopTabTexts;
        [SerializeField] private GameObject[] shopTabPanels;
        [SerializeField] private GameObject shopRecommendedPanel;
        [SerializeField] private GameObject shopPickupPanel;
        [SerializeField] private GameObject shopMascotsPanel;
        [SerializeField] private GameObject shopInGameThemesPanel;
        [SerializeField] private GameObject shopLobbyThemesPanel;

        [Header("Shop Packages & Summons & Mascot Items")]
        [SerializeField] private Button[] shopPackageButtons;
        [SerializeField] private Button btnSummon1;
        [SerializeField] private Button btnSummon10;
        [SerializeField] private Button[] mascotShopActionButtons;
        [SerializeField] private TMP_Text[] mascotShopActionTexts;

        [Header("Custom Shop Tab Sprites")]
        public bool useNanoBananaTabButtons = true;
        [SerializeField] private Sprite tabGameActiveSprite;
        [SerializeField] private Sprite tabLobbyActiveSprite;
        [SerializeField] private Sprite tabInactiveSprite;
        [SerializeField] private Sprite tabOriginalPillSprite;
        [SerializeField] private Sprite tabVerticalActiveSprite;
        [SerializeField] private Sprite tabVerticalInactiveSprite;

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
        private int _currentWindowModeIdx = 1; // 0: 창모드, 1: 테두리없는 창모드, 2: 전체화면 (Default: 테두리없는 창모드)

        [Header("Mascot Avatars (0:Pink, 1:Mint, 2:Gold, 3:Purple)")]
        [SerializeField] private Sprite[] mascotAvatars;

        [Header("Shop Action Button Sprites")]
        [SerializeField] private Sprite shopEquipBtnSprite;
        [SerializeField] private Sprite shopEquippedBtnSprite;

        private int _currentAvatarIdx = 0;
        private string _currentNickname = "";
        private string _nicknameOnly = "말랑이";
        private string _tagOnly = "8276";
        private string _currentBio = "";
        private int _currentCoins = 100; // Mobile default: 100 Gold
        private int _currentDiamonds = 10; // Mobile default: 10 Diamonds
        private bool _isLoggedIn = true;

        private bool _isEditingNick = false;
        private bool _isNickVerified = false;
        private string _verifiedNick = "";
        private string _verifiedTag = "";

        private const string KEY_NICKNAME_ONLY = "Lobby_NicknameOnly";
        private const string KEY_TAG_ONLY = "Lobby_TagOnly";

        // Simulated central database of existing players for duplicate checking
        // In production with your web server (Node.js/Express, Python, etc.), this is queried via UnityWebRequest!
        private static readonly HashSet<string> ReservedUserTagDatabase = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "말랑이#0001", "말랑이#1234", "말랑이#8276", "젤리왕#7777", "핑크퐁#1004", "민트초코#9999", "골드킹#0007", "퍼플베리#3333"
        };

        private int _selectedMenuIdx = 0;
        private Coroutine _mascotBounceCoroutine;
        private float _ignoreEscUntil = 0f;

        // Button Colors & Texts matching mascots (Adorable Pastel Palette)
        private static readonly Color ColorPink = new Color(1f, 0.52f, 0.68f, 1f);     // Soft Strawberry Milk (#FFA6C4)
        private static readonly Color ColorMint = new Color(0.42f, 0.88f, 0.78f, 1f);   // Soft Pastel Mint (#85E8D1)
        private static readonly Color ColorGold = new Color(1f, 0.82f, 0.42f, 1f);       // Soft Honey Butter (#FFDC85)
        private static readonly Color ColorPurple = new Color(0.78f, 0.60f, 0.98f, 1f);  // Soft Sweet Lavender (#C9ADFA)

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
            if (mascotModal != null) mascotModal.SetActive(false);
            if (summonResultModal != null) summonResultModal.SetActive(false);

            _currentAspectIdx = PlayerPrefs.GetInt(KEY_ASPECT_RATIO_INDEX, 3);
            if (_currentAspectIdx < 0 || _currentAspectIdx > 3) _currentAspectIdx = 3;
            _currentWindowModeIdx = PlayerPrefs.GetInt(KEY_WINDOW_MODE_INDEX, 1);
            if (_currentWindowModeIdx < 0 || _currentWindowModeIdx > 2) _currentWindowModeIdx = 1;
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
                _nicknameOnly = "말랑이";
                _tagOnly = Random.Range(1000, 9999).ToString();
                _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                _currentBio = "말랑블라스트에 오신 걸 환영해요!";
                _currentAvatarIdx = 0; // Pink Mascot default
                _currentCoins = 0;     // Steam default: 0 Gold
                _isLoggedIn = true;

                PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
                PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
                PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
                PlayerPrefs.SetString(KEY_BIO, _currentBio);
                PlayerPrefs.SetInt(KEY_AVATAR, _currentAvatarIdx);
                PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            }
            else
            {
                if (PlayerPrefs.HasKey(KEY_NICKNAME_ONLY) && PlayerPrefs.HasKey(KEY_TAG_ONLY))
                {
                    _nicknameOnly = PlayerPrefs.GetString(KEY_NICKNAME_ONLY, "말랑이");
                    _tagOnly = PlayerPrefs.GetString(KEY_TAG_ONLY, "8276");
                    _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                }
                else
                {
                    string raw = PlayerPrefs.GetString(KEY_NICKNAME, "말랑이#8276");
                    if (raw.Contains("#"))
                    {
                        var parts = raw.Split('#');
                        _nicknameOnly = parts[0].Trim();
                        _tagOnly = parts.Length > 1 ? parts[1].Trim() : Random.Range(1000, 9999).ToString();
                    }
                    else
                    {
                        _nicknameOnly = raw.Trim();
                        _tagOnly = Random.Range(1000, 9999).ToString();
                    }
                    PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
                    PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
                    _currentNickname = $"{_nicknameOnly}#{_tagOnly}";
                }

                _currentBio = PlayerPrefs.GetString(KEY_BIO, "말랑블라스트에 오신 걸 환영해요!");
                _currentNickname = System.Text.RegularExpressions.Regex.Replace(_currentNickname, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentBio = System.Text.RegularExpressions.Regex.Replace(_currentBio, @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()#+-]", "").Trim();
                _currentAvatarIdx = PlayerPrefs.GetInt(KEY_AVATAR, 0);
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 100);
                _currentDiamonds = PlayerPrefs.GetInt(KEY_DIAMONDS, 10);
                _isLoggedIn = PlayerPrefs.GetInt(KEY_LOGGED_IN, 1) == 1;
            }

            // Mobile Launch Economy Configuration: 100 Gold, 10 Diamonds, Theme 0 & Lobby 0 & Mascot 0 owned!
            if (PlayerPrefs.GetInt(KEY_MOBILE_INIT_ECONOMY, 0) == 0)
            {
                PlayerPrefs.SetInt(KEY_MOBILE_INIT_ECONOMY, 1);
                _currentCoins = 100;
                _currentDiamonds = 10;
                PlayerPrefs.SetInt(KEY_COINS, 100);
                PlayerPrefs.SetInt(KEY_DIAMONDS, 10);

                // Game Themes: Theme 0 owned (default free), 1, 2, 3 unowned (1k, 2k, 3k)
                PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + 0, 1);
                for (int i = 1; i < 10; i++)
                {
                    PlayerPrefs.SetInt(KEY_THEME_OWNED_PREFIX + i, 0);
                }
                PlayerPrefs.SetInt(KEY_EQUIPPED_THEME, 0);

                // Lobby Themes: Theme 0 owned (default free), 1, 2 unowned (1k, 2k)
                PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + 0, 1);
                for (int i = 1; i < 10; i++)
                {
                    PlayerPrefs.SetInt(KEY_LOBBY_THEME_OWNED_PREFIX + i, 0);
                }
                PlayerPrefs.SetInt(KEY_EQUIPPED_LOBBY_THEME, 0);

                // Mascots: Mascot 0 (Pink) owned (default free), 1, 2, 3, 4 locked
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 0, 1);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 1, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 2, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 3, 0);
                PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 4, 0);
                PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, 0);

                PlayerPrefs.Save();
            }
            else
            {
                _currentCoins = PlayerPrefs.GetInt(KEY_COINS, 100);
                _currentDiamonds = PlayerPrefs.GetInt(KEY_DIAMONDS, 10);
            }

            // Ensure player has diamonds to test new gacha features
            if (_currentDiamonds < 2000)
            {
                _currentDiamonds = 2000;
                PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);
                PlayerPrefs.Save();
            }
        }

        private void SetupEventListeners()
        {
            // Profile Button
            if (profileBtn != null)
            {
                profileBtn.onClick.RemoveAllListeners();
                profileBtn.onClick.AddListener(() => { PlayClickSound(); OpenProfileModal(); });
            }

            // Bottom Action Button
            if (btnPlayGame != null)
            {
                btnPlayGame.onClick.RemoveAllListeners();
                btnPlayGame.onClick.AddListener(() => { PlayClickSound(); ExecuteSelectedMenuAction(); });
            }

            // Profile Modal
            if (btnCloseProfile != null)
            {
                btnCloseProfile.onClick.RemoveAllListeners();
                btnCloseProfile.onClick.AddListener(() => { PlayClickSound(); CloseProfileModal(); });
            }
            if (btnStartEditNick != null)
            {
                btnStartEditNick.onClick.RemoveAllListeners();
                btnStartEditNick.onClick.AddListener(() => { PlayClickSound(); SetNickEditMode(true); });
            }
            if (btnCheckTag != null)
            {
                btnCheckTag.onClick.RemoveAllListeners();
                btnCheckTag.onClick.AddListener(() => { PlayClickSound(); CheckAndVerifyNicknameAndTag(); });
            }
            if (btnConfirmNick != null)
            {
                btnConfirmNick.onClick.RemoveAllListeners();
                btnConfirmNick.onClick.AddListener(() => { PlayClickSound(); OnConfirmNicknameChange(); });
            }
            if (profileModalNicknameInput != null)
            {
                profileModalNicknameInput.onValueChanged.RemoveAllListeners();
                profileModalNicknameInput.onValueChanged.AddListener(OnNickOrTagValueChanged);
            }
            if (profileModalTagInput != null)
            {
                profileModalTagInput.onValueChanged.RemoveAllListeners();
                profileModalTagInput.onValueChanged.AddListener(OnNickOrTagValueChanged);
            }
            if (profileModalBioInput != null)
            {
                profileModalBioInput.onEndEdit.RemoveAllListeners();
                profileModalBioInput.onEndEdit.AddListener(OnBioEndEdit);
            }
            if (btnLogout != null)
            {
                btnLogout.onClick.RemoveAllListeners();
                btnLogout.onClick.AddListener(OnLogoutClicked);
            }

            // Login Modal
            if (btnCloseLogin != null)
            {
                btnCloseLogin.onClick.RemoveAllListeners();
                btnCloseLogin.onClick.AddListener(CloseLoginModal);
            }
            if (btnSubmitLogin != null)
            {
                btnSubmitLogin.onClick.RemoveAllListeners();
                btnSubmitLogin.onClick.AddListener(OnSubmitLogin);
            }
            if (btnGoogleLogin != null)
            {
                btnGoogleLogin.onClick.RemoveAllListeners();
                btnGoogleLogin.onClick.AddListener(OnGoogleLoginClicked);
            }

            // Avatar Select Buttons
            if (avatarSelectButtons != null)
            {
                for (int i = 0; i < avatarSelectButtons.Length; i++)
                {
                    int idx = i;
                    if (avatarSelectButtons[i] != null)
                    {
                        avatarSelectButtons[i].onClick.RemoveAllListeners();
                        avatarSelectButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectAvatar(idx); });
                    }
                }
            }

            // Shop Modal
            if (btnCloseShop != null)
            {
                btnCloseShop.onClick.RemoveAllListeners();
                btnCloseShop.onClick.AddListener(() => { PlayClickSound(); CloseShopModal(); });
            }

            // Mascot Modal
            if (btnCloseMascotModal != null)
            {
                btnCloseMascotModal.onClick.RemoveAllListeners();
                btnCloseMascotModal.onClick.AddListener(() => { PlayClickSound(); CloseMascotModal(); });
            }

            // 5 Vertical Shop Tabs
            if (shopTabButtons != null)
            {
                for (int i = 0; i < shopTabButtons.Length; i++)
                {
                    int tabIdx = i;
                    if (shopTabButtons[i] != null)
                    {
                        shopTabButtons[i].onClick.RemoveAllListeners();
                        shopTabButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectShopTab(tabIdx); });
                    }
                }
            }

            // Shop Mascot Action Buttons
            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotShopActionButtons[i] != null)
                    {
                        mascotShopActionButtons[i].onClick.RemoveAllListeners();
                        mascotShopActionButtons[i].onClick.AddListener(() =>
                        {
                            BuyOrEquipMascot(idx);
                        });
                    }
                }
            }

            // Mascot Modal Equip Buttons
            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotActionButtons[i] != null)
                    {
                        mascotActionButtons[i].onClick.RemoveAllListeners();
                        mascotActionButtons[i].onClick.AddListener(() =>
                        {
                            EquipMascot(idx);
                        });
                    }
                }
            }

            // Summon Buttons
            if (btnSummon1 != null)
            {
                btnSummon1.onClick.RemoveAllListeners();
                btnSummon1.onClick.AddListener(() => SummonPickup(1));
            }
            if (btnSummon10 != null)
            {
                btnSummon10.onClick.RemoveAllListeners();
                btnSummon10.onClick.AddListener(() => SummonPickup(10));
            }

            // Package Buttons
            if (shopPackageButtons != null)
            {
                for (int i = 0; i < shopPackageButtons.Length; i++)
                {
                    int pIdx = i;
                    if (shopPackageButtons[i] != null)
                    {
                        shopPackageButtons[i].onClick.RemoveAllListeners();
                        shopPackageButtons[i].onClick.AddListener(() => BuyPackage(pIdx));
                    }
                }
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
                float defBgm = BlockAudioManager.Instance != null ? BlockAudioManager.Instance.bgmSliderLevel : BlockAudioManager.DEFAULT_SLIDER_PERCENT;
                bgmSlider.value = PlayerPrefs.GetFloat("BGM_Slider_Level", defBgm);
                bgmSlider.onValueChanged.RemoveAllListeners();
                bgmSlider.onValueChanged.AddListener((v) =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.SetBGMVolume(v);
                });
            }
            if (sfxSlider != null)
            {
                float defSfx = BlockAudioManager.Instance != null ? BlockAudioManager.Instance.sfxSliderLevel : BlockAudioManager.DEFAULT_SLIDER_PERCENT;
                sfxSlider.value = PlayerPrefs.GetFloat("SFX_Slider_Level", defSfx);
                sfxSlider.onValueChanged.RemoveAllListeners();
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
                case 2: return LocalizationManager.Get("lobby_mascot") + "!";
                case 3: return LocalizationManager.Get("lobby_settings") + "!";
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
                    OpenMascotModal();
                    break;
                case 3:
                    OpenSettingsModal();
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
                SetNickEditMode(false);
                SetNickStatus("", Color.white);
                RefreshProfileUI();
                profileModal.SetActive(true);
            }
        }

        public void CloseProfileModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            SetNickEditMode(false);
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

        public static string FormatCoins(int amount)
        {
            if (amount < 0) amount = 0;
            if (amount < 1000)
            {
                return amount.ToString();
            }
            if (amount < 10000)
            {
                return amount.ToString("N0"); // e.g. 1,234
            }
            if (amount < 1000000)
            {
                double val = amount / 1000.0;
                return val >= 100 ? $"{val:0}K" : $"{val:0.#}K"; // e.g. 10K, 12.5K
            }
            if (amount < 1000000000)
            {
                double val = amount / 1000000.0;
                return val >= 100 ? $"{val:0}M" : $"{val:0.##}M"; // e.g. 1M, 1.25M
            }
            return $"{amount / 1000000000.0:0.##}B";
        }

        public int Coins => _currentCoins;
        public int Diamonds => _currentDiamonds;

        public void AddCoins(int amount)
        {
            _currentCoins += amount;
            if (_currentCoins < 0) _currentCoins = 0;
            PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
            PlayerPrefs.Save();
            RefreshCurrenciesUI();
        }

        public void AddDiamonds(int amount)
        {
            _currentDiamonds += amount;
            if (_currentDiamonds < 0) _currentDiamonds = 0;
            PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);
            PlayerPrefs.Save();
            RefreshCurrenciesUI();
        }

        public void RefreshCurrenciesUI()
        {
            string coinStr = $"{FormatCoins(_currentCoins)} G";
            string diaStr = $"{FormatCoins(_currentDiamonds)}";

            if (lobbyCoinsText != null) lobbyCoinsText.text = coinStr;
            if (lobbyDiamondsText != null) lobbyDiamondsText.text = diaStr;
            if (shopCoinsText != null) shopCoinsText.text = coinStr;
            if (shopDiamondsText != null) shopDiamondsText.text = diaStr;
        }

        public void SetNickEditMode(bool isEditing)
        {
            _isEditingNick = isEditing;
            if (!_isEditingNick)
            {
                _isNickVerified = false;
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.text = _nicknameOnly;
                    profileModalNicknameInput.interactable = false;
                }
                if (profileModalTagInput != null)
                {
                    profileModalTagInput.text = _tagOnly;
                    profileModalTagInput.interactable = false;
                }
                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(true);
                if (btnCheckTag != null) btnCheckTag.gameObject.SetActive(false);
                if (btnConfirmNick != null) btnConfirmNick.gameObject.SetActive(false);
            }
            else
            {
                _isNickVerified = false;
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.interactable = true;
                    profileModalNicknameInput.ActivateInputField();
                }
                if (profileModalTagInput != null)
                {
                    profileModalTagInput.interactable = true;
                }
                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(false);
                if (btnCheckTag != null)
                {
                    btnCheckTag.gameObject.SetActive(true);
                    btnCheckTag.interactable = true;
                }
                if (btnConfirmNick != null)
                {
                    btnConfirmNick.gameObject.SetActive(true);
                    btnConfirmNick.interactable = false; // 중복 확인 완료 전에는 비활성화!
                }
                SetNickStatus("닉네임/태그 입력 후 [중복 확인]을 눌러주세요.", new Color(0.55f, 0.45f, 0.70f));
            }
        }

        private void OnNickOrTagValueChanged(string _)
        {
            if (_isEditingNick)
            {
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                SetNickStatus("[중복 확인]을 먼저 진행해주세요.", new Color(0.85f, 0.55f, 0.20f));
            }
        }

        public void CheckAndVerifyNicknameAndTag()
        {
            string newNick = System.Text.RegularExpressions.Regex.Replace(profileModalNicknameInput?.text ?? "", @"[^\u0000-\u007F\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\s.,!?:;~()+-]", "").Trim();
            string newTag = System.Text.RegularExpressions.Regex.Replace(profileModalTagInput?.text ?? "", @"[^0-9a-zA-Z]", "").Trim();

            if (string.IsNullOrWhiteSpace(newNick))
            {
                SetNickStatus("닉네임을 입력해주세요.", new Color(1f, 0.35f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                newTag = Random.Range(1000, 9999).ToString();
                if (profileModalTagInput != null) profileModalTagInput.text = newTag;
            }

            string combined = $"{newNick}#{newTag}";
            string currentSaved = $"{_nicknameOnly}#{_tagOnly}";

            if (combined.Equals(currentSaved, System.StringComparison.OrdinalIgnoreCase))
            {
                SetNickStatus("현재 사용 중인 본인의 닉네임입니다.", new Color(0.18f, 0.82f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                _isNickVerified = true;
                _verifiedNick = newNick;
                _verifiedTag = newTag;
                if (btnConfirmNick != null) btnConfirmNick.interactable = true;
                return;
            }

            // Central server duplicate check simulation
            if (ReservedUserTagDatabase.Contains(combined))
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                SetNickStatus("이미 있는 닉네임입니다.", new Color(1f, 0.28f, 0.38f)); // Coral Red
                _isNickVerified = false;
                if (btnConfirmNick != null) btnConfirmNick.interactable = false;
                return;
            }

            // Available & unique!
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            SetNickStatus("사용 가능한 닉네임입니다! [변경 완료]를 눌러주세요.", new Color(0.18f, 0.82f, 0.45f)); // Vibrant Mint Green
            _isNickVerified = true;
            _verifiedNick = newNick;
            _verifiedTag = newTag;
            if (btnConfirmNick != null) btnConfirmNick.interactable = true;
        }

        public void OnConfirmNicknameChange()
        {
            if (!_isNickVerified)
            {
                SetNickStatus("먼저 [중복 확인]을 진행해주세요.", new Color(1f, 0.35f, 0.45f));
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                return;
            }

            string oldCombined = $"{_nicknameOnly}#{_tagOnly}";
            string newCombined = $"{_verifiedNick}#{_verifiedTag}";

            ReservedUserTagDatabase.Remove(oldCombined);
            ReservedUserTagDatabase.Add(newCombined);

            _nicknameOnly = _verifiedNick;
            _tagOnly = _verifiedTag;
            _currentNickname = newCombined;

            PlayerPrefs.SetString(KEY_NICKNAME_ONLY, _nicknameOnly);
            PlayerPrefs.SetString(KEY_TAG_ONLY, _tagOnly);
            PlayerPrefs.SetString(KEY_NICKNAME, _currentNickname);
            PlayerPrefs.Save();

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            SetNickEditMode(false);
            SetNickStatus("닉네임이 성공적으로 변경되었습니다!", new Color(0.18f, 0.82f, 0.45f));
            RefreshProfileUI();
        }

        public void CheckAndSaveNicknameAndTag(string newNick, string newTag)
        {
            if (profileModalNicknameInput != null) profileModalNicknameInput.text = newNick;
            if (profileModalTagInput != null) profileModalTagInput.text = newTag;
            CheckAndVerifyNicknameAndTag();
            if (_isNickVerified)
            {
                OnConfirmNicknameChange();
            }
        }

        private void SetNickStatus(string msg, Color col)
        {
            if (profileModalNickStatus != null)
            {
                profileModalNickStatus.text = msg;
                profileModalNickStatus.color = col;
            }
        }

        // ==========================================
        // LOGIN MODAL & GOOGLE OAUTH
        // ==========================================

        public void OpenLoginModal()
        {
            PlayClickSound();
            if (loginModal != null)
            {
                loginModal.SetActive(true);
                if (loginStatusText != null) loginStatusText.text = "";
                if (loginIdInput != null) loginIdInput.text = "";
                if (loginPwInput != null) loginPwInput.text = "";
            }
        }

        public void CloseLoginModal()
        {
            PlayClickSound();
            if (loginModal != null) loginModal.SetActive(false);
        }

        private void OnSubmitLogin()
        {
            PlayClickSound();
            string id = loginIdInput != null ? loginIdInput.text.Trim() : "";
            string pw = loginPwInput != null ? loginPwInput.text.Trim() : "";

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pw))
            {
                if (loginStatusText != null)
                {
                    loginStatusText.text = "아이디와 비밀번호를 모두 입력해주세요.";
                    loginStatusText.color = new Color(1f, 0.35f, 0.45f);
                }
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWarning();
                return;
            }

            _isLoggedIn = true;
            PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            PlayerPrefs.Save();

            if (loginStatusText != null)
            {
                loginStatusText.text = "로그인되었습니다!";
                loginStatusText.color = new Color(0.18f, 0.82f, 0.45f);
            }
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();

            CloseLoginModal();
            RefreshProfileUI();
        }

        private void OnGoogleLoginClicked()
        {
            PlayClickSound();

            // Google OAuth 2.0 Web Client URL
            // In WebGL/Browser, this opens Google account sign-in in Chrome / external browser!
            string googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com" +
                "&redirect_uri=https://your-game-server.com/auth/callback" +
                "&response_type=code" +
                "&scope=openid%20profile%20email";

            Application.OpenURL(googleAuthUrl);

            // In-game immediate session login feedback
            _isLoggedIn = true;
            PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
            PlayerPrefs.Save();

            if (loginStatusText != null)
            {
                loginStatusText.text = "구글 계정 연동 창이 열렸습니다!";
                loginStatusText.color = new Color(0.26f, 0.52f, 0.96f);
            }

            CloseLoginModal();
            RefreshProfileUI();
        }

        private void OnLogoutClicked()
        {
            PlayClickSound();
            OpenLoginModal();
        }

        private void RefreshProfileUI()
        {
            Sprite avatarSprite = (mascotAvatars != null && _currentAvatarIdx < mascotAvatars.Length)
                ? mascotAvatars[_currentAvatarIdx]
                : null;

            if (profileBtnAvatar != null && avatarSprite != null)
            {
                profileBtnAvatar.sprite = avatarSprite;
                profileBtnAvatar.color = Color.white;
            }

            if (lobbyCoinsText != null)
            {
                lobbyCoinsText.text = $"{FormatCoins(_currentCoins)} C";
            }

            if (profileModalAvatar != null && avatarSprite != null)
            {
                profileModalAvatar.sprite = avatarSprite;
            }

            if (!_isEditingNick)
            {
                if (profileModalNicknameInput != null)
                {
                    profileModalNicknameInput.text = _nicknameOnly;
                    profileModalNicknameInput.interactable = false;
                }

                if (profileModalTagInput != null)
                {
                    profileModalTagInput.text = _tagOnly;
                    profileModalTagInput.interactable = false;
                }

                if (btnStartEditNick != null) btnStartEditNick.gameObject.SetActive(true);
                if (btnCheckTag != null) btnCheckTag.gameObject.SetActive(false);
                if (btnConfirmNick != null) btnConfirmNick.gameObject.SetActive(false);
            }

            if (profileModalNickname != null)
            {
                profileModalNickname.text = $"{_nicknameOnly}#{_tagOnly}";
            }

            if (profileModalBioInput != null)
            {
                profileModalBioInput.text = _currentBio;
                profileModalBioInput.interactable = true;
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

            // Remove 보유 코인 row from profile modal as requested
            if (profileModalCoins != null)
            {
                profileModalCoins.gameObject.SetActive(false);
            }

            if (btnLogout != null)
            {
                TMP_Text btnText = btnLogout.GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    btnText.text = "로그인";
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
                shopCoinsText.text = $"{LocalizationManager.Get("my_coins")}: {FormatCoins(_currentCoins)} C";
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
            _currentShopTab = Mathf.Clamp(tabIndex, 0, 4);

            if (shopRecommendedPanel != null) shopRecommendedPanel.SetActive(_currentShopTab == 0);
            if (shopPickupPanel != null) shopPickupPanel.SetActive(_currentShopTab == 1);
            if (shopMascotsPanel != null) shopMascotsPanel.SetActive(_currentShopTab == 2);
            if (shopInGameThemesPanel != null) shopInGameThemesPanel.SetActive(_currentShopTab == 3);
            if (shopLobbyThemesPanel != null) shopLobbyThemesPanel.SetActive(_currentShopTab == 4);

            if (shopTabBgs != null)
            {
                for (int i = 0; i < shopTabBgs.Length; i++)
                {
                    if (shopTabBgs[i] == null) continue;
                    bool isActive = (i == _currentShopTab);
                    if (tabVerticalActiveSprite != null && tabVerticalInactiveSprite != null)
                    {
                        shopTabBgs[i].sprite = isActive ? tabVerticalActiveSprite : tabVerticalInactiveSprite;
                        shopTabBgs[i].color = Color.white;
                    }
                    else
                    {
                        shopTabBgs[i].color = isActive ? ColorPink : new Color(0.92f, 0.90f, 0.96f, 0.85f);
                    }
                }
            }

            if (shopTabTexts != null)
            {
                for (int i = 0; i < shopTabTexts.Length; i++)
                {
                    if (shopTabTexts[i] == null) continue;
                    bool isActive = (i == _currentShopTab);
                    shopTabTexts[i].color = isActive ? Color.white : new Color(0.40f, 0.30f, 0.55f, 1f);
                    shopTabTexts[i].fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                }
            }

            RefreshCurrenciesUI();

            switch (_currentShopTab)
            {
                case 0:
                    RefreshShopPackagesUI();
                    break;
                case 1:
                    RefreshShopSummonUI();
                    break;
                case 2:
                    RefreshMascotShopUI();
                    break;
                case 3:
                    RefreshThemeShopUI();
                    break;
                case 4:
                    RefreshLobbyThemeShopUI();
                    break;
            }
        }

        // ==========================================
        // MASCOT MANAGEMENT MODAL
        // ==========================================

        public void OpenMascotModal()
        {
            if (mascotModal != null)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(false);
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                RefreshMascotModalUI();
                mascotModal.SetActive(true);
            }
        }

        public void CloseMascotModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (mascotModal != null) mascotModal.SetActive(false);
            if (btnPlayGame != null) btnPlayGame.gameObject.SetActive(true);
        }

        public void EquipMascot(int mascotIdx)
        {
            if (mascotIdx < 0 || mascotIdx >= 5) return;
            bool isOwned = (mascotIdx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 0) == 1);
            if (!isOwned)
            {
                PlayClickSound();
                return;
            }

            PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, mascotIdx);
            PlayerPrefs.Save();
            PlayClickSound();
            if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
            RefreshMascotModalUI();
            RefreshMascotShopUI();
        }

        public void TryUpgradeMascot(int idx)
        {
            if (idx < 0 || idx >= 5) return;
            int shards = GetMascotShards(idx);
            if (shards >= 60)
            {
                SetMascotShards(idx, shards - 60);
                int curLvl = GetMascotLevel(idx);
                SetMascotLevel(idx, curLvl + 1);
                PlayerPrefs.Save();

                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();

                RefreshMascotModalUI();
                RefreshMascotShopUI();
            }
            else
            {
                PlayClickSound();
            }
        }

        public void RefreshMascotModalUI()
        {
            int selectedMascot = PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0);

            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    if (mascotActionButtons[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                    bool isSelected = (selectedMascot == i);

                    TMP_Text txt = (mascotActionTexts != null && i < mascotActionTexts.Length && mascotActionTexts[i] != null)
                        ? mascotActionTexts[i]
                        : mascotActionButtons[i].GetComponentInChildren<TMP_Text>();

                    Image btnImg = mascotActionButtons[i].GetComponent<Image>();

                    if (isSelected)
                    {
                        if (txt != null)
                        {
                            txt.text = "사용 중";
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f);
                        }
                        if (btnImg != null && shopEquippedBtnSprite != null)
                        {
                            btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = "선택하기";
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null && shopEquipBtnSprite != null)
                        {
                            btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotActionButtons[i].interactable = true;
                    }
                    else
                    {
                        if (txt != null)
                        {
                            txt.text = (i == 4) ? "소환 전용" : "미보유 (잠김)";
                            txt.color = new Color(0.45f, 0.45f, 0.50f, 1f);
                        }
                        if (btnImg != null)
                        {
                            btnImg.color = new Color(0.85f, 0.85f, 0.88f, 1f);
                        }
                        mascotActionButtons[i].interactable = false;
                    }
                }
            }

            if (mascotStatusTexts != null)
            {
                for (int i = 0; i < mascotStatusTexts.Length; i++)
                {
                    if (mascotStatusTexts[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                    bool isSelected = (selectedMascot == i);
                    if (isSelected)
                    {
                        mascotStatusTexts[i].text = "<color=#2ECC71>● 장착 중</color>";
                    }
                    else if (isOwned)
                    {
                        mascotStatusTexts[i].text = "<color=#3498DB>보유 중</color>";
                    }
                    else
                    {
                        mascotStatusTexts[i].text = (i == 4) ? "<color=#9B59B6>픽업 소환 전용</color>" : "<color=#95A5A6>상점에서 획득</color>";
                    }
                }
            }

            if (mascotLevelTexts != null)
            {
                for (int i = 0; i < mascotLevelTexts.Length; i++)
                {
                    if (mascotLevelTexts[i] == null) continue;
                    int lvl = GetMascotLevel(i);
                    mascotLevelTexts[i].text = $"Lv.{lvl}";
                }
            }

            if (mascotShardTexts != null)
            {
                for (int i = 0; i < mascotShardTexts.Length; i++)
                {
                    if (mascotShardTexts[i] == null) continue;
                    int shards = GetMascotShards(i);
                    mascotShardTexts[i].text = $"말랑 조각: <color=#3498DB>{shards}</color> / 60";
                }
            }

            if (mascotAbilityTexts != null)
            {
                for (int i = 0; i < mascotAbilityTexts.Length; i++)
                {
                    if (mascotAbilityTexts[i] == null) continue;
                    int lvl = GetMascotLevel(i);
                    mascotAbilityTexts[i].text = GetMascotAbilityDescription(i, lvl);
                }
            }

            if (mascotUpgradeButtons != null)
            {
                for (int i = 0; i < mascotUpgradeButtons.Length; i++)
                {
                    if (mascotUpgradeButtons[i] == null) continue;
                    int shards = GetMascotShards(i);
                    bool canUpgrade = shards >= 60;
                    mascotUpgradeButtons[i].interactable = canUpgrade;

                    if (mascotUpgradeTexts != null && i < mascotUpgradeTexts.Length && mascotUpgradeTexts[i] != null)
                    {
                        mascotUpgradeTexts[i].text = canUpgrade ? "돌파 가능!" : "돌파 (60)";
                        mascotUpgradeTexts[i].color = canUpgrade ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.45f, 0.45f, 0.50f, 1f);
                    }
                }
            }
        }

        // ==========================================
        // MASCOT SHOP (TAB 2)
        // ==========================================

        public void BuyOrEquipMascot(int mascotIdx)
        {
            if (mascotIdx < 0 || mascotIdx >= 5) return;
            bool isOwned = (mascotIdx == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 0) == 1);

            if (isOwned)
            {
                EquipMascot(mascotIdx);
            }
            else if (mascotIdx == 4)
            {
                // Navigate directly to Pickup tab!
                PlayClickSound();
                SelectShopTab(1);
            }
            else
            {
                int price = MascotPricesCoins[mascotIdx];
                if (_currentCoins >= price)
                {
                    _currentCoins -= price;
                    PlayerPrefs.SetInt(KEY_COINS, _currentCoins);
                    PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + mascotIdx, 1);
                    PlayerPrefs.SetInt(KEY_SELECTED_MASCOT, mascotIdx);
                    PlayerPrefs.Save();

                    RefreshCurrenciesUI();
                    RefreshMascotShopUI();
                    RefreshMascotModalUI();

                    if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
                }
                else
                {
                    PlayClickSound();
                    if (shopCoinsText != null) StartCoroutine(FlashCoinsTextRed());
                }
            }
        }

        public void RefreshMascotShopUI()
        {
            RefreshCurrenciesUI();
            int selectedMascot = PlayerPrefs.GetInt(KEY_SELECTED_MASCOT, 0);

            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    if (mascotShopActionButtons[i] == null) continue;
                    bool isOwned = (i == 0) || (PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + i, 0) == 1);
                    bool isSelected = (selectedMascot == i);

                    TMP_Text txt = (mascotShopActionTexts != null && i < mascotShopActionTexts.Length && mascotShopActionTexts[i] != null)
                        ? mascotShopActionTexts[i]
                        : mascotShopActionButtons[i].GetComponentInChildren<TMP_Text>();

                    Image btnImg = mascotShopActionButtons[i].GetComponent<Image>();

                    if (isSelected)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equipped");
                            txt.color = new Color(0.06f, 0.35f, 0.26f, 1f);
                        }
                        if (btnImg != null && shopEquippedBtnSprite != null)
                        {
                            btnImg.sprite = shopEquippedBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotShopActionButtons[i].interactable = false;
                    }
                    else if (isOwned)
                    {
                        if (txt != null)
                        {
                            txt.text = LocalizationManager.Get("shop_btn_equip");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null && shopEquipBtnSprite != null)
                        {
                            btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotShopActionButtons[i].interactable = true;
                    }
                    else if (i == 4)
                    {
                        if (txt != null)
                        {
                            txt.text = "픽업 소환 전용";
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null && shopEquipBtnSprite != null)
                        {
                            btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotShopActionButtons[i].interactable = true;
                    }
                    else
                    {
                        int price = MascotPricesCoins[i];
                        if (txt != null)
                        {
                            txt.text = $"{price:N0} G " + LocalizationManager.Get("shop_btn_buy");
                            txt.color = new Color(0.46f, 0.08f, 0.24f, 1f);
                        }
                        if (btnImg != null && shopEquipBtnSprite != null)
                        {
                            btnImg.sprite = shopEquipBtnSprite;
                            btnImg.color = Color.white;
                        }
                        mascotShopActionButtons[i].interactable = (_currentCoins >= price);
                    }
                }
            }
        }

        // ==========================================
        // RECOMMENDED PACKAGES (TAB 0) & PICKUP SUMMON (TAB 1)
        // ==========================================

        public void BuyPackage(int packIdx)
        {
            int[] packPricesDia = new int[] { 20, 50, 80 };
            if (packIdx < 0 || packIdx >= packPricesDia.Length) return;

            int price = packPricesDia[packIdx];
            if (_currentDiamonds >= price)
            {
                _currentDiamonds -= price;
                PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);

                if (packIdx == 0)
                {
                    AddCoins(1500);
                }
                else if (packIdx == 1)
                {
                    PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 1, 1);
                    AddCoins(1000);
                    RefreshMascotShopUI();
                    RefreshMascotModalUI();
                }
                else if (packIdx == 2)
                {
                    AddCoins(5000);
                }

                PlayerPrefs.Save();
                RefreshCurrenciesUI();
                RefreshShopPackagesUI();

                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayBuy();
            }
            else
            {
                PlayClickSound();
                if (shopDiamondsText != null) StartCoroutine(FlashDiamondsTextRed());
            }
        }

        public void RefreshShopPackagesUI()
        {
            RefreshCurrenciesUI();
        }

        public void SummonPickup(int count)
        {
            int cost = (count == 10) ? 1000 : 100 * count;
            if (_currentDiamonds >= cost)
            {
                _currentDiamonds -= cost;
                PlayerPrefs.SetInt(KEY_DIAMONDS, _currentDiamonds);

                int specialCount = 0;
                int duplicateSpecialCount = 0;
                int[] shardGains = new int[5]; // 0: Pink, 1: Mint, 2: Gold, 3: Purple, 4: Special
                List<GachaDropItem> dropsList = new List<GachaDropItem>();

                for (int c = 0; c < count; c++)
                {
                    float roll = Random.Range(0f, 100f);
                    if (roll < 0.1f) // 0.1% chance for Special Mascot!
                    {
                        bool alreadyOwned = PlayerPrefs.GetInt(KEY_MASCOT_OWNED_PREFIX + 4, 0) == 1;
                        if (alreadyOwned)
                        {
                            // Duplicate: instantly convert to 60 Special Shards for instant breakthrough!
                            AddMascotShards(4, 60);
                            shardGains[4] += 60;
                            duplicateSpecialCount++;
                            dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 4, shardCount = 60, isDuplicateSpecial = true });
                        }
                        else
                        {
                            // First time unlock!
                            PlayerPrefs.SetInt(KEY_MASCOT_OWNED_PREFIX + 4, 1);
                            specialCount++;
                            dropsList.Add(new GachaDropItem { isSpecial = true, mascotIndex = 4, shardCount = 0, isDuplicateSpecial = false });
                        }
                    }
                    else
                    {
                        // 99.9% chance: Drops 5 Shards of random basic mascot (0, 1, 2, or 3)
                        int basicIdx = Random.Range(0, 4);
                        AddMascotShards(basicIdx, 5);
                        shardGains[basicIdx] += 5;
                        dropsList.Add(new GachaDropItem { isSpecial = false, mascotIndex = basicIdx, shardCount = 5, isDuplicateSpecial = false });
                    }
                }

                PlayerPrefs.Save();
                RefreshCurrenciesUI();
                RefreshMascotShopUI();
                RefreshMascotModalUI();

                if (FairyScreenTransition.Instance != null) FairyScreenTransition.Instance.EmitCornerSparkles();
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayBuy();
                }

                if (GachaPresentationController.Instance != null)
                {
                    GachaPresentationController.Instance.StartGachaSequence(dropsList, () =>
                    {
                        ShowSummonResultModal(count, specialCount, duplicateSpecialCount, shardGains);
                    });
                }
                else
                {
                    ShowSummonResultModal(count, specialCount, duplicateSpecialCount, shardGains);
                }
            }
            else
            {
                PlayClickSound();
                if (shopDiamondsText != null) StartCoroutine(FlashDiamondsTextRed());
            }
        }

        public void SetupSummonResultModal(GameObject modal, Button closeBtn, TMP_Text titleTxt, TMP_Text hlTxt, TMP_Text shardsTxt, Image iconImg)
        {
            summonResultModal = modal;
            btnCloseSummonResult = closeBtn;
            summonResultTitleText = titleTxt;
            summonResultHighlightText = hlTxt;
            summonResultShardsText = shardsTxt;
            summonResultMascotIcon = iconImg;

            if (btnCloseSummonResult != null)
            {
                btnCloseSummonResult.onClick.RemoveAllListeners();
                btnCloseSummonResult.onClick.AddListener(CloseSummonResultModal);
            }
        }

        public void CloseSummonResultModal()
        {
            PlayClickSound();
            if (summonResultModal != null) summonResultModal.SetActive(false);
        }

        public void ShowSummonResultModal(int count, int specialCount, int duplicateSpecialCount, int[] shardGains)
        {
            if (summonResultModal == null) return;

            if (summonResultTitleText != null)
            {
                summonResultTitleText.text = $"{count}회 소환 결과";
            }

            if (summonResultHighlightText != null)
            {
                if (specialCount > 0)
                {
                    summonResultHighlightText.text = "<color=#FFE600>★ [스페셜 말랑이] 획득! ★</color>\n<size=22><color=#A6E3E9>최초 소환 성공! 강력한 보드 올 클리어 능력이 해금되었습니다!</color></size>";
                }
                else if (duplicateSpecialCount > 0)
                {
                    summonResultHighlightText.text = "<color=#FFE600>★ [스페셜 말랑이] 중복 획득! ★</color>\n<size=22><color=#A6E3E9>스페셜 조각 60개로 즉시 변환되어 바로 돌파 가능!</color></size>";
                }
                else
                {
                    summonResultHighlightText.text = "<color=#845EC2>말랑이 조각 획득 완료!</color>\n<size=20><color=#554B64>조각 60개를 모아 능력을 돌파하세요!</color></size>";
                }
            }

            if (summonResultMascotIcon != null)
            {
                if (specialCount > 0 || duplicateSpecialCount > 0 || shardGains[4] > 0)
                {
                    if (mascotAvatars != null && mascotAvatars.Length > 4 && mascotAvatars[4] != null)
                    {
                        summonResultMascotIcon.sprite = mascotAvatars[4];
                    }
                    summonResultMascotIcon.gameObject.SetActive(true);
                }
                else
                {
                    int maxIdx = 0;
                    for (int i = 1; i < 4; i++)
                    {
                        if (shardGains[i] > shardGains[maxIdx]) maxIdx = i;
                    }
                    if (mascotAvatars != null && maxIdx < mascotAvatars.Length && mascotAvatars[maxIdx] != null)
                    {
                        summonResultMascotIcon.sprite = mascotAvatars[maxIdx];
                    }
                    summonResultMascotIcon.gameObject.SetActive(true);
                }
            }

            if (summonResultShardsText != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    if (shardGains[i] > 0)
                    {
                        string colTag = (i == 4) ? "#E056FD" : (i == 0) ? "#FF6B8B" : (i == 1) ? "#20BF6B" : (i == 2) ? "#FA8231" : "#8854D0";
                        sb.AppendLine($"<color={colTag}>• {MascotNames[i]} 조각: +{shardGains[i]}개 (보유: {GetMascotShards(i)}개)</color>");
                    }
                }
                summonResultShardsText.text = sb.ToString().TrimEnd();
            }

            summonResultModal.SetActive(true);
        }

        public void RefreshShopSummonUI()
        {
            RefreshCurrenciesUI();
        }

        private IEnumerator FlashDiamondsTextRed()
        {
            if (shopDiamondsText == null) yield break;
            Color orig = shopDiamondsText.color;
            shopDiamondsText.color = new Color(1f, 0.25f, 0.35f);
            yield return new WaitForSeconds(0.4f);
            shopDiamondsText.color = orig;
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
                shopCoinsText.text = $"{LocalizationManager.Get("my_coins")}: {FormatCoins(_currentCoins)} C";
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

        public void SetupShopVerticalTabs(
            Button[] tabBtns, Image[] tabBgs, TMP_Text[] tabTxts,
            GameObject recPanel, GameObject pickPanel, GameObject mascPanel,
            GameObject inGamePanel, GameObject lobbyPanel,
            Sprite activeSprite, Sprite inactiveSprite,
            TMP_Text sCoinsTxt = null, TMP_Text sDiaTxt = null)
        {
            shopTabButtons = tabBtns;
            shopTabBgs = tabBgs;
            shopTabTexts = tabTxts;
            shopRecommendedPanel = recPanel;
            shopPickupPanel = pickPanel;
            shopMascotsPanel = mascPanel;
            shopInGameThemesPanel = inGamePanel;
            shopLobbyThemesPanel = lobbyPanel;
            tabVerticalActiveSprite = activeSprite;
            tabVerticalInactiveSprite = inactiveSprite;
            if (sCoinsTxt != null) shopCoinsText = sCoinsTxt;
            if (sDiaTxt != null) shopDiamondsText = sDiaTxt;

            if (shopTabButtons != null)
            {
                for (int i = 0; i < shopTabButtons.Length; i++)
                {
                    int tabIdx = i;
                    if (shopTabButtons[i] != null)
                    {
                        shopTabButtons[i].onClick.RemoveAllListeners();
                        shopTabButtons[i].onClick.AddListener(() => { PlayClickSound(); SelectShopTab(tabIdx); });
                    }
                }
            }

            SelectShopTab(0);
        }

        public void SetupShopMascots(Button[] actionBtns, TMP_Text[] actionTexts)
        {
            mascotShopActionButtons = actionBtns;
            mascotShopActionTexts = actionTexts;

            if (mascotShopActionButtons != null)
            {
                for (int i = 0; i < mascotShopActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotShopActionButtons[i] != null)
                    {
                        mascotShopActionButtons[i].onClick.RemoveAllListeners();
                        mascotShopActionButtons[i].onClick.AddListener(() => BuyOrEquipMascot(idx));
                    }
                }
            }

            RefreshMascotShopUI();
        }

        public void SetupShopPackagesAndSummon(
            Button[] packBtns, Button summon1Btn, Button summon10Btn)
        {
            shopPackageButtons = packBtns;
            btnSummon1 = summon1Btn;
            btnSummon10 = summon10Btn;

            if (shopPackageButtons != null)
            {
                for (int i = 0; i < shopPackageButtons.Length; i++)
                {
                    int pIdx = i;
                    if (shopPackageButtons[i] != null)
                    {
                        shopPackageButtons[i].onClick.RemoveAllListeners();
                        shopPackageButtons[i].onClick.AddListener(() => BuyPackage(pIdx));
                    }
                }
            }

            if (btnSummon1 != null)
            {
                btnSummon1.onClick.RemoveAllListeners();
                btnSummon1.onClick.AddListener(() => SummonPickup(1));
            }

            if (btnSummon10 != null)
            {
                btnSummon10.onClick.RemoveAllListeners();
                btnSummon10.onClick.AddListener(() => SummonPickup(10));
            }
        }

        public void SetupMascotModal(
            GameObject modal, Button closeBtn,
            Button[] actionBtns, TMP_Text[] actionTexts, TMP_Text[] statusTexts = null,
            Button[] upgradeBtns = null, TMP_Text[] upgradeTexts = null,
            TMP_Text[] levelTexts = null, TMP_Text[] shardTexts = null,
            TMP_Text[] abilityTexts = null)
        {
            mascotModal = modal;
            btnCloseMascotModal = closeBtn;
            mascotActionButtons = actionBtns;
            mascotActionTexts = actionTexts;
            mascotStatusTexts = statusTexts;
            mascotUpgradeButtons = upgradeBtns;
            mascotUpgradeTexts = upgradeTexts;
            mascotLevelTexts = levelTexts;
            mascotShardTexts = shardTexts;
            mascotAbilityTexts = abilityTexts;

            if (btnCloseMascotModal != null)
            {
                btnCloseMascotModal.onClick.RemoveAllListeners();
                btnCloseMascotModal.onClick.AddListener(() => { PlayClickSound(); CloseMascotModal(); });
            }

            if (mascotActionButtons != null)
            {
                for (int i = 0; i < mascotActionButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotActionButtons[i] != null)
                    {
                        mascotActionButtons[i].onClick.RemoveAllListeners();
                        mascotActionButtons[i].onClick.AddListener(() => EquipMascot(idx));
                    }
                }
            }

            if (mascotUpgradeButtons != null)
            {
                for (int i = 0; i < mascotUpgradeButtons.Length; i++)
                {
                    int idx = i;
                    if (mascotUpgradeButtons[i] != null)
                    {
                        mascotUpgradeButtons[i].onClick.RemoveAllListeners();
                        mascotUpgradeButtons[i].onClick.AddListener(() => TryUpgradeMascot(idx));
                    }
                }
            }

            RefreshMascotModalUI();
        }

        public void SetupShopTabs(
            Button tabInGame, Button tabLobby,
            Image tabInGameBg, Image tabLobbyBg,
            TMP_Text tabInGameTxt, TMP_Text tabLobbyTxt,
            GameObject inGamePanel, GameObject lobbyPanel)
        {
            shopInGameThemesPanel = inGamePanel;
            shopLobbyThemesPanel = lobbyPanel;
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
            if (lobbyRoot == null || !lobbyRoot.activeInHierarchy || Time.unscaledTime < _ignoreEscUntil) return;

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
            if (loginModal != null && loginModal.activeSelf)
            {
                CloseLoginModal();
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
            _ignoreEscUntil = Time.unscaledTime + 0.35f;
            if (quitModal != null) quitModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (shopModal != null) shopModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
            if (profileModal != null) profileModal.SetActive(false);

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
            if (quitModal != null) quitModal.SetActive(false);
            if (GachaPresentationController.Instance != null) GachaPresentationController.Instance.CloseModal();

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
            TMP_Text partyTip = null,
            TMP_Text dText = null)
        {
            lobbyRoot = root;
            lobbyCanvasGroup = cg;

            profileBtn = pBtn;
            profileBtnAvatar = pAvatar;
            lobbyCoinsText = cText;
            lobbyDiamondsText = dText;

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

        public void SetupProfileModalExtraReferences(
            TMP_InputField tagInput, Button checkBtn, TMP_Text statusTxt,
            Image modalPlate, Image btnPlate,
            GameObject lModal, TMP_InputField lId, TMP_InputField lPw,
            Button lSub, Button lGgl, Button lCls, TMP_Text lStat,
            Button startEditBtn = null, Button confirmNickBtn = null)
        {
            profileModalTagInput = tagInput;
            btnCheckTag = checkBtn;
            profileModalNickStatus = statusTxt;
            profileModalAvatarPlate = modalPlate;
            profileBtnAvatarPlate = btnPlate;

            loginModal = lModal;
            loginIdInput = lId;
            loginPwInput = lPw;
            btnSubmitLogin = lSub;
            btnGoogleLogin = lGgl;
            btnCloseLogin = lCls;
            loginStatusText = lStat;

            btnStartEditNick = startEditBtn;
            btnConfirmNick = confirmNickBtn;

            SetNickEditMode(false);
            SetupEventListeners();
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
            if (shopTabTexts != null)
            {
                string[] tabKeys = new string[] { "shop_tab_recommended", "shop_tab_pickup", "shop_tab_mascot", "shop_tab_game", "shop_tab_lobby" };
                for (int i = 0; i < shopTabTexts.Length && i < tabKeys.Length; i++)
                {
                    if (shopTabTexts[i] != null) shopTabTexts[i].text = LocalizationManager.Get(tabKeys[i]);
                }
            }

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
                if (partyLabelTexts[2] != null) partyLabelTexts[2].text = LocalizationManager.Get("lobby_mascot");
                if (partyLabelTexts[3] != null) partyLabelTexts[3].text = LocalizationManager.Get("lobby_settings");
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
