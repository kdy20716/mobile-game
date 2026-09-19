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

        public static Sprite GetOrCreateCapsuleButtonSprite(string name, Color mainCol, Color subCol)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateCapsuleTexture(384, 128, mainCol, subCol);
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

        public static Sprite GetOrCreate3DJellyButtonSprite(string name, Color mainCol)
        {
            Color subCol = Color.Lerp(mainCol, Color.black, 0.25f);
            return GetOrCreateCapsuleButtonSprite(name, mainCol, subCol);
        }

        public static Sprite GetOrCreateBackgroundSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Background.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateDreamyBackground(540, 960);
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

        public static Sprite GetOrCreatePanelSprite(string name, Color mainCol, Color rimCol)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateRoundedPanelTexture(128, 128, 32f, mainCol, rimCol);
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteBorder = new Vector4(36, 36, 36, 36); // 9-slice border
                    importer.spritePixelsPerUnit = 100;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Smile.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateMascotTexture(128);
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

        public static Sprite GetOrCreateCrownSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Crown_Gold.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateCrownTexture(96, 96);
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

        public static Sprite GetOrCreateFlameSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Flame_Pink.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateFlameTexture(80, 80);
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

        public static Sprite GetOrCreateDiceSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Dice_Skip.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateDiceTexture(80, 80);
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

        public static Sprite GetOrCreateRotateArrowSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Rotate_Arrow.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateRotateArrowTexture(80, 80);
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

        // 1. Dreamy Starry Pastel Background (540x960)
        private static Texture2D GenerateDreamyBackground(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color topColor = new Color(0.09f, 0.10f, 0.18f); // Deep Midnight Blue
            Color midColor = new Color(0.14f, 0.15f, 0.28f); // Dream Lavender Indigo
            Color botColor = new Color(0.19f, 0.12f, 0.25f); // Sweet Plum Wine

            // Generate soft gradient
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / height;
                Color rowCol = t < 0.5f 
                    ? Color.Lerp(botColor, midColor, t * 2f) 
                    : Color.Lerp(midColor, topColor, (t - 0.5f) * 2f);

                for (int x = 0; x < width; x++)
                {
                    tex.SetPixel(x, y, rowCol);
                }
            }

            // Draw soft pastel bokeh orbs
            DrawBokehOrb(tex, width * 0.25f, height * 0.75f, 160f, new Color(0.45f, 0.85f, 1f, 0.12f)); // Soft Cyan
            DrawBokehOrb(tex, width * 0.80f, height * 0.65f, 180f, new Color(1f, 0.45f, 0.75f, 0.10f)); // Soft Pink
            DrawBokehOrb(tex, width * 0.35f, height * 0.25f, 200f, new Color(0.65f, 0.45f, 1f, 0.10f)); // Soft Purple
            DrawBokehOrb(tex, width * 0.75f, height * 0.15f, 150f, new Color(1f, 0.85f, 0.4f, 0.08f));  // Soft Gold

            // Scatter cute twinkling stars
            int starCount = 65;
            UnityEngine.Random.InitState(42);
            for (int i = 0; i < starCount; i++)
            {
                float sx = UnityEngine.Random.Range(20f, width - 20f);
                float sy = UnityEngine.Random.Range(30f, height - 30f);
                float sSize = UnityEngine.Random.Range(2.5f, 6f);
                Color sCol = UnityEngine.Random.value > 0.4f 
                    ? new Color(1f, 1f, 1f, UnityEngine.Random.Range(0.4f, 0.9f))
                    : new Color(1f, 0.88f, 0.55f, UnityEngine.Random.Range(0.5f, 0.95f));

                DrawCuteStar(tex, sx, sy, sSize, sCol);
            }

            tex.Apply();
            return tex;
        }

        private static void DrawBokehOrb(Texture2D tex, float cx, float cy, float radius, Color col)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + radius));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + radius));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist < radius)
                    {
                        float alpha = Mathf.SmoothStep(1f, 0f, dist / radius) * col.a;
                        Color orig = tex.GetPixel(x, y);
                        Color blended = Color.Lerp(orig, new Color(col.r, col.g, col.b, 1f), alpha);
                        tex.SetPixel(x, y, blended);
                    }
                }
            }
        }

        // 2. Rounded Jelly Panel (9-sliceable)
        private static Texture2D GenerateRoundedPanelTexture(int width, int height, float radius, Color fillCol, Color rimCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float innerRadius = radius - 3f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cx = Mathf.Clamp(x, radius, width - radius);
                    float cy = Mathf.Clamp(y, radius, height - radius);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 2f);
                        Color c;
                        if (dist > innerRadius)
                        {
                            // Rim border
                            float rimT = (dist - innerRadius) / (radius - innerRadius);
                            c = Color.Lerp(fillCol, rimCol, rimT);
                        }
                        else
                        {
                            // Inner subtle gradient
                            float normY = (float)y / height;
                            c = Color.Lerp(Color.Lerp(fillCol, Color.black, 0.1f), fillCol, normY);
                        }

                        c.a *= alpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        // 3. 3D Glossy Jelly Button (9-sliceable)
        private static Texture2D Generate3DJellyButtonTexture(int width, int height, Color baseCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float radius = height * 0.42f;

            Color darkShadow = Color.Lerp(baseCol, Color.black, 0.35f);
            Color lightHighlight = Color.Lerp(baseCol, Color.white, 0.75f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cx = Mathf.Clamp(x, radius, width - radius);
                    float cy = Mathf.Clamp(y, radius, height - radius);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 2.5f);
                        float normY = (float)y / height;

                        // 3D Bevel Body
                        Color c = Color.Lerp(darkShadow, baseCol, Mathf.SmoothStep(0.12f, 0.70f, normY));

                        // Top Glossy Gel Highlight
                        if (y > height * 0.52f)
                        {
                            float hlDist = Mathf.Abs(y - height * 0.78f);
                            float hl = Mathf.SmoothStep(height * 0.28f, 0f, hlDist) * 0.45f;
                            c = Color.Lerp(c, Color.white, hl);
                        }

                        // Bottom Rim Reflection
                        if (y < height * 0.15f)
                        {
                            float rim = (height * 0.15f - y) / (height * 0.15f) * 0.3f;
                            c = Color.Lerp(c, lightHighlight, rim);
                        }

                        c.a *= alpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
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
            float radius = size * 0.28f;
            float center = size * 0.5f;

            Color darkRim = Color.Lerp(baseCol, Color.black, 0.25f);
            Color brightLight = Color.Lerp(baseCol, Color.white, 0.70f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center) - (center - radius - 4f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - center) - (center - radius - 4f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float edgeFactor = Mathf.Clamp01((radius - dist) / 5f);
                        float normY = (float)y / size;

                        Color c = Color.Lerp(darkRim, baseCol, Mathf.SmoothStep(0.08f, 0.92f, normY));

                        // Glossy Bubble Highlight on top-left
                        float hlDist = Mathf.Sqrt(Mathf.Pow(x - size * 0.36f, 2) + Mathf.Pow(y - size * 0.74f, 2));
                        if (hlDist < size * 0.30f)
                        {
                            float hl = Mathf.SmoothStep(size * 0.30f, size * 0.04f, hlDist) * 0.65f;
                            c = Color.Lerp(c, Color.white, hl);
                        }

                        // Bottom rim ambient glow
                        float bottomGlow = Mathf.SmoothStep(0.28f, 0.05f, normY) * 0.40f;
                        c = Color.Lerp(c, brightLight, bottomGlow);

                        c.a = edgeFactor;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            if (isBomb)
            {
                DrawCuteStar(tex, center, center, size * 0.26f, Color.white);
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateCapsuleTexture(int width, int height, Color topCol, Color bottomCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float radius = height * 0.48f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float capX = Mathf.Clamp(x, radius, width - radius);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(capX, height * 0.5f));

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 3f);
                        float normY = (float)y / height;
                        Color c = Color.Lerp(bottomCol, topCol, normY);

                        if (y > height * 0.55f)
                        {
                            float hl = (y - height * 0.55f) / (height * 0.45f);
                            c = Color.Lerp(c, Color.white, hl * 0.35f);
                        }

                        c.a = alpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static void DrawCuteStar(Texture2D tex, float cx, float cy, float r, Color starCol)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - r * 1.5f));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + r * 1.5f));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - r * 1.5f));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + r * 1.5f));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = Mathf.Abs(x - cx);
                    float dy = Mathf.Abs(y - cy);
                    if (dx + dy <= r || (dx < r * 0.35f && dy < r * 1.4f) || (dy < r * 0.35f && dx < r * 1.4f))
                    {
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        float a = Mathf.Clamp01((r * 1.4f - dist) / (r * 0.5f)) * starCol.a;
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, starCol, a));
                    }
                }
            }
        }

        // 4. Cute Jelly Mascot Character (128x128)
        private static Texture2D GenerateMascotTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float radius = size * 0.38f;
            float center = size * 0.5f;

            Color bodyCol = new Color(1f, 0.55f, 0.72f); // Strawberry Milk Pink
            Color darkBody = new Color(0.85f, 0.30f, 0.50f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center) - (center - radius - 2f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - (center - 4f)) - (center - radius - 2f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 2.5f);
                        float normY = (float)y / size;
                        Color c = Color.Lerp(darkBody, bodyCol, Mathf.SmoothStep(0.1f, 0.85f, normY));

                        // Top Glossy Gel
                        if (y > size * 0.58f)
                        {
                            float hlDist = Mathf.Sqrt(Mathf.Pow(x - size * 0.38f, 2) + Mathf.Pow(y - size * 0.76f, 2));
                            float hl = Mathf.SmoothStep(size * 0.28f, 0f, hlDist) * 0.55f;
                            c = Color.Lerp(c, Color.white, hl);
                        }

                        c.a = alpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // Draw Cute Eyes (Left & Right)
            DrawEye(tex, size * 0.36f, size * 0.48f, 6.5f);
            DrawEye(tex, size * 0.64f, size * 0.48f, 6.5f);

            // Draw Rosy Cheeks
            DrawBlush(tex, size * 0.26f, size * 0.36f, 8.5f, new Color(1f, 0.35f, 0.55f, 0.6f));
            DrawBlush(tex, size * 0.74f, size * 0.36f, 8.5f, new Color(1f, 0.35f, 0.55f, 0.6f));

            // Draw Cute Smile Mouth
            DrawSmile(tex, center, size * 0.34f, 8f, new Color(0.4f, 0.1f, 0.2f));

            tex.Apply();
            return tex;
        }

        private static void DrawEye(Texture2D tex, float cx, float cy, float r)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - r - 2));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + r + 2));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - r - 2));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + r + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist <= r)
                    {
                        // Catchlight shine on top-right
                        float shineDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx + r * 0.35f, cy + r * 0.35f));
                        Color col = shineDist < r * 0.45f ? Color.white : new Color(0.15f, 0.12f, 0.22f);
                        tex.SetPixel(x, y, col);
                    }
                }
            }
        }

        private static void DrawBlush(Texture2D tex, float cx, float cy, float r, Color col)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - r * 1.5f));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + r * 1.5f));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - r));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + r));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = (x - cx) / (r * 1.4f);
                    float dy = (y - cy) / (r * 0.8f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist < 1f)
                    {
                        float alpha = Mathf.SmoothStep(1f, 0f, dist) * col.a;
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, col, alpha));
                    }
                }
            }
        }

        private static void DrawSmile(Texture2D tex, float cx, float cy, float r, Color col)
        {
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (Mathf.Abs(dist - r) < 1.8f && y < cy)
                    {
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, col, 0.95f));
                    }
                }
            }
        }

        // 5. 3D Golden Crown (96x96)
        private static Texture2D GenerateCrownTexture(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color goldTop = new Color(1f, 0.88f, 0.35f);
            Color goldBase = new Color(0.92f, 0.65f, 0.12f);
            Color gemCol = new Color(1f, 0.30f, 0.55f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float nx = (float)x / w;
                    float ny = (float)y / h;

                    // Crown Silhouette: 3 peaks
                    bool inCrown = false;
                    if (ny >= 0.15f && ny <= 0.35f && nx >= 0.15f && nx <= 0.85f)
                    {
                        inCrown = true; // Bottom base band
                    }
                    else if (ny > 0.35f && ny <= 0.85f)
                    {
                        float centerPeak = 0.85f - Mathf.Abs(nx - 0.5f) * 1.6f;
                        float leftPeak = 0.70f - Mathf.Abs(nx - 0.25f) * 2.2f;
                        float rightPeak = 0.70f - Mathf.Abs(nx - 0.75f) * 2.2f;

                        if (ny < Mathf.Max(centerPeak, Mathf.Max(leftPeak, rightPeak)))
                        {
                            inCrown = true;
                        }
                    }

                    if (inCrown)
                    {
                        Color c = Color.Lerp(goldBase, goldTop, ny);
                        // Bevel highlight
                        if (x < w * 0.45f) c = Color.Lerp(c, Color.white, 0.25f);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            // Center Ruby Gem
            DrawEye(tex, w * 0.5f, h * 0.28f, 5f);
            DrawCuteStar(tex, w * 0.5f, h * 0.82f, 7f, Color.white);

            tex.Apply();
            return tex;
        }

        // 6. Cute Pink Flame Icon (80x80)
        private static Texture2D GenerateFlameTexture(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color flamePink = new Color(1f, 0.45f, 0.65f);
            Color flameCore = new Color(1f, 0.90f, 0.40f);

            float cx = w * 0.5f;
            float cy = h * 0.45f;
            float r = w * 0.35f;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float ny = (float)y / h;
                    float flameShape = r * (1.1f - Mathf.Pow(ny - 0.3f, 2) * 1.8f);

                    if (dist < flameShape && y < h * 0.88f)
                    {
                        float t = dist / flameShape;
                        Color c = Color.Lerp(flameCore, flamePink, t);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        // 7. 3D Jelly Dice Icon (80x80)
        private static Texture2D GenerateDiceTexture(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            float radius = w * 0.38f;
            float cx = w * 0.5f;
            float cy = h * 0.5f;

            Color diceCol = new Color(1f, 0.96f, 0.98f);
            Color shadowCol = new Color(0.85f, 0.82f, 0.92f);
            Color pipCol = new Color(0.95f, 0.30f, 0.55f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - cx) - (cx - radius - 2f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - cy) - (cy - radius - 2f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float normY = (float)y / h;
                        Color c = Color.Lerp(shadowCol, diceCol, normY);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // 5 Pips on Dice
            DrawEye(tex, cx, cy, 4f);
            DrawEye(tex, cx - 14f, cy - 14f, 3.5f);
            DrawEye(tex, cx + 14f, cy - 14f, 3.5f);
            DrawEye(tex, cx - 14f, cy + 14f, 3.5f);
            DrawEye(tex, cx + 14f, cy + 14f, 3.5f);

            tex.Apply();
            return tex;
        }

        // 8. Jelly Rotate Arrow Icon (80x80)
        private static Texture2D GenerateRotateArrowTexture(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            float cx = w * 0.5f;
            float cy = h * 0.5f;
            float r = w * 0.32f;
            Color arrowCol = Color.white;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float angle = Mathf.Atan2(y - cy, x - cx);

                    // Circular arc from -PI*0.8 to PI*0.6
                    if (Mathf.Abs(dist - r) < 5.5f && (angle > -2.2f && angle < 2.0f))
                    {
                        tex.SetPixel(x, y, arrowCol);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            // Arrow head
            for (int dy = -7; dy <= 7; dy++)
            {
                for (int dx = -7; dx <= 7; dx++)
                {
                    if (Mathf.Abs(dy) + dx < 6 && dx > -6)
                    {
                        tex.SetPixel((int)(cx + r) + dx, (int)cy + dy, arrowCol);
                    }
                }
            }

            tex.Apply();
            return tex;
        }
    }
}
#endif
