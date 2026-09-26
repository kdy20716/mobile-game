#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BlockBlast;

namespace BlockBlast.Editor
{
    public static class ReleaseVerificationUtility
    {
        [MenuItem("Block Blast/Debug/Skip Intro To Lobby")]
        public static void SkipIntroToLobby()
        {
            var intro = Object.FindObjectOfType<MainMenuCinematicController>();
            if (intro != null)
            {
                intro.TriggerStartGame(Vector2.zero);
                Debug.Log("<color=cyan>[ReleaseVerification] Triggered StartGame into Lobby!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Open Settings Modal")]
        public static void OpenSettingsModal()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OpenSettingsModal();
                Debug.Log("<color=cyan>[ReleaseVerification] Opened Settings Modal!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Open Quit Modal")]
        public static void OpenQuitModal()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OpenQuitModal();
                Debug.Log("<color=cyan>[ReleaseVerification] Opened Quit Confirm Modal!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Click Quit Confirm Cancel (No)")]
        public static void ClickQuitConfirmNo()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.CloseQuitModal();
                Debug.Log("<color=cyan>[ReleaseVerification] Clicked Quit Modal Cancel Button -> Modal Closed!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Set Aspect 16:9")]
        public static void SetAspect16x9()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SetAspectRatio(0);
                Debug.Log("<color=cyan>[ReleaseVerification] Set Aspect 16:9!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Set Aspect 9:16")]
        public static void SetAspect9x16()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SetAspectRatio(3);
                Debug.Log("<color=cyan>[ReleaseVerification] Set Aspect 9:16!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Set Windowed Mode")]
        public static void SetWindowedMode()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SetWindowMode(0);
                Debug.Log("<color=cyan>[ReleaseVerification] Set Windowed Mode!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Set Fullscreen Mode")]
        public static void SetFullscreenMode()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SetWindowMode(2);
                Debug.Log("<color=cyan>[ReleaseVerification] Set Fullscreen Mode!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Start Game")]
        public static void StartGame()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.StartGameFromLobby();
                Debug.Log("<color=cyan>[ReleaseVerification] Started In-Game!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Open InGame Pause Modal")]
        public static void OpenInGamePauseModal()
        {
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.OpenPauseModal();
                Debug.Log("<color=cyan>[ReleaseVerification] In-Game Pause Modal Opened via ESC!</color>");
            }
        }
    }
}
#endif
