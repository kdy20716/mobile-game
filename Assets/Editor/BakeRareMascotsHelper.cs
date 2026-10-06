using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class BakeRareMascotsHelper
    {
        private const string Folder = "Assets/Textures/BlockBlastCute";

        [MenuItem("Tools/Bake All Rare Mascots")]
        public static void BakeAllRareMascots()
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);

            Debug.Log("<color=cyan>[RareMascots] Starting Rare Mascots Baking & Cutouts...</color>");

            // 1. Cutout Blue Mascot (Aqua Soda)
            string rawBlue = $"{Folder}/Block_Blue_Mascot_Raw.jpg";
            string dstBlue = $"{Folder}/Block_Blue_Mascot.png";
            string dstBlueJelly = $"{Folder}/Jelly_Mascot_Blue.png";
            if (File.Exists(rawBlue))
            {
                CutoutWhiteBackground(rawBlue, dstBlue);
                File.Copy(dstBlue, dstBlueJelly, true);
                AssetDatabase.ImportAsset(dstBlueJelly, ImportAssetOptions.ForceUpdate);
                SetSpriteImportSettings(dstBlueJelly, 2048);
                Debug.Log("[RareMascots] Blue Mascot cutout complete.");
            }

            // 2. Cutout Berry Mascot (Berry Cream)
            string rawBerry = $"{Folder}/Block_Berry_Mascot_Raw.jpg";
            string dstBerry = $"{Folder}/Block_Berry_Mascot.png";
            string dstBerryJelly = $"{Folder}/Jelly_Mascot_Berry.png";
            if (File.Exists(rawBerry))
            {
                CutoutWhiteBackground(rawBerry, dstBerry);
                File.Copy(dstBerry, dstBerryJelly, true);
                AssetDatabase.ImportAsset(dstBerryJelly, ImportAssetOptions.ForceUpdate);
                SetSpriteImportSettings(dstBerryJelly, 2048);
                Debug.Log("[RareMascots] Berry Mascot cutout complete.");
            }

            // 3. Cutout Lemon Mascot (Honey Lemon)
            string rawLemon = $"{Folder}/Block_Lemon_Mascot_Raw.jpg";
            string dstLemon = $"{Folder}/Block_Lemon_Mascot.png";
            string dstLemonJelly = $"{Folder}/Jelly_Mascot_Lemon.png";
            if (File.Exists(rawLemon))
            {
                CutoutWhiteBackground(rawLemon, dstLemon);
                File.Copy(dstLemon, dstLemonJelly, true);
                AssetDatabase.ImportAsset(dstLemonJelly, ImportAssetOptions.ForceUpdate);
                SetSpriteImportSettings(dstLemonJelly, 2048);
                Debug.Log("[RareMascots] Lemon Mascot cutout complete.");
            }

            // 4. Procedural 3D Dream Cloud Mascot (Cotton Dream)
            string dstCloud = $"{Folder}/Block_Cloud_Mascot.png";
            string dstCloudJelly = $"{Folder}/Jelly_Mascot_Cloud.png";
            BakeProceduralCloudMascot(dstCloud);
            File.Copy(dstCloud, dstCloudJelly, true);
            AssetDatabase.ImportAsset(dstCloudJelly, ImportAssetOptions.ForceUpdate);
            SetSpriteImportSettings(dstCloudJelly, 2048);
            Debug.Log("[RareMascots] Cloud Mascot procedural baking complete.");

            // 5. Re-bake Avatar Sprites
            CuteBlockTextureGenerator.GetOrCreateHighResAvatarSprite("Avatar_Blue", "Block_Blue_Mascot.png", true);
            CuteBlockTextureGenerator.GetOrCreateHighResAvatarSprite("Avatar_Berry", "Block_Berry_Mascot.png", true);
            CuteBlockTextureGenerator.GetOrCreateHighResAvatarSprite("Avatar_Lemon", "Block_Lemon_Mascot.png", true);
            CuteBlockTextureGenerator.GetOrCreateHighResAvatarSprite("Avatar_Cloud", "Block_Cloud_Mascot.png", true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green><b>[RareMascots] All 4 Rare Mascots & Avatars Successfully Baked!</b></color>");
        }

        public static void CutoutWhiteBackground(string srcJpgPath, string dstPngPath)
        {
            byte[] rawBytes = File.ReadAllBytes(srcJpgPath);
            Texture2D srcTex = new Texture2D(2, 2);
            srcTex.LoadImage(rawBytes);

            int sw = srcTex.width;
            int sh = srcTex.height;
            Color[] srcPixels = srcTex.GetPixels();

            bool[] isBg = new bool[sw * sh];
            Queue<int> q = new Queue<int>();

            // Flood fill seed from all 4 borders
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
                            // Clean white background threshold
                            if (c.r >= 0.92f && c.g >= 0.92f && c.b >= 0.92f)
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

                    if (adjacentBg && minDiff < 0.20f)
                    {
                        alphaMap[i] = Mathf.Clamp01(minDiff / 0.20f);
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
                            c.r = Mathf.Clamp01(c.r - invA * 0.95f) / Mathf.Max(a, 0.05f);
                            c.g = Mathf.Clamp01(c.g - invA * 0.95f) / Mathf.Max(a, 0.05f);
                            c.b = Mathf.Clamp01(c.b - invA * 0.95f) / Mathf.Max(a, 0.05f);
                        }
                        c.a = a;
                        outPixels[y * targetSize + x] = c;
                    }
                }
            }

            outTex.SetPixels(outPixels);
            outTex.Apply();

            File.WriteAllBytes(dstPngPath, outTex.EncodeToPNG());
            AssetDatabase.ImportAsset(dstPngPath, ImportAssetOptions.ForceUpdate);
            SetSpriteImportSettings(dstPngPath, 2048);
        }

        private static void TryEnqueueBg(int x, int y, int sw, int sh, Color[] srcPixels, bool[] isBg, Queue<int> q)
        {
            int idx = y * sw + x;
            if (!isBg[idx])
            {
                Color c = srcPixels[idx];
                if (c.r >= 0.92f && c.g >= 0.92f && c.b >= 0.92f)
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

        // ==========================================
        // PROCEDURAL 3D DREAM CLOUD MASCOT BAKER
        // ==========================================
        public static void BakeProceduralCloudMascot(string dstPath)
        {
            int S = 2048;
            Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
            Color[] px = new Color[S * S];

            // Setup Cloud Puffs (Positions & Radii in pixels)
            var puffs = new (Vector2 pos, float r)[]
            {
                (new Vector2(1024f, 960f), 650f),  // Main central body
                (new Vector2(580f, 920f), 450f),   // Left cheek puff
                (new Vector2(1468f, 920f), 450f),  // Right cheek puff
                (new Vector2(740f, 1340f), 420f),  // Top-left crown puff
                (new Vector2(1308f, 1340f), 420f), // Top-right crown puff
                (new Vector2(1024f, 1480f), 360f), // Center-top puff
                (new Vector2(780f, 440f), 220f),   // Left foot
                (new Vector2(1268f, 440f), 220f)   // Right foot
            };

            Vector3 lightDir = new Vector3(-0.45f, 0.65f, 0.60f).normalized;
            Vector3 rimLightDir = new Vector3(0.55f, 0.40f, -0.72f).normalized;

            Color colLilacTop = new Color(0.96f, 0.93f, 1.0f);     // Dreamy soft lilac-white
            Color colPinkMid = new Color(1.0f, 0.91f, 0.95f);      // Pastel cotton candy pink
            Color colPeachBot = new Color(1.0f, 0.94f, 0.90f);     // Warm sweet peach
            Color colSkyRim = new Color(0.86f, 0.95f, 1.0f);       // Soft sparkling soda rim
            Color colEyeDark = new Color(0.20f, 0.14f, 0.28f);     // Deep anime eye
            Color colBlush = new Color(1.0f, 0.52f, 0.68f, 1.0f);  // Rosy cheeks
            Color colStarGold = new Color(1.0f, 0.82f, 0.18f);    // Gem star clip
            Color colStarGlow = new Color(1.0f, 0.96f, 0.60f);

            for (int y = 0; y < S; y++)
            {
                float v = (float)y / (S - 1);
                for (int x = 0; x < S; x++)
                {
                    float u = (float)x / (S - 1);
                    Vector2 p = new Vector2(x, y);

                    // Compute maximum blended sphere height z
                    float maxZ = 0f;
                    Vector3 bestNorm = Vector3.forward;
                    float maxDistRatio = 0f;

                    for (int i = 0; i < puffs.Length; i++)
                    {
                        float d = Vector2.Distance(p, puffs[i].pos);
                        float r = puffs[i].r;
                        if (d < r)
                        {
                            float z = Mathf.Sqrt(Mathf.Max(0f, r * r - d * d));
                            if (z > maxZ)
                            {
                                maxZ = z;
                                Vector3 n = new Vector3((p.x - puffs[i].pos.x) / r, (p.y - puffs[i].pos.y) / r, z / r);
                                bestNorm = n.normalized;
                                maxDistRatio = 1f - (d / r);
                            }
                        }
                    }

                    if (maxZ <= 0.001f)
                    {
                        px[y * S + x] = Color.clear;
                        continue;
                    }

                    // Antialiased edge based on maxDistRatio
                    float alpha = Mathf.Clamp01(maxDistRatio * 32f);

                    // Soft fluffy marshmallow lighting (Very high ambient, gentle diffuse, dreamy rim)
                    float diff = Mathf.Clamp01(Vector3.Dot(bestNorm, lightDir));
                    float rim = Mathf.Pow(Mathf.Clamp01(1f - bestNorm.z), 2.2f);
                    float spec = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(bestNorm, (lightDir + Vector3.forward).normalized)), 20f);

                    // Bright pastel marshmallow body: Pure creamy white with soft baby pink blush gradient and baby blue rim
                    Color baseCol = Color.Lerp(new Color(1.0f, 0.94f, 0.97f), new Color(0.98f, 0.97f, 1.0f), v);
                    baseCol = Color.Lerp(baseCol, new Color(0.88f, 0.95f, 1.0f), rim * 0.40f);

                    Color finalCol = baseCol * (0.90f + 0.10f * diff) + Color.white * (spec * 0.35f);

                    // --- Facial Features ---
                    // Cheeks Blush
                    Vector2 leftCheek = new Vector2(740f, 850f);
                    Vector2 rightCheek = new Vector2(1308f, 850f);
                    float dLC = Vector2.Distance(p, leftCheek);
                    float dRC = Vector2.Distance(p, rightCheek);
                    float blushA = Mathf.Max(Mathf.Clamp01((150f - dLC) / 150f), Mathf.Clamp01((150f - dRC) / 150f));
                    if (blushA > 0f)
                    {
                        blushA = Mathf.SmoothStep(0f, 1f, blushA);
                        finalCol = Color.Lerp(finalCol, colBlush, blushA * 0.55f);
                    }

                    // Anime Eyes (Large sparkling eyes)
                    Vector2 leftEye = new Vector2(830f, 1020f);
                    Vector2 rightEye = new Vector2(1218f, 1020f);
                    float eyeRad = 115f;
                    float dLE = Vector2.Distance(p, leftEye);
                    float dRE = Vector2.Distance(p, rightEye);
                    float eyeDist = Mathf.Min(dLE, dRE);

                    if (eyeDist <= eyeRad)
                    {
                        float eyeA = Mathf.Clamp01((eyeRad - eyeDist) / 10f);
                        Color eyePixel = colEyeDark;

                        // Star sparkle highlight (Upper right in eye)
                        Vector2 eyeOffset = (dLE < dRE) ? (p - leftEye) : (p - rightEye);
                        Vector2 starOff = new Vector2(35f, 38f);
                        float distStar = Vector2.Distance(eyeOffset, starOff);
                        if (distStar < 42f)
                        {
                            eyePixel = Color.white;
                        }
                        // Secondary diamond catchlight
                        Vector2 subOff = new Vector2(-32f, -34f);
                        if (Vector2.Distance(eyeOffset, subOff) < 24f)
                        {
                            eyePixel = new Color(0.85f, 0.92f, 1f);
                        }

                        finalCol = Color.Lerp(finalCol, eyePixel, eyeA);
                    }

                    // Cute smiling mouth
                    Vector2 mouthCenter = new Vector2(1024f, 820f);
                    float mdx = (p.x - mouthCenter.x) * 1.4f;
                    float mdy = (p.y - mouthCenter.y);
                    if (mdy < 0f && mdy > -45f && Mathf.Abs(mdx) < 70f)
                    {
                        float curveY = -45f + (mdx * mdx) * 0.009f;
                        if (mdy >= curveY && mdy <= curveY + 16f)
                        {
                            finalCol = Color.Lerp(finalCol, new Color(0.38f, 0.16f, 0.30f), 0.92f);
                        }
                    }

                    // Golden Star Hairpin on Top-Left Puff (Center: 580, 1440)
                    Vector2 starCenter = new Vector2(580f, 1440f);
                    Vector2 sOff = p - starCenter;
                    float sDist = sOff.magnitude;
                    float sAngle = Mathf.Atan2(sOff.y, sOff.x);
                    float starR = 145f * (0.62f + 0.38f * Mathf.Cos(5f * (sAngle - Mathf.PI * 0.5f)));

                    if (sDist <= starR + 15f)
                    {
                        float starEdgeA = Mathf.Clamp01((starR - sDist) / 8f);
                        Color starCol = colStarGold;
                        if (sDist < 42f) starCol = Color.white; // gleaming center diamond
                        else if (sOff.y > 20f) starCol = Color.Lerp(colStarGold, colStarGlow, 0.7f);

                        // Outer star glow
                        if (starEdgeA < 1f)
                        {
                            float glowA = Mathf.Clamp01((starR + 15f - sDist) / 15f);
                            finalCol = Color.Lerp(finalCol, colStarGlow, glowA * 0.45f);
                        }
                        finalCol = Color.Lerp(finalCol, starCol, starEdgeA);
                    }

                    finalCol.a = alpha;
                    px[y * S + x] = finalCol;
                }
            }

            tex.SetPixels(px);
            tex.Apply();

            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            SetSpriteImportSettings(dstPath, 2048);
        }
    }
}
