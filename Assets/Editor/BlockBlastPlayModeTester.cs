#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BlockBlast;

namespace BlockBlast.Editor
{
    public static class BlockBlastPlayModeTester
    {
        [MenuItem("Block Blast/Debug/Open Settings Modal")]
        public static void OpenSettings()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.OpenSettingsModal();
            }
        }

        [MenuItem("Block Blast/Debug/Start Game")]
        public static void StartGame()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.StartGameFromLobby();
            }
        }

        [MenuItem("Block Blast/Debug/Return To Lobby")]
        public static void ReturnToLobby()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.ReturnToLobby();
            }
        }

        [MenuItem("Block Blast/Debug/Set Res 16x9 (1600x900)")]
        public static void SetRes1600x900()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.SetAspectRatio(0); // 16:9
            }
        }

        [MenuItem("Block Blast/Debug/Set Res 9x16 (720x1280)")]
        public static void SetRes720x1280()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.SetAspectRatio(3); // 9:16
            }
        }

        [MenuItem("Block Blast/Debug/Equip Candy Theme")]
        public static void EquipCandyTheme()
        {
            var lobby = Object.FindFirstObjectByType<LobbyManager>();
            if (lobby != null)
            {
                lobby.ApplyTheme(1);
            }
        }
    }
}
#endif
