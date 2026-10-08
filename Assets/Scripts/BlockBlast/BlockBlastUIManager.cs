using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlockBlast
{
    public class BlockBlastUIManager : MonoBehaviour
    {
        public static BlockBlastUIManager Instance { get; private set; }

        [Header("Score HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestScoreText;

        [Header("Time Limit & Skills")]
        [SerializeField] private Image timeBarFill;
        [SerializeField] private TMP_Text timeRemainingText;
        [SerializeField] private Image vignetteDangerOverlay;
        [SerializeField] private Button btnSkip;
        [SerializeField] private TMP_Text skipBadgeText;
        [SerializeField] private Button btnRotate;

        [Header("Special Mascot Skill (Board Clear)")]
        [SerializeField] private Button btnBoardClear;
        [SerializeField] private Image boardClearGlow;
        [SerializeField] private TMP_Text boardClearProgressText;
        [SerializeField] private Image boardClearFillImage;

        [Header("Special Skill Board Clear Cut-in")]
        [SerializeField] private GameObject specialSkillCutinRoot;
        [SerializeField] private Image specialSkillCutinMascotImg;
        [SerializeField] private Image specialSkillCutinAuraImg;
        [SerializeField] private TMP_Text specialSkillCutinTitle;
        [SerializeField] private RectTransform[] specialSkillSparkleStars;

        private int _specialLinesProgress = 0;
        private int _specialLinesRequired = 30;
        private bool _isSpecialSkillReady = false;

        [Header("Guide Tips")]
        [SerializeField] private TMP_Text guideTipText;
        [SerializeField] private CanvasGroup guideTipCanvasGroup;

        [Header("Combo & Shake")]
        [SerializeField] private TMP_Text comboPopupText;
        [SerializeField] private RectTransform boardContainer;

        [Header("Main Menu Modal")]
        [SerializeField] private GameObject mainMenuModal;
        [SerializeField] private Button btnStartGame;
        [SerializeField] private TMP_Text menuBestScoreText;

        [Header("Game Over Modal")]
        [SerializeField] private GameObject gameOverModal;
        [SerializeField] private TMP_Text modalFinalScoreText;
        [SerializeField] private TMP_Text modalBestScoreText;
        [SerializeField] private TMP_Text modalGoldRewardText;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnGameOverLobby;
        [SerializeField] private Button btnReviveAd;
        [SerializeField] private TMP_Text reviveAdText;
        [SerializeField] private TMP_Text reviveAdRewardText;

        [Header("Pause Modal")]
        [SerializeField] private GameObject pauseModal;
        [SerializeField] private Button btnPause;
        [SerializeField] private Button btnPauseResume;
        [SerializeField] private Button btnPauseRestart;
        [SerializeField] private Button btnPauseLobby;

        [Header("Restart Confirm Modal")]
        [SerializeField] private GameObject restartConfirmModal;
        [SerializeField] private Button btnRestartConfirmYes;
        [SerializeField] private Button btnRestartConfirmNo;

        [Header("Lobby Confirm Modal")]
        [SerializeField] private GameObject lobbyConfirmModal;
        [SerializeField] private Button btnLobbyConfirmYes;
        [SerializeField] private Button btnLobbyConfirmNo;

        [Header("White Flash Overlay")]
        [SerializeField] private Image whiteFlashOverlay;

        [Header("In-Game Root")]
        [SerializeField] private GameObject inGameRoot;

        [Header("Language Logos")]
        [SerializeField] private Image titleLogoImage;
        [SerializeField] private Sprite[] languageLogos;

        public void ShowInGameUI(bool show)
        {
            if (inGameRoot != null) inGameRoot.SetActive(show);
            if (!show)
            {
                Time.timeScale = 1f;
                _isTimerActive = false;
                _wasTimerActiveBeforePause = false;
                if (pauseModal != null) pauseModal.SetActive(false);
                if (restartConfirmModal != null) restartConfirmModal.SetActive(false);
                if (lobbyConfirmModal != null) lobbyConfirmModal.SetActive(false);
                if (gameOverModal != null) gameOverModal.SetActive(false);
            }
        }

        private static readonly string[] GuideTips = new string[]
        {
            "돌리기 버튼으로 블록 회전!",
            "한 줄을 채우면 블록이 팡팡!",
            "연속으로 터뜨려 콤보 보너스!",
            "시간 내에 서둘러 블록을 놓으세요!",
            "스킵 버튼으로 블록 교체!",
            "놓을 자리가 없으면 게임 종료!",
            "긴 콤보로 최고 점수 도전!"
        };

        private int _score = 0;
        private int _bestScore = 0;
        private float _currentTurnMaxTime = 180f;
        private float _turnRemainingTime = 180f;
        private bool _isTimerActive = false;

        [Header("Turn Timer Difficulty Settings")]
        [Tooltip("첫 턴 제한 시간 (초 단위, 기본값: 180초 = 3분)")]
        [SerializeField] private float initialTurnMaxTime = 180f;
        [Tooltip("1줄을 터뜨릴 때마다 줄어드는 시간 (초 단위, 기본값: 5초)")]
        [SerializeField] private float timeDecreasePerLine = 5f;
        [Tooltip("최소 턴 제한 시간 (초 단위, 기본값: 5초 - 도달 시 5초 안에 블록을 놓아야 함)")]
        [SerializeField] private float minTurnMaxTime = 5f;

        private int _totalLinesCleared = 0;

        public const int MAX_SKIP_STOCK = 3;
        public const int LINES_PER_SKIP_CHARGE = 10;
        private int _skipStock = 1;
        private int _skipLinesProgress = 0;
        private Coroutine _tipCoroutine;
        private int _currentTipIndex = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _bestScore = PlayerPrefs.GetInt("BlockBlast_Best", 0);
            AutoBindConfirmModalsAndOverlays();

            // Mobile & PC smooth 60 FPS cap
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private void AutoBindConfirmModalsAndOverlays()
        {
            if (inGameRoot == null)
            {
                var ig = transform.Find("InGameRoot");
                if (ig != null) inGameRoot = ig.gameObject;
            }

            if (inGameRoot != null)
            {
                if (restartConfirmModal == null)
                {
                    var t = inGameRoot.transform.Find("RestartConfirmModal");
                    if (t != null) restartConfirmModal = t.gameObject;
                }
                if (restartConfirmModal != null)
                {
                    if (btnRestartConfirmYes == null) btnRestartConfirmYes = restartConfirmModal.transform.Find("DialogCard/BtnYes")?.GetComponent<Button>();
                    if (btnRestartConfirmNo == null) btnRestartConfirmNo = restartConfirmModal.transform.Find("DialogCard/BtnNo")?.GetComponent<Button>();
                }

                if (lobbyConfirmModal == null)
                {
                    var t = inGameRoot.transform.Find("LobbyConfirmModal");
                    if (t != null) lobbyConfirmModal = t.gameObject;
                }
                if (lobbyConfirmModal != null)
                {
                    if (btnLobbyConfirmYes == null) btnLobbyConfirmYes = lobbyConfirmModal.transform.Find("DialogCard/BtnYes")?.GetComponent<Button>();
                    if (btnLobbyConfirmNo == null) btnLobbyConfirmNo = lobbyConfirmModal.transform.Find("DialogCard/BtnNo")?.GetComponent<Button>();
                }

                if (whiteFlashOverlay == null)
                {
                    whiteFlashOverlay = inGameRoot.transform.Find("WhiteFlashOverlay")?.GetComponent<Image>();
                }
            }
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
            UpdatePauseModalTexts();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
        }

        private void HandleLanguageChanged(GameLanguage lang)
        {
            UpdatePauseModalTexts();
        }

        private void UpdatePauseModalTexts()
        {
            if (btnPauseResume != null)
            {
                var txt = btnPauseResume.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = LocalizationManager.Get("ingame_resume");
            }
            if (btnPauseRestart != null)
            {
                var txt = btnPauseRestart.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = LocalizationManager.Get("ingame_restart");
            }
            if (btnPauseLobby != null)
            {
                var txt = btnPauseLobby.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = LocalizationManager.Get("ingame_lobby");
            }
            if (pauseModal != null)
            {
                var title = pauseModal.transform.Find("Dialog/Title")?.GetComponent<TMP_Text>();
                if (title != null) title.text = LocalizationManager.Get("ingame_pause");
                var sub = pauseModal.transform.Find("Dialog/Subtitle")?.GetComponent<TMP_Text>();
                if (sub != null) sub.text = LocalizationManager.Get("ingame_pause_sub");
            }
            if (restartConfirmModal != null)
            {
                var title = restartConfirmModal.transform.Find("DialogCard/Title")?.GetComponent<TMP_Text>();
                if (title != null) title.text = LocalizationManager.Get("confirm_restart_title");
                var sub = restartConfirmModal.transform.Find("DialogCard/Subtitle")?.GetComponent<TMP_Text>();
                if (sub != null) sub.text = LocalizationManager.Get("confirm_restart_sub");
                var desc = restartConfirmModal.transform.Find("DialogCard/Desc")?.GetComponent<TMP_Text>();
                if (desc != null) desc.text = LocalizationManager.Get("confirm_restart_desc");
                if (btnRestartConfirmYes != null)
                {
                    var txt = btnRestartConfirmYes.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("confirm_restart_yes");
                }
                if (btnRestartConfirmNo != null)
                {
                    var txt = btnRestartConfirmNo.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("confirm_restart_no");
                }
            }
            if (lobbyConfirmModal != null)
            {
                var title = lobbyConfirmModal.transform.Find("DialogCard/Title")?.GetComponent<TMP_Text>();
                if (title != null) title.text = LocalizationManager.Get("confirm_lobby_title");
                var sub = lobbyConfirmModal.transform.Find("DialogCard/Subtitle")?.GetComponent<TMP_Text>();
                if (sub != null) sub.text = LocalizationManager.Get("confirm_lobby_sub");
                var desc = lobbyConfirmModal.transform.Find("DialogCard/Desc")?.GetComponent<TMP_Text>();
                if (desc != null) desc.text = LocalizationManager.Get("confirm_lobby_desc");
                if (btnLobbyConfirmYes != null)
                {
                    var txt = btnLobbyConfirmYes.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("confirm_lobby_yes");
                }
                if (btnLobbyConfirmNo != null)
                {
                    var txt = btnLobbyConfirmNo.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("confirm_lobby_no");
                }
            }
            if (gameOverModal != null)
            {
                var title = gameOverModal.transform.Find("Dialog/Title")?.GetComponent<TMP_Text>();
                if (title != null) title.text = LocalizationManager.Get("ingame_gameover_no_moves");
                var sub = gameOverModal.transform.Find("Dialog/Subtitle")?.GetComponent<TMP_Text>();
                if (sub != null) sub.text = LocalizationManager.Get("ingame_gameover_sub");
                var fLabel = gameOverModal.transform.Find("Dialog/ScoreCard/FLabel")?.GetComponent<TMP_Text>();
                if (fLabel != null) fLabel.text = LocalizationManager.Get("ingame_gameover_score");
                if (btnRestart != null)
                {
                    var txt = btnRestart.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("ingame_restart");
                }
                if (btnGameOverLobby != null)
                {
                    var txt = btnGameOverLobby.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = LocalizationManager.Get("ingame_lobby");
                }
                if (btnReviveAd != null)
                {
                    if (reviveAdText != null) reviveAdText.text = LocalizationManager.Get("ingame_continue_ad");
                    if (reviveAdRewardText != null) reviveAdRewardText.text = LocalizationManager.Get("ingame_continue_reward_badge");
                }
            }
            if (btnPause != null)
            {
                var pLabel = btnPause.transform.Find("PauseLabel")?.GetComponent<TMP_Text>();
                if (pLabel != null) pLabel.text = LocalizationManager.Get("ingame_pause");
            }
            if (inGameRoot != null)
            {
                var scoreLbl = inGameRoot.transform.Find("Header/ScoreBox/Label")?.GetComponent<TMP_Text>();
                if (scoreLbl != null) scoreLbl.text = LocalizationManager.Get("ingame_score");
                var bestLbl = inGameRoot.transform.Find("Header/BestBox/Label")?.GetComponent<TMP_Text>();
                if (bestLbl != null) bestLbl.text = LocalizationManager.Get("ingame_best");
                var timeLbl = inGameRoot.transform.Find("SkillsBar/TimeLabel")?.GetComponent<TMP_Text>();
                if (timeLbl != null) timeLbl.text = LocalizationManager.Get("ingame_time");
            }
            if (mainMenuModal != null)
            {
                var touchText = mainMenuModal.transform.Find("TouchPrompt/TouchText")?.GetComponent<TMP_Text>();
                if (touchText != null) touchText.text = LocalizationManager.Get("intro_touch");
            }
            if (guideTipText != null)
            {
                guideTipText.text = LocalizationManager.Get($"ingame_tip_{_currentTipIndex}");
            }

            // Update In-Game Header Title Logo based on language
            GameLanguage curLang = LocalizationManager.CurrentLanguage;
            if (titleLogoImage != null && languageLogos != null && (int)curLang >= 0 && (int)curLang < languageLogos.Length)
            {
                if (languageLogos[(int)curLang] != null)
                {
                    titleLogoImage.sprite = languageLogos[(int)curLang];
                }
            }
        }

        public void SetupLanguageLogos(Image img, Sprite[] logos)
        {
            titleLogoImage = img;
            languageLogos = logos;
            GameLanguage curLang = LocalizationManager.CurrentLanguage;
            if (titleLogoImage != null && languageLogos != null && (int)curLang >= 0 && (int)curLang < languageLogos.Length)
            {
                if (languageLogos[(int)curLang] != null)
                {
                    titleLogoImage.sprite = languageLogos[(int)curLang];
                }
            }
        }

        private void Start()
        {
            if (skipBadgeText != null)
            {
                skipBadgeText.gameObject.SetActive(false);
                if (skipBadgeText.transform.parent != null && skipBadgeText.transform.parent.name == "SkipStockBadge")
                {
                    skipBadgeText.transform.parent.gameObject.SetActive(false);
                }
            }

            UpdateScoreUI();
            UpdateTurnMaxTime();
            UpdateTimerUI();
            UpdateSkipUI();

            if (gameOverModal != null) gameOverModal.SetActive(false);
            if (comboPopupText != null) comboPopupText.gameObject.SetActive(false);
            if (vignetteDangerOverlay != null) vignetteDangerOverlay.gameObject.SetActive(false);

            if (guideTipText != null)
            {
                if (_tipCoroutine != null) StopCoroutine(_tipCoroutine);
                _tipCoroutine = StartCoroutine(CycleGuideTipsRoutine());
            }

            if (mainMenuModal != null)
            {
                mainMenuModal.SetActive(true);
                if (menuBestScoreText != null) menuBestScoreText.text = $"최고 점수: {_bestScore}";
            }

            if (btnStartGame != null)
            {
                btnStartGame.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    StartGameFromMenu();
                });
            }

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.OnScoreAdded += AddScore;
                BlockGridManager.Instance.OnLinesCleared += HandleLinesCleared;
                BlockGridManager.Instance.OnShapePlaced += HandleShapePlaced;
            }

            if (btnSkip != null)
            {
                btnSkip.onClick.AddListener(TryUseSkip);
            }

            if (btnBoardClear != null)
            {
                btnBoardClear.onClick.RemoveAllListeners();
                btnBoardClear.onClick.AddListener(UseBoardClearSkill);
            }
            UpdateSpecialSkillUI();

            if (btnRotate != null)
            {
                btnRotate.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayRotate();
                    if (BlockSpawner.Instance != null)
                    {
                        BlockSpawner.Instance.RotateHandBlocks();
                    }
                });
            }

            if (btnRestart != null)
            {
                btnRestart.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    RestartGame();
                });
            }

            if (btnGameOverLobby != null)
            {
                btnGameOverLobby.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    if (gameOverModal != null) gameOverModal.SetActive(false);
                    if (LobbyManager.Instance != null)
                    {
                        LobbyManager.Instance.ReturnToLobby();
                    }
                });
            }

            if (btnReviveAd != null)
            {
                btnReviveAd.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    ExecuteReviveAd();
                });
            }

            if (pauseModal != null) pauseModal.SetActive(false);

            if (btnPause != null)
            {
                btnPause.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    OpenPauseModal();
                });
            }

            if (btnPauseResume != null)
            {
                btnPauseResume.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    ClosePauseModal();
                });
            }

            if (btnPauseRestart != null)
            {
                btnPauseRestart.onClick.RemoveAllListeners();
                btnPauseRestart.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    OpenRestartConfirmModal();
                });
            }

            if (restartConfirmModal != null) restartConfirmModal.SetActive(false);

            if (btnRestartConfirmYes != null)
            {
                btnRestartConfirmYes.onClick.RemoveAllListeners();
                btnRestartConfirmYes.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    RestartGameWithFlash();
                });
            }

            if (btnRestartConfirmNo != null)
            {
                btnRestartConfirmNo.onClick.RemoveAllListeners();
                btnRestartConfirmNo.onClick.AddListener(() =>
                {
                    CloseRestartConfirmModal();
                });
            }

            if (btnPauseLobby != null)
            {
                btnPauseLobby.onClick.RemoveAllListeners();
                btnPauseLobby.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    OpenLobbyConfirmModal();
                });
            }

            if (lobbyConfirmModal != null) lobbyConfirmModal.SetActive(false);

            if (btnLobbyConfirmYes != null)
            {
                btnLobbyConfirmYes.onClick.RemoveAllListeners();
                btnLobbyConfirmYes.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    CloseLobbyConfirmModal();
                    ClosePauseModal();
                    _isTimerActive = false;
                    if (LobbyManager.Instance != null)
                    {
                        LobbyManager.Instance.ReturnToLobby();
                    }
                });
            }

            if (btnLobbyConfirmNo != null)
            {
                btnLobbyConfirmNo.onClick.RemoveAllListeners();
                btnLobbyConfirmNo.onClick.AddListener(() =>
                {
                    CloseLobbyConfirmModal();
                });
            }
        }

        private void Update()
        {
            // 1. Guard: Only process in-game hotkeys and game timer when in-game UI is active!
            if (inGameRoot == null || !inGameRoot.activeInHierarchy) return;

            // 2. Global PC Keyboard Hotkeys
            bool isEscPressed = false;
            bool isRotatePressed = false;
            bool isSkipPressed = false;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                isEscPressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
                isRotatePressed = UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame || UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;
                isSkipPressed = UnityEngine.InputSystem.Keyboard.current.sKey.wasPressedThisFrame;
            }
