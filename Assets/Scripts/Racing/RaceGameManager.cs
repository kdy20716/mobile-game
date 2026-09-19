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

        [Header("Highway Track Specs")]
        public float FinishLineZ = 1200f; // 1.2km Straight Drag Race
        public int PlayerRank { get; private set; } = 1;
        public float CountdownTimer { get; private set; } = 3f;
        public float RaceTime { get; private set; } = 0f;

        public event Action<int> OnCountdownTick; // 3, 2, 1, 0 (GO)
        public event Action OnRaceStarted;
        public event Action<int> OnPlayerRankChanged;
        public event Action<int, float> OnRaceFinishedWithResults; // rank, finishTime

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private IEnumerator Start()
        {
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
        }

        private void Update()
        {
            if (CurrentState == RaceState.Racing)
            {
                RaceTime += Time.deltaTime;
                CalculateRanks();
                CheckFinishLine();
            }
        }

        private void CalculateRanks()
        {
            if (playerCar == null) return;

            int rank = 1;
            float playerZ = playerCar.transform.position.z;

            foreach (var ai in aiCars)
            {
                if (ai == null) continue;
                if (ai.transform.position.z > playerZ)
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

        private void CheckFinishLine()
        {
            if (playerCar != null && playerCar.transform.position.z >= FinishLineZ)
            {
                CurrentState = RaceState.Finished;
                OnRaceFinishedWithResults?.Invoke(PlayerRank, RaceTime);
            }
        }

        public void SetupParticipants(ArcadeCarController player, List<AICarController> rivals, float finishZ = 1200f)
        {
            playerCar = player;
            aiCars = rivals;
            FinishLineZ = finishZ;
        }
    }
}
