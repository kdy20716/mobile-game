#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlockBlast.Editor
{
    public static class NanoBananaAssetProcessor
    {
        [MenuItem("Block Blast/Process NanoBanana Assets")]
        public static void ProcessAll()
        {
            string brainDir = @"C:\Users\kdy02\.gemini\antigravity\brain\11e13a30-67ac-4ca4-aa02-999cf9c4f657";
            string targetDir = @"Assets/Textures/BlockBlastCute";

            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            // 1. Process Cloud Mallang
            string cloudJpg = Path.Combine(brainDir, "cloud_mallang_mascot_1791380382613.jpg");
            if (File.Exists(cloudJpg))
            {
                Texture2D cutout = ProcessWhiteBgCutout(cloudJpg, 0.982f);
                byte[] pngData = cutout.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(targetDir, "Block_Cloud_Mascot.png"), pngData);
                File.WriteAllBytes(Path.Combine(targetDir, "Jelly_Mascot_Cloud.png"), pngData);

                // Create Avatar version (centered with cute white circle backing plate)
                Texture2D avatarTex = CreateAvatarWithPlate(cutout, 512);
                File.WriteAllBytes(Path.Combine(targetDir, "Avatar_Cloud.png"), avatarTex.EncodeToPNG());
                Debug.Log("<color=cyan>[NanoBanana] Processed Cloud Mallang mascot!</color>");
            }

            // 2. Process Gold Gacha Ball (Spherical Masking)
            string ballJpg = Path.Combine(brainDir, "gacha_ball_gold_1791380403546.jpg");
            if (File.Exists(ballJpg))
            {
                Texture2D cutout = ProcessSphericalGachaBall(ballJpg);
                File.WriteAllBytes(Path.Combine(targetDir, "Gacha_Ball_Gold.png"), cutout.EncodeToPNG());
                Debug.Log("<color=cyan>[NanoBanana] Processed Gold Gacha Ball!</color>");
            }

            // 3. Process Gacha Machine
            string machineJpg = Path.Combine(brainDir, "gacha_machine_1791380434089.jpg");
            if (File.Exists(machineJpg))
            {
                Texture2D cutout = ProcessWhiteBgCutout(machineJpg, 0.88f);
                CleanGachaMachineEdges(cutout);
                File.WriteAllBytes(Path.Combine(targetDir, "Gacha_Machine.png"), cutout.EncodeToPNG());
                Debug.Log("<color=cyan>[NanoBanana] Processed Gacha Machine!</color>");
            }

            // 4. Generate Silver Arcade Coin (1-Pull)
            Texture2D silverCoin = GenerateArcadeCoin(256, isGold: false);
            File.WriteAllBytes(Path.Combine(targetDir, "Gacha_Coin_Silver.png"), silverCoin.EncodeToPNG());

            // 5. Generate Gold Arcade Coin (10-Pull)
            Texture2D goldCoin = GenerateArcadeCoin(256, isGold: true);
            File.WriteAllBytes(Path.Combine(targetDir, "Gacha_Coin_Gold.png"), goldCoin.EncodeToPNG());

            // 6. Generate Unified Inactive Button Sprite (Pastel Cream-Lavender with subtle candy border)
            Texture2D inactiveBtn = GenerateInactivePillButton(256, 96);
            File.WriteAllBytes(Path.Combine(targetDir, "UI_Btn_Jelly_Inactive.png"), inactiveBtn.EncodeToPNG());

            AssetDatabase.Refresh();

            // Configure Sprites
            ConfigureSprite(Path.Combine(targetDir, "Block_Cloud_Mascot.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Avatar_Cloud.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Jelly_Mascot_Cloud.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Gacha_Ball_Gold.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Gacha_Machine.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Gacha_Coin_Silver.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "Gacha_Coin_Gold.png"), Vector4.zero);
            ConfigureSprite(Path.Combine(targetDir, "UI_Btn_Jelly_Inactive.png"), new Vector4(36, 28, 36, 28));

            Debug.Log("<color=#FF7AA2><b>[NanoBanana]</b> All assets processed and reimported successfully!</color>");
        }

        private static Texture2D ProcessWhiteBgCutout(string filePath, float threshold = 0.92f)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D src = new Texture2D(2, 2);
            src.LoadImage(bytes);

            int w = src.width;
            int h = src.height;
            Color[] pixels = src.GetPixels();
            bool[] isBg = new bool[w * h];
            Queue<int> q = new Queue<int>();

            void TryEnqueue(int x, int y)
            {
                int idx = y * w + x;
                if (!isBg[idx])
                {
                    Color c = pixels[idx];
                    float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
                    float min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
                    if (min >= 0.93f && (max - min) <= 0.035f)
                    {
                        isBg[idx] = true;
                        q.Enqueue(idx);
                    }
                }
            }

            for (int x = 0; x < w; x++)
            {
                TryEnqueue(x, 0);
                TryEnqueue(x, h - 1);
            }
            for (int y = 0; y < h; y++)
            {
                TryEnqueue(0, y);
                TryEnqueue(w - 1, y);
            }

            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            while (q.Count > 0)
            {
                int curr = q.Dequeue();
                int cx = curr % w;
                int cy = curr / w;

                for (int k = 0; k < 4; k++)
                {
                    int nx = cx + dx[k];
                    int ny = cy + dy[k];
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                    {
                        int nidx = ny * w + nx;
                        if (!isBg[nidx])
                        {
                            Color c = pixels[nidx];
                            float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
                            float min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
                            if (min >= 0.93f && (max - min) <= 0.035f)
                            {
                                isBg[nidx] = true;
                                q.Enqueue(nidx);
                            }
                        }
                    }
                }
            }

            Texture2D outTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color[] outPixels = new Color[w * h];

            for (int i = 0; i < w * h; i++)
            {
                if (isBg[i])
                {
                    outPixels[i] = Color.clear;
                }
                else
                {
                    Color c = pixels[i];
                    int cx = i % w;
                    int cy = i / w;

                    // Feather edge pixels adjacent to background
                    bool adjacentBg = false;
                    for (int k = 0; k < 4; k++)
                    {
                        int nx = cx + dx[k];
                        int ny = cy + dy[k];
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h && isBg[ny * w + nx])
                        {
                            adjacentBg = true;
                            break;
                        }
                    }

                    if (adjacentBg)
                    {
                        float brightness = (c.r + c.g + c.b) / 3f;
                        float alpha = Mathf.Clamp01((1f - brightness) / (1f - threshold));
                        c.a = Mathf.Max(0.15f, alpha);
                    }
                    else
                    {
                        c.a = 1f;
                    }
                    outPixels[i] = c;
                }
            }

            outTex.SetPixels(outPixels);
            outTex.Apply();
            return outTex;
        }

        private static Texture2D ProcessSphericalGachaBall(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D src = new Texture2D(2, 2);
            src.LoadImage(bytes);

            int w = src.width;
            int h = src.height;
            float cx = w * 0.50f;
            float cy = h * 0.49f; // The sphere is centered slightly below middle in the original render
            float radius = w * 0.355f;

            Texture2D outTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist > radius + 1f)
                    {
                        outTex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        Color c = src.GetPixel(x, y);
                        if (dist > radius - 1.5f)
                        {
                            c.a = Mathf.Clamp01(radius + 1f - dist);
                        }
                        outTex.SetPixel(x, y, c);
                    }
                }
            }
            outTex.Apply();
            return outTex;
        }

        private static void CleanCloudMascotEdges(Texture2D tex)
        {
            int w = tex.width;
            int h = tex.height;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = tex.GetPixel(x, y);
                    if (c.a <= 0.01f) continue;

                    // Remove ground contact shadow at bottom
                    if (y < h * 0.16f && (x > w * 0.65f || x < w * 0.30f))
                    {
                        float maxDiff = Mathf.Max(Mathf.Abs(c.r - c.g), Mathf.Max(Mathf.Abs(c.r - c.b), Mathf.Abs(c.g - c.b)));
                        if (maxDiff < 0.12f && c.r > 0.60f)
                        {
                            tex.SetPixel(x, y, Color.clear);
                            continue;
                        }
                    }

                    // Remove top corners
                    if (y > h * 0.78f && (x < w * 0.22f || x > w * 0.78f))
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
        }

        private static void CleanGachaMachineEdges(Texture2D tex)
        {
            int w = tex.width;
            int h = tex.height;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (y > h * 0.85f && (x < w * 0.20f || x > w * 0.80f))
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
        }

        private static Texture2D CreateAvatarWithPlate(Texture2D mascotCutout, int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float radius = size * 0.46f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    Color c = Color.clear;
                    if (dist <= radius)
                    {
                        float edge = Mathf.Clamp01(radius - dist);
                        // Soft white plate with pastel blue rim
                        float rimT = Mathf.Clamp01((dist - (radius - 12f)) / 12f);
                        Color fill = Color.Lerp(Color.white, new Color(0.88f, 0.94f, 1f, 1f), rimT);
                        fill.a = edge;
                        c = fill;
                    }
                    tex.SetPixel(x, y, c);
                }
            }

            // Overlay scaled mascot
            int mw = mascotCutout.width;
            int mh = mascotCutout.height;
            float scale = (size * 0.80f) / Mathf.Max(mw, mh);
            int targetW = Mathf.RoundToInt(mw * scale);
            int targetH = Mathf.RoundToInt(mh * scale);
            int startX = (size - targetW) / 2;
            int startY = (size - targetH) / 2;

            for (int y = 0; y < targetH; y++)
            {
                for (int x = 0; x < targetW; x++)
                {
                    float u = (float)x / targetW;
                    float v = (float)y / targetH;
                    Color mc = mascotCutout.GetPixelBilinear(u, v);
                    if (mc.a > 0.01f)
                    {
                        int px = startX + x;
                        int py = startY + y;
                        Color bg = tex.GetPixel(px, py);
                        Color blended = Color.Lerp(bg, mc, mc.a);
                        blended.a = Mathf.Max(bg.a, mc.a);
                        tex.SetPixel(px, py, blended);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateArcadeCoin(int size, bool isGold)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;
            float cy = size * 0.5f;
            float r = size * 0.45f;
            float rInner = size * 0.36f;

            Color baseCol = isGold ? new Color(1.0f, 0.76f, 0.16f) : new Color(0.80f, 0.84f, 0.90f);
            Color lightCol = isGold ? new Color(1.0f, 0.95f, 0.60f) : new Color(0.96f, 0.98f, 1.0f);
            Color darkCol = isGold ? new Color(0.76f, 0.48f, 0.06f) : new Color(0.50f, 0.54f, 0.62f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist > r + 3f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    // Drop shadow
                    float shDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy - 4f));
                    if (dist > r && shDist <= r + 3f)
                    {
                        tex.SetPixel(x, y, new Color(0, 0, 0, 0.25f));
                        continue;
                    }

                    float angle = Mathf.Atan2(y - cy, x - cx);
                    float shine = Mathf.Cos(angle - 0.785f); // 45 deg light angle

                    Color col;
                    if (dist > rInner)
                    {
                        // Outer Beveled Rim
                        Color rimTone = (shine > 0) ? Color.Lerp(baseCol, lightCol, shine * 0.7f) : Color.Lerp(baseCol, darkCol, -shine * 0.7f);
                        col = rimTone;
                    }
                    else
                    {
                        // Recessed Inner Face
                        Color innerTone = (shine > 0) ? Color.Lerp(baseCol * 0.95f, lightCol * 0.95f, shine * 0.5f) : Color.Lerp(baseCol * 0.95f, darkCol, -shine * 0.5f);
                        col = innerTone;
                    }

                    // Smooth edge AA
                    if (dist > r - 1.5f)
                    {
                        col.a = Mathf.Clamp01(r + 1f - dist);
                    }

                    tex.SetPixel(x, y, col);
                }
            }

            // Draw star in center
            int starPoints = 5;
            float rStarOut = size * 0.19f;
            float rStarIn = size * 0.085f;
            for (int y = (int)(cy - rStarOut - 2); y <= (int)(cy + rStarOut + 2); y++)
            {
                for (int x = (int)(cx - rStarOut - 2); x <= (int)(cx + rStarOut + 2); x++)
                {
                    float angle = Mathf.Atan2(y - cy, x - cx) + Mathf.PI * 0.5f;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float theta = Mathf.Repeat(angle, 2f * Mathf.PI / starPoints) - (Mathf.PI / starPoints);
                    float starR = rStarIn / Mathf.Cos(theta);
                    if (Mathf.Abs(theta) < Mathf.PI / (starPoints * 2f))
                    {
                        float t = Mathf.Abs(theta) / (Mathf.PI / starPoints);
                        starR = Mathf.Lerp(rStarOut, rStarIn, t * 2f);
                    }

                    if (dist <= starR)
                    {
                        Color orig = tex.GetPixel(x, y);
                        Color starCol = isGold ? new Color(1.0f, 0.98f, 0.82f) : Color.white;
                        tex.SetPixel(x, y, Color.Lerp(orig, starCol, 0.85f));
                    }
                }
            }

            // Add specular gleam glint
            float glintX = cx - size * 0.22f;
            float glintY = cy + size * 0.22f;
            for (int y = (int)glintY - 10; y <= (int)glintY + 10; y++)
            {
                for (int x = (int)glintX - 10; x <= (int)glintX + 10; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(glintX, glintY));
                    if (d < 10f)
                    {
                        Color orig = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(orig, Color.white, (10f - d) / 10f * 0.8f));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateInactivePillButton(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float radius = (height - 8f) * 0.46f;
            float bevelHeight = 6f;

            Color baseCol = new Color(0.96f, 0.94f, 0.98f, 1f); // Soft milky lavender
            Color rimCol = new Color(0.86f, 0.80f, 0.92f, 1f);  // Elegant lavender border
            Color cushionCol = new Color(0.82f, 0.76f, 0.90f, 1f); // Subtle bottom depth cushion

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Shadow
                    float shCx = Mathf.Clamp(x, radius + 4f, width - 1 - radius - 4f);
                    float shCy = radius + 2f;
                    float shDist = Vector2.Distance(new Vector2(x, y), new Vector2(shCx, shCy));
                    float shAlpha = 0f;
                    if (y < radius + 6f && shDist <= radius + 4f)
                    {
                        shAlpha = Mathf.Clamp01((radius + 4f - shDist) / 4f) * 0.15f;
                    }

                    // Bottom cushion
                    float extCx = Mathf.Clamp(x, radius, width - 1 - radius);
                    float extCy = radius + 2f;
                    float extDist = Vector2.Distance(new Vector2(x, y), new Vector2(extCx, extCy));
                    float extAlpha = 0f;
                    if (y <= radius + bevelHeight && extDist <= radius + 1f)
                    {
                        extAlpha = Mathf.Clamp01(radius + 1f - extDist);
                    }

                    // Top face
                    float topCx = Mathf.Clamp(x, radius, width - 1 - radius);
                    float topCy = radius + bevelHeight;
                    float topDist = Vector2.Distance(new Vector2(x, y), new Vector2(topCx, topCy));
                    float topAlpha = 0f;
                    Color faceColor = Color.clear;

                    if (topDist <= radius + 1f)
                    {
                        topAlpha = Mathf.Clamp01(radius + 1f - topDist);
                        float normY = Mathf.Clamp01((y - bevelHeight) / (height - bevelHeight));

                        faceColor = Color.Lerp(baseCol * 0.96f, Color.white, normY);

                        // Rim
                        if (topDist > radius - 2.5f)
                        {
                            float rimT = (topDist - (radius - 2.5f)) / 2.5f;
                            faceColor = Color.Lerp(faceColor, rimCol, rimT * 0.9f);
                        }
                    }

                    Color pix = Color.clear;
                    if (shAlpha > 0f) pix = new Color(0.2f, 0.15f, 0.3f, shAlpha);
                    if (extAlpha > 0f)
                    {
                        Color c = cushionCol;
                        c.a = extAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, c, extAlpha) : c;
                    }
                    if (topAlpha > 0f)
                    {
                        faceColor.a = topAlpha;
                        pix = (pix.a > 0f) ? Color.Lerp(pix, faceColor, topAlpha) : faceColor;
                    }

                    tex.SetPixel(x, y, pix);
                }
            }

            tex.Apply();
            return tex;
        }

        private static void ConfigureSprite(string path, Vector4 border)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.isReadable = true;
                importer.spriteBorder = border;
                importer.SaveAndReimport();
            }
        }
    }
}
#endif
