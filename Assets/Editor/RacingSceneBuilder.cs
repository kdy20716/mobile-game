#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MobileRacing.Editor
{
    public static class RacingSceneBuilder
    {
        [MenuItem("Racing Game/Generate Racing Track & Setup Scene")]
        public static void GenerateFullRacingScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Lighting & Environment
            SetupEnvironment();

            // 2. Materials
            Material roadMat = GetOrCreateMaterial("Assets/Materials/M_Road.mat", new Color(0.12f, 0.12f, 0.14f));
            Material whiteLineMat = GetOrCreateMaterial("Assets/Materials/M_WhiteLine.mat", new Color(0.95f, 0.95f, 0.95f));
            Material yellowLineMat = GetOrCreateMaterial("Assets/Materials/M_YellowLine.mat", new Color(1f, 0.85f, 0.1f));
            Material grassMat = GetOrCreateMaterial("Assets/Materials/M_Grass.mat", new Color(0.2f, 0.45f, 0.2f));
            Material barrierMat = GetOrCreateMaterial("Assets/Materials/M_Barrier.mat", new Color(0.5f, 0.55f, 0.6f));
            Material lampMat = GetOrCreateMaterial("Assets/Materials/M_StreetLamp.mat", new Color(1f, 0.95f, 0.7f));

            // Car Materials
            Material playerCarMat = GetOrCreateMaterial("Assets/Materials/M_PlayerCar.mat", new Color(0.95f, 0.15f, 0.15f)); // Red
            Material aiCar1Mat = GetOrCreateMaterial("Assets/Materials/M_AICar1.mat", new Color(0.15f, 0.45f, 0.95f));    // Blue
            Material aiCar2Mat = GetOrCreateMaterial("Assets/Materials/M_AICar2.mat", new Color(0.95f, 0.8f, 0.1f));      // Yellow
            Material aiCar3Mat = GetOrCreateMaterial("Assets/Materials/M_AICar3.mat", new Color(0.1f, 0.85f, 0.4f));      // Green
            Material glassMat = GetOrCreateMaterial("Assets/Materials/M_CarGlass.mat", new Color(0.08f, 0.08f, 0.15f, 0.85f));
            Material wheelMat = GetOrCreateMaterial("Assets/Materials/M_CarWheel.mat", new Color(0.1f, 0.1f, 0.1f));

            // 3. Ground Plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_Grass";
            ground.transform.position = new Vector3(0, -0.05f, 750f);
            ground.transform.localScale = new Vector3(25f, 1f, 160f); // 250m wide x 1600m long
            ground.GetComponent<MeshRenderer>().material = grassMat;

            // 4. Build 5-Lane Highway (Total Length: 1300m)
            // Lanes: 5 lanes, lane width = 7m, total width = 35m
            // Lane Centers: -14m, -7m, 0m, +7m, +14m
            float highwayLength = 1300f;
            float finishZ = 1200f;
            float totalRoadWidth = 35f;

            GameObject roadRoot = new GameObject("Track_5LaneHighway");

            // Main Road Surface
            GameObject mainRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mainRoad.name = "Highway_Surface";
            mainRoad.transform.SetParent(roadRoot.transform);
            mainRoad.transform.position = new Vector3(0f, 0f, highwayLength * 0.5f);
            mainRoad.transform.localScale = new Vector3(totalRoadWidth, 0.1f, highwayLength);
            mainRoad.GetComponent<MeshRenderer>().material = roadMat;

            // Side Barriers (Left & Right)
            CreateSideBarrier(roadRoot.transform, -totalRoadWidth * 0.5f - 0.3f, highwayLength, barrierMat, "Barrier_Left");
            CreateSideBarrier(roadRoot.transform, totalRoadWidth * 0.5f + 0.3f, highwayLength, barrierMat, "Barrier_Right");

            // Lane Divider Lines
            // 4 internal dividers at X = -10.5, -3.5, +3.5, +10.5
            float[] dividerX = new float[] { -10.5f, -3.5f, 3.5f, 10.5f };
            for (int d = 0; d < dividerX.Length; d++)
            {
                // Dashed lines along highway
                for (float z = 10f; z < highwayLength; z += 20f)
                {
                    GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    dash.name = $"DashLine_{d}_{z}";
                    dash.transform.SetParent(roadRoot.transform);
                    dash.transform.position = new Vector3(dividerX[d], 0.06f, z);
                    dash.transform.localScale = new Vector3(0.25f, 0.05f, 10f);
                    dash.GetComponent<MeshRenderer>().material = whiteLineMat;
                    Object.DestroyImmediate(dash.GetComponent<Collider>());
                }
            }

            // Road Edge Solid Lines
            CreateSolidLine(roadRoot.transform, -totalRoadWidth * 0.5f + 0.3f, highwayLength, yellowLineMat, "EdgeLine_Left");
            CreateSolidLine(roadRoot.transform, totalRoadWidth * 0.5f - 0.3f, highwayLength, yellowLineMat, "EdgeLine_Right");

            // Street Lamps every 60m along highway
            for (float z = 30f; z < highwayLength; z += 60f)
            {
                CreateStreetLamp(roadRoot.transform, -totalRoadWidth * 0.5f - 1.5f, z, barrierMat, lampMat);
                CreateStreetLamp(roadRoot.transform, totalRoadWidth * 0.5f + 1.5f, z, barrierMat, lampMat);
            }

            // Start & Finish Gantries
            BuildGantry(roadRoot.transform, 0f, totalRoadWidth, "START", roadMat, yellowLineMat);
            BuildGantry(roadRoot.transform, finishZ, totalRoadWidth, "FINISH", roadMat, whiteLineMat);

            // 5. Spawn Player Car (Middle Lane: X = 0)
            Vector3 playerStartPos = new Vector3(0f, 0.4f, 5f);
            GameObject playerCar = BuildCar("PlayerCar", playerStartPos, playerCarMat, glassMat, wheelMat, true);

            // 6. Spawn AI Rival Cars (Lanes: -14, -7, +7, +14)
            List<AICarController> rivals = new List<AICarController>();

            // AI 1 (Lane 1: X = -14)
            GameObject ai1 = BuildCar("Rival_BlueBolt", new Vector3(-14f, 0.4f, 5f), aiCar1Mat, glassMat, wheelMat, false);
            var aiCtrl1 = ai1.AddComponent<AICarController>();
            SetupAIWheels(ai1, aiCtrl1);
            aiCtrl1.SetPerformance(42f, 32f, -14f);
            rivals.Add(aiCtrl1);

            // AI 2 (Lane 2: X = -7)
            GameObject ai2 = BuildCar("Rival_YellowFury", new Vector3(-7f, 0.4f, 8f), aiCar2Mat, glassMat, wheelMat, false);
            var aiCtrl2 = ai2.AddComponent<AICarController>();
            SetupAIWheels(ai2, aiCtrl2);
            aiCtrl2.SetPerformance(40f, 34f, -7f);
            rivals.Add(aiCtrl2);

            // AI 3 (Lane 4: X = +7)
            GameObject ai3 = BuildCar("Rival_GreenApex", new Vector3(7f, 0.4f, 6f), aiCar3Mat, glassMat, wheelMat, false);
            var aiCtrl3 = ai3.AddComponent<AICarController>();
            SetupAIWheels(ai3, aiCtrl3);
            aiCtrl3.SetPerformance(41f, 30f, 7f);
            rivals.Add(aiCtrl3);

            // 7. Chase Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            var chaseCam = camObj.AddComponent<ChaseCamera>();
            chaseCam.SetTarget(playerCar.transform);
            camObj.transform.position = playerStartPos + new Vector3(0f, 2.5f, -5.5f);

            // 8. EventSystem
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

            // 9. Input & Game Manager
            GameObject gameMgrObj = new GameObject("GameManager");
            gameMgrObj.AddComponent<MobileInputManager>();
            var raceMgr = gameMgrObj.AddComponent<RaceGameManager>();
            raceMgr.SetupParticipants(playerCar.GetComponent<ArcadeCarController>(), rivals, finishZ);

            // 10. UI Canvas
            BuildUI(playerCar.GetComponent<ArcadeCarController>());

            // 11. Save Scene
            string scenePath = "Assets/Scenes/MobileRacingScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = playerCar;
            Debug.Log("<color=green><b>[Racing Game]</b> 5차선 고속도로 직선 드래그 레이싱 씬이 빌드되었습니다!</color>");
        }

        private static void SetupEnvironment()
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.88f);
            light.intensity = 1.3f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            RenderSettings.ambientLight = new Color(0.4f, 0.42f, 0.46f);
        }

        private static Material GetOrCreateMaterial(string path, Color color)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
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

        private static void CreateSideBarrier(Transform parent, float xPos, float length, Material mat, string name)
        {
            GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = name;
            barrier.transform.SetParent(parent);
            barrier.transform.position = new Vector3(xPos, 0.6f, length * 0.5f);
            barrier.transform.localScale = new Vector3(0.6f, 1.2f, length);
            barrier.GetComponent<MeshRenderer>().material = mat;
        }

        private static void CreateSolidLine(Transform parent, float xPos, float length, Material mat, string name)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = name;
            line.transform.SetParent(parent);
            line.transform.position = new Vector3(xPos, 0.06f, length * 0.5f);
            line.transform.localScale = new Vector3(0.3f, 0.05f, length);
            line.GetComponent<MeshRenderer>().material = mat;
            Object.DestroyImmediate(line.GetComponent<Collider>());
        }

        private static void CreateStreetLamp(Transform parent, float xPos, float zPos, Material poleMat, Material lightMat)
        {
            GameObject lamp = new GameObject("StreetLamp");
            lamp.transform.SetParent(parent);
            lamp.transform.position = new Vector3(xPos, 0, zPos);

            // Pole
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.SetParent(lamp.transform);
            pole.transform.localPosition = new Vector3(0, 4f, 0);
            pole.transform.localScale = new Vector3(0.3f, 4f, 0.3f);
            pole.GetComponent<MeshRenderer>().material = poleMat;
            Object.DestroyImmediate(pole.GetComponent<Collider>());

            // Arm
            float armDir = xPos < 0 ? 1f : -1f;
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.transform.SetParent(lamp.transform);
            arm.transform.localPosition = new Vector3(armDir * 1.5f, 7.8f, 0);
            arm.transform.localScale = new Vector3(3f, 0.2f, 0.3f);
            arm.GetComponent<MeshRenderer>().material = poleMat;
            Object.DestroyImmediate(arm.GetComponent<Collider>());

            // Light Head
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(lamp.transform);
            head.transform.localPosition = new Vector3(armDir * 3f, 7.6f, 0);
            head.transform.localScale = new Vector3(0.8f, 0.3f, 0.6f);
            head.GetComponent<MeshRenderer>().material = lightMat;
            Object.DestroyImmediate(head.GetComponent<Collider>());
        }

        private static void BuildGantry(Transform parent, float zPos, float roadWidth, string label, Material frameMat, Material textMat)
        {
            GameObject gantry = new GameObject($"Gantry_{label}");
            gantry.transform.SetParent(parent);
            gantry.transform.position = new Vector3(0, 0, zPos);

            // Left Post
            GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.transform.SetParent(gantry.transform);
            postL.transform.localPosition = new Vector3(-roadWidth * 0.55f, 4f, 0);
            postL.transform.localScale = new Vector3(0.8f, 4f, 0.8f);
            postL.GetComponent<MeshRenderer>().material = frameMat;

            // Right Post
            GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.transform.SetParent(gantry.transform);
            postR.transform.localPosition = new Vector3(roadWidth * 0.55f, 4f, 0);
            postR.transform.localScale = new Vector3(0.8f, 4f, 0.8f);
            postR.GetComponent<MeshRenderer>().material = frameMat;

            // Beam
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.transform.SetParent(gantry.transform);
            beam.transform.localPosition = new Vector3(0, 8f, 0);
            beam.transform.localScale = new Vector3(roadWidth * 1.15f, 1.4f, 1f);
            beam.GetComponent<MeshRenderer>().material = textMat;
        }

        private static GameObject BuildCar(string name, Vector3 position, Material bodyMat, Material glassMat, Material wheelMat, bool isPlayer)
        {
            GameObject car = new GameObject(name);
            car.transform.position = position;

            Rigidbody rb = car.AddComponent<Rigidbody>();
            rb.mass = 1200f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            BoxCollider collider = car.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, 0.6f, 0);
            collider.size = new Vector3(1.8f, 0.9f, 3.8f);

            // Car Lower Chassis
            GameObject bodyLower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyLower.name = "Body_Lower";
            bodyLower.transform.SetParent(car.transform);
            bodyLower.transform.localPosition = new Vector3(0, 0.5f, 0);
            bodyLower.transform.localScale = new Vector3(1.8f, 0.5f, 3.8f);
            bodyLower.GetComponent<MeshRenderer>().material = bodyMat;
            Object.DestroyImmediate(bodyLower.GetComponent<Collider>());

            // Cabin / Cockpit Glass
            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(car.transform);
            cabin.transform.localPosition = new Vector3(0, 0.95f, -0.2f);
            cabin.transform.localScale = new Vector3(1.4f, 0.55f, 2.0f);
            cabin.GetComponent<MeshRenderer>().material = glassMat;
            Object.DestroyImmediate(cabin.GetComponent<Collider>());

            // Rear Wing / Spoiler
            GameObject spoiler = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spoiler.name = "Spoiler";
            spoiler.transform.SetParent(car.transform);
            spoiler.transform.localPosition = new Vector3(0, 1.1f, -1.8f);
            spoiler.transform.localScale = new Vector3(1.8f, 0.08f, 0.45f);
            spoiler.GetComponent<MeshRenderer>().material = bodyMat;
            Object.DestroyImmediate(spoiler.GetComponent<Collider>());

            // Wheels
            Transform fl = CreateWheel(car.transform, new Vector3(-0.95f, 0.35f, 1.2f), wheelMat, "Wheel_FL");
            Transform fr = CreateWheel(car.transform, new Vector3(0.95f, 0.35f, 1.2f), wheelMat, "Wheel_FR");
            Transform rl = CreateWheel(car.transform, new Vector3(-0.95f, 0.35f, -1.2f), wheelMat, "Wheel_RL");
            Transform rr = CreateWheel(car.transform, new Vector3(0.95f, 0.35f, -1.2f), wheelMat, "Wheel_RR");

            if (isPlayer)
            {
                var controller = car.AddComponent<ArcadeCarController>();
                car.AddComponent<CarAudioSystem>();
                controller.AssignWheels(fl, fr, rl, rr);
            }

            return car;
        }

        private static void SetupAIWheels(GameObject car, AICarController ai)
        {
            Transform fl = car.transform.Find("Wheel_FL");
            Transform fr = car.transform.Find("Wheel_FR");
            Transform rl = car.transform.Find("Wheel_RL");
            Transform rr = car.transform.Find("Wheel_RR");
            ai.AssignWheels(fl, fr, rl, rr);
        }

        private static Transform CreateWheel(Transform parent, Vector3 localPos, Material mat, string name)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent);
            wheel.transform.localPosition = localPos;
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            wheel.transform.localScale = new Vector3(0.7f, 0.2f, 0.7f);
            wheel.GetComponent<MeshRenderer>().material = mat;
            Object.DestroyImmediate(wheel.GetComponent<Collider>());
            return wheel.transform;
        }

        private static void BuildUI(ArcadeCarController car)
        {
            GameObject canvasObj = new GameObject("RacingCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            var uiMgr = canvasObj.AddComponent<RacingUIManager>();
            uiMgr.SetCar(car);

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Speedometer (Top Right)
            GameObject speedObj = CreateUIText(canvasObj.transform, "SpeedText", "0 KM/H", 32, TextAnchor.UpperRight, defaultFont);
            SetRectTransform(speedObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-120, -50), new Vector2(200, 60));
            speedObj.GetComponent<Text>().color = Color.white;

            // Distance Remaining (Top Center)
            GameObject distObj = CreateUIText(canvasObj.transform, "DistanceText", "FINISH: 1200m", 30, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(distObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(300, 50));
            distObj.GetComponent<Text>().color = Color.yellow;

            // Race Time (Top Center below Distance)
            GameObject timeObj = CreateUIText(canvasObj.transform, "RaceTimeText", "TIME: 00:00.00", 20, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(timeObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(250, 30));
            timeObj.GetComponent<Text>().color = Color.white;

            // Real-time Rank Text (Top Left)
            GameObject rankObj = CreateUIText(canvasObj.transform, "RankText", "1ST", 40, TextAnchor.UpperLeft, defaultFont);
            SetRectTransform(rankObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -50), new Vector2(180, 65));
            rankObj.GetComponent<Text>().color = new Color(1f, 0.85f, 0.1f);

            // Center Giant Countdown Text
            GameObject countdownObj = CreateUIText(canvasObj.transform, "CountdownText", "3", 80, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(countdownObj, new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, new Vector2(300, 150));
            countdownObj.GetComponent<Text>().color = Color.yellow;

            // Finish Race Banner (Center)
            GameObject finishPanel = new GameObject("FinishPanel");
            finishPanel.transform.SetParent(canvasObj.transform);
            Image finishBg = finishPanel.AddComponent<Image>();
            finishBg.color = new Color(0, 0, 0, 0.85f);
            SetRectTransform(finishPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460, 230));

            GameObject resultRank = CreateUIText(finishPanel.transform, "ResultRank", "🏆 1ST PLACE WINNER!", 36, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(resultRank, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(420, 60));
            resultRank.GetComponent<Text>().color = Color.yellow;

            GameObject resultTime = CreateUIText(finishPanel.transform, "ResultTime", "TIME: 00:25.45", 22, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(resultTime, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(380, 40));
            resultTime.GetComponent<Text>().color = Color.white;

            GameObject restartBtn = CreateButton(finishPanel.transform, "RestartBtn", "PLAY AGAIN", defaultFont, new Vector2(0, -55), new Vector2(180, 50));
            restartBtn.GetComponent<Button>().onClick.AddListener(() => uiMgr.RestartRace());

            // Connect UI References via SerializedObject
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("speedText").objectReferenceValue = speedObj.GetComponent<Text>();
            so.FindProperty("distanceText").objectReferenceValue = distObj.GetComponent<Text>();
            so.FindProperty("raceTimeText").objectReferenceValue = timeObj.GetComponent<Text>();
            so.FindProperty("rankText").objectReferenceValue = rankObj.GetComponent<Text>();
            so.FindProperty("countdownText").objectReferenceValue = countdownObj.GetComponent<Text>();
            so.FindProperty("raceFinishedPanel").objectReferenceValue = finishPanel;
            so.FindProperty("resultRankText").objectReferenceValue = resultRank.GetComponent<Text>();
            so.FindProperty("resultTimeText").objectReferenceValue = resultTime.GetComponent<Text>();
            so.ApplyModifiedProperties();

            // Mobile On-Screen Buttons
            CreateTouchButton(canvasObj.transform, "Btn_SteerLeft", "◀", defaultFont, MobileTouchButton.ButtonType.SteerLeft,
                new Vector2(0, 0), new Vector2(80, 80), new Vector2(90, 90));

            CreateTouchButton(canvasObj.transform, "Btn_SteerRight", "▶", defaultFont, MobileTouchButton.ButtonType.SteerRight,
                new Vector2(0, 0), new Vector2(190, 80), new Vector2(90, 90));

            CreateTouchButton(canvasObj.transform, "Btn_Gas", "GAS", defaultFont, MobileTouchButton.ButtonType.Accelerate,
                new Vector2(1, 0), new Vector2(-90, 110), new Vector2(100, 110));

            CreateTouchButton(canvasObj.transform, "Btn_Brake", "BRAKE", defaultFont, MobileTouchButton.ButtonType.Brake,
                new Vector2(1, 0), new Vector2(-210, 80), new Vector2(90, 80));

            CreateTouchButton(canvasObj.transform, "Btn_Boost", "NITRO", defaultFont, MobileTouchButton.ButtonType.Boost,
                new Vector2(1, 0), new Vector2(-90, 230), new Vector2(100, 70));
        }

        private static GameObject CreateUIText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Font font)
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

        private static GameObject CreateButton(Transform parent, string name, string label, Font font, Vector2 pos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f);
            Button btn = btnObj.AddComponent<Button>();

            SetRectTransform(btnObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);

            GameObject textObj = CreateUIText(btnObj.transform, "Text", label, 20, TextAnchor.MiddleCenter, font);
            SetRectTransform(textObj, new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().color = Color.white;

            return btnObj;
        }

        private static void CreateTouchButton(Transform parent, string name, string label, Font font, MobileTouchButton.ButtonType type, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = type == MobileTouchButton.ButtonType.Accelerate ? new Color(0.2f, 0.8f, 0.3f, 0.65f) :
                        type == MobileTouchButton.ButtonType.Boost ? new Color(1f, 0.5f, 0.1f, 0.7f) :
                        type == MobileTouchButton.ButtonType.Brake ? new Color(0.9f, 0.2f, 0.2f, 0.65f) :
                        new Color(0.2f, 0.4f, 0.8f, 0.65f);

            var touchBtn = btnObj.AddComponent<MobileTouchButton>();
            touchBtn.type = type;

            SetRectTransform(btnObj, anchor, anchor, pos, size);

            GameObject textObj = CreateUIText(btnObj.transform, "Text", label, 24, TextAnchor.MiddleCenter, font);
            SetRectTransform(textObj, new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
            textObj.GetComponent<Text>().color = Color.white;
        }

        private static void SetRectTransform(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            RectTransform rt = obj.GetComponent<RectTransform>() ?? obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }
    }
}
#endif
