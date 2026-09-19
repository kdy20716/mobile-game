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

            // 1. Generate & Load Procedural Textures
            Texture2D texDiamond = LethalTextureGenerator.GetOrCreateTexture("Tex_DiamondPlate.png", () => LethalTextureGenerator.GenerateDiamondPlate(512));
            Texture2D texRusty = LethalTextureGenerator.GetOrCreateTexture("Tex_RustyMetal.png", () => LethalTextureGenerator.GenerateRustyMetal(512));
            Texture2D texHazard = LethalTextureGenerator.GetOrCreateTexture("Tex_HazardStripe.png", () => LethalTextureGenerator.GenerateHazardStripe(512));
            Texture2D texConcrete = LethalTextureGenerator.GetOrCreateTexture("Tex_GrungyConcrete.png", () => LethalTextureGenerator.GenerateGrungyConcrete(512));
            Texture2D texTerrain = LethalTextureGenerator.GetOrCreateTexture("Tex_AlienTerrain.png", () => LethalTextureGenerator.GenerateAlienTerrain(512));
            Texture2D texCeiling = LethalTextureGenerator.GetOrCreateTexture("Tex_CeilingGrid.png", () => LethalTextureGenerator.GenerateCeilingGrid(512));

            // 2. Materials with Textures & Tiling
            Material shipWallMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_ShipWall.mat", texRusty, new Vector2(4f, 2f), new Color(0.35f, 0.38f, 0.42f));
            Material shipFloorMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_ShipFloor.mat", texDiamond, new Vector2(8f, 10f), new Color(0.45f, 0.45f, 0.48f));
            Material cargoMarkerMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_CargoMarker.mat", texHazard, new Vector2(4f, 4f), Color.white);

            Material terrainMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_PlanetSurface.mat", texTerrain, new Vector2(25f, 25f), new Color(0.35f, 0.32f, 0.30f));
            Material cliffMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_CanyonCliff.mat", texTerrain, new Vector2(10f, 6f), new Color(0.25f, 0.22f, 0.20f));

            Material facilityWallMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_FacilityWall.mat", texRusty, new Vector2(6f, 3f), new Color(0.32f, 0.34f, 0.36f));
            Material facilityFloorMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_FacilityFloor.mat", texConcrete, new Vector2(16f, 16f), new Color(0.38f, 0.39f, 0.40f));
            Material facilityCeilingMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_FacilityCeiling.mat", texCeiling, new Vector2(12f, 12f), new Color(0.22f, 0.23f, 0.25f));
            Material catwalkMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_Catwalk.mat", texDiamond, new Vector2(4f, 12f), new Color(0.5f, 0.5f, 0.52f));

            Material metalScrapMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_MetalScrap.mat", texRusty, new Vector2(2f, 2f), new Color(0.65f, 0.60f, 0.50f));
            Material engineMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_HeavyEngine.mat", texRusty, new Vector2(1f, 1f), new Color(0.25f, 0.26f, 0.28f));
            Material goldMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_BrassBell.mat", texDiamond, new Vector2(1f, 1f), new Color(0.95f, 0.80f, 0.20f));
            Material doorMat = CreateMaterialWithTexture("Assets/Materials/Lethal/M_HeavyDoor.mat", texHazard, new Vector2(1f, 2f), new Color(0.85f, 0.25f, 0.20f));

            // Pitch Black Horror Atmosphere & Heavy Fog
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.008f, 0.008f, 0.012f); // Pitch black
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.02f, 0.025f, 0.035f);
            RenderSettings.fogDensity = 0.025f;

            // 3. Build Large Ship at (0, 0, 0)
            GameObject shipRoot = new GameObject("Ship_LandingCraft");
            Transform playerSpawn = BuildSpaciousShip(shipRoot.transform, shipWallMat, shipFloorMat, cargoMarkerMat);

            // 4. Build Expansive Planet Exterior (200m x 200m Valley with Cliffs)
            GameObject exteriorRoot = new GameObject("Planet_Exterior");
            BuildVastPlanetExterior(exteriorRoot.transform, terrainMat, cliffMat, facilityWallMat);

            // 5. Build Facility Exterior Entrance at (0, 0, 85)
            GameObject exteriorDoorObj = CreateDoorObject(new Vector3(0f, 1.6f, 85f), Quaternion.identity, "Door_FacilityEntrance", doorMat);
            var exteriorDoor = exteriorDoorObj.AddComponent<FacilityDoor>();

            // 6. Build Giant Facility Interior (Multi-Room + 2nd Floor Catwalk) at (200, 0, 200)
            Vector3 facilityOrigin = new Vector3(200f, 0f, 200f);
            GameObject facilityRoot = new GameObject("Facility_Interior");
            facilityRoot.transform.position = facilityOrigin;
            BuildGiantFacilityInterior(facilityRoot.transform, facilityFloorMat, facilityWallMat, facilityCeilingMat, catwalkMat);

            // Facility Interior Exit Door
            GameObject interiorDoorObj = CreateDoorObject(facilityOrigin + new Vector3(0f, 1.6f, -17.5f), Quaternion.Euler(0f, 180f, 0f), "Door_FacilityExit", doorMat);
            var interiorDoor = interiorDoorObj.AddComponent<FacilityDoor>();

            // Link doors
            exteriorDoor.SetDestination(interiorDoor.transform, false);
            interiorDoor.SetDestination(exteriorDoor.transform, true);

            // 7. Spawn Diverse Scraps inside Giant Facility
            SpawnAbundantScraps(facilityOrigin, metalScrapMat, engineMat, goldMat);

            // 8. Player Character Setup (Spawns in Ship)
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
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.01f, 0.012f, 0.015f);
            camObj.AddComponent<AudioListener>();

            // Hand Holder for Holding Scrap
            GameObject handHolder = new GameObject("RightHandHolder");
            handHolder.transform.SetParent(camObj.transform);
            handHolder.transform.localPosition = new Vector3(0.35f, -0.3f, 0.6f);

            playerCtrl.SetCamera(camObj.transform);
            inventory.SetHandHolder(handHolder.transform);
            interaction.SetCamera(camObj.transform);

            // Strong Atmospheric Headlight (Flashlight)
            GameObject headLight = new GameObject("Headlight");
            headLight.transform.SetParent(camObj.transform);
            headLight.transform.localPosition = Vector3.forward * 0.1f;
            Light hl = headLight.AddComponent<Light>();
            hl.type = LightType.Spot;
            hl.range = 32f;
            hl.spotAngle = 52f;
            hl.intensity = 2.4f;
            hl.color = new Color(1f, 0.96f, 0.88f);
            hl.shadows = LightShadows.Hard;

            // 9. Game Managers
            GameObject gmObj = new GameObject("GameManager");
            var gameMgr = gmObj.AddComponent<LethalGameManager>();

            SerializedObject soGM = new SerializedObject(gameMgr);
            soGM.FindProperty("shipSpawnPoint").objectReferenceValue = playerSpawn;
            soGM.FindProperty("playerObj").objectReferenceValue = player;
            soGM.ApplyModifiedProperties();

            // 10. EventSystem
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

            // 11. UI Canvas
            BuildLethalUI(inventory, interaction);

            // 12. Save Scene
            string scenePath = "Assets/Scenes/LethalScrapScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Selection.activeGameObject = player;
            Debug.Log("<color=green><b>[Lethal Company]</b> 리썰 컴퍼니 스케일 확장 및 절차적 텍스처/완전 밀폐 천장 씬이 완성되었습니다! (Assets/Scenes/LethalScrapScene.unity)</color>");
        }

        private static Transform BuildSpaciousShip(Transform parent, Material wallMat, Material floorMat, Material cargoMarkerMat)
        {
            // Spacious Ship: 12m Wide x 16m Long x 4.5m High
            // Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Ship_Floor";
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(0f, 0.1f, 0f);
            floor.transform.localScale = new Vector3(12f, 0.3f, 16f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            // Solid Ceiling (No light leaks!)
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ship_Ceiling";
            ceiling.transform.SetParent(parent);
            ceiling.transform.position = new Vector3(0f, 4.6f, 0f);
            ceiling.transform.localScale = new Vector3(12.4f, 0.4f, 16.4f);
            ceiling.GetComponent<MeshRenderer>().material = wallMat;

            // Outer Walls
            CreateWall(parent, new Vector3(-6f, 2.3f, 0f), new Vector3(0.4f, 4.4f, 16f), wallMat, "Ship_Wall_West");
            CreateWall(parent, new Vector3(6f, 2.3f, 0f), new Vector3(0.4f, 4.4f, 16f), wallMat, "Ship_Wall_East");
            CreateWall(parent, new Vector3(0f, 2.3f, -8f), new Vector3(12f, 4.4f, 0.4f), wallMat, "Ship_Wall_South");

            // Front Wall with Door Opening (North)
            CreateWall(parent, new Vector3(-4.5f, 2.3f, 8f), new Vector3(3f, 4.4f, 0.4f), wallMat, "Ship_Wall_NorthL");
            CreateWall(parent, new Vector3(4.5f, 2.3f, 8f), new Vector3(3f, 4.4f, 0.4f), wallMat, "Ship_Wall_NorthR");
            CreateWall(parent, new Vector3(0f, 3.8f, 8f), new Vector3(6f, 1.4f, 0.4f), wallMat, "Ship_Wall_NorthTop");

            // Wide Ramp leading outside
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ship_Ramp";
            ramp.transform.SetParent(parent);
            ramp.transform.position = new Vector3(0f, -0.05f, 10.5f);
            ramp.transform.rotation = Quaternion.Euler(7f, 0f, 0f);
            ramp.transform.localScale = new Vector3(5.5f, 0.25f, 5.5f);
            ramp.GetComponent<MeshRenderer>().material = floorMat;

            // Big Scrap Cargo Zone (Drop-off Zone) on West side of ship
            GameObject cargoZone = new GameObject("ScrapCargoZone");
            cargoZone.transform.SetParent(parent);
            cargoZone.transform.position = new Vector3(-2.8f, 1.0f, -2.5f);
            BoxCollider cargoCol = cargoZone.AddComponent<BoxCollider>();
            cargoCol.isTrigger = true;
            cargoCol.size = new Vector3(5.5f, 2.0f, 8.0f);

            cargoZone.AddComponent<LethalShipManager>();

            // Hazard Stripe Floor Marker
            GameObject cargoMark = GameObject.CreatePrimitive(PrimitiveType.Quad);
            cargoMark.name = "CargoFloorMarker";
            cargoMark.transform.SetParent(cargoZone.transform);
            cargoMark.transform.localPosition = new Vector3(0f, -0.83f, 0f);
            cargoMark.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            cargoMark.transform.localScale = new Vector3(5.2f, 7.6f, 1f);
            cargoMark.GetComponent<MeshRenderer>().material = cargoMarkerMat;
            Object.DestroyImmediate(cargoMark.GetComponent<Collider>());

            // Interior Storage Shelves (Metal Racks)
            CreateShelfRack(parent, new Vector3(-5.2f, 1.2f, 3.5f), wallMat);
            CreateShelfRack(parent, new Vector3(5.2f, 1.2f, -3.5f), wallMat);

            // Control Console & Take-off Lever on East side
            GameObject leverConsole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leverConsole.name = "ShipControlConsole";
            leverConsole.transform.SetParent(parent);
            leverConsole.transform.position = new Vector3(4f, 1.0f, 4f);
            leverConsole.transform.localScale = new Vector3(1.2f, 1.6f, 1.2f);
            leverConsole.GetComponent<MeshRenderer>().material = wallMat;

            GameObject leverObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leverObj.name = "TakeoffLever";
            leverObj.transform.SetParent(leverConsole.transform);
            leverObj.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            leverObj.transform.localRotation = Quaternion.Euler(-45f, 0f, 0f);
            leverObj.transform.localScale = new Vector3(0.14f, 0.45f, 0.14f);
            leverObj.GetComponent<MeshRenderer>().material = CreateMaterialWithTexture("Assets/Materials/Lethal/M_LeverRed.mat", null, Vector2.one, Color.red);
            leverObj.AddComponent<ShipLever>();

            // Ship Interior Atmosphere Lighting (Warm Fluorescent Lamps)
            CreateShipLight(parent, new Vector3(-2f, 4.0f, -2f), new Color(1f, 0.85f, 0.7f), 1.6f);
            CreateShipLight(parent, new Vector3(2f, 4.0f, 3f), new Color(1f, 0.9f, 0.75f), 1.6f);

            // Player Spawn Transform
            GameObject spawnPoint = new GameObject("ShipPlayerSpawn");
            spawnPoint.transform.SetParent(parent);
            spawnPoint.transform.position = new Vector3(1.5f, 0.4f, -1.0f);
            spawnPoint.transform.rotation = Quaternion.identity;

            return spawnPoint.transform;
        }

        private static void BuildVastPlanetExterior(Transform parent, Material terrainMat, Material cliffMat, Material pipeMat)
        {
            // 200m x 200m Planet Ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "PlanetGround";
            ground.transform.SetParent(parent);
            ground.transform.position = new Vector3(0f, -0.05f, 50f);
            ground.transform.localScale = new Vector3(20f, 1f, 20f);
            ground.GetComponent<MeshRenderer>().material = terrainMat;

            // Massive Canyon Cliff Walls on East and West (15m high)
            CreateWall(parent, new Vector3(-35f, 7.5f, 50f), new Vector3(12f, 15f, 200f), cliffMat, "Canyon_West");
            CreateWall(parent, new Vector3(35f, 7.5f, 50f), new Vector3(12f, 15f, 200f), cliffMat, "Canyon_East");
            CreateWall(parent, new Vector3(0f, 7.5f, -30f), new Vector3(80f, 15f, 15f), cliffMat, "Canyon_South_BehindShip");
            CreateWall(parent, new Vector3(0f, 7.5f, 120f), new Vector3(80f, 15f, 20f), cliffMat, "Canyon_North_BehindFacility");

            // Industrial Street Lamps / Floodlights along the path to the facility
            CreateFloodlightPole(parent, new Vector3(-8f, 0f, 25f), pipeMat);
            CreateFloodlightPole(parent, new Vector3(8f, 0f, 55f), pipeMat);

            // Large Rocks and Terrain Props
            CreateRockCluster(parent, new Vector3(-14f, 1.5f, 20f), 3.5f, cliffMat);
            CreateRockCluster(parent, new Vector3(16f, 2.0f, 40f), 4.2f, cliffMat);
            CreateRockCluster(parent, new Vector3(-12f, 1.8f, 65f), 3.8f, cliffMat);
            CreateRockCluster(parent, new Vector3(10f, 1.2f, 75f), 3.0f, cliffMat);

            // Industrial Pipeline running along the valley
            GameObject pipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pipe.name = "IndustrialPipeline";
            pipe.transform.SetParent(parent);
            pipe.transform.position = new Vector3(-18f, 1.2f, 50f);
            pipe.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            pipe.transform.localScale = new Vector3(1.2f, 65f, 1.2f);
            pipe.GetComponent<MeshRenderer>().material = pipeMat;
        }

        private static void BuildGiantFacilityInterior(Transform parent, Material floorMat, Material wallMat, Material ceilingMat, Material catwalkMat)
        {
            Vector3 origin = parent.position;

            // ==========================================
            // 1. MAIN FACTORY HALL (36m x 32m x 7m)
            // ==========================================
            // Floor
            GameObject mainFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            mainFloor.name = "MainHall_Floor";
            mainFloor.transform.SetParent(parent);
            mainFloor.transform.position = origin + new Vector3(0f, 0f, 0f);
            mainFloor.transform.localScale = new Vector3(3.6f, 1f, 3.2f);
            mainFloor.GetComponent<MeshRenderer>().material = floorMat;

            // Complete Solid Ceiling (Pitch Black, No Skybox Leak!)
            GameObject mainCeiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mainCeiling.name = "MainHall_Ceiling";
            mainCeiling.transform.SetParent(parent);
            mainCeiling.transform.position = origin + new Vector3(0f, 6.8f, 0f);
            mainCeiling.transform.localScale = new Vector3(36.5f, 0.4f, 32.5f);
            mainCeiling.GetComponent<MeshRenderer>().material = ceilingMat;

            // Outer Walls of Main Hall
            CreateWall(parent, origin + new Vector3(0f, 3.4f, 16f), new Vector3(36f, 6.8f, 0.6f), wallMat, "Main_Wall_North");
            CreateWall(parent, origin + new Vector3(0f, 3.4f, -16f), new Vector3(36f, 6.8f, 0.6f), wallMat, "Main_Wall_South");
            CreateWall(parent, origin + new Vector3(18f, 3.4f, 0f), new Vector3(0.6f, 6.8f, 32f), wallMat, "Main_Wall_East");
            CreateWall(parent, origin + new Vector3(-18f, 3.4f, 0f), new Vector3(0.6f, 6.8f, 32f), wallMat, "Main_Wall_West");

            // 2nd Floor Catwalk System (Elevated Walkway: 3.2m high)
            GameObject catwalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            catwalk.name = "Catwalk_Main";
            catwalk.transform.SetParent(parent);
            catwalk.transform.position = origin + new Vector3(0f, 3.2f, 4f);
            catwalk.transform.localScale = new Vector3(28f, 0.25f, 3.5f);
            catwalk.GetComponent<MeshRenderer>().material = catwalkMat;

            // Catwalk Stairs
            GameObject stairs = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairs.name = "Catwalk_Stairs";
            stairs.transform.SetParent(parent);
            stairs.transform.position = origin + new Vector3(-11f, 1.6f, -1f);
            stairs.transform.rotation = Quaternion.Euler(-25f, 0f, 0f);
            stairs.transform.localScale = new Vector3(2.5f, 0.25f, 7.5f);
            stairs.GetComponent<MeshRenderer>().material = catwalkMat;

            // Heavy Industrial Machinery / Crates in Main Hall
            CreateCrate(parent, origin + new Vector3(-8f, 1.2f, -8f), new Vector3(3f, 2.4f, 3f), wallMat);
            CreateCrate(parent, origin + new Vector3(9f, 1.5f, -6f), new Vector3(3.5f, 3f, 3.5f), wallMat);
            CreateCrate(parent, origin + new Vector3(10f, 0.8f, 8f), new Vector3(2.5f, 1.6f, 4f), wallMat);

            // Spooky Atmospheric Lights (Flickering Industrial Lamps)
            CreateAtmosphericLight(parent, origin + new Vector3(-8f, 5.8f, 6f), new Color(0.9f, 0.8f, 0.6f), 1.5f, true);
            CreateAtmosphericLight(parent, origin + new Vector3(8f, 5.8f, -6f), new Color(0.7f, 0.85f, 1f), 1.2f, true);
            CreateAtmosphericLight(parent, origin + new Vector3(0f, 5.8f, -12f), new Color(1f, 0.3f, 0.2f), 1.4f, false); // Emergency red light near door

            // ==========================================
            // 2. STORAGE WING (East Wing: 24m x 20m x 5m)
            // ==========================================
            Vector3 storageOrigin = origin + new Vector3(30f, 0f, 0f);

            GameObject storFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            storFloor.name = "Storage_Floor";
            storFloor.transform.SetParent(parent);
            storFloor.transform.position = storageOrigin;
            storFloor.transform.localScale = new Vector3(2.4f, 1f, 2.0f);
            storFloor.GetComponent<MeshRenderer>().material = floorMat;

            GameObject storCeiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            storCeiling.name = "Storage_Ceiling";
            storCeiling.transform.SetParent(parent);
            storCeiling.transform.position = storageOrigin + new Vector3(0f, 5.0f, 0f);
            storCeiling.transform.localScale = new Vector3(24.5f, 0.4f, 20.5f);
            storCeiling.GetComponent<MeshRenderer>().material = ceilingMat;

            CreateWall(parent, storageOrigin + new Vector3(0f, 2.5f, 10f), new Vector3(24f, 5.0f, 0.6f), wallMat, "Stor_Wall_North");
            CreateWall(parent, storageOrigin + new Vector3(0f, 2.5f, -10f), new Vector3(24f, 5.0f, 0.6f), wallMat, "Stor_Wall_South");
            CreateWall(parent, storageOrigin + new Vector3(12f, 2.5f, 0f), new Vector3(0.6f, 5.0f, 20f), wallMat, "Stor_Wall_East");

            // Storage Shelves and Pillars
            CreateShelfRack(parent, storageOrigin + new Vector3(-4f, 1.5f, 4f), wallMat);
            CreateShelfRack(parent, storageOrigin + new Vector3(4f, 1.5f, 4f), wallMat);
            CreateShelfRack(parent, storageOrigin + new Vector3(-4f, 1.5f, -4f), wallMat);
            CreateShelfRack(parent, storageOrigin + new Vector3(4f, 1.5f, -4f), wallMat);

            CreateAtmosphericLight(parent, storageOrigin + new Vector3(0f, 4.4f, 0f), new Color(0.85f, 0.75f, 0.5f), 1.3f, true);

            // ==========================================
            // 3. DARK GENERATOR CORRIDOR (West Wing: 30m x 8m x 5m)
            // ==========================================
            Vector3 corrOrigin = origin + new Vector3(-33f, 0f, 0f);

            GameObject corrFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            corrFloor.name = "Corridor_Floor";
            corrFloor.transform.SetParent(parent);
            corrFloor.transform.position = corrOrigin;
            corrFloor.transform.localScale = new Vector3(3.0f, 1f, 0.8f);
            corrFloor.GetComponent<MeshRenderer>().material = floorMat;

            GameObject corrCeiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corrCeiling.name = "Corridor_Ceiling";
            corrCeiling.transform.SetParent(parent);
            corrCeiling.transform.position = corrOrigin + new Vector3(0f, 5.0f, 0f);
            corrCeiling.transform.localScale = new Vector3(30.5f, 0.4f, 8.5f);
            corrCeiling.GetComponent<MeshRenderer>().material = ceilingMat;

            CreateWall(parent, corrOrigin + new Vector3(0f, 2.5f, 4f), new Vector3(30f, 5.0f, 0.6f), wallMat, "Corr_Wall_North");
            CreateWall(parent, corrOrigin + new Vector3(0f, 2.5f, -4f), new Vector3(30f, 5.0f, 0.6f), wallMat, "Corr_Wall_South");
            CreateWall(parent, corrOrigin + new Vector3(-15f, 2.5f, 0f), new Vector3(0.6f, 5.0f, 8f), wallMat, "Corr_Wall_West");

            // Very Dim Flickering Red Lamp at end of corridor
            CreateAtmosphericLight(parent, corrOrigin + new Vector3(-10f, 4.2f, 0f), new Color(1f, 0.2f, 0.15f), 0.9f, true);
        }

        private static void CreateShelfRack(Transform parent, Vector3 pos, Material mat)
        {
            GameObject rack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rack.name = "StorageRack";
            rack.transform.SetParent(parent);
            rack.transform.position = pos;
            rack.transform.localScale = new Vector3(1.2f, 2.8f, 4.5f);
            rack.GetComponent<MeshRenderer>().material = mat;
        }

        private static void CreateFloodlightPole(Transform parent, Vector3 pos, Material mat)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "FloodlightPole";
            pole.transform.SetParent(parent);
            pole.transform.position = pos + Vector3.up * 4f;
            pole.transform.localScale = new Vector3(0.3f, 4f, 0.3f);
            pole.GetComponent<MeshRenderer>().material = mat;

            GameObject lightObj = new GameObject("Floodlight");
            lightObj.transform.SetParent(pole.transform);
            lightObj.transform.localPosition = new Vector3(0f, 1f, 0f);

            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Spot;
            l.range = 28f;
            l.spotAngle = 65f;
            l.intensity = 2.2f;
            l.color = new Color(1f, 0.92f, 0.8f);
        }

        private static void CreateAtmosphericLight(Transform parent, Vector3 pos, Color color, float intensity, bool flicker)
        {
            GameObject lObj = new GameObject("FacilityLamp");
            lObj.transform.SetParent(parent);
            lObj.transform.position = pos;

            Light l = lObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 16f;
            l.intensity = intensity;
            l.color = color;

            if (flicker)
            {
                lObj.AddComponent<FlickeringLight>();
            }
        }

        private static void CreateShipLight(Transform parent, Vector3 pos, Color color, float intensity)
        {
            GameObject lObj = new GameObject("ShipLamp");
            lObj.transform.SetParent(parent);
            lObj.transform.position = pos;

            Light l = lObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 12f;
            l.intensity = intensity;
            l.color = color;
        }

        private static void CreateRockCluster(Transform parent, Vector3 pos, float scale, Material mat)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "PlanetRock";
            rock.transform.SetParent(parent);
            rock.transform.position = pos;
            rock.transform.localScale = new Vector3(scale * 1.3f, scale * 0.9f, scale * 1.1f);
            rock.GetComponent<MeshRenderer>().material = mat;
        }

        private static GameObject CreateDoorObject(Vector3 pos, Quaternion rot, string name, Material mat)
        {
            GameObject doorFrame = new GameObject(name);
            doorFrame.transform.position = pos;
            doorFrame.transform.rotation = rot;

            CreateWall(doorFrame.transform, new Vector3(-1.3f, 0f, 0f), new Vector3(0.4f, 3.2f, 0.6f), mat, "Post_L");
            CreateWall(doorFrame.transform, new Vector3(1.3f, 0f, 0f), new Vector3(0.4f, 3.2f, 0.6f), mat, "Post_R");
            CreateWall(doorFrame.transform, new Vector3(0f, 1.6f, 0f), new Vector3(3.0f, 0.4f, 0.6f), mat, "Beam_Top");

            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "Door_Panel";
            panel.transform.SetParent(doorFrame.transform);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = new Vector3(2.2f, 3.0f, 0.25f);
            panel.GetComponent<MeshRenderer>().material = mat;

            GameObject lightObj = new GameObject("DoorLight");
            lightObj.transform.SetParent(doorFrame.transform);
            lightObj.transform.localPosition = new Vector3(0f, 1.8f, 0.5f);
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 8f;
            l.intensity = 2.5f;
            l.color = new Color(1f, 0.3f, 0.2f);

            return doorFrame;
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

        private static void SpawnAbundantScraps(Vector3 origin, Material metalMat, Material engineMat, Material goldMat)
        {
            // Main Hall Scraps
            CreateEngineScrap(origin + new Vector3(-8f, 2.8f, -8f), engineMat, "V-Type Engine", 105, 45f);
            CreateAxleScrap(origin + new Vector3(-4f, 0.3f, 8f), metalMat, "Metal Axle", 58, 24f);
            CreateBoltScrap(origin + new Vector3(2f, 0.3f, 4f), metalMat, "Big Bolt", 34, 6f);
            CreateBoltScrap(origin + new Vector3(5f, 0.3f, 9f), metalMat, "Big Bolt", 28, 5f);
            CreateBellScrap(origin + new Vector3(10f, 1.8f, 8f), goldMat, "Brass Bell", 72, 18f);

            // On the Catwalk
            CreateCupScrap(origin + new Vector3(2f, 3.6f, 4f), goldMat, "Golden Goblet", 120, 4f);
            CreateBoltScrap(origin + new Vector3(-6f, 3.4f, 4f), metalMat, "Big Bolt", 32, 6f);

            // Storage Wing Scraps
            Vector3 stor = origin + new Vector3(30f, 0f, 0f);
            CreateGoldBarScrap(stor + new Vector3(-4f, 1.8f, 4f), goldMat, "Gold Bar", 160, 20f);
            CreateCupScrap(stor + new Vector3(4f, 1.8f, -4f), goldMat, "Golden Goblet", 115, 4f);
            CreateBellScrap(stor + new Vector3(0f, 0.4f, 6f), goldMat, "Brass Bell", 68, 18f);
            CreateBoltScrap(stor + new Vector3(6f, 0.3f, 0f), metalMat, "Big Bolt", 30, 5f);

            // Corridor Scraps (Deep Dark)
            Vector3 corr = origin + new Vector3(-33f, 0f, 0f);
            CreateEngineScrap(corr + new Vector3(-12f, 0.6f, 0f), engineMat, "V-Type Engine", 110, 48f);
            CreateAxleScrap(corr + new Vector3(-6f, 0.3f, 1f), metalMat, "Metal Axle", 52, 22f);
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

        private static Material CreateMaterialWithTexture(string path, Texture2D texture, Vector2 tiling, Color tint)
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
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.color = tint;
            if (texture != null)
            {
                mat.mainTexture = texture;
                mat.mainTextureScale = tiling;
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
#endif
