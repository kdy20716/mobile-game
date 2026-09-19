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
            Material lineMat = GetOrCreateMaterial("Assets/Materials/M_RoadLine.mat", new Color(1f, 0.9f, 0.2f));
            Material grassMat = GetOrCreateMaterial("Assets/Materials/M_Grass.mat", new Color(0.22f, 0.52f, 0.24f));
            Material curbRed = GetOrCreateMaterial("Assets/Materials/M_CurbRed.mat", new Color(0.85f, 0.15f, 0.15f));
            Material curbWhite = GetOrCreateMaterial("Assets/Materials/M_CurbWhite.mat", new Color(0.95f, 0.95f, 0.95f));
            Material barrierMat = GetOrCreateMaterial("Assets/Materials/M_Barrier.mat", new Color(0.6f, 0.65f, 0.7f));

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
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(120f, 1f, 120f);
            ground.GetComponent<MeshRenderer>().material = grassMat;

            // 4. Track & Waypoints
            GameObject trackRoot = new GameObject("Track_Circuit");
            GameObject checkpointRoot = new GameObject("Track_Checkpoints");
            var trackManager = checkpointRoot.AddComponent<CheckpointTrackManager>();

            List<Transform> waypoints = BuildCircuitTrack(trackRoot, checkpointRoot, roadMat, lineMat, curbRed, curbWhite, barrierMat);
            trackManager.SetCheckpoints(waypoints, 3);

            // 5. Spawn Player Car (Grid 4 - 4th position or Pole)
            Vector3 startPos = new Vector3(3f, 0.4f, 0f);
            GameObject playerCar = BuildCar("PlayerCar", startPos, playerCarMat, glassMat, wheelMat, true);

            // 6. Spawn AI Rival Cars
            List<AICarController> rivals = new List<AICarController>();

            // AI 1 (Pole position)
            GameObject ai1 = BuildCar("Rival_PhantomBlue", new Vector3(-3f, 0.4f, 8f), aiCar1Mat, glassMat, wheelMat, false);
            var aiCtrl1 = ai1.AddComponent<AICarController>();
            SetupAIWheels(ai1, aiCtrl1);
            aiCtrl1.SetWaypoints(waypoints, 34f, 28f);
            rivals.Add(aiCtrl1);

            // AI 2 (Grid 2)
            GameObject ai2 = BuildCar("Rival_ViperYellow", new Vector3(3f, 0.4f, 14f), aiCar2Mat, glassMat, wheelMat, false);
            var aiCtrl2 = ai2.AddComponent<AICarController>();
            SetupAIWheels(ai2, aiCtrl2);
            aiCtrl2.SetWaypoints(waypoints, 33f, 26f);
            rivals.Add(aiCtrl2);

            // AI 3 (Grid 3)
            GameObject ai3 = BuildCar("Rival_ApexGreen", new Vector3(-3f, 0.4f, 20f), aiCar3Mat, glassMat, wheelMat, false);
            var aiCtrl3 = ai3.AddComponent<AICarController>();
            SetupAIWheels(ai3, aiCtrl3);
            aiCtrl3.SetWaypoints(waypoints, 35f, 30f);
            rivals.Add(aiCtrl3);

            // 7. Chase Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            var chaseCam = camObj.AddComponent<ChaseCamera>();
            chaseCam.SetTarget(playerCar.transform);
            camObj.transform.position = startPos + new Vector3(0f, 3f, -6f);

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
            raceMgr.SetupParticipants(playerCar.GetComponent<ArcadeCarController>(), rivals);

            // 10. UI Canvas
            BuildUI(playerCar.GetComponent<ArcadeCarController>());

            // 11. Save Scene
            string scenePath = "Assets/Scenes/MobileRacingScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = playerCar;
            Debug.Log("<color=green><b>[Racing Game]</b> 업그레이드된 모바일 레이싱 씬이 성공적으로 빌드되었습니다!</color>");
        }

        private static void SetupEnvironment()
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.88f);
            light.intensity = 1.3f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -40f, 0f);
            RenderSettings.ambientLight = new Color(0.38f, 0.4f, 0.45f);
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

        private static List<Transform> BuildCircuitTrack(GameObject trackRoot, GameObject cpRoot, Material roadMat, Material lineMat, Material curbRed, Material curbWhite, Material barrierMat)
        {
            List<Transform> checkpoints = new List<Transform>();

            // Professional Grand Prix Circuit Waypoints
            Vector3[] waypoints = new Vector3[]
            {
                new Vector3(0, 0, 0),        // Start/Finish
                new Vector3(0, 0, 90),       // Main Straight
                new Vector3(15, 0, 130),     // Turn 1 Entry
                new Vector3(50, 0, 155),     // Turn 1 Apex
                new Vector3(90, 0, 140),     // Turn 2
                new Vector3(120, 0, 90),     // East Straight
                new Vector3(115, 0, 20),     // Chicane Entry
                new Vector3(80, 0, -10),     // Chicane Apex
                new Vector3(90, 0, -60),     // South Bend
                new Vector3(60, 0, -110),    // Hairpin Entry
                new Vector3(20, 0, -110),    // Hairpin Apex
                new Vector3(-10, 0, -70),    // Final Curve
                new Vector3(-5, 0, -25)      // Heading to Grid
            };

            float roadWidth = 15f;

            for (int i = 0; i < waypoints.Length; i++)
            {
                Vector3 p1 = waypoints[i];
                Vector3 p2 = waypoints[(i + 1) % waypoints.Length];
                Vector3 midPoint = (p1 + p2) * 0.5f;
                Vector3 forward = (p2 - p1).normalized;
                float segLength = Vector3.Distance(p1, p2);
                Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);

                // Road Segment
                GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                seg.name = $"Road_Segment_{i}";
                seg.transform.SetParent(trackRoot.transform);
                seg.transform.position = midPoint + Vector3.up * 0.05f;
                seg.transform.rotation = rot;
                seg.transform.localScale = new Vector3(roadWidth, 0.1f, segLength);
                seg.GetComponent<MeshRenderer>().material = roadMat;

                // Center Dashed Line
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.name = "CenterLine";
                line.transform.SetParent(seg.transform);
                line.transform.localPosition = new Vector3(0f, 0.52f, 0f);
                line.transform.localScale = new Vector3(0.04f, 0.1f, 0.85f);
                line.GetComponent<MeshRenderer>().material = lineMat;
                Object.DestroyImmediate(line.GetComponent<Collider>());

                // Curbs (Red & White Alternating)
                Material curbColor = (i % 2 == 0) ? curbRed : curbWhite;
                CreateCurb(seg.transform, -0.52f, curbColor, "Curb_L");
                CreateCurb(seg.transform, 0.52f, curbColor, "Curb_R");

                // Guardrails / Barriers
                CreateBarrier(seg.transform, -0.56f, barrierMat, "Barrier_L");
                CreateBarrier(seg.transform, 0.56f, barrierMat, "Barrier_R");

                // Checkpoint
                GameObject cp = new GameObject($"Checkpoint_{i}");
                cp.transform.SetParent(cpRoot.transform);
                cp.transform.position = midPoint + Vector3.up * 1f;
                cp.transform.rotation = rot;
                BoxCollider box = cp.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = new Vector3(roadWidth + 4f, 6f, 5f);
                var cpTrigger = cp.AddComponent<CheckpointTrigger>();
                cpTrigger.CheckpointIndex = i;
                checkpoints.Add(cp.transform);
            }

            // Gantry Arch
            BuildStartGantry(trackRoot.transform, roadWidth, curbRed, roadMat);

            return checkpoints;
        }

        private static void CreateCurb(Transform parent, float localXRatio, Material mat, string name)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = name;
            curb.transform.SetParent(parent);
            curb.transform.localPosition = new Vector3(localXRatio, 0.55f, 0f);
            curb.transform.localScale = new Vector3(0.05f, 0.25f, 1f);
            curb.GetComponent<MeshRenderer>().material = mat;
        }

        private static void CreateBarrier(Transform parent, float localXRatio, Material mat, string name)
        {
            GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = name;
            barrier.transform.SetParent(parent);
            barrier.transform.localPosition = new Vector3(localXRatio, 1.2f, 0f);
            barrier.transform.localScale = new Vector3(0.04f, 1.2f, 1f);
            barrier.GetComponent<MeshRenderer>().material = mat;
        }

        private static void BuildStartGantry(Transform parent, float roadWidth, Material redMat, Material darkMat)
        {
            GameObject arch = new GameObject("StartFinish_Gantry");
            arch.transform.SetParent(parent);
            arch.transform.position = new Vector3(0, 0, 0);

            GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.transform.SetParent(arch.transform);
            postL.transform.position = new Vector3(-roadWidth * 0.55f, 3.5f, 0);
            postL.transform.localScale = new Vector3(0.6f, 3.5f, 0.6f);
            postL.GetComponent<MeshRenderer>().material = darkMat;

            GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.transform.SetParent(arch.transform);
            postR.transform.position = new Vector3(roadWidth * 0.55f, 3.5f, 0);
            postR.transform.localScale = new Vector3(0.6f, 3.5f, 0.6f);
            postR.GetComponent<MeshRenderer>().material = darkMat;

            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.transform.SetParent(arch.transform);
            beam.transform.position = new Vector3(0, 7f, 0);
            beam.transform.localScale = new Vector3(roadWidth * 1.2f, 1.2f, 0.8f);
            beam.GetComponent<MeshRenderer>().material = redMat;
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

            // Lap Text (Top Left)
            GameObject lapObj = CreateUIText(canvasObj.transform, "LapText", "LAP 1/3", 26, TextAnchor.UpperLeft, defaultFont);
            SetRectTransform(lapObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -40), new Vector2(180, 50));
            lapObj.GetComponent<Text>().color = Color.yellow;

            // Real-time Rank Text (Top Left below Lap)
            GameObject rankObj = CreateUIText(canvasObj.transform, "RankText", "1ST", 36, TextAnchor.UpperLeft, defaultFont);
            SetRectTransform(rankObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -90), new Vector2(180, 55));
            rankObj.GetComponent<Text>().color = new Color(1f, 0.85f, 0.1f);

            // Lap Time Text (Top Center)
            GameObject timeObj = CreateUIText(canvasObj.transform, "LapTimeText", "00:00.00", 30, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(timeObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(250, 50));
            timeObj.GetComponent<Text>().color = Color.white;

            // Best Time Text
            GameObject bestObj = CreateUIText(canvasObj.transform, "BestLapText", "BEST: --:--.--", 18, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(bestObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(250, 30));
            bestObj.GetComponent<Text>().color = new Color(0.8f, 0.8f, 0.8f);

            // Center Giant Countdown Text
            GameObject countdownObj = CreateUIText(canvasObj.transform, "CountdownText", "3", 80, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(countdownObj, new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, new Vector2(300, 150));
            countdownObj.GetComponent<Text>().color = Color.yellow;

            // Finish Race Banner (Center)
            GameObject finishPanel = new GameObject("FinishPanel");
            finishPanel.transform.SetParent(canvasObj.transform);
            Image finishBg = finishPanel.AddComponent<Image>();
            finishBg.color = new Color(0, 0, 0, 0.82f);
            SetRectTransform(finishPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(440, 220));

            GameObject resultRank = CreateUIText(finishPanel.transform, "ResultRank", "🏆 1ST PLACE!", 36, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(resultRank, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(400, 60));
            resultRank.GetComponent<Text>().color = Color.yellow;

            GameObject resultTime = CreateUIText(finishPanel.transform, "ResultTime", "TOTAL TIME: 01:23.45", 22, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(resultTime, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(380, 40));
            resultTime.GetComponent<Text>().color = Color.white;

            GameObject restartBtn = CreateButton(finishPanel.transform, "RestartBtn", "PLAY AGAIN", defaultFont, new Vector2(0, -55), new Vector2(180, 50));
            restartBtn.GetComponent<Button>().onClick.AddListener(() => uiMgr.RestartRace());

            // Connect UI References via SerializedObject
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("speedText").objectReferenceValue = speedObj.GetComponent<Text>();
            so.FindProperty("lapText").objectReferenceValue = lapObj.GetComponent<Text>();
            so.FindProperty("lapTimeText").objectReferenceValue = timeObj.GetComponent<Text>();
            so.FindProperty("bestLapText").objectReferenceValue = bestObj.GetComponent<Text>();
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
