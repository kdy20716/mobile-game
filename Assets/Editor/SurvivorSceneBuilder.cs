#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Survivor2D.Editor
{
    public static class SurvivorSceneBuilder
    {
        [MenuItem("Vampire Survivors/Generate 2D Survivor Scene")]
        public static void GenerateSurvivorScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera Setup (2D Orthographic)
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 7.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.1f, 0.12f); // Dark retro dungeon color
            camObj.AddComponent<AudioListener>();
            camObj.transform.position = new Vector3(0f, 0f, -10f);

            // 2. Background Grid (2D Tile Plane)
            GameObject bg = new GameObject("Background_DungeonGround");
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = CreateTileSprite();
            bgSr.drawMode = SpriteDrawMode.Tiled;
            bgSr.size = new Vector2(100f, 100f);
            bgSr.color = new Color(0.18f, 0.22f, 0.25f);
            bgSr.sortingOrder = -10;
            bg.transform.position = Vector3.zero;

            // 3. Player Character (Ramen Mage Hero)
            GameObject player = new GameObject("SurvivorPlayer");
            player.transform.position = Vector3.zero;

            string heroSpritePath = "Assets/Sprites/Survivor/Hero_RamenMage.jpg";
            Sprite heroSprite = SpriteTextureUtility.LoadSpriteWithChromaKey(heroSpritePath, Color.white, 0.45f, 400f);

            SpriteRenderer playerSr = player.AddComponent<SpriteRenderer>();
            playerSr.sprite = heroSprite != null ? heroSprite : CreatePlayerSprite();
            playerSr.color = Color.white;
            playerSr.sortingOrder = 4;

            CircleCollider2D playerCol = player.AddComponent<CircleCollider2D>();
            playerCol.radius = 0.5f;

            var playerComp = player.AddComponent<SurvivorPlayer2D>();

            // Attach Weapons to Player
            GameObject weaponsRoot = new GameObject("Weapons");
            weaponsRoot.transform.SetParent(player.transform);
            weaponsRoot.transform.localPosition = Vector3.zero;

            weaponsRoot.AddComponent<OrbitingBladesWeapon>();
            weaponsRoot.AddComponent<MagicMissileWeapon>();

            // Camera Follow
            var follow = camObj.AddComponent<CameraFollow2D>();
            follow.target = player.transform;

            // 4. Wave Spawner & Damage Number Manager
            GameObject spawnerObj = new GameObject("WaveSpawner");
            var spawner = spawnerObj.AddComponent<WaveSpawner2D>();

            string enemySpritePath = "Assets/Sprites/Survivor/Enemy_ChiliBat.jpg";
            Sprite enemySprite = SpriteTextureUtility.LoadSpriteWithChromaKey(enemySpritePath, Color.white, 0.45f, 450f);
            spawner.enemySprite = enemySprite;

            GameObject dmgMgr = new GameObject("DamageNumberManager");
            dmgMgr.AddComponent<DamageNumberManager>();

            // 5. EventSystem
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                es.AddComponent<StandaloneInputModule>();
#endif
            }

            // 6. UI Canvas
            BuildSurvivorUI(playerComp);

            // 7. Save Scene
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            string scenePath = "Assets/Scenes/Survivor2DScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = player;
            Debug.Log("<color=green><b>[Vampire Survivors]</b> 2D 서바이벌 씬 생성이 완료되었습니다! (Assets/Scenes/Survivor2DScene.unity)</color>");
        }

        private static void BuildSurvivorUI(SurvivorPlayer2D player)
        {
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf")
                ?? AssetDatabase.LoadAssetAtPath<Font>("Assets/SharedAssets/TextMeshPro/Fonts/LiberationSans.ttf");

            GameObject canvasObj = new GameObject("SurvivorCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Configure CanvasScaler for Mobile Responsive UI
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // Mobile Portrait/Landscape match
            scaler.matchWidthOrHeight = 0.5f;

            var uiMgr = canvasObj.AddComponent<SurvivorUIManager>();
            var levelUpMgr = canvasObj.AddComponent<LevelUpManager>();

            // --- Top EXP Bar ---
            GameObject expBarObj = CreateSlider(canvasObj.transform, "ExpBar", new Color(0.2f, 0.7f, 1f), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -25), new Vector2(0, 36));
            Slider expSlider = expBarObj.GetComponent<Slider>();

            // Level Text (Top Left)
            GameObject lvObj = CreateText(canvasObj.transform, "LevelText", "LV. 1", 28, TextAnchor.MiddleLeft, defaultFont);
            SetRect(lvObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(50, -70), new Vector2(160, 50));
            lvObj.GetComponent<Text>().color = Color.yellow;

            // Timer Text (Top Center)
            GameObject timerObj = CreateText(canvasObj.transform, "TimerText", "00:00", 36, TextAnchor.MiddleCenter, defaultFont);
            SetRect(timerObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -70), new Vector2(200, 50));
            timerObj.GetComponent<Text>().color = Color.white;

            // Kill Count (Top Right)
            GameObject killObj = CreateText(canvasObj.transform, "KillText", "💀 0", 28, TextAnchor.MiddleRight, defaultFont);
            SetRect(killObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-50, -70), new Vector2(160, 50));
            killObj.GetComponent<Text>().color = new Color(1f, 0.4f, 0.4f);

            // --- Player HP Bar ---
            GameObject hpBarObj = CreateSlider(canvasObj.transform, "HpBar", new Color(0.9f, 0.2f, 0.2f), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -120), new Vector2(320, 24));
            Slider hpSlider = hpBarObj.GetComponent<Slider>();

            // --- Virtual Joystick (Bottom Left) ---
            GameObject joyBg = new GameObject("VirtualJoystick");
            joyBg.transform.SetParent(canvasObj.transform);
            Image bgImg = joyBg.AddComponent<Image>();
            bgImg.color = new Color(1f, 1f, 1f, 0.25f);
            SetRect(joyBg, new Vector2(0, 0), new Vector2(0, 0), new Vector2(180, 180), new Vector2(220, 220));

            GameObject joyHandle = new GameObject("Handle");
            joyHandle.transform.SetParent(joyBg.transform);
            Image hdImg = joyHandle.AddComponent<Image>();
            hdImg.color = new Color(1f, 1f, 1f, 0.75f);
            SetRect(joyHandle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(85, 85));

            var joystick = joyBg.AddComponent<VirtualJoystick>();
            joystick.SetupReferences(joyBg.GetComponent<RectTransform>(), joyHandle.GetComponent<RectTransform>(), 80f);

            // --- Level Up Modal Panel ---
            GameObject levelModal = new GameObject("LevelUpModal");
            levelModal.transform.SetParent(canvasObj.transform);
            Image modalBg = levelModal.AddComponent<Image>();
            modalBg.color = new Color(0.06f, 0.08f, 0.14f, 0.96f);
            SetRect(levelModal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 520));

            GameObject modalTitle = CreateText(levelModal.transform, "Title", "⭐ LEVEL UP! SELECT UPGRADE ⭐", 26, TextAnchor.MiddleCenter, defaultFont);
            SetRect(modalTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(560, 50));
            modalTitle.GetComponent<Text>().color = Color.yellow;

            Button[] choiceBtns = new Button[3];
            Text[] titles = new Text[3];
            Text[] descs = new Text[3];

            for (int i = 0; i < 3; i++)
            {
                GameObject card = CreateCardButton(levelModal.transform, $"Card_{i}", defaultFont, new Vector2(0, 100 - i * 135), out Button btn, out Text t, out Text d);
                choiceBtns[i] = btn;
                titles[i] = t;
                descs[i] = d;
            }

            // Bind LevelUpManager serialized properties so they persist in scene
            SerializedObject soLevel = new SerializedObject(levelUpMgr);
            soLevel.FindProperty("levelUpPanel").objectReferenceValue = levelModal;
            SerializedProperty propBtns = soLevel.FindProperty("choiceButtons");
            SerializedProperty propTitles = soLevel.FindProperty("choiceTitleTexts");
            SerializedProperty propDescs = soLevel.FindProperty("choiceDescTexts");
            propBtns.arraySize = 3;
            propTitles.arraySize = 3;
            propDescs.arraySize = 3;
            for (int i = 0; i < 3; i++)
            {
                propBtns.GetArrayElementAtIndex(i).objectReferenceValue = choiceBtns[i];
                propTitles.GetArrayElementAtIndex(i).objectReferenceValue = titles[i];
                propDescs.GetArrayElementAtIndex(i).objectReferenceValue = descs[i];
            }
            soLevel.ApplyModifiedProperties();

            // --- Game Over Panel ---
            GameObject overModal = new GameObject("GameOverModal");
            overModal.transform.SetParent(canvasObj.transform);
            Image overBg = overModal.AddComponent<Image>();
            overBg.color = new Color(0.12f, 0.02f, 0.02f, 0.96f);
            SetRect(overModal, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 320));

            GameObject overTitle = CreateText(overModal.transform, "Title", "YOU DIED", 50, TextAnchor.MiddleCenter, defaultFont);
            SetRect(overTitle, new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.82f), Vector2.zero, new Vector2(440, 60));
            overTitle.GetComponent<Text>().color = Color.red;

            GameObject resTime = CreateText(overModal.transform, "Time", "SURVIVED: 00:00", 24, TextAnchor.MiddleCenter, defaultFont);
            SetRect(resTime, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), Vector2.zero, new Vector2(420, 36));
            resTime.GetComponent<Text>().color = Color.white;

            GameObject resKills = CreateText(overModal.transform, "Kills", "ENEMIES SLAIN: 0", 24, TextAnchor.MiddleCenter, defaultFont);
            SetRect(resKills, new Vector2(0.5f, 0.38f), new Vector2(0.5f, 0.38f), Vector2.zero, new Vector2(420, 36));
            resKills.GetComponent<Text>().color = Color.yellow;

            GameObject retryBtnObj = CreateSimpleButton(overModal.transform, "RetryBtn", "PLAY AGAIN", defaultFont, new Vector2(0, -85), new Vector2(220, 56));
            Button retryBtn = retryBtnObj.GetComponent<Button>();

            // Bind SurvivorUIManager serialized properties
            SerializedObject soUI = new SerializedObject(uiMgr);
            soUI.FindProperty("expSlider").objectReferenceValue = expSlider;
            soUI.FindProperty("levelText").objectReferenceValue = lvObj.GetComponent<Text>();
            soUI.FindProperty("timerText").objectReferenceValue = timerObj.GetComponent<Text>();
            soUI.FindProperty("killCountText").objectReferenceValue = killObj.GetComponent<Text>();
            soUI.FindProperty("hpSlider").objectReferenceValue = hpSlider;
            soUI.FindProperty("gameOverPanel").objectReferenceValue = overModal;
            soUI.FindProperty("resultTimeText").objectReferenceValue = resTime.GetComponent<Text>();
            soUI.FindProperty("resultKillsText").objectReferenceValue = resKills.GetComponent<Text>();
            soUI.FindProperty("restartButton").objectReferenceValue = retryBtn;
            soUI.ApplyModifiedProperties();
        }

        private static GameObject CreateCardButton(Transform parent, string name, Font font, Vector2 pos, out Button btn, out Text title, out Text desc)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            Image img = obj.AddComponent<Image>();
            img.color = new Color(0.18f, 0.22f, 0.32f);
            btn = obj.AddComponent<Button>();

            SetRect(obj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(460, 95));

            GameObject tObj = CreateText(obj.transform, "Title", "Upgrade Title", 20, TextAnchor.MiddleLeft, font);
            SetRect(tObj, new Vector2(0, 1), new Vector2(1, 1), new Vector2(15, -25), new Vector2(-30, 30));
            title = tObj.GetComponent<Text>();
            title.color = Color.white;

            GameObject dObj = CreateText(obj.transform, "Desc", "Upgrade description text...", 15, TextAnchor.UpperLeft, font);
            SetRect(dObj, new Vector2(0, 0), new Vector2(1, 1), new Vector2(15, -5), new Vector2(-30, -45));
            desc = dObj.GetComponent<Text>();
            desc.color = new Color(0.75f, 0.85f, 0.95f);

            return obj;
        }

        private static GameObject CreateSimpleButton(Transform parent, string name, string label, Font font, Vector2 pos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f);
            Button btn = btnObj.AddComponent<Button>();

            SetRect(btnObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);

            GameObject textObj = CreateText(btnObj.transform, "Text", label, 20, TextAnchor.MiddleCenter, font);
            SetRect(textObj, new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().color = Color.white;

            return btnObj;
        }

        private static GameObject CreateSlider(Transform parent, string name, Color fillCol, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent);
            Slider slider = sliderObj.AddComponent<Slider>();

            SetRect(sliderObj, anchorMin, anchorMax, pos, size);

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform);
            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            SetRect(bgObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform);
            SetRect(fillArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform);
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = fillCol;
            SetRect(fill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.value = 1f;
            return sliderObj;
        }

        private static GameObject CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Font font)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            Text t = obj.AddComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.alignment = alignment;
            t.font = font;
            return obj;
        }

        private static void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            RectTransform rt = obj.GetComponent<RectTransform>() ?? obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static Sprite CreateTileSprite()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            Color baseCol = new Color(0.2f, 0.23f, 0.26f);
            Color borderCol = new Color(0.14f, 0.16f, 0.19f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    tex.SetPixel(x, y, isBorder ? borderCol : baseCol);
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
        }

        private static Sprite CreatePlayerSprite()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f)) / (size * 0.5f);
                    tex.SetPixel(x, y, dist <= 0.85f ? Color.white : Color.clear);
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
        }
    }

    public class CameraFollow2D : MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = 8f;

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = new Vector3(target.position.x, target.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        }
    }
}
#endif
