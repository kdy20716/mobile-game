using UnityEngine;
using UnityEngine.UI;

namespace Survivor2D
{
    public class SurvivorUIManager : MonoBehaviour
    {
        [Header("Top Bars")]
        [SerializeField] private Slider expSlider;
        [SerializeField] private Text levelText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text killCountText;

        [Header("Player HP Bar")]
        [SerializeField] private Slider hpSlider;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text resultTimeText;
        [SerializeField] private Text resultKillsText;
        [SerializeField] private Button restartButton;

        private float _survivedTime = 0f;
        private int _killCount = 0;
        private bool _isGameOver = false;

        private void Start()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                SurvivorPlayer2D.Instance.OnHpChanged += UpdateHp;
                SurvivorPlayer2D.Instance.OnExpChanged += UpdateExp;
                SurvivorPlayer2D.Instance.OnPlayerDied += ShowGameOver;
            }

            Enemy2D.OnEnemyKilled += HandleEnemyKilled;

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (restartButton != null) restartButton.onClick.AddListener(RestartGame);

            UpdateKillDisplay();
        }

        private void OnDestroy()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                SurvivorPlayer2D.Instance.OnHpChanged -= UpdateHp;
                SurvivorPlayer2D.Instance.OnExpChanged -= UpdateExp;
                SurvivorPlayer2D.Instance.OnPlayerDied -= ShowGameOver;
            }

            Enemy2D.OnEnemyKilled -= HandleEnemyKilled;
        }

        private void Update()
        {
            if (!_isGameOver)
            {
                _survivedTime += Time.deltaTime;
                if (timerText != null)
                {
                    int min = Mathf.FloorToInt(_survivedTime / 60f);
                    int sec = Mathf.FloorToInt(_survivedTime % 60f);
                    timerText.text = $"{min:00}:{sec:00}";
                }
            }
        }

        private void UpdateHp(float current, float max)
        {
            if (hpSlider != null)
            {
                hpSlider.value = Mathf.Clamp01(current / max);
            }
        }

        private void UpdateExp(int current, int max, int level)
        {
            if (expSlider != null)
            {
                expSlider.value = (float)current / Mathf.Max(1, max);
            }
            if (levelText != null)
            {
                levelText.text = $"LV. {level}";
            }
        }

        private void HandleEnemyKilled()
        {
            _killCount++;
            UpdateKillDisplay();
        }

        private void UpdateKillDisplay()
        {
            if (killCountText != null)
            {
                killCountText.text = $"💀 {_killCount}";
            }
        }

        private void ShowGameOver()
        {
            _isGameOver = true;
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (resultTimeText != null)
                {
                    int min = Mathf.FloorToInt(_survivedTime / 60f);
                    int sec = Mathf.FloorToInt(_survivedTime % 60f);
                    resultTimeText.text = $"SURVIVED: {min:00}:{sec:00}";
                }
                if (resultKillsText != null)
                {
                    resultKillsText.text = $"ENEMIES SLAIN: {_killCount}";
                }
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void SetupReferences(Slider exp, Text lv, Text timer, Text kills, Slider hp, GameObject overPanel, Text resTime, Text resKills, Button restart)
        {
            expSlider = exp;
            levelText = lv;
            timerText = timer;
            killCountText = kills;
            hpSlider = hp;
            gameOverPanel = overPanel;
            resultTimeText = resTime;
            resultKillsText = resKills;
            restartButton = restart;
        }
    }
}
