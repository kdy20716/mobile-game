#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class CuteCursorGenerator
    {
        private const string Folder = "Assets/Textures/Cursor";

        public static void GenerateCursorTextures(out Texture2D normalCursor, out Texture2D clickCursor)
        {
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            string normalPath = $"{Folder}/CuteCursor_Normal.png";
            string clickPath = $"{Folder}/CuteCursor_Click.png";

            GenerateNormalCursor(normalPath);
            GenerateClickCursor(clickPath);

            AssetDatabase.Refresh();

            ConfigureCursorImporter(normalPath);
            ConfigureCursorImporter(clickPath);

            normalCursor = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
            clickCursor = AssetDatabase.LoadAssetAtPath<Texture2D>(clickPath);

            // Also configure PlayerSettings default cursor
            if (normalCursor != null)
            {
                PlayerSettings.defaultCursor = normalCursor;
                Debug.Log("<color=#FF9EC0><b>[말랑블라스트]</b> 귀여운 파스텔 젤리 마우스 커서가 성공적으로 생성 및 등록되었습니다!</color>");
            }
        }

        private static void ConfigureCursorImporter(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Cursor) { importer.textureType = TextureImporterType.Cursor; dirty = true; }
                if (!importer.isReadable) { importer.isReadable = true; dirty = true; }
                if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; dirty = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; dirty = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
                if (importer.wrapMode != TextureWrapMode.Clamp) { importer.wrapMode = TextureWrapMode.Clamp; dirty = true; }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        private static void GenerateNormalCursor(string path)
        {
            const int W = 32;
            const int H = 32;
            Texture2D tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[W * H];

            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            // Palette
            Color outline = new Color(0.38f, 0.16f, 0.42f, 1.0f);        // Deep Sweet Purple Outline
            Color shadow = new Color(0.25f, 0.10f, 0.30f, 0.35f);        // Translucent Drop Shadow
            Color jellyFill = new Color(1.0f, 0.62f, 0.78f, 1.0f);       // Strawberry Milk Pink
            Color jellyGradient = new Color(1.0f, 0.78f, 0.88f, 1.0f);   // Soft Marshmallow Pink
            Color highlight = new Color(1.0f, 1.0f, 1.0f, 0.95f);        // Crisp Jelly Glaze White
            Color starYellow = new Color(1.0f, 0.92f, 0.42f, 1.0f);      // Fairy Star Gold
            Color starCore = new Color(1.0f, 1.0f, 0.85f, 1.0f);        // Star Glow Core

            // Coordinate system: (0,0) is bottom-left, (31,31) is top-right.
            // Arrow tip will be at (3, 28) - hotspot at (3, 3) in top-left cursor coordinates.
            void SetPix(int x, int y, Color c)
            {
                if (x >= 0 && x < W && y >= 0 && y < H)
                {
                    // Alpha blending
                    if (c.a < 1.0f)
                    {
                        Color bg = pixels[y * W + x];
                        float outA = c.a + bg.a * (1f - c.a);
                        if (outA > 0f)
                        {
                            Color blended = (c * c.a + bg * bg.a * (1f - c.a)) / outA;
                            blended.a = outA;
                            pixels[y * W + x] = blended;
                        }
                    }
                    else
                    {
                        pixels[y * W + x] = c;
                    }
                }
            }

            // 1. Draw Drop Shadow (offset +1, -1)
            int[,] arrowShape = GetArrowShapeMatrix();
            int rows = arrowShape.GetLength(0);
            int cols = arrowShape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int val = arrowShape[r, c];
                    if (val != 0)
                    {
                        int px = 3 + c + 1;
                        int py = 28 - r - 1;
                        SetPix(px, py, shadow);
                    }
                }
            }

            // 2. Draw Arrow Body, Outline & Highlights
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int val = arrowShape[r, c];
                    int px = 3 + c;
                    int py = 28 - r;

                    if (val == 1) // Outline
                    {
                        SetPix(px, py, outline);
                    }
                    else if (val == 2) // White Highlight Ridge
                    {
                        SetPix(px, py, highlight);
                    }
                    else if (val == 3) // Jelly Fill (Top light)
                    {
                        SetPix(px, py, jellyGradient);
                    }
                    else if (val == 4) // Jelly Fill (Base pink)
                    {
                        SetPix(px, py, jellyFill);
                    }
                    else if (val == 5) // Star Gold
                    {
                        SetPix(px, py, starYellow);
                    }
                    else if (val == 6) // Star Core White
                    {
                        SetPix(px, py, starCore);
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
        }

        private static void GenerateClickCursor(string path)
        {
            const int W = 32;
            const int H = 32;
            Texture2D tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[W * H];

            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            // Slightly squished / bouncy clicked state
            Color outline = new Color(0.38f, 0.16f, 0.42f, 1.0f);
            Color shadow = new Color(0.25f, 0.10f, 0.30f, 0.35f);
            Color jellyFill = new Color(1.0f, 0.52f, 0.72f, 1.0f);       // Deeper Jelly Pink on click
            Color jellyGradient = new Color(1.0f, 0.72f, 0.85f, 1.0f);
            Color highlight = new Color(1.0f, 1.0f, 1.0f, 0.95f);
            Color starYellow = new Color(1.0f, 0.96f, 0.55f, 1.0f);      // Bright Star Burst
            Color starCore = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Color burstPink = new Color(1.0f, 0.75f, 0.90f, 0.75f);      // Fairy Click Sparkle Ring

            void SetPix(int x, int y, Color c)
            {
                if (x >= 0 && x < W && y >= 0 && y < H)
                {
                    if (c.a < 1.0f)
                    {
                        Color bg = pixels[y * W + x];
                        float outA = c.a + bg.a * (1f - c.a);
                        if (outA > 0f)
                        {
                            Color blended = (c * c.a + bg * bg.a * (1f - c.a)) / outA;
                            blended.a = outA;
                            pixels[y * W + x] = blended;
                        }
                    }
                    else
                    {
                        pixels[y * W + x] = c;
                    }
                }
            }

            int[,] arrowShape = GetArrowShapeMatrix();
            int rows = arrowShape.GetLength(0);
            int cols = arrowShape.GetLength(1);

            // Click Burst Sparkles around tip (3, 28)
            SetPix(1, 28, burstPink);
            SetPix(3, 30, burstPink);
            SetPix(6, 29, burstPink);
            SetPix(0, 26, starYellow);

            // Shadow
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (arrowShape[r, c] != 0)
                    {
                        SetPix(3 + c + 1, 28 - r - 1, shadow);
                    }
                }
            }

            // Body
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int val = arrowShape[r, c];
                    int px = 3 + c;
                    int py = 28 - r;

                    if (val == 1) SetPix(px, py, outline);
                    else if (val == 2) SetPix(px, py, highlight);
                    else if (val == 3) SetPix(px, py, jellyGradient);
                    else if (val == 4) SetPix(px, py, jellyFill);
                    else if (val == 5) SetPix(px, py, starYellow);
                    else if (val == 6) SetPix(px, py, starCore);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
        }

        /// <summary>
        /// 20x23 hand-crafted adorable marshmallow pointer matrix
        /// 0: Transparent, 1: Outline, 2: Highlight, 3: Soft Pink, 4: Pink Fill, 5: Star Gold, 6: Star White Core
        /// </summary>
        private static int[,] GetArrowShapeMatrix()
        {
            return new int[,]
            {
                // Top row at r=0 (Tip of arrow)
                { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 3, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 2, 1, 0, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 3, 2, 1, 0, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 3, 3, 4, 4, 4, 4, 2, 1, 0, 0, 0, 0 },
                { 1, 2, 3, 3, 4, 4, 4, 4, 4, 4, 4, 2, 1, 0, 0, 0 },
                { 1, 2, 3, 4, 4, 4, 4, 5, 4, 4, 4, 4, 2, 1, 0, 0 },
                { 1, 2, 4, 4, 4, 4, 5, 6, 5, 4, 4, 4, 4, 2, 1, 0 },
                { 1, 2, 4, 4, 4, 5, 6, 6, 6, 5, 4, 4, 4, 4, 1, 1 },
                { 1, 2, 4, 4, 1, 1, 5, 6, 5, 1, 1, 1, 1, 1, 1, 0 },
                { 1, 2, 4, 1, 0, 0, 1, 5, 1, 4, 4, 1, 0, 0, 0, 0 },
                { 1, 2, 1, 0, 0, 0, 1, 4, 4, 4, 4, 1, 0, 0, 0, 0 },
                { 1, 1, 0, 0, 0, 0, 0, 1, 4, 4, 4, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 4, 4, 4, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 4, 4, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 }
            };
        }
    }
}
#endif
