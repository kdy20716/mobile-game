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
            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetupPastelBlockSprites(pinkBlockSprite, mintBlockSprite, goldBlockSprite, purpleBlockSprite, blueBlockSprite, starBombBlockSprite);

            // ==========================================
            // CUTE PASTEL UI HIERARCHY (RESPONSIVE)
            // ==========================================

            // --- In-Game Root Container (Starts inactive so it NEVER shows during main menu or lobby!) ---
            GameObject inGameRoot = new GameObject("InGameRoot", typeof(RectTransform));
            inGameRoot.transform.SetParent(canvasObj.transform, false);
            SetRect(inGameRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1080f, 0f));
            inGameRoot.SetActive(false);

            // --- A. Header (Top Anchor) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(inGameRoot.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -115), new Vector2(-60, 160));

            // Pause Button (Far Left: pos 52, size 86x86 - high visibility, non-overlapping)
            Sprite pauseBarsSprite = CuteBlockTextureGenerator.GetOrCreatePauseBarsSprite();
            GameObject pauseBtnObj = CreateButton(headerObj.transform, "BtnPause", "", cuteFont, new Vector2(0, 0.5f), new Vector2(52, 0), new Vector2(86, 86), btnPauseCircleSprite, 20);
            
            // Icon: Pause Bars (||)
            GameObject pIcon = CreateImage(pauseBtnObj.transform, "PauseBarsIcon", pauseBarsSprite, new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(32, 32));
            pIcon.GetComponent<Image>().raycastTarget = false;
            
            // Label: "일시정지" (Clean Korean text, 100% supported by Jua SDF font)
            GameObject pLabel = CreateText(pauseBtnObj.transform, "PauseLabel", "일시정지", 17, TextAlignmentOptions.Center, cuteFont, Color.white, true, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            SetRect(pLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -21), new Vector2(80, 22));
            pLabel.GetComponent<TMP_Text>().raycastTarget = false;

            // Current Score (Left-Center)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(185, 0), new Vector2(160, 110));
            Image sBoxImg = scoreBox.GetComponent<Image>();
            sBoxImg.sprite = scoreBoxSprite;
            sBoxImg.type = Image.Type.Sliced;

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 20, TextAlignmentOptions.Center, cuteFont, new Color(0.32f, 0.18f, 0.52f));
            SetRect(scoreLabel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 24), new Vector2(140, 26));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 44, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.40f, 0.02f)); // Warm Golden Honey
            SetRect(scoreVal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -16), new Vector2(140, 48));

            // Center Title (3D Jelly Mallang Logo Banner)
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform), typeof(Image));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(12, 6), new Vector2(460, 160));
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
            GameObject skipBtnObj = CreateButton(skillsBar.transform, "BtnSkip", "", cuteFont, new Vector2(1, 0.5f), new Vector2(-215, 0), new Vector2(96, 96), btnSkipUser, 32);
            skipBtnObj.AddComponent<CanvasGroup>(); // For dimming/brightening skip button
            var btnSkip = skipBtnObj.GetComponent<Button>();

            // Stock Badge on Skip Button
            GameObject skipBadgeObj = new GameObject("SkipStockBadge", typeof(RectTransform), typeof(Image));
            skipBadgeObj.transform.SetParent(skipBtnObj.transform, false);
            SetRect(skipBadgeObj, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, -8f), new Vector2(104f, 24f));
            Image badgeBg = skipBadgeObj.GetComponent<Image>();
            badgeBg.sprite = tabPillSprite;
            badgeBg.type = Image.Type.Sliced;
            badgeBg.color = new Color(0.15f, 0.12f, 0.22f, 0.85f);

            GameObject badgeTextObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            badgeTextObj.transform.SetParent(skipBadgeObj.transform, false);
            SetRect(badgeTextObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            TMP_Text skipBadgeTMP = badgeTextObj.GetComponent<TMP_Text>();
            skipBadgeTMP.font = cuteFont;
            skipBadgeTMP.fontSize = 12.5f;
            skipBadgeTMP.fontStyle = FontStyles.Bold;
            skipBadgeTMP.alignment = TextAlignmentOptions.Center;
            skipBadgeTMP.color = Color.white;
            skipBadgeTMP.raycastTarget = false;
            skipBadgeTMP.text = "1/3 (0/10)";

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
            SetRect(finalLabel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -16), new Vector2(340, 32));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 64, TextAlignmentOptions.Center, cuteFont, new Color(0.95f, 0.24f, 0.48f));
            SetRect(finalVal, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -60), new Vector2(340, 62));

            string bestPrefix = LocalizationManager.Get("profile_best_score_prefix", "BEST");
            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", $"{bestPrefix}: 0", 24, TextAlignmentOptions.Center, cuteFont, new Color(0.48f, 0.42f, 0.62f));
            SetRect(mBestVal, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 14), new Vector2(340, 28));

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
                lobbyBtn.GetComponent<Button>()
            );

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
            GameObject rSub = CreateText(rCard.transform, "Subtitle", "지금까지의 점수가 사라져요 🥺", 26, TextAlignmentOptions.Center, cuteFont, new Color(0.52f, 0.40f, 0.68f));
            SetRect(rSub, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -128), new Vector2(580, 36));

            // Desc
            GameObject rDesc = CreateText(rCard.transform, "Desc", "그래도 다시 시작하시겠어요?", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(rDesc, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -185), new Vector2(600, 36));

            // Buttons
            GameObject rYesBtn = CreateButton(rCard.transform, "BtnYes", "다시 시작! 🎮", cuteFont, new Vector2(0.5f, 0f), new Vector2(-140, 80), new Vector2(270, 68), btnPinkSprite, 26, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));
            GameObject rNoBtn  = CreateButton(rCard.transform, "BtnNo",  "계속하기 💪", cuteFont, new Vector2(0.5f, 0f), new Vector2( 140, 80), new Vector2(270, 68), btnTealSprite, 26, Color.white, new Color(0.18f, 0.55f, 0.45f, 0.85f));

            uiMgr.SetupRestartConfirmModal(rConfirmModal, rYesBtn.GetComponent<Button>(), rNoBtn.GetComponent<Button>());

            // --- E4. White Flash Overlay (full-screen for restart transition) ---
            GameObject whiteFlash = new GameObject("WhiteFlashOverlay", typeof(RectTransform), typeof(Image));
            whiteFlash.transform.SetParent(inGameRoot.transform, false);
            SetRect(whiteFlash, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image wfImg = whiteFlash.GetComponent<Image>();
            wfImg.color = new Color(1f, 1f, 1f, 0f);
            wfImg.raycastTarget = true; // block input during flash
            whiteFlash.SetActive(false);

            uiMgr.SetupWhiteFlashOverlay(wfImg);

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

            Sprite pinkMascotSprite = CuteBlockTextureGenerator.GetOrCreateLeftPoppingMascotSprite();
            Sprite mintMascotSprite = CuteBlockTextureGenerator.GetOrCreateRightPoppingMascotSprite();
            Sprite goldMascotSprite = CuteBlockTextureGenerator.GetOrCreateGoldMascotSprite();
            Sprite purpleMascotSprite = CuteBlockTextureGenerator.GetOrCreatePurpleMascotSprite();
            Sprite closeXBtnSprite = CuteBlockTextureGenerator.GetOrCreateCloseXButtonSprite();

            Sprite[] mascotAvatars = new Sprite[] { pinkMascotSprite, mintMascotSprite, goldMascotSprite, purpleMascotSprite };

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

            // Big Avatar
            GameObject pBigAvFrame = CreateImage(pCard.transform, "AvFrame", avatarCircleFrameSprite, new Vector2(0.5f, 1), new Vector2(0, -180), new Vector2(160, 160));
            GameObject pBigAv = CreateImage(pBigAvFrame.transform, "AvIcon", pinkMascotSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(135, 135));
            Image pBigAvImg = pBigAv.GetComponent<Image>();
            pBigAvImg.preserveAspect = true;

            // Nickname (Editable Input Field with touch support)
            GameObject pNickInputObj = CreateInputField(pCard.transform, "NickInput", "말랑이#0000", "닉네임을 입력하세요", cuteFont, new Vector2(0, 240), new Vector2(460, 68), inputPillSprite);
            TMP_InputField pNickInput = pNickInputObj.GetComponent<TMP_InputField>();
            pNickInput.characterLimit = 12;
            if (pNickInput.textComponent != null)
            {
                pNickInput.textComponent.alignment = TextAlignmentOptions.Center;
                pNickInput.textComponent.fontSize = 32;
                pNickInput.textComponent.color = new Color(0.25f, 0.15f, 0.45f);
            }
            if (pNickInput.placeholder is TMP_Text phText)
            {
                phText.alignment = TextAlignmentOptions.Center;
                phText.fontSize = 26;
            }

            // Stats row (Best score & Coins)
            GameObject pStats = CreateText(pCard.transform, "Stats", "최고 점수: 0점  |  보유 코인: 1,000 C", 28, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.45f, 0.05f));
            SetRect(pStats, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -345), new Vector2(650, 40));

            // Bio Section
            GameObject pBioLbl = CreateText(pCard.transform, "BioLbl", "자기소개 (터치하여 수정)", 26, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.35f, 0.55f));
            SetRect(pBioLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -405), new Vector2(680, 35));

            GameObject pBioInputObj = CreateInputField(pCard.transform, "BioInput", "말랑블라스트에 오신 걸 환영해요!", "자기소개를 입력해주세요", cuteFont, new Vector2(0, 45), new Vector2(680, 85), inputPillSprite);
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
            GameObject pLogoutObj = CreateButton(pCard.transform, "BtnLogout", "로그아웃", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 95), new Vector2(340, 80), btnLavenderSprite, 32, Color.white, new Color(0.48f, 0.32f, 0.72f, 0.85f));
            Button pLogoutBtn = pLogoutObj.GetComponent<Button>();

            // --- Shop Modal (Background Theme Store) ---
            Sprite themeCandySprite = CuteBlockTextureGenerator.GetOrCreateCandyWonderlandBackgroundSprite();
            Sprite themeMermaidSprite = CuteBlockTextureGenerator.GetOrCreateCrystalMermaidBackgroundSprite();
            Sprite themeNebulaSprite = CuteBlockTextureGenerator.GetOrCreateStarryNebulaBackgroundSprite();
            Sprite[] allThemeSprites = new Sprite[] { bgSprite, themeCandySprite, themeMermaidSprite, themeNebulaSprite };

            Sprite lobbyCandySprite = CuteBlockTextureGenerator.GetOrCreateLobbyCandyStageSprite();
            Sprite lobbyOceanSprite = CuteBlockTextureGenerator.GetOrCreateLobbyOceanStageSprite();
            Sprite[] allLobbySprites = new Sprite[] { lobbyBgSprite, lobbyCandySprite, lobbyOceanSprite };

            GameObject sModal = new GameObject("ShopModal", typeof(RectTransform));
            sModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(sModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject sDarkBg = CreateImage(sModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            sDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            sDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject sCard = CreateImage(sModal.transform, "DialogCard", shopModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860, 1140));
            sCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject sCloseBtn = CreateButton(sCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject sTitle = CreateText(sCard.transform, "Title", "배경 테마 상점", 42, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -55), new Vector2(500, 50));

            GameObject sCoinTxt = CreateText(sCard.transform, "Coins", "내 코인: 1,000 C", 30, TextAlignmentOptions.Center, cuteFont, new Color(0.92f, 0.45f, 0.05f));
            SetRect(sCoinTxt, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -98), new Vector2(500, 36));

            // --- Shop Tabs (Game Theme vs Lobby Theme) ---
            Sprite tabGameActiveSprite = CuteBlockTextureGenerator.GetOrCreateShopGameTabButtonSprite();
            Sprite tabLobbyActiveSprite = CuteBlockTextureGenerator.GetOrCreateShopLobbyTabButtonSprite();
            Sprite tabInactiveSprite = CuteBlockTextureGenerator.GetOrCreateShopInactiveTabButtonSprite();

            GameObject tabInGameBtnObj = CreateButton(sCard.transform, "TabInGame", "게임 테마", cuteFont, new Vector2(0.5f, 1), new Vector2(-185, -162), new Vector2(355, 84), tabGameActiveSprite, 26);
            GameObject tabLobbyBtnObj = CreateButton(sCard.transform, "TabLobby", "로비 테마", cuteFont, new Vector2(0.5f, 1), new Vector2(185, -162), new Vector2(355, 84), tabInactiveSprite, 26);

            Button tabInGameBtn = tabInGameBtnObj.GetComponent<Button>();
            Button tabLobbyBtn = tabLobbyBtnObj.GetComponent<Button>();
            Image tabInGameBg = tabInGameBtnObj.GetComponent<Image>();
            Image tabLobbyBg = tabLobbyBtnObj.GetComponent<Image>();
            tabInGameBg.type = Image.Type.Sliced;
            tabInGameBg.preserveAspect = false;
            tabInGameBg.raycastTarget = true;
            tabLobbyBg.type = Image.Type.Sliced;
            tabLobbyBg.preserveAspect = false;
            tabLobbyBg.raycastTarget = true;

            TMP_Text tabInGameTxt = tabInGameBtnObj.GetComponentInChildren<TMP_Text>();
            TMP_Text tabLobbyTxt = tabLobbyBtnObj.GetComponentInChildren<TMP_Text>();

            // --- InGame Themes Panel ---
            GameObject inGamePanel = new GameObject("InGameThemesPanel", typeof(RectTransform));
            inGamePanel.transform.SetParent(sCard.transform, false);
            RectTransform inGameRT = inGamePanel.GetComponent<RectTransform>();
            inGameRT.pivot = new Vector2(0.5f, 1f);
            SetRect(inGamePanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -215), new Vector2(800, 890));

            string[] themeTitles = new string[] { "몽환의 밤", "캔디 랜드", "크리스탈 바다", "별빛 우주" };
            string[] themeDescs = new string[] { "기본 테마 - 달콤한 보랏빛 밤", "달콤한 디저트와 사탕 세상", "신비로운 반짝임의 바다 궁전", "아름다운 보랏빛 은하수 별빛" };

            Button[] themeActionBtns = new Button[4];
            TMP_Text[] themeActionTMPs = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                float yPos = -95f - i * 192f;
                // Outer Card Box
                GameObject itCard = CreateImage(inGamePanel.transform, $"ThemeItem_{i}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(780, 172));
                itCard.GetComponent<Image>().type = Image.Type.Sliced;
                itCard.GetComponent<Image>().color = new Color(0.97f, 0.95f, 1f, 0.96f);

                // Thumbnail Frame & Image
                GameObject thumbFrame = CreateImage(itCard.transform, "ThumbFrame", tabPillSprite, new Vector2(0, 0.5f), new Vector2(105, 0), new Vector2(150, 135));
                thumbFrame.GetComponent<Image>().type = Image.Type.Sliced;
                thumbFrame.GetComponent<Image>().color = new Color(0.85f, 0.80f, 0.95f, 0.9f);

                GameObject thumbImg = CreateImage(thumbFrame.transform, "Thumb", allThemeSprites[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(140, 125));
                thumbImg.GetComponent<Image>().preserveAspect = false;

                // Title & Subtitle Texts
                GameObject itName = CreateText(itCard.transform, "Name", themeTitles[i], 32, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(itName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(330, 26), new Vector2(270, 42));

                GameObject itDesc = CreateText(itCard.transform, "Desc", themeDescs[i], 22, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.48f, 0.65f));
                SetRect(itDesc, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(330, -22), new Vector2(270, 36));

                // Action Button (Equip / Buy) - Dedicated customizable graphic with light pink interior and high contrast font
                Sprite initBtnSprite = (i == 0) ? shopEquippedBtnSprite : shopEquipBtnSprite;
                string initBtnLabel = (i == 0) ? "적용 중" : "장착하기";
                GameObject actBtnObj = CreateButton(itCard.transform, $"BtnAction_{i}", initBtnLabel, cuteFont, new Vector2(1, 0.5f), new Vector2(-105, 0), new Vector2(185, 75), initBtnSprite, 26);
                actBtnObj.GetComponent<Image>().color = Color.white;
                themeActionBtns[i] = actBtnObj.GetComponent<Button>();
                themeActionTMPs[i] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (themeActionTMPs[i] != null)
                {
                    themeActionTMPs[i].color = (i == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }
            }

            // --- Lobby Themes Panel ---
            GameObject lobbyPanel = new GameObject("LobbyThemesPanel", typeof(RectTransform));
            lobbyPanel.transform.SetParent(sCard.transform, false);
            RectTransform lobbyRT = lobbyPanel.GetComponent<RectTransform>();
            lobbyRT.pivot = new Vector2(0.5f, 1f);
            SetRect(lobbyPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -215), new Vector2(800, 890));
            lobbyPanel.SetActive(false);

            Button[] lobbyThemeActionBtns = new Button[3];
            TMP_Text[] lobbyThemeActionTMPs = new TMP_Text[3];

            for (int i = 0; i < 3; i++)
            {
                float yPos = -115f - i * 230f;
                // Outer Card Box
                GameObject itCard = CreateImage(lobbyPanel.transform, $"LobbyThemeItem_{i}", shopItemCardSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(780, 205));
                itCard.GetComponent<Image>().type = Image.Type.Sliced;
                itCard.GetComponent<Image>().color = new Color(0.97f, 0.95f, 1f, 0.96f);

                // Thumbnail Frame & Image
                GameObject thumbFrame = CreateImage(itCard.transform, "ThumbFrame", tabPillSprite, new Vector2(0, 0.5f), new Vector2(115, 0), new Vector2(170, 165));
                thumbFrame.GetComponent<Image>().type = Image.Type.Sliced;
                thumbFrame.GetComponent<Image>().color = new Color(0.85f, 0.80f, 0.95f, 0.9f);

                GameObject thumbImg = CreateImage(thumbFrame.transform, "Thumb", allLobbySprites[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(160, 155));
                thumbImg.GetComponent<Image>().preserveAspect = false;

                // Title & Subtitle Texts
                GameObject itName = CreateText(itCard.transform, "Name", LobbyManager.LobbyThemeNames[i], 32, TextAlignmentOptions.Left, cuteFont, new Color(0.28f, 0.18f, 0.48f));
                SetRect(itName, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(345, 28), new Vector2(270, 42));

                GameObject itDesc = CreateText(itCard.transform, "Desc", LobbyManager.LobbyThemeDescs[i], 22, TextAlignmentOptions.Left, cuteFont, new Color(0.55f, 0.48f, 0.65f));
                SetRect(itDesc, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(345, -22), new Vector2(270, 36));

                // Action Button (Equip / Buy) - Dedicated customizable graphic with light pink interior and high contrast font
                Sprite initLobbyBtnSprite = (i == 0) ? shopEquippedBtnSprite : shopEquipBtnSprite;
                string initLobbyBtnLabel = (i == 0) ? "적용 중" : "장착하기";
                GameObject actBtnObj = CreateButton(itCard.transform, $"BtnAction_{i}", initLobbyBtnLabel, cuteFont, new Vector2(1, 0.5f), new Vector2(-105, 0), new Vector2(185, 75), initLobbyBtnSprite, 26);
                actBtnObj.GetComponent<Image>().color = Color.white;
                lobbyThemeActionBtns[i] = actBtnObj.GetComponent<Button>();
                lobbyThemeActionTMPs[i] = actBtnObj.GetComponentInChildren<TMP_Text>();
                if (lobbyThemeActionTMPs[i] != null)
                {
                    lobbyThemeActionTMPs[i].color = (i == 0) ? new Color(0.06f, 0.35f, 0.26f, 1f) : new Color(0.46f, 0.08f, 0.24f, 1f);
                }
            }

            // Ensure tab buttons and close button render above both panels for guaranteed raycast clicks
            tabInGameBtnObj.transform.SetAsLastSibling();
            tabLobbyBtnObj.transform.SetAsLastSibling();
            sCloseBtn.transform.SetAsLastSibling();

            // --- Settings Modal ---
            GameObject setModal = new GameObject("SettingsModal", typeof(RectTransform));
            setModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(setModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject setDarkBg = CreateImage(setModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            setDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            setDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject setCard = CreateImage(setModal.transform, "DialogCard", settingsModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(780, 780));
            setCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject setCloseBtn = CreateButton(setCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject setTitle = CreateText(setCard.transform, "Title", "게임 설정", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(setTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(400, 50));

            // BGM Slider
            GameObject bgmLbl = CreateText(setCard.transform, "BGMLbl", "배경음악 (BGM)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(bgmLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -125), new Vector2(580, 34));
            GameObject bgmSldObj = CreateSlider(setCard.transform, "BGMSlider", new Vector2(0, -172), new Vector2(580, 42), 0.7f, sliderTrackSprite, sliderFillSprite, sliderKnobSprite);

            // SFX Slider
            GameObject sfxLbl = CreateText(setCard.transform, "SFXLbl", "효과음 (SFX)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(sfxLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -238), new Vector2(580, 34));
            GameObject sfxSldObj = CreateSlider(setCard.transform, "SFXSlider", new Vector2(0, -285), new Vector2(580, 42), 0.85f, sliderTrackSprite, sliderFillSprite, sliderKnobSprite);

            // Language Selection
            GameObject langLbl = CreateText(setCard.transform, "LangLbl", "언어 설정 (Language)", 28, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(langLbl, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -350), new Vector2(580, 34));

            string[] langNames = new string[] { "한국어", "English", "日本語", "中文" };
            float[] langXOffsets = new float[] { -219f, -73f, 73f, 219f };
            Button[] langBtns = new Button[4];
            Image[] langBgs = new Image[4];
            TMP_Text[] langTexts = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                GameObject lBtnObj = CreateButton(setCard.transform, $"BtnLang_{i}", langNames[i], cuteFont, new Vector2(0.5f, 1), new Vector2(langXOffsets[i], -405), new Vector2(136, 50), tabPillSprite, 22);
                langBtns[i] = lBtnObj.GetComponent<Button>();
                langBgs[i] = lBtnObj.GetComponent<Image>();
                langTexts[i] = lBtnObj.GetComponentInChildren<TMP_Text>();
            }

            // Row 1: Aspect Ratio Selection (16:9, 16:10, 4:3, 9:16)
            GameObject aspectTitle = CreateText(setCard.transform, "AspectTitle", LocalizationManager.Get("settings_aspect_ratio_title"), 26, TextAlignmentOptions.Left, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(aspectTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -452), new Vector2(580, 32));

            string[] aspectNames = new string[] { "16:9", "16:10", "4:3", "9:16" };
            float[] aspectXOffsets = new float[] { -219f, -73f, 73f, 219f };
            Button[] aspectBtns = new Button[4];
            Image[] aspectBgs = new Image[4];
            TMP_Text[] aspectTexts = new TMP_Text[4];

            for (int i = 0; i < 4; i++)
            {
                Sprite initSprite = (i == 3) ? btnPinkSprite : tabPillSprite;
                GameObject aBtnObj = CreateButton(setCard.transform, $"BtnAspect_{i}", aspectNames[i], cuteFont, new Vector2(0.5f, 1), new Vector2(aspectXOffsets[i], -502), new Vector2(136, 48), initSprite, 22);
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
            SetRect(winModeTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -565), new Vector2(580, 32));

            string[] winModeNames = new string[] { "창 모드", "테두리 없는 창", "전체화면" };
            float[] winModeXOffsets = new float[] { -196f, 0f, 196f };
            Button[] winModeBtns = new Button[3];
            Image[] winModeBgs = new Image[3];
            TMP_Text[] winModeTexts = new TMP_Text[3];

            for (int j = 0; j < 3; j++)
            {
                Sprite initSprite = (j == 0) ? btnPinkSprite : tabPillSprite;
                GameObject wBtnObj = CreateButton(setCard.transform, $"BtnWindowMode_{j}", winModeNames[j], cuteFont, new Vector2(0.5f, 1), new Vector2(winModeXOffsets[j], -615), new Vector2(186, 48), initSprite, 21);
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
            SetRect(verTxt, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 35), new Vector2(600, 36));

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

            // --- Help Modal (NEW!) ---
            GameObject hModal = new GameObject("HelpModal", typeof(RectTransform));
            hModal.transform.SetParent(lobbyRoot.transform, false);
            SetRect(hModal, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(4000f, 0f));

            GameObject hDarkBg = CreateImage(hModal.transform, "DarkBg", null, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            hDarkBg.GetComponent<Image>().color = new Color(0.06f, 0.04f, 0.14f, 0.88f);
            hDarkBg.GetComponent<Image>().raycastTarget = true;

            GameObject hCard = CreateImage(hModal.transform, "DialogCard", helpModalCardSprite, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(840, 930));
            hCard.GetComponent<Image>().type = Image.Type.Sliced;

            GameObject hCloseBtn = CreateButton(hCard.transform, "BtnClose", "", cuteFont, new Vector2(1, 1), new Vector2(-45, -45), new Vector2(65, 65), closeXBtnSprite, 32);

            GameObject hTitle = CreateText(hCard.transform, "Title", "말랑블라스트 가이드", 40, TextAlignmentOptions.Center, cuteFont, new Color(0.35f, 0.22f, 0.55f));
            SetRect(hTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -50), new Vector2(500, 48));

            GameObject hSubTitle = CreateText(hCard.transform, "Subtitle", "달콤하고 쉬운 말랑이 블록 퍼즐 룰!", 23, TextAlignmentOptions.Center, cuteFont, new Color(0.55f, 0.45f, 0.70f));
            SetRect(hSubTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -96), new Vector2(600, 30));

            string[] stepTitles = new string[]
            {
                "말랑 젤리 블록 놓기",
                "가로 / 세로 줄 폭파",
                "달콤한 콤보 보너스",
                "긴장감 넘치는 타임어택"
            };

            string[] stepDescs = new string[]
            {
                "하단 3개의 블록을 터치 & 드래그하여 보드판에 올려놓아요.",
                "가로 또는 세로 한 줄을 빈틈없이 채우면 팡팡 터져요!",
                "연속으로 줄을 터뜨리면 피버 보너스 점수를 획득해요!",
                "점수가 오를수록 제한 시간이 점점 줄어드니 서두르세요!"
            };

            Color[] stepBadgeColors = new Color[]
            {
                new Color(1f, 0.35f, 0.55f),     // Pink
                new Color(0.20f, 0.80f, 0.65f),   // Mint
                new Color(1f, 0.70f, 0.10f),     // Gold
                new Color(0.65f, 0.35f, 0.95f)    // Purple
            };

            Color[] stepRowBgColors = new Color[]
            {
                new Color(1f, 0.95f, 0.97f, 0.95f),
                new Color(0.93f, 0.98f, 0.96f, 0.95f),
                new Color(1f, 0.98f, 0.91f, 0.95f),
                new Color(0.96f, 0.94f, 1f, 0.95f)
            };

            for (int i = 0; i < 4; i++)
            {
                float yPos = -198f - i * 150f;
                // Outer Step Card (Larger height for spacious readability)
                GameObject hRowCard = CreateImage(hCard.transform, $"HelpRow_{i}", scoreBoxSprite, new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(764, 140));
                hRowCard.GetComponent<Image>().type = Image.Type.Sliced;
                hRowCard.GetComponent<Image>().color = stepRowBgColors[i];

                // Left Circle Frame with Mascot Avatar
                GameObject avFrame = CreateImage(hRowCard.transform, "AvFrame", circleFrameSprite, new Vector2(0, 0.5f), new Vector2(68, 0), new Vector2(98, 98));
                GameObject avIcon = CreateImage(avFrame.transform, "AvIcon", mascotAvatars[i], new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80, 80));
                avIcon.GetComponent<Image>().preserveAspect = true;

                // Step Pill Badge
                GameObject stepBadge = CreateImage(hRowCard.transform, "StepBadge", tabPillSprite, new Vector2(0, 0.5f), new Vector2(178, 25), new Vector2(100, 36));
                stepBadge.GetComponent<Image>().type = Image.Type.Sliced;
                stepBadge.GetComponent<Image>().color = stepBadgeColors[i];

                GameObject stepBadgeTxt = CreateText(stepBadge.transform, "BadgeText", $"STEP {i + 1}", 20, TextAlignmentOptions.Center, cuteFont, Color.white, true);
                SetRect(stepBadgeTxt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                // Step Title (Left-aligned, zero overlap with badge)
                GameObject stepTitleTxt = CreateText(hRowCard.transform, "StepTitle", stepTitles[i], 30, TextAlignmentOptions.Left, cuteFont, new Color(0.25f, 0.16f, 0.42f));
                RectTransform titleRT = stepTitleTxt.GetComponent<RectTransform>();
                titleRT.pivot = new Vector2(0f, 0.5f);
                SetRect(stepTitleTxt, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(240, 25), new Vector2(500, 36));

                // Step Description (Left-aligned, cleanly under badge and title)
                GameObject stepDescTxt = CreateText(hRowCard.transform, "StepDesc", stepDescs[i], 24, TextAlignmentOptions.Left, cuteFont, new Color(0.45f, 0.38f, 0.55f));
                RectTransform descRT = stepDescTxt.GetComponent<RectTransform>();
                descRT.pivot = new Vector2(0f, 0.5f);
                SetRect(stepDescTxt, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(128, -25), new Vector2(620, 44));
            }

            // Bottom Confirmation Button: "이해했어요! (닫기)" (Shifted up with generous bottom padding)
            GameObject hConfirmBtnObj = CreateButton(hCard.transform, "BtnConfirm", "이해했어요!", cuteFont, new Vector2(0.5f, 0), new Vector2(0, 75), new Vector2(480, 80), btnPinkSprite, 34, Color.white, new Color(0.78f, 0.28f, 0.44f, 0.85f));

            // Setup LobbyManager References
            lobbyMgr.SetupReferences(
                lobbyRoot, lobbyCG,
                pBtn, pAvatarImg, coinLabel.GetComponent<TMP_Text>(),
                partyMascotRTs, partyLabelObjs, partyLabelBgImgs, partyLabelTMPs,
                playBtn, playBtnBg, playBtnText,
                pModal, pBigAvImg, pNickInput.textComponent, pBioInput, pStats.GetComponent<TMP_Text>(), pStats.GetComponent<TMP_Text>(), avPickBtns, pLogoutBtn, pCloseBtn.GetComponent<Button>(),
                sModal, sCoinTxt.GetComponent<TMP_Text>(), sCloseBtn.GetComponent<Button>(),
                setModal, bgmSldObj.GetComponent<Slider>(), sfxSldObj.GetComponent<Slider>(), setCloseBtn.GetComponent<Button>(),
                hModal, hCloseBtn.GetComponent<Button>(),
                mascotAvatars,
                partyGlowObjs,
                pNickInput,
                hConfirmBtnObj.GetComponent<Button>(),
                playGlowImg,
                partyTip.GetComponent<TMP_Text>()
            );

            lobbyMgr.SetupShopButtonSprites(shopEquipBtnSprite, shopEquippedBtnSprite);
            lobbyMgr.SetupShopTabSprites(tabGameActiveSprite, tabLobbyActiveSprite, tabInactiveSprite, tabPillSprite);
            lobbyMgr.SetupShopTabs(tabInGameBtn, tabLobbyBtn, tabInGameBg, tabLobbyBg, tabInGameTxt, tabLobbyTxt, inGamePanel, lobbyPanel);
            lobbyMgr.SetupShopThemes(bgImgComp, allThemeSprites, themeActionBtns, themeActionTMPs);
            lobbyMgr.SetupLobbyThemes(lobbyBg.GetComponent<Image>(), allLobbySprites, lobbyThemeActionBtns, lobbyThemeActionTMPs);
            lobbyMgr.SetupLanguageButtons(
                langBtns, langBgs, langTexts,
                setTitle.GetComponent<TMP_Text>(),
                bgmLbl.GetComponent<TMP_Text>(),
                sfxLbl.GetComponent<TMP_Text>(),
                langLbl.GetComponent<TMP_Text>(),
                sTitle.GetComponent<TMP_Text>(),
                hTitle.GetComponent<TMP_Text>(),
                hConfirmBtnObj.GetComponentInChildren<TMP_Text>(),
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
            EditorUtility.SetDirty(lobbyMgr);

            // 6. Fairy Screen Transition Overlay (Dissolve + 1-Sec Corner Sparkles)
            Sprite fairyRippleSprite = CuteBlockTextureGenerator.GetOrCreateFairyRippleSprite();
            Sprite fairySparkleSprite = CuteBlockTextureGenerator.GetOrCreateFairySparkleSprite();
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
            AssetDatabase.Refresh();

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
