#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Survivor2D.Editor
{
    public static class SpriteTextureUtility
    {
        public static Sprite LoadSpriteWithChromaKey(string path, Color keyColor, float tolerance, float pixelsPerUnit = 100f)
        {
            if (!File.Exists(path)) return null;

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D sourceTex = new Texture2D(2, 2);
            if (!sourceTex.LoadImage(fileData)) return null;

            int w = sourceTex.width;
            int h = sourceTex.height;
            Color[] pixels = sourceTex.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color p = pixels[i];
                float diff = Mathf.Abs(p.r - keyColor.r) + Mathf.Abs(p.g - keyColor.g) + Mathf.Abs(p.b - keyColor.b);
                if (diff <= tolerance)
                {
                    pixels[i] = Color.clear;
                }
            }

            Texture2D transparentTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            transparentTex.filterMode = FilterMode.Bilinear;
            transparentTex.SetPixels(pixels);
            transparentTex.Apply();

            return Sprite.Create(transparentTex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
    }
}
#endif
