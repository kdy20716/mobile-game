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
            Camera mainCam = Camera.main ?? Object.FindObjectOfType<Camera>();

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "InGameRoot") inGameRoot = g;
                if (g.name == "MainMenuScreen") mainMenu = g;
                if (g.name == "GameOverModal") gameOverModal = g;
                if (g.name == "PauseModal") pauseModal = g;
                if ((g.name == "Canvas" || g.name == "BlockBlastCanvas") && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
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
            Canvas.ForceUpdateCanvases();
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

        [MenuItem("Block Blast/Debug/Capture Shop Horizontal Tabs")]
        public static void CaptureShopHorizontalTabs()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var lobbyMgr = Object.FindObjectOfType<LobbyManager>(true);
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject splashScreen = null;
            GameObject mainMenu = null;
            GameObject lobbyScreen = null;
            GameObject shopModal = null;
            GameObject fairyTrans = null;
            GameObject fairyTouch = null;
            Canvas mainCanvas = null;
            Camera mainCam = Camera.main ?? Object.FindObjectOfType<Camera>();

            GameObject setModal = null;
            GameObject profModal = null;
            GameObject quitModal = null;
            GameObject mascModal = null;
            GameObject sumResModal = null;
            GameObject gachaPresModal = null;

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "SplashScreen" || g.name == "SplashScreenOverlay") splashScreen = g;
                if (g.name == "MainMenuScreen") mainMenu = g;
                if (g.name == "LobbyScreen") lobbyScreen = g;
                if (g.name == "ShopModal") shopModal = g;
                if (g.name == "SettingsModal") setModal = g;
                if (g.name == "ProfileModal") profModal = g;
                if (g.name == "QuitModal") quitModal = g;
                if (g.name == "MascotModal") mascModal = g;
                if (g.name == "SummonResultModal") sumResModal = g;
                if (g.name == "GachaPresentationModal") gachaPresModal = g;
                if (g.name == "FairyScreenTransitionOverlay") fairyTrans = g;
                if (g.name == "FairyTouchFXOverlay") fairyTouch = g;
                if ((g.name == "Canvas" || g.name == "BlockBlastCanvas") && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
            }

            if (shopModal == null || mainCanvas == null)
            {
                Debug.LogError("[InspectUIMenu] ShopModal or Canvas not found!");
                return;
            }

            RenderMode origRenderMode = mainCanvas.renderMode;
            Camera origWorldCam = mainCanvas.worldCamera;
            bool origSplash = splashScreen != null && splashScreen.activeSelf;
            bool origMain = mainMenu != null && mainMenu.activeSelf;
            bool origLobby = lobbyScreen != null && lobbyScreen.activeSelf;
            bool origShop = shopModal.activeSelf;
            bool origSet = setModal != null && setModal.activeSelf;
            bool origProf = profModal != null && profModal.activeSelf;
            bool origQuit = quitModal != null && quitModal.activeSelf;
            bool origMasc = mascModal != null && mascModal.activeSelf;
            bool origSumRes = sumResModal != null && sumResModal.activeSelf;
            bool origGachaPres = gachaPresModal != null && gachaPresModal.activeSelf;
            bool origFairyTrans = fairyTrans != null && fairyTrans.activeSelf;
            bool origFairyTouch = fairyTouch != null && fairyTouch.activeSelf;

            CanvasGroup lobbyCG = lobbyScreen != null ? lobbyScreen.GetComponent<CanvasGroup>() : null;
            float origLobbyAlpha = lobbyCG != null ? lobbyCG.alpha : 1f;

            try
            {
                mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                mainCanvas.worldCamera = mainCam;
                mainCanvas.planeDistance = 5f;

                if (splashScreen != null) splashScreen.SetActive(false);
                if (mainMenu != null) mainMenu.SetActive(false);
                if (fairyTrans != null) fairyTrans.SetActive(false);
                if (fairyTouch != null) fairyTouch.SetActive(false);
                if (setModal != null) setModal.SetActive(false);
                if (profModal != null) profModal.SetActive(false);
                if (quitModal != null) quitModal.SetActive(false);
                if (mascModal != null) mascModal.SetActive(false);
                if (sumResModal != null) sumResModal.SetActive(false);
                if (gachaPresModal != null) gachaPresModal.SetActive(false);

                if (lobbyScreen != null)
                {
                    lobbyScreen.SetActive(true);
                    if (lobbyCG != null) lobbyCG.alpha = 1f;
                }
                shopModal.SetActive(true);

                var shopAnimCtrl = shopModal.GetComponent<ShopUIAnimationController>();
                var allTrans = shopModal.GetComponentsInChildren<Transform>(true);
                RectTransform recPanel = null, pickPanel = null, mascPanel = null, inGamePanel = null, lobbyPanel = null;

                foreach (var t in allTrans)
                {
                    if (t.name == "RecommendedPanel") recPanel = t.GetComponent<RectTransform>();
                    if (t.name == "PickupPanel") pickPanel = t.GetComponent<RectTransform>();
                    if (t.name == "MascotsPanel") mascPanel = t.GetComponent<RectTransform>();
                    if (t.name == "InGameThemesPanel") inGamePanel = t.GetComponent<RectTransform>();
                    if (t.name == "LobbyThemesPanel") lobbyPanel = t.GetComponent<RectTransform>();
                }
                var bannerCtrl = shopModal.GetComponentInChildren<AnimatedPickupBannerController>(true);

                System.Action<int, RectTransform> prepTab = (idx, panel) =>
                {
                    if (lobbyMgr != null) lobbyMgr.SelectShopTab(idx);
                    if (shopAnimCtrl != null) shopAnimCtrl.SnapTab(idx, panel);
                    if (panel != null)
                    {
                        var cg = panel.GetComponent<CanvasGroup>();
                        if (cg != null) cg.alpha = 1f;
                    }
                    if (idx == 1 && bannerCtrl != null)
                    {
                        bannerCtrl.StopVideo();
                    }
                    Canvas.ForceUpdateCanvases();
                };

                // 1. Capture Pickup Tab (Tab 1)
                prepTab(1, pickPanel);
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_pickup_horizontal_animated.png"));

                // 2. Capture Recommended Tab (Tab 0)
                prepTab(0, recPanel);
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_rec_horizontal.png"));

                // 3. Capture Mascot Tab (Tab 2)
                prepTab(2, mascPanel);
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_mascots_horizontal.png"));

                // 4. Capture In-Game Themes Tab (Tab 3)
                prepTab(3, inGamePanel);
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_themes_horizontal.png"));

                // 5. Capture Lobby Themes Tab (Tab 4)
                prepTab(4, lobbyPanel);
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_lobbythemes_horizontal.png"));

                // Back to Pickup Tab
                prepTab(1, pickPanel);

                Debug.Log("[InspectUIMenu] Successfully captured Shop Horizontal Tabs screenshots!");
            }
            finally
            {
                mainCanvas.renderMode = origRenderMode;
                mainCanvas.worldCamera = origWorldCam;
                if (splashScreen != null) splashScreen.SetActive(origSplash);
                if (mainMenu != null) mainMenu.SetActive(origMain);
                if (lobbyScreen != null) lobbyScreen.SetActive(origLobby);
                if (lobbyCG != null) lobbyCG.alpha = origLobbyAlpha;
                shopModal.SetActive(origShop);
                if (setModal != null) setModal.SetActive(origSet);
                if (profModal != null) profModal.SetActive(origProf);
                if (quitModal != null) quitModal.SetActive(origQuit);
                if (mascModal != null) mascModal.SetActive(origMasc);
                if (sumResModal != null) sumResModal.SetActive(origSumRes);
                if (gachaPresModal != null) gachaPresModal.SetActive(origGachaPres);
                if (fairyTrans != null) fairyTrans.SetActive(origFairyTrans);
                if (fairyTouch != null) fairyTouch.SetActive(origFairyTouch);
            }
        }

        [MenuItem("Block Blast/Mobile/Apply Mobile Store Settings")]
        public static void ApplyMobileStoreSettings()
        {
            // 1. Target AAB (Android App Bundle)
            EditorUserBuildSettings.buildAppBundle = true;

            // 2. Lock Portrait Orientation
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            // 3. 64-bit Architecture
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

            // 4. Assign App Icons
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/AppIcon.png");
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new Texture2D[] { icon });
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
                Debug.Log("<color=green>[MobileStoreSettings] Successfully set Android & Default AppIcon!</color>");
            }
            else
            {
                Debug.LogWarning("[MobileStoreSettings] AppIcon.png not found!");
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=cyan>[MobileStoreSettings] AAB=true, Portrait=true, ARM64=true, AppIcon assigned!</color>");
        }

        [MenuItem("Block Blast/Debug/Capture Probability and Settings")]
        public static void CaptureProbabilityAndSettings()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var lobbyMgr = Object.FindObjectOfType<LobbyManager>(true);
            var all = Resources.FindObjectsOfTypeAll<GameObject>();

            GameObject splashScreen = null;
            GameObject mainMenu = null;
            GameObject lobbyScreen = null;
            GameObject probModal = null;
            GameObject setModal = null;
            Canvas mainCanvas = null;
            Camera mainCam = Camera.main ?? Object.FindObjectOfType<Camera>();

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "SplashScreen" || g.name == "SplashScreenOverlay") splashScreen = g;
                if (g.name == "MainMenuScreen") mainMenu = g;
                if (g.name == "LobbyScreen") lobbyScreen = g;
                if (g.name == "ProbabilityModal") probModal = g;
                if (g.name == "SettingsModal") setModal = g;
                if ((g.name == "Canvas" || g.name == "BlockBlastCanvas") && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
            }

            if (mainCanvas == null) return;

            RenderMode origMode = mainCanvas.renderMode;
            Camera origCam = mainCanvas.worldCamera;
            CanvasGroup lobbyCG = lobbyScreen != null ? lobbyScreen.GetComponent<CanvasGroup>() : null;
            float origAlpha = lobbyCG != null ? lobbyCG.alpha : 1f;

            try
            {
                mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                mainCanvas.worldCamera = mainCam;
                mainCanvas.planeDistance = 5f;

                if (splashScreen != null) splashScreen.SetActive(false);
                if (mainMenu != null) mainMenu.SetActive(false);
                if (lobbyScreen != null)
                {
                    lobbyScreen.SetActive(true);
                    if (lobbyCG != null) lobbyCG.alpha = 1f;
                }

                // 1. Capture Probability Modal
                if (setModal != null) setModal.SetActive(false);
                if (probModal != null)
                {
                    probModal.SetActive(true);
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_probability_modal.png"));
                    probModal.SetActive(false);
                }

                // 2. Capture Settings Modal (with mobile toggles)
                if (setModal != null)
                {
                    setModal.SetActive(true);
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_settings_modal_mobile.png"));
                    setModal.SetActive(false);
                }

                Debug.Log("[InspectUIMenu] Successfully captured Probability and Settings screenshots!");
            }
            finally
            {
                mainCanvas.renderMode = origMode;
                mainCanvas.worldCamera = origCam;
                if (lobbyCG != null) lobbyCG.alpha = origAlpha;
            }
        }

        [MenuItem("Block Blast/Debug/Capture Pickup Banner Video And Live")]
        public static void CapturePickupBannerVideoAndLive()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var lobbyMgr = Object.FindObjectOfType<LobbyManager>(true);
            var bannerCtrl = Object.FindObjectOfType<AnimatedPickupBannerController>(true);
            var all = Resources.FindObjectsOfTypeAll<GameObject>();

            GameObject shopModal = null;
            GameObject lobbyScreen = null;
            GameObject setModal = null;
            GameObject probModal = null;
            GameObject quitModal = null;
            GameObject mascModal = null;
            GameObject sumResModal = null;
            Canvas mainCanvas = null;
            Camera mainCam = Camera.main ?? Object.FindObjectOfType<Camera>();

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "ShopModal") shopModal = g;
                if (g.name == "LobbyScreen") lobbyScreen = g;
                if (g.name == "SettingsModal") setModal = g;
                if (g.name == "ProbabilityModal") probModal = g;
                if (g.name == "QuitConfirmModal") quitModal = g;
                if (g.name == "MascotModal") mascModal = g;
                if (g.name == "SummonResultModal") sumResModal = g;
                if ((g.name == "Canvas" || g.name == "BlockBlastCanvas") && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
            }

            if (mainCanvas == null || shopModal == null) return;

            RenderMode origMode = mainCanvas.renderMode;
            Camera origCam = mainCanvas.worldCamera;
            CanvasGroup lobbyCG = lobbyScreen != null ? lobbyScreen.GetComponent<CanvasGroup>() : null;
            float origAlpha = lobbyCG != null ? lobbyCG.alpha : 1f;

            try
            {
                mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                mainCanvas.worldCamera = mainCam;
                mainCanvas.planeDistance = 5f;

                if (setModal != null) setModal.SetActive(false);
                if (probModal != null) probModal.SetActive(false);
                if (quitModal != null) quitModal.SetActive(false);
                if (mascModal != null) mascModal.SetActive(false);
                if (sumResModal != null) sumResModal.SetActive(false);

                if (lobbyScreen != null)
                {
                    lobbyScreen.SetActive(true);
                    if (lobbyCG != null) lobbyCG.alpha = 1f;
                }
                shopModal.SetActive(true);

                if (lobbyMgr != null) lobbyMgr.SelectShopTab(1);

                // 1. Play Video Intro & Capture Video State
                if (bannerCtrl != null)
                {
                    bannerCtrl.PlayIntroVideo();
                }
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_pickup_tab_video_intro.png"));

                // 2. Skip to Live State & Capture Clean Background with Live Mascot
                if (bannerCtrl != null)
                {
                    bannerCtrl.ShowLiveElements(true);
                }
                CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_pickup_tab_live_clean.png"));

                Debug.Log("[InspectUIMenu] Successfully captured Pickup Banner Video & Live screenshots!");
            }
            finally
            {
                mainCanvas.renderMode = origMode;
                mainCanvas.worldCamera = origCam;
                if (lobbyCG != null) lobbyCG.alpha = origAlpha;
            }
        }

        [MenuItem("Block Blast/Debug/Capture All Pastel Modals And Codex")]
        public static void CaptureAllPastelModalsAndCodex()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var lobbyMgr = Object.FindObjectOfType<LobbyManager>(true);
            var bannerCtrl = Object.FindObjectOfType<AnimatedPickupBannerController>(true);
            var all = Resources.FindObjectsOfTypeAll<GameObject>();

            GameObject shopModal = null;
            GameObject lobbyScreen = null;
            GameObject probModal = null;
            GameObject pickupSkillModal = null;
            GameObject codexModal = null;
            GameObject mascotDetailModal = null;
            GameObject sumResModal = null;
            GameObject splashScreen = null;
            GameObject mainMenu = null;
            GameObject setModal = null;
            GameObject profModal = null;
            GameObject quitModal = null;
            GameObject fairyTrans = null;
            GameObject fairyTouch = null;
            Canvas mainCanvas = null;
            Camera mainCam = Camera.main ?? Object.FindObjectOfType<Camera>();

            foreach (var g in all)
            {
                if (!g.scene.isLoaded) continue;
                if (g.name == "SplashScreen" || g.name == "SplashScreenOverlay") splashScreen = g;
                if (g.name == "MainMenuScreen") mainMenu = g;
                if (g.name == "ShopModal") shopModal = g;
                if (g.name == "LobbyScreen") lobbyScreen = g;
                if (g.name == "SettingsModal") setModal = g;
                if (g.name == "ProfileModal") profModal = g;
                if (g.name == "QuitConfirmModal") quitModal = g;
                if (g.name == "ProbabilityModal") probModal = g;
                if (g.name == "PickupSkillDetailModal") pickupSkillModal = g;
                if (g.name == "MascotCodexModal") codexModal = g;
                if (g.name == "MascotDetailModal") mascotDetailModal = g;
                if (g.name == "SummonResultModal") sumResModal = g;
                if (g.name == "FairyScreenTransitionOverlay") fairyTrans = g;
                if (g.name == "FairyTouchFXOverlay") fairyTouch = g;
                if ((g.name == "Canvas" || g.name == "BlockBlastCanvas") && mainCanvas == null) mainCanvas = g.GetComponent<Canvas>();
            }

            if (mainCanvas == null) return;

            RenderMode origMode = mainCanvas.renderMode;
            Camera origCam = mainCanvas.worldCamera;
            CanvasGroup lobbyCG = lobbyScreen != null ? lobbyScreen.GetComponent<CanvasGroup>() : null;
            float origAlpha = lobbyCG != null ? lobbyCG.alpha : 1f;

            try
            {
                mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                mainCanvas.worldCamera = mainCam;
                mainCanvas.planeDistance = 5f;

                if (splashScreen != null) splashScreen.SetActive(false);
                if (mainMenu != null) mainMenu.SetActive(false);
                if (setModal != null) setModal.SetActive(false);
                if (profModal != null) profModal.SetActive(false);
                if (quitModal != null) quitModal.SetActive(false);
                if (probModal != null) probModal.SetActive(false);
                if (pickupSkillModal != null) pickupSkillModal.SetActive(false);
                if (codexModal != null) codexModal.SetActive(false);
                if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
                if (sumResModal != null) sumResModal.SetActive(false);
                if (shopModal != null) shopModal.SetActive(false);
                if (fairyTrans != null) fairyTrans.SetActive(false);
                if (fairyTouch != null) fairyTouch.SetActive(false);

                if (lobbyScreen != null)
                {
                    lobbyScreen.SetActive(true);
                    if (lobbyCG != null) lobbyCG.alpha = 1f;
                }

                // 1. Capture Clean Pickup Tab
                if (shopModal != null)
                {
                    shopModal.SetActive(true);
                    if (lobbyMgr != null) lobbyMgr.SelectShopTab(1);
                    if (bannerCtrl != null) bannerCtrl.ShowLiveElements(true);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_shop_pickup_clean.png"));
                    shopModal.SetActive(false);
                }

                // 2. Capture Probability Modal (Soft pastel ivory card, 1.0% rate table)
                if (probModal != null)
                {
                    probModal.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_probability_modal_pastel.png"));
                    probModal.SetActive(false);
                }

                // 3. Capture Pickup Skill Detail Modal
                if (pickupSkillModal != null)
                {
                    pickupSkillModal.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_pickup_skill_detail_modal.png"));
                    pickupSkillModal.SetActive(false);
                }

                // 4. Capture Mascot Codex Modal (3x3 Grid, 9 mascots)
                if (codexModal != null)
                {
                    if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
                    if (lobbyMgr != null) lobbyMgr.OpenMascotModal();
                    else codexModal.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_mascot_codex_grid.png"));
                }

                // 5. Capture Mascot Detail Modal (냥냥시노비 도감 육성 팝업)
                if (mascotDetailModal != null)
                {
                    if (lobbyMgr != null) lobbyMgr.OpenMascotDetail(0);
                    else mascotDetailModal.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_mascot_detail_modal.png"));
                }

                // 6. Test Language Switch (Switch to English and Japanese and capture previews)
                if (lobbyMgr != null)
                {
                    lobbyMgr.SelectLanguage(GameLanguage.EN);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_mascot_detail_en.png"));

                    lobbyMgr.SelectLanguage(GameLanguage.JA);
                    Canvas.ForceUpdateCanvases();
                    CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_mascot_detail_ja.png"));

                    if (pickupSkillModal != null)
                    {
                        if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
                        if (codexModal != null) codexModal.SetActive(false);
                        pickupSkillModal.SetActive(true);
                        Canvas.ForceUpdateCanvases();
                        CaptureCameraView(mainCam, Path.Combine(brainDir, "verify_pickup_skill_ja.png"));
                        pickupSkillModal.SetActive(false);
                    }

                    // Restore to Korean
                    lobbyMgr.SelectLanguage(GameLanguage.KO);
                }

                // Close opened modals
                if (mascotDetailModal != null) mascotDetailModal.SetActive(false);
                if (codexModal != null) codexModal.SetActive(false);

                Debug.Log("[InspectUIMenu] Successfully captured all pastel modals and codex previews!");
            }
            finally
            {
                mainCanvas.renderMode = origMode;
                mainCanvas.worldCamera = origCam;
                if (lobbyCG != null) lobbyCG.alpha = origAlpha;
            }
        }
    }
}
#endif
