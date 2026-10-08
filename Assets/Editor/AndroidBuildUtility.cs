#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class AndroidBuildUtility
    {
        public const string ScenePath = "Assets/Scenes/BlockBlastScene.unity";
        public const string OutputDir = "Builds/Android";
        public const string AabFileName = "MallangBlast_v1.2.0.aab";
        public const string PackageName = "com.MallangGames.MallangBlast";

        [MenuItem("Block Blast/Android/1. Configure Google Play Settings", priority = 150)]
        public static void ConfigureGooglePlaySettings()
        {
            // 1. App Identification
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PackageName);

            // 2. Google Play Target SDK (API Level 34 - Android 14) & Min SDK (API 26 - Android 8.0)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;

            // 3. 64-bit Compliance (IL2CPP + ARM64 & ARMv7)
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // 4. Android App Bundle (.aab) format for Google Play
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;

            // 5. Screen Orientation: Lock to Portrait
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            // 6. Disable Unity Splash Screen
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // 7. Icon Setup
            string iconPath = "Assets/Textures/AppIcon.png";
            string pinkMascotPath = "Assets/Textures/BlockBlastCute/Block_Pink_Mascot.png";
            if (!File.Exists(iconPath) && File.Exists(pinkMascotPath))
            {
                File.Copy(pinkMascotPath, iconPath, true);
                AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
            }
            Texture2D iconTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (iconTex != null)
            {
                Texture2D[] icons = new Texture2D[] { iconTex };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
            }

            // 8. Build Scenes (BlockBlastScene.unity as Scene #0)
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[AndroidBuild] Google Play release settings configured successfully! (Package: " + PackageName + ", Target SDK 34, IL2CPP ARM64+ARMv7, AAB=True, Portrait)</b></color>");
        }

        [MenuItem("Block Blast/Android/2. Switch Active Platform to Android", priority = 151)]
        public static void SwitchPlatformToAndroid()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                Debug.Log("<color=cyan>[AndroidBuild] Active build target is already Android.</color>");
                return;
            }

            Debug.Log("<color=yellow>[AndroidBuild] Switching active build target to Android...</color>");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        [MenuItem("Block Blast/Android/3. Build Release Android App Bundle (.aab)", priority = 152)]
        public static void BuildReleaseAab()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            ConfigureGooglePlaySettings();

            string fullOutputDir = Path.GetFullPath(OutputDir);
            if (!Directory.Exists(fullOutputDir))
            {
                Directory.CreateDirectory(fullOutputDir);
            }

            string aabFullPath = Path.Combine(fullOutputDir, AabFileName);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = aabFullPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };

            Debug.Log($"<color=cyan>[AndroidBuild] Starting Google Play Release AAB Build -> {aabFullPath}</color>");
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=green><b>[AndroidBuild] AAB Build SUCCESS!</b></color>\nSize: {summary.totalSize / (1024f * 1024f):F2} MB\nPath: {aabFullPath}");
                EditorUtility.RevealInFinder(aabFullPath);
            }
            else
            {
                Debug.LogError($"<color=red><b>[AndroidBuild] Build Failed!</b> Result: {summary.result}, Errors: {summary.totalErrors}</color>");
            }
        }
    }
}
#endif
