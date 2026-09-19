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
        [SerializeField] private Button btnHammer;
        [SerializeField] private Text hammerBadgeText;
        [SerializeField] private Button btnRotate;

        [Header("Combo & Shake")]
        [SerializeField] private Text comboPopupText;
        [SerializeField] private RectTransform boardContainer;

        [Header("Game Over Modal")]
        [SerializeField] private GameObject gameOverModal;
        [SerializeField] private Text modalFinalScoreText;
        [SerializeField] private Text modalBestScoreText;
        [SerializeField] private Button btnRestart;

        public bool IsHammerActive { get; private set; } = false;
        private int _score = 0;
        private int _bestScore = 0;
        private float _fever = 0f;
        private int _hammerCount = 1;

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
            UpdateHammerUI();

            if (gameOverModal != null) gameOverModal.SetActive(false);
            if (comboPopupText != null) comboPopupText.gameObject.SetActive(false);

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.OnScoreAdded += AddScore;
                BlockGridManager.Instance.OnLinesCleared += HandleLinesCleared;
                BlockGridManager.Instance.OnFeverAdded += AddFever;
                BlockGridManager.Instance.OnBombExploded += TriggerShake;
            }

            if (btnHammer != null)
            {
                btnHammer.onClick.AddListener(ToggleHammerMode);
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
                _hammerCount++;
                UpdateHammerUI();
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

        private void ToggleHammerMode()
        {
            if (_hammerCount <= 0) return;
            IsHammerActive = !IsHammerActive;
            UpdateHammerUI();
        }

        public void ConsumeHammer()
        {
            if (_hammerCount > 0)
            {
                _hammerCount--;
                IsHammerActive = false;
                UpdateHammerUI();
                TriggerShake();
            }
        }

        private void UpdateHammerUI()
        {
            if (hammerBadgeText != null) hammerBadgeText.text = _hammerCount.ToString();
            if (btnHammer != null)
            {
                var colors = btnHammer.colors;
                colors.normalColor = IsHammerActive ? new Color(0.9f, 0.4f, 0.1f) : Color.white;
                btnHammer.colors = colors;
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

            string title = $"COMBO x{combo}!";
            Color col = new Color(1f, 0.8f, 0.2f);

            if (combo >= 3) { title = $"AMAZING! x{combo}"; col = new Color(1f, 0.4f, 0.5f); }
            if (combo >= 5) { title = $"UNBELIEVABLE! x{combo}"; col = new Color(0.2f, 0.9f, 1f); }

            comboPopupText.text = $"{title}\n<size=28>+{totalLines} LINES</size>";
            comboPopupText.color = col;

            float elapsed = 0f;
            float dur = 0.8f;
            Vector3 startScale = Vector3.one * 0.5f;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                float s = Mathf.Sin(t * Mathf.PI) * 0.6f + 0.9f;
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
            _hammerCount = 1;
            IsHammerActive = false;

            UpdateScoreUI();
            UpdateFeverUI();
            UpdateHammerUI();

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

        public void SetupReferences(Text score, Text best, Image fever, Button hammer, Text hBadge, Button rotate, Text combo, RectTransform bContainer, GameObject modal, Text finalS, Text mBestS, Button restart)
        {
            scoreText = score;
            bestScoreText = best;
            feverBarFill = fever;
            btnHammer = hammer;
            hammerBadgeText = hBadge;
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
