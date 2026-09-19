using UnityEngine;
using UnityEngine.UI;

namespace MobileRacing
{
    public class RacingUIManager : MonoBehaviour
    {
        [Header("Car Reference")]
        [SerializeField] private ArcadeCarController car;

        [Header("UI Text Displays")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text lapText;
        [SerializeField] private Text lapTimeText;
        [SerializeField] private Text bestLapText;
        [SerializeField] private GameObject raceFinishedPanel;

        private void Start()
        {
            if (CheckpointTrackManager.Instance != null)
            {
                CheckpointTrackManager.Instance.OnLapChanged += UpdateLapDisplay;
                CheckpointTrackManager.Instance.OnLapCompleted += UpdateBestLapDisplay;
                CheckpointTrackManager.Instance.OnRaceCompleted += ShowRaceFinished;
            }

            if (raceFinishedPanel != null)
                raceFinishedPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (CheckpointTrackManager.Instance != null)
            {
                CheckpointTrackManager.Instance.OnLapChanged -= UpdateLapDisplay;
                CheckpointTrackManager.Instance.OnLapCompleted -= UpdateBestLapDisplay;
                CheckpointTrackManager.Instance.OnRaceCompleted -= ShowRaceFinished;
            }
        }

        private void Update()
        {
            if (car != null && speedText != null)
            {
                int speed = Mathf.RoundToInt(car.CurrentSpeedKmh);
                speedText.text = $"{speed} <size=18>KM/H</size>";
            }

            if (CheckpointTrackManager.Instance != null && lapTimeText != null)
            {
                lapTimeText.text = FormatTime(CheckpointTrackManager.Instance.CurrentLapTime);
            }
        }

        private void UpdateLapDisplay(int currentLap, int totalLaps)
        {
            if (lapText != null)
            {
                lapText.text = $"LAP {currentLap}/{totalLaps}";
            }
        }

        private void UpdateBestLapDisplay(float lapTime)
        {
            if (bestLapText != null && CheckpointTrackManager.Instance != null)
            {
                bestLapText.text = $"BEST: {FormatTime(CheckpointTrackManager.Instance.BestLapTime)}";
            }
        }

        private void ShowRaceFinished()
        {
            if (raceFinishedPanel != null)
            {
                raceFinishedPanel.SetActive(true);
            }
        }

        private string FormatTime(float timeInSeconds)
        {
            if (timeInSeconds >= 9999f) return "--:--.--";
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            int fraction = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{fraction:00}";
        }

        public void SetCar(ArcadeCarController targetCar)
        {
            car = targetCar;
        }

        public void RestartRace()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
