#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class ReleaseSetupUtility
    {
        [MenuItem("Block Blast/Configure Release PlayerSettings & Icon")]
        public static void ConfigurePlayerSettings()
        {
            // 1. Basic Metadata
            PlayerSettings.productName = "Mallang Blast";
            PlayerSettings.companyName = "MallangGames";
            PlayerSettings.bundleVersion = "1.2.0";

            // 2. Standalone Resolution Settings (Optimized 720x1280 Windowed)
            PlayerSettings.defaultScreenWidth = 720;
            PlayerSettings.defaultScreenHeight = 1280;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;

            // 2.1 Set Input Handling to Both (New Input System & Legacy Input)
            var pSettingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (pSettingsAsset != null && pSettingsAsset.Length > 0)
            {
                SerializedObject playerSettings = new SerializedObject(pSettingsAsset[0]);
                SerializedProperty activeInputHandler = playerSettings.FindProperty("activeInputHandler");
                if (activeInputHandler != null && activeInputHandler.intValue != 2)
                {
                    activeInputHandler.intValue = 2; // Both
                    playerSettings.ApplyModifiedProperties();
                    Debug.Log("<color=green>[ReleaseSetup] activeInputHandler set to Both (2).</color>");
                }
            }

            // 3. App Icon Setup (Pink Mascot)
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
                Debug.Log("<color=green><b>[ReleaseSetup] Successfully configured Pink Mascot as standalone App Icon!</b></color>");
            }
            else
            {
                Debug.LogWarning("[ReleaseSetup] Could not load icon texture from: " + iconPath);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green><b>[ReleaseSetup] Successfully configured PlayerSettings for Mallang Blast v1.2.0!</b></color>");
        }
    }
}
#endif
