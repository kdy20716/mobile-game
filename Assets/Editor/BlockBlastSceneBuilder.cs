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
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("알림", "현재 게임이 재생(Play) 중입니다!\n\n상단의 Play (▶) 버튼을 눌러 정지한 후 메뉴를 다시 클릭해주세요.", "확인");
                return;
            }

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

            // Force reimport Jua font with high-res 64px texture settings
            AssetDatabase.ImportAsset("Assets/Fonts/Jua-Regular.ttf", ImportAssetOptions.ForceUpdate);

            // 2. Generate 10+ Adorable Glossy Jelly Sprites & UI Textures
            Sprite jellyTileSprite = CuteBlockTextureGenerator.GetOrCreateCuteJellySprite("Jelly_Tile_Base", new Color(0.4f, 0.8f, 1f));
            Sprite starBombSprite = CuteBlockTextureGenerator.GetOrCreateCuteJellySprite("Jelly_StarBomb", BlockShapeData.BombColor, true);
            Sprite bgSprite = CuteBlockTextureGenerator.GetOrCreateBackgroundSprite();
            Sprite panelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Panel_Rounded", new Color(0.14f, 0.15f, 0.27f, 0.94f), new Color(0.32f, 0.38f, 0.62f, 0.85f));
            Sprite boardPanelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Board_Panel", new Color(0.12f, 0.13f, 0.24f, 0.96f), new Color(0.28f, 0.35f, 0.58f, 0.9f));
            Sprite scoreBoxSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Score_Box", new Color(0.10f, 0.11f, 0.20f, 0.90f), new Color(0.24f, 0.30f, 0.50f, 0.75f));
            Sprite btnPinkSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Pink", new Color(0.98f, 0.36f, 0.58f));
            Sprite btnTealSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Teal", new Color(0.18f, 0.78f, 0.62f));
            Sprite btnGoldSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Gold", new Color(1f, 0.78f, 0.22f));

            // Cute Character & Icon Sprites
            Sprite mascotSprite = CuteBlockTextureGenerator.GetOrCreateMascotSprite();
            Sprite crownSprite = CuteBlockTextureGenerator.GetOrCreateCrownSprite();
            Sprite flameSprite = CuteBlockTextureGenerator.GetOrCreateFlameSprite();
            Sprite diceSprite = CuteBlockTextureGenerator.GetOrCreateDiceSprite();
            Sprite arrowSprite = CuteBlockTextureGenerator.GetOrCreateRotateArrowSprite();

            // Cute Jua Korean/English Font
            Font cuteFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Jua-Regular.ttf")
                ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

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

            // 4. Canvas & Scaler (Universal Mobile Responsive: 1080 x 1920 Match Width)
            GameObject canvasObj = new GameObject("BlockBlastCanvas", typeof(RectTransform));
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.0f; // Match Width ensures board never clips on any device ratio (16:9, 19.5:9, 20:9, 4:3)!

            canvasObj.AddComponent<GraphicRaycaster>();

            // Fullscreen Dreamy Pastel Background
            GameObject bgObj = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(canvasObj.transform, false);
            bgObj.transform.SetAsFirstSibling();
            SetRect(bgObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image bgImgComp = bgObj.GetComponent<Image>();
            bgImgComp.sprite = bgSprite;
            bgImgComp.color = Color.white;
            bgImgComp.raycastTarget = false;

            // 5. Game Managers Object
            GameObject mgrObj = new GameObject("GameManagers");
            var gridMgr = mgrObj.AddComponent<BlockGridManager>();
            var spawner = mgrObj.AddComponent<BlockSpawner>();
            var audioMgr = mgrObj.AddComponent<BlockAudioManager>();
            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetSprites(jellyTileSprite, starBombSprite);

            // ==========================================
            // CUTE PASTEL UI HIERARCHY (RESPONSIVE)
            // ==========================================

            // --- A. Header (Top Anchor) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(canvasObj.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -115), new Vector2(-60, 150));

            // Current Score (Left)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(130, 0), new Vector2(260, 130));
            Image sBoxImg = scoreBox.GetComponent<Image>();
            sBoxImg.sprite = scoreBoxSprite;
            sBoxImg.type = Image.Type.Sliced;

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 28, TextAnchor.MiddleCenter, cuteFont, new Color(0.7f, 0.85f, 1f));
            SetRect(scoreLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 38));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 68, TextAnchor.MiddleCenter, cuteFont, new Color(1f, 0.84f, 0.35f)); // Mango Gold
            SetRect(scoreVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 18), new Vector2(0, 72));

            // Center Title & Mascots
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420, 130));

            // Left Mascot
            CreateImage(titleBox.transform, "MascotLeft", mascotSprite, new Vector2(0, 0.5f), new Vector2(20, 5), new Vector2(74, 74));
            // Right Mascot
            CreateImage(titleBox.transform, "MascotRight", mascotSprite, new Vector2(1, 0.5f), new Vector2(-20, 5), new Vector2(74, 74));

            GameObject mainTitle = CreateText(titleBox.transform, "MainTitle", "BLOCK BLAST", 52, TextAnchor.MiddleCenter, cuteFont, new Color(0.55f, 0.92f, 1f));
            SetRect(mainTitle, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -18), new Vector2(0, 60));

            GameObject subTitle = CreateText(titleBox.transform, "SubTitle", "✨ E L E M E N T A L ✨", 26, TextAnchor.MiddleCenter, cuteFont, new Color(1f, 0.60f, 0.78f)); // Strawberry
            SetRect(subTitle, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 20), new Vector2(0, 42));

            // Best Score (Right) with Golden Crown
            GameObject bestBox = new GameObject("BestBox", typeof(RectTransform), typeof(Image));
            bestBox.transform.SetParent(headerObj.transform, false);
            SetRect(bestBox, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-130, 0), new Vector2(260, 130));
            Image bBoxImg = bestBox.GetComponent<Image>();
            bBoxImg.sprite = scoreBoxSprite;
            bBoxImg.type = Image.Type.Sliced;

            // Crown on top of Best Box
            CreateImage(bestBox.transform, "CrownIcon", crownSprite, new Vector2(0.5f, 1), new Vector2(0, 22), new Vector2(64, 64));

            GameObject bestLabel = CreateText(bestBox.transform, "Label", "BEST", 28, TextAnchor.MiddleCenter, cuteFont, new Color(0.7f, 0.85f, 1f));
            SetRect(bestLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 38));

            GameObject bestVal = CreateText(bestBox.transform, "Value", "0", 68, TextAnchor.MiddleCenter, cuteFont, new Color(0.96f, 0.96f, 1f));
            SetRect(bestVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 18), new Vector2(0, 72));

            // --- B. Skills Bar (Below Header) ---
            GameObject skillsBar = new GameObject("SkillsBar", typeof(RectTransform), typeof(Image));
            skillsBar.transform.SetParent(canvasObj.transform, false);
            SetRect(skillsBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -240), new Vector2(980, 92));
            Image sBarBg = skillsBar.GetComponent<Image>();
            sBarBg.sprite = panelSprite;
            sBarBg.type = Image.Type.Sliced;
            sBarBg.color = Color.white;

            // Pink Flame Icon next to Fever
            CreateImage(skillsBar.transform, "FlameIcon", flameSprite, new Vector2(0, 0.5f), new Vector2(42, 0), new Vector2(46, 46));

            // Fever Gauge Bar
            GameObject feverLabel = CreateText(skillsBar.transform, "FeverLabel", "FEVER", 28, TextAnchor.MiddleLeft, cuteFont, new Color(1f, 0.85f, 0.35f));
            SetRect(feverLabel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(120, 0), new Vector2(100, 44));

            GameObject feverBg = new GameObject("FeverBg", typeof(RectTransform), typeof(Image));
            feverBg.transform.SetParent(skillsBar.transform, false);
            SetRect(feverBg, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(335, 0), new Vector2(250, 32));
            Image fBgImg = feverBg.GetComponent<Image>();
            fBgImg.sprite = scoreBoxSprite;
            fBgImg.type = Image.Type.Sliced;
            fBgImg.color = new Color(0.08f, 0.09f, 0.16f);

            GameObject feverFill = new GameObject("FeverFill", typeof(RectTransform), typeof(Image));
            feverFill.transform.SetParent(feverBg.transform, false);
            SetRect(feverFill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fFillImg = feverFill.GetComponent<Image>();
            fFillImg.type = Image.Type.Filled;
            fFillImg.fillMethod = Image.FillMethod.Horizontal;
            fFillImg.fillAmount = 0f;
            fFillImg.color = new Color(1f, 0.45f, 0.65f); // Sweet Strawberry Pink

            // 🎲 Skip Button (3D Glossy Jelly Button with Dice Icon)
            GameObject skipBtnObj = CreateButton(skillsBar.transform, "BtnSkip", "  스킵", cuteFont, new Vector2(1, 0.5f), new Vector2(-235, 0), new Vector2(195, 72), btnPinkSprite, 32);
            CreateImage(skipBtnObj.transform, "DiceIcon", diceSprite, new Vector2(0, 0.5f), new Vector2(34, 0), new Vector2(42, 42));
            var btnSkip = skipBtnObj.GetComponent<Button>();

            // Skip Badge
            GameObject badgeObj = new GameObject("Badge", typeof(RectTransform), typeof(Image));
            badgeObj.transform.SetParent(skipBtnObj.transform, false);
            SetRect(badgeObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-12, -8), new Vector2(84, 34));
            Image badgeImg = badgeObj.GetComponent<Image>();
            badgeImg.sprite = btnGoldSprite;
            badgeImg.type = Image.Type.Sliced;
            GameObject badgeText = CreateText(badgeObj.transform, "Text", "READY", 18, TextAnchor.MiddleCenter, cuteFont, new Color(0.2f, 0.1f, 0f));
            SetRect(badgeText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // 🔄 Rotate Button (3D Glossy Jelly Button with Arrow Icon)
            GameObject rotateBtnObj = CreateButton(skillsBar.transform, "BtnRotate", "  돌리기", cuteFont, new Vector2(1, 0.5f), new Vector2(-35, 0), new Vector2(175, 72), btnTealSprite, 32);
            CreateImage(rotateBtnObj.transform, "ArrowIcon", arrowSprite, new Vector2(0, 0.5f), new Vector2(30, 0), new Vector2(40, 40));
            var btnRotate = rotateBtnObj.GetComponent<Button>();

            // --- C. Board Container (Center Anchor) ---
            GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform), typeof(Image));
            boardContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(boardContainer, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(980, 980));
            Image bBg = boardContainer.GetComponent<Image>();
            bBg.sprite = boardPanelSprite;
            bBg.type = Image.Type.Sliced;
            bBg.color = Color.white;

            // 64 Grid Cells
            float cellSize = 108f;
            float spacing = 10f;
            float totalGridWidth = (8 * cellSize) + (7 * spacing);
            float startGridX = -totalGridWidth * 0.5f + cellSize * 0.5f;
            float startGridY = totalGridWidth * 0.5f - cellSize * 0.5f;

            // Connect board metrics for 100% precision snapping
            gridMgr.SetupBoardMetrics(boardContainer.GetComponent<RectTransform>(), cellSize, spacing, startGridX, startGridY);

            for (int r = 0; r < BlockGridManager.GridSize; r++)
            {
                for (int c = 0; c < BlockGridManager.GridSize; c++)
                {
                    GameObject cellObj = new GameObject($"Cell_{r}_{c}", typeof(RectTransform), typeof(Image));
                    cellObj.transform.SetParent(boardContainer.transform, false);

                    RectTransform rt = cellObj.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cellSize, cellSize);
                    rt.anchoredPosition = new Vector2(startGridX + c * (cellSize + spacing), startGridY - r * (cellSize + spacing));

                    Image bgCellImg = cellObj.GetComponent<Image>();
                    bgCellImg.color = new Color(0.10f, 0.11f, 0.18f);

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

                    // Highlight Child (Cute Glowing Jelly Preview)
                    GameObject hlObj = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
                    hlObj.transform.SetParent(cellObj.transform, false);
                    SetRect(hlObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image hlImg = hlObj.GetComponent<Image>();
                    hlImg.sprite = jellyTileSprite;
                    hlImg.color = new Color(1f, 1f, 1f, 0.5f);
                    hlObj.SetActive(false);

                    BlockCellUI cellUI = cellObj.AddComponent<BlockCellUI>();
                    cellUI.SetupComponents(bgCellImg, fillImg, bIconImg, hlObj);
                    cellUI.Init(r, c);

                    gridMgr.RegisterCell(r, c, cellUI);
                }
            }

            // Combo Floating Text
            GameObject comboObj = CreateText(boardContainer.transform, "ComboPopup", "💖 COMBO x2!", 72, TextAnchor.MiddleCenter, cuteFont, new Color(1f, 0.55f, 0.7f));
            SetRect(comboObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 240));
            comboObj.SetActive(false);

            // --- D. Hand Slots (Bottom Anchor) ---
            GameObject handContainer = new GameObject("HandContainer", typeof(RectTransform), typeof(Image));
            handContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(handContainer, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 240), new Vector2(980, 260));
            Image hBg = handContainer.GetComponent<Image>();
            hBg.sprite = panelSprite;
            hBg.type = Image.Type.Sliced;
            hBg.color = Color.white;

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
            GameObject guideObj = CreateText(canvasObj.transform, "GuideText", "✨ 블록을 쏙! 넣어 라인을 팡팡 터뜨려요 💖", 30, TextAnchor.MiddleCenter, cuteFont, new Color(0.80f, 0.88f, 1f));
            SetRect(guideObj, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(980, 60));

            // --- E. Game Over Modal ---
            GameObject modalObj = new GameObject("GameOverModal", typeof(RectTransform));
            modalObj.transform.SetParent(canvasObj.transform, false);
            SetRect(modalObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image modalBg = modalObj.AddComponent<Image>();
            modalBg.color = new Color(0.06f, 0.07f, 0.12f, 0.92f);

            GameObject dialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            dialog.transform.SetParent(modalObj.transform, false);
            SetRect(dialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(680, 740));
            Image dImg = dialog.GetComponent<Image>();
            dImg.sprite = panelSprite;
            dImg.type = Image.Type.Sliced;
            dImg.color = Color.white;

            GameObject overTitle = CreateText(dialog.transform, "Title", "NO MORE MOVES", 56, TextAnchor.MiddleCenter, cuteFont, new Color(1f, 0.45f, 0.55f));
            SetRect(overTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(600, 80));

            GameObject overDesc = CreateText(dialog.transform, "Desc", "더 이상 넣을 수 있는 자리가 없어요!", 30, TextAnchor.MiddleCenter, cuteFont, new Color(0.75f, 0.8f, 0.95f));
            SetRect(overDesc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -165), new Vector2(600, 50));

            // Score Box in Modal
            GameObject modalScoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            modalScoreBox.transform.SetParent(dialog.transform, false);
            SetRect(modalScoreBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(560, 220));
            Image mScoreImg = modalScoreBox.GetComponent<Image>();
            mScoreImg.sprite = scoreBoxSprite;
            mScoreImg.type = Image.Type.Sliced;
            mScoreImg.color = Color.white;

            GameObject finalLabel = CreateText(modalScoreBox.transform, "FLabel", "FINAL SCORE", 30, TextAnchor.MiddleCenter, cuteFont, new Color(0.7f, 0.85f, 1f));
            SetRect(finalLabel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(500, 44));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 76, TextAnchor.MiddleCenter, cuteFont, new Color(1f, 0.82f, 0.32f));
            SetRect(finalVal, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -105), new Vector2(500, 75));

            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", "BEST: 0", 32, TextAnchor.MiddleCenter, cuteFont, new Color(0.9f, 0.92f, 1f));
            SetRect(mBestVal, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(500, 44));

            GameObject restartBtn = CreateButton(dialog.transform, "BtnRestart", "한 번 더 하기 ✨", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(560, 95), btnPinkSprite, 36);

            // Bind UI References
            uiMgr.SetupReferences(
                scoreVal.GetComponent<Text>(),
                bestVal.GetComponent<Text>(),
                fFillImg,
                btnSkip,
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

            Debug.Log("<color=#FF7AA2><b>[Block Blast]</b> 10종 이상의 젤리 그래픽 에셋, 주아체 고해상도 폰트가 적용된 프리미엄 씬이 완성되었습니다!</color>");
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Font font, Vector2 anchor, Vector2 pos, Vector2 size, Sprite btnSprite, int fontSize = 30)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            
            Image img = btnObj.GetComponent<Image>();
            img.sprite = btnSprite;
            img.type = Image.Type.Sliced;
            img.color = Color.white;

            SetRect(btnObj, anchor, anchor, pos, size);

            GameObject textObj = CreateText(btnObj.transform, "Text", label, fontSize, TextAnchor.MiddleCenter, font, Color.white);
            SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().raycastTarget = false;

            return btnObj;
        }

        private static GameObject CreateImage(Transform parent, string name, Sprite sprite, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            Image img = obj.GetComponent<Image>();
            img.sprite = sprite;
            img.color = Color.white;
            img.raycastTarget = false;
            SetRect(obj, anchor, anchor, pos, size);
            return obj;
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
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;

            // Cute soft shadow for text readability and pop
            Shadow shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.04f, 0.05f, 0.12f, 0.85f);
            shadow.effectDistance = new Vector2(2.5f, -3f);

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
