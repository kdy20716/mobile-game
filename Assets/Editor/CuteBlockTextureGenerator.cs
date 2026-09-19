#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class CuteBlockTextureGenerator
    {
        private const string Folder = "Assets/Textures/BlockBlastCute";

        public static Sprite GetOrCreateCuteJellySprite(string name, Color baseColor, bool isStarBomb = false)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateJellyTexture(256, baseColor, isStarBomb);
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 100;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateCuteIcon(string name, System.Action<Texture2D> painter)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
                // Clear transparent
                Color[] clear = new Color[256 * 256];
                for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
                tex.SetPixels(clear);

                painter(tex);
                tex.Apply();

                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 100;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Textures")) AssetDatabase.CreateFolder("Assets", "Textures");
            if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets/Textures", "BlockBlastCute");
        }

        // Generates an adorable glossy squircle jelly tile
        private static Texture2D GenerateJellyTexture(int size, Color baseCol, bool isBomb)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float radius = size * 0.28f; // Rounded squircle radius
            float center = size * 0.5f;

            Color darkRim = Color.Lerp(baseCol, Color.black, 0.22f);
            Color brightLight = Color.Lerp(baseCol, Color.white, 0.65f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Signed distance to rounded rect
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center) - (center - radius - 4f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - center) - (center - radius - 4f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear); // Outside
                    }
                    else
                    {
                        float edgeFactor = Mathf.Clamp01((radius - dist) / 6f); // Antialiasing edge
                        float normY = (float)y / size;
                        float normX = (float)x / size;

                        // Base Jelly Gradient
                        Color c = Color.Lerp(darkRim, baseCol, Mathf.SmoothStep(0.1f, 0.9f, normY));

                        // Top-left glossy bubble highlight
                        float hlDist = Mathf.Sqrt(Mathf.Pow(x - size * 0.38f, 2) + Mathf.Pow(y - size * 0.72f, 2));
                        if (hlDist < size * 0.28f)
                        {
                            float hl = Mathf.SmoothStep(size * 0.28f, size * 0.05f, hlDist) * 0.55f;
                            c = Color.Lerp(c, Color.white, hl);
                        }

                        // Bottom rim ambient glow
                        float bottomGlow = Mathf.SmoothStep(0.25f, 0.05f, normY) * 0.35f;
                        c = Color.Lerp(c, brightLight, bottomGlow);

                        c.a = edgeFactor;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // Draw cute star on bomb blocks
            if (isBomb)
            {
                DrawCuteStar(tex, center, center, size * 0.25f, Color.white);
            }

            tex.Apply();
            return tex;
        }

        private static void DrawCuteStar(Texture2D tex, float cx, float cy, float r, Color starCol)
        {
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > r * 1.5f) continue;

                    float angle = Mathf.Atan2(dy, dx);
                    // 5-point star equation
                    float arm = Mathf.Cos(5f * angle);
                    float starR = r * (0.6f + 0.4f * arm);

                    if (dist < starR)
                    {
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, starCol, 0.9f));
                    }
                }
            }
        }
    }
}
#endif
