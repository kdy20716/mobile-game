using UnityEngine;
using UnityEngine.UI;

namespace HorrorEscape
{
    public class HorrorUIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private Text keysText;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Slider batterySlider;

        [Header("Entity Static Screen Filter")]
        [SerializeField] private Image staticGlitchImage;

        [Header("Game Over / Win Modals")]
        [SerializeField] private GameObject jumpscarePanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Button jumpscareRetryBtn;
        [SerializeField] private Button winPlayAgainBtn;

        private HorrorEntityAI _entity;
        private HorrorPlayerController _player;
        private FlashlightController _flashlight;

        private void Start()
        {
            _entity = Object.FindFirstObjectByType<HorrorEntityAI>();
            _player = HorrorPlayerController.Instance;
            _flashlight = Object.FindFirstObjectByType<FlashlightController>();

            EscapeKeyItem.OnKeyCollected += UpdateKeysDisplay;
            EscapeKeyItem.ResetKeys();
            UpdateKeysDisplay(0);

            HorrorEntityAI.OnPlayerCaught += TriggerJumpscare;
            EscapeDoor.OnEscaped += TriggerWin;

            if (jumpscarePanel != null) jumpscarePanel.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);

            if (jumpscareRetryBtn != null) jumpscareRetryBtn.onClick.AddListener(RestartGame);
            if (winPlayAgainBtn != null) winPlayAgainBtn.onClick.AddListener(RestartGame);
        }

        private void Update()
        {
            if (_player != null && staminaSlider != null)
            {
                staminaSlider.value = _player.currentStamina / _player.maxStamina;
            }

            if (_flashlight != null && batterySlider != null)
            {
                batterySlider.value = _flashlight.currentBattery / 100f;
            }

            // Glitch static effect intensity based on entity distance
            if (_entity != null && staticGlitchImage != null)
            {
                float dist = _entity.DistanceToPlayer;
                if (dist < 15f)
                {
                    float glitchAlpha = Mathf.Lerp(0.85f, 0f, dist / 15f);
                    // Add flickering noise
                    glitchAlpha *= Random.Range(0.6f, 1.2f);
                    Color c = staticGlitchImage.color;
                    c.a = Mathf.Clamp01(glitchAlpha);
                    staticGlitchImage.color = c;
                }
                else
                {
                    Color c = staticGlitchImage.color;
                    c.a = 0f;
                    staticGlitchImage.color = c;
                }
            }
        }

        private void UpdateKeysDisplay(int keys)
        {
            if (keysText != null)
            {
                keysText.text = $"🔑 KEYS: {keys} / {EscapeKeyItem.TotalKeysNeeded}";
                keysText.color = keys >= EscapeKeyItem.TotalKeysNeeded ? Color.green : Color.yellow;
            }
        }

        private void TriggerJumpscare()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (jumpscarePanel != null) jumpscarePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private void TriggerWin()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (winPanel != null) winPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            EscapeKeyItem.ResetKeys();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
