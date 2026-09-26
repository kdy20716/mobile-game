#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class BlockBlastBuildUtility
    {
        public const string ScenePath = "Assets/Scenes/BlockBlastScene.unity";
        public const string BuildOutputDir = "Builds/MallangBlast_Steam_x64";
        public const string ExeName = "MallangBlast.exe";

        [MenuItem("Block Blast/Build/1. Apply Steam & 9-16 & No Splash Settings", priority = 100)]
        public static void ConfigureAllSettings()
        {
            // 1. Basic Metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // 2. 9:16 Resolution & Window Mode (Steam Default)
            PlayerSettings.defaultScreenWidth = 720;
            PlayerSettings.defaultScreenHeight = 1280;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.visibleInBackground = true;

            // 3. Disable Unity Splash Screen
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // 4. Input System handling (Both: New + Legacy)
            var pSettingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (pSettingsAsset != null && pSettingsAsset.Length > 0)
            {
                SerializedObject playerSettings = new SerializedObject(pSettingsAsset[0]);
                SerializedProperty activeInputHandler = playerSettings.FindProperty("activeInputHandler");
                if (activeInputHandler != null && activeInputHandler.intValue != 2)
                {
                    activeInputHandler.intValue = 2; // Both
                    playerSettings.ApplyModifiedProperties();
                }
            }

            // 5. App Icon Setup (Pink Mascot)
            string iconPath = "Assets/Textures/BlockBlastCute_Backup/Block_Pink_Mascot.png";
            TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.textureType = TextureImporterType.Default;
                importer.SaveAndReimport();
            }

            Texture2D iconTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (iconTex != null)
            {
                Texture2D[] icons = new Texture2D[] { iconTex };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, icons);
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
            }

            // 6. Set Build Scenes (BlockBlastScene as Scene #0)
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[BlockBlastBuild] Successfully applied Steam release settings (9:16 720x1280, No Splash, BlockBlastScene #0, Icon)!</b></color>");
        }

        [MenuItem("Block Blast/Build/2. Build Windows 64-bit for Steam", priority = 101)]
        public static void BuildWindows64()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureAllSettings();

            string fullOutputDir = Path.GetFullPath(BuildOutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            string exeFullPath = Path.Combine(fullOutputDir, ExeName);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = exeFullPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[BlockBlastBuild] Windows 64-bit Steam 빌드 시작: {exeFullPath}</color>");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                // Copy steam_appid.txt to output folder so direct execution works seamlessly
                string rootAppId = Path.GetFullPath("steam_appid.txt");
                string destAppId = Path.Combine(fullOutputDir, "steam_appid.txt");
                if (File.Exists(rootAppId))
                {
                    File.Copy(rootAppId, destAppId, true);
                    Debug.Log($"<color=cyan>[BlockBlastBuild] Copied steam_appid.txt to: {destAppId}</color>");
                }

                Debug.Log($"<color=green><b>[BlockBlastBuild] ★★★ 빌드 성공! ★★★\n경로: {exeFullPath}\n크기: {summary.totalSize / (1024 * 1024):N1} MB\n소요시간: {summary.totalTime.TotalSeconds:F1}초</b></color>");
                EditorUtility.RevealInFinder(exeFullPath);
            }
            else
            {
                Debug.LogError($"<color=red>[BlockBlastBuild] 빌드 실패! 결과: {summary.result}, 에러 수: {summary.totalErrors}</color>");
            }
        }

        [MenuItem("Block Blast/Build/3. Open Build Folder", priority = 102)]
        public static void OpenBuildFolder()
        {
            string fullOutputDir = Path.GetFullPath(BuildOutputDir);
            if (Directory.Exists(fullOutputDir))
            {
                EditorUtility.RevealInFinder(fullOutputDir);
            }
            else
            {
                Debug.LogWarning("[BlockBlastBuild] Build folder does not exist yet. Please run Build first.");
            }
        }
    }
}
#endif
