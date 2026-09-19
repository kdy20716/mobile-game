#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HorrorEscape.Editor
{
    public static class HorrorSceneBuilder
    {
        [MenuItem("Horror Game/Generate The Backrooms Escape Scene")]
        public static void GenerateBackroomsScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Materials
            Material wallMat = GetOrCreateMaterial("Assets/Materials/Horror/M_BackroomsWall.mat", new Color(0.82f, 0.74f, 0.42f)); // Vintage yellow wallpaper
            Material floorMat = GetOrCreateMaterial("Assets/Materials/Horror/M_DampCarpet.mat", new Color(0.35f, 0.32f, 0.22f));   // Musty damp carpet
            Material ceilingMat = GetOrCreateMaterial("Assets/Materials/Horror/M_CeilingTile.mat", new Color(0.7f, 0.7f, 0.65f));  // Acoustic ceiling tile
            Material doorMat = GetOrCreateMaterial("Assets/Materials/Horror/M_HeavyDoor.mat", new Color(0.2f, 0.2f, 0.22f));
            Material keyMat = GetOrCreateMaterial("Assets/Materials/Horror/M_GoldenKey.mat", new Color(1f, 0.85f, 0.1f));
            Material monsterMat = GetOrCreateMaterial("Assets/Materials/Horror/M_ShadowMonster.mat", new Color(0.04f, 0.04f, 0.04f));

            // Ambient lighting (Dim and grim)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.06f);

            // 2. Build Backrooms Maze (7x7 grid with corridors)
            GameObject mazeRoot = new GameObject("The_Backrooms_Maze");
            List<Transform> patrolWaypoints = new List<Transform>();
            BuildBackroomsMaze(mazeRoot.transform, wallMat, floorMat, ceilingMat, patrolWaypoints);

            // 3. Player Character Setup
            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 1f, 0f);

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0f, 0.9f, 0f);

            var playerCtrl = player.AddComponent<HorrorPlayerController>();

            // Player Camera
            GameObject camObj = new GameObject("FirstPersonCamera");
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            camObj.tag = "MainCamera";

            Camera cam = camObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.fieldOfView = 68f;
            camObj.AddComponent<AudioListener>();
            playerCtrl.SetCamera(camObj.transform);

            // Flashlight
            GameObject lightObj = new GameObject("Flashlight");
            lightObj.transform.SetParent(camObj.transform);
            lightObj.transform.localPosition = new Vector3(0.25f, -0.2f, 0.2f);

            Light spotLight = lightObj.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.range = 18f;
            spotLight.spotAngle = 48f;
            spotLight.innerSpotAngle = 30f;
            spotLight.intensity = 2.4f;
            spotLight.color = new Color(1f, 0.96f, 0.82f);
            spotLight.shadows = LightShadows.Hard;

            var flashCtrl = player.AddComponent<FlashlightController>();
            flashCtrl.AssignLight(spotLight);

            // 4. Spawn Escape Door
            GameObject doorObj = new GameObject("ExitDoor");
            doorObj.transform.position = new Vector3(24f, 0f, 24f);

            GameObject doorMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorMesh.name = "DoorMesh";
            doorMesh.transform.SetParent(doorObj.transform);
            doorMesh.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            doorMesh.transform.localScale = new Vector3(2.2f, 3.0f, 0.3f);
            doorMesh.GetComponent<MeshRenderer>().material = doorMat;

            GameObject doorLightObj = new GameObject("ExitLight");
            doorLightObj.transform.SetParent(doorObj.transform);
            doorLightObj.transform.localPosition = new Vector3(0f, 3.2f, -0.6f);
            Light dLight = doorLightObj.AddComponent<Light>();
            dLight.type = LightType.Point;
            dLight.range = 8f;
            dLight.intensity = 1.8f;
            dLight.color = Color.red;

            BoxCollider doorTrigger = doorObj.AddComponent<BoxCollider>();
            doorTrigger.isTrigger = true;
            doorTrigger.size = new Vector3(3f, 3f, 3f);
            doorTrigger.center = new Vector3(0f, 1.5f, 0f);

            var doorComp = doorObj.AddComponent<EscapeDoor>();
            doorComp.SetupDoor(dLight, doorMesh.transform);

            // 5. Spawn 3 Keys in Maze
            Vector3[] keyPositions = new Vector3[]
            {
                new Vector3(-16f, 0.8f, 16f),
                new Vector3(16f, 0.8f, -16f),
                new Vector3(-20f, 0.8f, -20f)
            };

            for (int k = 0; k < keyPositions.Length; k++)
            {
                CreateKeyItem(keyPositions[k], keyMat);
            }

            // 6. Spawn The Entity Monster
            GameObject monster = BuildEntityMonster(new Vector3(18f, 0f, -8f), monsterMat, patrolWaypoints);

            // 7. Audio & Scene Director
            GameObject directorObj = new GameObject("AudioDirector");
            directorObj.AddComponent<HorrorAudioDirector>();

            // 8. EventSystem
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

            // 9. UI Canvas
            BuildHorrorUI(playerCtrl, flashCtrl);

            // 10. Save Scene
            string scenePath = "Assets/Scenes/HorrorBackroomsScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = player;
            Debug.Log("<color=red><b>[Horror Game]</b> 1인칭 백룸 공포 탈출 씬이 완성되었습니다! (Assets/Scenes/HorrorBackroomsScene.unity)</color>");
        }

        private static void BuildBackroomsMaze(Transform root, Material wallMat, Material floorMat, Material ceilingMat, List<Transform> waypoints)
        {
            float roomSize = 8f;
            int gridSize = 7;
            float halfGrid = gridSize * roomSize * 0.5f;

            // Floor & Ceiling Plane
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(root);
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(gridSize * 0.9f, 1f, gridSize * 0.9f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(root);
            ceiling.transform.position = new Vector3(0f, 3.5f, 0f);
            ceiling.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            ceiling.transform.localScale = new Vector3(gridSize * 0.9f, 1f, gridSize * 0.9f);
            ceiling.GetComponent<MeshRenderer>().material = ceilingMat;

            // Maze Pattern Grid (1 = Wall, 0 = Hallway)
            int[,] maze = new int[,]
            {
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 0, 1, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 }
            };

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    float x = (c * roomSize) - halfGrid + (roomSize * 0.5f);
                    float z = (r * roomSize) - halfGrid + (roomSize * 0.5f);

                    if (maze[r, c] == 1)
                    {
                        // Wall Pillar / Block
                        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        wall.name = $"Wall_{r}_{c}";
                        wall.transform.SetParent(root);
                        wall.transform.position = new Vector3(x, 1.75f, z);
                        wall.transform.localScale = new Vector3(roomSize, 3.5f, roomSize);
                        wall.GetComponent<MeshRenderer>().material = wallMat;
                    }
                    else
                    {
                        // Waypoint for Entity Monster
                        GameObject wp = new GameObject($"Waypoint_{r}_{c}");
                        wp.transform.SetParent(root);
                        wp.transform.position = new Vector3(x, 0.5f, z);
                        waypoints.Add(wp.transform);

                        // Ceiling Fluorescent Light Panel (Every other room)
                        if ((r + c) % 2 == 0)
                        {
                            CreateCeilingLight(root, new Vector3(x, 3.4f, z));
                        }
                    }
                }
            }
        }

        private static void CreateCeilingLight(Transform parent, Vector3 pos)
        {
            GameObject lightFixture = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lightFixture.name = "FluorescentLight";
            lightFixture.transform.SetParent(parent);
            lightFixture.transform.position = pos;
            lightFixture.transform.localScale = new Vector3(2.5f, 0.1f, 0.8f);

            Light l = lightFixture.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 7.5f;
            l.intensity = 0.85f;
            l.color = new Color(0.95f, 0.92f, 0.72f);
            l.shadows = LightShadows.None;
        }

        private static void CreateKeyItem(Vector3 pos, Material mat)
        {
            GameObject keyRoot = new GameObject("EscapeKey");
            keyRoot.transform.position = pos;

            GameObject keyBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            keyBody.name = "KeyModel";
            keyBody.transform.SetParent(keyRoot.transform);
            keyBody.transform.localPosition = Vector3.zero;
            keyBody.transform.localScale = new Vector3(0.12f, 0.4f, 0.12f);
            keyBody.GetComponent<MeshRenderer>().material = mat;
            UnityEngine.Object.DestroyImmediate(keyBody.GetComponent<Collider>());

            BoxCollider col = keyRoot.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(1.2f, 1.2f, 1.2f);

            GameObject keyLight = new GameObject("KeyGlow");
            keyLight.transform.SetParent(keyRoot.transform);
            keyLight.transform.localPosition = Vector3.up * 0.2f;
            Light kl = keyLight.AddComponent<Light>();
            kl.type = LightType.Point;
            kl.range = 3.5f;
            kl.intensity = 1.6f;
            kl.color = Color.yellow;

            keyRoot.AddComponent<EscapeKeyItem>();
        }

        private static GameObject BuildEntityMonster(Vector3 pos, Material mat, List<Transform> waypoints)
        {
            GameObject monster = new GameObject("The_Entity");
            monster.transform.position = pos;

            CharacterController cc = monster.AddComponent<CharacterController>();
            cc.height = 2.4f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0f, 1.2f, 0f);

            // Tall spindly creepy humanoid mesh
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "MonsterBody";
            body.transform.SetParent(monster.transform);
            body.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            body.transform.localScale = new Vector3(0.5f, 1.2f, 0.4f);
            body.GetComponent<MeshRenderer>().material = mat;
            UnityEngine.Object.DestroyImmediate(body.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "MonsterHead";
            head.transform.SetParent(monster.transform);
            head.transform.localPosition = new Vector3(0f, 2.3f, 0f);
            head.transform.localScale = new Vector3(0.55f, 0.65f, 0.55f);
            head.GetComponent<MeshRenderer>().material = mat;
            UnityEngine.Object.DestroyImmediate(head.GetComponent<Collider>());

            // Glowing Eyes Light
            GameObject eyeLightObj = new GameObject("EyeLight");
            eyeLightObj.transform.SetParent(head.transform);
            eyeLightObj.transform.localPosition = new Vector3(0f, 0.1f, 0.35f);
            Light eyeLight = eyeLightObj.AddComponent<Light>();
            eyeLight.type = LightType.Point;
            eyeLight.range = 4f;
            eyeLight.intensity = 2f;
            eyeLight.color = Color.red;

            var ai = monster.AddComponent<HorrorEntityAI>();
            ai.SetupAI(waypoints, eyeLight);

            return monster;
        }

        private static void BuildHorrorUI(HorrorPlayerController player, FlashlightController flash)
        {
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf")
                ?? AssetDatabase.LoadAssetAtPath<Font>("Assets/SharedAssets/TextMeshPro/Fonts/LiberationSans.ttf");

            GameObject canvasObj = new GameObject("HorrorCanvas", typeof(RectTransform));
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            var uiMgr = canvasObj.AddComponent<HorrorUIManager>();

            // Fullscreen Glitch Static Image
            GameObject glitchObj = new GameObject("StaticGlitchImage", typeof(RectTransform));
            glitchObj.transform.SetParent(canvasObj.transform);
            Image glitchImg = glitchObj.AddComponent<Image>();
            glitchImg.color = new Color(0.8f, 0f, 0f, 0f);
            glitchImg.raycastTarget = false;
            SetRect(glitchObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Keys Text (Top Left)
            GameObject keyTextObj = CreateText(canvasObj.transform, "KeysText", "🔑 KEYS: 0 / 3", 32, TextAnchor.MiddleLeft, defaultFont);
            SetRect(keyTextObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(50, -60), new Vector2(300, 50));
            keyTextObj.GetComponent<Text>().color = Color.yellow;

            // Flashlight Battery (Top Right)
            GameObject batteryText = CreateText(canvasObj.transform, "BatteryLabel", "FLASHLIGHT", 22, TextAnchor.MiddleRight, defaultFont);
            SetRect(batteryText, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-150, -35), new Vector2(200, 30));
            batteryText.GetComponent<Text>().color = Color.white;

            GameObject batteryBar = CreateSlider(canvasObj.transform, "BatteryBar", new Color(1f, 0.9f, 0.2f), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-150, -65), new Vector2(180, 18));
            Slider batterySlider = batteryBar.GetComponent<Slider>();

            // Stamina Bar (Bottom Center)
            GameObject staminaBar = CreateSlider(canvasObj.transform, "StaminaBar", new Color(0.2f, 0.8f, 0.4f), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 50), new Vector2(320, 18));
            Slider staminaSlider = staminaBar.GetComponent<Slider>();

            // --- Mobile Touch Look Panel (Right Half of Screen) ---
            GameObject lookPanel = new GameObject("TouchLookPanel", typeof(RectTransform));
            lookPanel.transform.SetParent(canvasObj.transform);
            Image lookImg = lookPanel.AddComponent<Image>();
            lookImg.color = Color.clear; // Invisible touch zone
            SetRect(lookPanel, new Vector2(0.4f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            lookPanel.AddComponent<TouchLookPanel>();

            // --- Mobile Flashlight Toggle Button (Top Right Under Battery) ---
            GameObject flashBtnObj = CreateButton(canvasObj.transform, "Btn_Flashlight", "LIGHT", defaultFont, new Vector2(1, 1), new Vector2(-80, -130), new Vector2(100, 60));
            flashBtnObj.GetComponent<Button>().onClick.AddListener(() => flash.Toggle());

            // --- Mobile Sprint Button (Bottom Right) ---
            GameObject sprintBtnObj = CreateButton(canvasObj.transform, "Btn_Sprint", "RUN", defaultFont, new Vector2(1, 0), new Vector2(-90, 90), new Vector2(100, 100));
            var sprintTouch = sprintBtnObj.AddComponent<MobileRacing.MobileTouchButton>();
            // Use pointer events to sprint
            EventTrigger trigger = sprintBtnObj.AddComponent<EventTrigger>();
            var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            down.callback.AddListener((data) => player.SetSprinting(true));
            trigger.triggers.Add(down);
            var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener((data) => player.SetSprinting(false));
            trigger.triggers.Add(up);

            // --- Virtual Joystick for Mobile Movement (Bottom Left) ---
            GameObject joyBg = new GameObject("VirtualJoystick", typeof(RectTransform));
            joyBg.transform.SetParent(canvasObj.transform);
            Image bgImg = joyBg.AddComponent<Image>();
            bgImg.color = new Color(1f, 1f, 1f, 0.25f);
            SetRect(joyBg, new Vector2(0, 0), new Vector2(0, 0), new Vector2(180, 180), new Vector2(220, 220));

            GameObject joyHandle = new GameObject("Handle", typeof(RectTransform));
            joyHandle.transform.SetParent(joyBg.transform);
            Image hdImg = joyHandle.AddComponent<Image>();
            hdImg.color = new Color(1f, 1f, 1f, 0.75f);
            SetRect(joyHandle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(85, 85));

            var joystick = joyBg.AddComponent<Survivor2D.VirtualJoystick>();
            joystick.SetupReferences(joyBg.GetComponent<RectTransform>(), joyHandle.GetComponent<RectTransform>(), 80f);

            // --- Jumpscare Death Panel ---
            GameObject jumpscareModal = new GameObject("JumpscarePanel", typeof(RectTransform));
            jumpscareModal.transform.SetParent(canvasObj.transform);
            Image jumpBg = jumpscareModal.AddComponent<Image>();
            jumpBg.color = new Color(0.05f, 0f, 0f, 0.98f);
            SetRect(jumpscareModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject jumpTitle = CreateText(jumpscareModal.transform, "Title", "IT FOUND YOU", 56, TextAnchor.MiddleCenter, defaultFont);
            SetRect(jumpTitle, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(600, 80));
            jumpTitle.GetComponent<Text>().color = Color.red;

            GameObject retryBtnObj = CreateButton(jumpscareModal.transform, "RetryBtn", "TRY AGAIN", defaultFont, new Vector2(0.5f, 0.4f), Vector2.zero, new Vector2(240, 64));

            // --- Escape Win Panel ---
            GameObject winModal = new GameObject("WinPanel", typeof(RectTransform));
            winModal.transform.SetParent(canvasObj.transform);
            Image winBg = winModal.AddComponent<Image>();
            winBg.color = new Color(0.02f, 0.08f, 0.04f, 0.98f);
            SetRect(winModal, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject winTitle = CreateText(winModal.transform, "Title", "YOU ESCAPED THE BACKROOMS!", 46, TextAnchor.MiddleCenter, defaultFont);
            SetRect(winTitle, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(700, 80));
            winTitle.GetComponent<Text>().color = Color.green;

            GameObject winBtnObj = CreateButton(winModal.transform, "WinBtn", "PLAY AGAIN", defaultFont, new Vector2(0.5f, 0.4f), Vector2.zero, new Vector2(240, 64));

            // Bind Serialized Properties
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("keysText").objectReferenceValue = keyTextObj.GetComponent<Text>();
            so.FindProperty("staminaSlider").objectReferenceValue = staminaSlider;
            so.FindProperty("batterySlider").objectReferenceValue = batterySlider;
            so.FindProperty("staticGlitchImage").objectReferenceValue = glitchImg;
            so.FindProperty("jumpscarePanel").objectReferenceValue = jumpscareModal;
            so.FindProperty("winPanel").objectReferenceValue = winModal;
            so.FindProperty("jumpscareRetryBtn").objectReferenceValue = retryBtnObj.GetComponent<Button>();
            so.FindProperty("winPlayAgainBtn").objectReferenceValue = winBtnObj.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Font font, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform));
            btnObj.transform.SetParent(parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);
            Button btn = btnObj.AddComponent<Button>();

            SetRect(btnObj, anchor, anchor, pos, size);

            GameObject textObj = CreateText(btnObj.transform, "Text", label, 20, TextAnchor.MiddleCenter, font);
            SetRect(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().color = Color.white;
            textObj.GetComponent<Text>().raycastTarget = false;

            return btnObj;
        }

        private static GameObject CreateSlider(Transform parent, string name, Color fillCol, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject sliderObj = new GameObject(name, typeof(RectTransform));
            sliderObj.transform.SetParent(parent);
            Slider slider = sliderObj.AddComponent<Slider>();

            SetRect(sliderObj, anchorMin, anchorMax, pos, size);

            GameObject bgObj = new GameObject("Background", typeof(RectTransform));
            bgObj.transform.SetParent(sliderObj.transform);
            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            SetRect(bgObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform);
            SetRect(fillArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject fill = new GameObject("Fill", typeof(RectTransform));
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
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Horror"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                    AssetDatabase.CreateFolder("Assets", "Materials");
                AssetDatabase.CreateFolder("Assets/Materials", "Horror");
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
