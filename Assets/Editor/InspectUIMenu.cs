#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class InspectUIMenu
    {
        [MenuItem("Block Blast/Debug/Capture All UI Previews")]
        public static void CaptureAllUIPreviews()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject inGameRoot = null;
            GameObject mainMenu = null;
            GameObject gameOverModal = null;
            GameObject pauseModal = null;
            Canvas mainCanvas = null;
            Camera mainCam = Camera.main;

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "InGameRoot") inGameRoot = g;
                if (g.name == "MainMenuScreen") mainMenu = g;
                if (g.name == "GameOverModal") gameOverModal = g;
                if (g.name == "PauseModal") pauseModal = g;
                if (g.name == "Canvas" && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
            }

            if (inGameRoot == null || mainCanvas == null)
            {
                Debug.LogError("[InspectUIMenu] InGameRoot or Canvas not found!");
                return;
            }

            // Save original states
            bool origInGame = inGameRoot.activeSelf;
            bool origMainMenu = mainMenu != null && mainMenu.activeSelf;
            bool origGameOver = gameOverModal != null && gameOverModal.activeSelf;
            bool origPause = pauseModal != null && pauseModal.activeSelf;
            RenderMode origRenderMode = mainCanvas.renderMode;
            Camera origWorldCam = mainCanvas.worldCamera;

            try
            {
                mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                mainCanvas.worldCamera = mainCam;
                mainCanvas.planeDistance = 5f;

                if (mainMenu != null) mainMenu.SetActive(false);
                inGameRoot.SetActive(true);
                if (gameOverModal != null) gameOverModal.SetActive(false);
                if (pauseModal != null) pauseModal.SetActive(false);

                // 1. Capture InGame UI
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_ui_ingame.png"));

                // 2. Capture GameOver Modal
                if (gameOverModal != null)
                {
                    gameOverModal.SetActive(true);
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_ui_gameover.png"));
                    gameOverModal.SetActive(false);
                }

                // 3. Capture Pause Modal
                if (pauseModal != null)
                {
                    pauseModal.SetActive(true);
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_ui_pause.png"));
                    pauseModal.SetActive(false);
                }

                Debug.Log("[InspectUIMenu] Successfully captured InGame, GameOver, and Pause UI previews!");
            }
            finally
            {
                // Restore original states
                mainCanvas.renderMode = origRenderMode;
                mainCanvas.worldCamera = origWorldCam;
                if (mainMenu != null) mainMenu.SetActive(origMainMenu);
                inGameRoot.SetActive(origInGame);
                if (gameOverModal != null) gameOverModal.SetActive(origGameOver);
                if (pauseModal != null) pauseModal.SetActive(origPause);
            }
        }

        private static void CaptureCameraView(Camera cam, string outputPath)
        {
            if (cam == null) return;
            int width = 1080;
            int height = 1920;
            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            RenderTexture origRT = cam.targetTexture;
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            cam.targetTexture = origRT;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            File.WriteAllBytes(outputPath, bytes);
        }
    }
}
#endif
