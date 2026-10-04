#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
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
                EditorApplication.isPlaying = false;
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

            // High-Quality Complete Dedicated Pastel Block Sprites with Mascots Embedded! (2048x2048 Baked)
            Sprite pinkBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Pink", new Color(1f, 0.48f, 0.65f), "Block_Pink_Mascot.png", 0.90f, true);
            Sprite mintBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Mint", new Color(0.31f, 0.88f, 0.71f), "Block_Mint_Mascot.png", 0.90f, true);
            Sprite goldBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Gold", new Color(1f, 0.75f, 0.26f), "Block_Gold_Mascot.png", 0.90f, true);
            Sprite purpleBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Purple", new Color(0.65f, 0.49f, 1f), "Block_Purple_Mascot.png", 0.90f, true);
            Sprite starBombBlockSprite = CuteBlockTextureGenerator.GetOrCreatePastelBlockSprite("Block_Star_Bomb", new Color(1f, 0.30f, 0.41f), "Jelly_Mascot_Red.png", 0.90f, true);
            Sprite emptyCellSprite = CuteBlockTextureGenerator.GetOrCreateEmptyCellSprite();

            Sprite bgSprite = CuteBlockTextureGenerator.GetOrCreateBackgroundSprite();
            Sprite panelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Panel_Rounded", new Color(0.98f, 0.96f, 1f, 0.88f), new Color(0.85f, 0.80f, 1f, 0.95f));
            Sprite boardPanelSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Board_Panel", new Color(0.20f, 0.16f, 0.35f, 0.92f), new Color(0.55f, 0.45f, 0.85f, 0.95f));
            Sprite scoreBoxSprite = CuteBlockTextureGenerator.GetOrCreateIngameScoreBoxSprite();
            Sprite bestBoxSprite = CuteBlockTextureGenerator.GetOrCreateIngameBestBoxSprite();
            Sprite skillsBarSprite = CuteBlockTextureGenerator.GetOrCreatePanelSprite("Jelly_Skills_Bar", new Color(0.98f, 0.96f, 1f, 0.90f), new Color(0.88f, 0.82f, 1f, 0.95f));
            Sprite btnPinkSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Pink", CuteBlockTextureGenerator.PastelPink);
            Sprite btnTealSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Teal", CuteBlockTextureGenerator.PastelMint);
            Sprite btnLavenderSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Lavender", CuteBlockTextureGenerator.PastelLavender);
            Sprite btnGoldSprite = CuteBlockTextureGenerator.GetOrCreate3DJellyButtonSprite("Jelly_Button_Gold", CuteBlockTextureGenerator.PastelButter);
            Sprite btnPauseCircleSprite = CuteBlockTextureGenerator.GetOrCreate3DRoundJellyButtonSprite("Jelly_Button_Circle_Pink", CuteBlockTextureGenerator.PastelPink);

            Sprite circleFrameSprite = CuteBlockTextureGenerator.GetOrCreateCircleFrameSprite();
            Sprite tabPillSprite = CuteBlockTextureGenerator.GetOrCreateTabPillSprite();
            Sprite cuteCardSprite = CuteBlockTextureGenerator.GetOrCreateCuteCardSprite();

            // Dedicated Customizable Modal & UI Sprites
            Sprite settingsModalCardSprite = CuteBlockTextureGenerator.GetOrCreateSettingsModalCardSprite(true);
            Sprite profileModalCardSprite = CuteBlockTextureGenerator.GetOrCreateProfileModalCardSprite(true);
            Sprite shopModalCardSprite = CuteBlockTextureGenerator.GetOrCreateShopModalCardSprite(true);
            Sprite helpModalCardSprite = CuteBlockTextureGenerator.GetOrCreateHelpModalCardSprite(true);
            Sprite gameOverCardSprite = CuteBlockTextureGenerator.GetOrCreateGameOverCardSprite();
            Sprite shopEquipBtnSprite = CuteBlockTextureGenerator.GetOrCreateShopEquipButtonSprite(true);
            Sprite shopEquippedBtnSprite = CuteBlockTextureGenerator.GetOrCreateShopEquippedButtonSprite(true);
            Sprite shopItemCardSprite = CuteBlockTextureGenerator.GetOrCreateShopItemCardSprite();
            Sprite inputPillSprite = CuteBlockTextureGenerator.GetOrCreateInputPillSprite();
            Sprite sliderTrackSprite = CuteBlockTextureGenerator.GetOrCreateSliderTrackSprite();
            Sprite sliderFillSprite = CuteBlockTextureGenerator.GetOrCreateSliderFillSprite();
            Sprite sliderKnobSprite = CuteBlockTextureGenerator.GetOrCreateSliderKnobSprite();
            Sprite avatarCircleFrameSprite = CuteBlockTextureGenerator.GetOrCreateAvatarCircleFrameSprite();

            // Cute Character & Icon Sprites
            Sprite mascotSprite = CuteBlockTextureGenerator.GetOrCreateMascotSprite();
            Sprite mascotMintSprite = CuteBlockTextureGenerator.GetOrCreateMintMascotSprite();
            Sprite crownSprite = CuteBlockTextureGenerator.GetOrCreateCrownSprite();
            Sprite flameSprite = CuteBlockTextureGenerator.GetOrCreateFlameSprite();
            Sprite diceSprite = CuteBlockTextureGenerator.GetOrCreateDiceSprite();
            Sprite arrowSprite = CuteBlockTextureGenerator.GetOrCreateRotateArrowSprite();
            Sprite[] langLogoSprites = CuteBlockTextureGenerator.GetOrCreateAllLanguageLogoSprites();

            // Layered Pastel Side Wings Sprites (Widescreen PC Display Margins)
            Sprite sideWingLeftSprite = CuteBlockTextureGenerator.GetOrCreateSideWingLeftSprite();
            Sprite sideWingRightSprite = CuteBlockTextureGenerator.GetOrCreateSideWingRightSprite();
            Sprite sideWingSkySprite = CuteBlockTextureGenerator.GetOrCreateSideWingSkyGradientSprite();
            Sprite sideWingBorderSprite = CuteBlockTextureGenerator.GetOrCreateSideWingBorderSprite();

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
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 5f;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.0f; // Match Width ensures board never clips on any device ratio
            scaler.dynamicPixelsPerUnit = 1.0f; // Crisp, sharp 1:1 text rendering without atlas overflow!
            scaler.referencePixelsPerUnit = 100f;

            canvasObj.AddComponent<AspectRatioAdapter>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // 1. Fullscreen Base Sky (Ensures no black edges on super-ultrawide monitors)
            GameObject baseSkyObj = new GameObject("CanvasBaseSky", typeof(RectTransform), typeof(Image));
            baseSkyObj.transform.SetParent(canvasObj.transform, false);
            baseSkyObj.transform.SetAsFirstSibling();
            SetRect(baseSkyObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image baseSkyImg = baseSkyObj.GetComponent<Image>();
            baseSkyImg.sprite = sideWingSkySprite;
            baseSkyImg.color = Color.white;
            baseSkyImg.raycastTarget = false;

            // 2. Central InGame Theme Background (Centered 1080 width)
            GameObject bgObj = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(canvasObj.transform, false);
            SetRect(bgObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1080f, 0f));
            Image bgImgComp = bgObj.GetComponent<Image>();
            bgImgComp.sprite = bgSprite;
            bgImgComp.color = Color.white;
            bgImgComp.raycastTarget = false;

            // 3. Layered Pastel Side Wings (Appears in Left & Right margins on widescreen aspect ratios)
            GameObject sideWingsRoot = new GameObject("SideWingsRoot", typeof(RectTransform));
            sideWingsRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(sideWingsRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Left Wing (From screen left edge to X = -540)
            GameObject leftWing = new GameObject("LeftWing", typeof(RectTransform), typeof(Image));
            leftWing.transform.SetParent(sideWingsRoot.transform, false);
            RectTransform leftWingRT = leftWing.GetComponent<RectTransform>();
            leftWingRT.anchorMin = new Vector2(0f, 0f);
            leftWingRT.anchorMax = new Vector2(0.5f, 1f);
            leftWingRT.offsetMin = new Vector2(0f, 0f);
            leftWingRT.offsetMax = new Vector2(-540f, 0f);
            Image lWingImg = leftWing.GetComponent<Image>();
            lWingImg.color = Color.clear;
            lWingImg.raycastTarget = false;

            // Left Wing Sky Base
            GameObject leftSky = CreateImage(leftWing.transform, "SkyBase", sideWingSkySprite, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image leftSkyImg = leftSky.GetComponent<Image>();
            leftSkyImg.type = Image.Type.Simple;
            leftSkyImg.preserveAspect = false;

            // Left Wing Art Illustration (Anchored to right edge at X = -540, expanding outwards to the left)
            GameObject leftArt = new GameObject("ArtIllustration", typeof(RectTransform), typeof(Image));
            leftArt.transform.SetParent(leftWing.transform, false);
            RectTransform leftArtRT = leftArt.GetComponent<RectTransform>();
            leftArtRT.anchorMin = new Vector2(1f, 0f);
            leftArtRT.anchorMax = new Vector2(1f, 1f);
            leftArtRT.pivot = new Vector2(1f, 0.5f);
            leftArtRT.anchoredPosition = Vector2.zero;
            leftArtRT.sizeDelta = new Vector2(1200f, 0f);
            Image leftArtImg = leftArt.GetComponent<Image>();
            leftArtImg.sprite = sideWingLeftSprite;
            leftArtImg.type = Image.Type.Simple;
            leftArtImg.preserveAspect = false;
            leftArtImg.raycastTarget = false;

            // Left Floating Sparkles & Twinkles
            Sprite sideSparkleStarSprite = CuteBlockTextureGenerator.GetOrCreateFireworksSparkleStarSprite();
            Sprite sideGlowOrbSprite = CuteBlockTextureGenerator.GetOrCreateFireworksGlowOrbSprite();

            GameObject leftDecoGroup = new GameObject("DecoGroup", typeof(RectTransform));
            leftDecoGroup.transform.SetParent(leftWing.transform, false);
            SetRect(leftDecoGroup, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), Vector2.zero, new Vector2(1080f, 1920f));

            GameObject lStar1 = CreateImage(leftDecoGroup.transform, "Star1", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(-180, 520), new Vector2(52, 52));
            GameObject lStar2 = CreateImage(leftDecoGroup.transform, "Star2", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(-360, 240), new Vector2(40, 40));
            GameObject lStar3 = CreateImage(leftDecoGroup.transform, "Star3", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(-150, -320), new Vector2(44, 44));
            GameObject lBubble1 = CreateImage(leftDecoGroup.transform, "Bubble1", sideGlowOrbSprite, new Vector2(0.5f, 0.5f), new Vector2(-280, -80), new Vector2(56, 56));
            GameObject lBubble2 = CreateImage(leftDecoGroup.transform, "Bubble2", sideGlowOrbSprite, new Vector2(0.5f, 0.5f), new Vector2(-120, 110), new Vector2(42, 42));

            // Right Wing (From X = +540 to screen right edge)
            GameObject rightWing = new GameObject("RightWing", typeof(RectTransform), typeof(Image));
            rightWing.transform.SetParent(sideWingsRoot.transform, false);
            RectTransform rightWingRT = rightWing.GetComponent<RectTransform>();
            rightWingRT.anchorMin = new Vector2(0.5f, 0f);
            rightWingRT.anchorMax = new Vector2(1f, 1f);
            rightWingRT.offsetMin = new Vector2(540f, 0f);
            rightWingRT.offsetMax = new Vector2(0f, 0f);
            Image rWingImg = rightWing.GetComponent<Image>();
            rWingImg.color = Color.clear;
            rWingImg.raycastTarget = false;

            // Right Wing Sky Base
            GameObject rightSky = CreateImage(rightWing.transform, "SkyBase", sideWingSkySprite, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image rightSkyImg = rightSky.GetComponent<Image>();
            rightSkyImg.type = Image.Type.Simple;
            rightSkyImg.preserveAspect = false;

            // Right Wing Art Illustration (Anchored to left edge at X = +540, expanding outwards to the right)
            GameObject rightArt = new GameObject("ArtIllustration", typeof(RectTransform), typeof(Image));
            rightArt.transform.SetParent(rightWing.transform, false);
            RectTransform rightArtRT = rightArt.GetComponent<RectTransform>();
            rightArtRT.anchorMin = new Vector2(0f, 0f);
            rightArtRT.anchorMax = new Vector2(0f, 1f);
            rightArtRT.pivot = new Vector2(0f, 0.5f);
            rightArtRT.anchoredPosition = Vector2.zero;
            rightArtRT.sizeDelta = new Vector2(1200f, 0f);
            Image rightArtImg = rightArt.GetComponent<Image>();
            rightArtImg.sprite = sideWingRightSprite;
            rightArtImg.type = Image.Type.Simple;
            rightArtImg.preserveAspect = false;
            rightArtImg.raycastTarget = false;

            // Right Floating Sparkles & Twinkles
            GameObject rightDecoGroup = new GameObject("DecoGroup", typeof(RectTransform));
            rightDecoGroup.transform.SetParent(rightWing.transform, false);
            SetRect(rightDecoGroup, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(1080f, 1920f));

            GameObject rStar1 = CreateImage(rightDecoGroup.transform, "Star1", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(200, 500), new Vector2(52, 52));
            GameObject rStar2 = CreateImage(rightDecoGroup.transform, "Star2", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(340, 160), new Vector2(42, 42));
            GameObject rStar3 = CreateImage(rightDecoGroup.transform, "Star3", sideSparkleStarSprite, new Vector2(0.5f, 0.5f), new Vector2(160, -360), new Vector2(46, 46));
            GameObject rBubble1 = CreateImage(rightDecoGroup.transform, "Bubble1", sideGlowOrbSprite, new Vector2(0.5f, 0.5f), new Vector2(270, -60), new Vector2(58, 58));
            GameObject rBubble2 = CreateImage(rightDecoGroup.transform, "Bubble2", sideGlowOrbSprite, new Vector2(0.5f, 0.5f), new Vector2(130, 240), new Vector2(40, 40));

            // Subtle vertical framing divider lines at X = -540 and X = +540
            GameObject leftDivider = CreateImage(sideWingsRoot.transform, "LeftDivider", sideWingBorderSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), new Vector2(-540f, 0f), new Vector2(64f, 0f));
            GameObject rightDivider = CreateImage(sideWingsRoot.transform, "RightDivider", sideWingBorderSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), new Vector2(540f, 0f), new Vector2(64f, 0f));
            rightDivider.transform.localScale = new Vector3(-1f, 1f, 1f); // Mirror symmetrically

            // Attach SideWingsDecorator
            var wingsDeco = sideWingsRoot.AddComponent<SideWingsDecorator>();
            wingsDeco.SetupReferences(
                leftWing, rightWing,
                leftDivider, rightDivider,
                leftSkyImg, rightSkyImg,
                leftArtImg, rightArtImg,
                new RectTransform[] { leftDecoGroup.GetComponent<RectTransform>(), rightDecoGroup.GetComponent<RectTransform>() },
                new Image[] { lStar1.GetComponent<Image>(), lStar2.GetComponent<Image>(), lStar3.GetComponent<Image>(), rStar1.GetComponent<Image>(), rStar2.GetComponent<Image>(), rStar3.GetComponent<Image>() }
            );

            // 5. Game Managers Object
            GameObject mgrObj = new GameObject("GameManagers");
            var gridMgr = mgrObj.AddComponent<BlockGridManager>();
            var spawner = mgrObj.AddComponent<BlockSpawner>();
            var audioMgr = mgrObj.AddComponent<BlockAudioManager>();
            BindAudioClips(audioMgr);

            CuteCursorGenerator.GenerateCursorTextures(out Texture2D normalCursor, out Texture2D clickCursor);
            var cursorMgr = mgrObj.AddComponent<CuteCursorManager>();
            cursorMgr.Setup(normalCursor, clickCursor, new Vector2(3f, 3f));

            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetupPastelBlockSprites(pinkBlockSprite, mintBlockSprite, goldBlockSprite, purpleBlockSprite, starBombBlockSprite);
            EditorUtility.SetDirty(gridMgr);

            // ==========================================
            // CUTE PASTEL UI HIERARCHY (RESPONSIVE)
            // ==========================================

            // --- In-Game Root Container (Starts inactive so it NEVER shows during main menu or lobby!) ---
            GameObject inGameRoot = new GameObject("InGameRoot", typeof(RectTransform), typeof(SafeAreaFitter));
            inGameRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(inGameRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1080f, 0f));
            inGameRoot.SetActive(false);

            // --- A. Header (Top Anchor) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -115), new Vector2(-60, 160));

            // Pause Button (Far Left: pos 58, size 104x104 - high visibility, text fully contained with zero overflow)
            Sprite pauseBarsSprite = CuteBlockTextureGenerator.GetOrCreatePauseBarsSprite();
            GameObject pauseBtnObj = CreateButton(headerObj.transform, "BtnPause", "", cuteFont, new Vector2(0, 0.5f), new Vector2(58, 0), new Vector2(104, 104), btnPauseCircleSprite, 20);
            
            // Icon: Pause Bars (||)
            GameObject pIcon = CreateImage(pauseBtnObj.transform, "PauseBarsIcon", pauseBarsSprite, new Vector2(0.5f, 0.5f), new Vector2(0, 12), new Vector2(36, 36));
            pIcon.GetComponent<Image>().raycastTarget = false;
            
            // Label: "일시정지" (Clean Korean text, 100% supported by Jua SDF font, perfectly fitted)
            GameObject pLabel = CreateText(pauseBtnObj.transform, "PauseLabel", "일시정지", 17, TextAlignmentOptions.Center, cuteFont, Color.white, true, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            SetRect(pLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -22), new Vector2(96, 26));
            pLabel.GetComponent<TMP_Text>().raycastTarget = false;

            // Current Score (Left-Center)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(195, 0), new Vector2(160, 110));
            Image sBoxImg = scoreBox.GetComponent<Image>();
            sBoxImg.sprite = scoreBoxSprite;
            sBoxImg.type = Image.Type.Sliced;

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 20, TextAlignmentOptions.Center, cuteFont, new Color(0.32f, 0.18f, 0.52f));
            SetRect(scoreLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 24), new Vector2(140, 26));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 44, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.40f, 0.02f)); // Warm Golden Honey
            SetRect(scoreVal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -16), new Vector2(140, 48));

            // Center Title (3D Jelly Mallang Logo Banner) - Doubled 2x size as requested!
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform), typeof(Image));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(12, -18), new Vector2(480, 320));
            Image titleLogoImg = titleBox.GetComponent<Image>();
            titleLogoImg.sprite = CuteBlockTextureGenerator.GetOrCreateMallangBlastLogoSprite();
            titleLogoImg.preserveAspect = true;
            titleLogoImg.raycastTarget = false; // Never block raycasts/touches!
            uiMgr.SetupLanguageLogos(titleLogoImg, langLogoSprites);

            // Best Score (Right)
            GameObject bestBox = new GameObject("BestBox", typeof(RectTransform), typeof(Image));
            bestBox.transform.SetParent(headerObj.transform, false);
            SetRect(bestBox, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-95, 0), new Vector2(160, 110));
            Image bBoxImg = bestBox.GetComponent<Image>();
            bBoxImg.sprite = bestBoxSprite;
            bBoxImg.type = Image.Type.Sliced;

            GameObject bestLabel = CreateText(bestBox.transform, "Label", "BEST", 20, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.32f, 0.05f));
            SetRect(bestLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 24), new Vector2(140, 26));

            GameObject bestVal = CreateText(bestBox.transform, "Value", "0", 44, TextAlignmentOptions.Center, cuteFont, new Color(0.88f, 0.30f, 0.02f));
            SetRect(bestVal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -16), new Vector2(140, 48));

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
            SetRect(timeTextObj, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(430, 0), new Vector2(130, 40));
            TMP_Text timeRemainingTMP = timeTextObj.GetComponent<TMP_Text>();

            // Fullscreen Danger Vignette (Pulses in deep red when remaining time <= 3s)
            Sprite vignetteSprite = CuteBlockTextureGenerator.GetOrCreateVignetteSprite();
            GameObject vignetteObj = CreateImage(inGameRoot.transform, "VignetteDangerOverlay", vignetteSprite, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image vignetteImg = vignetteObj.GetComponent<Image>();
            vignetteImg.raycastTarget = false; // Never blocks clicks/touches!
            vignetteObj.SetActive(false);

            // 🎲 Skip Button (User-Uploaded 2.5D Marshmallow Jelly Button)
            Sprite btnSkipUser = CuteBlockTextureGenerator.GetOrCreateUserSkipButtonSprite();
            GameObject skipBtnObj = CreateButton(skillsBar.transform, "BtnSkip", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-155, 0), new Vector2(88, 88), btnSkipUser, 32);
            skipBtnObj.AddComponent<CanvasGroup>(); // For dimming/brightening skip button
            var btnSkip = skipBtnObj.GetComponent<Button>();

            TMP_Text skipBadgeTMP = null;

            // 🔄 Spin Button (User-Uploaded 2.5D Marshmallow Jelly Button)
            Sprite btnSpinUser = CuteBlockTextureGenerator.GetOrCreateUserSpinButtonSprite();
            GameObject rotateBtnObj = CreateButton(skillsBar.transform, "BtnRotate", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-60, 0), new Vector2(88, 88), btnSpinUser, 32);
            var btnRotate = rotateBtnObj.GetComponent<Button>();

            // 🌟 Special Mascot Board Clear Skill Button (Exclusive to Special Mascot)
            Sprite specialAvSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Avatar_Special.png");
            GameObject clearBtnObj = CreateButton(skillsBar.transform, "BtnBoardClear", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-265, 0), new Vector2(92, 92), circleFrameSprite, 20);
            clearBtnObj.GetComponent<Image>().color = new Color(1f, 0.95f, 0.85f, 1f);

            GameObject clearGlowObj = CreateImage(clearBtnObj.transform, "GlowAura", circleFrameSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(104, 104));
            Image clearGlowImg = clearGlowObj.GetComponent<Image>();
            clearGlowImg.color = new Color(1f, 0.85f, 0.2f, 0.6f);
            clearGlowImg.raycastTarget = false;

            GameObject clearIconObj = CreateImage(clearBtnObj.transform, "MascotIcon", specialAvSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(76, 76));
            clearIconObj.GetComponent<Image>().preserveAspect = true;
            clearIconObj.GetComponent<Image>().raycastTarget = false;

            GameObject clearBadgeObj = CreateImage(clearBtnObj.transform, "ClearBadge", tabPillSprite, new Vector2(0.5f, 0), new Vector2(0, -14), new Vector2(98, 26));
            clearBadgeObj.GetComponent<Image>().type = Image.Type.Sliced;
            clearBadgeObj.GetComponent<Image>().color = new Color(0.2f, 0.12f, 0.35f, 0.92f);

            GameObject clearBadgeTxt = CreateText(clearBadgeObj.transform, "Text", "올 클리어!", 16, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.95f, 0.2f, 1f));
            SetRect(clearBadgeTxt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var btnBoardClear = clearBtnObj.GetComponent<Button>();

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
                SetRect(slotCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(290, 240));
                Image scImg = slotCard.GetComponent<Image>();
                scImg.sprite = scoreBoxSprite;
                scImg.type = Image.Type.Sliced;
                scImg.color = new Color(1f, 1f, 1f, 0.92f); // Crisp glossy white pastel card
                scImg.raycastTarget = false;

                GameObject slot = new GameObject($"Slot_{i}", typeof(RectTransform));
                slot.transform.SetParent(handContainer.transform, false);
                SetRect(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(290, 240));
                slotTransforms[i] = slot.transform;
            }

            spawner.SetupReferences(slotTransforms, canvas, pinkBlockSprite, starBombBlockSprite);
            EditorUtility.SetDirty(spawner);

            // Bottom Guide Text (Auto-sizing enabled, padding to prevent overflow)
            GameObject guidePill = new GameObject("GuidePill", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            guidePill.transform.SetParent(inGameRoot.transform, false);
            SetRect(guidePill, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 85), new Vector2(880, 64));
            Image gpImg = guidePill.GetComponent<Image>();
            gpImg.sprite = scoreBoxSprite;
            gpImg.type = Image.Type.Sliced;
            gpImg.color = new Color(0.12f, 0.10f, 0.25f, 0.72f);
            gpImg.raycastTarget = false;

            GameObject guideObj = CreateText(guidePill.transform, "GuideText", "돌리기 버튼으로 블록을 회전해요!", 26, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.96f, 0.85f));
            SetRect(guideObj, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-40, -10));
            TextMeshProUGUI guideTMP = guideObj.GetComponent<TextMeshProUGUI>();
            guideTMP.enableAutoSizing = true;
            guideTMP.fontSizeMin = 18f;
            guideTMP.fontSizeMax = 26f;

            uiMgr.SetupGuideTip(guideTMP, guidePill.GetComponent<CanvasGroup>());

            // --- E. Game Over Modal Dialog ---
            GameObject modalObj = new GameObject("GameOverModal", typeof(RectTransform), typeof(Image));
            modalObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(modalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image modalOverlay = modalObj.GetComponent<Image>();
            modalOverlay.color = new Color(0.05f, 0.06f, 0.12f, 0.85f);
            modalObj.SetActive(false);

            GameObject dialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            dialog.transform.SetParent(modalObj.transform, false);
            SetRect(dialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(840, 1125));
            Image dImg = dialog.GetComponent<Image>();
            dImg.sprite = gameOverCardSprite != null ? gameOverCardSprite : panelSprite;
            dImg.type = Image.Type.Simple;
            dImg.preserveAspect = true;
            dImg.color = Color.white;

            // Empty placeholder for Title/Subtitle to preserve hierarchy compatibility
            GameObject overTitle = CreateText(dialog.transform, "Title", "", 1, TextAlignmentOptions.Center, cuteFont, Color.clear);
            overTitle.SetActive(false);
            GameObject overSub = CreateText(dialog.transform, "Subtitle", "", 1, TextAlignmentOptions.Center, cuteFont, Color.clear);
            overSub.SetActive(false);

            // Centered Score Card overlaid accurately inside the card's white rounded box (shifted slightly right to account for mascot on the left)
            GameObject modalScoreBox = new GameObject("ScoreCard", typeof(RectTransform), typeof(Image));
            modalScoreBox.transform.SetParent(dialog.transform, false);
            SetRect(modalScoreBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(32, 118), new Vector2(360, 140));
            Image mScoreImg = modalScoreBox.GetComponent<Image>();
            mScoreImg.color = Color.clear; // Transparent so the image's original white box shows through

            GameObject finalLabel = CreateText(modalScoreBox.transform, "FLabel", LocalizationManager.Get("ingame_gameover_score"), 26, TextAlignmentOptions.Center, cuteFont, new Color(0.42f, 0.30f, 0.58f));
            SetRect(finalLabel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -32), new Vector2(340, 32));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 64, TextAlignmentOptions.Center, cuteFont, new Color(0.95f, 0.24f, 0.48f));
            SetRect(finalVal, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -74), new Vector2(340, 62));

            string bestPrefix = LocalizationManager.Get("profile_best_score_prefix", "BEST");
            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", $"{bestPrefix}: 0", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.48f, 0.42f, 0.62f));
            SetRect(mBestVal, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 10), new Vector2(340, 28));

            // Gold Reward Pill Badge in GameOver Dialog ("+X G" reward for (score / 1000))
            Sprite goldCoinSprite = CuteBlockTextureGenerator.GetOrCreateGoldCoinSprite();
            GameObject goldRewardPill = CreateImage(dialog.transform, "GoldRewardPill", tabPillSprite, new Vector2(0.5f, 0.5f), new Vector2(32, 18), new Vector2(240, 50));
            goldRewardPill.GetComponent<Image>().type = Image.Type.Sliced;
            goldRewardPill.GetComponent<Image>().color = new Color(1f, 0.93f, 0.50f, 0.95f);

            GameObject gIcon = CreateImage(goldRewardPill.transform, "GoldIcon", goldCoinSprite, new Vector2(0, 0.5f), new Vector2(24, 0), new Vector2(34, 34));
            gIcon.GetComponent<Image>().preserveAspect = true;

            GameObject gRewardTxt = CreateText(goldRewardPill.transform, "RewardText", "+0 G", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.48f, 0.28f, 0.05f));
            SetRect(gRewardTxt, Vector2.zero, Vector2.one, new Vector2(26, 0), Vector2.zero);

            // Bottom Action Buttons: Restart & Lobby (Positioned with comfortable spacing below the mascot)
            GameObject restartBtn = CreateButton(dialog.transform, "BtnRestart", LocalizationManager.Get("ingame_restart"), cuteFont, new Vector2(0.5f, 0.5f), new Vector2(0, -265), new Vector2(500, 88), btnPinkSprite, 36, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            GameObject lobbyBtn = CreateButton(dialog.transform, "BtnLobby", LocalizationManager.Get("ingame_lobby"), cuteFont, new Vector2(0.5f, 0.5f), new Vector2(0, -368), new Vector2(500, 84), btnTealSprite, 34, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            // Bind UI References
            uiMgr.SetupReferences(
                scoreVal.GetComponent<TMP_Text>(),
                bestVal.GetComponent<TMP_Text>(),
                tFillImg,
                timeRemainingTMP,
                vignetteImg,
                btnSkip,
                skipBadgeTMP,
                btnRotate,
                comboObj.GetComponent<TMP_Text>(),
                boardContainer.GetComponent<RectTransform>(),
                modalObj,
                finalVal.GetComponent<TMP_Text>(),
                mBestVal.GetComponent<TMP_Text>(),
                restartBtn.GetComponent<Button>(),
                inGameRoot,
                lobbyBtn.GetComponent<Button>(),
                gRewardTxt.GetComponent<TMP_Text>()
            );
            uiMgr.SetupSpecialSkillButton(btnBoardClear, clearGlowImg, clearBadgeTxt.GetComponent<TMP_Text>(), null);

            // --- E2. Pause Modal Dialog ---
            GameObject pauseModalObj = new GameObject("PauseModal", typeof(RectTransform), typeof(Image));
            pauseModalObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(pauseModalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image pauseOverlay = pauseModalObj.GetComponent<Image>();
            pauseOverlay.color = new Color(0.05f, 0.06f, 0.12f, 0.85f);
            pauseModalObj.SetActive(false);

            GameObject pauseDialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            pauseDialog.transform.SetParent(pauseModalObj.transform, false);
            SetRect(pauseDialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 920));
            Image pDialogImg = pauseDialog.GetComponent<Image>();
            pDialogImg.sprite = cuteCardSprite;
            pDialogImg.type = Image.Type.Sliced;
            pDialogImg.color = Color.white;

            GameObject pauseTitle = CreateText(pauseDialog.transform, "Title", "일시 정지", 58, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(pauseTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -75), new Vector2(600, 70));

            GameObject pauseSub = CreateText(pauseDialog.transform, "Subtitle", "잠시 쉬어가는 중이에요~", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.45f, 0.70f));
            SetRect(pauseSub, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -135), new Vector2(600, 35));

            // Adorable Center Mascot (Transparent Cutout, shifted up)
            Sprite transparentPinkMascot = CuteBlockTextureGenerator.GetOrCreateLeftPoppingMascotSprite();
            GameObject pauseMascot = CreateImage(pauseDialog.transform, "Mascot", transparentPinkMascot, new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(165, 165));
            pauseMascot.GetComponent<Image>().preserveAspect = true;

            // 3 Action Buttons (Shifted up with 36px safe padding from card bottom)
            GameObject btnResumeObj = CreateButton(pauseDialog.transform, "BtnResume", "계속하기 (Resume)", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 290), new Vector2(540, 88), btnTealSprite, 34, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
            GameObject btnRestartObj = CreateButton(pauseDialog.transform, "BtnRestart", "다시 시작 (Restart)", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 185), new Vector2(540, 88), btnPinkSprite, 34, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            GameObject btnLobbyObj = CreateButton(pauseDialog.transform, "BtnLobby", "로비로 나가기", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(540, 88), btnLavenderSprite, 32, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));

            uiMgr.SetupPauseModal(
                pauseModalObj,
                pauseBtnObj.GetComponent<Button>(),
                btnResumeObj.GetComponent<Button>(),
                btnRestartObj.GetComponent<Button>(),
                btnLobbyObj.GetComponent<Button>()
            );

            // --- E3. Restart Confirm Modal (overlay on top of PauseModal) ---
            GameObject rConfirmModal = new GameObject("RestartConfirmModal", typeof(RectTransform), typeof(Image));
            rConfirmModal.transform.SetParent(inGameRoot.transform, false);
            SetRect(rConfirmModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image rConfirmOverlay = rConfirmModal.GetComponent<Image>();
            rConfirmOverlay.color = new Color(0.05f, 0.03f, 0.12f, 0.75f);
            rConfirmModal.SetActive(false);

            // Confirm Card
            GameObject rCard = new GameObject("DialogCard", typeof(RectTransform), typeof(Image));
            rCard.transform.SetParent(rConfirmModal.transform, false);
            SetRect(rCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720, 440));
            Image rCardImg = rCard.GetComponent<Image>();
            rCardImg.sprite = settingsModalCardSprite;
            rCardImg.type = Image.Type.Sliced;
            rCardImg.color = Color.white;

            // Title
            GameObject rTitle = CreateText(rCard.transform, "Title", "다시 시작할까요?", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(rTitle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -58), new Vector2(600, 52));

            // Subtitle
            GameObject rSub = CreateText(rCard.transform, "Subtitle", "지금까지의 점수가 사라져요...", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.52f, 0.40f, 0.68f));
            SetRect(rSub, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -128), new Vector2(580, 36));

            // Desc
            GameObject rDesc = CreateText(rCard.transform, "Desc", "그래도 다시 시작하시겠어요?", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(rDesc, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -185), new Vector2(600, 36));

            // Buttons
            GameObject rYesBtn = CreateButton(rCard.transform, "BtnYes", "다시 시작!", cuteFont, new Vector2(0.5f, 0f), new Vector2(-140, 80), new Vector2(270, 68), btnPinkSprite, 28, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            GameObject rNoBtn  = CreateButton(rCard.transform, "BtnNo",  "계속하기", cuteFont, new Vector2(0.5f, 0f), new Vector2( 140, 80), new Vector2(270, 68), btnTealSprite, 28, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            uiMgr.SetupRestartConfirmModal(rConfirmModal, rYesBtn.GetComponent<Button>(), rNoBtn.GetComponent<Button>());

            // --- E3-B. Lobby Confirm Modal (overlay on top of PauseModal) ---
            GameObject lConfirmModal = new GameObject("LobbyConfirmModal", typeof(RectTransform), typeof(Image));
            lConfirmModal.transform.SetParent(inGameRoot.transform, false);
            SetRect(lConfirmModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image lConfirmOverlay = lConfirmModal.GetComponent<Image>();
            lConfirmOverlay.color = new Color(0.05f, 0.03f, 0.12f, 0.75f);
            lConfirmModal.SetActive(false);

            // Confirm Card
            GameObject lCard = new GameObject("DialogCard", typeof(RectTransform), typeof(Image));
            lCard.transform.SetParent(lConfirmModal.transform, false);
            SetRect(lCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720, 440));
            Image lCardImg = lCard.GetComponent<Image>();
            lCardImg.sprite = settingsModalCardSprite;
            lCardImg.type = Image.Type.Sliced;
            lCardImg.color = Color.white;

            // Title
            GameObject lTitle = CreateText(lCard.transform, "Title", "로비로 이동할까요?", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(lTitle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -58), new Vector2(600, 52));

            // Subtitle
            GameObject lSub = CreateText(lCard.transform, "Subtitle", "진행 중인 게임 내용이 저장되지 않아요.", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.52f, 0.40f, 0.68f));
            SetRect(lSub, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -128), new Vector2(580, 36));

            // Desc
            GameObject lDesc = CreateText(lCard.transform, "Desc", "정말 로비로 나가시겠어요?", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(lDesc, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -185), new Vector2(600, 36));

            // Buttons
            GameObject lYesBtn = CreateButton(lCard.transform, "BtnYes", "로비로 이동", cuteFont, new Vector2(0.5f, 0f), new Vector2(-140, 80), new Vector2(270, 68), btnLavenderSprite, 28, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            GameObject lNoBtn  = CreateButton(lCard.transform, "BtnNo",  "계속하기", cuteFont, new Vector2(0.5f, 0f), new Vector2( 140, 80), new Vector2(270, 68), btnTealSprite, 28, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            uiMgr.SetupLobbyConfirmModal(lConfirmModal, lYesBtn.GetComponent<Button>(), lNoBtn.GetComponent<Button>());

            // --- E4. White Flash Overlay (full-screen for restart transition) ---
            GameObject whiteFlash = new GameObject("WhiteFlashOverlay", typeof(RectTransform), typeof(Image));
            whiteFlash.transform.SetParent(inGameRoot.transform, false);
            SetRect(whiteFlash, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image wfImg = whiteFlash.GetComponent<Image>();
            wfImg.color = new Color(1f, 1f, 1f, 0f);
            wfImg.raycastTarget = true; // block input during flash
            whiteFlash.SetActive(false);

            uiMgr.SetupWhiteFlashOverlay(wfImg);

            // --- E5. Special Skill Board Clear Cut-in Overlay ("뾰로롱~~" Animation & Particles) ---
            GameObject cutinRoot = new GameObject("SpecialSkillCutinOverlay", typeof(RectTransform));
            cutinRoot.transform.SetParent(inGameRoot.transform, false);
            cutinRoot.transform.SetAsLastSibling();
            SetRect(cutinRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject cutinDarkBg = CreateImage(cutinRoot.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            cutinDarkBg.GetComponent<Image>().color = new Color(0.04f, 0.02f, 0.12f, 0.65f);
            cutinDarkBg.GetComponent<Image>().raycastTarget = false;

            Sprite cutinAuraSp = CuteBlockTextureGenerator.GetOrCreateGachaRainbowAuraSprite();
            GameObject cutinAura = CreateImage(cutinRoot.transform, "AuraGlow", cutinAuraSp, new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(560, 560));

            Sprite specialCutout = CuteBlockTextureGenerator.GetOrCreateSpecialMascotSprite();
            GameObject cutinMascot = CreateImage(cutinRoot.transform, "MascotCutout", specialCutout, new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(380, 380));
            cutinMascot.GetComponent<Image>().preserveAspect = true;

            GameObject cutinTitle = CreateText(cutinRoot.transform, "Title", "★ 올 클리어 엔젤! ★", 36, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.92f, 0.35f));
            SetRect(cutinTitle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -210), new Vector2(600, 60));

            Sprite starSp = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();
            RectTransform[] cutinStars = new RectTransform[12];
            for (int s = 0; s < 12; s++)
            {
                GameObject st = CreateImage(cutinRoot.transform, $"Sparkle_{s}", starSp, new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(52, 52));
                cutinStars[s] = st.GetComponent<RectTransform>();
            }
            cutinRoot.SetActive(false);

            uiMgr.SetupSpecialSkillCutin(cutinRoot, cutinMascot.GetComponent<Image>(), cutinAura.GetComponent<Image>(), cutinTitle.GetComponent<TMP_Text>(), cutinStars);
            EditorUtility.SetDirty(uiMgr);

            // --- F. Fullscreen Cinematic Main Menu Screen (Exact 9:16 Ratio matching Lobby) ---
            GameObject mainMenu = new GameObject("MainMenuScreen", typeof(RectTransform), typeof(CanvasGroup), typeof(RectMask2D), typeof(MainMenuCinematicController));
            mainMenu.transform.SetParent(canvasObj.transform, false);
            SetRect(mainMenu, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1080f, 0f));

            // Fullscreen invisible touch catcher so clicking anywhere triggers game start
            GameObject touchCatcher = new GameObject("TouchCatcher", typeof(RectTransform), typeof(Image));
            touchCatcher.transform.SetParent(mainMenu.transform, false);
            SetRect(touchCatcher, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            Image touchCatcherImg = touchCatcher.GetComponent<Image>();
            touchCatcherImg.color = Color.clear;
            touchCatcherImg.raycastTarget = true;

            // Cinematic Viewport Root (Handles Camera Zoom In, Pan, and Zoom Out)
            GameObject cinematicRootObj = new GameObject("CinematicRoot", typeof(RectTransform));
            cinematicRootObj.transform.SetParent(mainMenu.transform, false);
            SetRect(cinematicRootObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform cinematicRootRT = cinematicRootObj.GetComponent<RectTransform>();
            cinematicRootRT.localScale = Vector3.one * 1.55f;
            cinematicRootRT.anchoredPosition = new Vector2(360f, 60f);

            // Wide Seamless Dreamy Background (Ample 3000 x 3000 coverage so camera pan/zoom never exposes edges)
            Sprite wideBgSprite = CuteBlockTextureGenerator.GetOrCreateMainMenuWideBackgroundSprite();
            GameObject menuBgObj = new GameObject("MenuBackground", typeof(RectTransform), typeof(Image));
            menuBgObj.transform.SetParent(cinematicRootObj.transform, false);
            SetRect(menuBgObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(3000f, 3000f));
            Image menuBgImg = menuBgObj.GetComponent<Image>();
            menuBgImg.sprite = wideBgSprite;
            menuBgImg.color = Color.white;
            menuBgImg.raycastTarget = false;

            // Fireworks Particle FX Container (Screen space overlay on mainMenu)
            GameObject fireworksObj = new GameObject("FireworksFX", typeof(RectTransform), typeof(JellyFireworksEffect));
            fireworksObj.transform.SetParent(mainMenu.transform, false);
            SetRect(fireworksObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
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
            logoObj.GetComponent<RectTransform>().localScale = Vector3.zero;

            // Left Popping Mascot (Strawberry Smile) - Clean transparent cutout without black box
            Sprite leftMascotSprite = CuteBlockTextureGenerator.GetOrCreateLeftPoppingMascotSprite();
            GameObject leftMascotObj = CreateImage(cinematicRootObj.transform, "LeftMascot", leftMascotSprite, new Vector2(0.5f, 0.5f), new Vector2(-260, -80), new Vector2(300, 300));
            RectTransform leftMascotRT = leftMascotObj.GetComponent<RectTransform>();
            leftMascotRT.localScale = Vector3.zero;

            // Right Popping Mascot (Mint Soda) - Clean transparent cutout without black box
            Sprite rightMascotSprite = CuteBlockTextureGenerator.GetOrCreateRightPoppingMascotSprite();
            GameObject rightMascotObj = CreateImage(cinematicRootObj.transform, "RightMascot", rightMascotSprite, new Vector2(0.5f, 0.5f), new Vector2(260, -80), new Vector2(300, 300));
            RectTransform rightMascotRT = rightMascotObj.GetComponent<RectTransform>();
            rightMascotRT.localScale = Vector3.zero;

            // Blinking "화면을 터치해주세요" Prompt at Bottom
            GameObject touchPromptObj = new GameObject("TouchPromptGroup", typeof(RectTransform), typeof(CanvasGroup));
            touchPromptObj.transform.SetParent(mainMenu.transform, false);
            SetRect(touchPromptObj, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 140), new Vector2(650, 80));
            CanvasGroup touchPromptCG = touchPromptObj.GetComponent<CanvasGroup>();
            touchPromptCG.alpha = 0f;

            GameObject touchText = CreateText(touchPromptObj.transform, "TouchText", LocalizationManager.Get("intro_touch"), 40, TextAlignmentOptions.Center, cuteFont, new Color(1f, 1f, 1f, 0.95f));
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
            cinematicCtrl.SetupLanguageLogos(logoImg, langLogoSprites);

            AudioClip sfxMascot = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/item_pick_up_04.wav");
            if (sfxMascot == null) sfxMascot = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/CelerisLab/CompleteUISFX/inventory_and_item_management/item_pick_up_04.wav");
            AudioClip sfxPink = sfxMascot;
            AudioClip sfxMint = sfxMascot;
            AudioClip sfxLogo = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/item_acquired_04.wav");
            if (sfxLogo == null) sfxLogo = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/CelerisLab/CompleteUISFX/positive_feedback_and_success/item_acquired_04.wav");
            cinematicCtrl.SetupCinematicAudio(sfxPink, sfxMint, sfxLogo);
            cinematicCtrl.ResetToPreIntroState();
            EditorUtility.SetDirty(cinematicCtrl);

            uiMgr.SetupMainMenu(mainMenu, null, null);

            // --- F-1. Boot Splash Screen Overlay (Mallang Games Studio Logo) ---
            GameObject splashOverlay = new GameObject("SplashScreenOverlay", typeof(RectTransform), typeof(CanvasGroup), typeof(Image), typeof(SplashScreenController));
            splashOverlay.transform.SetParent(canvasObj.transform, false);
            SetRect(splashOverlay, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image splashBgImg = splashOverlay.GetComponent<Image>();
            splashBgImg.color = new Color(0.99f, 0.97f, 0.98f, 1f); // Warm creamy pastel white
            splashBgImg.raycastTarget = true; // Blocks touches to main menu during splash

            CanvasGroup splashCG = splashOverlay.GetComponent<CanvasGroup>();
            splashCG.alpha = 1f;

            Sprite studioLogoSprite = CuteBlockTextureGenerator.GetOrCreateMallangGamesStudioLogoSprite();
            GameObject studioLogoObj = new GameObject("StudioLogo", typeof(RectTransform), typeof(Image));
            studioLogoObj.transform.SetParent(splashOverlay.transform, false);
            SetRect(studioLogoObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860f, 860f));
            Image studioLogoImg = studioLogoObj.GetComponent<Image>();
            studioLogoImg.sprite = studioLogoSprite;
            studioLogoImg.preserveAspect = true;
            studioLogoImg.raycastTarget = false;

            SplashScreenController splashCtrl = splashOverlay.GetComponent<SplashScreenController>();
            splashCtrl.SetupReferences(splashCG, studioLogoObj.GetComponent<RectTransform>(), studioLogoImg, splashBgImg, cinematicCtrl);

            AudioClip splashSfxClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/item_acquired_06.wav");
            if (splashSfxClip == null) splashSfxClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/CelerisLab/CompleteUISFX/positive_feedback_and_success/item_acquired_06.wav");
            if (splashSfxClip != null)
            {
                splashCtrl.SetupAudio(splashSfxClip);
            }
            EditorUtility.SetDirty(splashOverlay);

            // --- G. Cute Mallang Party Lobby Screen ---
            CuteBlockTextureGenerator.EnsureMascotsCutout();

            circleFrameSprite = CuteBlockTextureGenerator.GetOrCreateCircleFrameSprite();
            tabPillSprite = CuteBlockTextureGenerator.GetOrCreateTabPillSprite();
            cuteCardSprite = CuteBlockTextureGenerator.GetOrCreateCuteCardSprite();

            Sprite pinkMascotSprite = CuteBlockTextureGenerator.GetOrCreatePinkMascotSprite();
            Sprite mintMascotSprite = CuteBlockTextureGenerator.GetOrCreateMintMascotSprite();
            Sprite goldMascotSprite = CuteBlockTextureGenerator.GetOrCreateGoldMascotSprite();
            Sprite purpleMascotSprite = CuteBlockTextureGenerator.GetOrCreatePurpleMascotSprite();
            Sprite closeXBtnSprite = CuteBlockTextureGenerator.GetOrCreateCloseXButtonSprite();
            Sprite whiteCirclePlateSprite = CuteBlockTextureGenerator.GetOrCreateWhiteCircleSprite(false);
            Sprite googleIconSprite = CuteBlockTextureGenerator.GetOrCreateGoogleIconSprite();
            Sprite googleBtnSprite = CuteBlockTextureGenerator.GetOrCreateGoogleLoginButtonSprite();
            // 2048x2048 Lossless Stage Mascots (Clean cutouts without plate)
            Sprite[] stageMascots = new Sprite[] { pinkMascotSprite, mintMascotSprite, goldMascotSprite, purpleMascotSprite };

            // 2048x2048 Lossless Avatars with pure white circular plate backing
            Sprite[] highResAvatars = CuteBlockTextureGenerator.GetOrCreateAllHighResAvatarSprites(false);
            Sprite[] mascotAvatars = highResAvatars;

            GameObject lobbyRoot = new GameObject("LobbyScreen", typeof(RectTransform), typeof(CanvasGroup));
            lobbyRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(lobbyRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1080f, 0f));
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

            // Top-Right: Currencies (Diamond + Coin Badges)
            Sprite diamondGemSprite = CuteBlockTextureGenerator.GetOrCreateDiamondIconSprite();
            goldCoinSprite = CuteBlockTextureGenerator.GetOrCreateGoldCoinSprite();
            Sprite coinBoxSprite = CuteBlockTextureGenerator.GetOrCreateLobbyCoinBoxSprite();

            // Diamond Currency Rounded Box (Diamond icon + 10 text to the left of Coin Badge)
            GameObject diamondBadge = CreateImage(lobbyRoot.transform, "DiamondBadge", coinBoxSprite, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-475, -80), new Vector2(200, 72));
            Image diaBgImg = diamondBadge.GetComponent<Image>();
            diaBgImg.type = Image.Type.Sliced;
            diaBgImg.color = Color.white;

            GameObject diaIconObj = CreateImage(diamondBadge.transform, "DiaIcon", diamondGemSprite, new Vector2(0, 0.5f), new Vector2(30, 0), new Vector2(46, 46));
            diaIconObj.GetComponent<Image>().preserveAspect = true;

            GameObject diaLabel = CreateText(diamondBadge.transform, "Diamonds", "10", 34, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.95f, 1f));
            SetRect(diaLabel, new Vector2(0, 0), new Vector2(1, 1), new Vector2(60, 0), new Vector2(-10, 0));

            // Polished Currency Rounded Box (Coin icon + 100 text in elegant rounded rectangle)
            GameObject coinBadge = CreateImage(lobbyRoot.transform, "CoinBadge", coinBoxSprite, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-265, -80), new Vector2(200, 72));
            Image coinBgImg = coinBadge.GetComponent<Image>();
            coinBgImg.type = Image.Type.Sliced;
            coinBgImg.color = Color.white; // Texture has baked translucent glass & border

            GameObject coinIconObj = CreateImage(coinBadge.transform, "CoinIcon", goldCoinSprite, new Vector2(0, 0.5f), new Vector2(30, 0), new Vector2(46, 46));
            coinIconObj.GetComponent<Image>().preserveAspect = true;

            GameObject coinLabel = CreateText(coinBadge.transform, "Coins", "100", 34, TextAlignmentOptions.Left, cuteFont, new Color(1f, 0.90f, 0.45f));
            SetRect(coinLabel, new Vector2(0, 0), new Vector2(1, 1), new Vector2(60, 0), new Vector2(-10, 0));

            // Top-Right: Profile Circle Button (Enlarged for touch & visual balance)
            GameObject profileBtnObj = new GameObject("ProfileButton", typeof(RectTransform), typeof(Image), typeof(Button));
            profileBtnObj.transform.SetParent(lobbyRoot.transform, false);
            SetRect(profileBtnObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-75, -80), new Vector2(114, 114));
            Image pBtnImg = profileBtnObj.GetComponent<Image>();
            pBtnImg.sprite = circleFrameSprite;
            Button pBtn = profileBtnObj.GetComponent<Button>();

            // High-res 2048 avatar (with pure white circular plate built-in)
            GameObject pAvatarObj = CreateImage(profileBtnObj.transform, "AvatarIcon", highResAvatars[0], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(104, 104));
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
            string[] floatingLabels = new string[] { "게임 시작", "상점", "말랑이", "설정" };
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
                GameObject mObj = CreateImage(mSlot.transform, $"Mascot_{i}", stageMascots[i], new Vector2(0.5f, 0.5f), Vector2.zero, mascotSizes[i]);
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
            GameObject partyTip = CreateText(partyStage.transform, "PartyTip", LocalizationManager.Get("lobby_party_tip"), 28, TextAlignmentOptions.Center, cuteFont, new Color(0.42f, 0.30f, 0.55f, 0.95f));
            SetRect(partyTip, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 115), new Vector2(600, 45));

            // Dynamic Bottom Action Button (Glossy 3D Fairy Capsule)
            Sprite actionBtnSprite = CuteBlockTextureGenerator.GetOrCreateFairyActionButtonSprite();

            GameObject playBtnObj = CreateButton(lobbyRoot.transform, "BtnBottomAction", "게임 시작!", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 165), new Vector2(500, 120), actionBtnSprite, 44);
            Button playBtn = playBtnObj.GetComponent<Button>();
            Image playBtnBg = playBtnObj.GetComponent<Image>();
            playBtnBg.type = Image.Type.Sliced;
            playBtnBg.preserveAspect = false;
            playBtnBg.color = new Color(1f, 0.33f, 0.53f, 1f); // #FF5588 default pink

            // Soft glowing aura behind the action button
            GameObject playGlowObj = CreateImage(playBtnObj.transform, "GlowAura", glowOrbSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(580, 170));
            playGlowObj.transform.SetAsFirstSibling();
            Image playGlowImg = playGlowObj.GetComponent<Image>();
            playGlowImg.color = new Color(1f, 0.33f, 0.53f, 0.45f);
            playGlowImg.raycastTarget = false;

            TMP_Text playBtnText = playBtnObj.GetComponentInChildren<TMP_Text>();
            if (playBtnText != null)
            {
                playBtnText.fontStyle = FontStyles.Bold;
                playBtnText.enableWordWrapping = false;
                playBtnText.overflowMode = TextOverflowModes.Overflow;
            }

            // --- Profile Modal ---
            GameObject pModal = new GameObject("ProfileModal", typeof(RectTransform));
            pModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(pModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject pDarkBg = CreateImage(pModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            pDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            pDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject pCard = CreateImage(pModal.transform, "DialogCard", profileModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820, 1060));
            pCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject pCloseBtn = CreateButton(pCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject pTitle = CreateText(pCard.transform, "Title", "내 프로필", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(pTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            // Top Section (Left: Avatar, Right: Name & Tag)
            // Left: Big Avatar (170x170) with pure white circular backing + high-res cutout mascot
            GameObject pBigAvRoot = new GameObject("AvatarGroup", typeof(RectTransform));
            pBigAvRoot.transform.SetParent(pCard.transform, false);
            SetRect(pBigAvRoot, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-225, -200), new Vector2(170, 170));

            // 1. High-res cutout mascot icon with pure white circular plate (2048x2048)
            GameObject pBigAv = CreateImage(pBigAvRoot.transform, "AvIcon", highResAvatars[0], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(160, 160));
            Image pBigAvImg = pBigAv.GetComponent<Image>();
            pBigAvImg.preserveAspect = true;
            // 2. Circle Frame Ring
            GameObject pBigAvFrame = CreateImage(pBigAvRoot.transform, "AvFrame", avatarCircleFrameSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(170, 170));

            // Right: Name and Tag Section
            GameObject pRightInfo = new GameObject("ProfileInfoRight", typeof(RectTransform));
            pRightInfo.transform.SetParent(pCard.transform, false);
            SetRect(pRightInfo, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(130, -195), new Vector2(440, 200));

            // Nickname row: Label + Input
            GameObject pNickLbl = CreateText(pRightInfo.transform, "NickLbl", "닉네임", 22, TextAlignmentOptions.Left, cuteFont, new Color(0.42f, 0.30f, 0.56f));
            SetRect(pNickLbl, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(40, 52), new Vector2(80, 36));

            GameObject pNickInputObj = CreateInputField(pRightInfo.transform, "NickInput", "말랑이", "닉네임", cuteFont, new Vector2(65, 52), new Vector2(270, 48), inputPillSprite);
            TMP_InputField pNickInput = pNickInputObj.GetComponent<TMP_InputField>();
            pNickInput.characterLimit = 10;
            pNickInput.interactable = false;
            if (pNickInput.textComponent != null)
            {
                pNickInput.textComponent.alignment = TextAlignmentOptions.Left;
                pNickInput.textComponent.fontSize = 24;
                pNickInput.textComponent.color = new Color(0.25f, 0.15f, 0.45f);
                pNickInput.textComponent.margin = new Vector4(16, 0, 12, 0);
            }

            // Tag row: Label + Input
            GameObject pTagLbl = CreateText(pRightInfo.transform, "TagLbl", "태그", 22, TextAlignmentOptions.Left, cuteFont, new Color(0.42f, 0.30f, 0.56f));
            SetRect(pTagLbl, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(40, 0), new Vector2(80, 36));

            GameObject pTagInputObj = CreateInputField(pRightInfo.transform, "TagInput", "8276", "#0000", cuteFont, new Vector2(-15, 0), new Vector2(110, 48), inputPillSprite);
            TMP_InputField pTagInput = pTagInputObj.GetComponent<TMP_InputField>();
            pTagInput.characterLimit = 5;
            pTagInput.interactable = false;
            if (pTagInput.textComponent != null)
            {
                pTagInput.textComponent.alignment = TextAlignmentOptions.Center;
                pTagInput.textComponent.fontSize = 24;
                pTagInput.textComponent.color = new Color(0.45f, 0.25f, 0.65f);
            }

            // Buttons row:
            // 1) Mode A: "닉네임 변경" Start Edit Button
            GameObject pStartEditBtnObj = CreateButton(pRightInfo.transform, "BtnStartEditNick", "닉네임 변경", cuteFont, new Vector2(0.5f, 0.5f), new Vector2(65, -52), new Vector2(270, 46), btnLavenderSprite, 22, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            Button pStartEditBtn = pStartEditBtnObj.GetComponent<Button>();

            // 2) Mode B: "중복 확인" Button
            GameObject pCheckBtnObj = CreateButton(pRightInfo.transform, "BtnCheckTag", "중복 확인", cuteFont, new Vector2(0.5f, 0.5f), new Vector2(-10, -52), new Vector2(120, 46), btnLavenderSprite, 20, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            Button pCheckBtn = pCheckBtnObj.GetComponent<Button>();
            pCheckBtnObj.SetActive(false);

            // 3) Mode B: "변경 완료" Confirm Button (Requires successful duplicate check)
            GameObject pConfirmBtnObj = CreateButton(pRightInfo.transform, "BtnConfirmNick", "변경 완료", cuteFont, new Vector2(0.5f, 0.5f), new Vector2(140, -52), new Vector2(140, 46), btnTealSprite, 20, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
            Button pConfirmBtn = pConfirmBtnObj.GetComponent<Button>();
            pConfirmBtnObj.SetActive(false);

            // Duplicate Status Message:
            GameObject pStatusObj = CreateText(pRightInfo.transform, "NickStatus", "", 18, TextAlignmentOptions.Center, cuteFont, new Color(0.18f, 0.82f, 0.45f));
            SetRect(pStatusObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(20, -10), new Vector2(420, 24));
            TMP_Text pStatusTxt = pStatusObj.GetComponent<TMP_Text>();

            // Stats row (Best score ONLY - "보유 코인" REMOVED as requested!)
            GameObject pStats = CreateText(pCard.transform, "Stats", "최고 점수: 0점", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.45f, 0.05f));
            SetRect(pStats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -320), new Vector2(650, 38));

            // Bio Section
            GameObject pBioLbl = CreateText(pCard.transform, "BioLbl", "자기소개 (터치하여 수정)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.35f, 0.55f));
            SetRect(pBioLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -375), new Vector2(680, 35));

            GameObject pBioInputObj = CreateInputField(pCard.transform, "BioInput", "말랑블라스트에 오신 걸 환영해요!", "자기소개를 입력해주세요", cuteFont, new Vector2(0, 75), new Vector2(680, 80), inputPillSprite);
            TMP_InputField pBioInput = pBioInputObj.GetComponent<TMP_InputField>();

            // Avatar Picker Section (5 Mascots - Pink, Mint, Gold, Purple, Special!)
            GameObject pPickLbl = CreateText(pCard.transform, "PickLbl", "프로필 꾸미기 (말랑이 선택)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.35f, 0.55f));
            SetRect(pPickLbl, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -25), new Vector2(680, 35));

            Button[] avPickBtns = new Button[5];
            float[] avXPositions = new float[] { -260f, -130f, 0f, 130f, 260f };
            for (int i = 0; i < 5; i++)
            {
                GameObject avBtnObj = CreateButton(pCard.transform, $"AvBtn_{i}", "", cuteFont, new Vector2(0.5f, 0.5f), new Vector2(avXPositions[i], -125), new Vector2(120, 120), null);
                Image btnBaseImg = avBtnObj.GetComponent<Image>();
                if (btnBaseImg != null) btnBaseImg.color = Color.clear;

                // 1. High-res cutout mascot with pure white circular plate (2048x2048)
                Sprite avSp = (mascotAvatars != null && i < mascotAvatars.Length) ? mascotAvatars[i] : null;
                GameObject iconObj = CreateImage(avBtnObj.transform, "Icon", avSp, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(114, 114));
                iconObj.GetComponent<Image>().preserveAspect = true;
                // 2. Ring Frame on top (Hollow delicate ring frame)
                CreateImage(avBtnObj.transform, "Frame", avatarCircleFrameSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(120, 120));

                avPickBtns[i] = avBtnObj.GetComponent<Button>();
            }

            // Bottom Login / Logout Action Button (Shows "로그인" by default, or "로그아웃")
            GameObject pLogoutObj = CreateButton(pCard.transform, "BtnLogout", "로그인", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 90), new Vector2(360, 76), btnLavenderSprite, 32, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            Button pLogoutBtn = pLogoutObj.GetComponent<Button>();

            // --- Login Modal (ID/PW & Google OAuth Sign-In Popup) ---
            GameObject lModal = new GameObject("LoginModal", typeof(RectTransform));
            lModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(lModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            lModal.SetActive(false);

            GameObject lDarkBg = CreateImage(lModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            lDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            lDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject loginCard = CreateImage(lModal.transform, "DialogCard", profileModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(740, 880));
            loginCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject lCloseBtn = CreateButton(loginCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject loginTitle = CreateText(loginCard.transform, "Title", "로그인", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(loginTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(400, 50));

            GameObject lSubtitle = CreateText(loginCard.transform, "Subtitle", "말랑블라스트 계정으로 플레이 기록을 안전하게 보관하세요", 22, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.45f, 0.65f));
            SetRect(lSubtitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -115), new Vector2(600, 35));

            // ID / Email Field
            GameObject lIdLbl = CreateText(loginCard.transform, "IdLbl", "아이디 또는 구글 이메일", 24, TextAlignmentOptions.Left, cuteFont, new Color(0.42f, 0.30f, 0.56f));
            SetRect(lIdLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -165), new Vector2(560, 32));

            GameObject lIdInputObj = CreateInputField(loginCard.transform, "IdInput", "", "example@gmail.com", cuteFont, Vector2.zero, new Vector2(560, 60), inputPillSprite);
            SetRect(lIdInputObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -220), new Vector2(560, 60));
            TMP_InputField lIdInput = lIdInputObj.GetComponent<TMP_InputField>();

            // Password Field
            GameObject lPwLbl = CreateText(loginCard.transform, "PwLbl", "비밀번호", 24, TextAlignmentOptions.Left, cuteFont, new Color(0.42f, 0.30f, 0.56f));
            SetRect(lPwLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -280), new Vector2(560, 32));

            GameObject lPwInputObj = CreateInputField(loginCard.transform, "PwInput", "", "비밀번호를 입력하세요", cuteFont, Vector2.zero, new Vector2(560, 60), inputPillSprite);
            SetRect(lPwInputObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -335), new Vector2(560, 60));
            TMP_InputField lPwInput = lPwInputObj.GetComponent<TMP_InputField>();
            lPwInput.contentType = TMP_InputField.ContentType.Password;

            // Login Status / Message
            GameObject lStatObj = CreateText(loginCard.transform, "LoginStatus", "", 20, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.35f, 0.45f));
            SetRect(lStatObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -400), new Vector2(560, 30));
            TMP_Text lStatTxt = lStatObj.GetComponent<TMP_Text>();

            // Submit Button
            GameObject lSubBtnObj = CreateButton(loginCard.transform, "BtnSubmitLogin", "로그인", cuteFont, new Vector2(0.5f, 1), new Vector2(0, -460), new Vector2(560, 68), btnLavenderSprite, 28, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            Button lSubBtn = lSubBtnObj.GetComponent<Button>();

            // Divider: "────── 또는 간편 로그인 ──────"
            GameObject lDivider = CreateText(loginCard.transform, "Divider", "────────   또는 간편 로그인   ────────", 20, TextAlignmentOptions.Center, cuteFont, new Color(0.68f, 0.60f, 0.78f));
            SetRect(lDivider, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -545), new Vector2(560, 30));

            // Google OAuth Button (White card + Google 'G' icon + "Google 계정으로 로그인")
            GameObject lGglBtnObj = CreateButton(loginCard.transform, "BtnGoogleLogin", "    Google 계정으로 로그인", cuteFont, new Vector2(0.5f, 1), new Vector2(0, -620), new Vector2(560, 72), googleBtnSprite, 26, new Color(0.24f, 0.25f, 0.28f), new Color(0.92f, 0.93f, 0.96f));
            Button lGglBtn = lGglBtnObj.GetComponent<Button>();

            // Google 'G' icon on the left
            GameObject lGglIcon = CreateImage(lGglBtnObj.transform, "GoogleIcon", googleIconSprite, new Vector2(0, 0.5f), new Vector2(48, 0), new Vector2(40, 40));
            lGglIcon.GetComponent<Image>().preserveAspect = true;

            // --- Shop Modal (Commercial Mobile Game Store Revamp) ---
            Sprite themeCandySprite = CuteBlockTextureGenerator.GetOrCreateCandyWonderlandBackgroundSprite();
            Sprite themeMermaidSprite = CuteBlockTextureGenerator.GetOrCreateCrystalMermaidBackgroundSprite();
            Sprite themeNebulaSprite = CuteBlockTextureGenerator.GetOrCreateStarryNebulaBackgroundSprite();
            Sprite[] allThemeSprites = new Sprite[] { bgSprite, themeCandySprite, themeMermaidSprite, themeNebulaSprite };

            Sprite lobbyCandySprite = CuteBlockTextureGenerator.GetOrCreateLobbyCandyStageSprite();
            Sprite lobbyOceanSprite = CuteBlockTextureGenerator.GetOrCreateLobbyOceanStageSprite();
            Sprite[] allLobbySprites = new Sprite[] { lobbyBgSprite, lobbyCandySprite, lobbyOceanSprite };

            Sprite tabVerticalActiveSprite = CuteBlockTextureGenerator.GetOrCreateShopVerticalTabActiveSprite();
            Sprite tabVerticalInactiveSprite = CuteBlockTextureGenerator.GetOrCreateShopVerticalTabInactiveSprite();
            Sprite bannerNewMascotSprite = CuteBlockTextureGenerator.GetOrCreateShopBannerNewMascotSprite();
            CuteBlockTextureGenerator.GetOrCreateShopBannerPickupSprite(true);
            Sprite bannerPickupSprite = CuteBlockTextureGenerator.GetOrCreateShopBannerPickupSprite();
            Sprite packageCardSprite = CuteBlockTextureGenerator.GetOrCreateShopPackageCardSprite();
            Sprite shootingStarSprite = CuteBlockTextureGenerator.GetOrCreateShootingStarSprite();
            Sprite sunburstSprite = CuteBlockTextureGenerator.GetOrCreateGachaSunburstSprite();
            Sprite rainbowAuraSprite = CuteBlockTextureGenerator.GetOrCreateGachaRainbowAuraSprite();
            Sprite fairySparkleSprite = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();
            Sprite specialMascotCutoutSprite = CuteBlockTextureGenerator.GetOrCreateSpecialMascotSprite();

            GameObject sModal = new GameObject("ShopModal", typeof(RectTransform));
            sModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(sModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject sDarkBg = CreateImage(sModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            sDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            sDarkBg.GetComponent<Image>().raycastTarget = true;

            // Shop Card (Dimensions for mobile commercial shop: 1000 x 1560)
            GameObject sCard = CreateImage(sModal.transform, "DialogCard", shopModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1000, 1560));
            sCard.GetComponent<Image>().type = Image.Type.Sliced;

            // --- Top Header ---
            GameObject sTitle = CreateText(sCard.transform, "Title", "말랑 상점", 40, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sTitle, new Vector2(0, 1), new Vector2(0, 1), new Vector2(175, -50), new Vector2(250, 50));

            // Top-Right: Diamond Badge in Shop
            GameObject sDiaBadge = CreateImage(sCard.transform, "DiaBadge", coinBoxSprite, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-360, -50), new Vector2(165, 58));
            sDiaBadge.GetComponent<Image>().type = Image.Type.Sliced;
            GameObject sDiaIcon = CreateImage(sDiaBadge.transform, "Icon", diamondGemSprite, new Vector2(0, 0.5f), new Vector2(24, 0), new Vector2(38, 38));
            sDiaIcon.GetComponent<Image>().preserveAspect = true;
            GameObject sDiaTxt = CreateText(sDiaBadge.transform, "Diamonds", "10", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.95f, 1f));
            SetRect(sDiaTxt, new Vector2(0, 0), new Vector2(1, 1), new Vector2(50, 0), new Vector2(-8, 0));

            // Top-Right: Gold Badge in Shop
            GameObject sCoinBadge = CreateImage(sCard.transform, "CoinBadge", coinBoxSprite, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-185, -50), new Vector2(165, 58));
            sCoinBadge.GetComponent<Image>().type = Image.Type.Sliced;
            GameObject sCoinIcon = CreateImage(sCoinBadge.transform, "Icon", goldCoinSprite, new Vector2(0, 0.5f), new Vector2(24, 0), new Vector2(38, 38));
            sCoinIcon.GetComponent<Image>().preserveAspect = true;
            GameObject sCoinTxt = CreateText(sCoinBadge.transform, "Coins", "100 G", 28, TextAlignmentOptions.Left, cuteFont, new Color(1f, 0.90f, 0.45f));
            SetRect(sCoinTxt, new Vector2(0, 0), new Vector2(1, 1), new Vector2(50, 0), new Vector2(-8, 0));

            // Close Button
            GameObject sCloseBtn = CreateButton(sCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -50), new Vector2(60, 60), closeXBtnSprite, 30);

            // --- Top Horizontal Tab Bar: 5 Tabs ---
            GameObject tabHeaderBar = new GameObject("TabHeaderBar", typeof(RectTransform));
            tabHeaderBar.transform.SetParent(sCard.transform, false);
            SetRect(tabHeaderBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -125), new Vector2(920, 68));

            string[] tabNames = new string[] { "추천", "픽업", "말랑이", "게임 배경", "로비 배경" };
            Button[] shopTabBtns = new Button[5];
            Image[] shopTabBgs = new Image[5];
            TMP_Text[] shopTabTexts = new TMP_Text[5];
            float[] tabXOffsets = new float[] { -352f, -176f, 0f, 176f, 352f };

            for (int t = 0; t < 5; t++)
            {
                Sprite initTabSp = (t == 0) ? tabVerticalActiveSprite : tabVerticalInactiveSprite;
                GameObject tBtnObj = CreateButton(tabHeaderBar.transform, $"Tab_{t}", tabNames[t], cuteFont, new Vector2(0.5f, 0.5f), new Vector2(tabXOffsets[t], 0), new Vector2(170, 62), initTabSp, 24);
                shopTabBtns[t] = tBtnObj.GetComponent<Button>();
                shopTabBgs[t] = tBtnObj.GetComponent<Image>();
                shopTabBgs[t].type = Image.Type.Sliced;
                shopTabTexts[t] = tBtnObj.GetComponentInChildren<TMP_Text>();
                if (shopTabTexts[t] != null)
                {
                    shopTabTexts[t].color = (t == 0) ? Color.white : new Color(0.40f, 0.30f, 0.55f, 1f);
                    shopTabTexts[t].fontStyle = (t == 0) ? FontStyles.Bold : FontStyles.Normal;
                }
            }

            // --- Main Content Container (Full 920px width below tabs) ---
            GameObject contentContainer = new GameObject("ContentContainer", typeof(RectTransform));
            contentContainer.transform.SetParent(sCard.transform, false);
            SetRect(contentContainer, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -840), new Vector2(920, 1340));

            // ==========================================
            // PANEL 0: 추천 (RECOMMENDED)
            // ==========================================
            GameObject recPanel = new GameObject("RecommendedPanel", typeof(RectTransform));
            recPanel.transform.SetParent(contentContainer.transform, false);
            SetRect(recPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Huge Top Banner: "신규 말랑이 출시!" (Width 880, Height 480)
            GameObject newMascotBanner = CreateImage(recPanel.transform, "NewMascotBanner", bannerNewMascotSprite, new Vector2(0.5f, 1), new Vector2(0, -250), new Vector2(880, 480));
            newMascotBanner.GetComponent<Image>().preserveAspect = false;

            // Section Subtitle
            GameObject packSub = CreateText(recPanel.transform, "PackSubtitle", "이달의 특별 한정 추천 패키지", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(packSub, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -515), new Vector2(860, 36));

            // 3 Package Cards Row (Width 275 each, full width 880)
            Button[] packageBtns = new Button[3];
            string[] packTitles = new string[] { "스타터 팩", "민트 웰컴 팩", "골드 대박 팩" };
            string[] packRewards = new string[] { "+1,500 G\n+30 다이아", "민트 말랑이\n+1,000 G", "+5,000 G\n특가 찬스!" };
            string[] packPrices = new string[] { "20 다이아 구매", "50 다이아 구매", "80 다이아 구매" };
            float[] packXOffsets = new float[] { -295f, 0f, 295f };
            Sprite[] packIcons = new Sprite[] { goldCoinSprite, highResAvatars[1], goldCoinSprite };

            for (int p = 0; p < 3; p++)
            {
                GameObject pCardObj = CreateImage(recPanel.transform, $"PackageCard_{p}", packageCardSprite, new Vector2(0.5f, 1), new Vector2(packXOffsets[p], -750), new Vector2(275, 420));
                pCardObj.GetComponent<Image>().type = Image.Type.Sliced;

                // Card Title
                GameObject pTitleTxt = CreateText(pCardObj.transform, "Title", packTitles[p], 24, TextAlignmentOptions.Center, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(pTitleTxt, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -32), new Vector2(250, 30));

                // Icon Frame
                GameObject pIconFrame = CreateImage(pCardObj.transform, "IconFrame", circleFrameSprite, new Vector2(0.5f, 1), new Vector2(0, -120), new Vector2(115, 115));
                GameObject pIconImg = CreateImage(pIconFrame.transform, "Icon", packIcons[p], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(95, 95));
                pIconImg.GetComponent<Image>().preserveAspect = true;

                // Reward Text
                GameObject pRewardTxt = CreateText(pCardObj.transform, "Rewards", packRewards[p], 23, TextAlignmentOptions.Center, cuteFont, new Color(0.45f, 0.35f, 0.60f));
                SetRect(pRewardTxt, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -220), new Vector2(250, 58));

                // Buy Button
                GameObject pBtnObj = CreateButton(pCardObj.transform, "BtnBuy", packPrices[p], cuteFont, new Vector2(0.5f, 0), new Vector2(0, 45), new Vector2(235, 65), btnPinkSprite, 23, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
                packageBtns[p] = pBtnObj.GetComponent<Button>();
            }

            // Bottom Info Strip in Recommended Panel
            GameObject recTipCard = CreateImage(recPanel.transform, "RecTipCard", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -1035), new Vector2(880, 110));
            recTipCard.GetComponent<Image>().type = Image.Type.Sliced;
            recTipCard.GetComponent<Image>().color = new Color(0.95f, 0.92f, 1f, 0.92f);

            GameObject recTipTxt = CreateText(recTipCard.transform, "TipTxt", "TIP: 다이아몬드는 매일 퀘스트 및 업적 달성 시에도 무료로 획득할 수 있습니다!\n스페셜 말랑이 픽업 소환으로 판을 시원하게 쓸어담아 보세요!", 21, TextAlignmentOptions.Center, cuteFont, new Color(0.38f, 0.25f, 0.60f));
            SetRect(recTipTxt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // ==========================================
            // PANEL 1: 픽업 소환 (PICKUP SUMMON)
            // ==========================================
            GameObject pickPanel = new GameObject("PickupPanel", typeof(RectTransform));
            pickPanel.transform.SetParent(contentContainer.transform, false);
            SetRect(pickPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            pickPanel.SetActive(false);

            // Huge Celestial Animated Pickup Banner (16:9 uncompressed widescreen: width 880, height 495)
            GameObject pickupBanner = CreateImage(pickPanel.transform, "PickupBanner", bannerPickupSprite, new Vector2(0.5f, 1), new Vector2(0, -255), new Vector2(880, 495));
            pickupBanner.GetComponent<Image>().preserveAspect = false;

            // Live Animation Layer 1: Sunburst Rays behind mascot
            GameObject bSunburst = CreateImage(pickupBanner.transform, "SunburstRays", sunburstSprite, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(440, 440));
            bSunburst.GetComponent<Image>().color = new Color(1f, 0.90f, 0.50f, 0.55f);

            // Live Animation Layer 2: Rainbow Aura
            GameObject bRainbowAura = CreateImage(pickupBanner.transform, "RainbowAura", rainbowAuraSprite, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(330, 330));
            bRainbowAura.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.70f);

            // Live Animation Layer 3: Live Special Mascot Cutout (Levitation, Breathing Squash & Tilt)
            GameObject bLiveMascot = CreateImage(pickupBanner.transform, "LiveMascot", specialMascotCutoutSprite, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(360, 360));
            bLiveMascot.GetComponent<Image>().preserveAspect = true;

            // Live Animation Layer 4: Sparkle Particles Container
            GameObject bSparkleRoot = new GameObject("SparkleContainer", typeof(RectTransform));
            bSparkleRoot.transform.SetParent(pickupBanner.transform, false);
            SetRect(bSparkleRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Live Animation Layer 5: Shooting Star Container (Masked to banner)
            GameObject bStarRoot = new GameObject("ShootingStarContainer", typeof(RectTransform), typeof(RectMask2D));
            bStarRoot.transform.SetParent(pickupBanner.transform, false);
            SetRect(bStarRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Live Animation Layer 6: Video Player & Overlay (mallang_pick1 Video Support)
            GameObject bVideoOverlay = new GameObject("VideoOverlay", typeof(RectTransform), typeof(RawImage), typeof(UnityEngine.Video.VideoPlayer), typeof(CanvasGroup));
            bVideoOverlay.transform.SetParent(pickupBanner.transform, false);
            SetRect(bVideoOverlay, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RawImage vRawImg = bVideoOverlay.GetComponent<RawImage>();
            vRawImg.raycastTarget = true;
            bVideoOverlay.SetActive(false);

            VideoClip pickVideoClip = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/movies/mallang_pick1.mp4");

            // Cute skip pill button on top right
            GameObject btnSkipVideo = CreateButton(bVideoOverlay.transform, "BtnSkipVideo", "스킵 >>", cuteFont, new Vector2(1, 1), new Vector2(-75, -40), new Vector2(115, 46), tabPillSprite, 20, Color.white, new Color(0.2f, 0.1f, 0.3f, 0.8f));

            // Attach & Wire AnimatedPickupBannerController
            var bannerCtrl = pickupBanner.AddComponent<AnimatedPickupBannerController>();
            bannerCtrl.SetupReferences(
                pickupBanner.GetComponent<Image>(),
                bLiveMascot.GetComponent<RectTransform>(), bLiveMascot.GetComponent<Image>(),
                bSunburst.GetComponent<RectTransform>(), bSunburst.GetComponent<Image>(),
                bRainbowAura.GetComponent<RectTransform>(), bRainbowAura.GetComponent<Image>(),
                bStarRoot.GetComponent<RectTransform>(), shootingStarSprite,
                bSparkleRoot.GetComponent<RectTransform>(), fairySparkleSprite,
                bVideoOverlay.GetComponent<UnityEngine.Video.VideoPlayer>(), vRawImg,
                pickVideoClip, btnSkipVideo.GetComponent<Button>()
            );

            // Pickup Info Box & Legal Probability Button
            GameObject pickInfoCard = CreateImage(pickPanel.transform, "InfoCard", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -560), new Vector2(880, 90));
            pickInfoCard.GetComponent<Image>().type = Image.Type.Sliced;
            pickInfoCard.GetComponent<Image>().color = new Color(0.95f, 0.92f, 1f, 0.95f);

            GameObject pickInfoTxt = CreateText(pickInfoCard.transform, "InfoTxt", "[0.1% 확률] 스페셜 말랑이 픽업 소환!\n중복 획득 시 60조각 즉시 지급 (바로 돌파 가능!)\n기본 보상: 일반 말랑이 조각 5개 지급 (능력 업그레이드)", 19, TextAlignmentOptions.Left, cuteFont, new Color(0.38f, 0.25f, 0.60f));
            SetRect(pickInfoTxt, new Vector2(0, 0), new Vector2(1, 1), new Vector2(25, 0), new Vector2(-185, 0));

            GameObject btnProbCheck = CreateButton(pickInfoCard.transform, "BtnProbCheck", "확률 정보", cuteFont, new Vector2(1, 0.5f), new Vector2(-95, 0), new Vector2(145, 54), btnTealSprite, 20, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
            btnProbCheck.GetComponent<Button>().onClick.AddListener(lobbyMgr.OpenProbabilityModal);

            // Special Mascot Skill Feature Showcase Box
            GameObject featBox = CreateImage(pickPanel.transform, "MascotShowcaseBox", packageCardSprite, new Vector2(0.5f, 1), new Vector2(0, -780), new Vector2(880, 310));
            featBox.GetComponent<Image>().type = Image.Type.Sliced;
            featBox.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.98f);

            GameObject featHeader = CreateText(featBox.transform, "Header", "★ [올 클리어 엔젤] 스페셜 말랑이 전용 능력 미리보기 ★", 25, TextAlignmentOptions.Center, cuteFont, new Color(0.38f, 0.18f, 0.55f));
            SetRect(featHeader, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -28), new Vector2(850, 32));

            // Avatar Frame & Cutout Icon
            GameObject fAvFrame = CreateImage(featBox.transform, "AvFrame", circleFrameSprite, new Vector2(0, 0.5f), new Vector2(110, -15), new Vector2(150, 150));
            GameObject fAvIcon = CreateImage(fAvFrame.transform, "Icon", specialMascotCutoutSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(130, 130));
            fAvIcon.GetComponent<Image>().preserveAspect = true;

            // Skill details on the right
            GameObject fSkillTitle = CreateText(featBox.transform, "SkillTitle", "고유 스킬: [보드 올 클리어 (Board Wipe Magic)]", 25, TextAlignmentOptions.Left, cuteFont, new Color(0.12f, 0.48f, 0.42f));
            SetRect(fSkillTitle, new Vector2(0, 1), new Vector2(0, 1), new Vector2(535, -75), new Vector2(650, 32));

            string featDescStr = "• 30줄 클리어 시 스킬 게이지 100% 충전 (버튼 점등!)\n" +
                                 "• 버튼 터치 시 화면의 모든 블록 폭파 + 뾰로롱 마법 연출!\n" +
                                 "• 깨끗한 빈 판에서 막힘없이 무한 콤보 연속 클리어!\n" +
                                 "• [중복 획득 특전] 60조각 즉시 지급으로 바로 1차 돌파 가능!";
            GameObject fDescTxt = CreateText(featBox.transform, "SkillDesc", featDescStr, 20, TextAlignmentOptions.Left, cuteFont, new Color(0.32f, 0.22f, 0.46f));
            SetRect(fDescTxt, new Vector2(0, 1), new Vector2(0, 1), new Vector2(535, -175), new Vector2(650, 150));

            // 1x Summon Button (100 Dia)
            GameObject btnSummon1Obj = CreateButton(pickPanel.transform, "BtnSummon1", "1회 소환\n(100 다이아)", cuteFont, new Vector2(0.5f, 1), new Vector2(-225, -1075), new Vector2(410, 115), btnTealSprite, 28, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
            Button btnSummon1 = btnSummon1Obj.GetComponent<Button>();

            // 10x Summon Button (1,000 Dia)
            GameObject btnSummon10Obj = CreateButton(pickPanel.transform, "BtnSummon10", "10회 소환\n(1,000 다이아)", cuteFont, new Vector2(0.5f, 1), new Vector2(225, -1075), new Vector2(410, 115), btnPinkSprite, 28, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            Button btnSummon10 = btnSummon10Obj.GetComponent<Button>();

            // ==========================================
            // PANEL 2: 말랑이 상점 (MASCOT SHOP - 5 Mascots)
            // ==========================================
            GameObject mascPanel = new GameObject("MascotsPanel", typeof(RectTransform));
            mascPanel.transform.SetParent(contentContainer.transform, false);
            SetRect(mascPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            mascPanel.SetActive(false);

            Button[] mascotShopActionBtns = new Button[5];
            TMP_Text[] mascotShopActionTexts = new TMP_Text[5];

            for (int m = 0; m < 5; m++)
            {
                float yPos = -115f - m * 235f;
                GameObject mCard = CreateImage(mascPanel.transform, $"MascotShopCard_{m}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(880, 215));
                mCard.GetComponent<Image>().type = Image.Type.Sliced;
                mCard.GetComponent<Image>().color = new Color(0.97f, 0.95f, 1f, 0.96f);

                // Avatar Frame & Icon
                GameObject avFrame = CreateImage(mCard.transform, "AvFrame", circleFrameSprite, new Vector2(0, 0.5f), new Vector2(90, 0), new Vector2(120, 120));
                Sprite mAv = (mascotAvatars != null && m < mascotAvatars.Length) ? mascotAvatars[m] : null;
                GameObject avIcon = CreateImage(avFrame.transform, "AvIcon", mAv, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(104, 104));
                avIcon.GetComponent<Image>().preserveAspect = true;

                // Name & Title
                GameObject mName = CreateText(mCard.transform, "Name", LobbyManager.MascotNames[m], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(mName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(180, 50), new Vector2(460, 36), new Vector2(0, 0.5f));

                GameObject mTitle = CreateText(mCard.transform, "Title", $"[{LobbyManager.MascotTitles[m]}]", 22, TextAlignmentOptions.Left, cuteFont, new Color(0.50f, 0.35f, 0.70f));
                SetRect(mTitle, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(180, 16), new Vector2(460, 28), new Vector2(0, 0.5f));

                // Ability
                GameObject mAbil = CreateText(mCard.transform, "Ability", LobbyManager.MascotAbilities[m], 19, TextAlignmentOptions.Left, cuteFont, new Color(0.38f, 0.28f, 0.50f));
                SetRect(mAbil, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(180, -32), new Vector2(460, 56), new Vector2(0, 0.5f));

                // Buy / Equip Button
                int price = LobbyManager.MascotPricesCoins[m];
                string btnLbl;
                Sprite btnSp;
                if (m == 0)
                {
                    btnLbl = "사용 중";
                    btnSp = shopEquippedBtnSprite;
                }
                else if (m == 4)
                {
                    btnLbl = "픽업 소환 전용";
                    btnSp = shopEquipBtnSprite;
                }
                else
                {
                    btnLbl = $"{price:N0} G 구매";
                    btnSp = shopEquipBtnSprite;
                }

                GameObject actBtnObj = CreateButton(mCard.transform, "BtnAction", btnLbl, cuteFont, new Vector2(1, 0.5f), new Vector2(-110, 0), new Vector2(190, 75), btnSp, 24);
                mascotShopActionBtns[m] = actBtnObj.GetComponent<Button>();
                mascotShopActionTexts[m] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (mascotShopActionTexts[m] != null)
                {
                    mascotShopActionTexts[m].color = (m == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }
            }

            // ==========================================
            // PANEL 3: 게임 배경 (IN-GAME THEMES)
            // ==========================================
            GameObject inGamePanel = new GameObject("InGameThemesPanel", typeof(RectTransform));
            inGamePanel.transform.SetParent(contentContainer.transform, false);
            SetRect(inGamePanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            inGamePanel.SetActive(false);

            string[] themeTitles = new string[] { "몽환의 밤", "캔디 랜드", "크리스탈 바다", "별빛 우주" };
            string[] themeDescs = new string[] { "기본 테마 - 달콤한 보랏빛 밤", "달콤한 디저트와 사탕 세상", "신비로운 반짝임의 바다 궁전", "아름다운 보랏빛 은하수 별빛" };
            Button[] themeActionBtns = new Button[4];
            TMP_Text[] themeActionTMPs = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                float yPos = -120f - i * 240f;
                GameObject itCard = CreateImage(inGamePanel.transform, $"ThemeItem_{i}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(880, 215));
                itCard.GetComponent<Image>().type = Image.Type.Sliced;
                itCard.GetComponent<Image>().color = new Color(0.97f, 0.95f, 1f, 0.96f);

                GameObject thumbFrame = CreateImage(itCard.transform, "ThumbFrame", tabPillSprite, new Vector2(0, 0.5f), new Vector2(105, 0), new Vector2(160, 155));
                thumbFrame.GetComponent<Image>().type = Image.Type.Sliced;
                thumbFrame.GetComponent<Image>().color = new Color(0.85f, 0.80f, 0.95f, 0.9f);

                GameObject thumbImg = CreateImage(thumbFrame.transform, "Thumb", allThemeSprites[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(150, 145));
                thumbImg.GetComponent<Image>().preserveAspect = false;

                GameObject itName = CreateText(itCard.transform, "Name", themeTitles[i], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(itName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(215, 26), new Vector2(430, 38), new Vector2(0, 0.5f));

                GameObject itDesc = CreateText(itCard.transform, "Desc", themeDescs[i], 20, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.48f, 0.65f));
                SetRect(itDesc, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(215, -22), new Vector2(430, 36), new Vector2(0, 0.5f));

                int tPrice = LobbyManager.ThemePrices[i];
                string initBtnLabel = (i == 0) ? "적용 중" : $"{tPrice:N0} G 구매";
                Sprite initBtnSprite = (i == 0) ? shopEquippedBtnSprite : shopEquipBtnSprite;
                GameObject actBtnObj = CreateButton(itCard.transform, $"BtnAction_{i}", initBtnLabel, cuteFont, new Vector2(1, 0.5f), new Vector2(-105, 0), new Vector2(185, 75), initBtnSprite, 24);
                themeActionBtns[i] = actBtnObj.GetComponent<Button>();
                themeActionTMPs[i] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (themeActionTMPs[i] != null)
                {
                    themeActionTMPs[i].color = (i == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }
            }

            // ==========================================
            // PANEL 4: 로비 배경 (LOBBY THEMES)
            // ==========================================
            GameObject lobbyPanel = new GameObject("LobbyThemesPanel", typeof(RectTransform));
            lobbyPanel.transform.SetParent(contentContainer.transform, false);
            SetRect(lobbyPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            lobbyPanel.SetActive(false);

            Button[] lobbyThemeActionBtns = new Button[3];
            TMP_Text[] lobbyThemeActionTMPs = new TMP_Text[3];

            for (int i = 0; i < 3; i++)
            {
                float yPos = -130f - i * 255f;
                GameObject itCard = CreateImage(lobbyPanel.transform, $"LobbyThemeItem_{i}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(880, 225));
                itCard.GetComponent<Image>().type = Image.Type.Sliced;
                itCard.GetComponent<Image>().color = new Color(0.97f, 0.95f, 1f, 0.96f);

                GameObject thumbFrame = CreateImage(itCard.transform, "ThumbFrame", tabPillSprite, new Vector2(0, 0.5f), new Vector2(115, 0), new Vector2(170, 160));
                thumbFrame.GetComponent<Image>().type = Image.Type.Sliced;
                thumbFrame.GetComponent<Image>().color = new Color(0.85f, 0.80f, 0.95f, 0.9f);

                GameObject thumbImg = CreateImage(thumbFrame.transform, "Thumb", allLobbySprites[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(160, 150));
                thumbImg.GetComponent<Image>().preserveAspect = false;

                GameObject itName = CreateText(itCard.transform, "Name", LobbyManager.LobbyThemeNames[i], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(itName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(225, 28), new Vector2(420, 38), new Vector2(0, 0.5f));

                GameObject itDesc = CreateText(itCard.transform, "Desc", LobbyManager.LobbyThemeDescs[i], 20, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.48f, 0.65f));
                SetRect(itDesc, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(225, -22), new Vector2(420, 36), new Vector2(0, 0.5f));

                int ltPrice = LobbyManager.LobbyThemePrices[i];
                string initLobbyBtnLabel = (i == 0) ? "적용 중" : $"{ltPrice:N0} G 구매";
                Sprite initLobbyBtnSprite = (i == 0) ? shopEquippedBtnSprite : shopEquipBtnSprite;
                GameObject actBtnObj = CreateButton(itCard.transform, $"BtnAction_{i}", initLobbyBtnLabel, cuteFont, new Vector2(1, 0.5f), new Vector2(-105, 0), new Vector2(185, 75), initLobbyBtnSprite, 24);
                lobbyThemeActionBtns[i] = actBtnObj.GetComponent<Button>();
                lobbyThemeActionTMPs[i] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (lobbyThemeActionTMPs[i] != null)
                {
                    lobbyThemeActionTMPs[i].color = (i == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }
            }

            sCloseBtn.transform.SetAsLastSibling();
            sCloseBtn.transform.SetAsLastSibling();

            // --- Settings Modal ---
            GameObject setModal = new GameObject("SettingsModal", typeof(RectTransform));
            setModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(setModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject setDarkBg = CreateImage(setModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            setDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            setDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject setCard = CreateImage(setModal.transform, "DialogCard", settingsModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 940));
            setCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject setCloseBtn = CreateButton(setCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject setTitle = CreateText(setCard.transform, "Title", "게임 설정", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(setTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -55), new Vector2(400, 48));

            // BGM Slider
            GameObject bgmLbl = CreateText(setCard.transform, "BGMLbl", "배경음악 (BGM)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(bgmLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -115), new Vector2(580, 32));
            GameObject bgmSldObj = CreateSlider(setCard.transform, "BGMSlider", new Vector2(0, -160), new Vector2(580, 40), 0.7f, sliderTrackSprite, sliderFillSprite, sliderKnobSprite);

            // SFX Slider
            GameObject sfxLbl = CreateText(setCard.transform, "SFXLbl", "효과음 (SFX)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sfxLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -220), new Vector2(580, 32));
            GameObject sfxSldObj = CreateSlider(setCard.transform, "SFXSlider", new Vector2(0, -265), new Vector2(580, 40), 0.85f, sliderTrackSprite, sliderFillSprite, sliderKnobSprite);

            // Language Selection
            GameObject langLbl = CreateText(setCard.transform, "LangLbl", "언어 설정 (Language)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(langLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -325), new Vector2(580, 32));

            string[] langNames = new string[] { "한국어", "English", "日本語", "中文" };
            float[] langXOffsets = new float[] { -219f, -73f, 73f, 219f };
            Button[] langBtns = new Button[4];
            Image[] langBgs = new Image[4];
            TMP_Text[] langTexts = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                GameObject lBtnObj = CreateButton(setCard.transform, $"BtnLang_{i}", langNames[i], cuteFont, new Vector2(0.5f, 1), new Vector2(langXOffsets[i], -375), new Vector2(136, 48), tabPillSprite, 22);
                langBtns[i] = lBtnObj.GetComponent<Button>();
                langBgs[i] = lBtnObj.GetComponent<Image>();
                langTexts[i] = lBtnObj.GetComponentInChildren<TMP_Text>();
            }

            // Mobile Support Row (Haptics, Privacy Policy, Probability Info)
            GameObject mobileTitle = CreateText(setCard.transform, "MobileTitle", "모바일 & 편의 기능 (Mobile Support)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(mobileTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -438), new Vector2(580, 32));

            GameObject btnHapticObj = CreateButton(setCard.transform, "BtnHaptic", "진동: 켜짐", cuteFont, new Vector2(0.5f, 1), new Vector2(-200, -488), new Vector2(185, 48), btnTealSprite, 21, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
            Button btnHaptic = btnHapticObj.GetComponent<Button>();
            TMP_Text hapticTxt = btnHapticObj.GetComponentInChildren<TMP_Text>();

            GameObject btnPrivacyObj = CreateButton(setCard.transform, "BtnPrivacy", "개인정보방침", cuteFont, new Vector2(0.5f, 1), new Vector2(0, -488), new Vector2(195, 48), tabPillSprite, 21, new Color(0.35f, 0.22f, 0.55f));
            Button btnPrivacy = btnPrivacyObj.GetComponent<Button>();

            GameObject btnProbSetObj = CreateButton(setCard.transform, "BtnProbSet", "확률 정보", cuteFont, new Vector2(0.5f, 1), new Vector2(200, -488), new Vector2(185, 48), btnPinkSprite, 21, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            btnProbSetObj.GetComponent<Button>().onClick.AddListener(lobbyMgr.OpenProbabilityModal);

            // Row 1: Aspect Ratio Selection (16:9, 16:10, 4:3, 9:16)
            GameObject aspectTitle = CreateText(setCard.transform, "AspectTitle", LocalizationManager.Get("settings_aspect_ratio_title"), 26, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(aspectTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -552), new Vector2(580, 32));

            string[] aspectNames = new string[] { "16:9", "16:10", "4:3", "9:16" };
            float[] aspectXOffsets = new float[] { -219f, -73f, 73f, 219f };
            Button[] aspectBtns = new Button[4];
            Image[] aspectBgs = new Image[4];
            TMP_Text[] aspectTexts = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                Sprite initSprite = (i == 3) ? btnPinkSprite : tabPillSprite;
                GameObject aBtnObj = CreateButton(setCard.transform, $"BtnAspect_{i}", aspectNames[i], cuteFont, new Vector2(0.5f, 1), new Vector2(aspectXOffsets[i], -602), new Vector2(136, 46), initSprite, 22);
                aspectBtns[i] = aBtnObj.GetComponent<Button>();
                aspectBgs[i] = aBtnObj.GetComponent<Image>();
                aspectTexts[i] = aBtnObj.GetComponentInChildren<TMP_Text>();
                if (aspectTexts[i] != null)
                {
                    aspectTexts[i].color = (i == 3) ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                    aspectTexts[i].fontStyle = (i == 3) ? FontStyles.Bold : FontStyles.Normal;
                }
            }

            // Row 2: Window Mode Selection (창 모드, 테두리 없는 창, 전체화면)
            GameObject winModeTitle = CreateText(setCard.transform, "WindowModeTitle", LocalizationManager.Get("settings_window_mode_title"), 26, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(winModeTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -665), new Vector2(580, 32));

            string[] winModeNames = new string[] { "창 모드", "테두리 없는 창", "전체화면" };
            float[] winModeXOffsets = new float[] { -196f, 0f, 196f };
            Button[] winModeBtns = new Button[3];
            Image[] winModeBgs = new Image[3];
            TMP_Text[] winModeTexts = new TMP_Text[3];

            for (int j = 0; j < 3; j++)
            {
                Sprite initSprite = (j == 0) ? btnPinkSprite : tabPillSprite;
                GameObject wBtnObj = CreateButton(setCard.transform, $"BtnWindowMode_{j}", winModeNames[j], cuteFont, new Vector2(0.5f, 1), new Vector2(winModeXOffsets[j], -715), new Vector2(186, 46), initSprite, 21);
                winModeBtns[j] = wBtnObj.GetComponent<Button>();
                winModeBgs[j] = wBtnObj.GetComponent<Image>();
                winModeTexts[j] = wBtnObj.GetComponentInChildren<TMP_Text>();
                if (winModeTexts[j] != null)
                {
                    winModeTexts[j].color = (j == 0) ? Color.white : new Color(0.35f, 0.22f, 0.55f);
                    winModeTexts[j].fontStyle = (j == 0) ? FontStyles.Bold : FontStyles.Normal;
                }
            }

            // Version
            GameObject verTxt = CreateText(setCard.transform, "Version", "말랑블라스트 v1.2.0 (Fairy Party Edition)", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.6f, 0.55f, 0.7f));
            SetRect(verTxt, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(600, 36));

            // --- Quit Confirm Modal (NEW!) ---
            GameObject qModal = new GameObject("QuitConfirmModal", typeof(RectTransform));
            qModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(qModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject qDarkBg = CreateImage(qModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            qDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            qDarkBg.GetComponent<Image>().raycastTarget = true;
            Button qDarkBgBtn = qDarkBg.AddComponent<Button>();
            qDarkBgBtn.transition = Selectable.Transition.None;

            GameObject qCard = CreateImage(qModal.transform, "DialogCard", settingsModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(680, 420));
            qCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject qTitle = CreateText(qCard.transform, "Title", "게임 종료", 38, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(qTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -55), new Vector2(400, 48));

            GameObject qDesc = CreateText(qCard.transform, "Desc", "정말 말랑블라스트를 종료하시겠어요?", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.45f, 0.35f, 0.60f));
            SetRect(qDesc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -145), new Vector2(580, 60));

            GameObject qYesBtnObj = CreateButton(qCard.transform, "BtnYes", "종료", cuteFont, new Vector2(0.5f, 0), new Vector2(-130, 80), new Vector2(210, 62), btnPinkSprite, 26, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            GameObject qNoBtnObj = CreateButton(qCard.transform, "BtnNo", "취소", cuteFont, new Vector2(0.5f, 0), new Vector2(130, 80), new Vector2(210, 62), btnTealSprite, 26, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            // --- Mascot Select & Management Modal (5 Mascots + Shards & Breakthrough System) ---
            GameObject mascotModalObj = new GameObject("MascotModal", typeof(RectTransform));
            mascotModalObj.transform.SetParent(lobbyRoot.transform, false);
            SetRect(mascotModalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject mascotDarkBg = CreateImage(mascotModalObj.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            mascotDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            mascotDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject mascotCard = CreateImage(mascotModalObj.transform, "DialogCard", shopModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(920, 1500));
            mascotCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject mascotCloseBtn = CreateButton(mascotCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject mascotTitle = CreateText(mascotCard.transform, "Title", "말랑이 관리 & 돌파", 42, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(mascotTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -50), new Vector2(600, 50));

            GameObject mascotSubTitle = CreateText(mascotCard.transform, "Subtitle", "말랑이 조각 60개로 능력을 돌파 업그레이드하세요!", 22, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.45f, 0.70f));
            SetRect(mascotSubTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -95), new Vector2(750, 32));

            // Scroll container for smooth scrolling
            GameObject mScrollObj = new GameObject("ScrollArea", typeof(RectTransform), typeof(ScrollRect), typeof(RectMask2D));
            mScrollObj.transform.SetParent(mascotCard.transform, false);
            SetRect(mScrollObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(880, 1300));
            ScrollRect mScrollRect = mScrollObj.GetComponent<ScrollRect>();
            mScrollRect.horizontal = false;
            mScrollRect.vertical = true;
            mScrollRect.movementType = ScrollRect.MovementType.Clamped;

            GameObject mContent = new GameObject("Content", typeof(RectTransform));
            mContent.transform.SetParent(mScrollObj.transform, false);
            SetRect(mContent, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(880, 1300), new Vector2(0.5f, 1f));
            mScrollRect.content = mContent.GetComponent<RectTransform>();
            mScrollRect.viewport = mScrollObj.GetComponent<RectTransform>();

            Button[] mActionBtns = new Button[5];
            TMP_Text[] mActionTexts = new TMP_Text[5];
            Button[] mUpgradeBtns = new Button[5];
            TMP_Text[] mUpgradeTexts = new TMP_Text[5];
            TMP_Text[] mLevelTexts = new TMP_Text[5];
            TMP_Text[] mShardTexts = new TMP_Text[5];
            TMP_Text[] mAbilityTexts = new TMP_Text[5];

            Color[] mCardBgColors = new Color[]
            {
                new Color(1f, 0.95f, 0.97f, 0.96f),
                new Color(0.93f, 0.98f, 0.96f, 0.96f),
                new Color(1f, 0.98f, 0.91f, 0.96f),
                new Color(0.96f, 0.94f, 1f, 0.96f),
                new Color(0.98f, 0.95f, 1f, 0.98f)
            };

            for (int i = 0; i < 5; i++)
            {
                float yPos = -118f - i * 250f;
                GameObject mItemCard = CreateImage(mContent.transform, $"MascotItem_{i}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(850, 235));
                mItemCard.GetComponent<Image>().type = Image.Type.Sliced;
                mItemCard.GetComponent<Image>().color = mCardBgColors[i];

                // Left Circle Frame with Mascot Avatar
                GameObject avFrame = CreateImage(mItemCard.transform, "AvFrame", circleFrameSprite, new Vector2(0, 0.5f), new Vector2(80, 0), new Vector2(125, 125));
                Sprite mAv = (mascotAvatars != null && i < mascotAvatars.Length) ? mascotAvatars[i] : null;
                GameObject avIcon = CreateImage(avFrame.transform, "AvIcon", mAv, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(110, 110));
                avIcon.GetComponent<Image>().preserveAspect = true;

                // Mascot Name
                GameObject mName = CreateText(mItemCard.transform, "Name", LobbyManager.MascotNames[i], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.25f, 0.16f, 0.42f));
                SetRect(mName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(160, 56), new Vector2(250, 36), new Vector2(0, 0.5f));

                // Level badge
                GameObject mLvl = CreateText(mItemCard.transform, "Level", "Lv.1", 24, TextAlignmentOptions.Left, cuteFont, new Color(0.92f, 0.45f, 0.05f));
                SetRect(mLvl, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(340, 56), new Vector2(90, 36), new Vector2(0, 0.5f));
                mLevelTexts[i] = mLvl.GetComponent<TMP_Text>();

                // Title
                GameObject mTitleTxt = CreateText(mItemCard.transform, "TitleTxt", $"[{LobbyManager.MascotTitles[i]}]", 20, TextAlignmentOptions.Left, cuteFont, new Color(0.50f, 0.35f, 0.70f));
                SetRect(mTitleTxt, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(160, 24), new Vector2(300, 26), new Vector2(0, 0.5f));

                // Ability Description
                GameObject mAbil = CreateText(mItemCard.transform, "Ability", LobbyManager.GetMascotAbilityDescription(i, 1), 19, TextAlignmentOptions.Left, cuteFont, new Color(0.38f, 0.28f, 0.50f));
                SetRect(mAbil, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(160, -22), new Vector2(460, 52), new Vector2(0, 0.5f));
                mAbilityTexts[i] = mAbil.GetComponent<TMP_Text>();

                // Shards Progress
                GameObject mShard = CreateText(mItemCard.transform, "Shards", "말랑 조각: <color=#3498DB>0</color> / 60", 19, TextAlignmentOptions.Left, cuteFont, new Color(0.40f, 0.30f, 0.55f));
                SetRect(mShard, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(160, -68), new Vector2(400, 28), new Vector2(0, 0.5f));
                mShardTexts[i] = mShard.GetComponent<TMP_Text>();

                // Action Button (Equip / Select / Locked)
                Sprite initBtnSp = (i == 0) ? shopEquippedBtnSprite : shopEquipBtnSprite;
                string initLbl = (i == 0) ? "사용 중" : (i == 4) ? "소환 전용" : "선택하기";
                GameObject actBtnObj = CreateButton(mItemCard.transform, "BtnAction", initLbl, cuteFont, new Vector2(1, 0.5f), new Vector2(-105, 38), new Vector2(175, 68), initBtnSp, 24);
                mActionBtns[i] = actBtnObj.GetComponent<Button>();
                mActionTexts[i] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (mActionTexts[i] != null)
                {
                    mActionTexts[i].color = (i == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }

                // Upgrade / Breakthrough Button
                GameObject upgBtnObj = CreateButton(mItemCard.transform, "BtnUpgrade", "돌파 (60)", cuteFont, new Vector2(1, 0.5f), new Vector2(-105, -42), new Vector2(175, 62), btnTealSprite, 22, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));
                mUpgradeBtns[i] = upgBtnObj.GetComponent<Button>();
                mUpgradeTexts[i] = upgBtnObj.GetComponentInChildren<TMP_Text>();
                if (mUpgradeTexts[i] != null)
                {
                    mUpgradeTexts[i].color = new Color(0.45f, 0.45f, 0.50f, 1f);
                }
            }

            mascotModalObj.SetActive(false);

            // --- Summon Result Modal (Gacha Reveal Modal) ---
            GameObject summonResultModalObj = new GameObject("SummonResultModal", typeof(RectTransform));
            summonResultModalObj.transform.SetParent(lobbyRoot.transform, false);
            SetRect(summonResultModalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            summonResultModalObj.SetActive(false);

            GameObject srDarkBg = CreateImage(summonResultModalObj.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            srDarkBg.GetComponent<Image>().color = new Color(0.04f, 0.02f, 0.12f, 0.92f);
            srDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject srCard = CreateImage(summonResultModalObj.transform, "DialogCard", shopModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 800));
            srCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject srTitle = CreateText(srCard.transform, "Title", "소환 결과", 38, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(srTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(500, 45));

            // Center Icon Frame
            GameObject srIconFrame = CreateImage(srCard.transform, "IconFrame", circleFrameSprite, new Vector2(0.5f, 1), new Vector2(0, -155), new Vector2(160, 160));
            Sprite specialAvDefault = (mascotAvatars != null && mascotAvatars.Length > 4) ? mascotAvatars[4] : null;
            GameObject srIconImg = CreateImage(srIconFrame.transform, "Icon", specialAvDefault, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(140, 140));
            srIconImg.GetComponent<Image>().preserveAspect = true;

            // Highlight Text
            GameObject srHlText = CreateText(srCard.transform, "HighlightText", "말랑이 조각 획득 완료!", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.28f, 0.18f, 0.48f));
            SetRect(srHlText, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -280), new Vector2(720, 80));

            // Shards Gained Box
            GameObject srShardsBox = CreateImage(srCard.transform, "ShardsBox", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -435), new Vector2(700, 200));
            srShardsBox.GetComponent<Image>().type = Image.Type.Sliced;
            srShardsBox.GetComponent<Image>().color = new Color(0.95f, 0.92f, 1f, 0.95f);

            GameObject srShardsText = CreateText(srShardsBox.transform, "ShardsText", "• 핑크 말랑이 조각: +5개", 22, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.25f, 0.55f));
            SetRect(srShardsText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Confirm Button
            GameObject srConfirmBtn = CreateButton(srCard.transform, "BtnConfirm", "확인", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(260, 72), btnPinkSprite, 28, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));

            lobbyMgr.SetupSummonResultModal(
                summonResultModalObj,
                srConfirmBtn.GetComponent<Button>(),
                srTitle.GetComponent<TMP_Text>(),
                srHlText.GetComponent<TMP_Text>(),
                srShardsText.GetComponent<TMP_Text>(),
                srIconImg.GetComponent<Image>()
            );

            // --- Probability Info Modal (Korean Game Industry Act Compliance) ---
            GameObject probModalObj = new GameObject("ProbabilityModal", typeof(RectTransform));
            probModalObj.transform.SetParent(lobbyRoot.transform, false);
            SetRect(probModalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            probModalObj.SetActive(false);

            GameObject probDarkBg = CreateImage(probModalObj.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            probDarkBg.GetComponent<Image>().color = new Color(0.04f, 0.02f, 0.12f, 0.90f);
            probDarkBg.GetComponent<Image>().raycastTarget = true;
            Button probDarkBgBtn = probDarkBg.AddComponent<Button>();
            probDarkBgBtn.transition = Selectable.Transition.None;

            GameObject probCard = CreateImage(probModalObj.transform, "DialogCard", shopModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(850, 1080));
            probCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject probCloseBtn = CreateButton(probCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject probTitle = CreateText(probCard.transform, "Title", "소환 확률 정보", 38, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(probTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -50), new Vector2(500, 48));

            // Legal Compliance Notice Card
            GameObject probLawCard = CreateImage(probCard.transform, "LawCard", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -125), new Vector2(770, 85));
            probLawCard.GetComponent<Image>().type = Image.Type.Sliced;
            probLawCard.GetComponent<Image>().color = new Color(0.94f, 0.91f, 1f, 0.95f);

            GameObject probLawTxt = CreateText(probLawCard.transform, "LawTxt", "본 게임은 대한민국 게임산업진흥에 관한 법률 제33조에 따라\n확률형 아이템의 소환 확률 정보를 100% 투명하게 공개하고 있습니다.", 18, TextAlignmentOptions.Center, cuteFont, new Color(0.32f, 0.20f, 0.55f));
            SetRect(probLawTxt, Vector2.zero, Vector2.one, new Vector2(20, 0), new Vector2(-20, 0));

            // Table Header Card
            GameObject tableHeader = CreateImage(probCard.transform, "TableHeader", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -230), new Vector2(770, 45));
            tableHeader.GetComponent<Image>().type = Image.Type.Sliced;
            tableHeader.GetComponent<Image>().color = new Color(0.85f, 0.80f, 0.96f, 0.95f);

            GameObject thName = CreateText(tableHeader.transform, "ThName", "등장 항목 / 구성품", 20, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.16f, 0.48f));
            SetRect(thName, new Vector2(0, 0), new Vector2(0.55f, 1), new Vector2(25, 0), Vector2.zero);

            GameObject thType = CreateText(tableHeader.transform, "ThType", "구분", 20, TextAlignmentOptions.Center, cuteFont, new Color(0.28f, 0.16f, 0.48f));
            SetRect(thType, new Vector2(0.55f, 0), new Vector2(0.75f, 1), Vector2.zero, Vector2.zero);

            GameObject thRate = CreateText(tableHeader.transform, "ThRate", "소환 확률", 20, TextAlignmentOptions.Right, cuteFont, new Color(0.28f, 0.16f, 0.48f));
            SetRect(thRate, new Vector2(0.75f, 0), new Vector2(1, 1), Vector2.zero, new Vector2(-25, 0));

            // 5 Items Data Rows
            string[] probItemNames = new string[]
            {
                "★ [올 클리어 엔젤] 스페셜 말랑이",
                "분홍 말랑이 조각 (5개)",
                "하늘 말랑이 조각 (5개)",
                "노랑 말랑이 조각 (5개)",
                "보라 말랑이 조각 (5개)"
            };
            string[] probItemTypes = new string[] { "스페셜 완제", "조각 보상", "조각 보상", "조각 보상", "조각 보상" };
            string[] probItemRates = new string[] { "0.100 %", "24.975 %", "24.975 %", "24.975 %", "24.975 %" };
            Color[] rowBgs = new Color[]
            {
                new Color(1f, 0.92f, 0.95f, 0.95f),
                new Color(0.97f, 0.97f, 1f, 0.95f),
                new Color(0.94f, 0.98f, 1f, 0.95f),
                new Color(1f, 0.98f, 0.92f, 0.95f),
                new Color(0.97f, 0.94f, 1f, 0.95f)
            };

            for (int r = 0; r < 5; r++)
            {
                float rowY = -285f - (r * 62f);
                GameObject rowObj = CreateImage(probCard.transform, $"Row_{r}", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, rowY), new Vector2(770, 54));
                rowObj.GetComponent<Image>().type = Image.Type.Sliced;
                rowObj.GetComponent<Image>().color = rowBgs[r];

                Color nameCol = (r == 0) ? new Color(0.85f, 0.20f, 0.42f) : new Color(0.25f, 0.18f, 0.45f);
                GameObject rName = CreateText(rowObj.transform, "Name", probItemNames[r], (r == 0) ? 21 : 19, TextAlignmentOptions.Left, cuteFont, nameCol);
                SetRect(rName, new Vector2(0, 0), new Vector2(0.55f, 1), new Vector2(25, 0), Vector2.zero);

                Color typeCol = (r == 0) ? new Color(0.80f, 0.25f, 0.45f) : new Color(0.40f, 0.35f, 0.55f);
                GameObject rType = CreateText(rowObj.transform, "Type", probItemTypes[r], 18, TextAlignmentOptions.Center, cuteFont, typeCol);
                SetRect(rType, new Vector2(0.55f, 0), new Vector2(0.75f, 1), Vector2.zero, Vector2.zero);

                Color rateCol = (r == 0) ? new Color(0.95f, 0.30f, 0.15f) : new Color(0.20f, 0.45f, 0.70f);
                GameObject rRate = CreateText(rowObj.transform, "Rate", probItemRates[r], (r == 0) ? 22 : 20, TextAlignmentOptions.Right, cuteFont, rateCol);
                SetRect(rRate, new Vector2(0.75f, 0), new Vector2(1, 1), Vector2.zero, new Vector2(-25, 0));
            }

            // Total Row Card
            GameObject totalRow = CreateImage(probCard.transform, "RowTotal", tabPillSprite, new Vector2(0.5f, 1), new Vector2(0, -600), new Vector2(770, 52));
            totalRow.GetComponent<Image>().type = Image.Type.Sliced;
            totalRow.GetComponent<Image>().color = new Color(0.90f, 0.86f, 0.98f, 0.95f);

            GameObject totalLbl = CreateText(totalRow.transform, "TotalLbl", "합계 (Total Probability)", 20, TextAlignmentOptions.Left, cuteFont, new Color(0.25f, 0.15f, 0.45f));
            SetRect(totalLbl, new Vector2(0, 0), new Vector2(0.55f, 1), new Vector2(25, 0), Vector2.zero);

            GameObject totalVal = CreateText(totalRow.transform, "TotalVal", "100.000 %", 21, TextAlignmentOptions.Right, cuteFont, new Color(0.25f, 0.15f, 0.45f));
            SetRect(totalVal, new Vector2(0.75f, 0), new Vector2(1, 1), Vector2.zero, new Vector2(-25, 0));

            // Policy / Explanation Card
            GameObject probNotesCard = CreateImage(probCard.transform, "NotesCard", packageCardSprite, new Vector2(0.5f, 1), new Vector2(0, -745), new Vector2(770, 160));
            probNotesCard.GetComponent<Image>().type = Image.Type.Sliced;
            probNotesCard.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.96f);

            string notesStr = "<b>[안내 사항]</b>\n" +
                              "• 스페셜 말랑이를 중복 획득 시 <b>스페셜 조각 60개</b>로 지급되어 즉시 돌파 가능합니다.\n" +
                              "• 획득한 말랑이 조각은 <b>[말랑이 관리 & 돌파]</b> 메뉴에서 능력치를 영구 강화할 수 있습니다.\n" +
                              "• 소환 확률은 독립 시행으로 적용되며, 구매 전 확률 정보를 상시 확인할 수 있습니다.";
            GameObject probNotesTxt = CreateText(probNotesCard.transform, "NotesTxt", notesStr, 17, TextAlignmentOptions.Left, cuteFont, new Color(0.38f, 0.28f, 0.52f));
            SetRect(probNotesTxt, Vector2.zero, Vector2.one, new Vector2(20, -10), new Vector2(-20, 10));

            // Confirm Button
            GameObject probConfirmBtn = CreateButton(probCard.transform, "BtnConfirm", "확인", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 55), new Vector2(240, 68), btnPinkSprite, 26, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));

            // --- Gacha Ball Opening Presentation Modal ---
            GameObject gachaModalObj = new GameObject("GachaPresentationModal", typeof(RectTransform), typeof(GachaPresentationController));
            gachaModalObj.transform.SetParent(canvasObj.transform, false);
            gachaModalObj.transform.SetAsLastSibling();
            SetRect(gachaModalObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));
            gachaModalObj.SetActive(true);

            GameObject gachaContentRoot = new GameObject("ContentRoot", typeof(RectTransform));
            gachaContentRoot.transform.SetParent(gachaModalObj.transform, false);
            SetRect(gachaContentRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject gachaDarkBg = CreateImage(gachaContentRoot.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            gachaDarkBg.GetComponent<Image>().color = new Color(0.04f, 0.02f, 0.12f, 0.90f);
            gachaDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject gachaTitle = CreateText(gachaContentRoot.transform, "Title", "말랑이 픽업 소환", 40, TextAlignmentOptions.Center, cuteFont, new Color(1f, 0.92f, 0.55f));
            SetRect(gachaTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -65), new Vector2(600, 50));

            GameObject gachaInstr = CreateText(gachaContentRoot.transform, "Instruction", "가챠볼을 터치해 하나씩 열어보세요!", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.85f, 0.82f, 0.95f));
            SetRect(gachaInstr, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -115), new Vector2(700, 35));

            GameObject ballsContObj = new GameObject("BallsContainer", typeof(RectTransform));
            ballsContObj.transform.SetParent(gachaContentRoot.transform, false);
            SetRect(ballsContObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(850, 500));

            GameObject btnOpenAllObj = CreateButton(gachaContentRoot.transform, "BtnOpenAll", "모두 열기", cuteFont, new Vector2(0.5f, 0), new Vector2(240, 75), new Vector2(220, 68), btnTealSprite, 24, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            GameObject btnGachaConfirmObj = CreateButton(gachaContentRoot.transform, "BtnConfirm", "확인", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 75), new Vector2(260, 72), btnPinkSprite, 28, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            btnGachaConfirmObj.SetActive(false);

            GameObject climaxObj = new GameObject("SpecialClimaxOverlay", typeof(RectTransform));
            climaxObj.transform.SetParent(gachaContentRoot.transform, false);
            climaxObj.transform.SetAsLastSibling();
            SetRect(climaxObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject climaxDark = CreateImage(climaxObj.transform, "Darken", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            climaxDark.GetComponent<Image>().color = new Color(0.02f, 0.01f, 0.08f, 0.94f);
            climaxDark.GetComponent<Image>().raycastTarget = true;

            Sprite sunburstSp = CuteBlockTextureGenerator.GetOrCreateGachaSunburstSprite();
            GameObject climaxBurst = CreateImage(climaxObj.transform, "Sunburst", sunburstSp, new Vector2(0.5f, 0.5f), new Vector2(0, 60), new Vector2(1100, 1100));

            Sprite gachaAuraSp = CuteBlockTextureGenerator.GetOrCreateGachaRainbowAuraSprite();
            GameObject climaxAura = CreateImage(climaxObj.transform, "AuraGlow", gachaAuraSp, new Vector2(0.5f, 0.5f), new Vector2(0, 60), new Vector2(750, 750));

            Sprite gachaSpecialCutout = CuteBlockTextureGenerator.GetOrCreateSpecialMascotSprite();
            GameObject climaxMascot = CreateImage(climaxObj.transform, "GiantMascot", gachaSpecialCutout, new Vector2(0.5f, 0.5f), new Vector2(0, 60), new Vector2(620, 620));
            climaxMascot.GetComponent<Image>().preserveAspect = true;

            GameObject climaxTitle = CreateText(climaxObj.transform, "ClimaxTitle", "<size=44><color=#FFE600>★ SPECIAL MASCOT! ★</color></size>\n<size=30><color=#FFFFFF>스페셜 말랑이 강림!</color></size>", 32, TextAlignmentOptions.Center, cuteFont, Color.white);
            SetRect(climaxTitle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -320), new Vector2(800, 120));

            GameObject climaxSub = CreateText(climaxObj.transform, "ClimaxSub", "화면을 터치하여 계속하기", 22, TextAlignmentOptions.Center, cuteFont, new Color(0.85f, 0.82f, 0.95f));
            SetRect(climaxSub, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -410), new Vector2(600, 40));

            GameObject climaxDismissObj = CreateButton(climaxObj.transform, "BtnDismiss", "", cuteFont, Vector2.zero, Vector2.zero, Vector2.zero, null, 1);
            SetRect(climaxDismissObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            climaxDismissObj.GetComponent<Image>().color = Color.clear;

            climaxObj.SetActive(false);
            gachaContentRoot.SetActive(false);

            Sprite greyBallSp = CuteBlockTextureGenerator.GetOrCreateGachaBallGreySprite();
            Sprite rainbowBallSp = CuteBlockTextureGenerator.GetOrCreateGachaBallRainbowSprite();
            Sprite sparkleStarSp = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();

            var gachaCtrl = gachaModalObj.GetComponent<GachaPresentationController>();
            gachaCtrl.SetupSprites(greyBallSp, rainbowBallSp, gachaAuraSp, sunburstSp, sparkleStarSp, mascotAvatars, gachaSpecialCutout);
            gachaCtrl.SetupReferences(
                gachaContentRoot, ballsContObj.GetComponent<RectTransform>(), gachaInstr.GetComponent<TMP_Text>(),
                btnOpenAllObj.GetComponent<Button>(), btnGachaConfirmObj.GetComponent<Button>(),
                climaxObj, climaxBurst.GetComponent<Image>(), climaxMascot.GetComponent<Image>(),
                climaxTitle.GetComponent<TMP_Text>(), climaxSub.GetComponent<TMP_Text>(),
                climaxDismissObj.GetComponent<Button>()
            );
            EditorUtility.SetDirty(gachaCtrl);

            // Setup LobbyManager References
            lobbyMgr.SetupReferences(
                lobbyRoot, lobbyCG,
                pBtn, pAvatarImg, coinLabel.GetComponent<TMP_Text>(),
                partyMascotRTs, partyLabelObjs, partyLabelBgImgs, partyLabelTMPs,
                playBtn, playBtnBg, playBtnText,
                pModal, pBigAvImg, pNickInput.textComponent, pBioInput, pStats.GetComponent<TMP_Text>(), pStats.GetComponent<TMP_Text>(), avPickBtns, pLogoutBtn, pCloseBtn.GetComponent<Button>(),
                sModal, sCoinTxt.GetComponent<TMP_Text>(), sCloseBtn.GetComponent<Button>(),
                setModal, bgmSldObj.GetComponent<Slider>(), sfxSldObj.GetComponent<Slider>(), setCloseBtn.GetComponent<Button>(),
                null, null,
                mascotAvatars,
                partyGlowObjs,
                pNickInput,
                null,
                playGlowImg,
                partyTip.GetComponent<TMP_Text>(),
                diaLabel.GetComponent<TMP_Text>()
            );

            lobbyMgr.SetupProfileModalExtraReferences(
                pTagInput, pCheckBtn, pStatusTxt,
                null, null,
                lModal, lIdInput, lPwInput,
                lSubBtn, lGglBtn, lCloseBtn.GetComponent<Button>(), lStatTxt,
                pStartEditBtn, pConfirmBtn
            );

            lobbyMgr.SetupMascotModal(
                mascotModalObj,
                mascotCloseBtn.GetComponent<Button>(),
                mActionBtns,
                mActionTexts,
                null,
                mUpgradeBtns,
                mUpgradeTexts,
                mLevelTexts,
                mShardTexts,
                mAbilityTexts
            );
            lobbyMgr.SetupShopButtonSprites(shopEquipBtnSprite, shopEquippedBtnSprite);
            lobbyMgr.SetupShopVerticalTabs(
                shopTabBtns, shopTabBgs, shopTabTexts,
                recPanel, pickPanel, mascPanel,
                inGamePanel, lobbyPanel,
                tabVerticalActiveSprite, tabVerticalInactiveSprite,
                sCoinTxt.GetComponent<TMP_Text>(), sDiaTxt.GetComponent<TMP_Text>()
            );
            lobbyMgr.SetupPickupBannerController(bannerCtrl);
            lobbyMgr.SetupShopPackagesAndSummon(packageBtns, btnSummon1, btnSummon10);
            lobbyMgr.SetupShopMascots(mascotShopActionBtns, mascotShopActionTexts);
            lobbyMgr.SetupShopThemes(bgImgComp, allThemeSprites, themeActionBtns, themeActionTMPs);
            lobbyMgr.SetupLobbyThemes(lobbyBg.GetComponent<Image>(), allLobbySprites, lobbyThemeActionBtns, lobbyThemeActionTMPs);
            lobbyMgr.SetupLanguageButtons(
                langBtns, langBgs, langTexts,
                setTitle.GetComponent<TMP_Text>(),
                bgmLbl.GetComponent<TMP_Text>(),
                sfxLbl.GetComponent<TMP_Text>(),
                langLbl.GetComponent<TMP_Text>(),
                sTitle.GetComponent<TMP_Text>(),
                null,
                null,
                pTitle.GetComponent<TMP_Text>(),
                verTxt.GetComponent<TMP_Text>()
            );
            lobbyMgr.SetupLanguageLogos(lobbyLogoObj.GetComponent<Image>(), langLogoSprites);
            lobbyMgr.SetupScreenSettingsAndQuitReferences(
                qModal,
                qYesBtnObj.GetComponent<Button>(),
                qNoBtnObj.GetComponent<Button>(),
                qDarkBgBtn,
                qTitle.GetComponent<TMP_Text>(),
                qDesc.GetComponent<TMP_Text>(),
                qYesBtnObj.GetComponentInChildren<TMP_Text>(),
                qNoBtnObj.GetComponentInChildren<TMP_Text>(),
                aspectTitle.GetComponent<TMP_Text>(),
                aspectBtns,
                aspectBgs,
                aspectTexts,
                winModeTitle.GetComponent<TMP_Text>(),
                winModeBtns,
                winModeBgs,
                winModeTexts,
                btnPinkSprite,
                tabPillSprite
            );
            lobbyMgr.SetupProbabilityModal(
                probModalObj,
                probCloseBtn.GetComponent<Button>(),
                probConfirmBtn.GetComponent<Button>(),
                probDarkBgBtn
            );
            lobbyMgr.SetupMobileSettings(btnHaptic, hapticTxt, btnPrivacy);
            EditorUtility.SetDirty(lobbyMgr);

            // 6. Fairy Screen Transition Overlay (Dissolve + 1-Sec Corner Sparkles)
            Sprite fairyRippleSprite = CuteBlockTextureGenerator.GetOrCreateFairyRippleSprite();
            fairySparkleSprite = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();
            Sprite fairyTrailSprite = CuteBlockTextureGenerator.GetOrCreateFairyTrailSprite();

            GameObject transObj = new GameObject("FairyScreenTransitionOverlay", typeof(RectTransform));
            transObj.transform.SetParent(canvasObj.transform, false);
            transObj.transform.SetAsLastSibling();
            SetRect(transObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            var screenTrans = transObj.AddComponent<FairyScreenTransition>();
            screenTrans.SetupSprites(fairySparkleSprite, fairyTrailSprite);

            // 7. Fairy Touch & Drag FX Overlay (Topmost UI Layer)
            GameObject fairyFXObj = new GameObject("FairyTouchFXOverlay", typeof(RectTransform));
            fairyFXObj.transform.SetParent(canvasObj.transform, false);
            fairyFXObj.transform.SetAsLastSibling();
            SetRect(fairyFXObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            var fairyFX = fairyFXObj.AddComponent<FairyTouchFXManager>();
            fairyFX.SetupSprites(fairyRippleSprite, fairySparkleSprite, fairyTrailSprite);

            // 7.5. Steam Manager
            GameObject steamMgrObj = new GameObject("[SteamManager]");
            steamMgrObj.AddComponent<SteamManager>();

            // 8. Save Scene
            string scenePath = "Assets/Scenes/BlockBlastScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            string active1003Path = "Assets/Scenes/10-03.unity";
            if (System.IO.File.Exists(active1003Path))
            {
                EditorSceneManager.SaveScene(newScene, active1003Path);
            }
            AssetDatabase.Refresh();

            InspectUIMenu.ApplyMobileStoreSettings();

            Debug.Log("<color=#FF7AA2><b>[Block Blast]</b> 10종 이상의 젤리 그래픽 에셋, 주아체 고해상도 폰트가 적용된 프리미엄 씬이 완성되었습니다!</color>");
        }

        private static GameObject CreateButton(Transform parent, string name, string label, TMP_FontAsset fontAsset, Vector2 anchor, Vector2 pos, Vector2 size, Sprite btnSprite, float fontSize = 30, Color? textCol = null, Color? textShadowCol = null)
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
            cb.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
            cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.65f, 0.65f, 0.65f, 0.65f);
            btn.colors = cb;

            SetRect(btnObj, anchor, anchor, pos, size);

            if (!string.IsNullOrEmpty(label))
            {
                Color fontColor = textCol ?? Color.white;
                bool hasShadow = textShadowCol.HasValue;
                GameObject textObj = CreateText(btnObj.transform, "Text", label, fontSize, TextAlignmentOptions.Center, fontAsset, fontColor, hasShadow, textShadowCol);
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

            if (addShadow)
            {
                var shadow = obj.AddComponent<UnityEngine.UI.Shadow>();
                shadow.effectColor = shadowCol ?? new Color(0f, 0f, 0f, 0.45f);
                shadow.effectDistance = new Vector2(0f, -2.5f);
            }

            return obj;
        }

        private static GameObject CreateSlider(Transform parent, string name, Vector2 pos, Vector2 size, float initialVal, Sprite trackSprite, Sprite fillSprite, Sprite knobSprite)
        {
            GameObject sliderObj = new GameObject(name, typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(parent, false);
            SetRect(sliderObj, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), pos, size);
            Slider slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = initialVal;

            // Background (Track)
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderObj.transform, false);
            SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image bgImg = bg.GetComponent<Image>();
            bgImg.sprite = trackSprite;
            bgImg.type = Image.Type.Sliced;
            bgImg.color = Color.white;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform, false);
            SetRect(fillArea, new Vector2(0, 0), new Vector2(1, 1), new Vector2(8, 0), new Vector2(-16, 0));

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            SetRect(fill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.sprite = fillSprite;
            fillImg.type = Image.Type.Sliced;
            fillImg.color = Color.white;
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Handle Slide Area & Knob
            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderObj.transform, false);
            SetRect(handleArea, new Vector2(0, 0), new Vector2(1, 1), new Vector2(15, 0), new Vector2(-30, 0));

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleArea.transform, false);
            SetRect(handle, new Vector2(0, 0.5f), new Vector2(0, 0.5f), Vector2.zero, new Vector2(56, 56));
            Image handleImg = handle.GetComponent<Image>();
            handleImg.sprite = knobSprite;
            handleImg.preserveAspect = true;
            handleImg.color = Color.white;
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImg;

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

        private static void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2? pivot = null)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (pivot.HasValue) rt.pivot = pivot.Value;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static void BindAudioClips(BlockAudioManager audioMgr)
        {
            if (audioMgr == null) return;

            // 1. 6 Dedicated SFX Clips specified by user
            audioMgr.sfxBuy = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/buy.wav");
            audioMgr.sfxClick = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/click.wav");
            audioMgr.sfxGrab = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/grab.wav");
            audioMgr.sfxGrabDown = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/grab-down.wav");
            audioMgr.sfxWarning = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/warning.wav");
            audioMgr.sfxWindow = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/SFX/window.wav");

            // 2. BGM Auto-Binding from dedicated subfolders
            audioMgr.bgmIntro = FindFirstAudioClipInFolder("Assets/Sounds/BGM/Intro");
            audioMgr.bgmLobbyTracks = FindAudioClipsInFolderSorted("Assets/Sounds/BGM/Lobby");
            if (audioMgr.bgmLobbyTracks.Count > 0) audioMgr.bgmLobby = audioMgr.bgmLobbyTracks[0];
            audioMgr.bgmInGamePlaylist = FindAudioClipsInFolderSorted("Assets/Sounds/BGM/InGame");

            // 3. Default volume 50% slider calibration
            audioMgr.sfxSliderLevel = BlockAudioManager.DEFAULT_SLIDER_PERCENT;
            audioMgr.bgmSliderLevel = BlockAudioManager.DEFAULT_SLIDER_PERCENT;

            EditorUtility.SetDirty(audioMgr);
        }

        private static AudioClip FindFirstAudioClipInFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath)) return null;
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new string[] { folderPath });
            if (guids != null && guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            }
            return null;
        }

        private static System.Collections.Generic.List<AudioClip> FindAudioClipsInFolderSorted(string folderPath)
        {
            var list = new System.Collections.Generic.List<AudioClip>();
            if (!AssetDatabase.IsValidFolder(folderPath)) return list;

            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new string[] { folderPath });
            if (guids == null || guids.Length == 0) return list;

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null) list.Add(clip);
            }

            list.Sort((a, b) =>
            {
                int numA = ExtractTrackNumber(a.name);
                int numB = ExtractTrackNumber(b.name);
                if (numA != numB) return numA.CompareTo(numB);
                return string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase);
            });

            return list;
        }

        private static int ExtractTrackNumber(string name)
        {
            var match = System.Text.RegularExpressions.Regex.Match(name, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int result))
            {
                return result;
            }
            return int.MaxValue;
        }
    }
}
#endif