#endif
            // Mobile Android Back Button & fallback
            if (!isEscPressed && Input.GetKeyDown(KeyCode.Escape))
            {
                isEscPressed = true;
            }
            if (!isRotatePressed && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)))
            {
                isRotatePressed = true;
            }
            if (!isSkipPressed && Input.GetKeyDown(KeyCode.S))
            {
                isSkipPressed = true;
            }

            // 3. Confirm Modals active: ESC closes them and returns to Pause Modal
            if (restartConfirmModal != null && restartConfirmModal.activeSelf)
            {
                if (isEscPressed)
                {
                    CloseRestartConfirmModal();
                }
                return;
            }
            if (lobbyConfirmModal != null && lobbyConfirmModal.activeSelf)
            {
                if (isEscPressed)
                {
                    CloseLobbyConfirmModal();
                }
                return;
            }

            // 4. When Pause Modal is active, game timer is frozen; ESC resumes the game
            if (pauseModal != null && pauseModal.activeSelf)
            {
                if (isEscPressed)
                {
                    ClosePauseModal();
                }
                return;
            }

            // 4. In-Game ESC Handling
            if (isEscPressed)
            {
                if (gameOverModal == null || !gameOverModal.activeSelf)
                {
                    OpenPauseModal();
                }
                return;
            }

            // Right-click cancels drag smoothly without pausing
            bool isRightClick = false;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                isRightClick = UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame;
            }
