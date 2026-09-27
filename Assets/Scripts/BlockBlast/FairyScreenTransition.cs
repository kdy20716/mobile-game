using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class FairyScreenTransition : MonoBehaviour
    {
        public static FairyScreenTransition Instance { get; private set; }

        [Header("Sprites")]
        [SerializeField] private Sprite sparkleSprite;
        [SerializeField] private Sprite glowSprite;

        [Header("Settings")]
        [SerializeField] private float transitionDuration = 0.45f;
        [SerializeField] private float sparkleDuration = 1.0f; // Exactly 1 second fade-out

        private CanvasGroup _dissolveGroup;
        private Image _dissolveVeil;
        private RectTransform _rectTransform;

        // Fairy Pastel Color Palette
        private readonly Color[] _fairyColors = new Color[]
        {
            new Color(1.0f, 0.60f, 0.78f, 1.0f),  // Strawberry Pink
            new Color(0.40f, 0.95f, 0.85f, 1.0f),  // Mint Soda
            new Color(1.0f, 0.90f, 0.45f, 1.0f),  // Lemon Gold
            new Color(0.80f, 0.62f, 1.0f, 1.0f),  // Lavender Purple
            new Color(0.48f, 0.88f, 1.0f, 1.0f)   // Sky Blue
        };

        // --- Sparkle Object Pooling ---
        private class CornerSparkle
        {
            public RectTransform rt;
            public Image img;
            public float time;
            public Vector2 velocity;
            public float rotSpeed;
            public Color baseColor;
            public float initialScale;
        }

        private readonly Queue<CornerSparkle> _sparklePool = new Queue<CornerSparkle>();
        private readonly List<CornerSparkle> _activeSparkles = new List<CornerSparkle>();

        private Coroutine _currentTransition;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            _rectTransform = GetComponent<RectTransform>();

            // Setup Dissolve Veil
            _dissolveGroup = gameObject.AddComponent<CanvasGroup>();
            _dissolveGroup.alpha = 0f;
            _dissolveGroup.blocksRaycasts = false;

            _dissolveVeil = gameObject.AddComponent<Image>();
            _dissolveVeil.color = new Color(1.0f, 0.96f, 1.0f, 1.0f); // 100% opaque dreamy pastel veil (no bleed-through!)
            _dissolveVeil.raycastTarget = false;

            PrewarmSparkles();
        }

        public void SetupSprites(Sprite sparkle, Sprite glow)
        {
            sparkleSprite = sparkle;
            glowSprite = glow;
        }

        private void PrewarmSparkles()
        {
            for (int i = 0; i < 28; i++)
            {
                _sparklePool.Enqueue(CreateSparkle());
            }
        }

        private CornerSparkle CreateSparkle()
        {
            GameObject obj = new GameObject("CornerSparkle", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);

            Image img = obj.GetComponent<Image>();
            img.sprite = sparkleSprite != null ? sparkleSprite : glowSprite;
            img.raycastTarget = false;

            return new CornerSparkle
            {
                rt = obj.GetComponent<RectTransform>(),
                img = img
            };
        }

        private void Update()
        {
            UpdateSparkles();
        }

        // ==========================================
        // DISSOLVE & TRANSITION CONTROLLER
        // ==========================================

        /// <summary>
        /// 화면 전환 시 부드러운 디졸브 효과 + 화면 모서리 1초 반짝 가루 연출
        /// </summary>
        public void DoTransition(Action onHalfway, float customDuration = -1f)
        {
            float dur = customDuration > 0f ? customDuration : transitionDuration;
            if (_currentTransition != null) StopCoroutine(_currentTransition);
            _currentTransition = StartCoroutine(TransitionRoutine(onHalfway, dur));
        }

        private IEnumerator TransitionRoutine(Action onHalfway, float duration)
        {
            _dissolveGroup.blocksRaycasts = true;

            // 1. 화면 모서리 반짝 가루 뿌리기 (1초 지속)
            EmitCornerSparkles();

            // 2. Dissolve In (부드럽게 밝아지며 가려짐)
            float halfDur = duration * 0.45f;
            float elapsed = 0f;
            while (elapsed < halfDur)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / halfDur;
                _dissolveGroup.alpha = Mathf.SmoothStep(0f, 1f, t);
                yield return null;
            }
            _dissolveGroup.alpha = 1f;

            // 3. 화면 교체 (로비 ➔ 인게임, 메인 ➔ 로비 등)
            onHalfway?.Invoke();
            yield return new WaitForSecondsRealtime(0.04f);

            // 4. Dissolve Out (부드럽게 걷히며 새 화면 등장)
            elapsed = 0f;
            float outDur = duration * 0.55f;
            while (elapsed < outDur)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / outDur;
                _dissolveGroup.alpha = Mathf.SmoothStep(1f, 0f, t);
                yield return null;
            }

            _dissolveGroup.alpha = 0f;
            _dissolveGroup.blocksRaycasts = false;
            _currentTransition = null;
        }

        // ==========================================
        // CORNER STARDUST SPARKLES (1-SECOND DURATION)
        // ==========================================

        public void EmitCornerSparkles()
        {
            float halfW = _rectTransform.rect.width * 0.5f;
            float halfH = _rectTransform.rect.height * 0.5f;

            // 4 Corners & Edges
            Vector2[] cornerOrigins = new Vector2[]
            {
                new Vector2(-halfW + 40f, halfH - 40f),   // Top-Left
                new Vector2(halfW - 40f, halfH - 40f),    // Top-Right
                new Vector2(-halfW + 40f, -halfH + 40f),  // Bottom-Left
                new Vector2(halfW - 40f, -halfH + 40f),   // Bottom-Right
                new Vector2(0f, halfH - 30f),             // Top-Center
                new Vector2(0f, -halfH + 30f)             // Bottom-Center
            };

            for (int i = 0; i < cornerOrigins.Length; i++)
            {
                Vector2 origin = cornerOrigins[i];
                // Direction pointing gently toward screen center
                Vector2 toCenter = (-origin).normalized;

                int count = UnityEngine.Random.Range(2, 4);
                for (int j = 0; j < count; j++)
                {
                    Vector2 jitter = UnityEngine.Random.insideUnitCircle * 50f;
                    Vector2 spawnPos = origin + jitter;

                    float angle = Mathf.Atan2(toCenter.y, toCenter.x) + UnityEngine.Random.Range(-0.6f, 0.6f);
                    float speed = UnityEngine.Random.Range(60f, 150f);
                    Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

                    Color col = _fairyColors[UnityEngine.Random.Range(0, _fairyColors.Length)];
                    SpawnSparkle(spawnPos, vel, col);
                }
            }
        }

        private void SpawnSparkle(Vector2 pos, Vector2 vel, Color col)
        {
            CornerSparkle s = _sparklePool.Count > 0 ? _sparklePool.Dequeue() : CreateSparkle();
            s.rt.anchoredPosition = pos;
            s.velocity = vel;
            s.rotSpeed = UnityEngine.Random.Range(-260f, 260f);
            s.initialScale = UnityEngine.Random.Range(24f, 40f);
            s.rt.sizeDelta = new Vector2(s.initialScale, s.initialScale);
            s.rt.localScale = Vector3.one;
            s.rt.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
            s.time = 0f;
            s.baseColor = col;
            s.img.color = col;
            s.rt.gameObject.SetActive(true);

            _activeSparkles.Add(s);
        }

        private void UpdateSparkles()
        {
            for (int i = _activeSparkles.Count - 1; i >= 0; i--)
            {
                CornerSparkle s = _activeSparkles[i];
                s.time += Time.unscaledDeltaTime;
                float progress = s.time / sparkleDuration; // Exactly 1.0 second

                if (progress >= 1f)
                {
                    s.rt.gameObject.SetActive(false);
                    _activeSparkles.RemoveAt(i);
                    _sparklePool.Enqueue(s);
                }
                else
                {
                    // Move & gentle drift
                    s.rt.anchoredPosition += s.velocity * Time.unscaledDeltaTime;
                    s.velocity *= (1f - 1.8f * Time.unscaledDeltaTime);

                    // Rotate
                    s.rt.Rotate(0, 0, s.rotSpeed * Time.unscaledDeltaTime);

                    // Smooth scale & 1-second alpha fadeout
                    float scale = 1f - Mathf.Pow(progress, 2.5f);
                    s.rt.localScale = new Vector3(scale, scale, 1f);

                    float alpha = Mathf.Clamp01(1f - progress);
                    s.img.color = new Color(s.baseColor.r, s.baseColor.g, s.baseColor.b, alpha);
                }
            }
        }
    }
}
