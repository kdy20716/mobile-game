#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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

            // Force reimport Jua font with high-res settings
            AssetDatabase.ImportAsset("Assets/Fonts/Jua-Regular.ttf", ImportAssetOptions.ForceUpdate);

            // 2. Import NanoBanana Images & Generate Adorable Glossy Jelly Sprites & UI Textures
            CuteBlockTextureGenerator.ImportAndApplyNanoBananaImages();
            CuteBlockTextureGenerator.ConfigureFontSettings();

            // High-Quality Complete Pastel Block Sprites with 6 Distinct Mascots Embedded!
            Sprite pinkBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Pink_Mascot", new Color(1f, 0.48f, 0.65f), "Jelly_Mascot_Smile.png");
            Sprite mintBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Mint_Mascot", new Color(0.31f, 0.88f, 0.71f), "Jelly_Mascot_Mint.png");
            Sprite goldBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Gold_Mascot", new Color(1f, 0.75f, 0.26f), "Jelly_Mascot_Gold.png");
            Sprite purpleBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Purple_Mascot", new Color(0.65f, 0.49f, 1f), "Jelly_Mascot_Purple.png");
            Sprite blueBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Blue_Mascot", new Color(0.36f, 0.77f, 1f), "Jelly_Mascot_Blue.png");
            Sprite starBombBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Star_Bomb", new Color(1f, 0.30f, 0.41f), "Jelly_Mascot_Red.png");
            Sprite emptyCellSprite = CuteBlockTextureGenerator.GetOrCreateEmptyCellSprite();

            Sprite bgSprite = CuteBlockTextureGenerator.GetOrCreateBackgroundSprite();
            Sprite panelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Panel_Rounded", new Color(0.98f, 0.96f, 1f, 0.88f), new Color(0.85f, 0.80f, 1f, 0.95f));
            Sprite boardPanelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Board_Panel", new Color(0.20f, 0.16f, 0.35f, 0.92f), new Color(0.55f, 0.45f, 0.85f, 0.95f));
            Sprite scoreBoxSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Score_Box", new Color(0.98f, 0.95f, 1f, 0.92f), new Color(0.85f, 0.75f, 1f, 0.95f));
            Sprite bestBoxSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Best_Box", new Color(1f, 0.98f, 0.92f, 0.92f), new Color(1f, 0.85f, 0.45f, 0.95f));
            Sprite skillsBarSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Skills_Bar", new Color(0.98f, 0.96f, 1f, 0.90f), new Color(0.88f, 0.82f, 1f, 0.95f));
            Sprite btnPinkSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Pink", new Color(0.98f, 0.36f, 0.58f));
            Sprite btnTealSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Teal", new Color(0.18f, 0.78f, 0.62f));
            Sprite btnGoldSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Gold", new Color(1f, 0.78f, 0.22f));

            // Cute Character & Icon Sprites
            Sprite mascotSprite = CuteBlockTextureGenerator.GetOrCreateMascotSprite();
            Sprite mascotMintSprite = CuteBlockTextureGenerator.GetOrCreateMintMascotSprite();
            Sprite crownSprite = CuteBlockTextureGenerator.GetOrCreateCrownSprite();
            Sprite flameSprite = CuteBlockTextureGenerator.GetOrCreateFlameSprite();
            Sprite diceSprite = CuteBlockTextureGenerator.GetOrCreateDiceSprite();
            Sprite arrowSprite = CuteBlockTextureGenerator.GetOrCreateRotateArrowSprite();

            // Vector-Sharp TextMeshPro SDF Font Asset (Jua-Regular SDF)
            TMP_FontAsset cuteFont = CuteBlockTextureGenerator.GetOrCreateJuaFontAsset();

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
            scaler.matchWidthOrHeight = 0.0f; // Match Width ensures board never clips on any device ratio
            scaler.dynamicPixelsPerUnit = 1.0f; // Crisp, sharp 1:1 text rendering without atlas overflow!
            scaler.referencePixelsPerUnit = 100f;

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
            BindAudioClips(audioMgr);
            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetupPastelBlockSprites(pinkBlockSprite, mintBlockSprite, goldBlockSprite, purpleBlockSprite, blueBlockSprite, starBombBlockSprite);

            // ==========================================
            // CUTE PASTEL UI HIERARCHY (RESPONSIVE)
            // ==========================================

            // --- In-Game Root Container (Starts inactive so it NEVER shows during main menu or lobby!) ---
            GameObject inGameRoot = new GameObject("InGameRoot", typeof(RectTransform));
            inGameRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(inGameRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            inGameRoot.SetActive(false);

            // --- A. Header (Top Anchor) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -115), new Vector2(-60, 150));

            // Current Score (Left)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(130, 0), new Vector2(260, 130));
            Image sBoxImg = scoreBox.GetComponent<Image>();
            sBoxImg.sprite = scoreBoxSprite;
            sBoxImg.type = Image.Type.Sliced;

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.38f, 0.22f, 0.62f));
            SetRect(scoreLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 26), new Vector2(240, 32));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 58, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.40f, 0.02f)); // Warm Golden Honey
            SetRect(scoreVal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -18), new Vector2(240, 64));

            // Center Title (3D Jelly Mallang Logo Banner - 2x Prominent Scale)
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform), typeof(Image));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(640, 220));
            Image titleLogoImg = titleBox.GetComponent<Image>();
            titleLogoImg.sprite = CuteBlockTextureGenerator.GetOrCreateMallangBlastLogoSprite();
            titleLogoImg.preserveAspect = true;

            // Best Score (Right) - Crown removed as requested
            GameObject bestBox = new GameObject("BestBox", typeof(RectTransform), typeof(Image));
            bestBox.transform.SetParent(headerObj.transform, false);
            SetRect(bestBox, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-130, 0), new Vector2(260, 130));
            Image bBoxImg = bestBox.GetComponent<Image>();
            bBoxImg.sprite = bestBoxSprite;
            bBoxImg.type = Image.Type.Sliced;

            GameObject bestLabel = CreateText(bestBox.transform, "Label", "BEST", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.32f, 0.05f));
            SetRect(bestLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 26), new Vector2(240, 32));

            GameObject bestVal = CreateText(bestBox.transform, "Value", "0", 58, TextAlignmentOptions.Center, cuteFont, new Color(0.88f, 0.30f, 0.02f));
            SetRect(bestVal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -18), new Vector2(240, 64));

            // --- B. Skills Bar (Below Header - Shifted Down) ---
            GameObject skillsBar = new GameObject("SkillsBar", typeof(RectTransform), typeof(Image));
            skillsBar.transform.SetParent(inGameRoot.transform, false);
            SetRect(skillsBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -320), new Vector2(980, 92));
            Image sBarBg = skillsBar.GetComponent<Image>();
            sBarBg.sprite = skillsBarSprite;
            sBarBg.type = Image.Type.Sliced;
            sBarBg.color = Color.white;

            // Time Limit Gauge Bar (Dynamic Turn Timer)
            GameObject timeLabel = CreateText(skillsBar.transform, "TimeLabel", "TIME", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.20f, 0.65f, 0.55f));
            SetRect(timeLabel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(55, 0), new Vector2(80, 44));

            GameObject timeBg = new GameObject("TimeBg", typeof(RectTransform), typeof(Image));
            timeBg.transform.SetParent(skillsBar.transform, false);
            SetRect(timeBg, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(235, 0), new Vector2(210, 32));
            Image tBgImg = timeBg.GetComponent<Image>();
            tBgImg.sprite = scoreBoxSprite;
            tBgImg.type = Image.Type.Sliced;
            tBgImg.color = new Color(0.88f, 0.96f, 0.94f); // Crisp soft mint backing

            Sprite timeGradientSprite = CuteBlockTextureGenerator.GetOrCreateTimeBarGradientSprite();
            GameObject timeFill = new GameObject("TimeFill", typeof(RectTransform), typeof(Image));
            timeFill.transform.SetParent(timeBg.transform, false);
            SetRect(timeFill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image tFillImg = timeFill.GetComponent<Image>();
            tFillImg.sprite = timeGradientSprite;
            tFillImg.type = Image.Type.Filled;
            tFillImg.fillMethod = Image.FillMethod.Horizontal;
            tFillImg.fillAmount = 1.0f;
            tFillImg.color = Color.white;

            GameObject timeTextObj = CreateText(skillsBar.transform, "TimeText", "03:00", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.38f, 0.22f, 0.62f));
            SetRect(timeTextObj, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(395, 0), new Vector2(110, 40));
            TMP_Text timeRemainingTMP = timeTextObj.GetComponent<TMP_Text>();

            // Fullscreen Danger Vignette (Pulses in deep red when remaining time <= 3s)
            Sprite vignetteSprite = CuteBlockTextureGenerator.GetOrCreateVignetteSprite();
            GameObject vignetteObj = CreateImage(inGameRoot.transform, "VignetteDangerOverlay", vignetteSprite, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image vignetteImg = vignetteObj.GetComponent<Image>();
            vignetteImg.raycastTarget = false; // Never blocks clicks/touches!
            vignetteObj.SetActive(false);

            // 🎲 Skip Button (User-Uploaded 2.5D Marshmallow Jelly Button)
            Sprite btnSkipUser = CuteBlockTextureGenerator.GetOrCreateUserSkipButtonSprite();
            GameObject skipBtnObj = CreateButton(skillsBar.transform, "BtnSkip", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-215, 0), new Vector2(96, 96), btnSkipUser, 32);
            skipBtnObj.AddComponent<CanvasGroup>(); // For dimming/brightening skip button
            var btnSkip = skipBtnObj.GetComponent<Button>();

            // 🔄 Spin Button (User-Uploaded 2.5D Marshmallow Jelly Button)
            Sprite btnSpinUser = CuteBlockTextureGenerator.GetOrCreateUserSpinButtonSprite();
            GameObject rotateBtnObj = CreateButton(skillsBar.transform, "BtnRotate", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-95, 0), new Vector2(96, 96), btnSpinUser, 32);
            var btnRotate = rotateBtnObj.GetComponent<Button>();

            // --- C. Board Container (Center Anchor - Shifted Down) ---
            GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform), typeof(Image));
            boardContainer.transform.SetParent(inGameRoot.transform, false);
            SetRect(boardContainer, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(980, 980));
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
            EditorUtility.SetDirty(gridMgr);

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
                    bgCellImg.sprite = emptyCellSprite;
                    bgCellImg.color = Color.white;

                    // Fill Child (Glossy Pastel Jelly Block)
                    GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                    fillObj.transform.SetParent(cellObj.transform, false);
                    SetRect(fillObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image fillImg = fillObj.GetComponent<Image>();
                    fillImg.sprite = pinkBlockSprite;
                    fillObj.SetActive(false);

                    // Cute Face Icon Child inside Cell (Expanded to fit cell boundaries snugly)
                    GameObject faceObj = new GameObject("FaceIcon", typeof(RectTransform), typeof(Image));
                    faceObj.transform.SetParent(cellObj.transform, false);
                    SetRect(faceObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(cellSize * 0.98f, cellSize * 0.98f));
                    Image faceImg = faceObj.GetComponent<Image>();
                    faceImg.color = Color.white;
                    faceImg.raycastTarget = false;
                    faceObj.SetActive(false);

                    // Bomb Icon Child (Star Bomb)
                    GameObject bIconObj = new GameObject("BombIcon", typeof(RectTransform), typeof(Image));
                    bIconObj.transform.SetParent(cellObj.transform, false);
                    SetRect(bIconObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(85, 85));
                    Image bIconImg = bIconObj.GetComponent<Image>();
                    bIconImg.sprite = starBombBlockSprite;
                    bIconObj.SetActive(false);

                    // Highlight Child (Cute Glowing Jelly Preview with Face)
                    GameObject hlObj = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
                    hlObj.transform.SetParent(cellObj.transform, false);
                    SetRect(hlObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image hlImg = hlObj.GetComponent<Image>();
                    hlImg.sprite = pinkBlockSprite;
                    hlImg.color = new Color(1f, 1f, 1f, 0.65f);

                    GameObject hlFace = new GameObject("FaceHighlight", typeof(RectTransform), typeof(Image));
                    hlFace.transform.SetParent(hlObj.transform, false);
                    SetRect(hlFace, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(cellSize * 0.98f, cellSize * 0.98f));
                    Image hlFaceImg = hlFace.GetComponent<Image>();
                    hlFaceImg.color = new Color(1f, 1f, 1f, 0.75f);
                    hlFaceImg.raycastTarget = false;
                    hlFace.SetActive(false);

                    hlObj.SetActive(false);

                    BlockCellUI cellUI = cellObj.AddComponent<BlockCellUI>();
                    cellUI.SetupComponents(bgCellImg, fillImg, bIconImg, hlObj, faceImg);
                    cellUI.Init(r, c);
                    EditorUtility.SetDirty(cellUI);

                    gridMgr.RegisterCell(r, c, cellUI);
                }
            }

            // Combo Floating Text
            GameObject comboObj = CreateText(boardContainer.transform, "ComboPopup", "COMBO x2!", 72, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.55f, 0.7f));
            SetRect(comboObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 240));
            comboObj.SetActive(false);

            // --- D. Hand Slots (Bottom Anchor) ---
            GameObject handContainer = new GameObject("HandContainer", typeof(RectTransform), typeof(Image));
            handContainer.transform.SetParent(inGameRoot.transform, false);
            SetRect(handContainer, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 285), new Vector2(980, 260));
            Image hBg = handContainer.GetComponent<Image>();
            hBg.sprite = panelSprite;
            hBg.type = Image.Type.Sliced;
            hBg.color = new Color(1f, 1f, 1f, 0f); // Completely transparent dock! No giant dark pill!
            hBg.raycastTarget = false; // Never blocks block clicks/drags

            Transform[] slotTransforms = new Transform[3];
            float slotSpacing = 310f;
            for (int i = 0; i < 3; i++)
            {
                // Slot Background Card to clearly separate blocks from the scene background
                GameObject slotCard = new GameObject($"SlotCard_{i}", typeof(RectTransform), typeof(Image));
                slotCard.transform.SetParent(handContainer.transform, false);
                SetRect(slotCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(290, 230));
                Image scImg = slotCard.GetComponent<Image>();
                scImg.sprite = scoreBoxSprite;
                scImg.type = Image.Type.Sliced;
                scImg.color = new Color(1f, 1f, 1f, 0.92f); // Crisp glossy white pastel card
                scImg.raycastTarget = false;

                GameObject slot = new GameObject($"Slot_{i}", typeof(RectTransform));
                slot.transform.SetParent(handContainer.transform, false);
                SetRect(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(280, 240));
                slotTransforms[i] = slot.transform;
            }

            spawner.SetupReferences(slotTransforms, canvas, pinkBlockSprite, starBombBlockSprite);

            // Bottom Guide Text (1.5x larger, 7 tips 5-second rolling)
            GameObject guidePill = new GameObject("GuidePill", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            guidePill.transform.SetParent(inGameRoot.transform, false);
            SetRect(guidePill, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 85), new Vector2(880, 64));
            Image gpImg = guidePill.GetComponent<Image>();
            gpImg.sprite = scoreBoxSprite;
            gpImg.type = Image.Type.Sliced;
            gpImg.color = new Color(0.12f, 0.10f, 0.25f, 0.72f);
            gpImg.raycastTarget = false;

            GameObject guideObj = CreateText(guidePill.transform, "GuideText", "돌리기 버튼을 누르면 블록을 회전시킬 수 있어요!", 36, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.96f, 0.85f));
            SetRect(guideObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            uiMgr.SetupGuideTip(guideObj.GetComponent<TextMeshProUGUI>(), guidePill.GetComponent<CanvasGroup>());

            // --- E. Game Over Modal Dialog ---
            GameObject modalObj = new GameObject("GameOverModal", typeof(RectTransform), typeof(Image));
            modalObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(modalObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image modalOverlay = modalObj.GetComponent<Image>();
            modalOverlay.color = new Color(0.05f, 0.06f, 0.12f, 0.85f);
            modalObj.SetActive(false);

            GameObject dialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            dialog.transform.SetParent(modalObj.transform, false);
            SetRect(dialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 950));
            Image dImg = dialog.GetComponent<Image>();
            dImg.sprite = panelSprite;
            dImg.type = Image.Type.Sliced;
            dImg.color = Color.white;

            GameObject overTitle = CreateText(dialog.transform, "Title", "NO MORE MOVES", 64, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.5f, 0.65f));
            SetRect(overTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(700, 80));

            GameObject overSub = CreateText(dialog.transform, "Subtitle", "더 이상 블록을 놓을 자리가 없어요!", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.75f, 0.8f, 0.95f));
            SetRect(overSub, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -165), new Vector2(700, 45));

            GameObject modalScoreBox = new GameObject("ScoreCard", typeof(RectTransform), typeof(Image));
            modalScoreBox.transform.SetParent(dialog.transform, false);
            SetRect(modalScoreBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 15), new Vector2(640, 320));
            Image mScoreImg = modalScoreBox.GetComponent<Image>();
            mScoreImg.sprite = scoreBoxSprite;
            mScoreImg.type = Image.Type.Sliced;
            mScoreImg.color = Color.white;

            GameObject finalLabel = CreateText(modalScoreBox.transform, "FLabel", "FINAL SCORE", 30, TextAlignmentOptions.Center, cuteFont, new Color(0.7f, 0.85f, 1f));
            SetRect(finalLabel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(500, 44));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 76, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.82f, 0.32f));
            SetRect(finalVal, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -105), new Vector2(500, 75));

            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", "BEST: 0", 32, TextAlignmentOptions.Center, cuteFont, new Color(0.9f, 0.92f, 1f));
            SetRect(mBestVal, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(500, 44));

            GameObject restartBtn = CreateButton(dialog.transform, "BtnRestart", "한 번 더 하기!", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(560, 95), btnPinkSprite, 36);

            // Bind UI References
            uiMgr.SetupReferences(
                scoreVal.GetComponent<TMP_Text>(),
                bestVal.GetComponent<TMP_Text>(),
                tFillImg,
                timeRemainingTMP,
                vignetteImg,
                btnSkip,
                null,
                btnRotate,
                comboObj.GetComponent<TMP_Text>(),
                boardContainer.GetComponent<RectTransform>(),
                modalObj,
                finalVal.GetComponent<TMP_Text>(),
                mBestVal.GetComponent<TMP_Text>(),
                restartBtn.GetComponent<Button>(),
                inGameRoot
            );

            // --- F. Fullscreen Cinematic Main Menu Screen ---
            GameObject mainMenu = new GameObject("MainMenuScreen", typeof(RectTransform), typeof(CanvasGroup), typeof(MainMenuCinematicController));
            mainMenu.transform.SetParent(canvasObj.transform, false);
            SetRect(mainMenu, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fullscreen invisible touch catcher so clicking anywhere triggers game start
            GameObject touchCatcher = new GameObject("TouchCatcher", typeof(RectTransform), typeof(Image));
            touchCatcher.transform.SetParent(mainMenu.transform, false);
            SetRect(touchCatcher, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image touchCatcherImg = touchCatcher.GetComponent<Image>();
            touchCatcherImg.color = Color.clear;
            touchCatcherImg.raycastTarget = true;

            // Cinematic Viewport Root (Handles Camera Zoom In, Pan, and Zoom Out)
            GameObject cinematicRootObj = new GameObject("CinematicRoot", typeof(RectTransform));
            cinematicRootObj.transform.SetParent(mainMenu.transform, false);
            SetRect(cinematicRootObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform cinematicRootRT = cinematicRootObj.GetComponent<RectTransform>();

            // 1:1 Square Seamless Expanded Dreamy Background (2160 x 2160, never compressed!)
            Sprite wideBgSprite = CuteBlockTextureGenerator.GetOrCreateMainMenuWideBackgroundSprite();
            GameObject menuBgObj = new GameObject("MenuBackground", typeof(RectTransform), typeof(Image));
            menuBgObj.transform.SetParent(cinematicRootObj.transform, false);
            SetRect(menuBgObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(2160, 2160));
            Image menuBgImg = menuBgObj.GetComponent<Image>();
            menuBgImg.sprite = wideBgSprite;
            menuBgImg.color = Color.white;
            menuBgImg.raycastTarget = false;

            // Fireworks Particle FX Container (Screen space overlay on mainMenu)
            GameObject fireworksObj = new GameObject("FireworksFX", typeof(RectTransform), typeof(JellyFireworksEffect));
            fireworksObj.transform.SetParent(mainMenu.transform, false);
            SetRect(fireworksObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            JellyFireworksEffect fireworksFX = fireworksObj.GetComponent<JellyFireworksEffect>();
            Sprite glowOrb = CuteBlockTextureGenerator.GetOrCreateFireworksGlowOrbSprite();
            Sprite sparkleStar = CuteBlockTextureGenerator.GetOrCreateFireworksSparkleStarSprite();
            Sprite shockwaveRing = CuteBlockTextureGenerator.GetOrCreateFireworksShockwaveRingSprite();
            fireworksFX.SetupSprites(glowOrb, sparkleStar, shockwaveRing);
            EditorUtility.SetDirty(fireworksFX);

            // Cute 3D Gummy Jelly Title Logo ("말랑블라스트")
            Sprite logoSprite = CuteBlockTextureGenerator.GetOrCreateMallangBlastLogoSprite();
            GameObject logoObj = new GameObject("LogoBanner", typeof(RectTransform), typeof(Image));
            logoObj.transform.SetParent(cinematicRootObj.transform, false);
            SetRect(logoObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 240), new Vector2(760, 480));
            Image logoImg = logoObj.GetComponent<Image>();
            logoImg.sprite = logoSprite;
            logoImg.preserveAspect = true;

            // Left Popping Mascot (Strawberry Smile) - Clean transparent cutout without black box
            Sprite leftMascotSprite = CuteBlockTextureGenerator.GetOrCreateLeftPoppingMascotSprite();
            GameObject leftMascotObj = CreateImage(cinematicRootObj.transform, "LeftMascot", leftMascotSprite, new Vector2(0.5f, 0.5f), new Vector2(-260, -80), new Vector2(300, 300));
            RectTransform leftMascotRT = leftMascotObj.GetComponent<RectTransform>();

            // Right Popping Mascot (Mint Soda) - Clean transparent cutout without black box
            Sprite rightMascotSprite = CuteBlockTextureGenerator.GetOrCreateRightPoppingMascotSprite();
            GameObject rightMascotObj = CreateImage(cinematicRootObj.transform, "RightMascot", rightMascotSprite, new Vector2(0.5f, 0.5f), new Vector2(260, -80), new Vector2(300, 300));
            RectTransform rightMascotRT = rightMascotObj.GetComponent<RectTransform>();

            // Blinking "화면을 터치해주세요" Prompt at Bottom
            GameObject touchPromptObj = new GameObject("TouchPromptGroup", typeof(RectTransform), typeof(CanvasGroup));
            touchPromptObj.transform.SetParent(mainMenu.transform, false);
            SetRect(touchPromptObj, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 140), new Vector2(650, 80));
            CanvasGroup touchPromptCG = touchPromptObj.GetComponent<CanvasGroup>();

            GameObject touchText = CreateText(touchPromptObj.transform, "TouchText", "화면을 터치해주세요", 40, TextAlignmentOptions.Center, cuteFont, new Color(1f, 1f, 1f, 0.95f));
            SetRect(touchText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Hook up MainMenuCinematicController & UIManager
            MainMenuCinematicController cinematicCtrl = mainMenu.GetComponent<MainMenuCinematicController>();
            cinematicCtrl.SetupReferences(
                cam,
                cinematicRootRT,
                leftMascotRT,
                rightMascotRT,
                logoObj.GetComponent<RectTransform>(),
                touchPromptCG,
                touchText.GetComponent<TMP_Text>(),
                null,
                fireworksFX,
                mainMenu.GetComponent<CanvasGroup>(),
                uiMgr
            );

            uiMgr.SetupMainMenu(mainMenu, null, null);

            // --- G. Cute Mallang Party Lobby Screen ---
            CuteBlockTextureGenerator.EnsureMascotsCutout();

            Sprite circleFrameSprite = CuteBlockTextureGenerator.GetOrCreateCircleFrameSprite();
            Sprite tabPillSprite = CuteBlockTextureGenerator.GetOrCreateTabPillSprite();
            Sprite cuteCardSprite = CuteBlockTextureGenerator.GetOrCreateCuteCardSprite();

            Sprite pinkMascotSprite = CuteBlockTextureGenerator.GetOrCreateLeftPoppingMascotSprite();
            Sprite mintMascotSprite = CuteBlockTextureGenerator.GetOrCreateRightPoppingMascotSprite();
            Sprite goldMascotSprite = CuteBlockTextureGenerator.GetOrCreateGoldMascotSprite();
            Sprite purpleMascotSprite = CuteBlockTextureGenerator.GetOrCreatePurpleMascotSprite();

            Sprite[] mascotAvatars = new Sprite[] { pinkMascotSprite, mintMascotSprite, goldMascotSprite, purpleMascotSprite };

            GameObject lobbyRoot = new GameObject("LobbyScreen", typeof(RectTransform), typeof(CanvasGroup));
            lobbyRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(lobbyRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            CanvasGroup lobbyCG = lobbyRoot.GetComponent<CanvasGroup>();
            lobbyCG.alpha = 0f;
            lobbyRoot.SetActive(false);

            var lobbyMgr = canvasObj.AddComponent<LobbyManager>();

            // Lobby Background (Fairy-tale 3D character select stage without characters)
            Sprite lobbyBgSprite = CuteBlockTextureGenerator.GetOrCreateLobbyStageBackgroundSprite();
            GameObject lobbyBg = CreateImage(lobbyRoot.transform, "LobbyBg", lobbyBgSprite, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // --- Top Bar: Left Logo (1.5x Larger & Shifted Left) & Right Profile/Coins ---
            // 1.5x Enriched Mallang Blast Logo placed prominently at Top-Left
            GameObject lobbyLogoObj = CreateImage(lobbyRoot.transform, "LobbyLogo", logoSprite, new Vector2(0, 1), new Vector2(0, 1), new Vector2(240, -145), new Vector2(900, 310));
            lobbyLogoObj.GetComponent<Image>().preserveAspect = true;

            // Top-Right: Polished Currency Capsule (Coin icon + 1,000 text in elegant translucent pill)
            Sprite goldCoinSprite = CuteBlockTextureGenerator.GetOrCreateGoldCoinSprite();
            GameObject coinBadge = CreateImage(lobbyRoot.transform, "CoinBadge", tabPillSprite, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-195, -75), new Vector2(195, 56));
            Image coinBgImg = coinBadge.GetComponent<Image>();
            coinBgImg.color = new Color(0.18f, 0.12f, 0.28f, 0.65f); // Translucent dark glass

            GameObject coinIconObj = CreateImage(coinBadge.transform, "CoinIcon", goldCoinSprite, new Vector2(0, 0.5f), new Vector2(28, 0), new Vector2(38, 38));
            coinIconObj.GetComponent<Image>().preserveAspect = true;

            GameObject coinLabel = CreateText(coinBadge.transform, "Coins", "1,000", 28, TextAlignmentOptions.Left, cuteFont, new Color(1f, 0.88f, 0.40f));
            SetRect(coinLabel, new Vector2(0, 0), new Vector2(1, 1), new Vector2(54, 0), new Vector2(-10, 0));

            // Top-Right: Profile Circle Button
            GameObject profileBtnObj = new GameObject("ProfileButton", typeof(RectTransform), typeof(Image), typeof(Button));
            profileBtnObj.transform.SetParent(lobbyRoot.transform, false);
            SetRect(profileBtnObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-60, -75), new Vector2(84, 84));
            Image pBtnImg = profileBtnObj.GetComponent<Image>();
            pBtnImg.sprite = circleFrameSprite;
            Button pBtn = profileBtnObj.GetComponent<Button>();

            GameObject pAvatarObj = CreateImage(profileBtnObj.transform, "AvatarIcon", pinkMascotSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(70, 70));
            Image pAvatarImg = pAvatarObj.GetComponent<Image>();
            pAvatarImg.preserveAspect = true;

            // --- Center: Party Stage with 4 Mascots, Glowing Auras & Floating Labels ---
            GameObject partyStage = new GameObject("PartyStage", typeof(RectTransform));
            partyStage.transform.SetParent(lobbyRoot.transform, false);
            SetRect(partyStage, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(1000, 750));

            // 4 Party Mascots: Pink (-315), Mint (-105), Gold (105), Purple (315)
            Vector2[] mascotPositions = new Vector2[]
            {
                new Vector2(-315, -60),
                new Vector2(-105, 10),
                new Vector2(105, 10),
                new Vector2(315, -60)
            };
            Vector2[] mascotSizes = new Vector2[]
            {
                new Vector2(235, 235),
                new Vector2(245, 245),
                new Vector2(245, 245),
                new Vector2(235, 235)
            };
            string[] floatingLabels = new string[] { "게임 시작", "상점", "설정", "도움말" };
            Color[] labelColors = new Color[]
            {
                new Color(1f, 0.33f, 0.53f),
                new Color(0.17f, 0.83f, 0.64f),
                new Color(1f, 0.70f, 0.0f),
                new Color(0.66f, 0.33f, 0.97f)
            };

            Sprite glowOrbSprite = CuteBlockTextureGenerator.GetOrCreateFireworksGlowOrbSprite();
            RectTransform[] partyMascotRTs = new RectTransform[4];
            GameObject[] partyLabelObjs = new GameObject[4];
            Image[] partyLabelBgImgs = new Image[4];
            TMP_Text[] partyLabelTMPs = new TMP_Text[4];
            GameObject[] partyGlowObjs = new GameObject[4];

            for (int i = 0; i < 4; i++)
            {
                // Slot Root Container (Anchored at mascotPositions[i])
                // Allows whole slot (glow aura behind + mascot + floating label) to bounce & scale together!
                GameObject mSlot = new GameObject($"MascotSlot_{i}", typeof(RectTransform));
                mSlot.transform.SetParent(partyStage.transform, false);
                SetRect(mSlot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), mascotPositions[i], mascotSizes[i]);
                partyMascotRTs[i] = mSlot.GetComponent<RectTransform>();

                // --- 1. Glow Aura (Child index 0: Renders FIRST, strictly BEHIND the mascot image!) ---
                // 1.5x Brighter & Larger Magical Radiance Bloom (520px outer bloom + 380px intense core)
                GameObject glowObj = new GameObject("GlowAura", typeof(RectTransform));
                glowObj.transform.SetParent(mSlot.transform, false);
                SetRect(glowObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520, 520));

                // Outer blooming aura
                GameObject glowOuter = CreateImage(glowObj.transform, "GlowOuter", glowOrbSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520, 520));
                glowOuter.GetComponent<Image>().raycastTarget = false;
                Image gOuterImg = glowOuter.GetComponent<Image>();
                gOuterImg.color = new Color(labelColors[i].r, labelColors[i].g, labelColors[i].b, 0.90f);

                // Inner bright intense core (1.5x brightness boost)
                GameObject glowInner = CreateImage(glowObj.transform, "GlowInner", glowOrbSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 380));
                glowInner.GetComponent<Image>().raycastTarget = false;
                Image gInnerImg = glowInner.GetComponent<Image>();
                Color brightCoreCol = Color.Lerp(labelColors[i], Color.white, 0.40f);
                gInnerImg.color = new Color(brightCoreCol.r, brightCoreCol.g, brightCoreCol.b, 1.0f);

                glowObj.SetActive(i == 0); // 0 (Pink) active by default
                partyGlowObjs[i] = glowObj;

                // --- 2. Mascot Character (Child index 1: Renders AFTER GlowAura, strictly IN FRONT of the glow!) ---
                GameObject mObj = CreateImage(mSlot.transform, $"Mascot_{i}", mascotAvatars[i], new Vector2(0.5f, 0.5f), Vector2.zero, mascotSizes[i]);
                mObj.GetComponent<Image>().preserveAspect = true;
                mObj.GetComponent<Image>().raycastTarget = true;

                // --- 3. Floating Menu Label (Child index 2: Above the mascot's head) ---
                GameObject pillObj = CreateImage(mSlot.transform, "FloatingLabel", tabPillSprite, new Vector2(0.5f, 1f), new Vector2(0, 32), new Vector2(165, 52));
                pillObj.GetComponent<Image>().raycastTarget = true;
                Image pillBg = pillObj.GetComponent<Image>();
                pillBg.color = (i == 0) ? Color.white : new Color(1f, 1f, 1f, 0.75f);
                partyLabelObjs[i] = pillObj;
                partyLabelBgImgs[i] = pillBg;

                GameObject pillTxtObj = CreateText(pillObj.transform, "LabelText", floatingLabels[i], 26, TextAlignmentOptions.Center, cuteFont, labelColors[i]);
                SetRect(pillTxtObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                partyLabelTMPs[i] = pillTxtObj.GetComponent<TMP_Text>();

                // Attach Buttons to slot, mascot, and label for responsive touch/click
                Button slotBtn = mSlot.AddComponent<Button>();
                slotBtn.transition = Selectable.Transition.None;
                Button mBtn = mObj.AddComponent<Button>();
                mBtn.transition = Selectable.Transition.None;
                Button pBtn2 = pillObj.AddComponent<Button>();
                pBtn2.transition = Selectable.Transition.None;
            }

            // Party Stage Tip Text (No square emojis!)
            GameObject partyTip = CreateText(partyStage.transform, "PartyTip", "말랑이들을 톡톡 눌러보세요!", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.42f, 0.30f, 0.55f, 0.95f));
            SetRect(partyTip, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 115), new Vector2(600, 45));

            // Dynamic Bottom Action Button
            GameObject playBtnObj = CreateButton(lobbyRoot.transform, "BtnBottomAction", "게임 시작!", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 165), new Vector2(480, 115), btnPinkSprite, 40);
            Button playBtn = playBtnObj.GetComponent<Button>();
            Image playBtnBg = playBtnObj.GetComponent<Image>();
            TMP_Text playBtnText = playBtnObj.GetComponentInChildren<TMP_Text>();

            // --- Profile Modal ---
            GameObject pModal = new GameObject("ProfileModal", typeof(RectTransform));
            pModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(pModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject pDarkBg = CreateImage(pModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            pDarkBg.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
            pDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject pCard = CreateImage(pModal.transform, "DialogCard", cuteCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820, 1060));
            pCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject pCloseBtn = CreateButton(pCard.transform, "BtnClose", "✕", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), btnPinkSprite, 32);

            GameObject pTitle = CreateText(pCard.transform, "Title", "내 프로필", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(pTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            // Big Avatar
            GameObject pBigAvFrame = CreateImage(pCard.transform, "AvFrame", circleFrameSprite, new Vector2(0.5f, 1), new Vector2(0, -180), new Vector2(160, 160));
            GameObject pBigAv = CreateImage(pBigAvFrame.transform, "AvIcon", pinkMascotSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(135, 135));
            Image pBigAvImg = pBigAv.GetComponent<Image>();
            pBigAvImg.preserveAspect = true;

            // Nickname
            GameObject pNick = CreateText(pCard.transform, "Nick", "말랑이#0000", 38, TextAlignmentOptions.Center, cuteFont, new Color(0.25f, 0.15f, 0.45f));
            SetRect(pNick, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -290), new Vector2(500, 48));

            // Stats row (Best score & Coins)
            GameObject pStats = CreateText(pCard.transform, "Stats", "최고 점수: 0점  |  보유 코인: 1,000 C", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.45f, 0.05f));
            SetRect(pStats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -345), new Vector2(650, 40));

            // Bio Section
            GameObject pBioLbl = CreateText(pCard.transform, "BioLbl", "자기소개 (터치하여 수정)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.35f, 0.55f));
            SetRect(pBioLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -405), new Vector2(680, 35));

            GameObject pBioInputObj = CreateInputField(pCard.transform, "BioInput", "말랑블라스트에 오신 걸 환영해요!", "자기소개를 입력해주세요", cuteFont, new Vector2(0, 45), new Vector2(680, 85), tabPillSprite);
            TMP_InputField pBioInput = pBioInputObj.GetComponent<TMP_InputField>();

            // Avatar Picker Section (4 Mascots)
            GameObject pPickLbl = CreateText(pCard.transform, "PickLbl", "프로필 꾸미기 (말랑이 선택)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.35f, 0.55f));
            SetRect(pPickLbl, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -30), new Vector2(680, 35));

            Button[] avPickBtns = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                float xOff = (i - 1.5f) * 140f;
                GameObject avBtnObj = CreateButton(pCard.transform, $"AvBtn_{i}", "", cuteFont, new Vector2(0.5f, 0.5f), new Vector2(xOff, -110), new Vector2(98, 98), circleFrameSprite);
                GameObject iconObj = CreateImage(avBtnObj.transform, "Icon", mascotAvatars[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80, 80));
                iconObj.GetComponent<Image>().preserveAspect = true;
                avPickBtns[i] = avBtnObj.GetComponent<Button>();
            }

            // Logout Button
            GameObject pLogoutObj = CreateButton(pCard.transform, "BtnLogout", "로그아웃", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 95), new Vector2(340, 80), btnPinkSprite, 32);
            Button pLogoutBtn = pLogoutObj.GetComponent<Button>();

            // --- Shop Modal ---
            GameObject sModal = new GameObject("ShopModal", typeof(RectTransform));
            sModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(sModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject sDarkBg = CreateImage(sModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            sDarkBg.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
            sDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject sCard = CreateImage(sModal.transform, "DialogCard", cuteCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 880));
            sCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject sCloseBtn = CreateButton(sCard.transform, "BtnClose", "✕", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), btnPinkSprite, 32);

            GameObject sTitle = CreateText(sCard.transform, "Title", "말랑이 상점", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            GameObject sCoinTxt = CreateText(sCard.transform, "Coins", "내 코인: 1,000 C", 30, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.45f, 0.05f));
            SetRect(sCoinTxt, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -125), new Vector2(500, 40));

            // 3 Sample shop item cards
            string[] itemNames = new string[] { "무지개 젤리 테마", "별빛 폭탄 부스터 x3", "마시멜로 스킨 팩" };
            string[] itemPrices = new string[] { "500 C", "300 C", "800 C" };
            for (int i = 0; i < 3; i++)
            {
                float yPos = -220f - i * 160f;
                GameObject itCard = CreateImage(sCard.transform, $"Item_{i}", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(680, 130));
                itCard.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.7f);

                GameObject itName = CreateText(itCard.transform, "Name", itemNames[i], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(itName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(180, 0), new Vector2(320, 50));

                CreateButton(itCard.transform, "BtnBuy", itemPrices[i], cuteFont, new Vector2(1, 0.5f), new Vector2(-110, 0), new Vector2(170, 65), btnPinkSprite, 26);
            }

            // --- Settings Modal ---
            GameObject setModal = new GameObject("SettingsModal", typeof(RectTransform));
            setModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(setModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject setDarkBg = CreateImage(setModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            setDarkBg.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
            setDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject setCard = CreateImage(setModal.transform, "DialogCard", cuteCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760, 780));
            setCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject setCloseBtn = CreateButton(setCard.transform, "BtnClose", "✕", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), btnPinkSprite, 32);

            GameObject setTitle = CreateText(setCard.transform, "Title", "게임 설정", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(setTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            // BGM Slider
            GameObject bgmLbl = CreateText(setCard.transform, "BGMLbl", "배경음악 (BGM)", 30, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(bgmLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -165), new Vector2(580, 40));
            GameObject bgmSldObj = CreateSlider(setCard.transform, "BGMSlider", new Vector2(0, 140), new Vector2(580, 36), 0.7f);

            // SFX Slider
            GameObject sfxLbl = CreateText(setCard.transform, "SFXLbl", "효과음 (SFX)", 30, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sfxLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -290), new Vector2(580, 40));
            GameObject sfxSldObj = CreateSlider(setCard.transform, "SFXSlider", new Vector2(0, 15), new Vector2(580, 36), 0.85f);

            // Version
            GameObject verTxt = CreateText(setCard.transform, "Version", "말랑블라스트 v1.2.0 (Fairy Party Edition)", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.6f, 0.55f, 0.7f));
            SetRect(verTxt, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(600, 40));

            // --- Help Modal (NEW!) ---
            GameObject hModal = new GameObject("HelpModal", typeof(RectTransform));
            hModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(hModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject hDarkBg = CreateImage(hModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            hDarkBg.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
            hDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject hCard = CreateImage(hModal.transform, "DialogCard", cuteCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 880));
            hCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject hCloseBtn = CreateButton(hCard.transform, "BtnClose", "✕", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), btnPinkSprite, 32);

            GameObject hTitle = CreateText(hCard.transform, "Title", "게임 도움말", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(hTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            string[] helpLines = new string[]
            {
                "1. 블록 드래그: 아래 3개의 젤리 블록을 보드판에 올려놓아요.",
                "2. 줄 완성 폭파: 가로 또는 세로 한 줄을 채우면 팡팡!",
                "3. 달콤한 콤보: 연속으로 블록을 터뜨려 고득점에 도전!",
                "4. 별빛 피버: 게이지를 가득 채워 피버 타임의 폭탄을 즐겨요!"
            };

            for (int i = 0; i < 4; i++)
            {
                float yPos = -160f - i * 140f;
                GameObject hRowCard = CreateImage(hCard.transform, $"HelpRow_{i}", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(700, 115));
                hRowCard.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.85f);

                GameObject hRowTxt = CreateText(hRowCard.transform, "Text", helpLines[i], 24, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(hRowTxt, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(25, 0), new Vector2(-50, 70));
            }

            // Setup LobbyManager References
            lobbyMgr.SetupReferences(
                lobbyRoot, lobbyCG,
                pBtn, pAvatarImg, coinLabel.GetComponent<TMP_Text>(),
                partyMascotRTs, partyLabelObjs, partyLabelBgImgs, partyLabelTMPs,
                playBtn, playBtnBg, playBtnText,
                pModal, pBigAvImg, pNick.GetComponent<TMP_Text>(), pBioInput, pStats.GetComponent<TMP_Text>(), pStats.GetComponent<TMP_Text>(), avPickBtns, pLogoutBtn, pCloseBtn.GetComponent<Button>(),
                sModal, sCoinTxt.GetComponent<TMP_Text>(), sCloseBtn.GetComponent<Button>(),
                setModal, bgmSldObj.GetComponent<Slider>(), sfxSldObj.GetComponent<Slider>(), setCloseBtn.GetComponent<Button>(),
                hModal, hCloseBtn.GetComponent<Button>(),
                mascotAvatars,
                partyGlowObjs
            );

            // In-Game Home Button (in Header, between Score and Best or on Left)
            GameObject homeBtnObj = CreateButton(headerObj.transform, "BtnHome", "로비", cuteFont, new Vector2(0, 0.5f), new Vector2(300, 0), new Vector2(130, 58), btnPinkSprite, 24);
            homeBtnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (LobbyManager.Instance != null) LobbyManager.Instance.ReturnToLobby();
            });

            // 6. Fairy Screen Transition Overlay (Dissolve + 1-Sec Corner Sparkles)
            Sprite fairyRippleSprite = CuteBlockTextureGenerator.GetOrCreateFairyRippleSprite();
            Sprite fairySparkleSprite = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();
            Sprite fairyTrailSprite = CuteBlockTextureGenerator.GetOrCreateFairyTrailSprite();

            GameObject transObj = new GameObject("FairyScreenTransitionOverlay", typeof(RectTransform));
            transObj.transform.SetParent(canvasObj.transform, false);
            transObj.transform.SetAsLastSibling();
            SetRect(transObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var screenTrans = transObj.AddComponent<FairyScreenTransition>();
            screenTrans.SetupSprites(fairySparkleSprite, fairyTrailSprite);

            // 7. Fairy Touch & Drag FX Overlay (Topmost UI Layer)
            GameObject fairyFXObj = new GameObject("FairyTouchFXOverlay", typeof(RectTransform));
            fairyFXObj.transform.SetParent(canvasObj.transform, false);
            fairyFXObj.transform.SetAsLastSibling();
            SetRect(fairyFXObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var fairyFX = fairyFXObj.AddComponent<FairyTouchFXManager>();
            fairyFX.SetupSprites(fairyRippleSprite, fairySparkleSprite, fairyTrailSprite);

            // 8. Save Scene
            string scenePath = "Assets/Scenes/BlockBlastScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Debug.Log("<color=#FF7AA2><b>[Block Blast]</b> 10종 이상의 젤리 그래픽 에셋, 주아체 고해상도 폰트가 적용된 프리미엄 씬이 완성되었습니다!</color>");
        }

        private static GameObject CreateButton(Transform parent, string name, string label, TMP_FontAsset fontAsset, Vector2 anchor, Vector2 pos, Vector2 size, Sprite btnSprite, float fontSize = 30)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            
            Image img = btnObj.GetComponent<Image>();
            img.sprite = btnSprite;
            if (btnSprite != null && btnSprite.border != Vector4.zero)
            {
                img.type = Image.Type.Sliced;
            }
            else
            {
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }
            img.color = Color.white;

            Button btn = btnObj.GetComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.65f);
            btn.colors = cb;

            SetRect(btnObj, anchor, anchor, pos, size);

            if (!string.IsNullOrEmpty(label))
            {
                GameObject textObj = CreateText(btnObj.transform, "Text", label, fontSize, TextAlignmentOptions.Center, fontAsset, Color.white);
                SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            }

            return btnObj;
        }

        private static GameObject CreateImage(Transform parent, string name, Sprite sprite, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            return CreateImage(parent, name, sprite, anchor, anchor, pos, size);
        }

        private static GameObject CreateImage(Transform parent, string name, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            Image img = obj.GetComponent<Image>();
            img.sprite = sprite;
            img.color = Color.white;
            img.raycastTarget = false;
            SetRect(obj, anchorMin, anchorMax, pos, size);
            return obj;
        }

        private static GameObject CreateText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions alignment, TMP_FontAsset fontAsset, Color color, bool addShadow = false, Color? shadowCol = null)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = fontSize;
            t.font = fontAsset;
            t.fontStyle = FontStyles.Bold; // Crisp, bold vector rendering for 100% sharp legibility
            t.alignment = alignment;
            t.color = color;
            t.enableWordWrapping = false;
            t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false;

            return obj;
        }

        private static GameObject CreateSlider(Transform parent, string name, Vector2 pos, Vector2 size, float initialVal)
        {
            GameObject sliderObj = new GameObject(name, typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(parent, false);
            SetRect(sliderObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
            Slider slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = initialVal;

            // Background
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderObj.transform, false);
            SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image bgImg = bg.GetComponent<Image>();
            bgImg.color = new Color(0.85f, 0.85f, 0.9f, 0.8f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform, false);
            SetRect(fillArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            SetRect(fill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = new Color(1f, 0.45f, 0.68f, 1f); // Vibrant Pink
            slider.fillRect = fill.GetComponent<RectTransform>();

            return sliderObj;
        }

        private static GameObject CreateInputField(Transform parent, string name, string initialText, string placeholderText, TMP_FontAsset fontAsset, Vector2 pos, Vector2 size, Sprite bgSprite)
        {
            GameObject inputObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            inputObj.transform.SetParent(parent, false);
            SetRect(inputObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);

            Image bgImg = inputObj.GetComponent<Image>();
            bgImg.sprite = bgSprite;
            bgImg.type = Image.Type.Sliced;
            bgImg.color = Color.white;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();

            // Text Area
            GameObject textArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
            textArea.transform.SetParent(inputObj.transform, false);
            SetRect(textArea, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-30, -10));

            // Placeholder
            GameObject phObj = CreateText(textArea.transform, "Placeholder", placeholderText, 24, TextAlignmentOptions.Left, fontAsset, new Color(0.6f, 0.6f, 0.6f, 0.6f));
            SetRect(phObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            inputField.placeholder = phObj.GetComponent<TMP_Text>();

            // Text Component
            GameObject textObj = CreateText(textArea.transform, "Text", initialText, 26, TextAlignmentOptions.Left, fontAsset, new Color(0.25f, 0.2f, 0.35f, 1f));
            SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            inputField.textComponent = textObj.GetComponent<TMP_Text>();
            inputField.text = initialText;

            return inputObj;
        }

        private static void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static void BindAudioClips(BlockAudioManager audioMgr)
        {
            if (audioMgr == null) return;

            // SFX Auto-Binding
            audioMgr.sfxPickup = FindAudioClip("Assets/Sounds/SFX", "sfx_block_pickup", "block_pickup", "pickup");
            audioMgr.sfxPlace = FindAudioClip("Assets/Sounds/SFX", "sfx_block_place", "block_place", "place");
            audioMgr.sfxClear = FindAudioClip("Assets/Sounds/SFX", "sfx_block_clear", "block_clear", "clear", "blast");
            audioMgr.sfxUIClick = FindAudioClip("Assets/Sounds/SFX", "sfx_ui_click", "ui_click", "btn_click", "button_click", "click");
            audioMgr.sfxScreenTouch = FindAudioClip("Assets/Sounds/SFX", "sfx_screen_touch", "screen_touch", "touch_screen", "touch", "tap");
            audioMgr.sfxBomb = FindAudioClip("Assets/Sounds/SFX", "sfx_block_bomb", "block_bomb", "bomb");
            audioMgr.sfxRotate = FindAudioClip("Assets/Sounds/SFX", "sfx_block_rotate", "block_rotate", "rotate", "spin");
            audioMgr.sfxSkip = FindAudioClip("Assets/Sounds/SFX", "sfx_block_skip", "block_skip", "skip");
            audioMgr.sfxFever = FindAudioClip("Assets/Sounds/SFX", "sfx_fever_start", "fever_start", "fever");
            audioMgr.sfxGameOver = FindAudioClip("Assets/Sounds/SFX", "sfx_game_over", "game_over");

            // BGM Auto-Binding
            audioMgr.bgmMain = FindAudioClip("Assets/Sounds/BGM", "bgm_main", "bgm", "main_theme");
        }

        private static AudioClip FindAudioClip(string folderPath, params string[] namePatterns)
        {
            string[] extensions = new string[] { ".wav", ".mp3", ".ogg" };
            foreach (var name in namePatterns)
            {
                foreach (var ext in extensions)
                {
                    string path = $"{folderPath}/{name}{ext}";
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip != null) return clip;
                }
            }
            return null;
        }
    }
}
#endif