#else
            isRightClick = Input.GetMouseButtonDown(1);
#endif
            if (isRightClick && DraggableBlockUI.CurrentlyDraggedBlock != null)
            {
                // 드래그 중 오른쪽 클릭 → 회전 (취소 아님)
                DraggableBlockUI.CurrentlyDraggedBlock.RotateSelf();
            }

            if (isRotatePressed)
            {
                if (gameOverModal == null || !gameOverModal.activeSelf)
                {
                    var dragged = DraggableBlockUI.CurrentlyDraggedBlock;
                    if (dragged != null)
                    {
                        // 드래그 중인 블록만 회전 (다른 슬롯 블록은 건드리지 않음)
                        dragged.RotateSelf();
                    }
                    else if (btnRotate != null && btnRotate.interactable)
                    {
                        btnRotate.onClick.Invoke();
                    }
                }
            }

            if (isSkipPressed)
            {
                if (gameOverModal == null || !gameOverModal.activeSelf)
                {
                    TryUseSkip();
                }
            }

            if (!_isTimerActive) return;

            _turnRemainingTime -= Time.deltaTime;
            if (_turnRemainingTime <= 0f)
            {
                _turnRemainingTime = 0f;
                _isTimerActive = false;
                UpdateTimerUI();
                ShowGameOver();
                return;
            }

            UpdateTimerUI();
            UpdateDangerVignette();
        }

        private void HandleShapePlaced()
        {
            MobileDeviceManager.TriggerHapticLight();
            ResetTurnTimer();
        }

        private void UpdateTurnMaxTime()
        {
            // Pink Mascot (0): +1.0s base (+0.05s per upgrade level) extra time
            // Cloud Mascot (7): +2.0s base (+0.1s per upgrade level) extra cozy time
            int mascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0);
            float bonusSec = 0f;
            if (mascot == 0)
            {
                int lvl = LobbyManager.GetMascotLevel(0);
                bonusSec = 1.0f + (lvl - 1) * 0.05f;
            }
            else if (mascot == 7)
            {
                int lvl = LobbyManager.GetMascotLevel(7);
                bonusSec = 2.0f + (lvl - 1) * 0.1f;
            }
            _currentTurnMaxTime = Mathf.Max(minTurnMaxTime + bonusSec, (initialTurnMaxTime + bonusSec) - _totalLinesCleared * timeDecreasePerLine);
        }

        private void ResetTurnTimer()
        {
            UpdateTurnMaxTime();
            _turnRemainingTime = _currentTurnMaxTime;
            UpdateTimerUI();
            if (vignetteDangerOverlay != null && vignetteDangerOverlay.gameObject.activeSelf)
            {
                vignetteDangerOverlay.gameObject.SetActive(false);
            }
        }

        private void UpdateTimerUI()
        {
            if (timeBarFill != null)
            {
                timeBarFill.fillAmount = Mathf.Clamp01(_turnRemainingTime / _currentTurnMaxTime);

                if (_turnRemainingTime <= 3f)
                {
                    timeBarFill.color = new Color(1f, 0.25f, 0.25f, 1f); // Urgent red
                }
                else if (_turnRemainingTime <= 5f)
                {
                    timeBarFill.color = new Color(1f, 0.65f, 0.15f, 1f); // Warning orange
                }
                else
                {
                    timeBarFill.color = Color.white;
                }
            }

            if (timeRemainingText != null)
            {
                if (_turnRemainingTime > 10f)
                {
                    int min = Mathf.FloorToInt(_turnRemainingTime / 60f);
                    int sec = Mathf.FloorToInt(_turnRemainingTime % 60f);
                    timeRemainingText.text = $"{min:00}:{sec:00}";
                }
                else
                {
                    timeRemainingText.text = $"{_turnRemainingTime:00.0}s";
                }

                if (_turnRemainingTime <= 3f)
                {
                    timeRemainingText.color = new Color(1f, 0.25f, 0.25f, 1f); // Urgent red
                }
                else if (_turnRemainingTime <= 5f)
                {
                    timeRemainingText.color = new Color(1f, 0.65f, 0.15f, 1f); // Warning orange
                }
                else
                {
                    timeRemainingText.color = new Color(0.38f, 0.22f, 0.62f);
                }
            }
        }

        private void UpdateDangerVignette()
        {
            if (vignetteDangerOverlay == null) return;

            if (_turnRemainingTime <= 3.0f && _turnRemainingTime > 0f)
            {
                if (!vignetteDangerOverlay.gameObject.activeSelf)
                {
                    vignetteDangerOverlay.gameObject.SetActive(true);
                    if (BlockAudioManager.Instance != null)
                    {
                        BlockAudioManager.Instance.PlayWarning();
                    }
                }

                // Heartbeat pulse: faster and more intense as time runs out
                float pulseSpeed = Mathf.Lerp(12f, 7f, _turnRemainingTime / 3f);
                float pulse = 0.5f + 0.45f * Mathf.Sin(Time.time * pulseSpeed);
                vignetteDangerOverlay.color = new Color(1f, 1f, 1f, pulse);
            }
            else
            {
                if (vignetteDangerOverlay.gameObject.activeSelf)
                {
                    vignetteDangerOverlay.gameObject.SetActive(false);
                }
            }
        }

        public void AddScore(int points)
        {
            _score += points;
            if (_score > _bestScore)
            {
                _bestScore = _score;
                PlayerPrefs.SetInt("BlockBlast_Best", _bestScore);
            }
            UpdateScoreUI();
            UpdateTurnMaxTime();
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null) scoreText.text = _score.ToString();
            if (bestScoreText != null) bestScoreText.text = _bestScore.ToString();
        }

        public void TryUseSkip()
        {
            if (_skipStock <= 0)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                return;
            }

            // Cancel and discard any currently dragged block immediately
            DraggableBlockUI.CancelAllActiveDrags(immediate: true);

            _skipStock--;
            UpdateSkipUI();

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlaySkip();
            }

            if (btnSkip != null)
            {
                StopCoroutine("PunchSkipButtonAnim");
                StartCoroutine("PunchSkipButtonAnim");
            }

            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.SkipHandBlocks();
            }
        }

        private IEnumerator PunchSkipButtonAnim()
        {
            if (btnSkip == null) yield break;
            Transform tr = btnSkip.transform;
            Vector3 origScale = Vector3.one;
            float elapsed = 0f;
            float dur = 0.22f;
            while (elapsed < dur)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / dur;
                float s = 1f + Mathf.Sin(t * Mathf.PI) * 0.22f;
                tr.localScale = origScale * s;
                yield return null;
            }
            tr.localScale = origScale;
        }

        public void NotifyNoMovesAvailable()
        {
            // If the player has skip stocks, gently pulse the skip button to prompt them to use it
            if (_skipStock > 0 && btnSkip != null)
            {
                StopCoroutine("PulseSkipPromptRoutine");
                StartCoroutine("PulseSkipPromptRoutine");
            }
        }

        private IEnumerator PulseSkipPromptRoutine()
        {
            if (btnSkip == null) yield break;
            Transform tr = btnSkip.transform;
            Vector3 origScale = Vector3.one;
            for (int pulse = 0; pulse < 2; pulse++)
            {
                float elapsed = 0f;
                float dur = 0.22f;
                while (elapsed < dur)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / dur;
                    float s = 1f + Mathf.Sin(t * Mathf.PI) * 0.22f;
                    tr.localScale = origScale * s;
                    yield return null;
                }
            }
            tr.localScale = origScale;
        }

        private void UpdateSkipUI()
        {
            bool canSkip = (_skipStock > 0);

            if (btnSkip != null)
            {
                btnSkip.interactable = canSkip;
                var cg = btnSkip.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = canSkip ? 1.0f : 0.45f;
                }
            }

            int maxStock = MAX_SKIP_STOCK;
            if (PlayerPrefs.GetInt("Selected_Mascot_Idx", 0) == 1)
            {
                int lvl = LobbyManager.GetMascotLevel(1);
                maxStock = 3 + lvl;
            }

            if (skipBadgeText != null)
            {
                if (_skipStock >= maxStock)
                {
                    skipBadgeText.text = $"{maxStock}/{maxStock} MAX";
                    skipBadgeText.color = new Color(0.12f, 0.72f, 0.45f, 1f); // Vibrant Mint Green
                }
                else if (_skipStock > 0)
                {
                    skipBadgeText.text = $"{_skipStock}/{maxStock} ({_skipLinesProgress}/10)";
                    skipBadgeText.color = new Color(0.35f, 0.22f, 0.62f, 1f); // Deep Purple
                }
                else
                {
                    skipBadgeText.text = $"0/{maxStock} ({_skipLinesProgress}/10)";
                    skipBadgeText.color = new Color(0.85f, 0.25f, 0.35f, 1f); // Muted Crimson
                }
            }
        }

        private void HandleLinesCleared(int combo, int totalLines)
        {
            MobileDeviceManager.TriggerHapticLineClear();
            TriggerShake();

            // ⏱️ Line clear difficulty timer: 1줄 터뜨릴 때마다 5초씩 제한 시간 감소!
            _totalLinesCleared += totalLines;
            UpdateTurnMaxTime();
            _turnRemainingTime = _currentTurnMaxTime;
            UpdateTimerUI();

            // 🎲 Skip Stock System: +1 stock every 10 lines (Mint mascot max 3+level, otherwise max 3)
            int maxStock = MAX_SKIP_STOCK;
            if (PlayerPrefs.GetInt("Selected_Mascot_Idx", 0) == 1)
            {
                int lvl = LobbyManager.GetMascotLevel(1);
                maxStock = 3 + lvl;
            }

            if (_skipStock < maxStock)
            {
                _skipLinesProgress += totalLines;
                while (_skipLinesProgress >= LINES_PER_SKIP_CHARGE && _skipStock < maxStock)
                {
                    _skipLinesProgress -= LINES_PER_SKIP_CHARGE;
                    _skipStock++;
                }

                if (_skipStock >= maxStock)
                {
                    _skipLinesProgress = 0;
                }
            }
            UpdateSkipUI();

            // 🌟 Special Mascot Skill: Charge board clear skill (30 lines, reduced by upgrade level)
            int mascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0);
            if (mascot == 8)
            {
                int lvl = LobbyManager.GetMascotLevel(8);
                _specialLinesRequired = Mathf.Max(15, 30 - (lvl - 1) * 2);
                _specialLinesProgress += totalLines;
                if (_specialLinesProgress >= _specialLinesRequired)
                {
                    _specialLinesProgress = _specialLinesRequired;
                    _isSpecialSkillReady = true;
                }
                UpdateSpecialSkillUI();
            }

            if (comboPopupText != null)
            {
                StopCoroutine("ShowComboAnim");
                StartCoroutine(ShowComboAnim(combo, totalLines));
            }
        }

        private IEnumerator ShowComboAnim(int combo, int totalLines)
        {
            comboPopupText.gameObject.SetActive(true);

            string title = $"COMBO x{combo}!";
            Color col = new Color(1f, 0.55f, 0.7f); // Sweet Pink

            if (combo == 2) { title = $"SWEET! x2"; col = new Color(1f, 0.8f, 0.25f); } // Mango Yellow
            else if (combo == 3) { title = $"DELICIOUS! x3"; col = new Color(1f, 0.45f, 0.6f); } // Strawberry
            else if (combo == 4) { title = $"MARVELOUS! x4"; col = new Color(0.4f, 0.88f, 0.75f); } // Mint
            else if (combo >= 5) { title = $"UNBELIEVABLE! x{combo}"; col = new Color(0.7f, 0.55f, 1f); } // Lavender

            comboPopupText.text = $"{title}\n<size=30>+{totalLines} LINES</size>";
            comboPopupText.color = col;

            float elapsed = 0f;
            float dur = 0.8f;
            Vector3 startScale = Vector3.one * 0.4f;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                float s = Mathf.Sin(t * Mathf.PI) * 0.75f + 0.85f;
                comboPopupText.transform.localScale = startScale * s;
                yield return null;
            }

            comboPopupText.gameObject.SetActive(false);
        }

        public void TriggerShake()
        {
            if (boardContainer != null)
            {
                StopCoroutine("ShakeCoroutine");
                StartCoroutine("ShakeCoroutine");
            }
        }

        private IEnumerator ShakeCoroutine()
        {
            Vector3 origPos = boardContainer.localPosition;
            float elapsed = 0f;
            float dur = 0.2f;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float x = Random.Range(-8f, 8f);
                float y = Random.Range(-8f, 8f);
                boardContainer.localPosition = origPos + new Vector3(x, y, 0f);
                yield return null;
            }

            boardContainer.localPosition = origPos;
        }

        public void ShowGameOver()
        {
            _isTimerActive = false;
            if (vignetteDangerOverlay != null) vignetteDangerOverlay.gameObject.SetActive(false);

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();

            // 💰 Gold Reward Calculation: drop last 3 digits (score / 1000)
            int earnedGold = _score / 1000;
            if (earnedGold > 0)
            {
                if (LobbyManager.Instance != null)
                {
                    LobbyManager.Instance.AddCoins(earnedGold);
                }
                else
                {
                    int c = PlayerPrefs.GetInt("Mallang_Coins", 0);
                    PlayerPrefs.SetInt("Mallang_Coins", c + earnedGold);
                    PlayerPrefs.Save();
                }
            }

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
                EnsureReviveAdButton();
                UpdatePauseModalTexts();
                if (modalFinalScoreText != null) modalFinalScoreText.text = _score.ToString("N0");
                if (modalBestScoreText != null)
                {
                    string bestPrefix = LocalizationManager.Get("profile_best_score_prefix", "BEST");
                    modalBestScoreText.text = $"{bestPrefix}: {_bestScore:N0}";
                }
                if (modalGoldRewardText != null)
                {
                    modalGoldRewardText.text = $"+{earnedGold:N0} G";
                }
                if (btnReviveAd != null)
                {
                    btnReviveAd.interactable = true;
                }
            }
        }

        private void EnsureReviveAdButton()
        {
            if (gameOverModal == null) return;

            Transform dialog = gameOverModal.transform.Find("Dialog");
            if (dialog == null) return;

            Transform existing = dialog.Find("BtnReviveAd");
            if (existing != null)
            {
                if (btnReviveAd == null)
                {
                    btnReviveAd = existing.GetComponent<Button>();
                    if (btnReviveAd != null)
                    {
                        btnReviveAd.onClick.RemoveAllListeners();
                        btnReviveAd.onClick.AddListener(() =>
                        {
                            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                            ExecuteReviveAd();
                        });
                    }
                }
                if (reviveAdText == null)
                {
                    var t = existing.Find("ReviveText");
                    if (t != null) reviveAdText = t.GetComponent<TMP_Text>();
                }
                if (reviveAdRewardText == null)
                {
                    var rt = existing.Find("RewardPill/RewardTxt");
                    if (rt != null) reviveAdRewardText = rt.GetComponent<TMP_Text>();
                }
                return;
            }

            // Fallback: Dynamically generate BtnReviveAd if not baked into scene asset
            if (btnRestart != null)
            {
                GameObject clone = Instantiate(btnRestart.gameObject, dialog);
                clone.name = "BtnReviveAd";
                RectTransform rt = clone.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(0, -155);
                    rt.sizeDelta = new Vector2(530, 92);
                }

                // Adjust restart and lobby buttons down to maintain balanced spacing
                RectTransform restartRt = btnRestart.GetComponent<RectTransform>();
                if (restartRt != null) restartRt.anchoredPosition = new Vector2(0, -265);

                if (btnGameOverLobby != null)
                {
                    RectTransform lobbyRt = btnGameOverLobby.GetComponent<RectTransform>();
                    if (lobbyRt != null) lobbyRt.anchoredPosition = new Vector2(0, -368);
                }

                Image img = clone.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(0.18f, 0.82f, 0.65f, 1f);
                }

                btnReviveAd = clone.GetComponent<Button>();
                btnReviveAd.onClick.RemoveAllListeners();
                btnReviveAd.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    ExecuteReviveAd();
                });

                // Main Center Text: "이어하기"
                TMP_Text mainTxt = clone.GetComponentInChildren<TMP_Text>();
                if (mainTxt != null)
                {
                    mainTxt.gameObject.name = "ReviveText";
                    mainTxt.text = LocalizationManager.Get("ingame_continue_ad");
                    mainTxt.fontSize = 32;
                    mainTxt.color = Color.white;
                    reviveAdText = mainTxt;
                    RectTransform mrt = mainTxt.GetComponent<RectTransform>();
                    if (mrt != null) mrt.anchoredPosition = new Vector2(10, 0);
                }

                // Left Ad Badge: "[🎬 AD]"
                GameObject adBadge = new GameObject("AdBadge", typeof(RectTransform), typeof(Image));
                adBadge.transform.SetParent(clone.transform, false);
                RectTransform adRt = adBadge.GetComponent<RectTransform>();
                adRt.anchorMin = new Vector2(0, 0.5f);
                adRt.anchorMax = new Vector2(0, 0.5f);
                adRt.anchoredPosition = new Vector2(62, 0);
                adRt.sizeDelta = new Vector2(85, 46);
                Image adImg = adBadge.GetComponent<Image>();
                adImg.color = new Color(0.10f, 0.35f, 0.28f, 0.85f);
                if (img != null && img.sprite != null) { adImg.sprite = img.sprite; adImg.type = Image.Type.Sliced; }

                GameObject adTxtObj = new GameObject("BadgeTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                adTxtObj.transform.SetParent(adBadge.transform, false);
                TextMeshProUGUI adTmp = adTxtObj.GetComponent<TextMeshProUGUI>();
                adTmp.text = "AD";
                adTmp.fontSize = 22;
                adTmp.fontStyle = FontStyles.Bold;
                adTmp.alignment = TextAlignmentOptions.Center;
                adTmp.color = Color.white;
                if (mainTxt != null) adTmp.font = mainTxt.font;
                RectTransform adTxtRt = adTxtObj.GetComponent<RectTransform>();
                adTxtRt.anchorMin = Vector2.zero;
                adTxtRt.anchorMax = Vector2.one;
                adTxtRt.sizeDelta = Vector2.zero;
                adTxtRt.anchoredPosition = Vector2.zero;

                // Right Reward Pill: "+30 다이아"
                GameObject rewPill = new GameObject("RewardPill", typeof(RectTransform), typeof(Image));
                rewPill.transform.SetParent(clone.transform, false);
                RectTransform rewRt = rewPill.GetComponent<RectTransform>();
                rewRt.anchorMin = new Vector2(1f, 0.5f);
                rewRt.anchorMax = new Vector2(1f, 0.5f);
                rewRt.anchoredPosition = new Vector2(-75, 0);
                rewRt.sizeDelta = new Vector2(120, 46);
                Image rewImg = rewPill.GetComponent<Image>();
                rewImg.color = new Color(1f, 0.92f, 0.35f, 0.95f);
                if (img != null && img.sprite != null) { rewImg.sprite = img.sprite; rewImg.type = Image.Type.Sliced; }

                GameObject rewTxtObj = new GameObject("RewardTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
                rewTxtObj.transform.SetParent(rewPill.transform, false);
                TextMeshProUGUI rewTmp = rewTxtObj.GetComponent<TextMeshProUGUI>();
                rewTmp.text = LocalizationManager.Get("ingame_continue_reward_badge");
                rewTmp.fontSize = 22;
                rewTmp.fontStyle = FontStyles.Bold;
                rewTmp.alignment = TextAlignmentOptions.Center;
                rewTmp.color = new Color(0.45f, 0.25f, 0.05f);
                if (mainTxt != null) rewTmp.font = mainTxt.font;
                RectTransform rewTxtRt = rewTxtObj.GetComponent<RectTransform>();
                rewTxtRt.anchorMin = Vector2.zero;
                rewTxtRt.anchorMax = Vector2.one;
                rewTxtRt.sizeDelta = Vector2.zero;
                rewTxtRt.anchoredPosition = Vector2.zero;
                reviveAdRewardText = rewTmp;
            }
        }

        public void ExecuteReviveAd()
        {
            if (btnReviveAd != null) btnReviveAd.interactable = false;

            if (AdManager.Instance != null)
            {
                AdManager.Instance.ShowRewardedAd(
                    onRewardEarned: () =>
                    {
                        // Grant 30 diamonds as requested
                        if (LobbyManager.Instance != null)
                        {
                            LobbyManager.Instance.AddDiamonds(30);
                        }
                        else
                        {
                            int cur = PlayerPrefs.GetInt("Mallang_Diamonds", 0);
                            PlayerPrefs.SetInt("Mallang_Diamonds", cur + 30);
                            PlayerPrefs.Save();
                        }

                        ReviveGame();
                    },
                    onAdClosed: () =>
                    {
                        if (btnReviveAd != null) btnReviveAd.interactable = true;
                    },
                    onAdFailed: (err) =>
                    {
                        Debug.LogWarning($"[BlockBlastUIManager] Ad failed: {err}");
                        if (btnReviveAd != null) btnReviveAd.interactable = true;
                    }
                );
            }
            else
            {
                // Fallback if AdManager isn't running
                if (LobbyManager.Instance != null)
                {
                    LobbyManager.Instance.AddDiamonds(30);
                }
                else
                {
                    int cur = PlayerPrefs.GetInt("Mallang_Diamonds", 0);
                    PlayerPrefs.SetInt("Mallang_Diamonds", cur + 30);
                    PlayerPrefs.Save();
                }
                ReviveGame();
            }
        }

        public void ReviveGame()
        {
            if (gameOverModal != null) gameOverModal.SetActive(false);
            Time.timeScale = 1f;

            // 1. Reset timer and activate
            ResetTurnTimer();

            // 2. Clear cluttered board with fireworks explosion
            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearAllBlocksWithExplosion();
            }

            // 3. Reroll hand for fresh moves
            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.RerollHand();
            }

            // 4. Visual celebration
            if (FairyScreenTransition.Instance != null)
            {
                FairyScreenTransition.Instance.EmitCornerSparkles();
            }
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayLevelUp();
            }

            // 5. Update UI
            UpdateScoreUI();
            UpdateSkipUI();
        }

        public void RestartGame()
        {
            DraggableBlockUI.CancelAllActiveDrags(immediate: true);

            _score = 0;
            _totalLinesCleared = 0;
            int mascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0);
            if (mascot == 1)
            {
                int lvl = LobbyManager.GetMascotLevel(1);
                _skipStock = 1 + Mathf.Min(3, lvl);
            }
            else
            {
                _skipStock = 1;
            }
            _skipLinesProgress = 0;

            if (mascot == 8)
            {
                int lvl = LobbyManager.GetMascotLevel(8);
                _specialLinesRequired = Mathf.Max(15, 30 - (lvl - 1) * 2);
                _specialLinesProgress = _specialLinesRequired;
                _isSpecialSkillReady = true;
            }
            else
            {
                _specialLinesProgress = 0;
                _isSpecialSkillReady = false;
            }
            UpdateSpecialSkillUI();

            Time.timeScale = 1f;
            _wasTimerActiveBeforePause = false;
            if (pauseModal != null) pauseModal.SetActive(false);
            if (restartConfirmModal != null) restartConfirmModal.SetActive(false);
            if (lobbyConfirmModal != null) lobbyConfirmModal.SetActive(false);

            UpdateScoreUI();
            UpdateSkipUI();

            if (gameOverModal != null && gameOverModal.activeSelf)
            {
                if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                gameOverModal.SetActive(false);
            }
            else if (gameOverModal != null)
            {
                gameOverModal.SetActive(false);
            }

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ResetBoard();
            }

            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.ClearHand();
                BlockSpawner.Instance.SpawnNewHand();
            }

            // Restart ingame BGM from the very beginning
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayInGameBGM();
            }

            _isTimerActive = true;
            ResetTurnTimer();
        }

        /// <summary>완전 재시작: 하얀 플래시 후 BGM 포함 완전 리셋</summary>
        public void RestartGameWithFlash()
        {
            StartCoroutine(RestartWithWhiteFlash());
        }

        private IEnumerator RestartWithWhiteFlash()
        {
            // 1. Stop time and close modals immediately
            _isTimerActive = false;
            Time.timeScale = 0f;
            if (pauseModal != null) pauseModal.SetActive(false);
            if (restartConfirmModal != null) restartConfirmModal.SetActive(false);
            if (lobbyConfirmModal != null) lobbyConfirmModal.SetActive(false);

            // 2. Stop BGM with instant stop
            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.StopBGM(0f);
            }

            // 3. Fade IN white overlay (unscaled time so timeScale=0 doesn't block it)
            if (whiteFlashOverlay != null)
            {
                whiteFlashOverlay.transform.SetAsLastSibling();
                whiteFlashOverlay.gameObject.SetActive(true);
                float elapsed = 0f;
                float fadeDur = 0.35f;
                Color c = Color.white;
                c.a = 0f;
                whiteFlashOverlay.color = c;
                while (elapsed < fadeDur)
                {
                    elapsed += Time.unscaledDeltaTime;
                    c.a = Mathf.Clamp01(elapsed / fadeDur);
                    whiteFlashOverlay.color = c;
                    yield return null;
                }
                c.a = 1f;
                whiteFlashOverlay.color = c;
            }

            // 4. Brief white hold
            float holdStart = Time.unscaledTime;
            while (Time.unscaledTime - holdStart < 0.25f) yield return null;

            // 5. Full reset under the white screen
            Time.timeScale = 1f;
            RestartGame();

            // 6. Fade OUT white overlay
            if (whiteFlashOverlay != null)
            {
                whiteFlashOverlay.transform.SetAsLastSibling();
                float elapsed = 0f;
                float fadeDur = 0.45f;
                Color c = whiteFlashOverlay.color;
                while (elapsed < fadeDur)
                {
                    elapsed += Time.unscaledDeltaTime;
                    c.a = Mathf.Clamp01(1f - elapsed / fadeDur);
                    whiteFlashOverlay.color = c;
                    yield return null;
                }
                c.a = 0f;
                whiteFlashOverlay.color = c;
                whiteFlashOverlay.gameObject.SetActive(false);
            }
        }

        public void OpenRestartConfirmModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (restartConfirmModal != null)
            {
                restartConfirmModal.transform.SetAsLastSibling();
                restartConfirmModal.SetActive(true);
            }
        }

        public void CloseRestartConfirmModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (restartConfirmModal != null)
            {
                restartConfirmModal.SetActive(false);
            }
        }

        public void SetupRestartConfirmModal(GameObject modal, Button yesBtn, Button noBtn)
        {
            restartConfirmModal = modal;
            btnRestartConfirmYes = yesBtn;
            btnRestartConfirmNo = noBtn;

            if (restartConfirmModal != null) restartConfirmModal.SetActive(false);

            if (btnRestartConfirmYes != null)
            {
                btnRestartConfirmYes.onClick.RemoveAllListeners();
                btnRestartConfirmYes.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
                    RestartGameWithFlash();
                });
            }

            if (btnRestartConfirmNo != null)
            {
                btnRestartConfirmNo.onClick.RemoveAllListeners();
                btnRestartConfirmNo.onClick.AddListener(() =>
                {
                    CloseRestartConfirmModal();
                });
            }
        }

        public void OpenLobbyConfirmModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (lobbyConfirmModal != null)
            {
                lobbyConfirmModal.transform.SetAsLastSibling();
                lobbyConfirmModal.SetActive(true);
            }
        }

        public void CloseLobbyConfirmModal()
        {
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (lobbyConfirmModal != null)
            {
                lobbyConfirmModal.SetActive(false);
            }
        }

        public void SetupLobbyConfirmModal(GameObject modal, Button yesBtn, Button noBtn)
        {
            lobbyConfirmModal = modal;
            btnLobbyConfirmYes = yesBtn;
            btnLobbyConfirmNo = noBtn;

            if (lobbyConfirmModal != null) lobbyConfirmModal.SetActive(false);

            if (btnLobbyConfirmYes != null)
            {
                btnLobbyConfirmYes.onClick.RemoveAllListeners();
                btnLobbyConfirmYes.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    CloseLobbyConfirmModal();
                    ClosePauseModal();
                    _isTimerActive = false;
                    if (LobbyManager.Instance != null)
                    {
                        LobbyManager.Instance.ReturnToLobby();
                    }
                });
            }

            if (btnLobbyConfirmNo != null)
            {
                btnLobbyConfirmNo.onClick.RemoveAllListeners();
                btnLobbyConfirmNo.onClick.AddListener(() =>
                {
                    CloseLobbyConfirmModal();
                });
            }
        }

        public void SetupWhiteFlashOverlay(Image overlay)
        {
            whiteFlashOverlay = overlay;
            if (whiteFlashOverlay != null)
            {
                whiteFlashOverlay.color = new Color(1f, 1f, 1f, 0f);
                whiteFlashOverlay.gameObject.SetActive(false);
            }
        }

        public void StartGameFromMenu()
        {
            if (mainMenuModal != null)
            {
                mainMenuModal.SetActive(false);
            }
            if (inGameRoot != null)
            {
                inGameRoot.SetActive(true);
            }
            if (pauseModal != null)
            {
                pauseModal.SetActive(false);
            }
            if (gameOverModal != null)
            {
                gameOverModal.SetActive(false);
            }
            Time.timeScale = 1f;
            _wasTimerActiveBeforePause = false;
            RestartGame();
        }

        public void SetupMainMenu(GameObject menuModal, Button startBtn, TMP_Text menuBest)
        {
            mainMenuModal = menuModal;
            btnStartGame = startBtn;
            menuBestScoreText = menuBest;
        }

        public bool IsPaused => pauseModal != null && pauseModal.activeSelf;
        private bool _wasTimerActiveBeforePause = false;

        public void OpenPauseModal()
        {
            DraggableBlockUI.CancelAllActiveDrags(immediate: true);

            _wasTimerActiveBeforePause = _isTimerActive;
            _isTimerActive = false;
            Time.timeScale = 0f;
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (pauseModal != null) pauseModal.SetActive(true);
        }

        public void ClosePauseModal()
        {
            DraggableBlockUI.CancelAllActiveDrags(immediate: true);

            Time.timeScale = 1f;
            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayWindow();
            if (pauseModal != null) pauseModal.SetActive(false);
            if (_wasTimerActiveBeforePause)
            {
                _isTimerActive = true;
            }
        }

        public void SetupPauseModal(GameObject modal, Button pauseBtn, Button resumeBtn, Button restartBtn, Button lobbyBtn)
        {
            pauseModal = modal;
            btnPause = pauseBtn;
            btnPauseResume = resumeBtn;
            btnPauseRestart = restartBtn;
            btnPauseLobby = lobbyBtn;

            if (pauseModal != null) pauseModal.SetActive(false);

            if (btnPause != null)
            {
                btnPause.onClick.RemoveAllListeners();
                btnPause.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    OpenPauseModal();
                });
            }

            if (btnPauseResume != null)
            {
                btnPauseResume.onClick.RemoveAllListeners();
                btnPauseResume.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    ClosePauseModal();
                });
            }

            if (btnPauseRestart != null)
            {
                btnPauseRestart.onClick.RemoveAllListeners();
                btnPauseRestart.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    // Show confirmation dialog — don't close pause modal yet
                    OpenRestartConfirmModal();
                });
            }

            if (btnPauseLobby != null)
            {
                btnPauseLobby.onClick.RemoveAllListeners();
                btnPauseLobby.onClick.AddListener(() =>
                {
                    if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayUIClick();
                    OpenLobbyConfirmModal();
                });
            }
        }

        public void SetupReferences(TMP_Text score, TMP_Text best, Image tFill, TMP_Text tText, Image vignette, Button skip, TMP_Text sBadge, Button rotate, TMP_Text combo, RectTransform bContainer, GameObject modal, TMP_Text finalS, TMP_Text mBestS, Button restart, GameObject inGameR = null, Button gameOverLobby = null, TMP_Text goldRewardText = null, Button reviveBtn = null, TMP_Text reviveTxt = null, TMP_Text reviveRewardTxt = null)
        {
            scoreText = score;
            bestScoreText = best;
            timeBarFill = tFill;
            timeRemainingText = tText;
            vignetteDangerOverlay = vignette;
            btnSkip = skip;
            skipBadgeText = sBadge;
            btnRotate = rotate;
            comboPopupText = combo;
            boardContainer = bContainer;
            gameOverModal = modal;
            modalFinalScoreText = finalS;
            modalBestScoreText = mBestS;
            modalGoldRewardText = goldRewardText;
            btnRestart = restart;
            inGameRoot = inGameR;
            btnGameOverLobby = gameOverLobby;
            btnReviveAd = reviveBtn;
            reviveAdText = reviveTxt;
            reviveAdRewardText = reviveRewardTxt;
        }

        public void SetupGuideTip(TMP_Text tip, CanvasGroup group = null)
        {
            guideTipText = tip;
            guideTipCanvasGroup = group;
            if (guideTipText != null && gameObject.activeInHierarchy)
            {
                if (_tipCoroutine != null) StopCoroutine(_tipCoroutine);
                _tipCoroutine = StartCoroutine(CycleGuideTipsRoutine());
            }
        }

        private IEnumerator CycleGuideTipsRoutine()
        {
            if (guideTipText == null) yield break;

            int index = 0;
            _currentTipIndex = 0;
            guideTipText.text = LocalizationManager.Get("ingame_tip_0");

            while (true)
            {
                yield return new WaitForSeconds(5.0f);

                // Smooth fade out
                if (guideTipCanvasGroup != null)
                {
                    float elapsed = 0f;
                    while (elapsed < 0.25f)
                    {
                        elapsed += Time.deltaTime;
                        guideTipCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.25f);
                        yield return null;
                    }
                    guideTipCanvasGroup.alpha = 0f;
                }

                index = (index + 1) % GuideTips.Length;
                _currentTipIndex = index;
                guideTipText.text = LocalizationManager.Get($"ingame_tip_{index}");

                // Smooth fade in
                if (guideTipCanvasGroup != null)
                {
                    float elapsed = 0f;
                    while (elapsed < 0.25f)
                    {
                        elapsed += Time.deltaTime;
                        guideTipCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.25f);
                        yield return null;
                    }
                    guideTipCanvasGroup.alpha = 1f;
                }
            }
        }

        public void SetupSpecialSkillButton(Button btn, Image glow = null, TMP_Text progTxt = null, Image fillImg = null)
        {
            btnBoardClear = btn;
            boardClearGlow = glow;
            boardClearProgressText = progTxt;
            boardClearFillImage = fillImg;

            if (btnBoardClear != null)
            {
                btnBoardClear.onClick.RemoveAllListeners();
                btnBoardClear.onClick.AddListener(UseBoardClearSkill);
            }
            UpdateSpecialSkillUI();
        }

        public void UpdateSpecialSkillUI()
        {
            int mascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0);
            if (mascot != 8)
            {
                if (btnBoardClear != null) btnBoardClear.gameObject.SetActive(false);
                return;
            }

            if (btnBoardClear != null)
            {
                btnBoardClear.gameObject.SetActive(true);
                btnBoardClear.interactable = _isSpecialSkillReady;
            }

            if (boardClearGlow != null)
            {
                boardClearGlow.gameObject.SetActive(_isSpecialSkillReady);
            }

            if (boardClearProgressText != null)
            {
                if (_isSpecialSkillReady)
                {
                    boardClearProgressText.text = "올 클리어!";
                    boardClearProgressText.color = new Color(1f, 0.95f, 0.2f, 1f);
                }
                else
                {
                    boardClearProgressText.text = $"{_specialLinesProgress}/{_specialLinesRequired}줄";
                    boardClearProgressText.color = new Color(0.9f, 0.9f, 1f, 1f);
                }
            }

            if (boardClearFillImage != null)
            {
                boardClearFillImage.fillAmount = Mathf.Clamp01((float)_specialLinesProgress / Mathf.Max(1, _specialLinesRequired));
            }
        }

        public void SetupSpecialSkillCutin(GameObject root, Image mascotImg, Image auraImg, TMP_Text title, RectTransform[] stars)
        {
            specialSkillCutinRoot = root;
            specialSkillCutinMascotImg = mascotImg;
            specialSkillCutinAuraImg = auraImg;
            specialSkillCutinTitle = title;
            specialSkillSparkleStars = stars;
            if (specialSkillCutinRoot != null) specialSkillCutinRoot.SetActive(false);
        }

        public void UseBoardClearSkill()
        {
            int mascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0);
            if (mascot != 8 || !_isSpecialSkillReady) return;

            _isSpecialSkillReady = false;
            _specialLinesProgress = 0;
            UpdateSpecialSkillUI();

            StartCoroutine(PlaySpecialSkillCutinSequenceRoutine());
        }

        public void ForceTriggerBoardClearSkill()
        {
            _isSpecialSkillReady = true;
            _specialLinesProgress = _specialLinesRequired;
            UpdateSpecialSkillUI();
            StartCoroutine(PlaySpecialSkillCutinSequenceRoutine());
        }

        public void ShowCutinStaticForDebug()
        {
            if (specialSkillCutinRoot != null)
            {
                specialSkillCutinRoot.SetActive(true);
                if (specialSkillCutinMascotImg != null)
                {
                    specialSkillCutinMascotImg.rectTransform.localScale = Vector3.one;
                    specialSkillCutinMascotImg.rectTransform.anchoredPosition = Vector2.zero;
                }
                if (specialSkillCutinAuraImg != null)
                {
                    specialSkillCutinAuraImg.transform.localScale = Vector3.one;
                }
                if (specialSkillSparkleStars != null)
                {
                    for (int i = 0; i < specialSkillSparkleStars.Length; i++)
                    {
                        var star = specialSkillSparkleStars[i];
                        if (star == null) continue;
                        float baseAngle = (i / (float)specialSkillSparkleStars.Length) * Mathf.PI * 2f;
                        float radius = 180f;
                        star.anchoredPosition = new Vector2(Mathf.Cos(baseAngle) * radius, Mathf.Sin(baseAngle) * radius);
                        star.localScale = Vector3.one * 1.2f;
                    }
                }
            }
        }

        private IEnumerator PlaySpecialSkillCutinSequenceRoutine()
        {
            // 1. Cutin presentation: Mascot appears with cute particles & "뾰로롱~~" sound
            if (specialSkillCutinRoot != null)
            {
                specialSkillCutinRoot.SetActive(true);

                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayFairyChime();
                }

                RectTransform mrt = (specialSkillCutinMascotImg != null) ? specialSkillCutinMascotImg.rectTransform : null;
                if (mrt != null) mrt.localScale = Vector3.zero;

                // Pop-in bounce
                float popDur = 0.28f;
                float el = 0f;
                while (el < popDur)
                {
                    el += Time.deltaTime;
                    float t = el / popDur;
                    float s = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.25f;
                    if (t > 0.8f) s = Mathf.Lerp(1.25f, 1.0f, (t - 0.8f) / 0.2f);
                    if (mrt != null) mrt.localScale = Vector3.one * s;
                    yield return null;
                }
                if (mrt != null) mrt.localScale = Vector3.one;

                // Float & swirl particles ("뾰로롱~~")
                float floatDur = 0.75f;
                el = 0f;
                while (el < floatDur)
                {
                    el += Time.deltaTime;
                    float t = el / floatDur;

                    if (mrt != null)
                    {
                        mrt.anchoredPosition = new Vector2(0, Mathf.Sin(el * 6f) * 15f);
                    }
                    if (specialSkillCutinAuraImg != null)
                    {
                        specialSkillCutinAuraImg.transform.Rotate(0, 0, 50f * Time.deltaTime);
                    }

                    if (specialSkillSparkleStars != null)
                    {
                        for (int i = 0; i < specialSkillSparkleStars.Length; i++)
                        {
                            var star = specialSkillSparkleStars[i];
                            if (star == null) continue;
                            float baseAngle = (i / (float)specialSkillSparkleStars.Length) * Mathf.PI * 2f;
                            float angle = baseAngle + el * 3.5f;
                            float radius = Mathf.Lerp(80f, 260f, t);
                            star.anchoredPosition = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                            star.localScale = Vector3.one * (Mathf.Sin((t + i * 0.1f) * Mathf.PI) * 1.3f);
                            star.Rotate(0, 0, -180f * Time.deltaTime);
                        }
                    }
                    yield return null;
                }

                // Disappear with magical spin shrink & burst ("뾰로롱~~하면서 사라짐")
                float exitDur = 0.25f;
                el = 0f;
                while (el < exitDur)
                {
                    el += Time.deltaTime;
                    float t = el / exitDur;
                    float s = Mathf.Lerp(1.0f, 0f, t);
                    if (mrt != null)
                    {
                        mrt.localScale = Vector3.one * s;
                        mrt.Rotate(0, 0, 360f * Time.deltaTime * 3f);
                    }
                    if (specialSkillCutinAuraImg != null)
                    {
                        specialSkillCutinAuraImg.transform.localScale = Vector3.one * s;
                    }
                    yield return null;
                }

                specialSkillCutinRoot.SetActive(false);
            }

            // 2. Wipe the board with explosion cascade & VFX
            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearAllBlocksWithExplosion();
            }

            yield return StartCoroutine(DoBoardClearVFX());
        }

        private IEnumerator DoBoardClearVFX()
        {
            if (whiteFlashOverlay != null)
            {
                whiteFlashOverlay.gameObject.SetActive(true);
                whiteFlashOverlay.color = new Color(1f, 1f, 1f, 0.85f);
                float el = 0f;
                while (el < 0.45f)
                {
                    el += Time.deltaTime;
                    whiteFlashOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.85f, 0f, el / 0.45f));
                    yield return null;
                }
                whiteFlashOverlay.gameObject.SetActive(false);
            }

            TriggerShake();

            if (comboPopupText != null)
            {
                comboPopupText.text = "<color=#FFE600>★ BOARD ALL CLEAR! ★</color>";
                comboPopupText.transform.localScale = Vector3.one * 1.5f;
                comboPopupText.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.3f);
                comboPopupText.gameObject.SetActive(false);
            }
        }
    }
}
