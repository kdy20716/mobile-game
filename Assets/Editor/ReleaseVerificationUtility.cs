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

        [MenuItem("Block Blast/Debug/Inspect Mascot Modal")]
        public static void InspectMascotModal()
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject modal = null;
            foreach (var g in all)
            {
                if (g.name == "MascotModal" && !EditorUtility.IsPersistent(g))
                {
                    modal = g;
                    break;
                }
            }
            if (modal == null)
            {
                Debug.LogError("[Inspect] MascotModal not found!");
                return;
            }
            Debug.Log($"[Inspect] MascotModal active={modal.activeSelf}, activeInHierarchy={modal.activeInHierarchy}");
            foreach (Transform t in modal.GetComponentsInChildren<Transform>(true))
            {
                var rt = t.GetComponent<RectTransform>();
                var img = t.GetComponent<UnityEngine.UI.Image>();
                var txt = t.GetComponent<TMPro.TMP_Text>();
                var mask = t.GetComponent<UnityEngine.UI.Mask>();
                var rmask = t.GetComponent<UnityEngine.UI.RectMask2D>();
                Debug.Log($"[Child] {t.name} (act={t.gameObject.activeSelf}) pos={(rt!=null?rt.anchoredPosition.ToString():"")} size={(rt!=null?rt.sizeDelta.ToString():"")} img={(img!=null?img.enabled.ToString():"no")} txt={(txt!=null?txt.text:"no")} mask={(mask!=null)} rmask={(rmask!=null)}");
            }
        }

        [MenuItem("Block Blast/Debug/Open Mascot Modal")]
        public static void OpenMascotModal()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OpenMascotModal();
                Debug.Log("<color=cyan>[ReleaseVerification] Opened Mascot Modal!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Open Shop Modal")]
        public static void OpenShopModal()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OpenShopModal();
                Debug.Log("<color=cyan>[ReleaseVerification] Opened Shop Modal!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Shop Switch To Pickup Tab")]
        public static void ShopSwitchToPickupTab()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SelectShopTab(1);
                Debug.Log("<color=cyan>[ReleaseVerification] Switched Shop to Pickup Tab!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Summon 10 Pickup")]
        public static void Summon10Pickup()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.SummonPickup(10);
                Debug.Log("<color=cyan>[ReleaseVerification] Summoned 10 Pickup!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Equip Special Mascot And Start")]
        public static void EquipSpecialAndStart()
        {
            if (LobbyManager.Instance != null)
            {
                PlayerPrefs.SetInt("Mallang_Mascot_Owned_4", 1);
                PlayerPrefs.Save();
                if (GachaPresentationController.Instance != null) GachaPresentationController.Instance.CloseModal();
                LobbyManager.Instance.CloseSummonResultModal();
                LobbyManager.Instance.CloseShopModal();
                LobbyManager.Instance.CloseMascotModal();
                LobbyManager.Instance.EquipMascot(4);
                LobbyManager.Instance.StartGameFromLobby();
                Debug.Log("<color=cyan>[ReleaseVerification] Equipped Special Mascot and Started Game!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Trigger Special Board Clear Skill")]
        public static void TriggerSpecialBoardClearSkill()
        {
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ForceTriggerBoardClearSkill();
                Debug.Log("<color=cyan>[ReleaseVerification] Triggered Special Board Clear Skill Cutin!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Show Special Cutin Peak Frame")]
        public static void ShowSpecialCutinPeakFrame()
        {
            if (BlockBlastUIManager.Instance != null)
            {
                BlockBlastUIManager.Instance.ShowCutinStaticForDebug();
                Debug.Log("<color=cyan>[ReleaseVerification] Showing Special Cutin Peak Frame!</color>");
            }
        }

        [MenuItem("Block Blast/Debug/Test Single Pull Gacha Ball Screen")]
        public static void TestSinglePullGachaBallScreen()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.ShowLobby();
            }

            if (GachaPresentationController.Instance != null)
            {
                var drops = new System.Collections.Generic.List<GachaDropItem>();
                drops.Add(new GachaDropItem { isSpecial = true, mascotIndex = 4, shardCount = 60, isDuplicateSpecial = false });

                GachaPresentationController.Instance.StartGachaSequence(drops, () =>
                {
                    Debug.Log("<color=cyan>[ReleaseVerification] Single Pull Gacha Completed!</color>");
                });
            }
        }

        [MenuItem("Block Blast/Debug/Test 10 Pull Gacha Ball Screen")]
        public static void Test10PullGachaBallScreen()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.ShowLobby();
            }

            if (GachaPresentationController.Instance != null)
            {
                var drops = new System.Collections.Generic.List<GachaDropItem>();
                for (int i = 0; i < 9; i++)
                {
                    drops.Add(new GachaDropItem { isSpecial = false, mascotIndex = i % 4, shardCount = 5, isDuplicateSpecial = false });
                }
                // 10th ball is glowing rainbow special mascot!
                drops.Add(new GachaDropItem { isSpecial = true, mascotIndex = 4, shardCount = 60, isDuplicateSpecial = false });

                GachaPresentationController.Instance.StartGachaSequence(drops, () =>
                {
                    Debug.Log("<color=cyan>[ReleaseVerification] 10 Pull Gacha Completed!</color>");
                });
            }
        }

        [MenuItem("Block Blast/Debug/Test Open Special Mascot Ball")]
        public static void TestOpenSpecialMascotBall()
        {
            if (GachaPresentationController.Instance != null)
            {
                GachaPresentationController.Instance.OpenSlot(9);
            }
        }

        [MenuItem("Block Blast/Debug/Show Special Mascot Climax Overlay")]
        public static void ShowSpecialMascotClimaxOverlay()
        {
            if (GachaPresentationController.Instance != null)
            {
                var drop = new GachaDropItem { isSpecial = true, mascotIndex = 4, shardCount = 60, isDuplicateSpecial = false };
                GachaPresentationController.Instance.ShowClimaxDirect(drop);
            }
        }
    }
}
#endif
