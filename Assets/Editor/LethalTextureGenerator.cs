#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LethalCompany.Editor
{
    public static class LethalTextureGenerator
    {
        private const string TextureFolder = "Assets/Textures/Lethal";

        public static Texture2D GetOrCreateTexture(string fileName, System.Func<Texture2D> generator)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Textures"))
            {
                AssetDatabase.CreateFolder("Assets", "Textures");
            }
            if (!AssetDatabase.IsValidFolder(TextureFolder))
            {
                AssetDatabase.CreateFolder("Assets/Textures", "Lethal");
            }

            string fullPath = Path.Combine(TextureFolder, fileName);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
            if (tex != null) return tex;

            tex = generator();
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(fullPath, bytes);
            AssetDatabase.ImportAsset(fullPath);

            // Configure importer for tiling
            TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            if (importer != null)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
        }

        public static Texture2D GenerateDiamondPlate(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color baseColor = new Color(0.22f, 0.23f, 0.26f);
            Color darkShadow = new Color(0.12f, 0.13f, 0.15f);
            Color highlight = new Color(0.42f, 0.44f, 0.48f);

            int tileSize = size / 8;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int tx = x % tileSize;
                    int ty = y % tileSize;
                    int cellX = x / tileSize;
                    int cellY = y / tileSize;

                    bool alternate = (cellX + cellY) % 2 == 0;

                    // Diamond pattern
                    float dist;
                    if (alternate)
                    {
                        dist = Mathf.Abs((tx - tileSize * 0.5f) * 1.5f) + Mathf.Abs(ty - tileSize * 0.5f);
                    }
                    else
                    {
                        dist = Mathf.Abs(tx - tileSize * 0.5f) + Mathf.Abs((ty - tileSize * 0.5f) * 1.5f);
                    }

                    Color c = baseColor;
                    if (dist < tileSize * 0.35f)
                    {
                        c = (tx > ty) ? highlight : darkShadow;
                    }

                    // Add subtle noise
                    float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f) * 0.08f - 0.04f;
                    c.r = Mathf.Clamp01(c.r + noise);
                    c.g = Mathf.Clamp01(c.g + noise);
                    c.b = Mathf.Clamp01(c.b + noise);

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateRustyMetal(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color metalColor = new Color(0.28f, 0.30f, 0.33f);
            Color rustColor1 = new Color(0.45f, 0.22f, 0.10f);
            Color rustColor2 = new Color(0.62f, 0.30f, 0.12f);
            Color rivetColor = new Color(0.12f, 0.12f, 0.14f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Multi-scale noise for rust patches
                    float n1 = Mathf.PerlinNoise(x * 0.012f, y * 0.012f);
                    float n2 = Mathf.PerlinNoise(x * 0.045f, y * 0.045f) * 0.5f;
                    float n3 = Mathf.PerlinNoise(x * 0.12f, y * 0.12f) * 0.25f;
                    float combined = n1 + n2 + n3;

                    Color c = metalColor;
                    if (combined > 0.95f)
                    {
                        float t = (combined - 0.95f) / 0.5f;
                        c = Color.Lerp(rustColor1, rustColor2, t);
                    }
                    else if (combined > 0.85f)
                    {
                        float t = (combined - 0.85f) / 0.1f;
                        c = Color.Lerp(metalColor, rustColor1, t);
                    }

                    // Rivets around borders (every 64 pixels)
                    int borderDistX = Mathf.Min(x % 128, 128 - (x % 128));
                    int borderDistY = Mathf.Min(y % 128, 128 - (y % 128));
                    if ((borderDistX < 8 && Mathf.Abs((y % 32) - 16) < 4) ||
                        (borderDistY < 8 && Mathf.Abs((x % 32) - 16) < 4))
                    {
                        c = rivetColor;
                    }

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateHazardStripe(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color yellow = new Color(0.92f, 0.72f, 0.05f);
            Color black = new Color(0.12f, 0.12f, 0.14f);

            int stripeWidth = size / 8;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int pos = (x + y) % (stripeWidth * 2);
                    Color c = pos < stripeWidth ? yellow : black;

                    // Grunge / dirt noise
                    float dirt = Mathf.PerlinNoise(x * 0.03f, y * 0.03f);
                    if (dirt < 0.35f)
                    {
                        c = Color.Lerp(c, new Color(0.15f, 0.12f, 0.08f), (0.35f - dirt) * 2f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateGrungyConcrete(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color baseConcrete = new Color(0.26f, 0.27f, 0.28f);
            Color darkStain = new Color(0.14f, 0.15f, 0.16f);
            Color palePatch = new Color(0.38f, 0.39f, 0.40f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nLarge = Mathf.PerlinNoise(x * 0.008f, y * 0.008f);
                    float nMed = Mathf.PerlinNoise(x * 0.035f, y * 0.035f) * 0.4f;
                    float nDetail = Mathf.PerlinNoise(x * 0.15f, y * 0.15f) * 0.15f;
                    float total = nLarge + nMed + nDetail;

                    Color c = baseConcrete;
                    if (total < 0.65f)
                    {
                        c = Color.Lerp(darkStain, baseConcrete, total / 0.65f);
                    }
                    else if (total > 0.95f)
                    {
                        c = Color.Lerp(baseConcrete, palePatch, (total - 0.95f) / 0.4f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateAlienTerrain(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color darkSoil = new Color(0.12f, 0.10f, 0.11f);
            Color rockMid = new Color(0.20f, 0.17f, 0.16f);
            Color mineral = new Color(0.28f, 0.22f, 0.18f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float n1 = Mathf.PerlinNoise(x * 0.015f, y * 0.015f);
                    float n2 = Mathf.PerlinNoise(x * 0.06f, y * 0.06f) * 0.5f;
                    float n3 = Mathf.PerlinNoise(x * 0.25f, y * 0.25f) * 0.25f;
                    float combined = n1 + n2 + n3;

                    Color c;
                    if (combined < 0.7f)
                    {
                        c = Color.Lerp(darkSoil, rockMid, combined / 0.7f);
                    }
                    else
                    {
                        c = Color.Lerp(rockMid, mineral, (combined - 0.7f) / 0.6f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCeilingGrid(int size = 512)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color tileColor = new Color(0.18f, 0.19f, 0.21f);
            Color gridLine = new Color(0.08f, 0.08f, 0.09f);
            Color ventSlit = new Color(0.04f, 0.04f, 0.05f);

            int tileSize = size / 4;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int tx = x % tileSize;
                    int ty = y % tileSize;

                    Color c = tileColor;

                    // Grid border
                    if (tx < 4 || ty < 4 || tx > tileSize - 4 || ty > tileSize - 4)
                    {
                        c = gridLine;
                    }
                    // Vent slits inside tile
                    else if ((x / tileSize + y / tileSize) % 2 == 1)
                    {
                        if (ty % 16 < 4 && tx > 16 && tx < tileSize - 16)
                        {
                            c = ventSlit;
                        }
                    }

                    float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f) * 0.06f - 0.03f;
                    c.r = Mathf.Clamp01(c.r + noise);
                    c.g = Mathf.Clamp01(c.g + noise);
                    c.b = Mathf.Clamp01(c.b + noise);

                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
#endif
