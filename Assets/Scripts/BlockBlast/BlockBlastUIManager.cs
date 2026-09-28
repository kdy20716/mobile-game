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
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnGameOverLobby;

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
#else
            isEscPressed = Input.GetKeyDown(KeyCode.Escape);
            isRotatePressed = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space);
            isSkipPressed = Input.GetKeyDown(KeyCode.S);
#endif

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
                DraggableBlockUI.CurrentlyDraggedBlock.CancelDrag(immediate: false);
            }

            if (isRotatePressed)
            {
                if (btnRotate != null && btnRotate.interactable && (gameOverModal == null || !gameOverModal.activeSelf))
                {
                    btnRotate.onClick.Invoke();
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
            ResetTurnTimer();
        }

        private void UpdateTurnMaxTime()
        {
            // Starts at 180s (3 minutes). Decreases by 4s for every 250 points, down to minimum 8s!
            int steps = _score / 250;
            _currentTurnMaxTime = Mathf.Max(8f, 180f - steps * 4f);
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
                    timeRemainingText.color = new Color(1f, 0.25f, 0.25f, 1f);
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

        private void AddScore(int points)
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

            if (skipBadgeText != null)
            {
                if (_skipStock >= MAX_SKIP_STOCK)
                {
                    skipBadgeText.text = "3/3 MAX";
                    skipBadgeText.color = new Color(0.12f, 0.72f, 0.45f, 1f); // Vibrant Mint Green
                }
                else if (_skipStock > 0)
                {
                    skipBadgeText.text = $"{_skipStock}/3 ({_skipLinesProgress}/10)";
                    skipBadgeText.color = new Color(0.35f, 0.22f, 0.62f, 1f); // Deep Purple
                }
                else
                {
                    skipBadgeText.text = $"0/3 ({_skipLinesProgress}/10)";
                    skipBadgeText.color = new Color(0.85f, 0.25f, 0.35f, 1f); // Muted Crimson
                }
            }
        }

        private void HandleLinesCleared(int combo, int totalLines)
        {
            TriggerShake();

            // 🎲 Skip Stock System: +1 stock every 10 lines, up to max 3
            if (_skipStock < MAX_SKIP_STOCK)
            {
                _skipLinesProgress += totalLines;
                while (_skipLinesProgress >= LINES_PER_SKIP_CHARGE && _skipStock < MAX_SKIP_STOCK)
                {
                    _skipLinesProgress -= LINES_PER_SKIP_CHARGE;
                    _skipStock++;
                }

                if (_skipStock >= MAX_SKIP_STOCK)
                {
                    _skipLinesProgress = 0;
                }
            }
            UpdateSkipUI();

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

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
                UpdatePauseModalTexts();
                if (modalFinalScoreText != null) modalFinalScoreText.text = _score.ToString("N0");
                if (modalBestScoreText != null)
                {
                    string bestPrefix = LocalizationManager.Get("profile_best_score_prefix", "BEST");
                    modalBestScoreText.text = $"{bestPrefix}: {_bestScore:N0}";
                }
            }
        }

        public void RestartGame()
        {
            DraggableBlockUI.CancelAllActiveDrags(immediate: true);

            _score = 0;
            _skipStock = 1;
            _skipLinesProgress = 0;

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

        public void SetupReferences(TMP_Text score, TMP_Text best, Image tFill, TMP_Text tText, Image vignette, Button skip, TMP_Text sBadge, Button rotate, TMP_Text combo, RectTransform bContainer, GameObject modal, TMP_Text finalS, TMP_Text mBestS, Button restart, GameObject inGameR = null, Button gameOverLobby = null)
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
            btnRestart = restart;
            inGameRoot = inGameR;
            btnGameOverLobby = gameOverLobby;
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
    }
}
