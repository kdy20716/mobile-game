#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BlockBlast.Editor
{
    public static class BlockBlastSceneBuilder
    {
        [MenuItem("Block Blast/Generate Block Blast Mobile Scene")]
        public static void GenerateBlockBlastScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera: Sweet Night Pastel Background (Deep dreamy lavender indigo)
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.11f, 0.12f, 0.20f); // #1C1E33 (Dreamy Sweet Night)
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";

            // 2. Generate Adorable Glossy Jelly Sprites
            Sprite jellyTileSprite = CuteBlockTextureGenerator.GetOrCreateCuteJellySprite("Jelly_Tile_Base", new Color(0.4f, 0.8f, 1f));
            Sprite starBombSprite = CuteBlockTextureGenerator.GetOrCreateCuteJellySprite("Jelly_StarBomb", BlockShapeData.BombColor, true);

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf")
                ?? AssetDatabase.LoadAssetAtPath<Font>("Assets/SharedAssets/TextMeshPro/Fonts/LiberationSans.ttf");

            // 3. EventSystem
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                es.AddComponent<StandaloneInputModule>();
#endif
            }

            // 4. Canvas & Scaler (Mobile Portrait 1080 x 1920)
            GameObject canvasObj = new GameObject("BlockBlastCanvas", typeof(RectTransform));
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f; // match width for portrait mobile

            canvasObj.AddComponent<GraphicRaycaster>();

            // 5. Game Managers Object
            GameObject mgrObj = new GameObject("GameManagers");
            var gridMgr = mgrObj.AddComponent<BlockGridManager>();
            var spawner = mgrObj.AddComponent<BlockSpawner>();
            var audioMgr = mgrObj.AddComponent<BlockAudioManager>();
            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetSprites(jellyTileSprite, starBombSprite);

            // ==========================================
            // CUTE PASTEL UI HIERARCHY
            // ==========================================

            // --- A. Header (Top) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(canvasObj.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -115), new Vector2(-60, 150));

            // Current Score (Left)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(120, 0), new Vector2(250, 130));

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 24, TextAnchor.MiddleLeft, defaultFont, new Color(0.7f, 0.75f, 0.9f));
            SetRect(scoreLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -15), new Vector2(0, 35));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 56, TextAnchor.MiddleLeft, defaultFont, new Color(1f, 0.76f, 0.25f)); // Mango Gold
            SetRect(scoreVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 15), new Vector2(0, 70));

            // Center Title
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(360, 130));

            GameObject mainTitle = CreateText(titleBox.transform, "MainTitle", "BLOCK BLAST", 36, TextAnchor.MiddleCenter, defaultFont, new Color(0.45f, 0.88f, 1f));
            SetRect(mainTitle, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 45));

            GameObject subTitle = CreateText(titleBox.transform, "SubTitle", "✨ E L E M E N T A L ✨", 19, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.55f, 0.72f)); // Strawberry
            SetRect(subTitle, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 20), new Vector2(0, 35));

            // Best Score (Right)
            GameObject bestBox = new GameObject("BestBox", typeof(RectTransform));
            bestBox.transform.SetParent(headerObj.transform, false);
            SetRect(bestBox, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-120, 0), new Vector2(250, 130));

            GameObject bestLabel = CreateText(bestBox.transform, "Label", "BEST", 24, TextAnchor.MiddleRight, defaultFont, new Color(0.7f, 0.75f, 0.9f));
            SetRect(bestLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -15), new Vector2(0, 35));

            GameObject bestVal = CreateText(bestBox.transform, "Value", "0", 56, TextAnchor.MiddleRight, defaultFont, new Color(0.95f, 0.95f, 1f));
            SetRect(bestVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 15), new Vector2(0, 70));

            // --- B. Skills Bar (Below Header) ---
            GameObject skillsBar = new GameObject("SkillsBar", typeof(RectTransform));
            skillsBar.transform.SetParent(canvasObj.transform, false);
            SetRect(skillsBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -235), new Vector2(980, 90));
            Image sBarBg = skillsBar.AddComponent<Image>();
            sBarBg.color = new Color(0.16f, 0.17f, 0.28f, 0.95f); // Soft rounded container

            // Fever Gauge Bar
            GameObject feverLabel = CreateText(skillsBar.transform, "FeverLabel", "FEVER", 24, TextAnchor.MiddleLeft, defaultFont, new Color(1f, 0.85f, 0.35f));
            SetRect(feverLabel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(75, 0), new Vector2(100, 40));

            GameObject feverBg = new GameObject("FeverBg", typeof(RectTransform), typeof(Image));
            feverBg.transform.SetParent(skillsBar.transform, false);
            SetRect(feverBg, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(305, 0), new Vector2(280, 28));
            feverBg.GetComponent<Image>().color = new Color(0.10f, 0.11f, 0.18f);

            GameObject feverFill = new GameObject("FeverFill", typeof(RectTransform), typeof(Image));
            feverFill.transform.SetParent(feverBg.transform, false);
            SetRect(feverFill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fFillImg = feverFill.GetComponent<Image>();
            fFillImg.type = Image.Type.Filled;
            fFillImg.fillMethod = Image.FillMethod.Horizontal;
            fFillImg.fillAmount = 0f;
            fFillImg.color = new Color(1f, 0.45f, 0.65f); // Sweet Strawberry Pink

            // Cute Squeaky Hammer Button (Capsule)
            GameObject hammerBtnObj = CreateButton(skillsBar.transform, "BtnHammer", "🔨 뿅망치", defaultFont, new Vector2(1, 0.5f), new Vector2(-235, 0), new Vector2(185, 64), new Color(0.95f, 0.35f, 0.55f));
            var btnHammer = hammerBtnObj.GetComponent<Button>();

            // Hammer Badge
            GameObject badgeObj = new GameObject("Badge", typeof(RectTransform), typeof(Image));
            badgeObj.transform.SetParent(hammerBtnObj.transform, false);
            SetRect(badgeObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-4, -4), new Vector2(32, 32));
            badgeObj.GetComponent<Image>().color = new Color(1f, 0.85f, 0.2f); // Gold badge
            GameObject badgeText = CreateText(badgeObj.transform, "Text", "1", 18, TextAnchor.MiddleCenter, defaultFont, new Color(0.2f, 0.1f, 0f));
            SetRect(badgeText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Cute Rotate Button (Capsule)
            GameObject rotateBtnObj = CreateButton(skillsBar.transform, "BtnRotate", "🔄 돌리기", defaultFont, new Vector2(1, 0.5f), new Vector2(-40, 0), new Vector2(165, 64), new Color(0.20f, 0.78f, 0.62f));
            var btnRotate = rotateBtnObj.GetComponent<Button>();

            // --- C. Board Container (Center) ---
            GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
            boardContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(boardContainer, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 105), new Vector2(980, 980));
            Image bBg = boardContainer.AddComponent<Image>();
            bBg.color = new Color(0.15f, 0.16f, 0.27f, 0.98f); // Cute macaron tray

            // 64 Grid Cells
            float cellSize = 108f;
            float spacing = 10f;
            float totalGridWidth = (8 * cellSize) + (7 * spacing);
            float startGridX = -totalGridWidth * 0.5f + cellSize * 0.5f;
            float startGridY = totalGridWidth * 0.5f - cellSize * 0.5f;

            for (int r = 0; r < BlockGridManager.GridSize; r++)
            {
                for (int c = 0; c < BlockGridManager.GridSize; c++)
                {
                    GameObject cellObj = new GameObject($"Cell_{r}_{c}", typeof(RectTransform), typeof(Image), typeof(Button));
                    cellObj.transform.SetParent(boardContainer.transform, false);

                    RectTransform rt = cellObj.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cellSize, cellSize);
                    rt.anchoredPosition = new Vector2(startGridX + c * (cellSize + spacing), startGridY - r * (cellSize + spacing));

                    Image bgImg = cellObj.GetComponent<Image>();
                    bgImg.color = new Color(0.10f, 0.11f, 0.18f); // Soft dark indented cell

                    // Fill Child (Jelly Sprite)
                    GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                    fillObj.transform.SetParent(cellObj.transform, false);
                    SetRect(fillObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image fillImg = fillObj.GetComponent<Image>();
                    fillImg.sprite = jellyTileSprite;
                    fillObj.SetActive(false);

                    // Bomb Icon Child (Star Bomb)
                    GameObject bIconObj = new GameObject("BombIcon", typeof(RectTransform), typeof(Image));
                    bIconObj.transform.SetParent(cellObj.transform, false);
                    SetRect(bIconObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(85, 85));
                    Image bIconImg = bIconObj.GetComponent<Image>();
                    bIconImg.sprite = starBombSprite;
                    bIconObj.SetActive(false);

                    // Highlight Child
                    GameObject hlObj = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
                    hlObj.transform.SetParent(cellObj.transform, false);
                    SetRect(hlObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image hlImg = hlObj.GetComponent<Image>();
                    hlImg.color = new Color(1f, 1f, 1f, 0.4f);
                    hlObj.SetActive(false);

                    BlockCellUI cellUI = cellObj.AddComponent<BlockCellUI>();
                    cellUI.SetupComponents(bgImg, fillImg, bIconImg, hlObj);
                    cellUI.Init(r, c);

                    gridMgr.RegisterCell(r, c, cellUI);
                }
            }

            // Combo Floating Text
            GameObject comboObj = CreateText(boardContainer.transform, "ComboPopup", "💖 COMBO x2!", 60, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.55f, 0.7f));
            SetRect(comboObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650, 220));
            comboObj.SetActive(false);

            // --- D. Hand Slots (Bottom) ---
            GameObject handContainer = new GameObject("HandContainer", typeof(RectTransform));
            handContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(handContainer, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 240), new Vector2(980, 260));
            Image hBg = handContainer.AddComponent<Image>();
            hBg.color = new Color(0.15f, 0.16f, 0.27f, 0.95f); // Macaron plate

            Transform[] slotTransforms = new Transform[3];
            float slotSpacing = 310f;
            for (int i = 0; i < 3; i++)
            {
                GameObject slot = new GameObject($"Slot_{i}", typeof(RectTransform));
                slot.transform.SetParent(handContainer.transform, false);
                SetRect(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(280, 240));
                slotTransforms[i] = slot.transform;
            }

            spawner.SetupReferences(slotTransforms, canvas, jellyTileSprite, starBombSprite);

            // Bottom Guide Text
            GameObject guideObj = CreateText(canvasObj.transform, "GuideText", "✨ 블록을 쏙! 넣어 라인을 팡팡 터뜨려요 💖", 24, TextAnchor.MiddleCenter, defaultFont, new Color(0.7f, 0.75f, 0.9f));
            SetRect(guideObj, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(980, 50));

            // --- E. Game Over Modal ---
            GameObject modalObj = new GameObject("GameOverModal", typeof(RectTransform));
            modalObj.transform.SetParent(canvasObj.transform, false);
            SetRect(modalObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image modalBg = modalObj.AddComponent<Image>();
            modalBg.color = new Color(0.06f, 0.07f, 0.12f, 0.92f);

            GameObject dialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            dialog.transform.SetParent(modalObj.transform, false);
            SetRect(dialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(680, 740));
            dialog.GetComponent<Image>().color = new Color(0.16f, 0.17f, 0.28f);

            GameObject overTitle = CreateText(dialog.transform, "Title", "NO MORE MOVES", 48, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.45f, 0.55f));
            SetRect(overTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(600, 80));

            GameObject overDesc = CreateText(dialog.transform, "Desc", "더 이상 넣을 수 있는 자리가 없어요!", 26, TextAnchor.MiddleCenter, defaultFont, new Color(0.75f, 0.8f, 0.95f));
            SetRect(overDesc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -165), new Vector2(600, 50));

            // Score Box in Modal
            GameObject modalScoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            modalScoreBox.transform.SetParent(dialog.transform, false);
            SetRect(modalScoreBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(560, 220));
            modalScoreBox.GetComponent<Image>().color = new Color(0.10f, 0.11f, 0.18f);

            GameObject finalLabel = CreateText(modalScoreBox.transform, "FLabel", "FINAL SCORE", 24, TextAnchor.MiddleCenter, defaultFont, new Color(0.7f, 0.75f, 0.9f));
            SetRect(finalLabel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(500, 40));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 64, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.76f, 0.25f));
            SetRect(finalVal, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -100), new Vector2(500, 70));

            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", "BEST: 0", 26, TextAnchor.MiddleCenter, defaultFont, new Color(0.85f, 0.9f, 1f));
            SetRect(mBestVal, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(500, 40));

            GameObject restartBtn = CreateButton(dialog.transform, "BtnRestart", "한 번 더 하기 ✨", defaultFont, new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(560, 95), new Color(1f, 0.48f, 0.64f));

            // Bind UI References
            uiMgr.SetupReferences(
                scoreVal.GetComponent<Text>(),
                bestVal.GetComponent<Text>(),
                fFillImg,
                btnHammer,
                badgeText.GetComponent<Text>(),
                btnRotate,
                comboObj.GetComponent<Text>(),
                boardContainer.GetComponent<RectTransform>(),
                modalObj,
                finalVal.GetComponent<Text>(),
                mBestVal.GetComponent<Text>(),
                restartBtn.GetComponent<Button>()
            );

            // 6. Save Scene
            string scenePath = "Assets/Scenes/BlockBlastScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Debug.Log("<color=#FF7AA2><b>[Block Blast]</b> 귀염뽀짝 파스텔 젤리 모바일 씬이 성공적으로 완성되었습니다! (Assets/Scenes/BlockBlastScene.unity)</color>");
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Font font, Vector2 anchor, Vector2 pos, Vector2 size, Color bgColor)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            btnObj.GetComponent<Image>().color = bgColor;

            SetRect(btnObj, anchor, anchor, pos, size);

            GameObject textObj = CreateText(btnObj.transform, "Text", label, 26, TextAnchor.MiddleCenter, font, Color.white);
            SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().raycastTarget = false;

            return btnObj;
        }

        private static GameObject CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Font font, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
            obj.transform.SetParent(parent, false);
            Text t = obj.GetComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.alignment = alignment;
            t.font = font;
            t.color = color;
            return obj;
        }

        private static void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }
    }
}
#endif
