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

        [Header("In-Game Root")]
        [SerializeField] private GameObject inGameRoot;

        public void ShowInGameUI(bool show)
        {
            if (inGameRoot != null) inGameRoot.SetActive(show);
        }

        private static readonly string[] GuideTips = new string[]
        {
            "돌리기 버튼을 누르면 블록을 회전시킬 수 있어요!",
            "가로 또는 세로 한 줄을 가득 채우면 블록이 팡! 터져요!",
            "여러 줄을 한 번에 터뜨리면 짜릿한 콤보 보너스 획득!",
            "시간이 다 되기 전에 서둘러 블록을 놓으세요! (점수가 높을수록 시간이 줄어들어요)",
            "스킵 버튼으로 마음에 들지 않는 블록을 바꿀 수 있어요!",
            "보드에 블록을 놓을 자리가 없으면 게임이 종료되니 주의하세요!",
            "콤보를 길게 유지할수록 어마어마한 최고 점수를 달성할 수 있어요!"
        };

        private int _score = 0;
        private int _bestScore = 0;
        private float _currentTurnMaxTime = 180f;
        private float _turnRemainingTime = 180f;
        private bool _isTimerActive = false;

        private bool _isSkipReady = true;
        private int _skipRequiredLines = 10;
        private int _skipCurrentLines = 0;
        private Coroutine _tipCoroutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _bestScore = PlayerPrefs.GetInt("BlockBlast_Best", 0);
        }

        private void Start()
        {
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
                btnSkip.onClick.AddListener(OnSkipClicked);
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
        }

        private void Update()
        {
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
            // Starts at 180s. Decreases by 5s for every 250 points, down to minimum 5s!
            int steps = _score / 250;
            _currentTurnMaxTime = Mathf.Max(5f, 180f - steps * 5f);
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

        private void OnSkipClicked()
        {
            if (!_isSkipReady) return;

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlaySkip();

            _isSkipReady = false;
            _skipCurrentLines = 0;
            UpdateSkipUI();

            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.SkipHandBlocks();
            }
        }

        private void UpdateSkipUI()
        {
            if (btnSkip != null)
            {
                btnSkip.interactable = _isSkipReady;
                var cg = btnSkip.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = _isSkipReady ? 1.0f : 0.45f;
                }
            }

            if (skipBadgeText != null)
            {
                if (_isSkipReady)
                {
                    skipBadgeText.text = "READY";
                    skipBadgeText.color = new Color(0.1f, 0.4f, 0.1f);
                }
                else
                {
                    skipBadgeText.text = $"{_skipCurrentLines}/{_skipRequiredLines}";
                    skipBadgeText.color = new Color(0.4f, 0.1f, 0.1f);
                }
            }
        }

        private void HandleLinesCleared(int combo, int totalLines)
        {
            TriggerShake();

            // Progressive Skip Cooldown
            if (!_isSkipReady)
            {
                _skipCurrentLines += totalLines;
                if (_skipCurrentLines >= _skipRequiredLines)
                {
                    _isSkipReady = true;
                    _skipCurrentLines = 0;
                    _skipRequiredLines += 10; // 10 -> 20 -> 30...
                }
                UpdateSkipUI();
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

            if (BlockAudioManager.Instance != null) BlockAudioManager.Instance.PlayGameOver();

            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
                if (modalFinalScoreText != null) modalFinalScoreText.text = _score.ToString();
                if (modalBestScoreText != null) modalBestScoreText.text = _bestScore.ToString();
            }
        }

        public void RestartGame()
        {
            _score = 0;
            _isSkipReady = true;
            _skipRequiredLines = 10;
            _skipCurrentLines = 0;

            UpdateScoreUI();
            UpdateSkipUI();

            if (gameOverModal != null) gameOverModal.SetActive(false);

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ResetBoard();
            }

            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.ClearHand();
                BlockSpawner.Instance.SpawnNewHand();
            }

            _isTimerActive = true;
            ResetTurnTimer();
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
            RestartGame();
        }

        public void SetupMainMenu(GameObject menuModal, Button startBtn, TMP_Text menuBest)
        {
            mainMenuModal = menuModal;
            btnStartGame = startBtn;
            menuBestScoreText = menuBest;
        }

        public void SetupReferences(TMP_Text score, TMP_Text best, Image tFill, TMP_Text tText, Image vignette, Button skip, TMP_Text sBadge, Button rotate, TMP_Text combo, RectTransform bContainer, GameObject modal, TMP_Text finalS, TMP_Text mBestS, Button restart, GameObject inGameR = null)
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
            guideTipText.text = GuideTips[0];

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
                guideTipText.text = GuideTips[index];

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
