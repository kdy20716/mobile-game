#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class LuxuryUIAssetProcessor
    {
        private static readonly string BrainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
        private static readonly string TargetDir = "Assets/Textures/BlockBlastCute";

        [MenuItem("Block Blast/Debug/Process Luxury Assets")]
        public static void ProcessLuxuryAssets()
        {
            if (!Directory.Exists(TargetDir)) Directory.CreateDirectory(TargetDir);

            // 1. Procedurally generate Luxury Capsule Buttons (512x144, perfect 9-sliceable pill)
            GenerateProceduralLuxuryCapsuleButton(
                Path.Combine(TargetDir, "UI_Btn_Luxury_Pink.png"),
                new Color(1.0f, 0.44f, 0.66f, 1f),
                new Color(0.95f, 0.16f, 0.42f, 1f),
                new Color(0.45f, 0.88f, 1.0f, 0.98f),
                new Color(0.55f, 0.05f, 0.25f, 0.35f)
            );

            GenerateProceduralLuxuryCapsuleButton(
                Path.Combine(TargetDir, "UI_Btn_Luxury_Mint.png"),
                new Color(0.24f, 0.88f, 0.82f, 1f),
                new Color(0.06f, 0.68f, 0.62f, 1f),
                new Color(0.50f, 0.95f, 1.0f, 0.98f),
                new Color(0.04f, 0.35f, 0.32f, 0.35f)
            );

            // 2. Procedurally generate Blue Archive-style Frosted Glass Card
            GenerateLuxuryGlassCard(Path.Combine(TargetDir, "UI_Shop_Card_Luxury_BA.png"));

            // 3. Procedurally generate Blue Archive-style Tab Track & Sliding Indicator
            GenerateLuxuryTabTrack(Path.Combine(TargetDir, "UI_Tab_Track_BA.png"));
            GenerateLuxuryTabIndicator(Path.Combine(TargetDir, "UI_Tab_Indicator_BA.png"));

            // 4. Procedurally generate Luxury Close Button & Item Card
            GenerateLuxuryCloseButton(Path.Combine(TargetDir, "UI_Btn_Close_Luxury_BA.png"));
            GenerateLuxuryItemCard(Path.Combine(TargetDir, "UI_Card_Glass_Item_BA.png"));

            AssetDatabase.Refresh();
            Debug.Log("[LuxuryUIAssetProcessor] Successfully generated and processed all luxury UI assets!");
        }

        private static void ConvertButtonWithCleanGlow(string srcJpgPath, string dstPngPath, Color glowColor)
        {
            byte[] fileData = File.ReadAllBytes(srcJpgPath);
            Texture2D srcTex = new Texture2D(2, 2);
            srcTex.LoadImage(fileData);

            int w = srcTex.width;
            int h = srcTex.height;

            // Crop to actual button
            int minX = w, maxX = 0, minY = h, maxY = 0;
            Color[] pixels = srcTex.GetPixels();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = pixels[y * w + x];
                    float brightness = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
                    if (brightness > 0.08f)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            int pad = 8;
            minX = Mathf.Clamp(minX - pad, 0, w - 1);
            maxX = Mathf.Clamp(maxX + pad, 0, w - 1);
            minY = Mathf.Clamp(minY - pad, 0, h - 1);
            maxY = Mathf.Clamp(maxY + pad, 0, h - 1);

            int cropW = maxX - minX + 1;
            int cropH = maxY - minY + 1;

            Texture2D dstTex = new Texture2D(cropW, cropH, TextureFormat.RGBA32, false);

            for (int y = 0; y < cropH; y++)
            {
                for (int x = 0; x < cropW; x++)
                {
                    int srcX = minX + x;
                    int srcY = minY + y;
                    Color c = srcTex.GetPixel(srcX, srcY);

                    float brightness = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
                    float bgCutoff = 0.055f;
                    float glowEnd = 0.32f;

                    if (brightness <= bgCutoff)
                    {
                        dstTex.SetPixel(x, y, Color.clear);
                    }
                    else if (brightness < glowEnd)
                    {
                        // Clean neon glow: replace dark halo with bright luminous glow color
                        float alpha = Mathf.SmoothStep(0f, 1f, (brightness - bgCutoff) / (glowEnd - bgCutoff));
                        dstTex.SetPixel(x, y, new Color(glowColor.r, glowColor.g, glowColor.b, alpha * 0.90f));
                    }
                    else
                    {
                        // Inside the button body / rim
                        dstTex.SetPixel(x, y, new Color(c.r, c.g, c.b, 1f));
                    }
                }
            }

            dstTex.Apply();
            File.WriteAllBytes(dstPngPath, dstTex.EncodeToPNG());

            Object.DestroyImmediate(srcTex);
            Object.DestroyImmediate(dstTex);

            AssetDatabase.ImportAsset(dstPngPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPngPath, new Vector4(90, 50, 90, 50));
        }

        private static void GenerateLuxuryGlassCard(string dstPath)
        {
            int w = 512;
            int h = 768;
            float r = 52f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color outerRimCyan = new Color(0.42f, 0.90f, 1.0f, 0.95f);    // Crisp neon cyan
            Color outerRimLavender = new Color(0.76f, 0.60f, 1.0f, 0.95f); // Neon lavender
            Color innerGlow = new Color(0.38f, 0.80f, 1.0f, 0.22f);       // Subtle inner neon glow
            Color glassTop = new Color(0.13f, 0.10f, 0.24f, 0.97f);        // Deep cosmic twilight obsidian
            Color glassBot = new Color(0.08f, 0.06f, 0.16f, 0.98f);        // Deep midnight obsidian
            Color shadowCol = new Color(0.02f, 0.01f, 0.06f, 0.70f);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / w;
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    // Drop shadow offset (dy = 4)
                    float sy = y + 5f;
                    float scy = Mathf.Clamp(sy, r, h - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 8f)
                        {
                            float sa = Mathf.Clamp01((r + 8f - sd) / 8f) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    else if (d > r - 1.5f)
                    {
                        // Anti-aliased outer edge
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        Color rimCol = Color.Lerp(outerRimLavender, outerRimCyan, (u + v) * 0.5f);
                        tex.SetPixel(x, y, new Color(rimCol.r, rimCol.g, rimCol.b, rimCol.a * a));
                    }
                    else if (d > r - 4.5f)
                    {
                        // Outer Neon Glow Rim (Cyan to Lavender gradient)
                        Color rimCol = Color.Lerp(outerRimLavender, outerRimCyan, (u + v) * 0.5f);
                        tex.SetPixel(x, y, rimCol);
                    }
                    else if (d > r - 6.5f)
                    {
                        // Fine Crisp White Pinstripe
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0.98f));
                    }
                    else if (d > r - 16f)
                    {
                        // Soft Inner Neon Cyan Glow
                        float tGlow = (r - 6.5f - d) / 9.5f;
                        Color bg = Color.Lerp(glassBot, glassTop, v);
                        Color c = Color.Lerp(innerGlow, bg, tGlow);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        // Frosted Milky Acrylic Glass Body
                        Color c = Color.Lerp(glassBot, glassTop, v);

                        // Subtle diagonal glass reflection on upper half
                        if (y > h * 0.5f)
                        {
                            float diag = (u * 0.5f + v * 0.5f);
                            if (diag > 0.65f && diag < 0.88f)
                            {
                                float sheen = Mathf.Sin((diag - 0.65f) / 0.23f * Mathf.PI) * 0.08f;
                                c = Color.Lerp(c, Color.white, sheen);
                            }
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(64, 64, 64, 64));
        }

        private static void GenerateLuxuryTabTrack(string dstPath)
        {
            int w = 512;
            int h = 80;
            float r = 38f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color trackBg = new Color(0.06f, 0.04f, 0.14f, 0.90f); // Deep midnight obsidian track
            Color trackRimCyan = new Color(0.38f, 0.75f, 0.95f, 0.65f);
            Color trackRimLavender = new Color(0.65f, 0.50f, 0.92f, 0.65f);
            Color innerShadow = new Color(0.01f, 0.01f, 0.04f, 0.75f);
            Color bottomSheen = new Color(0.55f, 0.68f, 1.0f, 0.22f);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / w;
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    Color rimCol = Color.Lerp(trackRimLavender, trackRimCyan, (u + v) * 0.5f);

                    if (d > r + 1.2f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (d > r - 1.2f)
                    {
                        float a = Mathf.Clamp01(r + 1.2f - d);
                        tex.SetPixel(x, y, new Color(rimCol.r, rimCol.g, rimCol.b, rimCol.a * a));
                    }
                    else if (d > r - 3.5f)
                    {
                        tex.SetPixel(x, y, rimCol);
                    }
                    else
                    {
                        Color c = trackBg;
                        // Top inner shadow for recessed inset slot feeling
                        if (y > h - 16)
                        {
                            float tSh = (y - (h - 16)) / 16f;
                            c = Color.Lerp(c, innerShadow, tSh * 0.50f);
                        }
                        // Bottom subtle glass bevel reflection
                        if (y < 12)
                        {
                            float tG = (12 - y) / 12f;
                            c = Color.Lerp(c, bottomSheen, tG * 0.35f);
                        }
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(42, 24, 42, 24));
        }

        private static void GenerateLuxuryTabIndicator(string dstPath)
        {
            int w = 256;
            int h = 72;
            float r = 34f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            // Blue Archive / Luxury Mobile Game vibrant active tab pill:
            // Rich Rose-Magenta to Coral gradient with neon cyan rim and glass gloss
            Color topCol = new Color(1.0f, 0.44f, 0.65f, 1f);     // Rose pink
            Color botCol = new Color(0.96f, 0.22f, 0.48f, 1f);    // Deep magenta-rose
            Color neonRim = new Color(0.55f, 0.90f, 1.0f, 0.95f);  // Cyan neon highlight
            Color shadowCol = new Color(0.70f, 0.10f, 0.35f, 0.40f);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 3f;
                    float scy = Mathf.Clamp(sy, r, h - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.2f)
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
                    else if (d > r - 1.2f)
                    {
                        float a = Mathf.Clamp01(r + 1.2f - d);
                        tex.SetPixel(x, y, new Color(neonRim.r, neonRim.g, neonRim.b, a));
                    }
                    else if (d > r - 3.2f)
                    {
                        tex.SetPixel(x, y, neonRim);
                    }
                    else
                    {
                        Color c = Color.Lerp(botCol, topCol, v);
                        // Glass reflection highlight
                        if (y > h * 0.52f)
                        {
                            float gy = (y - h * 0.52f) / (h * 0.48f);
                            c = Color.Lerp(c, Color.white, gy * 0.28f);
                        }
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(38, 22, 38, 22));
        }

        private static void GenerateLuxuryCloseButton(string dstPath)
        {
            int size = 128;
            float r = size * 0.44f;
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            Color rimCyan = new Color(0.42f, 0.88f, 1.0f, 0.98f);
            Color rimLavender = new Color(0.78f, 0.62f, 1.0f, 0.98f);
            Color bgTop = new Color(0.16f, 0.13f, 0.28f, 0.96f); // Deep acrylic obsidian
            Color bgBot = new Color(0.08f, 0.06f, 0.18f, 0.97f);
            Color xColor = new Color(1.0f, 1.0f, 1.0f, 0.98f);    // Crisp white X
            Color shadowCol = new Color(0.02f, 0.01f, 0.05f, 0.60f);

            for (int y = 0; y < size; y++)
            {
                float v = (float)y / size;
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float sd = Vector2.Distance(new Vector2(x, y + 2.5f), new Vector2(cx, cy));

                    if (d > r + 1.2f)
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
                    else if (d > r - 1.2f)
                    {
                        float a = Mathf.Clamp01(r + 1.2f - d);
                        Color rim = Color.Lerp(rimLavender, rimCyan, v);
                        tex.SetPixel(x, y, new Color(rim.r, rim.g, rim.b, a));
                    }
                    else if (d > r - 3.5f)
                    {
                        Color rim = Color.Lerp(rimLavender, rimCyan, v);
                        tex.SetPixel(x, y, rim);
                    }
                    else
                    {
                        Color c = Color.Lerp(bgBot, bgTop, v);

                        // Draw clean minimalist 'X'
                        float lx = Mathf.Abs(x - cx);
                        float ly = Mathf.Abs(y - cy);
                        float d1 = Mathf.Abs(lx - ly);
                        float maxCoord = Mathf.Max(lx, ly);

                        if (maxCoord < r * 0.52f && d1 < 3.2f)
                        {
                            float xAlpha = Mathf.Clamp01((3.2f - d1) / 1.0f);
                            c = Color.Lerp(c, xColor, xAlpha);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, Vector4.zero);
        }

        private static void GenerateLuxuryItemCard(string dstPath)
        {
            int w = 256;
            int h = 256;
            float r = 32f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color outerRimCyan = new Color(0.40f, 0.82f, 1.0f, 0.85f);    // Sleek cyan neon rim
            Color outerRimLavender = new Color(0.75f, 0.60f, 1.0f, 0.85f);
            Color innerTrim = new Color(0.65f, 0.75f, 1.0f, 0.40f);        // Subtle lavender/cyan inner accent
            Color fillTop = new Color(0.12f, 0.09f, 0.23f, 0.92f);          // Deep midnight translucent acrylic
            Color fillBot = new Color(0.07f, 0.05f, 0.15f, 0.95f);
            Color shadowCol = new Color(0.01f, 0.01f, 0.04f, 0.60f);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / w;
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 3f;
                    float scy = Mathf.Clamp(sy, r, h - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    Color rimCol = Color.Lerp(outerRimLavender, outerRimCyan, (u + v) * 0.5f);

                    if (d > r + 1.2f)
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
                    else if (d > r - 1.2f)
                    {
                        float a = Mathf.Clamp01(r + 1.2f - d);
                        tex.SetPixel(x, y, new Color(rimCol.r, rimCol.g, rimCol.b, a));
                    }
                    else if (d > r - 2.8f)
                    {
                        tex.SetPixel(x, y, rimCol);
                    }
                    else if (d > r - 4.2f)
                    {
                        tex.SetPixel(x, y, innerTrim);
                    }
                    else
                    {
                        Color c = Color.Lerp(fillBot, fillTop, v);

                        // Subtle diagonal acrylic glass reflection sheen
                        if (y > h * 0.45f)
                        {
                            float diag = (u * 0.5f + v * 0.5f);
                            if (diag > 0.60f && diag < 0.85f)
                            {
                                float sheen = Mathf.Sin((diag - 0.60f) / 0.25f * Mathf.PI) * 0.08f;
                                c = Color.Lerp(c, Color.white, sheen);
                            }
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(36, 36, 36, 36));
        }

        private static void GenerateProceduralLuxuryCapsuleButton(
            string dstPath,
            Color topGrad,
            Color botGrad,
            Color neonRimCol,
            Color shadowCol)
        {
            int w = 512;
            int h = 144;
            float r = 70f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / w;
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 4f;
                    float scy = Mathf.Clamp(sy, r, h - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.5f)
                    {
                        if (sd <= r + 7f)
                        {
                            float sa = Mathf.Clamp01((r + 7f - sd) / 7f) * shadowCol.a;
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
                        tex.SetPixel(x, y, new Color(neonRimCol.r, neonRimCol.g, neonRimCol.b, neonRimCol.a * a));
                    }
                    else if (d > r - 4.5f)
                    {
                        tex.SetPixel(x, y, neonRimCol);
                    }
                    else if (d > r - 6.5f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0.95f));
                    }
                    else
                    {
                        Color c = Color.Lerp(botGrad, topGrad, v);

                        // 3D curvature lighting
                        float ny = (y - cy) / r;
                        if (ny > 0.25f)
                        {
                            c = Color.Lerp(c, Color.white, (ny - 0.25f) * 0.28f);
                        }

                        // Diagonal glass reflection highlight on upper area
                        if (y > h * 0.42f)
                        {
                            float diag = u * 0.35f + v * 0.65f;
                            if (diag > 0.50f && diag < 0.85f)
                            {
                                float sheen = Mathf.Sin((diag - 0.50f) / 0.35f * Mathf.PI) * 0.22f;
                                c = Color.Lerp(c, Color.white, sheen);
                            }
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(70, 70, 70, 70));
        }

        private static void ConfigureSprite(string path, Vector4 border)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.spriteBorder = border;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }
    }
}
#endif
