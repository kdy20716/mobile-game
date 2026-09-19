using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileRacing
{
    public class RaceGameManager : MonoBehaviour
    {
        public static RaceGameManager Instance { get; private set; }

        public enum RaceState { Countdown, Racing, Finished }
        public RaceState CurrentState { get; private set; } = RaceState.Countdown;

        [Header("Participants")]
        [SerializeField] private ArcadeCarController playerCar;
        [SerializeField] private List<AICarController> aiCars = new List<AICarController>();

        [Header("Race Info")]
        public int PlayerRank { get; private set; } = 1;
        public float CountdownTimer { get; private set; } = 3f;
        public float RaceTime { get; private set; } = 0f;

        public event Action<int> OnCountdownTick; // 3, 2, 1, 0 (GO)
        public event Action OnRaceStarted;
        public event Action<int> OnPlayerRankChanged; // rank 1..4
        public event Action<int, float> OnRaceFinishedWithResults; // rank, finishTime

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            // Lock controls during countdown
            if (playerCar != null) playerCar.enabled = false;
            foreach (var ai in aiCars) if (ai != null) ai.SetCanRace(false);

            // 3-2-1 Countdown
            CountdownTimer = 3f;
            while (CountdownTimer > 0f)
            {
                int sec = Mathf.CeilToInt(CountdownTimer);
                OnCountdownTick?.Invoke(sec);
                yield return new WaitForSeconds(1f);
                CountdownTimer -= 1f;
            }

            // GO!
            OnCountdownTick?.Invoke(0);
            CurrentState = RaceState.Racing;
            if (playerCar != null) playerCar.enabled = true;
            foreach (var ai in aiCars) if (ai != null) ai.SetCanRace(true);
            OnRaceStarted?.Invoke();

            if (CheckpointTrackManager.Instance != null)
            {
                CheckpointTrackManager.Instance.OnRaceCompleted += HandlePlayerRaceFinish;
            }
        }

        private void Update()
        {
            if (CurrentState == RaceState.Racing)
            {
                RaceTime += Time.deltaTime;
                CalculateRanks();
            }
        }

        private void CalculateRanks()
        {
            if (playerCar == null || CheckpointTrackManager.Instance == null) return;

            int rank = 1;
            float playerProgress = GetPlayerProgress();

            foreach (var ai in aiCars)
            {
                if (ai == null) continue;
                float aiProgress = ai.LapsCompleted * 1000f + ai.CurrentWaypointIndex * 50f;
                if (aiProgress > playerProgress)
                {
                    rank++;
                }
            }

            if (rank != PlayerRank)
            {
                PlayerRank = rank;
                OnPlayerRankChanged?.Invoke(PlayerRank);
            }
        }

        private float GetPlayerProgress()
        {
            if (CheckpointTrackManager.Instance == null) return 0f;
            return (CheckpointTrackManager.Instance.CurrentLap - 1) * 1000f;
        }

        private void HandlePlayerRaceFinish()
        {
            CurrentState = RaceState.Finished;
            OnRaceFinishedWithResults?.Invoke(PlayerRank, RaceTime);
        }

        public void SetupParticipants(ArcadeCarController player, List<AICarController> rivals)
        {
            playerCar = player;
            aiCars = rivals;
        }
    }
}
