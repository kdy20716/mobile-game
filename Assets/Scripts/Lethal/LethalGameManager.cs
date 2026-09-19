using System;
using System.Collections.Generic;
using UnityEngine;

namespace LethalCompany
{
    public class LethalGameManager : MonoBehaviour
    {
        public static LethalGameManager Instance { get; private set; }

        [Header("Quota Settings")]
        [SerializeField] private int targetQuota = 300;
        [SerializeField] private int quotaIncreasePerCycle = 250;
        [SerializeField] private int totalDaysPerQuota = 3;

        public int TargetQuota => targetQuota;
        public int FulfilledQuota { get; private set; } = 0;
        public int CurrentDay { get; private set; } = 1;
        public int DaysLeft { get; private set; } = 3;

        [Header("Day Time Settings")]
        [Tooltip("Day starts at 8:00 AM (8f) and ends at 12:00 AM Midnight (24f)")]
        [SerializeField] private float dayDurationSeconds = 240f; // 4 minutes per day
        private float currentHour = 8f; // 8.0 = 8:00 AM
        public float CurrentHour => currentHour;
        public bool IsDayActive { get; private set; } = true;

        [Header("Spawning")]
        [SerializeField] private Transform shipSpawnPoint;
        [SerializeField] private GameObject playerObj;

        public event Action<float> OnTimeChanged; // Passes hour (e.g. 8.5 = 8:30 AM)
        public event Action<int, int, int> OnQuotaUpdated; // fulfilled, target, daysLeft

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DaysLeft = totalDaysPerQuota;
        }

        private void Start()
        {
            OnQuotaUpdated?.Invoke(FulfilledQuota, targetQuota, DaysLeft);
        }

        private void Update()
        {
            if (!IsDayActive) return;

            // Advance time from 8.0f (8 AM) to 24.0f (12 AM Midnight)
            float hoursPerSecond = (24f - 8f) / dayDurationSeconds;
            currentHour += Time.deltaTime * hoursPerSecond;
            OnTimeChanged?.Invoke(currentHour);

            if (currentHour >= 24f)
            {
                Debug.Log("[LethalGame] Midnight reached! Ship auto-departing...");
                EndDay();
            }
        }

        public string GetFormattedTime()
        {
            int hourInt = Mathf.FloorToInt(currentHour);
            int minuteInt = Mathf.FloorToInt((currentHour - hourInt) * 60f);

            string period = hourInt >= 12 && hourInt < 24 ? "PM" : "AM";
            int displayHour = hourInt % 12;
            if (displayHour == 0) displayHour = 12;

            return $"{displayHour:00}:{minuteInt:00} {period}";
        }

        public void EndDay()
        {
            if (!IsDayActive) return;
            IsDayActive = false;

            // Calculate cargo scrap
            List<ScrapItem> cargo = LethalShipManager.Instance != null 
                ? LethalShipManager.Instance.GetCargoScraps() 
                : new List<ScrapItem>();

            int dayEarnings = 0;
            foreach (var scrap in cargo)
            {
                if (scrap != null) dayEarnings += scrap.scrapValue;
            }

            FulfilledQuota += dayEarnings;
            DaysLeft--;

            bool isQuotaMet = FulfilledQuota >= targetQuota;
            bool isGameOver = DaysLeft <= 0 && !isQuotaMet;

            // Notify UI
            if (LethalUIManager.Instance != null)
            {
                LethalUIManager.Instance.ShowDaySummary(cargo, dayEarnings, FulfilledQuota, targetQuota, DaysLeft, isQuotaMet, isGameOver);
            }

            // Clear scrap from ship cargo
            if (LethalShipManager.Instance != null)
            {
                LethalShipManager.Instance.ClearCargoScraps();
            }

            OnQuotaUpdated?.Invoke(FulfilledQuota, targetQuota, DaysLeft);
        }

        public void StartNextDay()
        {
            if (DaysLeft <= 0)
            {
                if (FulfilledQuota >= targetQuota)
                {
                    // Met Quota! Advance to next quota cycle
                    targetQuota += quotaIncreasePerCycle;
                    FulfilledQuota = 0;
                    DaysLeft = totalDaysPerQuota;
                    Debug.Log($"[LethalGame] Quota Met! New Quota: ${targetQuota}");
                }
                else
                {
                    // Game Over Restart
                    targetQuota = 300;
                    FulfilledQuota = 0;
                    DaysLeft = totalDaysPerQuota;
                    CurrentDay = 0;
                }
            }

            CurrentDay++;
            currentHour = 8f;
            IsDayActive = true;

            // Reset ship lever
            if (LethalShipManager.Instance != null)
            {
                LethalShipManager.Instance.ResetLever();
            }

            // Move player to ship spawn point
            if (playerObj != null && shipSpawnPoint != null)
            {
                var cc = playerObj.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                playerObj.transform.position = shipSpawnPoint.position;
                playerObj.transform.rotation = shipSpawnPoint.rotation;
                if (cc != null) cc.enabled = true;
            }

            OnQuotaUpdated?.Invoke(FulfilledQuota, targetQuota, DaysLeft);

            if (LethalUIManager.Instance != null)
            {
                LethalUIManager.Instance.HideDaySummary();
            }
        }
    }
}
