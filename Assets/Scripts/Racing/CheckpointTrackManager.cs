using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobileRacing
{
    public class CheckpointTrackManager : MonoBehaviour
    {
        public static CheckpointTrackManager Instance { get; private set; }

        [Header("Track Settings")]
        [SerializeField] private List<Transform> checkpoints = new List<Transform>();
        [SerializeField] private int totalLaps = 3;

        public int CurrentLap { get; private set; } = 1;
        public float CurrentLapTime { get; private set; } = 0f;
        public float BestLapTime { get; private set; } = float.MaxValue;
        public bool IsRaceFinished { get; private set; } = false;

        public event Action<int, int> OnLapChanged; // (currentLap, totalLaps)
        public event Action<float> OnLapCompleted;  // (lapTime)
        public event Action OnRaceCompleted;

        private int _nextCheckpointIndex = 0;
        private bool _raceStarted = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _raceStarted = true;
            OnLapChanged?.Invoke(CurrentLap, totalLaps);
        }

        private void Update()
        {
            if (!_raceStarted || IsRaceFinished) return;
            CurrentLapTime += Time.deltaTime;
        }

        public void RegisterCheckpoint(Transform cp)
        {
            if (!checkpoints.Contains(cp))
            {
                checkpoints.Add(cp);
            }
        }

        public void CheckpointTriggered(int checkpointIndex)
        {
            if (IsRaceFinished) return;

            if (checkpointIndex == _nextCheckpointIndex)
            {
                _nextCheckpointIndex = (_nextCheckpointIndex + 1) % checkpoints.Count;

                // Completed a full lap
                if (_nextCheckpointIndex == 0)
                {
                    OnLapCompleted?.Invoke(CurrentLapTime);
                    if (CurrentLapTime < BestLapTime)
                    {
                        BestLapTime = CurrentLapTime;
                    }

                    if (CurrentLap < totalLaps)
                    {
                        CurrentLap++;
                        CurrentLapTime = 0f;
                        OnLapChanged?.Invoke(CurrentLap, totalLaps);
                    }
                    else
                    {
                        IsRaceFinished = true;
                        OnRaceCompleted?.Invoke();
                    }
                }
            }
        }

        public void SetCheckpoints(List<Transform> list, int laps)
        {
            checkpoints = list;
            totalLaps = laps;
        }
    }
}
