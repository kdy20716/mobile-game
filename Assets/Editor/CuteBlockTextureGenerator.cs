#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;

namespace BlockBlast.Editor
{
    public static class CuteBlockTextureGenerator
    {
        private const string Folder = "Assets/Textures/BlockBlastCute";

        public static TMP_FontAsset GetOrCreateJuaFontAsset()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/Jua-Regular SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null)
            {
                return fontAsset;
            }

            string sourceFontPath = "Assets/Fonts/Jua-Regular.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null)
            {
                Debug.LogWarning("[말랑블라스트] Assets/Fonts/Jua-Regular.ttf 폰트를 찾을 수 없습니다.");
                return TMP_Settings.defaultFontAsset;
            }

            // Create high-resolution Dynamic SDF font asset with multi-atlas support for crystal clear Korean text
            fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fontAsset == null)
            {
                Debug.LogError("[말랑블라스트] SDF 폰트 에셋 생성 실패!");
                return TMP_Settings.defaultFontAsset;
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
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=#55FFAA><b>[말랑블라스트]</b> 고해상도 벡터 SDF 폰트 에셋(Jua-Regular SDF)이 성공적으로 생성되었습니다!</color>");
            return fontAsset;
        }

        [MenuItem("Block Blast/Import & Apply NanoBanana Images")]
        public static void ImportAndApplyNanoBananaImages()
        {
            EnsureFolder();
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";

            var mappings = new (string srcPattern, string dstName, bool removeWhiteBg)[]
            {
                ("jelly_background_dreamy_*.jpg", "Jelly_Background.png", false),
                ("jelly_mascot_smile_*.jpg", "Jelly_Mascot_Smile.png", true),
                ("jelly_mascot_mint_*.jpg", "Jelly_Mascot_Mint.png", true),
                ("jelly_mascot_gold_*.jpg", "Jelly_Mascot_Gold.png", true),
                ("jelly_mascot_purple_*.jpg", "Jelly_Mascot_Purple.png", true),
                ("jelly_crown_gold_*.jpg", "Jelly_Crown_Gold.png", true),
                ("jelly_flame_pink_*.jpg", "Jelly_Flame_Pink.png", true),
                ("jelly_dice_skip_*.jpg", "Jelly_Dice_Skip.png", true),
                ("jelly_rotate_arrow_*.jpg", "Jelly_Rotate_Arrow.png", true),
                ("jelly_button_pink_*.jpg", "Jelly_Button_Pink.png", true),
                ("jelly_button_teal_*.jpg", "Jelly_Button_Teal.png", true),
                ("jelly_star_bomb_*.jpg", "Jelly_StarBomb.png", true),
                ("jelly_gem_cube_*.jpg", "Jelly_Tile_Base.png", true),
                ("jelly_mascot_red_*.jpg", "Jelly_Mascot_Red.png", true)
            };

            foreach (var item in mappings)
            {
                string[] files = Directory.GetFiles(brainDir, item.srcPattern);
                if (files.Length == 0) continue;
                string srcFile = files[files.Length - 1];
                string dstPath = $"{Folder}/{item.dstName}";

                byte[] rawBytes = File.ReadAllBytes(srcFile);
                Texture2D tempTex = new Texture2D(2, 2);
                if (tempTex.LoadImage(rawBytes))
                {
                    if (item.removeWhiteBg)
                    {
                        PreciseFloodFillCutout(tempTex);
                    }

                    byte[] pngBytes = tempTex.EncodeToPNG();
                    SafeWriteAllBytes(dstPath, pngBytes);
                    AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);

                    TextureImporter importer = AssetImporter.GetAtPath(dstPath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.alphaIsTransparency = true;
                        importer.spritePixelsPerUnit = 100;
                        importer.filterMode = FilterMode.Bilinear;
                        importer.isReadable = true;
                        importer.SaveAndReimport();
                    }
                }
            }

            GetOrCreateBlueMascotSprite();
            GetOrCreateRedMascotSprite();

            Debug.Log("<color=#FF66CC><b>[말랑블라스트]</b> 모든 색상의 젤리 마스코트 캐릭터들을 정밀 BFS 누끼 및 임포트 완료했습니다!</color>");
        }

        public static Sprite ForceGetOrImportSingleSprite(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            EnsureTextureReadable(path);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; dirty = true; }
                if (!importer.isReadable) { importer.isReadable = true; dirty = true; }
                if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; dirty = true; }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void SafeWriteAllBytes(string path, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (System.IO.IOException)
            {
                try
                {
                    EditorUtility.UnloadUnusedAssetsImmediate();
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        fs.SetLength(0);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                catch
                {
                    if (File.Exists(path))
                    {
                        // File already exists and is locked by Unity's AssetPreview; keep existing file safely
                        Debug.Log($"<color=#FFAA00>[말랑블라스트]</color> {System.IO.Path.GetFileName(path)} 파일이 유니티 에디터에서 사용 중이므로 기존 파일을 유지합니다.");
                    }
                    else
                    {
                        Debug.LogWarning($"[말랑블라스트] {path} 파일을 작성하지 못했습니다.");
                    }
                }
            }
        }

        private static Texture2D PreciseFloodFillCutout(Texture2D tex)
        {
            int w = tex.width;
            int h = tex.height;
            Color[] pixels = tex.GetPixels();
            bool[] isBg = new bool[w * h];

            System.Collections.Generic.Queue<int> queue = new System.Collections.Generic.Queue<int>();

            void CheckBorder(int x, int y)
            {
                int idx = y * w + x;
                Color c = pixels[idx];
                float b = (c.r + c.g + c.b) / 3f;
                float diff = Mathf.Max(Mathf.Abs(c.r - c.g), Mathf.Abs(c.g - c.b), Mathf.Abs(c.b - c.r));
                if (b > 0.65f && diff < 0.25f)
                {
                    isBg[idx] = true;
                    queue.Enqueue(idx);
                }
            }

            for (int x = 0; x < w; x++) { CheckBorder(x, 0); CheckBorder(x, h - 1); }
            for (int y = 0; y < h; y++) { CheckBorder(0, y); CheckBorder(w - 1, y); }

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                int curr = queue.Dequeue();
                int cx = curr % w;
                int cy = curr / w;

                for (int d = 0; d < 4; d++)
                {
                    int nx = cx + dx[d];
                    int ny = cy + dy[d];
                    if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;

                    int nidx = ny * w + nx;
                    if (!isBg[nidx])
                    {
                        Color nc = pixels[nidx];
                        float nb = (nc.r + nc.g + nc.b) / 3f;
                        float diff = Mathf.Max(Mathf.Abs(nc.r - nc.g), Mathf.Abs(nc.g - nc.b), Mathf.Abs(nc.b - nc.r));

                        if (nb > 0.72f && diff < 0.25f)
                        {
                            isBg[nidx] = true;
                            queue.Enqueue(nidx);
                        }
                    }
                }
            }

            // Apply soft transparency with feathering (NO black fringing!)
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    if (isBg[idx])
                    {
                        Color c = pixels[idx];
                        float b = (c.r + c.g + c.b) / 3f;
                        if (b > 0.82f)
                        {
                            pixels[idx] = new Color(c.r, c.g, c.b, 0f);
                        }
                        else
                        {
                            float alpha = Mathf.Clamp01((0.82f - b) / 0.15f) * 0.5f;
                            pixels[idx] = new Color(c.r, c.g, c.b, alpha);
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(w, h, TextureFormat.RGBA32, false);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static void ConfigureFontSettings()
        {
            string fontPath = "Assets/Fonts/Jua-Regular.ttf";
            TrueTypeFontImporter fontImporter = AssetImporter.GetAtPath(fontPath) as TrueTypeFontImporter;
            if (fontImporter != null)
            {
                fontImporter.fontSize = 64;
                fontImporter.fontRenderingMode = FontRenderingMode.Smooth; // 0 = Smooth vector anti-aliased
                fontImporter.characterPadding = 4;
                fontImporter.SaveAndReimport();
            }
        }

        public static void EnsureTextureReadable(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return;
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (!importer.isReadable) { importer.isReadable = true; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    dirty = true;
                }
                if (dirty) importer.SaveAndReimport();
            }
        }

        public static Sprite GetOrCreatePastelBlockSprite(string name, Color pastelCol, string overlayFileName = null, float overlayScale = 0.86f)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D overlayTex = null;
            if (!string.IsNullOrEmpty(overlayFileName))
            {
                string ovPath = $"{Folder}/{overlayFileName}";
                EnsureTextureReadable(ovPath);
                overlayTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ovPath);
                if (overlayTex != null)
                {
                    Color c0 = overlayTex.GetPixel(0, 0);
                    // If corners have an opaque background, cut it out cleanly with flood fill
                    if (c0.a > 0.8f && (c0.r + c0.g + c0.b) / 3f > 0.8f)
                    {
                        Texture2D cutoutTex = PreciseFloodFillCutout(overlayTex);
                        byte[] bytesCutout = cutoutTex.EncodeToPNG();
                        SafeWriteAllBytes(ovPath, bytesCutout);
                        AssetDatabase.ImportAsset(ovPath, ImportAssetOptions.ForceUpdate);
                        overlayTex = cutoutTex;
                    }
                }
            }

            Texture2D blockTex = GeneratePastelBlockTexture(512, pastelCol, overlayTex, overlayScale);
            byte[] bytes = blockTex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.anisoLevel = 4;
                importer.mipmapEnabled = false; // Never blur 2D sprites with low-res mips
                importer.textureCompression = TextureImporterCompression.Uncompressed; // 100% loss-free crisp rendering!
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateEmptyCellSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Empty_Cell.png";

            Texture2D tex = GenerateEmptyCellTexture(256);
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Texture2D GeneratePastelBlockTexture(int size, Color pastelCol, Texture2D overlayTex, float overlayScale = 0.86f)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];

            float radius = size * 0.22f;
            Color topCol = Color.Lerp(pastelCol, Color.white, 0.25f);
            Color botCol = Color.Lerp(pastelCol, Color.black, 0.12f);
            Color borderCol = Color.Lerp(pastelCol, Color.black, 0.28f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int idx = y * size + x;
                    float dist = GetRoundedRectDistance(x, y, size, size, radius);

                    if (dist > 0f)
                    {
                        pixels[idx] = Color.clear;
                    }
                    else
                    {
                        float t = (float)y / size;
                        Color baseCol = Color.Lerp(botCol, topCol, t);

                        // 3D glossy highlight on top-left
                        float hlDist = Mathf.Sqrt((x - size * 0.35f) * (x - size * 0.35f) + (y - size * 0.72f) * (y - size * 0.72f));
                        float hl = Mathf.Clamp01(1f - hlDist / (size * 0.45f));
                        baseCol = Color.Lerp(baseCol, Color.white, hl * 0.35f);

                        // Inner bevel highlight: top and left inner edge
                        if (x >= 4 && x <= 12 && y >= 8 && y <= size - 8)
                        {
                            baseCol = Color.Lerp(baseCol, Color.white, 0.28f);
                        }
                        if (y >= size - 12 && y <= size - 4 && x >= 8 && x <= size - 8)
                        {
                            baseCol = Color.Lerp(baseCol, Color.white, 0.32f);
                        }

                        // Border antialiasing / stroke
                        if (dist > -4f)
                        {
                            float borderT = Mathf.Clamp01((dist + 4f) / 4f);
                            baseCol = Color.Lerp(baseCol, borderCol, borderT * 0.75f);
                        }

                        pixels[idx] = baseCol;
                    }
                }
            }

            tex.SetPixels(pixels);

            // Composite overlay texture seamlessly into center with 4-point supersampling & solid opacity
            if (overlayTex != null)
            {
                int ovSize = Mathf.RoundToInt(size * overlayScale);
                int startX = (size - ovSize) / 2;
                int startY = (size - ovSize) / 2;
                float stepU = 1f / Mathf.Max(1, ovSize - 1);
                float stepV = 1f / Mathf.Max(1, ovSize - 1);
                float subOffset = 0.25f;

                for (int oy = 0; oy < ovSize; oy++)
                {
                    for (int ox = 0; ox < ovSize; ox++)
                    {
                        float u = (float)ox * stepU;
                        float v = (float)oy * stepV;

                        // 4-point supersampling for vector-crisp lines
                        Color s1 = overlayTex.GetPixelBilinear(u - stepU * subOffset, v - stepV * subOffset);
                        Color s2 = overlayTex.GetPixelBilinear(u + stepU * subOffset, v - stepV * subOffset);
                        Color s3 = overlayTex.GetPixelBilinear(u - stepU * subOffset, v + stepV * subOffset);
                        Color s4 = overlayTex.GetPixelBilinear(u + stepU * subOffset, v + stepV * subOffset);

                        Color oc = new Color(
                            (s1.r + s2.r + s3.r + s4.r) * 0.25f,
                            (s1.g + s2.g + s3.g + s4.g) * 0.25f,
                            (s1.b + s2.b + s3.b + s4.b) * 0.25f,
                            (s1.a + s2.a + s3.a + s4.a) * 0.25f
                        );

                        if (oc.a > 0.02f)
                        {
                            int px = startX + ox;
                            int py = startY + oy;
                            if (px >= 0 && px < size && py >= 0 && py < size)
                            {
                                int idx = py * size + px;
                                Color bc = pixels[idx];
                                if (bc.a > 0.1f)
                                {
                                    // Solidify mascot body so it's punchy, bold and never faint
                                    float a = (oc.a >= 0.85f) ? 1.0f : Mathf.SmoothStep(0f, 1f, oc.a * 1.25f);
                                    
                                    // Micro-sharpen dark lines (eyes, pupils, mouth)
                                    float brightness = (oc.r + oc.g + oc.b) / 3f;
                                    if (brightness < 0.35f && oc.a > 0.4f)
                                    {
                                        a = 1.0f;
                                    }

                                    Color blended = new Color(
                                        oc.r * a + bc.r * (1f - a),
                                        oc.g * a + bc.g * (1f - a),
                                        oc.b * a + bc.b * (1f - a),
                                        bc.a
                                    );
                                    pixels[idx] = blended;
                                }
                            }
                        }
                    }
                }
                tex.SetPixels(pixels);
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateEmptyCellTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];

            float radius = size * 0.22f;
            Color cellBg = new Color(0.24f, 0.20f, 0.38f, 0.92f); // Sweet Pastel Candy Violet
            Color borderCol = new Color(0.52f, 0.45f, 0.78f, 0.95f); // Soft Glowing Lavender Rim
            Color innerShadow = new Color(0.16f, 0.13f, 0.26f, 0.95f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int idx = y * size + x;
                    float dist = GetRoundedRectDistance(x, y, size, size, radius);

                    if (dist > 0f)
                    {
                        pixels[idx] = Color.clear;
                    }
                    else
                    {
                        Color c = cellBg;
                        // 3D Soft Inset Bevel (Top-Left Shadow, Bottom-Right Light)
                        if (x <= 14 || y >= size - 14)
                        {
                            c = Color.Lerp(c, innerShadow, 0.45f);
                        }
                        if (x >= size - 12 || y <= 12)
                        {
                            c = Color.Lerp(c, Color.white, 0.15f);
                        }
                        if (dist > -4.5f)
                        {
                            float bt = Mathf.Clamp01((dist + 4.5f) / 4.5f);
                            c = Color.Lerp(c, borderCol, bt);
                        }
                        pixels[idx] = c;
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static float GetRoundedRectDistance(float x, float y, float width, float height, float radius)
        {
            float halfW = width * 0.5f;
            float halfH = height * 0.5f;
            float cx = Mathf.Abs(x - halfW) - (halfW - radius);
            float cy = Mathf.Abs(y - halfH) - (halfH - radius);

            if (cx <= 0f && cy <= 0f) return -radius;
            if (cx > 0f && cy <= 0f) return cx - radius;
            if (cx <= 0f && cy > 0f) return cy - radius;

            return Mathf.Sqrt(cx * cx + cy * cy) - radius;
        }

        public static Sprite GetOrCreateMintMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Mint.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateMascotSprite();
        }

        public static Sprite GetOrCreateGoldMascotSprite()
        {
            EnsureFolder();
            EnsureMascotsCutout();
            string path = $"{Folder}/Jelly_Mascot_Gold.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateCrownSprite();
        }

        public static Sprite GetOrCreatePurpleMascotSprite()
        {
            EnsureFolder();
            EnsureMascotsCutout();
            string path = $"{Folder}/Jelly_Mascot_Purple.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateDiceSprite();
        }

        public static Sprite GetOrCreateLobbyStageBackgroundSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Stage_Background.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateLobbyCandyStageSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Stage_Candy.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateLobbyOceanStageSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Stage_Ocean.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateCandyWonderlandBackgroundSprite()
        {
            EnsureFolder();
            EnsureThemeBackgrounds();
            string path = $"{Folder}/Theme_Candy_Wonderland.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateCrystalMermaidBackgroundSprite()
        {
            EnsureFolder();
            EnsureThemeBackgrounds();
            string path = $"{Folder}/Theme_Crystal_Mermaid.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateStarryNebulaBackgroundSprite()
        {
            EnsureFolder();
            EnsureThemeBackgrounds();
            string path = $"{Folder}/Theme_Starry_Nebula.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreatePauseBarsSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Icon_Pause_Bars.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            // Two smooth rounded vertical bars:
            float halfW = 11f;
            float halfH = 34f;
            float radius = 7f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d1 = GetPauseBoxDistance(x - 44f, y - 64f, halfW, halfH, radius);
                    float d2 = GetPauseBoxDistance(x - 84f, y - 64f, halfW, halfH, radius);
                    float d = Mathf.Min(d1, d2);

                    if (d <= 0f)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                    else if (d < 1.5f)
                    {
                        float a = 1f - (d / 1.5f);
                        pixels[y * size + x] = new Color(1f, 1f, 1f, a);
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }

            return ForceGetOrImportSingleSprite(path);
        }

        private static float GetPauseBoxDistance(float px, float py, float halfW, float halfH, float r)
        {
            float dx = Mathf.Abs(px) - (halfW - r);
            float dy = Mathf.Abs(py) - (halfH - r);
            if (dx <= 0f && dy <= 0f) return -r;
            if (dx > 0f && dy <= 0f) return dx - r;
            if (dx <= 0f && dy > 0f) return dy - r;
            return Mathf.Sqrt(dx * dx + dy * dy) - r;
        }

        public static void EnsureThemeBackgrounds()
        {
            EnsureFolder();
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var targets = new (string pattern, string dstName)[]
            {
                ("bg_candy_wonderland_*.jpg", "Theme_Candy_Wonderland.jpg"),
                ("bg_crystal_mermaid_*.jpg", "Theme_Crystal_Mermaid.jpg"),
                ("bg_starry_nebula_*.jpg", "Theme_Starry_Nebula.jpg")
            };

            foreach (var item in targets)
            {
                string dstPath = $"{Folder}/{item.dstName}";
                if (!File.Exists(dstPath) && Directory.Exists(brainDir))
                {
                    string[] files = Directory.GetFiles(brainDir, item.pattern);
                    if (files.Length > 0)
                    {
                        File.Copy(files[files.Length - 1], dstPath, true);
                        AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);

                        TextureImporter importer = AssetImporter.GetAtPath(dstPath) as TextureImporter;
                        if (importer != null)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.spriteImportMode = SpriteImportMode.Single;
                            importer.alphaIsTransparency = false;
                            importer.spritePixelsPerUnit = 100;
                            importer.filterMode = FilterMode.Bilinear;
                            importer.SaveAndReimport();
                        }
                    }
                }
            }
        }

        public static void EnsureMascotsCutout()
        {
            EnsureFolder();
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            var targets = new (string pattern, string dstName)[]
            {
                ("jelly_mascot_gold_*.jpg", "Jelly_Mascot_Gold.png"),
                ("jelly_mascot_purple_*.jpg", "Jelly_Mascot_Purple.png")
            };

            foreach (var item in targets)
            {
                string dstPath = $"{Folder}/{item.dstName}";

                // If already cut out with transparent pixels, keep it!
                if (File.Exists(dstPath))
                {
                    byte[] existingBytes = File.ReadAllBytes(dstPath);
                    Texture2D checkTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (checkTex.LoadImage(existingBytes))
                    {
                        if (checkTex.GetPixel(0, 0).a < 0.1f)
                        {
                            continue;
                        }
                    }
                }

                byte[] rawBytes = null;
                if (Directory.Exists(brainDir))
                {
                    string[] files = Directory.GetFiles(brainDir, item.pattern);
                    if (files.Length > 0)
                    {
                        rawBytes = File.ReadAllBytes(files[files.Length - 1]);
                    }
                }

                if (rawBytes == null && File.Exists(dstPath))
                {
                    rawBytes = File.ReadAllBytes(dstPath);
                }

                if (rawBytes != null)
                {
                    Texture2D srcTex = new Texture2D(2, 2);
                    if (srcTex.LoadImage(rawBytes))
                    {
                        // Explicitly construct RGBA32 texture to ensure alpha channel is preserved!
                        Texture2D rgbaTex = new Texture2D(srcTex.width, srcTex.height, TextureFormat.RGBA32, false);
                        rgbaTex.SetPixels(srcTex.GetPixels());
                        rgbaTex.Apply();

                        PreciseFloodFillCutout(rgbaTex);
                        byte[] pngBytes = rgbaTex.EncodeToPNG();
                        SafeWriteAllBytes(dstPath, pngBytes);
                        AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);

                        TextureImporter importer = AssetImporter.GetAtPath(dstPath) as TextureImporter;
                        if (importer != null)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.spriteImportMode = SpriteImportMode.Single;
                            importer.alphaIsTransparency = true;
                            importer.spritePixelsPerUnit = 100;
                            importer.filterMode = FilterMode.Bilinear;
                            importer.isReadable = true;
                            importer.SaveAndReimport();
                        }
                    }
                }
            }
        }

        public static Sprite GetBlockFaceSpriteForColor(Color col, bool isBomb)
        {
            EnsureFolder();
            if (isBomb) return GetOrCreateCuteJellySprite("Jelly_StarBomb", Color.white, true);

            // Determine best matching cute face icon by hue
            Color.RGBToHSV(col, out float h, out float s, out float v);

            // Pink / Red (0.9 ~ 1.0 or 0.0 ~ 0.08) -> Pink Mascot or Heart Flame
            if (h >= 0.88f || h <= 0.08f)
            {
                return GetOrCreateMascotSprite();
            }
            // Cyan / Teal / Green (0.35 ~ 0.55) -> Mint Mascot
            else if (h >= 0.35f && h <= 0.58f)
            {
                return GetOrCreateMintMascotSprite();
            }
            // Yellow / Orange / Gold (0.09 ~ 0.20) -> Golden Crown
            else if (h >= 0.09f && h <= 0.22f)
            {
                return GetOrCreateCrownSprite();
            }
            // Blue / Purple (0.60 ~ 0.85) -> Jelly Dice / Star
            else
            {
                return GetOrCreateDiceSprite();
            }
        }

        public static Sprite GetOrCreateGoldCoinSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Coin_Gold.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = size * 0.46f;
            float innerR = size * 0.38f;

            Color goldOuter = new Color(1f, 0.65f, 0.05f, 1f);
            Color goldRim = new Color(1f, 0.90f, 0.35f, 1f);
            Color goldFace = new Color(1f, 0.78f, 0.15f, 1f);
            Color goldShadow = new Color(0.85f, 0.50f, 0.05f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist > maxR)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float edgeAlpha = Mathf.Clamp01((maxR - dist) / 1.5f);
                        Color c;
                        if (dist > innerR)
                        {
                            float angle = Mathf.Atan2(y - cy, x - cx);
                            float light = (Mathf.Sin(angle + 0.8f) + 1f) * 0.5f;
                            c = Color.Lerp(goldOuter, goldRim, light);
                        }
                        else
                        {
                            float normY = (y - (cy - innerR)) / (innerR * 2f);
                            c = Color.Lerp(goldShadow, goldFace, normY);

                            float starDist = Mathf.Abs(x - cx) + Mathf.Abs(y - cy);
                            if (starDist < innerR * 0.55f)
                            {
                                float starAlpha = Mathf.Clamp01(1f - starDist / (innerR * 0.55f));
                                c = Color.Lerp(c, Color.white, starAlpha * 0.85f);
                            }
                        }

                        c.a *= edgeAlpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

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
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = Generate3DJellyButtonTexture(256, 96, mainCol);
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritesheet = new SpriteMetaData[0];
                importer.spriteBorder = new Vector4(45, 45, 45, 45);
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateBackgroundSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Background.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreatePanelSprite(string name, Color mainCol, Color rimCol)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = GenerateRoundedPanelTexture(128, 128, 32f, mainCol, rimCol);
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(36, 36, 36, 36); // 9-slice border
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Smile.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateCrownSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Crown_Gold.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateFlameSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Flame_Pink.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateDiceSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Dice_Skip.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateRotateArrowSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Rotate_Arrow.png";
            return ForceGetOrImportSingleSprite(path);
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

        // 3. 3D Glossy Jelly Button (9-sliceable, completely clean with NO white lines)
        private static Texture2D Generate3DJellyButtonTexture(int width, int height, Color baseCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float radius = height * 0.44f;

            Color darkShadow = Color.Lerp(baseCol, new Color(0.12f, 0.05f, 0.12f), 0.35f);
            Color topGlow = Color.Lerp(baseCol, Color.white, 0.32f);

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
                        float alpha = Mathf.Clamp01((radius - dist) / 2.0f);
                        float normY = (float)y / height;

                        // Smooth vertical 3D jelly volume: deeper at bottom, bright and rich at top (NO horizontal lines)
                        Color c;
                        if (normY < 0.5f)
                        {
                            c = Color.Lerp(darkShadow, baseCol, Mathf.SmoothStep(0f, 0.5f, normY));
                        }
                        else
                        {
                            c = Color.Lerp(baseCol, topGlow, Mathf.SmoothStep(0.5f, 1f, normY));
                        }

                        // Soft pill-shaped contour shading near boundary
                        float innerDist = dist / radius;
                        float edgeShade = Mathf.SmoothStep(0.72f, 1.0f, innerDist);
                        c = Color.Lerp(c, darkShadow, edgeShade * 0.22f);

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

        private static void DrawWinkEye(Texture2D tex, float cx, float cy, float r, Color col)
        {
            for (float t = -1f; t <= 1f; t += 0.04f)
            {
                float x = cx + t * r;
                float y = cy - (1f - t * t) * (r * 0.55f);
                for (int dy = -2; dy <= 2; dy++)
                {
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        int px = Mathf.RoundToInt(x + dx);
                        int py = Mathf.RoundToInt(y + dy);
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            tex.SetPixel(px, py, col);
                        }
                    }
                }
            }
        }

        private static void DrawStarEye(Texture2D tex, float cx, float cy, float r, Color starCol)
        {
            DrawCuteStar(tex, cx, cy, r, starCol);
            DrawBlush(tex, cx + r * 0.35f, cy + r * 0.35f, r * 0.35f, Color.white);
        }

        public static Sprite GetOrCreateBlueMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Blue.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                tex = GenerateBlueWinkingMascotTexture(256);
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.alphaIsTransparency = true;
                    importer.spritePixelsPerUnit = 100;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateRedMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Red.png";
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            string[] files = Directory.GetFiles(brainDir, "jelly_mascot_red_*.jpg");
            if (files.Length > 0)
            {
                string srcFile = files[files.Length - 1];
                if (!File.Exists(path) || File.GetLastWriteTimeUtc(srcFile) > File.GetLastWriteTimeUtc(path))
                {
                    byte[] rawBytes = File.ReadAllBytes(srcFile);
                    Texture2D tempTex = new Texture2D(2, 2);
                    if (tempTex.LoadImage(rawBytes))
                    {
                        PreciseFloodFillCutout(tempTex);
                        byte[] pngBytes = tempTex.EncodeToPNG();
                        SafeWriteAllBytes(path, pngBytes);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            return ForceGetOrImportSingleSprite(path);
        }

        private static Texture2D GenerateBlueWinkingMascotTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float radius = size * 0.38f;
            float center = size * 0.5f;

            Color bodyCol = new Color(0.36f, 0.77f, 1f); // Sky Blue
            Color darkBody = new Color(0.18f, 0.52f, 0.85f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center) - (center - radius - 4f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - (center - 6f)) - (center - radius - 4f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 3f);
                        float normY = (float)y / size;
                        Color c = Color.Lerp(darkBody, bodyCol, Mathf.SmoothStep(0.1f, 0.85f, normY));

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

            // Left Eye: Big cute anime eye with sparkle
            DrawEye(tex, size * 0.36f, size * 0.48f, 13f);
            // Right Eye: Playful winking eye (^_~)
            DrawWinkEye(tex, size * 0.64f, size * 0.48f, 14f, new Color(0.10f, 0.15f, 0.30f));

            // Rosy Peach Blushes
            DrawBlush(tex, size * 0.24f, size * 0.36f, 16f, new Color(1f, 0.50f, 0.65f, 0.65f));
            DrawBlush(tex, size * 0.76f, size * 0.36f, 16f, new Color(1f, 0.50f, 0.65f, 0.65f));

            // Cute open smile
            DrawSmile(tex, center, size * 0.34f, 14f, new Color(0.12f, 0.18f, 0.35f));

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateRedStarMascotTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float radius = size * 0.38f;
            float center = size * 0.5f;

            Color bodyCol = new Color(1f, 0.32f, 0.46f); // Cherry Red
            Color darkBody = new Color(0.78f, 0.12f, 0.25f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center) - (center - radius - 4f));
                    float dy = Mathf.Max(0f, Mathf.Abs(y - (center - 6f)) - (center - radius - 4f));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 3f);
                        float normY = (float)y / size;
                        Color c = Color.Lerp(darkBody, bodyCol, Mathf.SmoothStep(0.1f, 0.85f, normY));

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

            // Eyes: Golden Star-shaped pupils (★_★)
            DrawStarEye(tex, size * 0.36f, size * 0.48f, 13f, new Color(1f, 0.88f, 0.35f));
            DrawStarEye(tex, size * 0.64f, size * 0.48f, 13f, new Color(1f, 0.88f, 0.35f));

            // Rosy Blushes
            DrawBlush(tex, size * 0.24f, size * 0.36f, 16f, new Color(1f, 0.65f, 0.40f, 0.65f));
            DrawBlush(tex, size * 0.76f, size * 0.36f, 16f, new Color(1f, 0.65f, 0.40f, 0.65f));

            // Mischievous cute open smile
            DrawSmile(tex, center, size * 0.34f, 14f, new Color(0.35f, 0.08f, 0.12f));

            // Tiny cute golden star antenna on top
            DrawCuteStar(tex, center, size * 0.88f, 16f, new Color(1f, 0.88f, 0.35f));

            tex.Apply();
            return tex;
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

        // ==========================================
        // 9. Main Menu Wide Background (1:1 Square Seamless Expanded)
        // ==========================================
        public static Sprite GetOrCreateMainMenuWideBackgroundSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_MainMenu_Wide_BG.png";
            return ForceGetOrImportSingleSprite(path);
        }

        private static void DrawFluffyCloud(Texture2D tex, int cx, int cy, int radius, Color cloudCol)
        {
            int[] offsetsX = { 0, -radius / 2, radius / 2, -radius / 3, radius / 3 };
            int[] offsetsY = { 0, -radius / 4, -radius / 4, radius / 4, radius / 4 };
            int[] radii = { radius, (int)(radius * 0.75f), (int)(radius * 0.8f), (int)(radius * 0.65f), (int)(radius * 0.7f) };

            for (int k = 0; k < offsetsX.Length; k++)
            {
                int ox = cx + offsetsX[k];
                int oy = cy + offsetsY[k];
                int r = radii[k];

                for (int y = oy - r; y <= oy + r; y++)
                {
                    for (int x = ox - r; x <= ox + r; x++)
                    {
                        if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                        float dist = Vector2.Distance(new Vector2(x, y), new Vector2(ox, oy));
                        if (dist <= r)
                        {
                            float a = Mathf.Clamp01((1f - dist / r) * 1.5f) * cloudCol.a;
                            Color prev = tex.GetPixel(x, y);
                            tex.SetPixel(x, y, Color.Lerp(prev, new Color(cloudCol.r, cloudCol.g, cloudCol.b, 1f), a));
                        }
                    }
                }
            }
        }

        private static void DrawStar(Texture2D tex, int cx, int cy, float r, Color col)
        {
            int ir = Mathf.CeilToInt(r * 2f);
            for (int dy = -ir; dy <= ir; dy++)
            {
                for (int dx = -ir; dx <= ir; dx++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px < 0 || px >= tex.width || py < 0 || py >= tex.height) continue;

                    // 4-point cross star distance
                    float d1 = Mathf.Abs(dx) + Mathf.Abs(dy) * 2.5f;
                    float d2 = Mathf.Abs(dx) * 2.5f + Mathf.Abs(dy);
                    float d = Mathf.Min(d1, d2);

                    if (d < r * 2.5f)
                    {
                        float alpha = Mathf.Clamp01(1f - d / (r * 2.5f)) * col.a;
                        Color prev = tex.GetPixel(px, py);
                        tex.SetPixel(px, py, Color.Lerp(prev, new Color(col.r, col.g, col.b, 1f), alpha));
                    }
                }
            }
        }

        // ==========================================
        // 10. Mallang Blast Cute 3D Jelly Title Logo
        // ==========================================
        public static Sprite GetOrCreateMallangBlastLogoSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mallang_Logo.png";
            return ForceGetOrImportSingleSprite(path);
        }

        // ==========================================
        // 11. Left Popping Mascot (Strawberry Smile)
        // ==========================================
        public static Sprite GetOrCreateLeftPoppingMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Left_Pop.png";
            return ForceGetOrImportSingleSprite(path);
        }

        // ==========================================
        // 12. Right Popping Mascot (Mint Soda)
        // ==========================================
        public static Sprite GetOrCreateRightPoppingMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Mascot_Right_Pop.png";
            return ForceGetOrImportSingleSprite(path);
        }

        // ==========================================
        // 13. Soft Glowing Fireworks Particle Sprites
        // ==========================================
        public static Sprite GetOrCreateFireworksGlowOrbSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Particle_GlowOrb.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = size * 0.47f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float u = dist / maxR;
                    if (u >= 1f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                    else
                    {
                        // Bright center core + smooth radial glow falloff
                        float core = Mathf.Clamp01(1f - u / 0.32f);
                        core = core * core;

                        float halo = Mathf.Clamp01(1f - u);
                        halo = Mathf.Pow(halo, 1.6f);

                        float alpha = Mathf.Clamp01(halo * 0.94f + core * 0.06f);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
            }

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateFireworksSparkleStarSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Particle_SparkleStar.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = size * 0.47f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - cx) / maxR;
                    float dy = Mathf.Abs(y - cy) / maxR;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > 1.25f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                        continue;
                    }

                    // 4-point sharp star rays
                    float rayX = Mathf.Pow(Mathf.Clamp01(1f - dx), 3.5f) * Mathf.Pow(Mathf.Clamp01(1f - dy / 0.16f), 2f);
                    float rayY = Mathf.Pow(Mathf.Clamp01(1f - dy), 3.5f) * Mathf.Pow(Mathf.Clamp01(1f - dx / 0.16f), 2f);

                    // Soft diagonal glints
                    float diag1 = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(dx - dy) / 0.22f), 2f) * Mathf.Pow(Mathf.Clamp01(1f - dist / 0.7f), 2.5f) * 0.35f;
                    float diag2 = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(dx + dy) / 0.22f), 2f) * Mathf.Pow(Mathf.Clamp01(1f - dist / 0.7f), 2.5f) * 0.35f;

                    // Central glowing core
                    float coreGlow = Mathf.Pow(Mathf.Clamp01(1f - dist / 0.38f), 2f);

                    float intensity = Mathf.Clamp01(rayX + rayY + diag1 + diag2 + coreGlow);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, intensity));
                }
            }

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateFireworksShockwaveRingSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Particle_Ring.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = size * 0.47f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float u = dist / maxR;
                    if (u > 1f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                    else
                    {
                        // Gaussian ring centered at u = 0.68 with sigma = 0.13
                        float diff = u - 0.68f;
                        float val = Mathf.Exp(-(diff * diff) / (2f * 0.13f * 0.13f));
                        float alpha = Mathf.Clamp01(val * 0.92f);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
            }

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // ==========================================
        // 14. User-Provided 2.5D Mallang Buttons & Fever Gradient
        // ==========================================
        public static Sprite GetOrCreateUserSkipButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Button_Skip.png";
            if (File.Exists(path))
            {
                EnsureTextureReadable(path);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    Color c0 = tex.GetPixel(0, 0);
                    if (c0.a > 0.9f && (c0.r + c0.g + c0.b) / 3f > 0.8f)
                    {
                        PreciseFloodFillCutout(tex);
                        byte[] bytes = tex.EncodeToPNG();
                        SafeWriteAllBytes(path, bytes);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreate2D5SkipButtonSprite();
        }

        public static Sprite GetOrCreateUserSpinButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Button_Spin.png";
            if (File.Exists(path))
            {
                EnsureTextureReadable(path);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    Color c0 = tex.GetPixel(0, 0);
                    if (c0.a > 0.9f && (c0.r + c0.g + c0.b) / 3f > 0.8f)
                    {
                        PreciseFloodFillCutout(tex);
                        byte[] bytes = tex.EncodeToPNG();
                        SafeWriteAllBytes(path, bytes);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreate2D5RotateButtonSprite();
        }

        public static Sprite GetOrCreate2D5SkipButtonSprite()
        {
            EnsureFolder();
            string dstPath = $"{Folder}/Jelly_Button_Skip_25D.png";
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            string[] files = Directory.GetFiles(brainDir, "jelly_btn_25d_skip_*.jpg");
            if (files.Length > 0 && !File.Exists(dstPath))
            {
                byte[] rawBytes = File.ReadAllBytes(files[files.Length - 1]);
                Texture2D tempTex = new Texture2D(2, 2);
                if (tempTex.LoadImage(rawBytes))
                {
                    PreciseFloodFillCutout(tempTex);
                    byte[] pngBytes = tempTex.EncodeToPNG();
                    SafeWriteAllBytes(dstPath, pngBytes);
                    AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
                }
            }
            return ForceGetOrImportSingleSprite(dstPath) ?? GetOrCreate3DJellyButtonSprite("Jelly_Button_Pink", new Color(0.98f, 0.36f, 0.58f));
        }

        public static Sprite GetOrCreate2D5RotateButtonSprite()
        {
            EnsureFolder();
            string dstPath = $"{Folder}/Jelly_Button_Rotate_25D.png";
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            string[] files = Directory.GetFiles(brainDir, "jelly_btn_25d_rotate_*.jpg");
            if (files.Length > 0 && !File.Exists(dstPath))
            {
                byte[] rawBytes = File.ReadAllBytes(files[files.Length - 1]);
                Texture2D tempTex = new Texture2D(2, 2);
                if (tempTex.LoadImage(rawBytes))
                {
                    PreciseFloodFillCutout(tempTex);
                    byte[] pngBytes = tempTex.EncodeToPNG();
                    SafeWriteAllBytes(dstPath, pngBytes);
                    AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
                }
            }
            return ForceGetOrImportSingleSprite(dstPath) ?? GetOrCreate3DJellyButtonSprite("Jelly_Button_Teal", new Color(0.24f, 0.82f, 0.68f));
        }

        public static Sprite GetOrCreateFeverFillGradientSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Fever_Fill_Gradient.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int w = 256;
            int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color pinkCol = new Color(1.0f, 0.38f, 0.62f); // Strawberry Pink
            Color mintCol = new Color(0.28f, 0.90f, 0.75f); // Mint Soda
            Color goldCol = new Color(1.0f, 0.86f, 0.35f); // Lemon Gold Accent

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / (h - 1);
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / (w - 1);

                    // 2-tone layered gradient (Pink on left transitioning through gold to Mint on right)
                    Color baseCol;
                    if (u < 0.5f)
                    {
                        baseCol = Color.Lerp(pinkCol, goldCol, u * 2f);
                    }
                    else
                    {
                        baseCol = Color.Lerp(goldCol, mintCol, (u - 0.5f) * 2f);
                    }

                    // Upper glossy candy specular shine
                    if (v > 0.55f)
                    {
                        float shine = Mathf.Clamp01((v - 0.55f) / 0.45f);
                        baseCol = Color.Lerp(baseCol, Color.white, shine * 0.45f);
                    }
                    // Subtle bottom shadow for 2.5D depth
                    else if (v < 0.2f)
                    {
                        float shadow = Mathf.Clamp01((0.2f - v) / 0.2f);
                        baseCol = Color.Lerp(baseCol, new Color(baseCol.r * 0.7f, baseCol.g * 0.7f, baseCol.b * 0.7f), shadow * 0.4f);
                    }

                    tex.SetPixel(x, y, baseCol);
                }
            }

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateVignetteSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Vignette_Danger.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int w = 512;
            int h = 512;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            float cx = (w - 1) * 0.5f;
            float cy = (h - 1) * 0.5f;

            Color dangerRed = new Color(0.95f, 0.08f, 0.16f); // Vivid crimson red

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dx = (x - cx) / cx;
                    float dy = (y - cy) / cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = 0f;
                    if (dist > 0.45f)
                    {
                        float t = (dist - 0.45f) / 0.75f;
                        alpha = Mathf.Clamp01(Mathf.Pow(t, 1.8f)) * 0.90f;
                    }

                    tex.SetPixel(x, y, new Color(dangerRed.r, dangerRed.g, dangerRed.b, alpha));
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateTimeBarGradientSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Jelly_Time_Fill_Gradient.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }

            int w = 256;
            int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color mintCol = new Color(0.18f, 0.86f, 0.68f); // Fresh mint
            Color goldCol = new Color(1.0f, 0.82f, 0.28f);  // Warm gold
            Color coralCol = new Color(1.0f, 0.40f, 0.55f); // Coral pink

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / (h - 1);
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / (w - 1);
                    Color baseCol = (u < 0.5f)
                        ? Color.Lerp(coralCol, goldCol, u * 2f)
                        : Color.Lerp(goldCol, mintCol, (u - 0.5f) * 2f);

                    if (v > 0.65f)
                    {
                        baseCol = Color.Lerp(baseCol, Color.white, (v - 0.65f) * 0.45f);
                    }

                    tex.SetPixel(x, y, baseCol);
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateFairyRippleSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Fairy_Ripple_Ring.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float ringR = size * 0.38f;
            float ringW = size * 0.07f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    float dist = Mathf.Abs(d - ringR);
                    float alpha = Mathf.Exp(-(dist * dist) / (2f * ringW * ringW));
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateFairySparkleSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Fairy_Sparkle_Star.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - cx) / maxR;
                    float dy = Mathf.Abs(y - cy) / maxR;

                    float beamX = Mathf.Exp(-dx * 5.5f) * Mathf.Exp(-dy * dy * 35f);
                    float beamY = Mathf.Exp(-dy * 5.5f) * Mathf.Exp(-dx * dx * 35f);
                    float core = Mathf.Exp(-(dx * dx + dy * dy) * 16f);
                    float alpha = Mathf.Clamp01(beamX + beamY + core);

                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateFairyTrailSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Fairy_Trail_Glow.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) / maxR;
                    float alpha = Mathf.Exp(-dist * dist * 4.5f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateCircleFrameSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Circle_Frame.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float outerR = size * 0.48f;
            float borderW = size * 0.07f;
            float innerR = outerR - borderW;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    if (d > outerR + 1f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (d > outerR - 1f)
                    {
                        // Anti-aliased outer edge
                        float a = Mathf.Clamp01(outerR + 1f - d);
                        tex.SetPixel(x, y, new Color(1f, 0.85f, 0.92f, a));
                    }
                    else if (d > innerR)
                    {
                        // Pastel pinkish-gold border
                        tex.SetPixel(x, y, new Color(1f, 0.82f, 0.90f, 1f));
                    }
                    else if (d > innerR - 1f)
                    {
                        // Inner border transition
                        float a = Mathf.Clamp01(d - (innerR - 1f));
                        Color c = Color.Lerp(new Color(1f, 0.95f, 0.98f, 1f), new Color(1f, 0.82f, 0.90f, 1f), a);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        // Soft white disc center
                        tex.SetPixel(x, y, new Color(1f, 0.96f, 0.98f, 0.95f));
                    }
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateTabPillSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Tab_Pill.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null && existing.border != Vector4.zero) return existing;

            int w = 96;
            int h = 48;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            float r = (h - 1) * 0.5f;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float cx = (x < r) ? r : (x > w - 1 - r) ? w - 1 - r : x;
                    float cy = r;
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    if (d > r + 1f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (d > r - 1f)
                    {
                        float a = Mathf.Clamp01(r + 1f - d);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * 0.9f));
                    }
                    else
                    {
                        // Semi-transparent frosty pastel white pill
                        float topShine = (float)y / h;
                        Color c = Color.Lerp(new Color(1f, 0.92f, 0.96f, 0.85f), new Color(1f, 1f, 1f, 0.95f), topShine);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.spriteBorder = new Vector4(24, 20, 24, 20); // 9-slice
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateFairyActionButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Btn_Action_3D.png";

            int w = 256;
            int h = 104;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            float radius = 42f;

            Color bottomShadow = new Color(0.80f, 0.76f, 0.88f); // soft grounding pearl
            Color topGlow = new Color(1.0f, 1.0f, 1.0f);         // clean bright white

            for (int y = 0; y < h; y++)
            {
                float normY = (float)y / (h - 1);
                for (int x = 0; x < w; x++)
                {
                    float cx = Mathf.Clamp(x, radius, w - 1 - radius);
                    float cy = Mathf.Clamp(y, radius, h - 1 - radius);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (dist > radius + 0.5f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01(radius + 0.5f - dist);
                        
                        // Pure continuous smooth vertical gradient
                        Color baseColor = Color.Lerp(bottomShadow, topGlow, normY);

                        // Top curved glass shine arc
                        if (normY > 0.40f)
                        {
                            float shineAlpha = Mathf.SmoothStep(0.40f, 0.90f, normY) * 0.35f;
                            baseColor = Color.Lerp(baseColor, Color.white, shineAlpha);
                        }

                        // Bottom 3D bevel depth
                        if (normY < 0.15f)
                        {
                            float shadowAlpha = (1f - normY / 0.15f) * 0.25f;
                            baseColor = Color.Lerp(baseColor, new Color(0.45f, 0.40f, 0.55f), shadowAlpha);
                        }

                        baseColor.a *= alpha;
                        tex.SetPixel(x, y, baseColor);
                    }
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.spriteBorder = new Vector4(46, 36, 46, 36); // 9-slice
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateCuteCardSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Lobby_Cute_Card.png";

            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float r = 52f;

            // Premium fairy-tale marshmallow card: crisp pastel lavender border + subtle glossy rim
            Color cardTop = new Color(1.0f, 0.99f, 1.0f, 0.98f);
            Color cardBot = new Color(0.97f, 0.95f, 0.99f, 0.98f);
            Color borderCol = new Color(0.92f, 0.85f, 0.97f, 1f);
            Color innerGlowCol = new Color(1.0f, 0.96f, 0.99f, 1f);
            Color shadowCol = new Color(0.15f, 0.10f, 0.25f, 0.22f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float cx = (x < r) ? r : (x > size - 1 - r) ? size - 1 - r : x;
                    float cy = (y < r) ? r : (y > size - 1 - r) ? size - 1 - r : y;
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    // Outer drop shadow (offset down 3px)
                    float sy = y + 3f;
                    float scy = (sy < r) ? r : (sy > size - 1 - r) ? size - 1 - r : sy;
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 5f)
                        {
                            float sa = Mathf.Clamp01((r + 5f - sd) / 5f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        // Outer anti-aliasing
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        tex.SetPixel(x, y, new Color(borderCol.r, borderCol.g, borderCol.b, a));
                    }
                    else if (d > r - 5.5f)
                    {
                        // Crisp delicate pastel rim
                        tex.SetPixel(x, y, borderCol);
                    }
                    else if (d > r - 8.5f)
                    {
                        // Soft inner glow
                        float gt = Mathf.Clamp01((d - (r - 8.5f)) / 3f);
                        Color c = Color.Lerp(cardTop, innerGlowCol, gt);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        // Card inside: clean sweet marshmallow gradient
                        float v = (float)y / size;
                        Color c = Color.Lerp(cardBot, cardTop, v);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.spritePixelsPerUnit = 100;
                importer.spriteBorder = new Vector4(60, 60, 60, 60); // 9-slice
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
#endif
