#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LethalCompany.Editor
{
    public static class LethalSceneBuilder
    {
        [MenuItem("Lethal Company/Generate Phase 1 Scrap System Scene")]
        public static void GenerateScrapSystemScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Materials
            Material floorMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_ConcreteFloor.mat", new Color(0.25f, 0.26f, 0.28f));
            Material wallMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_IndustrialWall.mat", new Color(0.35f, 0.38f, 0.4f));
            Material metalMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_MetalScrap.mat", new Color(0.6f, 0.55f, 0.45f));
            Material engineMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_HeavyEngine.mat", new Color(0.2f, 0.22f, 0.24f));
            Material goldMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_BrassBell.mat", new Color(0.9f, 0.75f, 0.2f));

            // Lighting (Dim industrial abandoned facility)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.12f, 0.14f, 0.16f);

            // 2. Build Facility Room (18m x 18m)
            GameObject facility = new GameObject("Facility_Room");
            BuildFacilityRoom(facility.transform, floorMat, wallMat);

            // 3. Player Character Setup
            GameObject player = new GameObject("LethalScavenger");
            player.transform.position = new Vector3(0f, 1f, -5f);

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0f, 0.9f, 0f);

            var playerCtrl = player.AddComponent<LethalPlayerController>();
            var inventory = player.AddComponent<LethalInventory>();
            var interaction = player.AddComponent<LethalInteraction>();

            // 1st Person Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            camObj.tag = "MainCamera";

            Camera cam = camObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.fieldOfView = 68f;
            camObj.AddComponent<AudioListener>();

            // Hand Holder for Holding Scrap
            GameObject handHolder = new GameObject("RightHandHolder");
            handHolder.transform.SetParent(camObj.transform);
            handHolder.transform.localPosition = new Vector3(0.35f, -0.3f, 0.6f);

            playerCtrl.SetCamera(camObj.transform);
            inventory.SetHandHolder(handHolder.transform);
            interaction.SetCamera(camObj.transform);

            // Flashlight on Head
            GameObject headLight = new GameObject("Headlight");
            headLight.transform.SetParent(camObj.transform);
            headLight.transform.localPosition = Vector3.forward * 0.1f;
            Light hl = headLight.AddComponent<Light>();
            hl.type = LightType.Spot;
            hl.range = 22f;
            hl.spotAngle = 55f;
            hl.intensity = 2.0f;
            hl.color = new Color(1f, 0.95f, 0.85f);
            hl.shadows = LightShadows.Hard;

            // 4. Spawn Diverse Scrap Items
            SpawnScrapItems(metalMat, engineMat, goldMat);

            // 5. EventSystem
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

            // 6. UI Canvas
            BuildLethalUI(inventory, interaction);

            // 7. Save Scene
            string scenePath = "Assets/Scenes/LethalScrapScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = player;
            Debug.Log("<color=green><b>[Lethal Company]</b> 1단계 고철 파밍 및 인벤토리 씬이 생성되었습니다! (Assets/Scenes/LethalScrapScene.unity)</color>");
        }

        private static void BuildFacilityRoom(Transform parent, Material floorMat, Material wallMat)
        {
            // Floor & Ceiling
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(parent);
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(2.5f, 1f, 2.5f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(parent);
            ceiling.transform.position = new Vector3(0f, 4.5f, 0f);
            ceiling.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            ceiling.transform.localScale = new Vector3(2.5f, 1f, 2.5f);
            ceiling.GetComponent<MeshRenderer>().material = wallMat;

            // 4 Outer Walls
            CreateWall(parent, new Vector3(0f, 2.25f, 12.5f), new Vector3(25f, 4.5f, 0.6f), wallMat, "Wall_North");
            CreateWall(parent, new Vector3(0f, 2.25f, -12.5f), new Vector3(25f, 4.5f, 0.6f), wallMat, "Wall_South");
            CreateWall(parent, new Vector3(12.5f, 2.25f, 0f), new Vector3(0.6f, 4.5f, 25f), wallMat, "Wall_East");
            CreateWall(parent, new Vector3(-12.5f, 2.25f, 0f), new Vector3(0.6f, 4.5f, 25f), wallMat, "Wall_West");

            // Some crates / shelves inside
            CreateCrate(parent, new Vector3(-4f, 0.75f, 3f), new Vector3(2f, 1.5f, 2f), wallMat);
            CreateCrate(parent, new Vector3(5f, 0.6f, -2f), new Vector3(1.8f, 1.2f, 3f), wallMat);
            CreateCrate(parent, new Vector3(6f, 1.6f, -2f), new Vector3(1.4f, 0.8f, 1.8f), wallMat);

            // Room Lights
            CreateRoomLight(parent, new Vector3(-5f, 4.2f, 5f));
            CreateRoomLight(parent, new Vector3(5f, 4.2f, -5f));
        }

        private static void CreateWall(Transform parent, Vector3 pos, Vector3 size, Material mat, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = size;
            wall.GetComponent<MeshRenderer>().material = mat;
        }

        private static void CreateCrate(Transform parent, Vector3 pos, Vector3 size, Material mat)
        {
            GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crate.name = "IndustrialCrate";
            crate.transform.SetParent(parent);
            crate.transform.position = pos;
            crate.transform.localScale = size;
            crate.GetComponent<MeshRenderer>().material = mat;
        }

        private static void CreateRoomLight(Transform parent, Vector3 pos)
        {
            GameObject lObj = new GameObject("CeilingLamp");
            lObj.transform.SetParent(parent);
            lObj.transform.position = pos;

            Light l = lObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 14f;
            l.intensity = 1.2f;
            l.color = new Color(0.9f, 0.85f, 0.7f);
        }

        private static void SpawnScrapItems(Material metalMat, Material engineMat, Material goldMat)
        {
            // 1. V-Type Engine (Heavy Scrap: 42 lb, $85)
            CreateEngineScrap(new Vector3(-4f, 1.8f, 3f), engineMat, "V-Type Engine", 85, 42f);

            // 2. Big Bolt (Light Scrap: 6 lb, $28)
            CreateBoltScrap(new Vector3(1f, 0.3f, 4f), metalMat, "Big Bolt", 28, 6f);
            CreateBoltScrap(new Vector3(2f, 0.3f, 4.5f), metalMat, "Big Bolt", 32, 7f);

            // 3. Brass Bell (Medium Scrap: 18 lb, $64)
            CreateBellScrap(new Vector3(5f, 1.45f, -2f), goldMat, "Brass Bell", 64, 18f);

            // 4. Metal Axle (Medium Scrap: 24 lb, $52)
            CreateAxleScrap(new Vector3(-6f, 0.3f, -4f), metalMat, "Metal Axle", 52, 24f);

            // 5. Golden Cup (Light Scrap: 4 lb, $95)
            CreateCupScrap(new Vector3(6.2f, 2.2f, -2f), goldMat, "Golden Goblet", 95, 4f);
        }

        private static void CreateEngineScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.9f, 0.6f, 0.7f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.isTwoHanded = true;
            scrap.handHoldOffset = new Vector3(0.2f, -0.3f, 0.7f);
            scrap.handHoldScale = new Vector3(0.55f, 0.4f, 0.45f);
        }

        private static void CreateBoltScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.handHoldOffset = new Vector3(0.3f, -0.2f, 0.5f);
            scrap.handHoldScale = new Vector3(0.18f, 0.22f, 0.18f);
        }

        private static void CreateBellScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.45f, 0.55f, 0.45f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.handHoldOffset = new Vector3(0.28f, -0.25f, 0.55f);
            scrap.handHoldScale = new Vector3(0.35f, 0.45f, 0.35f);
        }

        private static void CreateAxleScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.Euler(0, 45, 90);
            obj.transform.localScale = new Vector3(0.15f, 0.7f, 0.15f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.handHoldOffset = new Vector3(0.3f, -0.1f, 0.6f);
            scrap.handHoldRotation = new Vector3(45, 30, 0);
            scrap.handHoldScale = new Vector3(0.12f, 0.5f, 0.12f);
        }

        private static void CreateCupScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.2f, 0.25f, 0.2f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.handHoldOffset = new Vector3(0.25f, -0.2f, 0.45f);
            scrap.handHoldScale = new Vector3(0.15f, 0.2f, 0.15f);
        }

        private static void BuildLethalUI(LethalInventory inv, LethalInteraction inter)
        {
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf")
                ?? AssetDatabase.LoadAssetAtPath<Font>("Assets/SharedAssets/TextMeshPro/Fonts/LiberationSans.ttf");

            GameObject canvasObj = new GameObject("LethalCanvas", typeof(RectTransform));
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            var uiMgr = canvasObj.AddComponent<LethalUIManager>();

            // Weight Display (Top Left)
            GameObject weightObj = CreateText(canvasObj.transform, "WeightText", "WEIGHT: 0 lb", 32, TextAnchor.MiddleLeft, defaultFont);
            SetRect(weightObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(60, -60), new Vector2(320, 50));
            weightObj.GetComponent<Text>().color = Color.white;

            // Center Crosshair
            GameObject crosshair = new GameObject("Crosshair", typeof(RectTransform));
            crosshair.transform.SetParent(canvasObj.transform);
            Image crossImg = crosshair.AddComponent<Image>();
            crossImg.color = new Color(1f, 1f, 1f, 0.5f);
            SetRect(crosshair, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(8, 8));

            // Interaction Prompt (Center below crosshair)
            GameObject promptObj = CreateText(canvasObj.transform, "PromptText", "[E] Grab Scrap", 24, TextAnchor.UpperCenter, defaultFont);
            SetRect(promptObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(400, 70));
            promptObj.GetComponent<Text>().color = Color.yellow;

            // Scan Results Container
            GameObject scanContainer = new GameObject("ScanContainer", typeof(RectTransform));
            scanContainer.transform.SetParent(canvasObj.transform);
            SetRect(scanContainer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // 4-Slot Inventory HUD (Bottom Center)
            Image[] frames = new Image[LethalInventory.MaxSlots];
            Text[] names = new Text[LethalInventory.MaxSlots];

            float slotWidth = 110f;
            float slotSpacing = 15f;
            float totalWidth = (slotWidth * 4) + (slotSpacing * 3);
            float startX = -totalWidth * 0.5f + slotWidth * 0.5f;

            for (int i = 0; i < LethalInventory.MaxSlots; i++)
            {
                int slotIndex = i;
                GameObject slot = new GameObject($"Slot_{i}", typeof(RectTransform));
                slot.transform.SetParent(canvasObj.transform);
                Image frameImg = slot.AddComponent<Image>();
                frameImg.color = new Color(0.15f, 0.15f, 0.2f, 0.6f);
                frames[i] = frameImg;

                Button slotBtn = slot.AddComponent<Button>();
                slotBtn.onClick.AddListener(() => inv.SelectSlot(slotIndex));

                SetRect(slot, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(startX + i * (slotWidth + slotSpacing), 70), new Vector2(slotWidth, 75));

                GameObject slotText = CreateText(slot.transform, "ItemName", $"[ {i + 1} ]", 18, TextAnchor.MiddleCenter, defaultFont);
                SetRect(slotText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                names[i] = slotText.GetComponent<Text>();
                names[i].raycastTarget = false;
            }

            // --- Mobile Controls (Touch Look, Grab, Drop, Scan) ---
            GameObject lookPanel = new GameObject("TouchLookPanel", typeof(RectTransform));
            lookPanel.transform.SetParent(canvasObj.transform);
            Image lookImg = lookPanel.AddComponent<Image>();
            lookImg.color = Color.clear;
            SetRect(lookPanel, new Vector2(0.4f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            lookPanel.AddComponent<HorrorEscape.TouchLookPanel>();

            // Grab Button (Bottom Right)
            GameObject grabBtn = CreateButton(canvasObj.transform, "Btn_Grab", "GRAB", defaultFont, new Vector2(1, 0), new Vector2(-100, 190), new Vector2(110, 80));
            grabBtn.GetComponent<Button>().onClick.AddListener(() => inter.GrabFocusedItem());

            // Drop Button
            GameObject dropBtn = CreateButton(canvasObj.transform, "Btn_Drop", "DROP", defaultFont, new Vector2(1, 0), new Vector2(-225, 190), new Vector2(110, 80));
            dropBtn.GetComponent<Button>().onClick.AddListener(() => inter.DropCurrentItem());

            // Scan Button
            GameObject scanBtn = CreateButton(canvasObj.transform, "Btn_Scan", "SCAN", defaultFont, new Vector2(1, 0), new Vector2(-100, 290), new Vector2(110, 70));
            scanBtn.GetComponent<Button>().onClick.AddListener(() => inter.TriggerScan());

            // Bind Serialized Properties
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("weightText").objectReferenceValue = weightObj.GetComponent<Text>();
            so.FindProperty("promptText").objectReferenceValue = promptObj.GetComponent<Text>();
            so.FindProperty("scanContainer").objectReferenceValue = scanContainer.transform;

            SerializedProperty propFrames = so.FindProperty("slotFrames");
            SerializedProperty propNames = so.FindProperty("slotItemNames");
            propFrames.arraySize = LethalInventory.MaxSlots;
            propNames.arraySize = LethalInventory.MaxSlots;

            for (int i = 0; i < LethalInventory.MaxSlots; i++)
            {
                propFrames.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
                propNames.GetArrayElementAtIndex(i).objectReferenceValue = names[i];
            }
            so.ApplyModifiedProperties();
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Font font, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform));
            btnObj.transform.SetParent(parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.25f, 0.35f, 0.85f);
            Button btn = btnObj.AddComponent<Button>();

            SetRect(btnObj, anchor, anchor, pos, size);

            GameObject textObj = CreateText(btnObj.transform, "Text", label, 22, TextAnchor.MiddleCenter, font);
            SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().color = Color.white;
            textObj.GetComponent<Text>().raycastTarget = false;

            return btnObj;
        }

        private static GameObject CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Font font)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
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
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null) rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static Material GetOrCreateMaterial(string path, Color color)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Lethal"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                    AssetDatabase.CreateFolder("Assets", "Materials");
                AssetDatabase.CreateFolder("Assets/Materials", "Lethal");
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader standardShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                mat = new Material(standardShader);
                mat.color = color;
                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }
    }
}
#endif
