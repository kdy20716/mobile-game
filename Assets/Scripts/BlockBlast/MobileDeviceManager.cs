using System;
using UnityEngine;

namespace BlockBlast
{
    /// <summary>
    /// Central manager for mobile platform features:
    /// - 60 FPS Target Frame Rate & VSync handling
    /// - Screen.sleepTimeout = SleepTimeout.NeverSleep (prevents screen dimming during gameplay)
    /// - Application Pause / Resume lifecycle & auto-saving (PlayerPrefs.Save)
    /// - In-Game auto-pause on phone calls / app backgrounding
    /// - Haptic feedback / vibration support with settings persistence
    /// </summary>
    [DisallowMultipleComponent]
    public class MobileDeviceManager : MonoBehaviour
    {
        public static MobileDeviceManager Instance { get; private set; }

        private const string KEY_HAPTIC_ENABLED = "Setting_Haptic_Enabled";

        public static bool IsHapticEnabled
        {
            get => PlayerPrefs.GetInt(KEY_HAPTIC_ENABLED, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(KEY_HAPTIC_ENABLED, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance == null)
            {
                GameObject go = new GameObject("[MobileDeviceManager]");
                DontDestroyOnLoad(go);
                Instance = go.AddComponent<MobileDeviceManager>();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            ApplyMobilePerformanceSettings();
        }

        public static void ApplyMobilePerformanceSettings()
        {
            // Set smooth 60 FPS for fluid mobile touch interactions
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            // Keep screen awake while playing
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Debug.Log("<color=cyan>[MobileDeviceManager] Initialized: 60 FPS target, NeverSleep enabled.</color>");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // App moved to background (e.g. phone call, home button, switching apps)
                PlayerPrefs.Save();

                // If in-game, automatically pause to prevent game-over due to timer expiration
                if (BlockBlastUIManager.Instance != null)
                {
                    try
                    {
                        if (!BlockBlastUIManager.Instance.IsPaused)
                        {
                            BlockBlastUIManager.Instance.OpenPauseModal();
                            Debug.Log("[MobileDeviceManager] In-game paused automatically on backgrounding.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[MobileDeviceManager] Error auto-pausing game: {ex.Message}");
                    }
                }
            }
            else
            {
                // Resumed to foreground: Re-assert performance settings
                ApplyMobilePerformanceSettings();
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        // ==========================================
        // HAPTIC FEEDBACK (VIBRATION)
        // ==========================================

        /// <summary>
        /// Light tactile feedback when placing a single block on the board.
        /// </summary>
        public static void TriggerHapticLight()
        {
            if (!IsHapticEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Handheld.Vibrate is supported natively on Android for standard tactile click
                Handheld.Vibrate();
            }
            catch (Exception) { }
#endif
        }

        /// <summary>
        /// Impact vibration when a line or multiple lines explode.
        /// </summary>
        public static void TriggerHapticLineClear()
        {
            if (!IsHapticEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch (Exception) { }
#endif
        }
    }
}
