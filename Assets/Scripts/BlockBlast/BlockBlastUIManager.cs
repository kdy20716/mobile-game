using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class BlockBlastUIManager : MonoBehaviour
    {
        public static BlockBlastUIManager Instance { get; private set; }

        [Header("Score HUD")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text bestScoreText;

        [Header("Fever & Skills")]
        [SerializeField] private Image feverBarFill;
        [SerializeField] private Button btnSkip;
        [SerializeField] private Text skipBadgeText;
        [SerializeField] private Button btnRotate;

        [Header("Combo & Shake")]
        [SerializeField] private Text comboPopupText;
        [SerializeField] private RectTransform boardContainer;

        [Header("Game Over Modal")]
        [SerializeField] private GameObject gameOverModal;
        [SerializeField] private Text modalFinalScoreText;
        [SerializeField] private Text modalBestScoreText;
        [SerializeField] private Button btnRestart;

        private int _score = 0;
        private int _bestScore = 0;
        private float _fever = 0f;
        private int _skipCount = 1;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _bestScore = PlayerPrefs.GetInt("BlockBlast_Best", 0);
        }

        private void Start()
        {
            UpdateScoreUI();
            UpdateFeverUI();
            UpdateSkipUI();

            if (gameOverModal != null) gameOverModal.SetActive(false);
            if (comboPopupText != null) comboPopupText.gameObject.SetActive(false);

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.OnScoreAdded += AddScore;
                BlockGridManager.Instance.OnLinesCleared += HandleLinesCleared;
                BlockGridManager.Instance.OnFeverAdded += AddFever;
                BlockGridManager.Instance.OnBombExploded += TriggerShake;
            }

            if (btnSkip != null)
            {
                btnSkip.onClick.AddListener(OnSkipClicked);
            }

            if (btnRotate != null)
            {
                btnRotate.onClick.AddListener(() =>
                {
                    if (BlockSpawner.Instance != null)
                    {
                        BlockSpawner.Instance.RotateHandBlocks();
                    }
                });
            }

            if (btnRestart != null)
            {
                btnRestart.onClick.AddListener(RestartGame);
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
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null) scoreText.text = _score.ToString();
            if (bestScoreText != null) bestScoreText.text = _bestScore.ToString();
        }

        private void AddFever(float amount)
        {
            _fever += amount;
            if (_fever >= 100f)
            {
                _fever = 0f;
                _skipCount++;
                UpdateSkipUI();
            }
            UpdateFeverUI();
        }

        private void UpdateFeverUI()
        {
            if (feverBarFill != null)
            {
                feverBarFill.fillAmount = Mathf.Clamp01(_fever / 100f);
            }
        }

        private void OnSkipClicked()
        {
            if (_skipCount <= 0) return;

            _skipCount--;
            UpdateSkipUI();

            if (BlockSpawner.Instance != null)
            {
                BlockSpawner.Instance.SkipHandBlocks();
            }
        }

        private void UpdateSkipUI()
        {
            if (skipBadgeText != null) skipBadgeText.text = _skipCount.ToString();
            if (btnSkip != null)
            {
                btnSkip.interactable = _skipCount > 0;
            }
        }

        private void HandleLinesCleared(int combo, int totalLines)
        {
            TriggerShake();
            if (comboPopupText != null)
            {
                StopCoroutine("ShowComboAnim");
                StartCoroutine(ShowComboAnim(combo, totalLines));
            }
        }

        private IEnumerator ShowComboAnim(int combo, int totalLines)
        {
            comboPopupText.gameObject.SetActive(true);

            string title = $"💖 COMBO x{combo}!";
            Color col = new Color(1f, 0.55f, 0.7f); // Sweet Pink

            if (combo == 2) { title = $"✨ SWEET! x2"; col = new Color(1f, 0.8f, 0.25f); } // Mango Yellow
            else if (combo == 3) { title = $"🍓 DELICIOUS! x3"; col = new Color(1f, 0.45f, 0.6f); } // Strawberry
            else if (combo == 4) { title = $"🌟 MARVELOUS! x4"; col = new Color(0.4f, 0.88f, 0.75f); } // Mint
            else if (combo >= 5) { title = $"🎉 UNBELIEVABLE! x{combo}"; col = new Color(0.7f, 0.55f, 1f); } // Lavender

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
            _fever = 0f;
            _skipCount = 1;

            UpdateScoreUI();
            UpdateFeverUI();
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
        }

        public void SetupReferences(Text score, Text best, Image fever, Button skip, Text sBadge, Button rotate, Text combo, RectTransform bContainer, GameObject modal, Text finalS, Text mBestS, Button restart)
        {
            scoreText = score;
            bestScoreText = best;
            feverBarFill = fever;
            btnSkip = skip;
            skipBadgeText = sBadge;
            btnRotate = rotate;
            comboPopupText = combo;
            boardContainer = bContainer;
            gameOverModal = modal;
            modalFinalScoreText = finalS;
            modalBestScoreText = mBestS;
            btnRestart = restart;
        }
    }
}
