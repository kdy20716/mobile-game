#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class HighResTextureOptimizer
    {
        [MenuItem("Block Blast/Optimize Texture Resolution & Quality (2048 Lossless)", priority = 50)]
        public static void OptimizeAllMascotTextures()
        {
            string[] texturePaths = new string[]
            {
                "Assets/Textures/BlockBlastCute/Block_Pink_Mascot.png",
                "Assets/Textures/BlockBlastCute/Block_Mint_Mascot.png",
                "Assets/Textures/BlockBlastCute/Block_Gold_Mascot.png",
                "Assets/Textures/BlockBlastCute/Block_Purple_Mascot.png",
                "Assets/Textures/BlockBlastCute/Block_Blue_Mascot.png",
                "Assets/Textures/BlockBlastCute/Block_Star_Bomb.png",
                "Assets/Textures/BlockBlastCute/Jelly_Tile_Base.png",
                "Assets/Textures/BlockBlastCute/Jelly_MainMenu_Wide_BG.png",
                "Assets/Textures/MallangGames_Studio_Logo.png"
            };

            int updatedCount = 0;
            foreach (string path in texturePaths)
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    Debug.LogWarning($"[TextureOptimizer] Importer not found at {path}");
                    continue;
                }

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false; // No mipmap blur for UI sprites
                importer.filterMode = FilterMode.Bilinear;
                importer.anisoLevel = 16;
                importer.maxTextureSize = 2048;
                importer.alphaIsTransparency = true;
                importer.isReadable = true;

                // Default Platform Settings -> Uncompressed
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                defaultSettings.maxTextureSize = 2048;
                defaultSettings.textureCompression = TextureImporterCompression.Uncompressed;
                defaultSettings.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(defaultSettings);

                // Standalone Platform Settings -> Uncompressed RGBA32
                TextureImporterPlatformSettings standaloneSettings = importer.GetPlatformTextureSettings("Standalone");
                standaloneSettings.overridden = true;
                standaloneSettings.maxTextureSize = 2048;
                standaloneSettings.textureCompression = TextureImporterCompression.Uncompressed;
                standaloneSettings.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(standaloneSettings);

                importer.SaveAndReimport();
                updatedCount++;
                Debug.Log($"<color=green>[TextureOptimizer] Optimized {path} -> 2048px, RGBA32 Lossless, MipMaps OFF, Aniso 16.</color>");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<color=cyan><b>[TextureOptimizer] Finished optimizing {updatedCount} textures to Lossless High Quality!</b></color>");
        }
    }
}
#endif
