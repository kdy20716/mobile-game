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
            // 1. Create a new empty scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Setup Lighting & Sky
            SetupEnvironment();

            // 3. Create Materials
            Material roadMat = GetOrCreateMaterial("Assets/Materials/M_Road.mat", new Color(0.18f, 0.18f, 0.2f));
            Material lineMat = GetOrCreateMaterial("Assets/Materials/M_RoadLine.mat", new Color(1f, 0.85f, 0.1f));
            Material grassMat = GetOrCreateMaterial("Assets/Materials/M_Grass.mat", new Color(0.25f, 0.55f, 0.25f));
            Material curbMat = GetOrCreateMaterial("Assets/Materials/M_CurbRed.mat", new Color(0.85f, 0.15f, 0.15f));
            Material carMat = GetOrCreateMaterial("Assets/Materials/M_CarBody.mat", new Color(0.9f, 0.2f, 0.2f));
            Material glassMat = GetOrCreateMaterial("Assets/Materials/M_CarGlass.mat", new Color(0.1f, 0.1f, 0.2f, 0.8f));
            Material wheelMat = GetOrCreateMaterial("Assets/Materials/M_CarWheel.mat", new Color(0.1f, 0.1f, 0.1f));

            // 4. Ground Terrain Plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_Grass";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(80f, 1f, 80f);
            ground.GetComponent<MeshRenderer>().material = grassMat;

            // 5. Track Builder & Checkpoints
            GameObject trackRoot = new GameObject("Track_Circuit");
            GameObject checkpointRoot = new GameObject("Track_Checkpoints");
            var trackManager = checkpointRoot.AddComponent<CheckpointTrackManager>();

            List<Transform> checkpointList = BuildCircuitTrack(trackRoot, checkpointRoot, roadMat, lineMat, curbMat);
            trackManager.SetCheckpoints(checkpointList, 3);

            // 6. Spawn Car
            GameObject car = BuildCar(new Vector3(0f, 0.5f, 0f), carMat, glassMat, wheelMat);

            // 7. Chase Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            var chaseCam = camObj.AddComponent<ChaseCamera>();
            chaseCam.SetTarget(car.transform);
            camObj.transform.position = new Vector3(0f, 3f, -6f);

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

            // 9. Mobile Input Manager & UI
            GameObject inputMgrObj = new GameObject("InputManager");
            inputMgrObj.AddComponent<MobileInputManager>();

            BuildUI(car.GetComponent<ArcadeCarController>());

            // 10. Save Scene
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            string scenePath = "Assets/Scenes/MobileRacingScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            // Set Build Settings
            EditorBuildSettingsScene[] originalScenes = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
            newScenes[0] = new EditorBuildSettingsScene(scenePath, true);
            for (int i = 0; i < originalScenes.Length; i++)
            {
                newScenes[i + 1] = originalScenes[i];
            }
            EditorBuildSettings.scenes = newScenes;

            Selection.activeGameObject = car;
            Debug.Log("<color=green><b>[Racing Game]</b> 모바일 레이싱 씬 생성이 완료되었습니다! (Assets/Scenes/MobileRacingScene.unity)</color>");
        }

        private static void SetupEnvironment()
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.85f);
            light.intensity = 1.2f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientLight = new Color(0.35f, 0.35f, 0.4f);
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

        private static List<Transform> BuildCircuitTrack(GameObject trackRoot, GameObject cpRoot, Material roadMat, Material lineMat, Material curbMat)
        {
            List<Transform> checkpoints = new List<Transform>();

            // Circuit Waypoints (Oval / Stadium shape track)
            Vector3[] waypoints = new Vector3[]
            {
                new Vector3(0, 0, 0),        // Start / Finish
                new Vector3(0, 0, 70),       // Straight 1
                new Vector3(15, 0, 100),     // Turn 1
                new Vector3(45, 0, 115),     // Turn 2
                new Vector3(75, 0, 100),     // Turn 3
                new Vector3(90, 0, 70),      // Turn 4
                new Vector3(90, 0, -20),     // Back Straight
                new Vector3(75, 0, -50),     // Turn 5
                new Vector3(45, 0, -65),     // Turn 6
                new Vector3(15, 0, -50),     // Turn 7
            };

            float roadWidth = 14f;

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

                // Center Line
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.name = "CenterLine";
                line.transform.SetParent(seg.transform);
                line.transform.localPosition = new Vector3(0f, 0.52f, 0f);
                line.transform.localRotation = Quaternion.identity;
                line.transform.localScale = new Vector3(0.04f, 0.1f, 0.85f);
                line.GetComponent<MeshRenderer>().material = lineMat;
                Object.DestroyImmediate(line.GetComponent<Collider>());

                // Curbs (Left & Right)
                CreateCurb(seg.transform, -0.52f, curbMat, "Curb_L");
                CreateCurb(seg.transform, 0.52f, curbMat, "Curb_R");

                // Checkpoint Trigger
                GameObject cp = new GameObject($"Checkpoint_{i}");
                cp.transform.SetParent(cpRoot.transform);
                cp.transform.position = midPoint + Vector3.up * 1f;
                cp.transform.rotation = rot;
                BoxCollider box = cp.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = new Vector3(roadWidth + 4f, 6f, 4f);
                var cpTrigger = cp.AddComponent<CheckpointTrigger>();
                cpTrigger.CheckpointIndex = i;
                checkpoints.Add(cp.transform);
            }

            // Finish Line Arch / Banner
            GameObject arch = new GameObject("StartFinish_Banner");
            arch.transform.SetParent(trackRoot.transform);
            arch.transform.position = new Vector3(0, 0, 0);

            GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.transform.SetParent(arch.transform);
            postL.transform.position = new Vector3(-roadWidth * 0.55f, 3.5f, 0);
            postL.transform.localScale = new Vector3(0.6f, 3.5f, 0.6f);
            postL.GetComponent<MeshRenderer>().material = roadMat;

            GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.transform.SetParent(arch.transform);
            postR.transform.position = new Vector3(roadWidth * 0.55f, 3.5f, 0);
            postR.transform.localScale = new Vector3(0.6f, 3.5f, 0.6f);
            postR.GetComponent<MeshRenderer>().material = roadMat;

            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.transform.SetParent(arch.transform);
            beam.transform.position = new Vector3(0, 7f, 0);
            beam.transform.localScale = new Vector3(roadWidth * 1.2f, 1.2f, 0.8f);
            beam.GetComponent<MeshRenderer>().material = curbMat;

            return checkpoints;
        }

        private static void CreateCurb(Transform parent, float localXRatio, Material curbMat, string name)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = name;
            curb.transform.SetParent(parent);
            curb.transform.localPosition = new Vector3(localXRatio, 0.6f, 0f);
            curb.transform.localRotation = Quaternion.identity;
            curb.transform.localScale = new Vector3(0.06f, 0.3f, 1f);
            curb.GetComponent<MeshRenderer>().material = curbMat;
        }

        private static GameObject BuildCar(Vector3 position, Material bodyMat, Material glassMat, Material wheelMat)
        {
            GameObject car = new GameObject("PlayerCar");
            car.transform.position = position;

            Rigidbody rb = car.AddComponent<Rigidbody>();
            rb.mass = 1200f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            BoxCollider collider = car.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, 0.6f, 0);
            collider.size = new Vector3(1.8f, 0.9f, 3.8f);

            var controller = car.AddComponent<ArcadeCarController>();

            // Car Body Meshes
            GameObject bodyLower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyLower.name = "Body_Lower";
            bodyLower.transform.SetParent(car.transform);
            bodyLower.transform.localPosition = new Vector3(0, 0.5f, 0);
            bodyLower.transform.localScale = new Vector3(1.8f, 0.5f, 3.8f);
            bodyLower.GetComponent<MeshRenderer>().material = bodyMat;
            Object.DestroyImmediate(bodyLower.GetComponent<Collider>());

            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(car.transform);
            cabin.transform.localPosition = new Vector3(0, 0.95f, -0.2f);
            cabin.transform.localScale = new Vector3(1.4f, 0.55f, 2.0f);
            cabin.GetComponent<MeshRenderer>().material = glassMat;
            Object.DestroyImmediate(cabin.GetComponent<Collider>());

            // Wheels
            Transform fl = CreateWheel(car.transform, new Vector3(-0.95f, 0.35f, 1.2f), wheelMat, "Wheel_FL");
            Transform fr = CreateWheel(car.transform, new Vector3(0.95f, 0.35f, 1.2f), wheelMat, "Wheel_FR");
            Transform rl = CreateWheel(car.transform, new Vector3(-0.95f, 0.35f, -1.2f), wheelMat, "Wheel_RL");
            Transform rr = CreateWheel(car.transform, new Vector3(0.95f, 0.35f, -1.2f), wheelMat, "Wheel_RR");

            controller.AssignWheels(fl, fr, rl, rr);

            return car;
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

            // Speedometer Text (Top Right)
            GameObject speedObj = CreateUIText(canvasObj.transform, "SpeedText", "0 KM/H", 32, TextAnchor.UpperRight, defaultFont);
            SetRectTransform(speedObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-120, -50), new Vector2(200, 60));
            speedObj.GetComponent<Text>().color = Color.white;

            // Lap Text (Top Left)
            GameObject lapObj = CreateUIText(canvasObj.transform, "LapText", "LAP 1/3", 26, TextAnchor.UpperLeft, defaultFont);
            SetRectTransform(lapObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -40), new Vector2(180, 50));
            lapObj.GetComponent<Text>().color = Color.yellow;

            // Lap Time Text (Top Center)
            GameObject timeObj = CreateUIText(canvasObj.transform, "LapTimeText", "00:00.00", 30, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(timeObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(250, 50));
            timeObj.GetComponent<Text>().color = Color.white;

            // Best Time Text
            GameObject bestObj = CreateUIText(canvasObj.transform, "BestLapText", "BEST: --:--.--", 18, TextAnchor.UpperCenter, defaultFont);
            SetRectTransform(bestObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(250, 30));
            bestObj.GetComponent<Text>().color = new Color(0.8f, 0.8f, 0.8f);

            // Connect Text to UI Manager via SerializedObject
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("speedText").objectReferenceValue = speedObj.GetComponent<Text>();
            so.FindProperty("lapText").objectReferenceValue = lapObj.GetComponent<Text>();
            so.FindProperty("lapTimeText").objectReferenceValue = timeObj.GetComponent<Text>();
            so.FindProperty("bestLapText").objectReferenceValue = bestObj.GetComponent<Text>();

            // Finish Race Banner (Center)
            GameObject finishPanel = new GameObject("FinishPanel");
            finishPanel.transform.SetParent(canvasObj.transform);
            Image finishBg = finishPanel.AddComponent<Image>();
            finishBg.color = new Color(0, 0, 0, 0.75f);
            SetRectTransform(finishPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 180));

            GameObject finishText = CreateUIText(finishPanel.transform, "FinishText", "RACE FINISHED!", 36, TextAnchor.MiddleCenter, defaultFont);
            SetRectTransform(finishText, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, new Vector2(350, 60));
            finishText.GetComponent<Text>().color = Color.yellow;

            GameObject restartBtn = CreateButton(finishPanel.transform, "RestartBtn", "PLAY AGAIN", defaultFont, new Vector2(0, -40), new Vector2(160, 50));
            restartBtn.GetComponent<Button>().onClick.AddListener(() => uiMgr.RestartRace());

            so.FindProperty("raceFinishedPanel").objectReferenceValue = finishPanel;
            so.ApplyModifiedProperties();

            // Mobile On-Screen Buttons
            // 1. Left Steer Button (Bottom Left)
            CreateTouchButton(canvasObj.transform, "Btn_SteerLeft", "◀", defaultFont, MobileTouchButton.ButtonType.SteerLeft,
                new Vector2(0, 0), new Vector2(80, 80), new Vector2(90, 90));

            // 2. Right Steer Button (Bottom Left next to SteerLeft)
            CreateTouchButton(canvasObj.transform, "Btn_SteerRight", "▶", defaultFont, MobileTouchButton.ButtonType.SteerRight,
                new Vector2(0, 0), new Vector2(190, 80), new Vector2(90, 90));

            // 3. Accelerate Button (Bottom Right)
            CreateTouchButton(canvasObj.transform, "Btn_Gas", "GAS", defaultFont, MobileTouchButton.ButtonType.Accelerate,
                new Vector2(1, 0), new Vector2(-90, 110), new Vector2(100, 110));

            // 4. Brake Button (Bottom Right)
            CreateTouchButton(canvasObj.transform, "Btn_Brake", "BRAKE", defaultFont, MobileTouchButton.ButtonType.Brake,
                new Vector2(1, 0), new Vector2(-210, 80), new Vector2(90, 80));

            // 5. Boost Button (Bottom Right Above Brake)
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
