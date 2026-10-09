using UnityEngine;
using UnityEditor;
using TMPro;

namespace BlockBlast.Editor
{
    public static class FontUpgradeTool
    {
        [MenuItem("Tools/Inspect Font Details")]
        public static void InspectFont()
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Jua-Regular SDF.asset");
            if (font != null)
            {
                Debug.Log($"[FontUpgradeTool] Font: {font.name}, ptSize: {font.faceInfo.pointSize}, atlasW: {font.atlasWidth}, atlasH: {font.atlasHeight}, pad: {font.atlasPadding}, texCount: {font.atlasTextures?.Length}, shader: {font.material?.shader?.name}");
                if (font.material != null)
                {
                    Debug.Log($"[FontUpgradeTool] Material keywords: {string.Join(", ", font.material.shaderKeywords)}");
                    if (font.material.HasProperty("_Sharpness")) Debug.Log($"[FontUpgradeTool] _Sharpness: {font.material.GetFloat("_Sharpness")}");
                    if (font.material.HasProperty("_FaceDilate")) Debug.Log($"[FontUpgradeTool] _FaceDilate: {font.material.GetFloat("_FaceDilate")}");
                    if (font.material.HasProperty("_OutlineWidth")) Debug.Log($"[FontUpgradeTool] _OutlineWidth: {font.material.GetFloat("_OutlineWidth")}");
                    if (font.material.HasProperty("_OutlineSoftness")) Debug.Log($"[FontUpgradeTool] _OutlineSoftness: {font.material.GetFloat("_OutlineSoftness")}");
                }
                if (font.atlasTextures != null)
                {
                    for (int i = 0; i < font.atlasTextures.Length; i++)
                    {
                        var t = font.atlasTextures[i];
                        if (t != null)
                        {
                            Debug.Log($"[FontUpgradeTool] Tex[{i}]: {t.width}x{t.height}, format: {t.format}, filter: {t.filterMode}, aniso: {t.anisoLevel}, mipmaps: {t.mipmapCount}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("[FontUpgradeTool] Jua-Regular SDF.asset not found!");
            }
        }

        [MenuItem("Tools/Rebuild High-Res Jua Font (8K Ultra)")]
        public static void RebuildHighResFont()
        {
            string assetPath = "Assets/Fonts/Jua-Regular SDF.asset";
            string sourceFontPath = "Assets/Fonts/Jua-Regular.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null)
            {
                Debug.LogError("[FontUpgradeTool] Jua-Regular.ttf not found!");
                return;
            }

            // Delete existing asset
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            // Create Ultra High-Resolution 8192x8192 (or 4096 fallback) SDF Font Asset with multi-atlas support
            int targetAtlasSize = 8192;
            if (SystemInfo.maxTextureSize < 8192) targetAtlasSize = 4096;

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                samplingPointSize: 135,
                atlasPadding: 14,
                renderMode: UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
                atlasWidth: targetAtlasSize,
                atlasHeight: targetAtlasSize,
                atlasPopulationMode: AtlasPopulationMode.Dynamic,
                enableMultiAtlasSupport: true
            );

            if (fontAsset == null)
            {
                Debug.LogError("[FontUpgradeTool] Failed to create font asset at 8K, trying 4K...");
                fontAsset = TMP_FontAsset.CreateFontAsset(
                    sourceFont,
                    samplingPointSize: 110,
                    atlasPadding: 12,
                    renderMode: UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
                    atlasWidth: 4096,
                    atlasHeight: 4096,
                    atlasPopulationMode: AtlasPopulationMode.Dynamic,
                    enableMultiAtlasSupport: true
                );
            }

            if (fontAsset == null)
            {
                Debug.LogError("[FontUpgradeTool] Failed to create font asset!");
                return;
            }

            // Use crisp mobile distance field shader with slight dilate for maximum vector-like solid presence
            Shader dfShader = Shader.Find("TextMeshPro/Distance Field");
            if (dfShader != null && fontAsset.material != null)
            {
                fontAsset.material.shader = dfShader;
                fontAsset.material.SetFloat("_FaceDilate", 0.05f);
                fontAsset.material.SetFloat("_OutlineSoftness", 0f);
            }

            // Configure atlas textures for high filtering quality
            if (fontAsset.atlasTextures != null)
            {
                for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
                {
                    if (fontAsset.atlasTextures[i] != null)
                    {
                        fontAsset.atlasTextures[i].filterMode = FilterMode.Bilinear;
                        fontAsset.atlasTextures[i].anisoLevel = 16;
                    }
                }
            }

            AssetDatabase.CreateAsset(fontAsset, assetPath);
            if (fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
            }
            if (fontAsset.material != null)
            {
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            // Fallbacks
            CuteBlockTextureGenerator.EnsureFallbackFont(fontAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=#00FF88><b>[FontUpgradeTool]</b> Rebuilt 8K Ultra-Res Jua Font successfully! Atlas: {fontAsset.atlasWidth}x{fontAsset.atlasHeight}, ptSize: {fontAsset.faceInfo.pointSize}, shader: {fontAsset.material?.shader?.name}</color>");
        }
    }
}
