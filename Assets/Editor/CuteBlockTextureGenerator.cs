#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;

namespace BlockBlast.Editor
{
    public static class CuteBlockTextureGenerator
    {
        private const string Folder = "Assets/Textures/BlockBlastCute";

        // Adorable 2.5D Marshmallow Pastel Palette
        public static readonly Color PastelPink = new Color(1.0f, 0.65f, 0.77f, 1f);      // #FFA6C4 Soft Strawberry Milk
        public static readonly Color PastelMint = new Color(0.52f, 0.91f, 0.82f, 1f);      // #85E8D1 Soft Ice Mint Latte
        public static readonly Color PastelLavender = new Color(0.79f, 0.68f, 0.98f, 1f);  // #C9ADFA Soft Sweet Lavender
        public static readonly Color PastelButter = new Color(1.0f, 0.86f, 0.52f, 1f);    // #FFDC85 Soft Honey Butter

        public static TMP_FontAsset GetOrCreateJuaFontAsset()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/Jua-Regular SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null)
            {
                EnsureFallbackFont(fontAsset);
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
            EnsureFallbackFont(fontAsset);
            return fontAsset;
        }

        public static TMP_FontAsset GetOrCreateMalgunFont()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/Malgun SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null && fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                return fontAsset;
            }

            string sourceFontPath = "Assets/Fonts/Malgun.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null) return null;

            fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 72, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fontAsset == null) return null;

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
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
            return fontAsset;
        }

        public static TMP_FontAsset GetOrCreateSimSunFont()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/SimSun SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null && fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                return fontAsset;
            }

            string sourceFontPath = "Assets/Fonts/SimSun.ttf";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null) return null;

            fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 72, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fontAsset == null) return null;

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
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
            return fontAsset;
        }

        public static TMP_FontAsset GetOrCreateMSGothicFont()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/MSGothic SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null && fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                return fontAsset;
            }

            string sourceFontPath = "Assets/Fonts/MSGothic.ttc";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null) return null;

            fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 72, 9, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fontAsset == null) return null;

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
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
            return fontAsset;
        }

        public static TMP_FontAsset GetOrCreateMSYaHeiFont()
        {
            EnsureFolder();
            string assetPath = "Assets/Fonts/MSYaHei SDF.asset";
            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null && fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0 && fontAsset.atlasTextures[0] != null)
            {
                return fontAsset;
            }

            string sourceFontPath = "Assets/Fonts/MSYaHei.ttc";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourceFontPath);
            if (sourceFont == null) return null;

            fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 72, 9, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fontAsset == null) return null;

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
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
            return fontAsset;
        }

        public static Sprite GetOrCreateCloseXButtonSprite()
        {
            return GetOrCreateNanoBananaCandyCloseButtonSprite(false);
        }

        private static void EnsureFallbackFont(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null) return;
            if (fontAsset.fallbackFontAssetTable == null)
                fontAsset.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();

            fontAsset.fallbackFontAssetTable.RemoveAll(x => x == null);

            TMP_FontAsset malgunSdf = GetOrCreateMalgunFont();
            if (malgunSdf != null && !fontAsset.fallbackFontAssetTable.Contains(malgunSdf))
            {
                fontAsset.fallbackFontAssetTable.Add(malgunSdf);
                EditorUtility.SetDirty(fontAsset);
            }

            TMP_FontAsset gothicSdf = GetOrCreateMSGothicFont();
            if (gothicSdf != null && !fontAsset.fallbackFontAssetTable.Contains(gothicSdf))
            {
                fontAsset.fallbackFontAssetTable.Add(gothicSdf);
                EditorUtility.SetDirty(fontAsset);
            }

            TMP_FontAsset yaheiSdf = GetOrCreateMSYaHeiFont();
            if (yaheiSdf != null && !fontAsset.fallbackFontAssetTable.Contains(yaheiSdf))
            {
                fontAsset.fallbackFontAssetTable.Add(yaheiSdf);
                EditorUtility.SetDirty(fontAsset);
            }

            TMP_FontAsset simsunSdf = GetOrCreateSimSunFont();
            if (simsunSdf != null && !fontAsset.fallbackFontAssetTable.Contains(simsunSdf))
            {
                fontAsset.fallbackFontAssetTable.Add(simsunSdf);
                EditorUtility.SetDirty(fontAsset);
            }

            AssetDatabase.SaveAssets();
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
                if (importer.crunchedCompression) { importer.crunchedCompression = false; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    dirty = true;
                }
                if (dirty) importer.SaveAndReimport();
            }
        }

        public static Sprite GetOrCreatePastelBlockSprite(string name, Color pastelCol, string overlayFileName = null, float overlayScale = 0.86f, bool forceRecreate = false)
        {
            if (string.Equals(name, "Block_Blue", System.StringComparison.OrdinalIgnoreCase))
            {
                string delPath = $"{Folder}/Block_Blue.png";
                if (File.Exists(delPath)) AssetDatabase.DeleteAsset(delPath);
                return null;
            }

            EnsureFolder();
            string path = $"{Folder}/{name}.png";
            if (File.Exists(path) && !forceRecreate)
            {
                return ForceGetOrImportSingleSprite(path);
            }

            Texture2D overlayTex = null;
            if (!string.IsNullOrEmpty(overlayFileName))
            {
                string backupPath = $"Assets/Textures/BlockBlastCute_Backup/{overlayFileName}";
                string ovPath = File.Exists(backupPath) ? backupPath : $"{Folder}/{overlayFileName}";
                if (File.Exists(ovPath))
                {
                    try
                    {
                        byte[] fileBytes = File.ReadAllBytes(ovPath);
                        Texture2D rawTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (rawTex.LoadImage(fileBytes))
                        {
                            Color c0 = rawTex.GetPixel(0, 0);
                            // If corners have an opaque background, cut it out cleanly with flood fill
                            if (c0.a > 0.8f && (c0.r + c0.g + c0.b) / 3f > 0.8f)
                            {
                                Texture2D cutoutTex = PreciseFloodFillCutout(rawTex);
                                byte[] bytesCutout = cutoutTex.EncodeToPNG();
                                SafeWriteAllBytes(ovPath, bytesCutout);
                                AssetDatabase.ImportAsset(ovPath, ImportAssetOptions.ForceUpdate);
                                overlayTex = cutoutTex;
                            }
                            else
                            {
                                overlayTex = rawTex;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[CuteBlockTextureGenerator] Failed loading overlay {overlayFileName}: {ex.Message}");
                    }
                }
            }

            Texture2D blockTex = GeneratePastelBlockTexture(2048, pastelCol, overlayTex, overlayScale);
            byte[] bytes = blockTex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.anisoLevel = 16;
                importer.maxTextureSize = 2048;
                importer.mipmapEnabled = true; // Enables clean anti-aliased downsampling on mobile displays
                importer.mipMapBias = -0.3f; // Crisp, sharp rendering without aliasing
                importer.textureCompression = TextureImporterCompression.Uncompressed; // 100% loss-free crisp rendering!
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            Sprite loadedSp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (loadedSp == null)
            {
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var a in assets)
                {
                    if (a is Sprite s) return s;
                }
            }
            return loadedSp;
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

            float radius = size * 0.20f;

            // Soft marshmallow pastel gradient
            Color topCol = Color.Lerp(pastelCol, Color.white, 0.45f);
            Color botCol = Color.Lerp(pastelCol, Color.white, 0.18f);
            Color borderCol = Color.Lerp(pastelCol, Color.black, 0.28f);

            int bevelMinX = Mathf.RoundToInt(size * 0.012f);
            int bevelMaxX = Mathf.RoundToInt(size * 0.035f);
            int bevelMinY = Mathf.RoundToInt(size * 0.012f);
            int bevelMaxY = size - bevelMinY;
            float borderDist = Mathf.Max(6f, size * 0.014f);

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
                        float hlDist = Mathf.Sqrt((x - size * 0.32f) * (x - size * 0.32f) + (y - size * 0.74f) * (y - size * 0.74f));
                        float hl = Mathf.Clamp01(1f - hlDist / (size * 0.45f));
                        baseCol = Color.Lerp(baseCol, Color.white, hl * 0.38f);

                        // Inner bevel highlight: top and left inner edge
                        if (x >= bevelMinX && x <= bevelMaxX && y >= bevelMinY && y <= bevelMaxY)
                        {
                            baseCol = Color.Lerp(baseCol, Color.white, 0.28f);
                        }
                        if (y >= size - bevelMaxX && y <= size - bevelMinX && x >= bevelMinY && x <= size - bevelMinY)
                        {
                            baseCol = Color.Lerp(baseCol, Color.white, 0.32f);
                        }

                        // Bottom shadow band for 3D depth
                        if (y < size * 0.10f)
                        {
                            float shadowT = 1f - (y / (size * 0.10f));
                            baseCol = Color.Lerp(baseCol, Color.Lerp(pastelCol, Color.black, 0.25f), shadowT * 0.35f);
                        }

                        // Border antialiasing
                        if (dist > -borderDist)
                        {
                            float borderT = Mathf.Clamp01((dist + borderDist) / borderDist);
                            baseCol = Color.Lerp(baseCol, borderCol, borderT * 0.85f);
                        }

                        // Outside edge smooth anti-aliasing
                        if (dist > -1.5f)
                        {
                            float alpha = Mathf.Clamp01(-dist / 1.5f);
                            baseCol.a = alpha;
                        }

                        pixels[idx] = baseCol;
                    }
                }
            }

            // High-resolution mascot compositing directly on block with tight bounding-box centering
            if (overlayTex != null)
            {
                int ovW = overlayTex.width;
                int ovH = overlayTex.height;
                Color[] ovPixels = overlayTex.GetPixels();

                // Find tight bounding box of non-transparent mascot pixels
                int minX = ovW, maxX = 0, minY = ovH, maxY = 0;
                bool hasPixels = false;
                for (int y = 0; y < ovH; y += 2)
                {
                    int row = y * ovW;
                    for (int x = 0; x < ovW; x += 2)
                    {
                        if (ovPixels[row + x].a > 0.05f)
                        {
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                            hasPixels = true;
                        }
                    }
                }

                if (hasPixels)
                {
                    float cx = (minX + maxX) * 0.5f;
                    float cy = (minY + maxY) * 0.5f;
                    float contentW = (maxX - minX + 1);
                    float contentH = (maxY - minY + 1);
                    float maxDim = Mathf.Max(contentW, contentH);

                    // Mascot fills 85% of block so character expression is clear and prominent
                    float targetDim = size * 0.85f;
                    float scale = targetDim / maxDim;

                    float invScale = 1f / scale;
                    float blockCenter = size * 0.5f;

                    for (int py = 0; py < size; py++)
                    {
                        float ovY = cy + (py - blockCenter) * invScale;
                        if (ovY < 0f || ovY >= ovH) continue;
                        float v = ovY / ovH;
                        int rowIdx = py * size;

                        for (int px = 0; px < size; px++)
                        {
                            float ovX = cx + (px - blockCenter) * invScale;
                            if (ovX < 0f || ovX >= ovW) continue;
                            float u = ovX / ovW;

                            Color mc = overlayTex.GetPixelBilinear(u, v);
                            if (mc.a <= 0.001f) continue;

                            int idx = rowIdx + px;
                            Color bg = pixels[idx];
                            if (bg.a <= 0.001f) continue;

                            // Standard Porter-Duff alpha blend
                            float outA = mc.a + bg.a * (1f - mc.a);
                            if (outA > 0f)
                            {
                                Color outC = (mc * mc.a + bg * bg.a * (1f - mc.a)) / outA;
                                outC.a = Mathf.Min(outA, bg.a);
                                pixels[idx] = outC;
                            }
                        }
                    }
                }
            }

            tex.SetPixels(pixels);
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

        public static Sprite GetOrCreatePinkMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Pink_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            path = $"{Folder}/Jelly_Mascot_Left_Pop.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateMascotSprite();
        }

        public static Sprite GetOrCreateMintMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Mint_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            path = $"{Folder}/Jelly_Mascot_Mint.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateMascotSprite();
        }

        public static Sprite GetOrCreateGoldMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Gold_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            EnsureMascotsCutout();
            path = $"{Folder}/Jelly_Mascot_Gold.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateCrownSprite();
        }

        public static Sprite GetOrCreatePurpleMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Purple_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            EnsureMascotsCutout();
            path = $"{Folder}/Jelly_Mascot_Purple.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateDiceSprite();
        }

        public static Sprite GetOrCreateSpecialMascotSprite()
        {
            EnsureFolder();
            EnsureSpecialMascotCutout();
            string path = $"{Folder}/Block_Special_Mascot.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateMascotSprite();
        }

        public static Sprite GetOrCreateBlueMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Blue_Mascot.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateMintMascotSprite();
        }

        public static Sprite GetOrCreateBerryMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Berry_Mascot.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreatePinkMascotSprite();
        }

        public static Sprite GetOrCreateLemonMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Lemon_Mascot.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateGoldMascotSprite();
        }

        public static Sprite GetOrCreateCloudMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Cloud_Mascot.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateSpecialMascotSprite();
        }

        public static Sprite GetOrCreateCodexCardFrameSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Codex_Card_Frame.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateCuteCardSprite();
        }

        public static Sprite GetOrCreateBadgeCommonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Badge_Common.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateTabPillSprite();
        }

        public static Sprite GetOrCreateBadgeRareSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Badge_Rare.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateTabPillSprite();
        }

        public static Sprite GetOrCreateBadgeSpecialSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Badge_Special.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateTabPillSprite();
        }

        public static Sprite GetOrCreateStarActiveSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Star_Active.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateFireworksSparkleStarSprite();
        }

        public static Sprite GetOrCreateStarEmptySprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Star_Empty.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateFireworksSparkleStarSprite();
        }

        public static Sprite GetOrCreateSideWingLeftSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/SideWing_Pastel_Left.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateSideWingRightSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/SideWing_Pastel_Right.jpg";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateBackgroundSprite();
        }

        public static Sprite GetOrCreateSideWingSkyGradientSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/SideWing_Sky_Gradient.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 32;
            int height = 512;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color topColor = new Color(0.12f, 0.10f, 0.22f); // Deep Lavender Night
            Color midColor = new Color(0.38f, 0.25f, 0.48f); // Soft Dusk Mauve
            Color botColor = new Color(0.95f, 0.68f, 0.65f); // Warm Peach Dusk Glow

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

            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }

            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateSideWingBorderSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/SideWing_Inner_Border.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 64;
            int height = 128;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color glowLavender = new Color(0.92f, 0.82f, 1f, 0.85f);
            Color glowPink = new Color(1f, 0.80f, 0.92f, 0.70f);
            Color shadowCol = new Color(0.08f, 0.06f, 0.16f); // Deep rich ambient shadow

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c;
                    if (x == 32)
                    {
                        c = new Color(1f, 1f, 1f, 0.95f); // Bright core
                    }
                    else if (x == 31 || x == 33)
                    {
                        c = glowLavender;
                    }
                    else if (x == 30 || x == 34)
                    {
                        c = glowPink;
                    }
                    else if (x < 30)
                    {
                        // Outer wing side shadow
                        float t = (float)x / 30f;
                        float a = Mathf.Pow(t, 2.2f) * 0.42f;
                        c = new Color(shadowCol.r, shadowCol.g, shadowCol.b, a);
                    }
                    else
                    {
                        // Inner central game side shadow
                        float t = (63f - x) / 29f;
                        float a = Mathf.Pow(t, 2.2f) * 0.38f;
                        c = new Color(shadowCol.r, shadowCol.g, shadowCol.b, a);
                    }

                    tex.SetPixel(x, y, c);
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
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }

            return ForceGetOrImportSingleSprite(path);
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
            string gold = $"{Folder}/Block_Gold_Mascot.png";
            string dstGold = $"{Folder}/Jelly_Mascot_Gold.png";
            if (File.Exists(gold) && !File.Exists(dstGold))
            {
                File.Copy(gold, dstGold, true);
                AssetDatabase.ImportAsset(dstGold, ImportAssetOptions.ForceUpdate);
                ForceGetOrImportSingleSprite(dstGold);
            }

            string purple = $"{Folder}/Block_Purple_Mascot.png";
            string dstPurple = $"{Folder}/Jelly_Mascot_Purple.png";
            if (File.Exists(purple) && !File.Exists(dstPurple))
            {
                File.Copy(purple, dstPurple, true);
                AssetDatabase.ImportAsset(dstPurple, ImportAssetOptions.ForceUpdate);
                ForceGetOrImportSingleSprite(dstPurple);
            }

            EnsureSpecialMascotCutout();
        }

        public static void EnsureSpecialMascotCutout()
        {
            EnsureFolder();
            string backupFolder = "Assets/Textures/BlockBlastCute_Backup";
            if (!Directory.Exists(backupFolder)) Directory.CreateDirectory(backupFolder);

            string rawJpg = $"{Folder}/Block_Special_Mascot_Raw.jpg";
            string dstPng = $"{Folder}/Block_Special_Mascot.png";
            string dstBackupPng = $"{backupFolder}/Block_Special_Mascot.png";
            string dstJelly = $"{Folder}/Jelly_Mascot_Special.png";

            if (File.Exists(rawJpg) && (!File.Exists(dstPng) || !File.Exists(dstBackupPng)))
            {
                byte[] rawBytes = File.ReadAllBytes(rawJpg);
                Texture2D srcTex = new Texture2D(2, 2);
                srcTex.LoadImage(rawBytes);

                int sw = srcTex.width;
                int sh = srcTex.height;
                Color[] srcPixels = srcTex.GetPixels();

                // BFS flood-fill from border
                bool[] isBg = new bool[sw * sh];
                Queue<int> q = new Queue<int>();

                for (int x = 0; x < sw; x++)
                {
                    TryEnqueueBg(x, 0, sw, sh, srcPixels, isBg, q);
                    TryEnqueueBg(x, sh - 1, sw, sh, srcPixels, isBg, q);
                }
                for (int y = 0; y < sh; y++)
                {
                    TryEnqueueBg(0, y, sw, sh, srcPixels, isBg, q);
                    TryEnqueueBg(sw - 1, y, sw, sh, srcPixels, isBg, q);
                }

                int[] dx = { 0, 0, 1, -1 };
                int[] dy = { 1, -1, 0, 0 };

                while (q.Count > 0)
                {
                    int curr = q.Dequeue();
                    int cx = curr % sw;
                    int cy = curr / sw;

                    for (int k = 0; k < 4; k++)
                    {
                        int nx = cx + dx[k];
                        int ny = cy + dy[k];
                        if (nx >= 0 && nx < sw && ny >= 0 && ny < sh)
                        {
                            int nidx = ny * sw + nx;
                            if (!isBg[nidx])
                            {
                                Color c = srcPixels[nidx];
                                if (c.r >= 0.93f && c.g >= 0.93f && c.b >= 0.93f)
                                {
                                    isBg[nidx] = true;
                                    q.Enqueue(nidx);
                                }
                            }
                        }
                    }
                }

                float[] alphaMap = new float[sw * sh];
                for (int i = 0; i < sw * sh; i++)
                {
                    if (isBg[i])
                    {
                        alphaMap[i] = 0f;
                    }
                    else
                    {
                        int cx = i % sw;
                        int cy = i / sw;
                        Color c = srcPixels[i];
                        float minDiff = 1f - Mathf.Min(c.r, Mathf.Min(c.g, c.b));

                        bool adjacentBg = false;
                        for (int k = 0; k < 4; k++)
                        {
                            int nx = cx + dx[k];
                            int ny = cy + dy[k];
                            if (nx >= 0 && nx < sw && ny >= 0 && ny < sh)
                            {
                                if (isBg[ny * sw + nx]) { adjacentBg = true; break; }
                            }
                        }

                        if (adjacentBg && minDiff < 0.18f)
                        {
                            alphaMap[i] = Mathf.Clamp01(minDiff / 0.18f);
                        }
                        else
                        {
                            alphaMap[i] = 1f;
                        }
                    }
                }

                int targetSize = 2048;
                Texture2D outTex = new Texture2D(targetSize, targetSize, TextureFormat.RGBA32, false);
                Color[] outPixels = new Color[targetSize * targetSize];

                float step = 1f / (targetSize - 1);
                for (int y = 0; y < targetSize; y++)
                {
                    float v = y * step;
                    for (int x = 0; x < targetSize; x++)
                    {
                        float u = x * step;
                        float fx = u * (sw - 1);
                        float fy = v * (sh - 1);
                        int x0 = Mathf.FloorToInt(fx);
                        int x1 = Mathf.Min(x0 + 1, sw - 1);
                        int y0 = Mathf.FloorToInt(fy);
                        int y1 = Mathf.Min(y0 + 1, sh - 1);
                        float tx = fx - x0;
                        float ty = fy - y0;

                        float a00 = alphaMap[y0 * sw + x0];
                        float a10 = alphaMap[y0 * sw + x1];
                        float a01 = alphaMap[y1 * sw + x0];
                        float a11 = alphaMap[y1 * sw + x1];
                        float a = Mathf.Lerp(Mathf.Lerp(a00, a10, tx), Mathf.Lerp(a01, a11, tx), ty);

                        Color c = srcTex.GetPixelBilinear(u, v);
                        if (a <= 0.001f)
                        {
                            outPixels[y * targetSize + x] = Color.clear;
                        }
                        else
                        {
                            if (a < 0.99f)
                            {
                                float invA = 1f - a;
                                float r = Mathf.Clamp01((c.r - invA) / a);
                                float g = Mathf.Clamp01((c.g - invA) / a);
                                float b = Mathf.Clamp01((c.b - invA) / a);
                                c = new Color(r, g, b, a);
                            }
                            else
                            {
                                c.a = 1f;
                            }
                            outPixels[y * targetSize + x] = c;
                        }
                    }
                }

                outTex.SetPixels(outPixels);
                outTex.Apply();

                byte[] pngBytes = outTex.EncodeToPNG();
                SafeWriteAllBytes(dstPng, pngBytes);
                SafeWriteAllBytes(dstBackupPng, pngBytes);
                SafeWriteAllBytes(dstJelly, pngBytes);

                AssetDatabase.ImportAsset(dstPng, ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset(dstBackupPng, ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset(dstJelly, ImportAssetOptions.ForceUpdate);

                SetSpriteImportSettings(dstPng, 2048);
                SetSpriteImportSettings(dstBackupPng, 2048);
                SetSpriteImportSettings(dstJelly, 2048);
            }
        }

        private static void TryEnqueueBg(int x, int y, int sw, int sh, Color[] srcPixels, bool[] isBg, Queue<int> q)
        {
            int idx = y * sw + x;
            if (!isBg[idx])
            {
                Color c = srcPixels[idx];
                if (c.r >= 0.93f && c.g >= 0.93f && c.b >= 0.93f)
                {
                    isBg[idx] = true;
                    q.Enqueue(idx);
                }
            }
        }

        private static void SetSpriteImportSettings(string path, int maxSize)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.maxTextureSize = maxSize;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
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

        public static Sprite GetOrCreate3DRoundJellyButtonSprite(string name, Color mainCol, int size = 128)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";

            Texture2D tex = Generate3DRoundJellyButtonTexture(size, mainCol);
            byte[] bytes = tex.EncodeToPNG();
            SafeWriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritesheet = new SpriteMetaData[0];
                importer.spriteBorder = Vector4.zero;
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
            string path = $"{Folder}/Block_Pink_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            path = $"{Folder}/Jelly_Mascot_Smile.png";
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

        // 3. 3D Glossy Jelly / Marshmallow Pastel Button (9-sliceable, completely clean, adorable)
        private static Texture2D Generate3DJellyButtonTexture(int width, int height, Color baseCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float bevelHeight = 8f;
            float radius = (height - bevelHeight) * 0.46f;

            // Convert baseCol to HSV to compute harmonious, sweet tone-on-tone colors without ANY muddy blacks!
            Color.RGBToHSV(baseCol, out float h, out float s, out float v);

            // Rich tone-on-tone base cushion shadow (richer saturation, slightly deeper value, NEVER black)
            Color shadowCol = Color.HSVToRGB(h, Mathf.Clamp01(s * 1.25f + 0.10f), Mathf.Clamp01(v * 0.80f));
            // Soft outer border/stroke for clean separation against light backgrounds
            Color strokeCol = Color.HSVToRGB(h, Mathf.Clamp01(s * 1.35f + 0.15f), Mathf.Clamp01(v * 0.72f));
            // Milky cream top highlight
            Color topGlow = Color.Lerp(baseCol, Color.white, 0.16f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Top Face Pill
                    float topCx = Mathf.Clamp(x, radius, width - radius);
                    float topCy = Mathf.Clamp(y, radius + bevelHeight, height - radius);
                    float topDist = Vector2.Distance(new Vector2(x, y), new Vector2(topCx, topCy));

                    // Bottom Base Extrusion
                    float extY = y + bevelHeight;
                    float extCy = Mathf.Clamp(extY, radius + bevelHeight, height - radius);
                    float extDist = Vector2.Distance(new Vector2(x, extY), new Vector2(topCx, extCy));

                    // 1. Calculate Base Cushion Layer
                    Color botColor = shadowCol;
                    float botAlpha = 0f;
                    if (y < radius + bevelHeight && extDist <= radius + 1.2f)
                    {
                        botAlpha = Mathf.Clamp01((radius - extDist) / 1.2f);
                        if (extDist > radius - 2.0f)
                        {
                            float strokeT = (extDist - (radius - 2.0f)) / 2.0f;
                            botColor = Color.Lerp(shadowCol, strokeCol, strokeT * 0.70f);
                        }
                    }

                    // 2. Calculate Top Face Layer
                    Color topColor = Color.clear;
                    float topAlpha = 0f;
                    if (topDist <= radius + 1.2f)
                    {
                        topAlpha = Mathf.Clamp01((radius - topDist) / 1.2f);

                        float faceNormY = Mathf.Clamp01((y - bevelHeight) / (height - bevelHeight));
                        topColor = Color.Lerp(baseCol, topGlow, Mathf.SmoothStep(0f, 1f, faceNormY));

                        // Soft top rim highlight (gentle gloss along top curve)
                        if (y > height - 16 && topDist < radius - 1.5f)
                        {
                            float rimT = Mathf.Clamp01((y - (height - 16)) / 14f);
                            topColor = Color.Lerp(topColor, Color.white, rimT * 0.25f);
                        }

                        // Soft outer stroke on face edge
                        if (topDist > radius - 2.0f)
                        {
                            float strokeT = (topDist - (radius - 2.0f)) / 2.0f;
                            topColor = Color.Lerp(topColor, strokeCol, strokeT * 0.55f);
                        }
                    }

                    // 3. Composite Top Face over Base Cushion
                    Color finalCol;
                    if (topAlpha >= 1f)
                    {
                        finalCol = topColor;
                    }
                    else if (topAlpha > 0f)
                    {
                        if (botAlpha > 0f && y <= topCy)
                        {
                            finalCol = Color.Lerp(botColor, topColor, topAlpha);
                            finalCol.a = Mathf.Max(botAlpha, topAlpha);
                        }
                        else
                        {
                            finalCol = topColor;
                            finalCol.a = topAlpha;
                        }
                    }
                    else if (botAlpha > 0f)
                    {
                        finalCol = botColor;
                        finalCol.a = botAlpha;
                    }
                    else
                    {
                        finalCol = Color.clear;
                    }

                    tex.SetPixel(x, y, finalCol);
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D Generate3DRoundJellyButtonTexture(int size, Color baseCol)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float bevelHeight = 10f;
            float radius = (size - bevelHeight) * 0.46f;
            Vector2 topCenter = new Vector2(size * 0.5f, size * 0.5f + bevelHeight * 0.5f);

            Color.RGBToHSV(baseCol, out float h, out float s, out float v);
            Color shadowCol = Color.HSVToRGB(h, Mathf.Clamp01(s * 1.25f + 0.10f), Mathf.Clamp01(v * 0.80f));
            Color strokeCol = Color.HSVToRGB(h, Mathf.Clamp01(s * 1.35f + 0.15f), Mathf.Clamp01(v * 0.72f));
            Color topGlow = Color.Lerp(baseCol, Color.white, 0.16f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float topDist = Vector2.Distance(new Vector2(x, y), topCenter);
                    float extY = y + bevelHeight;
                    float extDist = Vector2.Distance(new Vector2(x, extY), topCenter);

                    // Base Cushion
                    Color botColor = shadowCol;
                    float botAlpha = 0f;
                    if (y < topCenter.y && extDist <= radius + 1.2f)
                    {
                        botAlpha = Mathf.Clamp01((radius - extDist) / 1.2f);
                        if (extDist > radius - 2.0f)
                        {
                            float strokeT = (extDist - (radius - 2.0f)) / 2.0f;
                            botColor = Color.Lerp(shadowCol, strokeCol, strokeT * 0.70f);
                        }
                    }

                    // Top Face
                    Color topColor = Color.clear;
                    float topAlpha = 0f;
                    if (topDist <= radius + 1.2f)
                    {
                        topAlpha = Mathf.Clamp01((radius - topDist) / 1.2f);
                        float faceNormY = Mathf.Clamp01((y - bevelHeight) / (size - bevelHeight));
                        topColor = Color.Lerp(baseCol, topGlow, Mathf.SmoothStep(0f, 1f, faceNormY));

                        if (y > size - 20 && topDist < radius - 2f)
                        {
                            float rimT = Mathf.Clamp01((y - (size - 20)) / 18f);
                            topColor = Color.Lerp(topColor, Color.white, rimT * 0.25f);
                        }

                        if (topDist > radius - 2.0f)
                        {
                            float strokeT = (topDist - (radius - 2.0f)) / 2.0f;
                            topColor = Color.Lerp(topColor, strokeCol, strokeT * 0.55f);
                        }
                    }

                    // Composite
                    Color finalCol;
                    if (topAlpha >= 1f)
                    {
                        finalCol = topColor;
                    }
                    else if (topAlpha > 0f)
                    {
                        if (botAlpha > 0f && y <= topCenter.y)
                        {
                            finalCol = Color.Lerp(botColor, topColor, topAlpha);
                            finalCol.a = Mathf.Max(botAlpha, topAlpha);
                        }
                        else
                        {
                            finalCol = topColor;
                            finalCol.a = topAlpha;
                        }
                    }
                    else if (botAlpha > 0f)
                    {
                        finalCol = botColor;
                        finalCol.a = botAlpha;
                    }
                    else
                    {
                        finalCol = Color.clear;
                    }

                    tex.SetPixel(x, y, finalCol);
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
            string newKoPath = $"{Folder}/logo - korea-Photoroom.png";
            if (File.Exists(newKoPath)) return ForceGetOrImportSingleSprite(newKoPath);
            string path = $"{Folder}/Jelly_Mallang_Logo.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateLanguageLogoSprite(BlockBlast.GameLanguage lang)
        {
            EnsureFolder();
            string fileName;
            switch (lang)
            {
                case BlockBlast.GameLanguage.KO:
                    fileName = "logo - korea-Photoroom.png";
                    break;
                case BlockBlast.GameLanguage.EN:
                    fileName = "logo - English-Photoroom.png";
                    break;
                case BlockBlast.GameLanguage.JA:
                    fileName = "logo - japen-Photoroom.png";
                    break;
                case BlockBlast.GameLanguage.ZH:
                    fileName = "logo - chinese-Photoroom.png";
                    break;
                default:
                    fileName = "logo - korea-Photoroom.png";
                    break;
            }

            string path = $"{Folder}/{fileName}";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }
            return GetOrCreateMallangBlastLogoSprite();
        }

        public static Sprite[] GetOrCreateAllLanguageLogoSprites()
        {
            return new Sprite[]
            {
                GetOrCreateLanguageLogoSprite(BlockBlast.GameLanguage.KO),
                GetOrCreateLanguageLogoSprite(BlockBlast.GameLanguage.EN),
                GetOrCreateLanguageLogoSprite(BlockBlast.GameLanguage.JA),
                GetOrCreateLanguageLogoSprite(BlockBlast.GameLanguage.ZH)
            };
        }

        // ==========================================
        // 10-1. Mallang Games Studio Logo
        // ==========================================
        public static Sprite GetOrCreateMallangGamesStudioLogoSprite()
        {
            string path = "Assets/Textures/MallangGames_Studio_Logo.png";
            if (File.Exists(path))
            {
                return ForceGetOrImportSingleSprite(path);
            }
            return GetOrCreateMallangBlastLogoSprite();
        }

        // ==========================================
        // 11. Left Popping Mascot (Strawberry Smile)
        // ==========================================
        public static Sprite GetOrCreateLeftPoppingMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Pink_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            path = $"{Folder}/Jelly_Mascot_Left_Pop.png";
            return ForceGetOrImportSingleSprite(path);
        }

        // ==========================================
        // 12. Right Popping Mascot (Mint Soda)
        // ==========================================
        public static Sprite GetOrCreateRightPoppingMascotSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Block_Mint_Mascot.png";
            if (File.Exists(path)) return ForceGetOrImportSingleSprite(path);
            path = $"{Folder}/Jelly_Mascot_Right_Pop.png";
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
            return GetOrCreateJellyButtonPinkSprite(false);
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

        private static Sprite SaveAndConfigureSprite(string path, Texture2D tex, Vector4 border, bool isSliced = true)
        {
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
                importer.maxTextureSize = 2048;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                if (isSliced)
                {
                    importer.spriteBorder = border;
                }
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Texture2D GenerateBeveledPlaqueTexture(int width, int height, float radius, Color borderCol, Color fillTop, Color fillBot, Color shadowCol, bool addTopGloss = false)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float r = radius;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cx = Mathf.Clamp(x, r, width - 1 - r);
                    float cy = Mathf.Clamp(y, r, height - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 3f;
                    float scy = Mathf.Clamp(sy, r, height - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 4f)
                        {
                            float sa = Mathf.Clamp01((r + 4f - sd) / 4f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        tex.SetPixel(x, y, new Color(borderCol.r, borderCol.g, borderCol.b, a));
                    }
                    else if (d > r - 5.5f)
                    {
                        tex.SetPixel(x, y, borderCol);
                    }
                    else
                    {
                        float v = (float)y / height;
                        Color c = Color.Lerp(fillBot, fillTop, v);

                        if (addTopGloss && y > height * 0.5f && d <= r - 6f)
                        {
                            float gy = ((float)y - height * 0.5f) / (height * 0.5f);
                            c = Color.Lerp(c, Color.white, gy * 0.35f);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateRichCandyCardTexture(
            int width, int height, float radius,
            Color outerRim, Color bevelTop, Color bevelBot, Color innerTrim,
            Color fillTop, Color fillBot, Color shadowCol, bool addTopGloss = true)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float r = radius;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cx = Mathf.Clamp(x, r, width - 1 - r);
                    float cy = Mathf.Clamp(y, r, height - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 4f;
                    float scy = Mathf.Clamp(sy, r, height - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 6f)
                        {
                            float sa = Mathf.Clamp01((r + 6f - sd) / 6f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        tex.SetPixel(x, y, new Color(outerRim.r, outerRim.g, outerRim.b, a));
                    }
                    else if (d > r - 4f)
                    {
                        tex.SetPixel(x, y, outerRim);
                    }
                    else if (d > r - 16f)
                    {
                        float tBevel = (r - 4f - d) / 12f;
                        float v = (float)y / height;
                        Color baseBevel = Color.Lerp(bevelBot, bevelTop, v);

                        float nx = (d > 0.001f) ? (x - cx) / d : 0f;
                        float ny = (d > 0.001f) ? (y - cy) / d : 0f;
                        float light3D = -nx * 0.35f + ny * 0.55f;

                        float arch = Mathf.Sin(tBevel * Mathf.PI);
                        Color c = Color.Lerp(baseBevel, Color.white, Mathf.Clamp01(light3D * 0.45f + arch * 0.25f));
                        if (light3D < -0.1f)
                        {
                            c = Color.Lerp(c, outerRim, Mathf.Clamp01(-light3D * 0.5f));
                        }
                        tex.SetPixel(x, y, c);
                    }
                    else if (d > r - 18.5f)
                    {
                        tex.SetPixel(x, y, innerTrim);
                    }
                    else
                    {
                        float v = (float)y / height;
                        Color c = Color.Lerp(fillBot, fillTop, v);

                        if (d > r - 22f)
                        {
                            float edgeDark = (d - (r - 22f)) / 3.5f;
                            c = Color.Lerp(c, innerTrim, edgeDark * 0.25f);
                        }

                        if (addTopGloss && y > height * 0.55f && d <= r - 20f)
                        {
                            float gy = ((float)y - height * 0.55f) / (height * 0.45f);
                            c = Color.Lerp(c, Color.white, gy * 0.22f);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateShopActionButtonTexture(
            int width, int height, float radius,
            Color outerRim, Color bevelTop, Color bevelBot,
            Color fillTop, Color fillBot, Color shadowCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float r = radius;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cx = Mathf.Clamp(x, r, width - 1 - r);
                    float cy = Mathf.Clamp(y, r, height - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 3f;
                    float scy = Mathf.Clamp(sy, r, height - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 4.5f)
                        {
                            float sa = Mathf.Clamp01((r + 4.5f - sd) / 4.5f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        tex.SetPixel(x, y, new Color(outerRim.r, outerRim.g, outerRim.b, a));
                    }
                    else if (d > r - 3.5f)
                    {
                        tex.SetPixel(x, y, outerRim);
                    }
                    else if (d > r - 9f)
                    {
                        float v = (float)y / height;
                        Color c = Color.Lerp(bevelBot, bevelTop, v);

                        float ny = (d > 0.001f) ? (y - cy) / d : 0f;
                        if (ny > 0.1f) c = Color.Lerp(c, Color.white, ny * 0.45f);
                        else if (ny < -0.1f) c = Color.Lerp(c, outerRim, -ny * 0.35f);

                        tex.SetPixel(x, y, c);
                    }
                    else if (d > r - 10.5f)
                    {
                        Color hl = Color.Lerp(bevelTop, Color.white, 0.6f);
                        tex.SetPixel(x, y, hl);
                    }
                    else
                    {
                        float v = (float)y / height;
                        Color c = Color.Lerp(fillBot, fillTop, v);

                        if (y > height * 0.5f)
                        {
                            float gy = ((float)y - height * 0.5f) / (height * 0.5f);
                            c = Color.Lerp(c, Color.white, gy * 0.30f);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateCircleKnobTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float r = size * 0.44f;
            Color shadowCol = new Color(0.15f, 0.08f, 0.25f, 0.28f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float sd = Vector2.Distance(new Vector2(x, y + 3f), new Vector2(cx, cy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 4f)
                        {
                            float sa = Mathf.Clamp01((r + 4f - sd) / 4f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                    else if (d > r - 4f)
                    {
                        tex.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        float nx = (x - cx) / r;
                        float ny = (y - cy) / r;
                        float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - nx * nx - ny * ny));

                        Vector3 lightDir = new Vector3(-0.4f, 0.6f, 0.7f).normalized;
                        float diff = Mathf.Max(0f, nx * lightDir.x + ny * lightDir.y + nz * lightDir.z);

                        Color topPink = new Color(1.0f, 0.45f, 0.68f);
                        Color botPink = new Color(0.88f, 0.16f, 0.45f);
                        Color col = Color.Lerp(botPink, topPink, diff);

                        Vector3 normal = new Vector3(nx, ny, nz);
                        Vector3 halfVec = (lightDir + Vector3.forward).normalized;
                        float spec = Mathf.Pow(Mathf.Max(0f, Vector3.Dot(normal, halfVec)), 16f);
                        col = Color.Lerp(col, Color.white, spec * 0.75f);

                        tex.SetPixel(x, y, col);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateCircleFrameTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float outerR = size * 0.46f;
            float innerR = size * 0.35f;
            Color shadowCol = new Color(0.15f, 0.08f, 0.25f, 0.25f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float sd = Vector2.Distance(new Vector2(x, y + 4f), new Vector2(cx, cy));

                    if (d > outerR + 1.5f)
                    {
                        if (sd <= outerR + 5f)
                        {
                            float sa = Mathf.Clamp01((outerR + 5f - sd) / 5f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > outerR - 1.5f)
                    {
                        float a = Mathf.Clamp01(outerR + 1.5f - d);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                    else if (d < innerR - 1.5f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (d < innerR + 1.5f)
                    {
                        float a = Mathf.Clamp01(d - (innerR - 1.5f));
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                    else
                    {
                        float t = (float)(y + x) / (size * 2f);
                        Color c1 = new Color(1f, 0.55f, 0.75f);
                        Color c2 = new Color(0.65f, 0.45f, 0.95f);
                        Color ringCol = Color.Lerp(c1, c2, t);

                        if (d > outerR - 4f || d < innerR + 4f)
                        {
                            ringCol = Color.Lerp(ringCol, Color.white, 0.7f);
                        }

                        tex.SetPixel(x, y, ringCol);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        // ==========================================
        // DEDICATED CUSTOMIZABLE UI SPRITES
        // ==========================================

        public static Sprite GetOrCreateIngameScoreBoxSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Ingame_Score_Box.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.51f, 0.33f, 0.77f, 1f); // Soft deep lavender
            Color fillTop = new Color(1.0f, 0.99f, 1.0f, 0.98f);
            Color fillBot = new Color(0.95f, 0.91f, 0.99f, 0.98f); // Sweet lavender cream
            Color shadow = new Color(0.12f, 0.08f, 0.22f, 0.28f);

            Texture2D tex = GenerateBeveledPlaqueTexture(256, 160, 38f, border, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(44, 44, 44, 44), true);
        }

        public static Sprite GetOrCreateIngameBestBoxSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Ingame_Best_Box.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.87f, 0.56f, 0.08f, 1f); // Warm golden caramel
            Color fillTop = new Color(1.0f, 0.99f, 0.97f, 0.98f);
            Color fillBot = new Color(1.0f, 0.93f, 0.76f, 0.98f); // Warm honey cream
            Color shadow = new Color(0.18f, 0.12f, 0.05f, 0.28f);

            Texture2D tex = GenerateBeveledPlaqueTexture(256, 160, 38f, border, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(44, 44, 44, 44), true);
        }

        public static Sprite GetOrCreateSettingsModalCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Modal_Settings_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.42f, 0.28f, 0.68f, 1f);     // Deep royal pastel violet
            Color bevTop = new Color(0.78f, 0.65f, 0.98f, 1f);    // Dreamy lavender candy
            Color bevBot = new Color(0.56f, 0.42f, 0.84f, 1f);    // Sweet plum amethyst
            Color trim = new Color(0.92f, 0.88f, 1.0f, 1f);       // Delicate lilac highlight
            Color fillTop = new Color(1.0f, 0.99f, 1.0f, 1f);     // Pure porcelain top
            Color fillBot = new Color(0.96f, 0.94f, 0.99f, 1f);    // Sweet lavender cream
            Color shadow = new Color(0.12f, 0.08f, 0.22f, 0.25f);

            Texture2D tex = GenerateRichCandyCardTexture(512, 512, 56f, outer, bevTop, bevBot, trim, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(64, 64, 64, 64), true);
        }

        public static Sprite GetOrCreateProfileModalCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Modal_Profile_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.76f, 0.28f, 0.52f, 1f);     // Sweet berry magenta
            Color bevTop = new Color(1.0f, 0.62f, 0.78f, 1f);     // Pastel berry rose
            Color bevBot = new Color(0.88f, 0.42f, 0.62f, 1f);    // Strawberry blossom
            Color trim = new Color(1.0f, 0.90f, 0.95f, 1f);       // Rosy milk highlight
            Color fillTop = new Color(1.0f, 0.99f, 1.0f, 1f);
            Color fillBot = new Color(1.0f, 0.95f, 0.97f, 1f);
            Color shadow = new Color(0.18f, 0.08f, 0.15f, 0.24f);

            Texture2D tex = GenerateRichCandyCardTexture(512, 640, 56f, outer, bevTop, bevBot, trim, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(64, 64, 64, 64), true);
        }

        public static Sprite GetOrCreateShopModalCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Modal_Shop_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.85f, 0.22f, 0.44f, 1f);     // Sweet strawberry magenta rim
            Color bevTop = new Color(1.0f, 0.56f, 0.72f, 1f);     // Candy pink 3D bevel
            Color bevBot = new Color(0.95f, 0.32f, 0.54f, 1f);    // Vibrant rose candy
            Color trim = new Color(1.0f, 0.88f, 0.58f, 1f);       // Warm champagne gold ribbon trim
            Color fillTop = new Color(1.0f, 1.0f, 1.0f, 1f);      // Clean porcelain top
            Color fillBot = new Color(1.0f, 0.96f, 0.98f, 1f);    // Sweet porcelain cream
            Color shadow = new Color(0.22f, 0.06f, 0.14f, 0.25f);

            Texture2D tex = GenerateRichCandyCardTexture(512, 640, 56f, outer, bevTop, bevBot, trim, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(64, 64, 64, 64), true);
        }

        public static Sprite GetOrCreateHelpModalCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Modal_Help_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.82f, 0.48f, 0.05f, 1f);     // Warm honey amber rim
            Color bevTop = new Color(1.0f, 0.82f, 0.35f, 1f);     // Sunshine butterscotch 3D bevel
            Color bevBot = new Color(0.96f, 0.65f, 0.12f, 1f);    // Golden caramel candy
            Color trim = new Color(1.0f, 0.96f, 0.82f, 1f);       // Vanilla cream pinstripe
            Color fillTop = new Color(1.0f, 0.99f, 0.97f, 1f);    // Warm vanilla porcelain
            Color fillBot = new Color(1.0f, 0.97f, 0.91f, 1f);    // Honey cream
            Color shadow = new Color(0.22f, 0.14f, 0.04f, 0.25f);

            Texture2D tex = GenerateRichCandyCardTexture(512, 640, 56f, outer, bevTop, bevBot, trim, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(64, 64, 64, 64), true);
        }

        public static Sprite GetOrCreateShopEquipButtonSprite(bool force = false)
        {
            return GetOrCreateJellyButtonPinkSprite(force);
        }

        public static Sprite GetOrCreateShopEquippedButtonSprite(bool force = false)
        {
            return GetOrCreateJellyButtonMintSprite(force);
        }

        public static Sprite GetOrCreateShopItemCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Item_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.78f, 0.74f, 0.89f, 1f); // Delicate lavender rim
            Color fillTop = new Color(0.99f, 0.98f, 1.0f, 0.98f);
            Color fillBot = new Color(0.95f, 0.93f, 0.98f, 0.98f);
            Color shadow = new Color(0.12f, 0.08f, 0.20f, 0.16f);

            Texture2D tex = GenerateBeveledPlaqueTexture(512, 160, 36f, border, fillTop, fillBot, shadow, addTopGloss: false);
            return SaveAndConfigureSprite(path, tex, new Vector4(44, 32, 44, 32), true);
        }

        public static Sprite GetOrCreateInputPillSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Input_Pill.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.78f, 0.75f, 0.89f, 1f);
            Color fillTop = new Color(0.97f, 0.96f, 0.99f, 1f);
            Color fillBot = Color.white;
            Color shadow = new Color(0.12f, 0.08f, 0.20f, 0.14f);

            Texture2D tex = GenerateBeveledPlaqueTexture(256, 80, 34f, border, fillTop, fillBot, shadow, addTopGloss: false);
            return SaveAndConfigureSprite(path, tex, new Vector4(40, 24, 40, 24), true);
        }

        public static Sprite GetOrCreateSliderTrackSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Slider_Track.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.76f, 0.73f, 0.81f, 1f);
            Color fillTop = new Color(0.84f, 0.81f, 0.89f, 1f);
            Color fillBot = new Color(0.92f, 0.90f, 0.96f, 1f);
            Color shadow = new Color(0.15f, 0.10f, 0.25f, 0.18f);

            Texture2D tex = GenerateBeveledPlaqueTexture(256, 48, 20f, border, fillTop, fillBot, shadow, addTopGloss: false);
            return SaveAndConfigureSprite(path, tex, new Vector4(24, 20, 24, 20), true);
        }

        public static Sprite GetOrCreateSliderFillSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Slider_Fill.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(1.0f, 0.22f, 0.48f, 1f);
            Color fillTop = new Color(1.0f, 0.42f, 0.65f, 1f);
            Color fillBot = new Color(1.0f, 0.21f, 0.49f, 1f);
            Color shadow = new Color(0.5f, 0.05f, 0.20f, 0.25f);

            Texture2D tex = GenerateBeveledPlaqueTexture(256, 48, 20f, border, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(24, 20, 24, 20), true);
        }

        public static Sprite GetOrCreateSliderKnobSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Slider_Knob.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Texture2D tex = GenerateCircleKnobTexture(80);
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateAvatarCircleFrameSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Avatar_Circle_Frame.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Texture2D tex = GenerateCircleFrameTexture(256);
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateShopGameTabButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Tab_Shop_Game_Active.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateShopLobbyTabButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Tab_Shop_Lobby_Active.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateShopInactiveTabButtonSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Tab_Shop_Inactive.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateGameOverCardSprite()
        {
            EnsureFolder();
            string path = $"{Folder}/Game Over.png";
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[CuteBlockTextureGenerator] {path} not found!");
                return null;
            }

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; dirty = true; }
                if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; dirty = true; }
                if (importer.maxTextureSize < 2048) { importer.maxTextureSize = 4096; dirty = true; }
                if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; dirty = true; }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite GetOrCreateLobbyCoinBoxSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Lobby_Coin_Box.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            // Elegant, modern rounded rectangle (모서리가 둥근 네모)
            // Translucent dark glass with delicate luminous lavender border & gentle drop shadow
            Color border = new Color(0.72f, 0.60f, 0.92f, 0.60f); // Delicate luminous lavender border
            Color fillTop = new Color(0.18f, 0.12f, 0.30f, 0.82f); // Deep translucent royal night top
            Color fillBot = new Color(0.11f, 0.07f, 0.20f, 0.88f); // Deep cozy night bottom
            Color shadow = new Color(0.06f, 0.03f, 0.12f, 0.35f);  // Soft drop shadow

            // 256x128 with radius 28f => modern rounded rectangle (NOT a pill/capsule)
            Texture2D tex = GenerateBeveledPlaqueTexture(256, 128, 28f, border, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(36, 32, 36, 32), true);
        }

        public static Sprite GetOrCreateWhiteCircleSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Circle_White_Plate.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 2048;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float r = size * 0.48f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                    float alpha = Mathf.Clamp01(r - d + 0.5f);
                    if (alpha > 0f)
                    {
                        float edgeRatio = Mathf.Clamp01(d / r);
                        Color col = Color.Lerp(Color.white, new Color(0.985f, 0.985f, 1.0f), edgeRatio * edgeRatio);
                        col.a = alpha;
                        tex.SetPixel(x, y, col);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        /// <summary>
        /// Generates a stunning 2048x2048 high-res avatar sprite with a pure white circular plate background
        /// and the 2048x2048 lossless cutout mascot crisply centered on top!
        /// </summary>
        public static Sprite GetOrCreateHighResAvatarSprite(string avatarName, string mascotFileName, bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/{avatarName}.png";
            if (!force && File.Exists(path))
            {
                Sprite s = ForceGetOrImportSingleSprite(path);
                if (s != null) return s;
            }

            int size = 2048;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float rPlate = size * 0.48f;

            // 1. Draw smooth anti-aliased pure white circular plate
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                    float alpha = Mathf.Clamp01(rPlate - d + 0.5f);
                    if (alpha > 0f)
                    {
                        float edgeRatio = Mathf.Clamp01(d / rPlate);
                        Color col = Color.Lerp(Color.white, new Color(0.985f, 0.985f, 1.0f), edgeRatio * edgeRatio);
                        col.a = alpha;
                        pixels[y * size + x] = col;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            // 2. Load 2048 lossless mascot overlay from BlockBlastCute or BlockBlastCute_Backup
            string ovPath = $"{Folder}/{mascotFileName}";
            string backupPath = $"Assets/Textures/BlockBlastCute_Backup/{mascotFileName}";
            Texture2D mascotTex = null;
            if (File.Exists(ovPath))
            {
                byte[] rawBytes = File.ReadAllBytes(ovPath);
                mascotTex = new Texture2D(2, 2);
                mascotTex.LoadImage(rawBytes);
            }
            else if (File.Exists(backupPath))
            {
                byte[] rawBytes = File.ReadAllBytes(backupPath);
                mascotTex = new Texture2D(2, 2);
                mascotTex.LoadImage(rawBytes);
            }

            // 3. Composite 2048 mascot onto the white circular plate (clean 4-point supersampling)
            if (mascotTex != null)
            {
                float mascotScale = 0.88f; // Mascot fills 88% of plate nicely
                int mSize = Mathf.RoundToInt(size * mascotScale);
                int startX = (size - mSize) / 2;
                int startY = (size - mSize) / 2;
                float stepU = 1f / Mathf.Max(1, mSize - 1);
                float stepV = 1f / Mathf.Max(1, mSize - 1);

                for (int oy = 0; oy < mSize; oy++)
                {
                    for (int ox = 0; ox < mSize; ox++)
                    {
                        float u = ox * stepU;
                        float v = oy * stepV;
                        Color mc = mascotTex.GetPixelBilinear(u, v);
                        if (mc.a <= 0.001f) continue;

                        int px = startX + ox;
                        int py = startY + oy;
                        if (px >= 0 && px < size && py >= 0 && py < size)
                        {
                            int idx = py * size + px;
                            Color bg = pixels[idx];
                            // Standard alpha blend
                            float outA = mc.a + bg.a * (1f - mc.a);
                            if (outA > 0f)
                            {
                                Color outC = (mc * mc.a + bg * bg.a * (1f - mc.a)) / outA;
                                outC.a = outA;
                                pixels[idx] = outC;
                            }
                        }
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            SafeWriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.anisoLevel = 16;
                importer.maxTextureSize = 2048;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static Sprite[] GetOrCreateAllHighResAvatarSprites(bool force = false)
        {
            EnsureSpecialMascotCutout();
            return new Sprite[]
            {
                GetOrCreateHighResAvatarSprite("Avatar_Pink", "Block_Pink_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Mint", "Block_Mint_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Gold", "Block_Gold_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Purple", "Block_Purple_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Blue", "Block_Blue_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Berry", "Block_Berry_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Lemon", "Block_Lemon_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Cloud", "Block_Cloud_Mascot.png", force),
                GetOrCreateHighResAvatarSprite("Avatar_Special", "Block_Special_Mascot.png", force)
            };
        }

        public static Sprite GetOrCreateGoogleIconSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Icon_Google.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float rOuter = size * 0.46f;
            float rInner = size * 0.26f;
            float barH = (rOuter - rInner) * 0.95f;

            Color cBlue = new Color(0.26f, 0.52f, 0.96f, 1f);   // #4285F4
            Color cRed = new Color(0.92f, 0.26f, 0.21f, 1f);    // #EA4335
            Color cYellow = new Color(0.98f, 0.74f, 0.02f, 1f); // #FBBC05
            Color cGreen = new Color(0.20f, 0.66f, 0.33f, 1f);  // #34A853

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    // Horizontal blue bar: from cx (dx=0) to right edge (rOuter), centered at cy
                    bool inBar = (dx >= -1f && dx <= rOuter && Mathf.Abs(dy) <= barH * 0.5f);
                    float alphaBar = inBar ? Mathf.Clamp01(rOuter - dx + 0.5f) * Mathf.Clamp01(barH * 0.5f - Mathf.Abs(dy) + 0.5f) : 0f;

                    // Doughnut ring
                    float alphaRing = Mathf.Clamp01(rOuter - dist + 0.5f) * Mathf.Clamp01(dist - rInner + 0.5f);

                    float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg; // -180 to 180

                    // Top-right opening above the bar (0 to 45 deg) is cut out
                    bool inOpening = (angle > 0f && angle < 45f && dist > rInner);

                    if (inBar && (!inOpening || dy <= 0f || inBar))
                    {
                        // Clean bar rendering
                        float a = Mathf.Max(alphaBar, alphaRing);
                        tex.SetPixel(x, y, new Color(cBlue.r, cBlue.g, cBlue.b, a));
                        continue;
                    }

                    if (alphaRing <= 0f || inOpening)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color col;
                    if (angle >= 45f && angle <= 135f) col = cRed;
                    else if (angle > 135f || angle < -135f) col = cYellow;
                    else if (angle >= -135f && angle <= -42f) col = cGreen;
                    else col = cBlue; // -42 to 0 deg

                    col.a = alphaRing;
                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateGoogleLoginButtonSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Btn_Google_Login.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.82f, 0.84f, 0.90f, 1f);
            Color bevTop = new Color(1f, 1f, 1f, 1f);
            Color bevBot = new Color(0.96f, 0.97f, 0.98f, 1f);
            Color fillTop = new Color(1f, 1f, 1f, 1f);
            Color fillBot = new Color(0.97f, 0.98f, 1f, 1f);
            Color shadow = new Color(0.12f, 0.14f, 0.22f, 0.14f);

            Texture2D tex = GenerateShopActionButtonTexture(256, 96, 28f, outer, bevTop, bevBot, fillTop, fillBot, shadow);
            return SaveAndConfigureSprite(path, tex, new Vector4(32, 24, 32, 24), true);
        }

        public static Sprite GetOrCreateDiamondIconSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Diamond_Gem.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            // Gem key coordinates
            Vector2 pTopL = new Vector2(76f, 192f);
            Vector2 pTopR = new Vector2(180f, 192f);
            Vector2 pMidL = new Vector2(30f, 142f);
            Vector2 pMidR = new Vector2(226f, 142f);
            Vector2 pBottom = new Vector2(128f, 24f);

            Vector2 pTableL = new Vector2(94f, 152f);
            Vector2 pTableR = new Vector2(162f, 152f);
            Vector2 pCenter = new Vector2(128f, 118f);

            // Facet Colors (Sparkling Cyan / Aqua / Diamond Palette)
            Color colTable = new Color(0.76f, 0.96f, 1.0f, 1f);     // Pale ice cyan highlight
            Color colTopMid = new Color(0.55f, 0.90f, 1.0f, 1f);    // Crisp sky cyan
            Color colTopL = new Color(0.40f, 0.82f, 0.98f, 1f);      // Aqua blue
            Color colTopR = new Color(0.60f, 0.92f, 1.0f, 1f);      // Bright cyan highlight
            Color colCenter = new Color(0.35f, 0.78f, 0.98f, 1f);    // Radiant cyan
            Color colBotMidL = new Color(0.14f, 0.60f, 0.92f, 1f);   // Medium deep azure
            Color colBotMidR = new Color(0.20f, 0.70f, 0.96f, 1f);   // Vibrant sapphire cyan
            Color colBotL = new Color(0.08f, 0.44f, 0.78f, 1f);      // Deep sapphire
            Color colBotR = new Color(0.10f, 0.52f, 0.85f, 1f);      // Rich ocean azure

            // Point-in-triangle helper
            bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3, out float w1, out float w2, out float w3)
            {
                float denom = (v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y);
                if (Mathf.Abs(denom) < 0.0001f) { w1 = w2 = w3 = 0; return false; }
                w1 = ((v2.y - v3.y) * (pt.x - v3.x) + (v3.x - v2.x) * (pt.y - v3.y)) / denom;
                w2 = ((v3.y - v1.y) * (pt.x - v3.x) + (v1.x - v3.x) * (pt.y - v3.y)) / denom;
                w3 = 1f - w1 - w2;
                return (w1 >= -0.01f && w2 >= -0.01f && w3 >= -0.01f);
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pt = new Vector2(x, y);
                    Color pixelCol = Color.clear;
                    bool hit = false;
                    float w1, w2, w3;

                    // 1. Table Top Trapezoid: (pTopL, pTopR, pTableR, pTableL)
                    if (PointInTriangle(pt, pTopL, pTopR, pTableR, out w1, out w2, out w3))
                    {
                        pixelCol = colTable * w3 + colTopMid * (w1 + w2);
                        hit = true;
                    }
                    else if (PointInTriangle(pt, pTopL, pTableR, pTableL, out w1, out w2, out w3))
                    {
                        pixelCol = colTable * (w2 + w3) + colTopMid * w1;
                        hit = true;
                    }
                    // 2. Upper Left Triangle: (pTopL, pTableL, pMidL)
                    else if (PointInTriangle(pt, pTopL, pTableL, pMidL, out w1, out w2, out w3))
                    {
                        pixelCol = colTopL * (w1 + w3) + colTable * w2;
                        hit = true;
                    }
                    // 3. Upper Right Triangle: (pTopR, pMidR, pTableR)
                    else if (PointInTriangle(pt, pTopR, pMidR, pTableR, out w1, out w2, out w3))
                    {
                        pixelCol = colTopR * (w1 + w2) + colTable * w3;
                        hit = true;
                    }
                    // 4. Center Kite / Upper Center Triangle: (pTableL, pTableR, pCenter)
                    else if (PointInTriangle(pt, pTableL, pTableR, pCenter, out w1, out w2, out w3))
                    {
                        Color cKite = new Color(0.82f, 0.95f, 1.0f, 1f); // Sparkling white-cyan
                        pixelCol = Color.Lerp(cKite, colCenter, w3);
                        pixelCol.a = 1f;
                        hit = true;
                    }
                    // 5. Lower Center Left: (pTableL, pCenter, pBottom)
                    else if (PointInTriangle(pt, pTableL, pCenter, pBottom, out w1, out w2, out w3))
                    {
                        pixelCol = Color.Lerp(colBotMidL, colCenter, w2);
                        pixelCol.a = 1f;
                        hit = true;
                    }
                    // 6. Lower Center Right: (pTableR, pBottom, pCenter)
                    else if (PointInTriangle(pt, pTableR, pBottom, pCenter, out w1, out w2, out w3))
                    {
                        pixelCol = Color.Lerp(colBotMidR, colCenter, w3);
                        pixelCol.a = 1f;
                        hit = true;
                    }
                    // 7. Lower Left: (pMidL, pTableL, pBottom)
                    else if (PointInTriangle(pt, pMidL, pTableL, pBottom, out w1, out w2, out w3))
                    {
                        pixelCol = Color.Lerp(colBotL, colTopL, w2);
                        pixelCol.a = 1f;
                        hit = true;
                    }
                    // 8. Lower Right: (pMidR, pBottom, pTableR)
                    else if (PointInTriangle(pt, pMidR, pBottom, pTableR, out w1, out w2, out w3))
                    {
                        pixelCol = Color.Lerp(colBotR, colTopR, w3);
                        pixelCol.a = 1f;
                        hit = true;
                    }

                    if (hit)
                    {
                        pixelCol.a = 1f;
                        tex.SetPixel(x, y, pixelCol);
                    }
                    else
                    {
                        // Outer magical bloom aura
                        float dTip = Vector2.Distance(pt, pBottom);
                        float dMidL = Vector2.Distance(pt, pMidL);
                        float dMidR = Vector2.Distance(pt, pMidR);
                        float dTop = Vector2.Distance(pt, new Vector2(128, 192));
                        float minD = Mathf.Min(Mathf.Min(dTip, dMidL), Mathf.Min(dMidR, dTop));
                        if (minD < 22f)
                        {
                            float glow = Mathf.Pow(Mathf.Clamp01(1f - minD / 22f), 2.2f) * 0.45f;
                            tex.SetPixel(x, y, new Color(0.45f, 0.88f, 1f, glow));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                }
            }

            // Draw sparkling 4-pointed specular stars
            DrawSparkleStar(tex, 175, 190, 24, Color.white);
            DrawSparkleStar(tex, 62, 142, 15, new Color(1f, 1f, 1f, 0.9f));
            DrawSparkleStar(tex, 130, 60, 12, new Color(0.85f, 0.98f, 1f, 0.8f));

            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        private static void DrawSparkleStar(Texture2D tex, int cx, int cy, int radius, Color starCol)
        {
            int w = tex.width;
            int h = tex.height;
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px < 0 || px >= w || py < 0 || py >= h) continue;

                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > radius) continue;

                    // 4-point cross falloff
                    float cross = Mathf.Max(
                        Mathf.Clamp01(1f - Mathf.Abs(dx) / (float)radius) * Mathf.Clamp01(1f - Mathf.Abs(dy) / 2.5f),
                        Mathf.Clamp01(1f - Mathf.Abs(dy) / (float)radius) * Mathf.Clamp01(1f - Mathf.Abs(dx) / 2.5f)
                    );
                    float core = Mathf.Clamp01(1f - dist / 4f);
                    float intensity = Mathf.Clamp01(cross + core);
                    if (intensity > 0.05f)
                    {
                        Color existing = tex.GetPixel(px, py);
                        Color blended = Color.Lerp(existing, starCol, intensity);
                        blended.a = Mathf.Max(existing.a, intensity);
                        tex.SetPixel(px, py, blended);
                    }
                }
            }
        }

        public static Sprite GetOrCreateShopBannerNewMascotSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Banner_New_Mascot.png";
            string jpgSource = $"{Folder}/UI_Shop_Banner_New_Mascot.jpg";
            if (File.Exists(jpgSource))
            {
                byte[] raw = File.ReadAllBytes(jpgSource);
                Texture2D jTex = new Texture2D(2, 2);
                jTex.LoadImage(raw);
                SafeWriteAllBytes(path, jTex.EncodeToPNG());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                SetSpriteImportSettings(path, 2048);
                return ForceGetOrImportSingleSprite(path);
            }
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 1024;
            int height = 560;
            Texture2D bannerTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Rich twilight party gradient background
            Color cTop = new Color(0.92f, 0.32f, 0.58f, 1f);     // Coral berry pink
            Color cMid = new Color(0.48f, 0.18f, 0.62f, 1f);     // Vibrant violet
            Color cBot = new Color(0.16f, 0.08f, 0.35f, 1f);     // Deep royal twilight

            for (int y = 0; y < height; y++)
            {
                float tY = y / (float)height;
                Color rowBg = (tY > 0.5f) ? Color.Lerp(cMid, cTop, (tY - 0.5f) * 2f) : Color.Lerp(cBot, cMid, tY * 2f);

                for (int x = 0; x < width; x++)
                {
                    float tX = x / (float)width;
                    // Radial glow from center-bottom
                    float dCenter = Vector2.Distance(new Vector2(x, y), new Vector2(width * 0.5f, height * 0.4f));
                    float radialBloom = Mathf.Clamp01(1f - dCenter / (width * 0.6f)) * 0.35f;

                    // Light sunburst rays
                    float angle = Mathf.Atan2(y - height * 0.4f, x - width * 0.5f);
                    float ray = (Mathf.Sin(angle * 12f) + 1f) * 0.5f * 0.15f;

                    Color bgPix = rowBg + new Color(0.4f, 0.2f, 0.5f, 0f) * radialBloom + Color.white * ray * radialBloom;
                    bgPix.a = 1f;

                    // Rounded corner mask
                    float r = 32f;
                    float cx = Mathf.Clamp(x, r, width - 1 - r);
                    float cy = Mathf.Clamp(y, r, height - 1 - r);
                    float dCorner = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dCorner > r)
                    {
                        bgPix.a = Mathf.Clamp01(1f - (dCorner - r));
                    }
                    bannerTex.SetPixel(x, y, bgPix);
                }
            }

            // Blit High-res Mascots onto the banner
            string backupDir = "Assets/Textures/BlockBlastCute_Backup";
            string pinkPath = $"{backupDir}/Block_Pink_Mascot.png";
            string mintPath = $"{backupDir}/Block_Mint_Mascot.png";
            string goldPath = $"{backupDir}/Block_Gold_Mascot.png";
            string purplePath = $"{backupDir}/Block_Purple_Mascot.png";

            // Mascot compositions (Destination X, Y, Width, Height)
            // Left to right joyful party line-up:
            BlitMascotOntoTexture(bannerTex, mintPath, 70, 70, 250, 250);
            BlitMascotOntoTexture(bannerTex, pinkPath, 280, 50, 310, 310);
            BlitMascotOntoTexture(bannerTex, goldPath, 520, 60, 275, 275);
            BlitMascotOntoTexture(bannerTex, purplePath, 730, 80, 245, 245);

            // Add celebration sparkles and light bursts
            DrawSparkleStar(bannerTex, 190, 440, 28, new Color(1f, 0.95f, 0.55f));
            DrawSparkleStar(bannerTex, 510, 480, 35, Color.white);
            DrawSparkleStar(bannerTex, 830, 420, 26, new Color(0.65f, 0.92f, 1f));
            DrawSparkleStar(bannerTex, 360, 410, 18, new Color(1f, 0.8f, 0.9f));
            DrawSparkleStar(bannerTex, 670, 390, 20, new Color(1f, 0.9f, 0.4f));

            bannerTex.Apply();
            return SaveAndConfigureSprite(path, bannerTex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateShopBannerPickupSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Banner_Pickup.png";
            string jpgSource = $"{Folder}/UI_Shop_Banner_Pickup.jpg";
            if (File.Exists(jpgSource))
            {
                byte[] raw = File.ReadAllBytes(jpgSource);
                Texture2D jTex = new Texture2D(2, 2);
                jTex.LoadImage(raw);
                SafeWriteAllBytes(path, jTex.EncodeToPNG());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                SetSpriteImportSettings(path, 2048);
                return ForceGetOrImportSingleSprite(path);
            }
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 1024;
            int height = 560;
            Texture2D bannerTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Radiant celestial summoning dreamscape
            Color cTop = new Color(0.65f, 0.15f, 0.72f, 1f);     // Vivid royal magenta
            Color cMid = new Color(0.25f, 0.08f, 0.48f, 1f);     // Deep celestial violet
            Color cBot = new Color(0.08f, 0.04f, 0.22f, 1f);     // Deep midnight indigo

            for (int y = 0; y < height; y++)
            {
                float tY = y / (float)height;
                Color rowBg = (tY > 0.5f) ? Color.Lerp(cMid, cTop, (tY - 0.5f) * 2f) : Color.Lerp(cBot, cMid, tY * 2f);

                for (int x = 0; x < width; x++)
                {
                    // Golden magic summoning circle center
                    Vector2 circleCenter = new Vector2(width * 0.48f, height * 0.45f);
                    float dCenter = Vector2.Distance(new Vector2(x, y), circleCenter);

                    // Concentric summoning magic rings
                    float r1 = 180f, r2 = 230f, r3 = 270f;
                    float ringIntensity = 0f;
                    if (Mathf.Abs(dCenter - r1) < 4f) ringIntensity += (1f - Mathf.Abs(dCenter - r1) / 4f) * 0.7f;
                    if (Mathf.Abs(dCenter - r2) < 3f) ringIntensity += (1f - Mathf.Abs(dCenter - r2) / 3f) * 0.5f;
                    if (Mathf.Abs(dCenter - r3) < 2f) ringIntensity += (1f - Mathf.Abs(dCenter - r3) / 2f) * 0.35f;

                    // Rotating starbeams
                    float angle = Mathf.Atan2(y - circleCenter.y, x - circleCenter.x);
                    float starbeam = (Mathf.Sin(angle * 16f) + 1f) * 0.5f * Mathf.Clamp01(1f - dCenter / 320f) * 0.3f;

                    Color bgPix = rowBg + new Color(1f, 0.85f, 0.40f, 0f) * (ringIntensity + starbeam);
                    bgPix.a = 1f;

                    // Rounded corner mask
                    float r = 32f;
                    float cx = Mathf.Clamp(x, r, width - 1 - r);
                    float cy = Mathf.Clamp(y, r, height - 1 - r);
                    float dCorner = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dCorner > r)
                    {
                        bgPix.a = Mathf.Clamp01(1f - (dCorner - r));
                    }
                    bannerTex.SetPixel(x, y, bgPix);
                }
            }

            // Blit Featured Pickup Mascot (Mint and Pink in glorious summon radiance)
            string backupDir = "Assets/Textures/BlockBlastCute_Backup";
            string mintPath = $"{backupDir}/Block_Mint_Mascot.png";
            string pinkPath = $"{backupDir}/Block_Pink_Mascot.png";
            string goldPath = $"{backupDir}/Block_Gold_Mascot.png";
            string purplePath = $"{backupDir}/Block_Purple_Mascot.png";

            // Left side: Pink & Purple backing, Center: Mint Pickup Star, Right: Gold Mascot
            BlitMascotOntoTexture(bannerTex, pinkPath, 140, 60, 240, 240);
            BlitMascotOntoTexture(bannerTex, purplePath, 670, 70, 240, 240);
            BlitMascotOntoTexture(bannerTex, mintPath, 340, 50, 340, 340); // Spotlight center featured pickup!

            // Majestic Golden Sparkles and summon stars
            DrawSparkleStar(bannerTex, 480, 480, 42, new Color(1f, 0.95f, 0.5f));
            DrawSparkleStar(bannerTex, 220, 420, 30, new Color(1f, 0.7f, 0.9f));
            DrawSparkleStar(bannerTex, 760, 410, 32, new Color(0.6f, 0.9f, 1f));
            DrawSparkleStar(bannerTex, 360, 360, 22, Color.white);
            DrawSparkleStar(bannerTex, 620, 350, 24, new Color(1f, 0.85f, 0.4f));

            bannerTex.Apply();
            return SaveAndConfigureSprite(path, bannerTex, Vector4.zero, false);
        }

        private static void BlitMascotOntoTexture(Texture2D dst, string srcPath, int dstX, int dstY, int dstW, int dstH)
        {
            if (!File.Exists(srcPath)) return;
            byte[] bytes = File.ReadAllBytes(srcPath);
            Texture2D srcTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!srcTex.LoadImage(bytes)) return;

            int srcW = srcTex.width;
            int srcH = srcTex.height;

            for (int dy = 0; dy < dstH; dy++)
            {
                int py = dstY + dy;
                if (py < 0 || py >= dst.height) continue;
                float v = dy / (float)dstH;
                int sy = Mathf.Clamp(Mathf.FloorToInt(v * srcH), 0, srcH - 1);

                for (int dx = 0; dx < dstW; dx++)
                {
                    int px = dstX + dx;
                    if (px < 0 || px >= dst.width) continue;
                    float u = dx / (float)dstW;
                    int sx = Mathf.Clamp(Mathf.FloorToInt(u * srcW), 0, srcW - 1);

                    Color srcPix = srcTex.GetPixel(sx, sy);
                    if (srcPix.a <= 0.01f) continue;

                    Color dstPix = dst.GetPixel(px, py);
                    Color blended = Color.Lerp(dstPix, srcPix, srcPix.a);
                    blended.a = Mathf.Max(dstPix.a, srcPix.a);
                    dst.SetPixel(px, py, blended);
                }
            }
        }

        public static Sprite GetOrCreateShopVerticalTabActiveSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Tab_Vertical_Active.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color outer = new Color(0.95f, 0.35f, 0.58f, 1f);     // Vibrant berry rim
            Color bevTop = new Color(1f, 0.65f, 0.80f, 1f);     // Sweet pastel pink bevel
            Color bevBot = new Color(0.92f, 0.30f, 0.55f, 1f);    // Coral pink
            Color fillTop = new Color(1f, 0.48f, 0.68f, 1f);     // Strawberry candy top
            Color fillBot = new Color(0.96f, 0.28f, 0.52f, 1f);    // Deep coral candy bot
            Color shadow = new Color(0.35f, 0.08f, 0.20f, 0.25f);

            Texture2D tex = GenerateShopActionButtonTexture(240, 96, 26f, outer, bevTop, bevBot, fillTop, fillBot, shadow);
            return SaveAndConfigureSprite(path, tex, new Vector4(30, 24, 30, 24), true);
        }

        public static Sprite GetOrCreateShopVerticalTabInactiveSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Tab_Vertical_Inactive.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(0.80f, 0.75f, 0.90f, 0.85f); // Soft lavender rim
            Color fillTop = new Color(0.97f, 0.95f, 1.0f, 0.82f);
            Color fillBot = new Color(0.92f, 0.88f, 0.96f, 0.82f);
            Color shadow = new Color(0.15f, 0.10f, 0.25f, 0.10f);

            Texture2D tex = GenerateBeveledPlaqueTexture(240, 96, 24f, border, fillTop, fillBot, shadow, addTopGloss: false);
            return SaveAndConfigureSprite(path, tex, new Vector4(28, 24, 28, 24), true);
        }

        public static Sprite GetOrCreateShopPackageCardSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shop_Package_Card.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            Color border = new Color(1f, 0.82f, 0.45f, 1f);      // Warm golden caramel border
            Color fillTop = new Color(1f, 1f, 1f, 0.98f);
            Color fillBot = new Color(0.98f, 0.96f, 1f, 0.98f);
            Color shadow = new Color(0.18f, 0.12f, 0.25f, 0.16f);

            Texture2D tex = GenerateBeveledPlaqueTexture(320, 420, 28f, border, fillTop, fillBot, shadow, addTopGloss: true);
            return SaveAndConfigureSprite(path, tex, new Vector4(32, 32, 32, 32), true);
        }

        public static Sprite GetOrCreateGachaBallGreySprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Gacha_Ball_Grey.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.48f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float r = dist / maxR;

                    if (r > 1.0f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float alpha = Mathf.Clamp01((1.0f - r) / 0.035f);
                    float nx = dx / maxR;
                    float ny = dy / maxR;
                    float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - r * r));

                    Color baseCol;
                    if (dy > 4f)
                    {
                        float t = (ny - 0.05f) / 0.95f;
                        baseCol = Color.Lerp(new Color(0.85f, 0.88f, 0.93f), new Color(0.96f, 0.97f, 1.0f), t);
                    }
                    else if (dy < -4f)
                    {
                        float t = (-ny - 0.05f) / 0.95f;
                        baseCol = Color.Lerp(new Color(0.70f, 0.73f, 0.78f), new Color(0.50f, 0.53f, 0.58f), t);
                    }
                    else
                    {
                        baseCol = new Color(0.38f, 0.40f, 0.46f);
                    }

                    Vector3 lightDir = new Vector3(-0.35f, 0.45f, 0.82f).normalized;
                    Vector3 normal = new Vector3(nx, ny, nz);
                    float diff = Mathf.Max(0.15f, Vector3.Dot(normal, lightDir) * 0.65f + 0.35f);
                    float spec = Mathf.Pow(Mathf.Max(0f, Vector3.Dot(normal, lightDir)), 14f) * 0.85f;
                    float rim = Mathf.Pow(1f - nz, 2.5f) * 0.35f;

                    Color finalCol = baseCol * diff + Color.white * spec + new Color(0.85f, 0.9f, 1f) * rim;
                    finalCol.a = alpha;
                    tex.SetPixel(x, y, finalCol);
                }
            }

            DrawSparkleStar(tex, (int)cx, (int)cy, 18, new Color(0.95f, 0.98f, 1f, 0.95f));
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateGachaBallRainbowSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Gacha_Ball_Rainbow.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.48f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float r = dist / maxR;

                    if (r > 1.0f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float alpha = Mathf.Clamp01((1.0f - r) / 0.035f);
                    float nx = dx / maxR;
                    float ny = dy / maxR;
                    float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - r * r));

                    float angle = Mathf.Atan2(dy, dx);
                    float hue = Mathf.Repeat((angle / (Mathf.PI * 2f)) + (nx * 0.25f) + 0.5f, 1f);
                    Color rainbowCol = Color.HSVToRGB(hue, 0.62f, 0.98f);

                    if (Mathf.Abs(dy) <= 5f)
                    {
                        rainbowCol = new Color(1f, 0.88f, 0.35f);
                    }

                    Vector3 lightDir = new Vector3(-0.35f, 0.45f, 0.82f).normalized;
                    Vector3 normal = new Vector3(nx, ny, nz);
                    float diff = Mathf.Max(0.2f, Vector3.Dot(normal, lightDir) * 0.6f + 0.4f);
                    float spec = Mathf.Pow(Mathf.Max(0f, Vector3.Dot(normal, lightDir)), 12f) * 0.95f;
                    float goldenRim = Mathf.Pow(1f - nz, 2.0f) * 0.55f;

                    Color finalCol = rainbowCol * diff + Color.white * spec + new Color(1f, 0.95f, 0.6f) * goldenRim;
                    finalCol.a = alpha;
                    tex.SetPixel(x, y, finalCol);
                }
            }

            DrawSparkleStar(tex, (int)cx, (int)cy, 22, new Color(1f, 0.98f, 0.75f, 1f));
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateGoldGachaBallSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Gacha_Ball_Gold.png";
            return ForceGetOrImportSingleSprite(path) ?? GetOrCreateGachaBallRainbowSprite();
        }

        public static Sprite GetOrCreateSilverCoinSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Gacha_Coin_Silver.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateGoldCoinSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Gacha_Coin_Gold.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateGachaMachineSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/Gacha_Machine.png";
            return ForceGetOrImportSingleSprite(path);
        }

        public static Sprite GetOrCreateJellyButtonInactiveSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Btn_Jelly_Inactive.png";
            Sprite sp = ForceGetOrImportSingleSprite(path);
            if (sp != null && sp.border == Vector4.zero)
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.spriteBorder = new Vector4(36, 28, 36, 28);
                    importer.SaveAndReimport();
                    sp = ForceGetOrImportSingleSprite(path);
                }
            }
            return sp ?? GetOrCreateTabPillSprite();
        }

        public static Sprite GetOrCreateGachaRainbowAuraSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Gacha_Rainbow_Aura.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float r = Mathf.Sqrt(dx * dx + dy * dy) / maxR;
                    if (r >= 1.0f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float angle = Mathf.Atan2(dy, dx);
                    float hue = Mathf.Repeat(angle / (Mathf.PI * 2f), 1f);
                    Color col = Color.HSVToRGB(hue, 0.5f, 1f);
                    col = Color.Lerp(Color.white, col, r);
                    col.a = Mathf.Pow(1f - r, 2.2f) * 0.9f;
                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateGachaSunburstSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Gacha_Sunburst_Rays.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 512;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            float maxR = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float r = Mathf.Sqrt(dx * dx + dy * dy) / maxR;
                    if (r >= 1.0f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float angle = Mathf.Atan2(dy, dx);
                    float rays = Mathf.Pow(Mathf.Max(0f, Mathf.Cos(angle * 12f)), 1.5f);
                    float falloff = Mathf.Pow(1f - r, 1.8f);

                    float hue = Mathf.Repeat(angle / (Mathf.PI * 2f), 1f);
                    Color rayCol = Color.Lerp(new Color(1f, 0.9f, 0.4f), Color.HSVToRGB(hue, 0.45f, 1f), 0.5f);
                    rayCol.a = rays * falloff * 0.85f;
                    tex.SetPixel(x, y, rayCol);
                }
            }
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        public static Sprite GetOrCreateShootingStarSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Shooting_Star.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 256;
            int height = 64;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            float headX = width - 36f;
            float cy = (height - 1) * 0.5f;

            for (int y = 0; y < height; y++)
            {
                float dy = y - cy;
                float absDy = Mathf.Abs(dy);

                for (int x = 0; x < width; x++)
                {
                    Color pixel = Color.clear;

                    // 1. Tapering Tail (from x = 0 to headX)
                    if (x <= headX)
                    {
                        float t = (float)x / headX; // 0 at tail tip, 1 at head
                        float maxHalfWidth = Mathf.Lerp(1.0f, 14f, Mathf.Pow(t, 0.7f));
                        if (absDy <= maxHalfWidth)
                        {
                            float crossAlpha = Mathf.Exp(- (absDy * absDy) / (2f * Mathf.Pow(maxHalfWidth * 0.45f, 2f)));
                            float lengthAlpha = Mathf.Pow(t, 1.4f);
                            float tailAlpha = crossAlpha * lengthAlpha;

                            Color tailCol = Color.Lerp(new Color(0.6f, 0.85f, 1f, 1f), new Color(1f, 0.95f, 0.7f, 1f), t);
                            tailCol.a = tailAlpha * 0.95f;
                            pixel = tailCol;
                        }
                    }

                    // 2. Glowing Head Flare (centered at headX, cy)
                    float distHead = Mathf.Sqrt((x - headX) * (x - headX) + dy * dy);
                    if (distHead < 28f)
                    {
                        float headRatio = distHead / 28f;
                        float coreAlpha = Mathf.Pow(1f - headRatio, 2.0f);
                        
                        // 4-star radiant rays
                        float crossSpike = Mathf.Max(
                            Mathf.Exp(- (dy * dy) / 4f) * Mathf.Exp(- Mathf.Pow(x - headX, 2f) / 180f),
                            Mathf.Exp(- Mathf.Pow(x - headX, 2f) / 4f) * Mathf.Exp(- (dy * dy) / 180f)
                        );

                        float totalHeadAlpha = Mathf.Clamp01(coreAlpha + crossSpike * 0.85f);
                        Color headCol = Color.Lerp(new Color(1f, 0.98f, 0.85f, 1f), Color.white, coreAlpha);
                        headCol.a = totalHeadAlpha;

                        // Blend head over tail
                        pixel = Color.Lerp(pixel, headCol, headCol.a);
                        pixel.a = Mathf.Max(pixel.a, totalHeadAlpha);
                    }

                    tex.SetPixel(x, y, pixel);
                }
            }
            tex.Apply();
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        // =========================================================
        // NANOBANANA JUICY 3D JELLY CANDY BUTTON GENERATOR
        // =========================================================

        public static Sprite GetOrCreateNanoBananaJellyButtonSprite(string name, Color baseCol, Color cushionCol, Color rimCol, bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/{name}.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int width = 256;
            int height = 96;
            Texture2D tex = GenerateNanoBananaJellyButtonTexture(width, height, baseCol, cushionCol, rimCol);
            return SaveAndConfigureSprite(path, tex, new Vector4(46, 38, 46, 38), true);
        }

        public static Sprite GetOrCreateJellyButtonPinkSprite(bool force = false) =>
            GetOrCreateNanoBananaJellyButtonSprite("UI_Btn_Jelly_Pink", new Color(1f, 0.38f, 0.60f, 1f), new Color(0.82f, 0.16f, 0.40f, 1f), new Color(0.92f, 0.22f, 0.48f, 1f), force);

        public static Sprite GetOrCreateJellyButtonMintSprite(bool force = false) =>
            GetOrCreateNanoBananaJellyButtonSprite("UI_Btn_Jelly_Mint", new Color(0.00f, 0.82f, 0.70f, 1f), new Color(0.00f, 0.54f, 0.48f, 1f), new Color(0.00f, 0.68f, 0.58f, 1f), force);

        public static Sprite GetOrCreateJellyButtonGoldSprite(bool force = false) =>
            GetOrCreateNanoBananaJellyButtonSprite("UI_Btn_Jelly_Gold", new Color(1f, 0.70f, 0.14f, 1f), new Color(0.82f, 0.48f, 0.05f, 1f), new Color(0.92f, 0.60f, 0.08f, 1f), force);

        public static Sprite GetOrCreateJellyButtonPurpleSprite(bool force = false) =>
            GetOrCreateNanoBananaJellyButtonSprite("UI_Btn_Jelly_Purple", new Color(0.68f, 0.40f, 0.98f, 1f), new Color(0.48f, 0.20f, 0.78f, 1f), new Color(0.58f, 0.28f, 0.88f, 1f), force);

        public static Sprite GetOrCreateJellyButtonCreamSprite(bool force = false) =>
            GetOrCreateNanoBananaJellyButtonSprite("UI_Btn_Jelly_Cream", new Color(0.99f, 0.97f, 1.0f, 1f), new Color(0.88f, 0.84f, 0.94f, 1f), new Color(0.82f, 0.76f, 0.90f, 1f), force);

        public static Sprite GetOrCreateNanoBananaCandyCloseButtonSprite(bool force = false)
        {
            EnsureFolder();
            string path = $"{Folder}/UI_Btn_Jelly_Close.png";
            if (!force && File.Exists(path)) return ForceGetOrImportSingleSprite(path);

            int size = 128;
            Texture2D tex = GenerateNanoBananaCandyCloseTexture(size);
            return SaveAndConfigureSprite(path, tex, Vector4.zero, false);
        }

        private static Texture2D GenerateNanoBananaJellyButtonTexture(int width, int height, Color baseCol, Color cushionCol, Color rimCol)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float bevelHeight = 10f;
            float radius = (height - bevelHeight) * 0.46f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 1. Soft Drop Shadow
                    float shCx = Mathf.Clamp(x, radius + 4f, width - 1 - radius - 4f);
                    float shCy = radius + 2f;
                    float shDist = Vector2.Distance(new Vector2(x, y), new Vector2(shCx, shCy));
                    float shAlpha = 0f;
                    if (y < radius + 8f && shDist <= radius + 5f)
                    {
                        shAlpha = Mathf.Clamp01((radius + 5f - shDist) / 5f) * 0.24f;
                    }

                    // 2. 3D Bottom Base Cushion
                    float extCx = Mathf.Clamp(x, radius, width - 1 - radius);
                    float extCy = radius + 2f;
                    float extDist = Vector2.Distance(new Vector2(x, y), new Vector2(extCx, extCy));
                    float extAlpha = 0f;
                    Color extColor = cushionCol;
                    if (y <= radius + bevelHeight + 2f && extDist <= radius + 1f)
                    {
                        extAlpha = Mathf.Clamp01(radius + 1f - extDist);
                        float ny = Mathf.Clamp01(y / (radius + bevelHeight));
                        extColor = Color.Lerp(cushionCol * 0.85f, cushionCol, ny);
                    }

                    // 3. Top Gelatin Face Pill
                    float topCx = Mathf.Clamp(x, radius, width - 1 - radius);
                    float topCy = radius + bevelHeight;
                    float topDist = Vector2.Distance(new Vector2(x, y), new Vector2(topCx, topCy));
                    float topAlpha = 0f;
                    Color faceColor = Color.clear;

                    if (topDist <= radius + 1f)
                    {
                        topAlpha = Mathf.Clamp01(radius + 1f - topDist);
                        float faceNormY = Mathf.Clamp01((y - bevelHeight) / (height - bevelHeight));

                        Color colLight = Color.Lerp(baseCol, Color.white, 0.22f);
                        Color colDeep = baseCol;
                        faceColor = Color.Lerp(colDeep, colLight, Mathf.SmoothStep(0.05f, 0.95f, faceNormY));

                        // Soft Subsurface Scattering along bottom curve
                        if (y < topCy && topDist > radius - 8f)
                        {
                            float rimUp = (topDist - (radius - 8f)) / 8f;
                            faceColor = Color.Lerp(faceColor, Color.Lerp(baseCol, Color.white, 0.45f), rimUp * 0.35f);
                        }

                        // Top Crescent Glass Gloss Arc
                        if (faceNormY > 0.48f)
                        {
                            float arcT = (faceNormY - 0.48f) / 0.52f;
                            float glossIntensity = Mathf.SmoothStep(0f, 1f, arcT) * 0.55f;
                            faceColor = Color.Lerp(faceColor, Color.white, glossIntensity);
                        }

                        // Topmost fine glaze highlight
                        if (y > height - 12 && topDist < radius - 2f)
                        {
                            float fineGlow = Mathf.Clamp01((y - (height - 12)) / 8f);
                            faceColor = Color.Lerp(faceColor, Color.white, fineGlow * 0.40f);
                        }

                        // Outer Crisp Rim / Stroke
                        if (topDist > radius - 2.5f)
                        {
                            float rimT = (topDist - (radius - 2.5f)) / 2.5f;
                            faceColor = Color.Lerp(faceColor, rimCol, rimT * 0.85f);
                        }
                    }

                    Color pix = Color.clear;
                    if (shAlpha > 0f)
                    {
                        Color shadow = new Color(cushionCol.r * 0.3f, cushionCol.g * 0.15f, cushionCol.b * 0.3f, shAlpha);
                        pix = shadow;
                    }
                    if (extAlpha > 0f)
                    {
                        extColor.a = extAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, extColor, extAlpha) : extColor;
                        pix.a = Mathf.Max(pix.a, extAlpha);
                    }
                    if (topAlpha > 0f)
                    {
                        faceColor.a = topAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, faceColor, topAlpha) : faceColor;
                        pix.a = Mathf.Max(pix.a, topAlpha);
                    }

                    tex.SetPixel(x, y, pix);
                }
            }

            // Draw cute glossy gleam spot on left shoulder
            float spotX = radius + 8f;
            float spotY = height - 20f;
            DrawSoftGleamSpot(tex, spotX, spotY, 14f, 7f, 0.70f);

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateNanoBananaCandyCloseTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f + 3f;
            float radius = size * 0.42f;

            Color baseCol = new Color(1f, 0.38f, 0.58f, 1f);     // Juicy strawberry candy
            Color cushionCol = new Color(0.80f, 0.14f, 0.38f, 1f); // Deep raspberry base
            Color rimCol = new Color(0.92f, 0.20f, 0.48f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // 1. Soft Drop Shadow
                    float shDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, radius + 2f));
                    float shAlpha = 0f;
                    if (shDist <= radius + 5f)
                    {
                        shAlpha = Mathf.Clamp01((radius + 5f - shDist) / 5f) * 0.25f;
                    }

                    // 2. 3D Cushion Base
                    float extDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy - 6f));
                    float extAlpha = 0f;
                    if (extDist <= radius + 1f)
                    {
                        extAlpha = Mathf.Clamp01(radius + 1f - extDist);
                    }

                    // 3. Top Sphere
                    float topDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float topAlpha = 0f;
                    Color faceColor = Color.clear;

                    if (topDist <= radius + 1f)
                    {
                        topAlpha = Mathf.Clamp01(radius + 1f - topDist);
                        float normY = Mathf.Clamp01((y - (cy - radius)) / (2f * radius));

                        faceColor = Color.Lerp(cushionCol, Color.Lerp(baseCol, Color.white, 0.3f), normY);

                        if (normY > 0.55f)
                        {
                            float gT = (normY - 0.55f) / 0.45f;
                            faceColor = Color.Lerp(faceColor, Color.white, gT * 0.50f);
                        }

                        if (topDist > radius - 2.5f)
                        {
                            float rT = (topDist - (radius - 2.5f)) / 2.5f;
                            faceColor = Color.Lerp(faceColor, rimCol, rT);
                        }
                    }

                    Color pix = Color.clear;
                    if (shAlpha > 0f)
                    {
                        pix = new Color(0.3f, 0.05f, 0.15f, shAlpha);
                    }
                    if (extAlpha > 0f)
                    {
                        Color ext = cushionCol; ext.a = extAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, ext, extAlpha) : ext;
                        pix.a = Mathf.Max(pix.a, extAlpha);
                    }
                    if (topAlpha > 0f)
                    {
                        faceColor.a = topAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, faceColor, topAlpha) : faceColor;
                        pix.a = Mathf.Max(pix.a, topAlpha);
                    }

                    tex.SetPixel(x, y, pix);
                }
            }

            // Draw cute specular highlight spot on top-left
            DrawSoftGleamSpot(tex, cx - radius * 0.42f, cy + radius * 0.42f, 10f, 6f, 0.85f);

            // Draw clean white rounded 'X' cross
            float armLen = radius * 0.42f;
            float thickness = 4.8f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - cx);
                    float dy = Mathf.Abs(y - cy);
                    float d1 = Mathf.Abs((x - cx) - (y - cy)) / 1.414f;
                    float d2 = Mathf.Abs((x - cx) + (y - cy)) / 1.414f;
                    float distLine = Mathf.Min(d1, d2);
                    float maxD = Mathf.Max(dx, dy);

                    if (maxD <= armLen && distLine <= thickness + 1.2f)
                    {
                        float a = Mathf.Clamp01(thickness + 1.2f - distLine);
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, Color.white, a));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static void DrawSoftGleamSpot(Texture2D tex, float cx, float cy, float rx, float ry, float maxAlpha)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx * 1.5f));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + rx * 1.5f));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry * 1.5f));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + ry * 1.5f));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float distSq = dx * dx + dy * dy;
                    if (distSq <= 1f)
                    {
                        float a = Mathf.SmoothStep(1f, 0f, Mathf.Sqrt(distSq)) * maxAlpha;
                        Color orig = tex.GetPixel(x, y);
                        if (orig.a > 0.1f)
                        {
                            tex.SetPixel(x, y, Color.Lerp(orig, Color.white, a));
                        }
                    }
                }
            }
        }

        [MenuItem("Block Blast/Generate New Shop & Diamond Assets")]
        public static void GenerateNewShopAndDiamondAssets()
        {
            Debug.Log("[Assets] Generating Diamond icon...");
            GetOrCreateDiamondIconSprite(true);

            Debug.Log("[Assets] Generating New Mascot Shop Banner...");
            GetOrCreateShopBannerNewMascotSprite(true);

            Debug.Log("[Assets] Generating Pickup Shop Banner...");
            GetOrCreateShopBannerPickupSprite(true);

            Debug.Log("[Assets] Generating Vertical Tab Sprites...");
            GetOrCreateShopVerticalTabActiveSprite(true);
            GetOrCreateShopVerticalTabInactiveSprite(true);

            Debug.Log("[Assets] Generating Package Card Sprite...");
            GetOrCreateShopPackageCardSprite(true);

            AssetDatabase.Refresh();
            Debug.Log("<color=green><b>[Assets] All New Shop & Diamond Assets Successfully Generated!</b></color>");
        }
    }
}
#endif
