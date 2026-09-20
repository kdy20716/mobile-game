using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class JellyFireworksEffect : MonoBehaviour
    {
        [Header("Glow & Sparkle Sprites")]
        [SerializeField] private Sprite glowOrbSprite;
        [SerializeField] private Sprite sparkleStarSprite;
        [SerializeField] private Sprite shockwaveRingSprite;

        // Legacy compatibility
        [SerializeField] private Sprite particleSprite;

        private static Sprite _fallbackOrb;
        private static Sprite _fallbackStar;
        private static Sprite _fallbackRing;

        // Sweet Pastel Fireworks Color Palette
        private static readonly Color[] PastelColors = new Color[]
        {
            new Color(1.00f, 0.45f, 0.65f), // Strawberry Pink
            new Color(0.32f, 0.90f, 0.75f), // Mint Soda
            new Color(1.00f, 0.88f, 0.32f), // Lemon Gold
            new Color(0.72f, 0.55f, 1.00f), // Lavender Purple
            new Color(0.38f, 0.80f, 1.00f), // Sky Blue
            new Color(1.00f, 0.62f, 0.42f), // Sweet Peach
            new Color(1.00f, 1.00f, 1.00f)  // Marshmallow White Sparkle
        };

        private void Awake()
        {
            EnsureFallbackSprites();
        }

        public void SetupSprites(Sprite orb, Sprite star, Sprite ring)
        {
            glowOrbSprite = orb;
            sparkleStarSprite = star;
            shockwaveRingSprite = ring;
        }

        private void EnsureFallbackSprites()
        {
            if (glowOrbSprite == null)
            {
                if (_fallbackOrb == null) _fallbackOrb = CreateRuntimeGlowOrb();
                glowOrbSprite = _fallbackOrb;
            }
            if (sparkleStarSprite == null)
            {
                if (_fallbackStar == null) _fallbackStar = CreateRuntimeSparkleStar();
                sparkleStarSprite = _fallbackStar;
            }
            if (shockwaveRingSprite == null)
            {
                if (_fallbackRing == null) _fallbackRing = CreateRuntimeShockwaveRing();
                shockwaveRingSprite = _fallbackRing;
            }
        }

        public void TriggerFireworks(Vector2 localPos, int count = 42, float speedMultiplier = 1.25f)
        {
            EnsureFallbackSprites();

            // 1. Shockwave Ripple Ring Burst (Scaled up 2.5x)
            StartCoroutine(SpawnShockwaveRoutine(localPos));

            // 2. Central Glow Flash (Scaled up 2.5x)
            StartCoroutine(SpawnCenterFlashRoutine(localPos));

            // 3. Multi-type Glowing Pastel Particles (Scaled up 2.5x)
            StartCoroutine(SpawnBurstRoutine(localPos, count, speedMultiplier));
        }

        private IEnumerator SpawnShockwaveRoutine(Vector2 localPos)
        {
            if (shockwaveRingSprite == null) yield break;

            GameObject ringObj = new GameObject("ShockwaveRing", typeof(RectTransform), typeof(Image));
            ringObj.transform.SetParent(transform, false);

            RectTransform rt = ringObj.GetComponent<RectTransform>();
            rt.anchoredPosition = localPos;
            rt.sizeDelta = new Vector2(120f, 120f); // 2.5x scale (was 48f)

            Image img = ringObj.GetComponent<Image>();
            img.sprite = shockwaveRingSprite;
            img.raycastTarget = false;
            img.color = new Color(1f, 0.96f, 0.85f, 0.90f);

            float duration = 0.38f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float scale = Mathf.Lerp(0.25f, 3.4f, Mathf.Sqrt(t));
                rt.localScale = new Vector3(scale, scale, 1f);

                float alpha = Mathf.Clamp01(1f - t);
                img.color = new Color(1f, 0.96f, 0.85f, alpha * 0.90f);

                yield return null;
            }

            Destroy(ringObj);
        }

        private IEnumerator SpawnCenterFlashRoutine(Vector2 localPos)
        {
            if (glowOrbSprite == null) yield break;

            GameObject flashObj = new GameObject("CenterFlash", typeof(RectTransform), typeof(Image));
            flashObj.transform.SetParent(transform, false);

            RectTransform rt = flashObj.GetComponent<RectTransform>();
            rt.anchoredPosition = localPos;
            rt.sizeDelta = new Vector2(200f, 200f); // 2.5x scale (was 80f)

            Image img = flashObj.GetComponent<Image>();
            img.sprite = glowOrbSprite;
            img.raycastTarget = false;
            img.color = new Color(1f, 1f, 1f, 0.95f);

            float duration = 0.24f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float scale = Mathf.Lerp(0.4f, 2.0f, Mathf.Sqrt(t));
                rt.localScale = new Vector3(scale, scale, 1f);

                float alpha = Mathf.Clamp01(1f - t * t);
                img.color = new Color(1f, 1f, 1f, alpha * 0.95f);

                yield return null;
            }

            Destroy(flashObj);
        }

        private IEnumerator SpawnBurstRoutine(Vector2 localPos, int count, float speedMultiplier)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnSingleParticle(localPos, speedMultiplier);
            }
            yield return null;
        }

        private void SpawnSingleParticle(Vector2 localPos, float speedMultiplier)
        {
            GameObject pObj = new GameObject("JellyParticle", typeof(RectTransform), typeof(Image));
            pObj.transform.SetParent(transform, false);

            RectTransform rt = pObj.GetComponent<RectTransform>();
            rt.anchoredPosition = localPos;

            Image img = pObj.GetComponent<Image>();
            img.raycastTarget = false;

            Color chosenCol = PastelColors[Random.Range(0, PastelColors.Length)];
            img.color = chosenCol;

            // Particle Type: 0 = Glow Orb (65%), 1 = Sparkle Star (25%), 2 = Fairy Dust Glimmer (10%)
            float typeRoll = Random.value;
            Sprite chosenSprite = glowOrbSprite;
            float size = Random.Range(45f, 85f); // 2.5x scale (was 18f ~ 34f)
            bool isStar = false;
            bool isFairyDust = false;

            if (typeRoll < 0.65f)
            {
                chosenSprite = glowOrbSprite;
                size = Random.Range(45f, 85f); // 2.5x scale
            }
            else if (typeRoll < 0.90f)
            {
                chosenSprite = sparkleStarSprite != null ? sparkleStarSprite : glowOrbSprite;
                size = Random.Range(55f, 100f); // 2.5x scale (was 22f ~ 40f)
                isStar = true;
            }
            else
            {
                chosenSprite = glowOrbSprite;
                size = Random.Range(22f, 38f); // 2.5x scale (was 9f ~ 15f)
                isFairyDust = true;
                if (Random.value < 0.5f) chosenCol = Color.white;
                img.color = chosenCol;
            }

            img.sprite = chosenSprite;
            rt.sizeDelta = new Vector2(size, size);

            StartCoroutine(AnimateParticle(rt, img, chosenCol, speedMultiplier, isStar, isFairyDust));
        }

        private IEnumerator AnimateParticle(RectTransform rt, Image img, Color baseCol, float speedMultiplier, bool isStar, bool isFairyDust)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = (isFairyDust ? Random.Range(120f, 320f) : Random.Range(260f, 750f)) * speedMultiplier;
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            float gravity = isFairyDust ? Random.Range(180f, 320f) : Random.Range(300f, 540f);
            float rotSpeed = isStar ? Random.Range(-380f, 380f) : Random.Range(-120f, 120f);
            float phase = Random.Range(0f, Mathf.PI * 2f);

            float duration = isFairyDust ? Random.Range(1.2f, 1.7f) : Random.Range(0.95f, 1.45f);
            float elapsed = 0f;
            Vector3 baseScale = rt.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Air resistance deceleration
                velocity.x *= Mathf.Pow(0.88f, Time.deltaTime * 60f);
                velocity.y *= Mathf.Pow(0.92f, Time.deltaTime * 60f);
                velocity.y -= gravity * Time.deltaTime;

                // Fairy dust horizontal wind sway
                if (isFairyDust)
                {
                    rt.anchoredPosition += new Vector2(Mathf.Sin(elapsed * 6f + phase) * 40f * Time.deltaTime, 0f);
                }

                rt.anchoredPosition += velocity * Time.deltaTime;
                rt.Rotate(0, 0, rotSpeed * Time.deltaTime);

                // Sudden burst scale pop -> sparkle twinkle -> gentle fade
                float popT = Mathf.Clamp01(elapsed / 0.12f);
                float popScale = Mathf.Lerp(0.25f, 1.25f, popT);

                // Twinkle oscillation
                float twinkle = isStar ? (1f + 0.32f * Mathf.Sin(elapsed * 25f + phase)) : (1f + 0.15f * Mathf.Sin(elapsed * 16f + phase));

                // Fade scale & alpha
                float shrink = Mathf.Clamp01(1f - t * 0.45f);
                rt.localScale = baseScale * (popScale * twinkle * shrink);

                float alpha = Mathf.Clamp01(1f - Mathf.Pow(t, 1.8f));
                img.color = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);

                yield return null;
            }

            Destroy(rt.gameObject);
        }

        // ==========================================
        // Procedural Fallback Sprites (Runtime Safe)
        // ==========================================
        private static Sprite CreateRuntimeGlowOrb()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float maxR = size * 0.46f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cx));
                    float u = dist / maxR;
                    if (u >= 1f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                    else
                    {
                        float core = Mathf.Clamp01(1f - u / 0.32f);
                        core = core * core;
                        float halo = Mathf.Pow(Mathf.Clamp01(1f - u), 1.6f);
                        float alpha = Mathf.Clamp01(halo * 0.94f + core * 0.06f);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite CreateRuntimeSparkleStar()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float maxR = size * 0.46f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - cx) / maxR;
                    float dy = Mathf.Abs(y - cx) / maxR;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > 1.25f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                        continue;
                    }

                    float rayX = Mathf.Pow(Mathf.Clamp01(1f - dx), 3.5f) * Mathf.Pow(Mathf.Clamp01(1f - dy / 0.16f), 2f);
                    float rayY = Mathf.Pow(Mathf.Clamp01(1f - dy), 3.5f) * Mathf.Pow(Mathf.Clamp01(1f - dx / 0.16f), 2f);
                    float coreGlow = Mathf.Pow(Mathf.Clamp01(1f - dist / 0.38f), 2f);
                    float intensity = Mathf.Clamp01(rayX + rayY + coreGlow);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, intensity));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite CreateRuntimeShockwaveRing()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float maxR = size * 0.46f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cx));
                    float u = dist / maxR;
                    if (u > 1f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                    else
                    {
                        float diff = u - 0.68f;
                        float val = Mathf.Exp(-(diff * diff) / (2f * 0.13f * 0.13f));
                        float alpha = Mathf.Clamp01(val * 0.92f);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
