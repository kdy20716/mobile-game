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
        [MenuItem("Lethal Company/Generate Phase 2 Exploration & Quota Scene")]
        public static void GeneratePhase2Scene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Materials
            Material shipMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_ShipHull.mat", new Color(0.2f, 0.22f, 0.25f));
            Material shipFloorMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_ShipFloor.mat", new Color(0.3f, 0.28f, 0.26f));
            Material terrainMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_PlanetSurface.mat", new Color(0.18f, 0.16f, 0.14f));
            Material facilityWallMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_IndustrialWall.mat", new Color(0.35f, 0.38f, 0.4f));
            Material facilityFloorMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_ConcreteFloor.mat", new Color(0.25f, 0.26f, 0.28f));
            Material metalMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_MetalScrap.mat", new Color(0.6f, 0.55f, 0.45f));
            Material engineMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_HeavyEngine.mat", new Color(0.2f, 0.22f, 0.24f));
            Material goldMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_BrassBell.mat", new Color(0.9f, 0.75f, 0.2f));
            Material doorMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_HeavyDoor.mat", new Color(0.8f, 0.2f, 0.2f));

            // Ambient & Fog (Dark Moody Atmosphere)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.09f, 0.12f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.05f, 0.06f, 0.08f);
            RenderSettings.fogDensity = 0.015f;

            // 2. Build Ship at (0, 0, 0)
            GameObject shipRoot = new GameObject("Ship_LandingCraft");
            Transform playerSpawn = BuildShip(shipRoot.transform, shipMat, shipFloorMat);

            // 3. Build Planet Terrain (60m x 60m)
            GameObject terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
            terrain.name = "PlanetSurface";
            terrain.transform.position = new Vector3(0f, -0.05f, 15f);
            terrain.transform.localScale = new Vector3(8f, 1f, 8f);
            terrain.GetComponent<MeshRenderer>().material = terrainMat;

            // Some rocks / pathway markers to facility
            CreatePathwayRocks(terrain.transform, terrainMat);

            // 4. Build Facility Entrance on Planet Surface at (0, 0, 32)
            GameObject exteriorDoorObj = CreateDoorObject(new Vector3(0f, 1.5f, 32f), Quaternion.identity, "Door_FacilityEntrance", doorMat);
            var exteriorDoor = exteriorDoorObj.AddComponent<FacilityDoor>();

            // 5. Build Facility Interior Far Away at (100, 0, 100)
            Vector3 facilityOrigin = new Vector3(100f, 0f, 100f);
            GameObject facilityRoot = new GameObject("Facility_Interior");
            facilityRoot.transform.position = facilityOrigin;
            BuildFacilityInterior(facilityRoot.transform, facilityFloorMat, facilityWallMat);

            // Facility Interior Exit Door
            GameObject interiorDoorObj = CreateDoorObject(facilityOrigin + new Vector3(0f, 1.5f, -9.5f), Quaternion.Euler(0f, 180f, 0f), "Door_FacilityExit", doorMat);
            var interiorDoor = interiorDoorObj.AddComponent<FacilityDoor>();

            // Link doors
            exteriorDoor.SetDestination(interiorDoor.transform, false);
            interiorDoor.SetDestination(exteriorDoor.transform, true);

            // 6. Spawn Diverse Scraps inside Facility
            SpawnScrapItems(facilityOrigin, metalMat, engineMat, goldMat);

            // 7. Player Character Setup (Spawns in Ship)
            GameObject player = new GameObject("LethalScavenger");
            player.transform.position = playerSpawn.position;
            player.transform.rotation = playerSpawn.rotation;

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

            // Headlight
            GameObject headLight = new GameObject("Headlight");
            headLight.transform.SetParent(camObj.transform);
            headLight.transform.localPosition = Vector3.forward * 0.1f;
            Light hl = headLight.AddComponent<Light>();
            hl.type = LightType.Spot;
            hl.range = 25f;
            hl.spotAngle = 55f;
            hl.intensity = 2.0f;
            hl.color = new Color(1f, 0.95f, 0.85f);
            hl.shadows = LightShadows.Hard;

            // 8. Game Managers
            GameObject gmObj = new GameObject("GameManager");
            var gameMgr = gmObj.AddComponent<LethalGameManager>();

            SerializedObject soGM = new SerializedObject(gameMgr);
            soGM.FindProperty("shipSpawnPoint").objectReferenceValue = playerSpawn;
            soGM.FindProperty("playerObj").objectReferenceValue = player;
            soGM.ApplyModifiedProperties();

            // 9. EventSystem
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

            // 10. UI Canvas
            BuildLethalUI(inventory, interaction);

            // 11. Save Scene
            string scenePath = "Assets/Scenes/LethalScrapScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = player;
            Debug.Log("<color=green><b>[Lethal Company]</b> Phase 2 우주선 탐사 루프 및 할당량 씬이 완성되었습니다! (Assets/Scenes/LethalScrapScene.unity)</color>");
        }

        private static Transform BuildShip(Transform parent, Material hullMat, Material floorMat)
        {
            // Ship Hull (Room size: 6m x 8m, Height 3m)
            // Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Ship_Floor";
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(0f, 0.1f, 0f);
            floor.transform.localScale = new Vector3(6f, 0.2f, 8f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            // Ceiling
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ship_Ceiling";
            ceiling.transform.SetParent(parent);
            ceiling.transform.position = new Vector3(0f, 3.1f, 0f);
            ceiling.transform.localScale = new Vector3(6f, 0.2f, 8f);
            ceiling.GetComponent<MeshRenderer>().material = hullMat;

            // Left Wall (West)
            CreateWall(parent, new Vector3(-3f, 1.6f, 0f), new Vector3(0.3f, 3f, 8f), hullMat, "Ship_Wall_Left");
            // Right Wall (East)
            CreateWall(parent, new Vector3(3f, 1.6f, 0f), new Vector3(0.3f, 3f, 8f), hullMat, "Ship_Wall_Right");
            // Back Wall (South)
            CreateWall(parent, new Vector3(0f, 1.6f, -4f), new Vector3(6f, 3f, 0.3f), hullMat, "Ship_Wall_Back");

            // Front Wall with Door Opening (North)
            CreateWall(parent, new Vector3(-2f, 1.6f, 4f), new Vector3(2f, 3f, 0.3f), hullMat, "Ship_Wall_FrontL");
            CreateWall(parent, new Vector3(2f, 1.6f, 4f), new Vector3(2f, 3f, 0.3f), hullMat, "Ship_Wall_FrontR");
            CreateWall(parent, new Vector3(0f, 2.7f, 4f), new Vector3(2f, 0.8f, 0.3f), hullMat, "Ship_Wall_FrontTop");

            // Ramp leading out of ship
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ship_Ramp";
            ramp.transform.SetParent(parent);
            ramp.transform.position = new Vector3(0f, 0.05f, 5.2f);
            ramp.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            ramp.transform.localScale = new Vector3(2.2f, 0.15f, 2.6f);
            ramp.GetComponent<MeshRenderer>().material = hullMat;

            // Ship Cargo Area Trigger (Drop-off Scrap Zone)
            GameObject cargoZone = new GameObject("ScrapCargoZone");
            cargoZone.transform.SetParent(parent);
            cargoZone.transform.position = new Vector3(-1.2f, 0.8f, -1.5f);
            BoxCollider cargoCol = cargoZone.AddComponent<BoxCollider>();
            cargoCol.isTrigger = true;
            cargoCol.size = new Vector3(3f, 1.5f, 4f);

            var shipMgr = cargoZone.AddComponent<LethalShipManager>();

            // Visual Cargo Floor marking
            GameObject cargoMark = GameObject.CreatePrimitive(PrimitiveType.Quad);
            cargoMark.name = "CargoFloorMarker";
            cargoMark.transform.SetParent(cargoZone.transform);
            cargoMark.transform.localPosition = new Vector3(0f, -0.68f, 0f);
            cargoMark.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            cargoMark.transform.localScale = new Vector3(2.8f, 3.8f, 1f);
            Material markMat = GetOrCreateMaterial("Assets/Materials/Lethal/M_CargoMarker.mat", new Color(0.8f, 0.6f, 0.1f, 0.5f));
            cargoMark.GetComponent<MeshRenderer>().material = markMat;
            Object.DestroyImmediate(cargoMark.GetComponent<Collider>());

            // Ship Take-off Lever on Console
            GameObject leverConsole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leverConsole.name = "ShipControlConsole";
            leverConsole.transform.SetParent(parent);
            leverConsole.transform.position = new Vector3(2f, 0.8f, 2f);
            leverConsole.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            leverConsole.GetComponent<MeshRenderer>().material = hullMat;

            GameObject leverObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leverObj.name = "TakeoffLever";
            leverObj.transform.SetParent(leverConsole.transform);
            leverObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            leverObj.transform.localRotation = Quaternion.Euler(-45f, 0f, 0f);
            leverObj.transform.localScale = new Vector3(0.12f, 0.4f, 0.12f);
            leverObj.GetComponent<MeshRenderer>().material = GetOrCreateMaterial("Assets/Materials/Lethal/M_LeverRed.mat", Color.red);
            leverObj.AddComponent<ShipLever>();

            // Interior Light
            GameObject shipLight = new GameObject("ShipLamp");
            shipLight.transform.SetParent(parent);
            shipLight.transform.position = new Vector3(0f, 2.7f, 0f);
            Light sl = shipLight.AddComponent<Light>();
            sl.type = LightType.Point;
            sl.range = 10f;
            sl.intensity = 1.8f;
            sl.color = new Color(1f, 0.9f, 0.75f);

            // Player Spawn Transform
            GameObject spawnPoint = new GameObject("ShipPlayerSpawn");
            spawnPoint.transform.SetParent(parent);
            spawnPoint.transform.position = new Vector3(0f, 0.3f, 0f);
            spawnPoint.transform.rotation = Quaternion.identity;

            return spawnPoint.transform;
        }

        private static void CreatePathwayRocks(Transform parent, Material mat)
        {
            Vector3[] rockPositions = new Vector3[]
            {
                new Vector3(-4f, 0.5f, 10f),
                new Vector3(5f, 0.8f, 16f),
                new Vector3(-6f, 1.2f, 22f),
                new Vector3(4f, 0.7f, 26f),
                new Vector3(-3f, 0.6f, 28f)
            };

            foreach (var pos in rockPositions)
            {
                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rock.name = "PlanetRock";
                rock.transform.SetParent(parent);
                rock.transform.position = pos;
                rock.transform.localScale = new Vector3(1.5f, 1.2f, 1.8f);
                rock.GetComponent<MeshRenderer>().material = mat;
            }
        }

        private static GameObject CreateDoorObject(Vector3 pos, Quaternion rot, string name, Material mat)
        {
            GameObject doorFrame = new GameObject(name);
            doorFrame.transform.position = pos;
            doorFrame.transform.rotation = rot;

            // Left post
            CreateWall(doorFrame.transform, new Vector3(-1.1f, 0f, 0f), new Vector3(0.3f, 3f, 0.5f), mat, "Post_L");
            // Right post
            CreateWall(doorFrame.transform, new Vector3(1.1f, 0f, 0f), new Vector3(0.3f, 3f, 0.5f), mat, "Post_R");
            // Top beam
            CreateWall(doorFrame.transform, new Vector3(0f, 1.5f, 0f), new Vector3(2.5f, 0.3f, 0.5f), mat, "Beam_Top");

            // Door panel (Interactive Collider)
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "Door_Panel";
            panel.transform.SetParent(doorFrame.transform);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = new Vector3(2f, 2.8f, 0.2f);
            panel.GetComponent<MeshRenderer>().material = mat;

            // Warning Light above door
            GameObject lightObj = new GameObject("DoorLight");
            lightObj.transform.SetParent(doorFrame.transform);
            lightObj.transform.localPosition = new Vector3(0f, 1.6f, 0.4f);
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 6f;
            l.intensity = 2f;
            l.color = new Color(1f, 0.3f, 0.2f);

            return doorFrame;
        }

        private static void BuildFacilityInterior(Transform parent, Material floorMat, Material wallMat)
        {
            // Main Facility Hall (24m x 20m)
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Interior_Floor";
            floor.transform.SetParent(parent);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(2.4f, 1f, 2.0f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ceiling.name = "Interior_Ceiling";
            ceiling.transform.SetParent(parent);
            ceiling.transform.localPosition = new Vector3(0f, 4.5f, 0f);
            ceiling.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            ceiling.transform.localScale = new Vector3(2.4f, 1f, 2.0f);
            ceiling.GetComponent<MeshRenderer>().material = wallMat;

            // Outer Walls
            CreateWall(parent, parent.position + new Vector3(0f, 2.25f, 10f), new Vector3(24f, 4.5f, 0.5f), wallMat, "Wall_North");
            CreateWall(parent, parent.position + new Vector3(0f, 2.25f, -10f), new Vector3(24f, 4.5f, 0.5f), wallMat, "Wall_South");
            CreateWall(parent, parent.position + new Vector3(12f, 2.25f, 0f), new Vector3(0.5f, 4.5f, 20f), wallMat, "Wall_East");
            CreateWall(parent, parent.position + new Vector3(-12f, 2.25f, 0f), new Vector3(0.5f, 4.5f, 20f), wallMat, "Wall_West");

            // Partition / Inner maze walls
            CreateWall(parent, parent.position + new Vector3(-4f, 2.25f, 2f), new Vector3(0.5f, 4.5f, 10f), wallMat, "Wall_Partition1");
            CreateWall(parent, parent.position + new Vector3(4f, 2.25f, -2f), new Vector3(0.5f, 4.5f, 10f), wallMat, "Wall_Partition2");

            // Industrial Crates
            CreateCrate(parent, parent.position + new Vector3(-8f, 0.75f, 5f), new Vector3(2f, 1.5f, 2f), wallMat);
            CreateCrate(parent, parent.position + new Vector3(7f, 0.6f, 4f), new Vector3(2f, 1.2f, 3f), wallMat);
            CreateCrate(parent, parent.position + new Vector3(-6f, 0.5f, -5f), new Vector3(1.5f, 1.0f, 1.5f), wallMat);

            // Flickering Interior Lights
            CreateRoomLight(parent, parent.position + new Vector3(-6f, 4f, 4f));
            CreateRoomLight(parent, parent.position + new Vector3(6f, 4f, -4f));
            CreateRoomLight(parent, parent.position + new Vector3(0f, 4f, 6f));
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

        private static void SpawnScrapItems(Vector3 origin, Material metalMat, Material engineMat, Material goldMat)
        {
            // 1. V-Type Engine (Heavy Scrap: 42 lb, $85)
            CreateEngineScrap(origin + new Vector3(-8f, 1.8f, 5f), engineMat, "V-Type Engine", 95, 45f);

            // 2. Big Bolt (Light Scrap: 6 lb, $28)
            CreateBoltScrap(origin + new Vector3(2f, 0.3f, 5f), metalMat, "Big Bolt", 32, 6f);
            CreateBoltScrap(origin + new Vector3(-2f, 0.3f, 7f), metalMat, "Big Bolt", 36, 7f);
            CreateBoltScrap(origin + new Vector3(8f, 0.3f, -6f), metalMat, "Big Bolt", 28, 5f);

            // 3. Brass Bell (Medium Scrap: 18 lb, $64)
            CreateBellScrap(origin + new Vector3(7f, 1.5f, 4f), goldMat, "Brass Bell", 68, 18f);

            // 4. Metal Axle (Medium Scrap: 24 lb, $52)
            CreateAxleScrap(origin + new Vector3(-7f, 0.3f, -4f), metalMat, "Metal Axle", 55, 24f);

            // 5. Golden Goblet (Light Scrap: 4 lb, $110)
            CreateCupScrap(origin + new Vector3(9f, 0.4f, 6f), goldMat, "Golden Goblet", 110, 4f);

            // 6. Gold Bar (Heavy Scrap: 20 lb, $145)
            CreateGoldBarScrap(origin + new Vector3(-6f, 1.2f, -5f), goldMat, "Gold Bar", 145, 20f);
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

        private static void CreateGoldBarScrap(Vector3 pos, Material mat, string name, int val, float weight)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.45f, 0.2f, 0.25f);
            obj.GetComponent<MeshRenderer>().material = mat;

            var scrap = obj.AddComponent<ScrapItem>();
            scrap.itemName = name;
            scrap.scrapValue = val;
            scrap.weightLb = weight;
            scrap.handHoldOffset = new Vector3(0.25f, -0.2f, 0.5f);
            scrap.handHoldScale = new Vector3(0.3f, 0.15f, 0.18f);
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
            GameObject weightObj = CreateText(canvasObj.transform, "WeightText", "WEIGHT: 0 lb", 30, TextAnchor.MiddleLeft, defaultFont);
            SetRect(weightObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(60, -60), new Vector2(320, 50));
            weightObj.GetComponent<Text>().color = Color.white;

            // Time & Day Display (Top Center / Right)
            GameObject timeObj = CreateText(canvasObj.transform, "TimeText", "08:00 AM", 32, TextAnchor.MiddleRight, defaultFont);
            SetRect(timeObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-60, -60), new Vector2(240, 50));
            timeObj.GetComponent<Text>().color = new Color(1f, 0.85f, 0.3f);

            GameObject dayObj = CreateText(canvasObj.transform, "DayText", "DAY 1\n3 DAYS LEFT", 24, TextAnchor.MiddleRight, defaultFont);
            SetRect(dayObj, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-60, -120), new Vector2(260, 60));
            dayObj.GetComponent<Text>().color = Color.white;

            // Quota & Cargo Display (Top Center)
            GameObject quotaObj = CreateText(canvasObj.transform, "QuotaText", "PROFIT QUOTA:\n$0 / $300", 26, TextAnchor.UpperCenter, defaultFont);
            SetRect(quotaObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(350, 70));
            quotaObj.GetComponent<Text>().color = Color.white;

            GameObject cargoObj = CreateText(canvasObj.transform, "CargoText", "SHIP CARGO: $0", 24, TextAnchor.UpperCenter, defaultFont);
            SetRect(cargoObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -135), new Vector2(300, 45));
            cargoObj.GetComponent<Text>().color = new Color(0.3f, 0.9f, 0.4f);

            // Center Crosshair
            GameObject crosshair = new GameObject("Crosshair", typeof(RectTransform));
            crosshair.transform.SetParent(canvasObj.transform);
            Image crossImg = crosshair.AddComponent<Image>();
            crossImg.color = new Color(1f, 1f, 1f, 0.5f);
            SetRect(crosshair, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(8, 8));

            // Interaction Prompt (Center below crosshair)
            GameObject promptObj = CreateText(canvasObj.transform, "PromptText", "[E] Interact", 24, TextAnchor.UpperCenter, defaultFont);
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

            // Mobile Controls
            GameObject lookPanel = new GameObject("TouchLookPanel", typeof(RectTransform));
            lookPanel.transform.SetParent(canvasObj.transform);
            Image lookImg = lookPanel.AddComponent<Image>();
            lookImg.color = Color.clear;
            SetRect(lookPanel, new Vector2(0.4f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            lookPanel.AddComponent<HorrorEscape.TouchLookPanel>();

            GameObject grabBtn = CreateButton(canvasObj.transform, "Btn_Grab", "ACTION", defaultFont, new Vector2(1, 0), new Vector2(-100, 190), new Vector2(110, 80));
            grabBtn.GetComponent<Button>().onClick.AddListener(() => inter.PerformPrimaryInteraction());

            GameObject dropBtn = CreateButton(canvasObj.transform, "Btn_Drop", "DROP", defaultFont, new Vector2(1, 0), new Vector2(-225, 190), new Vector2(110, 80));
            dropBtn.GetComponent<Button>().onClick.AddListener(() => inter.DropCurrentItem());

            GameObject scanBtn = CreateButton(canvasObj.transform, "Btn_Scan", "SCAN", defaultFont, new Vector2(1, 0), new Vector2(-100, 290), new Vector2(110, 70));
            scanBtn.GetComponent<Button>().onClick.AddListener(() => inter.TriggerScan());

            // --- Day Summary Modal Panel ---
            GameObject summaryPanel = new GameObject("SummaryPanel", typeof(RectTransform));
            summaryPanel.transform.SetParent(canvasObj.transform);
            Image sumBg = summaryPanel.AddComponent<Image>();
            sumBg.color = new Color(0.05f, 0.07f, 0.1f, 0.95f);
            SetRect(summaryPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 750));

            GameObject titleObj = CreateText(summaryPanel.transform, "Title", "DAY SUMMARY", 36, TextAnchor.MiddleCenter, defaultFont);
            SetRect(titleObj, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(600, 60));
            titleObj.GetComponent<Text>().color = new Color(1f, 0.8f, 0.2f);

            GameObject detailsObj = CreateText(summaryPanel.transform, "Details", "Details here...", 22, TextAnchor.UpperLeft, defaultFont);
            SetRect(detailsObj, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(580, 460));
            detailsObj.GetComponent<Text>().color = Color.white;

            GameObject continueBtn = CreateButton(summaryPanel.transform, "Btn_Continue", "START NEXT DAY", defaultFont, new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(300, 70));
            continueBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.3f);

            // Bind Serialized Properties
            SerializedObject so = new SerializedObject(uiMgr);
            so.FindProperty("weightText").objectReferenceValue = weightObj.GetComponent<Text>();
            so.FindProperty("promptText").objectReferenceValue = promptObj.GetComponent<Text>();
            so.FindProperty("timeText").objectReferenceValue = timeObj.GetComponent<Text>();
            so.FindProperty("dayText").objectReferenceValue = dayObj.GetComponent<Text>();
            so.FindProperty("quotaText").objectReferenceValue = quotaObj.GetComponent<Text>();
            so.FindProperty("cargoText").objectReferenceValue = cargoObj.GetComponent<Text>();
            so.FindProperty("scanContainer").objectReferenceValue = scanContainer.transform;

            so.FindProperty("summaryPanel").objectReferenceValue = summaryPanel;
            so.FindProperty("summaryTitleText").objectReferenceValue = titleObj.GetComponent<Text>();
            so.FindProperty("summaryDetailsText").objectReferenceValue = detailsObj.GetComponent<Text>();
            so.FindProperty("continueButton").objectReferenceValue = continueBtn.GetComponent<Button>();

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
