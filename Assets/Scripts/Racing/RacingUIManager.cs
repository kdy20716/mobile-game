using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MobileRacing
{
    public class RacingUIManager : MonoBehaviour
    {
        [Header("Car Reference")]
        [SerializeField] private ArcadeCarController car;

        [Header("HUD Displays")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text distanceText;
        [SerializeField] private Text raceTimeText;
        [SerializeField] private Text rankText;

        [Header("Countdown UI")]
        [SerializeField] private Text countdownText;

        [Header("Race Finished UI")]
        [SerializeField] private GameObject raceFinishedPanel;
        [SerializeField] private Text resultRankText;
        [SerializeField] private Text resultTimeText;

        private void Start()
        {
            if (RaceGameManager.Instance != null)
            {
                RaceGameManager.Instance.OnCountdownTick += HandleCountdown;
                RaceGameManager.Instance.OnPlayerRankChanged += UpdateRankDisplay;
                RaceGameManager.Instance.OnRaceFinishedWithResults += ShowRaceResults;
            }

            if (raceFinishedPanel != null)
                raceFinishedPanel.SetActive(false);

            UpdateRankDisplay(1);
        }

        private void OnDestroy()
        {
            if (RaceGameManager.Instance != null)
            {
                RaceGameManager.Instance.OnCountdownTick -= HandleCountdown;
                RaceGameManager.Instance.OnPlayerRankChanged -= UpdateRankDisplay;
                RaceGameManager.Instance.OnRaceFinishedWithResults -= ShowRaceResults;
            }
        }

        private void Update()
        {
            if (car != null)
            {
                if (speedText != null)
                {
                    int speed = Mathf.RoundToInt(car.CurrentSpeedKmh);
                    speedText.text = $"{speed} <size=18>KM/H</size>";
                }

                if (distanceText != null && RaceGameManager.Instance != null)
                {
                    float distRemaining = Mathf.Max(0f, RaceGameManager.Instance.FinishLineZ - car.transform.position.z);
                    distanceText.text = $"FINISH: {Mathf.RoundToInt(distRemaining)}m";
                }
            }

            if (RaceGameManager.Instance != null && raceTimeText != null)
            {
                raceTimeText.text = FormatTime(RaceGameManager.Instance.RaceTime);
            }
        }

        private void HandleCountdown(int sec)
        {
            if (countdownText == null) return;

            if (sec > 0)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = sec.ToString();
                countdownText.color = Color.yellow;
            }
            else
            {
                countdownText.text = "GO!";
                countdownText.color = Color.green;
                StartCoroutine(HideCountdownAfterDelay());
            }
        }

        private IEnumerator HideCountdownAfterDelay()
        {
            yield return new WaitForSeconds(1.2f);
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }

        private void UpdateRankDisplay(int rank)
        {
            if (rankText != null)
            {
                string suffix = rank == 1 ? "ST" : (rank == 2 ? "ND" : (rank == 3 ? "RD" : "TH"));
                rankText.text = $"{rank}<size=22>{suffix}</size>";
                rankText.color = rank == 1 ? new Color(1f, 0.85f, 0.1f) : Color.white;
            }
        }

        private void ShowRaceResults(int rank, float totalTime)
        {
            if (raceFinishedPanel != null)
            {
                raceFinishedPanel.SetActive(true);
                if (resultRankText != null)
                {
                    string trophy = rank == 1 ? "🏆 1ST PLACE WINNER!" : (rank == 2 ? "🥈 2ND PLACE" : (rank == 3 ? "🥉 3RD PLACE" : "4TH PLACE"));
                    resultRankText.text = trophy;
                    resultRankText.color = rank == 1 ? Color.yellow : Color.white;
                }
                if (resultTimeText != null)
                {
                    resultTimeText.text = $"TIME: {FormatTime(totalTime)}";
                }
            }
        }

        private string FormatTime(float timeInSeconds)
        {
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
