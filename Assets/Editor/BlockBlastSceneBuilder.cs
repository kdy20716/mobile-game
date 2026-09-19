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

            // 1. Setup Camera
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.06f, 0.1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";

            // 2. Load & Configure Texture Sprites
            Sprite gemSprite = LoadOrCreateSprite("Assets/Textures/BlockBlast/gem_block_tile.jpg");
            Sprite bombSprite = LoadOrCreateSprite("Assets/Textures/BlockBlast/bomb_block_icon.jpg");
            Sprite hammerSprite = LoadOrCreateSprite("Assets/Textures/BlockBlast/skill_hammer_icon.jpg");

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

            // 4. Canvas & Scaler (Mobile Portrait: 1080 x 1920)
            GameObject canvasObj = new GameObject("BlockBlastCanvas", typeof(RectTransform));
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f; // match width for mobile portrait

            canvasObj.AddComponent<GraphicRaycaster>();

            // 5. Game Managers Object
            GameObject mgrObj = new GameObject("GameManagers");
            var gridMgr = mgrObj.AddComponent<BlockGridManager>();
            var spawner = mgrObj.AddComponent<BlockSpawner>();
            var audioMgr = mgrObj.AddComponent<BlockAudioManager>();
            var uiMgr = canvasObj.AddComponent<BlockBlastUIManager>();

            gridMgr.SetSprites(gemSprite, bombSprite);

            // ==========================================
            // UI HIERARCHY
            // ==========================================

            // --- A. Header (Top) ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(canvasObj.transform, false);
            SetRect(headerObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -110), new Vector2(-60, 140));

            // Current Score (Top Left)
            GameObject scoreBox = new GameObject("ScoreBox", typeof(RectTransform));
            scoreBox.transform.SetParent(headerObj.transform, false);
            SetRect(scoreBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(120, 0), new Vector2(240, 120));

            GameObject scoreLabel = CreateText(scoreBox.transform, "Label", "SCORE", 26, TextAnchor.MiddleLeft, defaultFont, new Color(0.6f, 0.65f, 0.75f));
            SetRect(scoreLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 40));

            GameObject scoreVal = CreateText(scoreBox.transform, "Value", "0", 52, TextAnchor.MiddleLeft, defaultFont, new Color(1f, 0.82f, 0.15f));
            SetRect(scoreVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 20), new Vector2(0, 60));

            // Title (Center)
            GameObject titleBox = new GameObject("TitleBox", typeof(RectTransform));
            titleBox.transform.SetParent(headerObj.transform, false);
            SetRect(titleBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(340, 120));

            GameObject mainTitle = CreateText(titleBox.transform, "MainTitle", "BLOCK BLAST", 36, TextAnchor.MiddleCenter, defaultFont, new Color(0.2f, 0.85f, 1f));
            SetRect(mainTitle, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -25), new Vector2(0, 45));

            GameObject subTitle = CreateText(titleBox.transform, "SubTitle", "E L E M E N T A L", 20, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.65f, 0.2f));
            SetRect(subTitle, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 25), new Vector2(0, 35));

            // Best Score (Top Right)
            GameObject bestBox = new GameObject("BestBox", typeof(RectTransform));
            bestBox.transform.SetParent(headerObj.transform, false);
            SetRect(bestBox, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-120, 0), new Vector2(240, 120));

            GameObject bestLabel = CreateText(bestBox.transform, "Label", "BEST", 26, TextAnchor.MiddleRight, defaultFont, new Color(0.6f, 0.65f, 0.75f));
            SetRect(bestLabel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 40));

            GameObject bestVal = CreateText(bestBox.transform, "Value", "0", 52, TextAnchor.MiddleRight, defaultFont, Color.white);
            SetRect(bestVal, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 20), new Vector2(0, 60));

            // --- B. Skills Bar (Below Header) ---
            GameObject skillsBar = new GameObject("SkillsBar", typeof(RectTransform));
            skillsBar.transform.SetParent(canvasObj.transform, false);
            SetRect(skillsBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -230), new Vector2(980, 85));
            Image sBarBg = skillsBar.AddComponent<Image>();
            sBarBg.color = new Color(0.08f, 0.11f, 0.16f, 0.9f);

            // Fever Gauge Bar
            GameObject feverLabel = CreateText(skillsBar.transform, "FeverLabel", "FEVER", 24, TextAnchor.MiddleLeft, defaultFont, new Color(1f, 0.8f, 0.2f));
            SetRect(feverLabel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(75, 0), new Vector2(100, 40));

            GameObject feverBg = new GameObject("FeverBg", typeof(RectTransform), typeof(Image));
            feverBg.transform.SetParent(skillsBar.transform, false);
            SetRect(feverBg, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(300, 0), new Vector2(280, 26));
            feverBg.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.09f);

            GameObject feverFill = new GameObject("FeverFill", typeof(RectTransform), typeof(Image));
            feverFill.transform.SetParent(feverBg.transform, false);
            SetRect(feverFill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fFillImg = feverFill.GetComponent<Image>();
            fFillImg.type = Image.Type.Filled;
            fFillImg.fillMethod = Image.FillMethod.Horizontal;
            fFillImg.fillAmount = 0f;
            fFillImg.color = new Color(1f, 0.55f, 0.1f);

            // Hammer Button (Smash)
            GameObject hammerBtnObj = CreateButton(skillsBar.transform, "BtnHammer", "🔨 SMASH", defaultFont, new Vector2(1, 0.5f), new Vector2(-230, 0), new Vector2(180, 60), new Color(0.18f, 0.22f, 0.32f));
            var btnHammer = hammerBtnObj.GetComponent<Button>();

            // Hammer Badge
            GameObject badgeObj = new GameObject("Badge", typeof(RectTransform), typeof(Image));
            badgeObj.transform.SetParent(hammerBtnObj.transform, false);
            SetRect(badgeObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-5, -5), new Vector2(32, 32));
            badgeObj.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f);
            GameObject badgeText = CreateText(badgeObj.transform, "Text", "1", 18, TextAnchor.MiddleCenter, defaultFont, Color.white);
            SetRect(badgeText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Rotate Button (Turn)
            GameObject rotateBtnObj = CreateButton(skillsBar.transform, "BtnRotate", "🔄 TURN", defaultFont, new Vector2(1, 0.5f), new Vector2(-40, 0), new Vector2(160, 60), new Color(0.12f, 0.35f, 0.45f));
            var btnRotate = rotateBtnObj.GetComponent<Button>();

            // --- C. Board Container (Center) ---
            GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
            boardContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(boardContainer, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 110), new Vector2(980, 980));
            Image bBg = boardContainer.AddComponent<Image>();
            bBg.color = new Color(0.06f, 0.08f, 0.12f, 0.95f);

            // 64 Grid Cells
            float cellSize = 108f;
            float spacing = 10f;
            float totalGridWidth = (8 * cellSize) + (7 * spacing); // 934px
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
                    bgImg.color = new Color(0.1f, 0.13f, 0.18f);

                    // Fill Child
                    GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                    fillObj.transform.SetParent(cellObj.transform, false);
                    SetRect(fillObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image fillImg = fillObj.GetComponent<Image>();
                    fillObj.SetActive(false);

                    // Bomb Icon Child
                    GameObject bIconObj = new GameObject("BombIcon", typeof(RectTransform), typeof(Image));
                    bIconObj.transform.SetParent(cellObj.transform, false);
                    SetRect(bIconObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(75, 75));
                    Image bIconImg = bIconObj.GetComponent<Image>();
                    bIconObj.SetActive(false);

                    // Highlight Child
                    GameObject hlObj = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
                    hlObj.transform.SetParent(cellObj.transform, false);
                    SetRect(hlObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    Image hlImg = hlObj.GetComponent<Image>();
                    hlImg.color = new Color(1f, 1f, 1f, 0.35f);
                    hlObj.SetActive(false);

                    BlockCellUI cellUI = cellObj.AddComponent<BlockCellUI>();
                    cellUI.SetupComponents(bgImg, fillImg, bIconImg, hlObj);
                    cellUI.Init(r, c);

                    gridMgr.RegisterCell(r, c, cellUI);
                }
            }

            // Combo Floating Text
            GameObject comboObj = CreateText(boardContainer.transform, "ComboPopup", "COMBO x2!", 56, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.8f, 0.2f));
            SetRect(comboObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 200));
            comboObj.SetActive(false);

            // --- D. Hand Slots (Bottom) ---
            GameObject handContainer = new GameObject("HandContainer", typeof(RectTransform));
            handContainer.transform.SetParent(canvasObj.transform, false);
            SetRect(handContainer, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 240), new Vector2(980, 260));
            Image hBg = handContainer.AddComponent<Image>();
            hBg.color = new Color(0.06f, 0.08f, 0.12f, 0.85f);

            Transform[] slotTransforms = new Transform[3];
            float slotSpacing = 310f;
            for (int i = 0; i < 3; i++)
            {
                GameObject slot = new GameObject($"Slot_{i}", typeof(RectTransform));
                slot.transform.SetParent(handContainer.transform, false);
                SetRect(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2((i - 1) * slotSpacing, 0), new Vector2(280, 240));
                slotTransforms[i] = slot.transform;
            }

            spawner.SetupReferences(slotTransforms, canvas, gemSprite, bombSprite);

            // Bottom Guide Text
            GameObject guideObj = CreateText(canvasObj.transform, "GuideText", "블록을 드래그해 보드에 배치하세요! 🧨폭탄 3x3 연쇄 폭발", 22, TextAnchor.MiddleCenter, defaultFont, new Color(0.5f, 0.55f, 0.65f));
            SetRect(guideObj, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(980, 50));

            // --- E. Game Over Modal ---
            GameObject modalObj = new GameObject("GameOverModal", typeof(RectTransform));
            modalObj.transform.SetParent(canvasObj.transform, false);
            SetRect(modalObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image modalBg = modalObj.AddComponent<Image>();
            modalBg.color = new Color(0.02f, 0.03f, 0.06f, 0.92f);

            GameObject dialog = new GameObject("Dialog", typeof(RectTransform), typeof(Image));
            dialog.transform.SetParent(modalObj.transform, false);
            SetRect(dialog, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(680, 720));
            dialog.GetComponent<Image>().color = new Color(0.09f, 0.12f, 0.18f);

            GameObject overTitle = CreateText(dialog.transform, "Title", "NO MORE MOVES", 48, TextAnchor.MiddleCenter, defaultFont, new Color(0.95f, 0.3f, 0.3f));
            SetRect(overTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(600, 80));

            GameObject overDesc = CreateText(dialog.transform, "Desc", "더 이상 블록을 둘 곳이 없습니다!", 24, TextAnchor.MiddleCenter, defaultFont, new Color(0.6f, 0.65f, 0.75f));
            SetRect(overDesc, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -160), new Vector2(600, 50));

            // Score Box in Modal
            GameObject modalScoreBox = new GameObject("ScoreBox", typeof(RectTransform), typeof(Image));
            modalScoreBox.transform.SetParent(dialog.transform, false);
            SetRect(modalScoreBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(560, 220));
            modalScoreBox.GetComponent<Image>().color = new Color(0.05f, 0.07f, 0.11f);

            GameObject finalLabel = CreateText(modalScoreBox.transform, "FLabel", "FINAL SCORE", 24, TextAnchor.MiddleCenter, defaultFont, new Color(0.6f, 0.65f, 0.75f));
            SetRect(finalLabel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(500, 40));

            GameObject finalVal = CreateText(modalScoreBox.transform, "FValue", "0", 64, TextAnchor.MiddleCenter, defaultFont, new Color(1f, 0.82f, 0.15f));
            SetRect(finalVal, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -100), new Vector2(500, 70));

            GameObject mBestVal = CreateText(modalScoreBox.transform, "BValue", "BEST: 0", 26, TextAnchor.MiddleCenter, defaultFont, new Color(0.8f, 0.85f, 0.9f));
            SetRect(mBestVal, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(500, 40));

            GameObject restartBtn = CreateButton(dialog.transform, "BtnRestart", "PLAY AGAIN", defaultFont, new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(560, 95), new Color(0.95f, 0.55f, 0.1f));

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

            Debug.Log("<color=green><b>[Block Blast]</b> 모바일 블록 블라스트: 엘리멘탈 씬이 성공적으로 생성되었습니다! (Assets/Scenes/BlockBlastScene.unity)</color>");
        }

        private static Sprite LoadOrCreateSprite(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.SaveAndReimport();
                }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
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
