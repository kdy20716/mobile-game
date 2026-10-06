#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class LuxuryUIAssetProcessor
    {
        private static readonly string TargetDir = "Assets/Textures/BlockBlastCute";

        [MenuItem("Block Blast/Debug/Process Luxury Assets")]
        public static void ProcessLuxuryAssets()
        {
            if (!Directory.Exists(TargetDir)) Directory.CreateDirectory(TargetDir);

            // 1. Procedurally generate Luxury Capsule Buttons (512x144, perfect 9-sliceable pill)
            GenerateProceduralLuxuryCapsuleButton(
                Path.Combine(TargetDir, "UI_Btn_Luxury_Pink.png"),
                new Color(1.0f, 0.48f, 0.70f, 1f),
                new Color(0.96f, 0.22f, 0.48f, 1f),
                new Color(0.55f, 0.90f, 1.0f, 0.98f),
                new Color(0.60f, 0.15f, 0.35f, 0.25f)
            );

            GenerateProceduralLuxuryCapsuleButton(
                Path.Combine(TargetDir, "UI_Btn_Luxury_Mint.png"),
                new Color(0.32f, 0.90f, 0.85f, 1f),
                new Color(0.12f, 0.75f, 0.70f, 1f),
                new Color(0.60f, 0.98f, 1.0f, 0.98f),
                new Color(0.10f, 0.40f, 0.35f, 0.25f)
            );

            // 2. Bright & Fluffy Pastel Frosted Glass Card (No dark obsidian!)
            GenerateLuxuryGlassCard(Path.Combine(TargetDir, "UI_Shop_Card_Luxury_BA.png"));

            // 3. Bright & Fluffy Pastel Tab Track & Sliding Indicator
            GenerateLuxuryTabTrack(Path.Combine(TargetDir, "UI_Tab_Track_BA.png"));
            GenerateLuxuryTabIndicator(Path.Combine(TargetDir, "UI_Tab_Indicator_BA.png"));

            // 4. Bright Pastel Close Button & Item Card
            GenerateLuxuryCloseButton(Path.Combine(TargetDir, "UI_Btn_Close_Luxury_BA.png"));
            GenerateLuxuryItemCard(Path.Combine(TargetDir, "UI_Card_Glass_Item_BA.png"));

            // 5. Codex Card Frame, Badges & Stars
            GenerateCodexCardFrame(Path.Combine(TargetDir, "UI_Codex_Card_Frame.png"));
            GenerateRarityBadge(Path.Combine(TargetDir, "UI_Badge_Common.png"),
                new Color(0.40f, 0.86f, 0.72f, 1f), new Color(0.22f, 0.72f, 0.58f, 1f), new Color(1f, 1f, 1f, 0.9f));
            GenerateRarityBadge(Path.Combine(TargetDir, "UI_Badge_Rare.png"),
                new Color(0.42f, 0.72f, 1.0f, 1f), new Color(0.24f, 0.48f, 0.95f, 1f), new Color(1f, 1f, 1f, 0.9f));
            GenerateRarityBadge(Path.Combine(TargetDir, "UI_Badge_Special.png"),
                new Color(1.0f, 0.82f, 0.35f, 1f), new Color(1.0f, 0.52f, 0.20f, 1f), new Color(1f, 1f, 1f, 0.95f));

            GenerateStarSprite(Path.Combine(TargetDir, "UI_Star_Active.png"),
                new Color(1.0f, 0.86f, 0.20f, 1f), new Color(1.0f, 0.60f, 0.10f, 1f), true);
            GenerateStarSprite(Path.Combine(TargetDir, "UI_Star_Empty.png"),
                new Color(0.78f, 0.74f, 0.84f, 0.85f), new Color(0.68f, 0.64f, 0.76f, 0.85f), false);

            // 6. Generate 4 Rare Mascots (Blue, Berry, Lemon, Cloud)
            GenerateRareMascots();

            AssetDatabase.Refresh();
            Debug.Log("[LuxuryUIAssetProcessor] Successfully generated bright pastel luxury UI assets and 4 rare mascots!");
        }

        private static void GenerateLuxuryGlassCard(string dstPath)
        {
            int w = 512;
            int h = 768;
            float r = 52f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            // Bright, fluffy pastel aesthetic: Milky cream to soft cotton-candy lavender-pink
            Color outerRimCyan = new Color(0.55f, 0.85f, 1.0f, 0.95f);    // Soft pastel sky cyan
            Color outerRimLavender = new Color(1.0f, 0.65f, 0.82f, 0.95f); // Soft strawberry milk pink
            Color innerGlow = new Color(1.0f, 0.92f, 0.96f, 0.60f);        // Creamy blush
            Color glassTop = new Color(0.99f, 0.98f, 1.0f, 0.98f);         // Bright pure milky cream
            Color glassBot = new Color(0.94f, 0.92f, 0.98f, 0.98f);         // Soft marshmallow lavender
            Color shadowCol = new Color(0.40f, 0.25f, 0.50f, 0.18f);        // Dreamy soft shadow

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float u = (float)x / w;
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

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
                        float a = Mathf.Clamp01(r + 1.5f - d);
                        Color rimCol = Color.Lerp(outerRimLavender, outerRimCyan, (u + v) * 0.5f);
                        tex.SetPixel(x, y, new Color(rimCol.r, rimCol.g, rimCol.b, rimCol.a * a));
                    }
                    else if (d > r - 4.5f)
                    {
                        Color rimCol = Color.Lerp(outerRimLavender, outerRimCyan, (u + v) * 0.5f);
                        tex.SetPixel(x, y, rimCol);
                    }
                    else if (d > r - 6.5f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0.98f));
                    }
                    else if (d > r - 16f)
                    {
                        float tGlow = (r - 6.5f - d) / 9.5f;
                        Color bg = Color.Lerp(glassBot, glassTop, v);
                        Color c = Color.Lerp(innerGlow, bg, tGlow);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        Color c = Color.Lerp(glassBot, glassTop, v);
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

            // Bright pastel recessed slot track
            Color trackBg = new Color(0.92f, 0.90f, 0.96f, 0.96f);
            Color trackRimCyan = new Color(0.60f, 0.85f, 1.0f, 0.70f);
            Color trackRimLavender = new Color(0.90f, 0.72f, 0.95f, 0.70f);
            Color innerShadow = new Color(0.70f, 0.62f, 0.80f, 0.35f);
            Color bottomSheen = new Color(1.0f, 1.0f, 1.0f, 0.50f);

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
                        if (y > h - 16)
                        {
                            float tSh = (y - (h - 16)) / 16f;
                            c = Color.Lerp(c, innerShadow, tSh * 0.50f);
                        }
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

            Color topCol = new Color(1.0f, 0.48f, 0.70f, 1f);     // Rose pink
            Color botCol = new Color(0.96f, 0.25f, 0.52f, 1f);    // Deep magenta-rose
            Color neonRim = new Color(0.70f, 0.95f, 1.0f, 0.95f);  // Cyan neon highlight
            Color shadowCol = new Color(0.80f, 0.20f, 0.45f, 0.30f);

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

            Color rimCyan = new Color(1.0f, 0.55f, 0.75f, 0.95f);
            Color rimLavender = new Color(1.0f, 0.40f, 0.65f, 0.95f);
            Color bgTop = new Color(1.0f, 0.96f, 0.98f, 0.98f);
            Color bgBot = new Color(0.98f, 0.88f, 0.93f, 0.98f);
            Color xColor = new Color(0.85f, 0.22f, 0.44f, 0.98f);    // Sweet deep rose-plum X
            Color shadowCol = new Color(0.50f, 0.20f, 0.35f, 0.25f);

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

                        if (maxCoord < r * 0.50f && d1 < 3.2f)
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

            Color outerRimCyan = new Color(0.55f, 0.85f, 1.0f, 0.85f);
            Color outerRimLavender = new Color(0.95f, 0.65f, 0.85f, 0.85f);
            Color innerTrim = new Color(1.0f, 0.92f, 0.97f, 0.60f);
            Color fillTop = new Color(0.99f, 0.98f, 1.0f, 0.96f);
            Color fillBot = new Color(0.94f, 0.92f, 0.99f, 0.96f);
            Color shadowCol = new Color(0.40f, 0.25f, 0.50f, 0.15f);

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

        private static void GenerateCodexCardFrame(string dstPath)
        {
            int w = 256;
            int h = 360;
            float r = 36f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color outerRim = new Color(0.85f, 0.75f, 0.95f, 0.90f);
            Color pinstripe = new Color(1f, 1f, 1f, 0.95f);
            Color fillTop = new Color(1.0f, 0.99f, 1.0f, 0.98f);
            Color fillBot = new Color(0.95f, 0.93f, 0.99f, 0.98f);
            Color shadowCol = new Color(0.35f, 0.20f, 0.45f, 0.16f);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    float sy = y + 4f;
                    float scy = Mathf.Clamp(sy, r, h - 1 - r);
                    float sd = Mathf.Sqrt((x - cx) * (x - cx) + (sy - scy) * (sy - scy));

                    if (d > r + 1.2f)
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
                    else if (d > r - 1.2f)
                    {
                        float a = Mathf.Clamp01(r + 1.2f - d);
                        tex.SetPixel(x, y, new Color(outerRim.r, outerRim.g, outerRim.b, a));
                    }
                    else if (d > r - 3.2f)
                    {
                        tex.SetPixel(x, y, outerRim);
                    }
                    else if (d > r - 4.8f)
                    {
                        tex.SetPixel(x, y, pinstripe);
                    }
                    else
                    {
                        Color c = Color.Lerp(fillBot, fillTop, v);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(40, 40, 40, 40));
        }

        private static void GenerateRarityBadge(string dstPath, Color topCol, Color botCol, Color rimCol)
        {
            int w = 140;
            int h = 48;
            float r = 22f;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            for (int y = 0; y < h; y++)
            {
                float v = (float)y / h;
                for (int x = 0; x < w; x++)
                {
                    float cx = Mathf.Clamp(x, r, w - 1 - r);
                    float cy = Mathf.Clamp(y, r, h - 1 - r);
                    float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));

                    if (d > r + 1.2f)
                    {
                        tex.SetPixel(x, y, Color.clear);
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
                    else
                    {
                        Color c = Color.Lerp(botCol, topCol, v);
                        if (y > h * 0.5f)
                        {
                            c = Color.Lerp(c, Color.white, 0.20f);
                        }
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, new Vector4(24, 20, 24, 20));
        }

        private static void GenerateStarSprite(string dstPath, Color topCol, Color botCol, bool active)
        {
            int size = 64;
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float rOuter = 26f;
            float rInner = 11f;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            for (int y = 0; y < size; y++)
            {
                float v = (float)y / size;
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float angle = Mathf.Atan2(dy, dx) - Mathf.PI * 0.5f;
                    if (angle < 0) angle += Mathf.PI * 2f;

                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float section = Mathf.PI / 5f;
                    float relAngle = angle % (2f * section);
                    if (relAngle > section) relAngle = 2f * section - relAngle;

                    // Approximate star distance
                    float starR = Mathf.Lerp(rOuter, rInner, relAngle / section);

                    if (dist > starR + 1.2f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > starR - 1.2f)
                    {
                        float a = Mathf.Clamp01(starR + 1.2f - dist);
                        Color c = Color.Lerp(botCol, topCol, v);
                        tex.SetPixel(x, y, new Color(c.r, c.g, c.b, a));
                    }
                    else
                    {
                        Color c = Color.Lerp(botCol, topCol, v);
                        if (active && y > size * 0.55f && dist < starR * 0.7f)
                        {
                            c = Color.Lerp(c, Color.white, 0.40f);
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

                        float ny = (y - cy) / r;
                        if (ny > 0.25f)
                        {
                            c = Color.Lerp(c, Color.white, (ny - 0.25f) * 0.28f);
                        }

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

        // ==========================================
        // 4 RARE MASCOTS GENERATION
        // ==========================================
        private static void GenerateRareMascots()
        {
            // 1. Blue Mascot (Aqua Shield) - Ocean pastel blue squishy jelly
            GenerateCuteMascot(
                Path.Combine(TargetDir, "Block_Blue_Mascot.png"),
                new Color(0.35f, 0.76f, 1.0f, 1f),
                new Color(0.68f, 0.92f, 1.0f, 1f),
                MascotType.Blue
            );

            // 2. Berry Mascot (Sugar Burst) - Sweet strawberry-ruby pastel jelly
            GenerateCuteMascot(
                Path.Combine(TargetDir, "Block_Berry_Mascot.png"),
                new Color(1.0f, 0.38f, 0.62f, 1f),
                new Color(1.0f, 0.72f, 0.86f, 1f),
                MascotType.Berry
            );

            // 3. Lemon Mascot (Lemon Spark) - Cheerful sunshine honey-lemon pastel jelly
            GenerateCuteMascot(
                Path.Combine(TargetDir, "Block_Lemon_Mascot.png"),
                new Color(1.0f, 0.78f, 0.15f, 1f),
                new Color(1.0f, 0.95f, 0.50f, 1f),
                MascotType.Lemon
            );

            // 4. Cloud Mascot (Fluffy Cloud) - Soft dreamy marshmallow cotton-white jelly
            GenerateCuteMascot(
                Path.Combine(TargetDir, "Block_Cloud_Mascot.png"),
                new Color(0.85f, 0.90f, 1.0f, 1f),
                new Color(1.0f, 1.0f, 1.0f, 1f),
                MascotType.Cloud
            );
        }

        private enum MascotType { Blue, Berry, Lemon, Cloud }

        private static void GenerateCuteMascot(string dstPath, Color botCol, Color topCol, MascotType type)
        {
            int size = 512;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            float cx = size * 0.5f;
            float cy = size * 0.44f;
            float rx = 175f;
            float ry = 162f;

            Color shadowCol = new Color(0.20f, 0.15f, 0.35f, 0.25f);
            Color cheekCol = (type == MascotType.Berry) ? new Color(1.0f, 0.20f, 0.45f, 0.55f) : new Color(1.0f, 0.45f, 0.65f, 0.45f);

            for (int y = 0; y < size; y++)
            {
                float v = (float)y / size;
                for (int x = 0; x < size; x++)
                {
                    // Plump squishy base ellipse
                    float nx = (x - cx) / rx;
                    float ny = (y - cy) / ry;
                    // Slightly wider at bottom (squishy teardrop)
                    float squish = 1f - (y - cy) * 0.0006f;
                    nx /= squish;
                    float distSq = nx * nx + ny * ny;

                    // Dropshadow
                    float sny = (y - (cy - 12f)) / (ry * 0.4f);
                    float snx = (x - cx) / (rx * 1.05f);
                    float sDist = snx * snx + sny * sny;

                    // Ear / accessory distance check
                    bool inAccessory = false;
                    Color accessoryCol = Color.white;

                    if (type == MascotType.Blue)
                    {
                        // Water droplet on head: top droplet at (256, 400)
                        float dTop = Vector2.Distance(new Vector2(x, y), new Vector2(cx, 385f));
                        if (dTop < 42f)
                        {
                            inAccessory = true;
                            accessoryCol = Color.Lerp(topCol, Color.white, 0.35f);
                        }
                    }
                    else if (type == MascotType.Berry)
                    {
                        // Strawberry green leaves on top
                        float dL = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 30f, 380f));
                        float dR = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 30f, 380f));
                        float dM = Vector2.Distance(new Vector2(x, y), new Vector2(cx, 395f));
                        if (dL < 28f || dR < 28f || dM < 26f)
                        {
                            inAccessory = true;
                            accessoryCol = new Color(0.40f, 0.85f, 0.45f, 1f);
                        }
                    }
                    else if (type == MascotType.Lemon)
                    {
                        // Lightning spark antennae / cat ears
                        float dEarL = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 95f, 375f));
                        float dEarR = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 95f, 375f));
                        if (dEarL < 42f || dEarR < 42f)
                        {
                            inAccessory = true;
                            accessoryCol = Color.Lerp(botCol, topCol, 0.8f);
                        }
                    }
                    else if (type == MascotType.Cloud)
                    {
                        // Fluffy cloud puffs around upper body
                        float dP1 = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 130f, 310f));
                        float dP2 = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 130f, 310f));
                        float dP3 = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 70f, 370f));
                        float dP4 = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 70f, 370f));
                        if (dP1 < 50f || dP2 < 50f || dP3 < 52f || dP4 < 52f)
                        {
                            inAccessory = true;
                            accessoryCol = Color.Lerp(new Color(0.92f, 0.95f, 1f, 1f), Color.white, 0.5f);
                        }
                    }

                    if (distSq > 1.05f && !inAccessory)
                    {
                        if (sDist <= 1.0f && y < cy)
                        {
                            float sa = (1f - sDist) * shadowCol.a;
                            tex.SetPixel(x, y, new Color(shadowCol.r, shadowCol.g, shadowCol.b, sa));
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                        continue;
                    }

                    // Base Body Shading
                    Color c = inAccessory ? accessoryCol : Color.Lerp(botCol, topCol, v);

                    // 3D Rim and Top-Left Specular highlight
                    float lightDx = (x - (cx - 50f)) / rx;
                    float lightDy = (y - (cy + 60f)) / ry;
                    float lightDist = lightDx * lightDx + lightDy * lightDy;
                    if (lightDist < 0.25f && !inAccessory)
                    {
                        float sheen = (0.25f - lightDist) / 0.25f;
                        c = Color.Lerp(c, Color.white, sheen * 0.45f);
                    }

                    // Cheeks (Blush)
                    float dCheekL = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 95f, cy - 25f));
                    float dCheekR = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 95f, cy - 25f));
                    if (dCheekL < 35f)
                    {
                        float ba = Mathf.Clamp01((35f - dCheekL) / 35f) * cheekCol.a;
                        c = Color.Lerp(c, cheekCol, ba);
                    }
                    if (dCheekR < 35f)
                    {
                        float ba = Mathf.Clamp01((35f - dCheekR) / 35f) * cheekCol.a;
                        c = Color.Lerp(c, cheekCol, ba);
                    }

                    // Eyes
                    float eyeX1 = cx - 58f, eyeY = cy + 15f;
                    float eyeX2 = cx + 58f;
                    float dEye1 = Vector2.Distance(new Vector2(x, y), new Vector2(eyeX1, eyeY));
                    float dEye2 = Vector2.Distance(new Vector2(x, y), new Vector2(eyeX2, eyeY));

                    if (type == MascotType.Cloud)
                    {
                        // Cute closed smiling eyes (arc ⌒ ⌒)
                        float arc1 = Mathf.Abs(Vector2.Distance(new Vector2(x, y), new Vector2(eyeX1, eyeY - 8f)) - 20f);
                        float arc2 = Mathf.Abs(Vector2.Distance(new Vector2(x, y), new Vector2(eyeX2, eyeY - 8f)) - 20f);
                        if ((arc1 < 3.2f && y >= eyeY - 8f) || (arc2 < 3.2f && y >= eyeY - 8f))
                        {
                            c = new Color(0.20f, 0.18f, 0.32f, 1f);
                        }
                    }
                    else
                    {
                        // Big sparkling anime jelly eyes
                        float erx = 18f, ery = 26f;
                        float eDist1 = ((x - eyeX1) / erx) * ((x - eyeX1) / erx) + ((y - eyeY) / ery) * ((y - eyeY) / ery);
                        float eDist2 = ((x - eyeX2) / erx) * ((x - eyeX2) / erx) + ((y - eyeY) / ery) * ((y - eyeY) / ery);

                        if (eDist1 <= 1.0f || eDist2 <= 1.0f)
                        {
                            c = new Color(0.15f, 0.12f, 0.25f, 1f);

                            // Big white shine
                            float shX1 = eyeX1 + 5f, shY = eyeY + 8f;
                            float shX2 = eyeX2 + 5f;
                            float dSh1 = Vector2.Distance(new Vector2(x, y), new Vector2(shX1, shY));
                            float dSh2 = Vector2.Distance(new Vector2(x, y), new Vector2(shX2, shY));
                            if (dSh1 < 8f || dSh2 < 8f) c = Color.white;

                            // Small white sparkle
                            float spX1 = eyeX1 - 5f, spY = eyeY - 8f;
                            float spX2 = eyeX2 - 5f;
                            float dSp1 = Vector2.Distance(new Vector2(x, y), new Vector2(spX1, spY));
                            float dSp2 = Vector2.Distance(new Vector2(x, y), new Vector2(spX2, spY));
                            if (dSp1 < 4f || dSp2 < 4f) c = Color.white;
                        }
                    }

                    // Cute Smile Mouth
                    float dMouth = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy - 18f));
                    if (dMouth < 16f && y <= cy - 18f && y >= cy - 30f)
                    {
                        c = new Color(0.85f, 0.25f, 0.40f, 1f);
                    }

                    // Anti-aliased outer edge
                    if (distSq > 0.95f && !inAccessory)
                    {
                        float alpha = Mathf.Clamp01((1.05f - distSq) / 0.10f);
                        c.a *= alpha;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            File.WriteAllBytes(dstPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
            ConfigureSprite(dstPath, Vector4.zero);
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
