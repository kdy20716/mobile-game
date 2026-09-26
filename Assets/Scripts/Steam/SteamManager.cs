using System;
using UnityEngine;
using Steamworks;

namespace BlockBlast
{
    [DisallowMultipleComponent]
    public class SteamManager : MonoBehaviour
    {
        private static SteamManager s_instance;
        public static SteamManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    CreateInstance();
                }
                return s_instance;
            }
        }

        private static bool s_everInitialized = false;
        private bool m_bInitialized = false;

        public static bool Initialized
        {
            get
            {
                if (s_instance == null)
                {
                    CreateInstance();
                }
                return s_instance != null && s_instance.m_bInitialized;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (s_instance == null)
            {
                CreateInstance();
            }
        }

        private static void CreateInstance()
        {
            if (s_instance != null) return;
            GameObject go = new GameObject("[SteamManager]");
            DontDestroyOnLoad(go);
            s_instance = go.AddComponent<SteamManager>();
        }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);

            if (s_everInitialized)
            {
                return;
            }

            try
            {
                if (!SteamAPI.Init())
                {
                    Debug.LogWarning("[SteamManager] SteamAPI.Init() failed. (Make sure Steam client is running and steam_appid.txt is present)");
                    m_bInitialized = false;
                    return;
                }

                m_bInitialized = true;
                s_everInitialized = true;
                Debug.Log("<color=green>[SteamManager] Steamworks API successfully initialized!</color>");

                try
                {
                    LocalizationManager.OnSteamInitialized();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SteamManager] Failed to notify LocalizationManager: {ex.Message}");
                }
            }
            catch (DllNotFoundException e)
            {
                Debug.LogWarning($"[SteamManager] steam_api DLL not found: {e.Message}");
                m_bInitialized = false;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SteamManager] Exception during SteamAPI.Init(): {e.Message}");
                m_bInitialized = false;
            }
        }

        private void Update()
        {
            if (m_bInitialized)
            {
                SteamAPI.RunCallbacks();
            }
        }

        private void OnDestroy()
        {
            if (s_instance != this) return;
            s_instance = null;

            if (m_bInitialized)
            {
                SteamAPI.Shutdown();
                m_bInitialized = false;
            }
        }

        private void OnApplicationQuit()
        {
            if (m_bInitialized)
            {
                SteamAPI.Shutdown();
                m_bInitialized = false;
            }
        }

        /// <summary>
        /// Retrieves the current game language set in Steam.
        /// Returns null if Steam is not initialized.
        /// </summary>
        public static string GetCurrentSteamLanguage()
        {
            try
            {
                if (Initialized)
                {
                    string lang = SteamApps.GetCurrentGameLanguage();
                    Debug.Log($"<color=cyan>[SteamManager] SteamApps.GetCurrentGameLanguage() returned: '{lang}'</color>");
                    return lang;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SteamManager] Error calling GetCurrentGameLanguage: {ex.Message}");
            }
            return null;
        }
    }
}
