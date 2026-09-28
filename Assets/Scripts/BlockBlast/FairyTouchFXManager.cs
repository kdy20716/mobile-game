using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class FairyTouchFXManager : MonoBehaviour
    {
        public static FairyTouchFXManager Instance { get; private set; }

        [Header("Sprites")]
        [SerializeField] private Sprite rippleSprite;
        [SerializeField] private Sprite sparkleSprite;
        [SerializeField] private Sprite trailSprite;

        [Header("Settings")]
        [SerializeField] private float rippleDuration = 0.38f;
        [SerializeField] private float sparkleDuration = 0.40f;
        [SerializeField] private float trailDuration = 0.35f;
        [SerializeField] private float trailSpacing = 16f; // Minimum distance between trail stars

        // Fairy Pastel Color Palette
        private readonly Color[] _fairyColors = new Color[]
        {
            new Color(1.0f, 0.52f, 0.72f, 1.0f),  // Strawberry Pink
            new Color(0.35f, 0.95f, 0.82f, 1.0f),  // Mint Soda
            new Color(1.0f, 0.88f, 0.38f, 1.0f),  // Lemon Gold
            new Color(0.76f, 0.56f, 1.0f, 1.0f),  // Lavender Purple
            new Color(0.42f, 0.86f, 1.0f, 1.0f),  // Sky Blue
            new Color(1.0f, 0.72f, 0.88f, 1.0f)   // Cotton Candy
        };

        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private Camera _uiCamera;

        // --- Object Pools & Active Lists ---
        private class RippleItem
        {
            public RectTransform rt;
            public Image img;
            public float time;
            public Color baseColor;
        }

        private class SparkleItem
        {
            public RectTransform rt;
            public Image img;
            public float time;
            public Vector2 velocity;
            public float rotSpeed;
            public float initialSize;
            public Color baseColor;
        }

        private class TrailItem
        {
            public RectTransform rt;
            public Image img;
            public float time;
            public float rotSpeed;
            public float initialSize;
            public Vector2 drift;
            public Color baseColor;
        }

        private readonly Queue<RippleItem> _ripplePool = new Queue<RippleItem>();
        private readonly List<RippleItem> _activeRipples = new List<RippleItem>();

        private readonly Queue<SparkleItem> _sparklePool = new Queue<SparkleItem>();
        private readonly List<SparkleItem> _activeSparkles = new List<SparkleItem>();

        private readonly Queue<TrailItem> _trailPool = new Queue<TrailItem>();
        private readonly List<TrailItem> _activeTrails = new List<TrailItem>();

        // Tracking touches for trail
        private readonly Dictionary<int, Vector2> _lastPointerPositions = new Dictionary<int, Vector2>();
        private bool _isMouseDragging = false;
        private Vector2 _lastMousePos;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = _parentCanvas.worldCamera;
            }

            PrewarmPools();
        }

        public void SetupSprites(Sprite ripple, Sprite sparkle, Sprite trail)
        {
            rippleSprite = ripple;
            sparkleSprite = sparkle;
            trailSprite = trail;
        }

        private void PrewarmPools()
        {
            // Pre-create 12 ripples
            for (int i = 0; i < 12; i++)
            {
                _ripplePool.Enqueue(CreateRippleObject());
            }

            // Pre-create 45 sparkles
            for (int i = 0; i < 45; i++)
            {
                _sparklePool.Enqueue(CreateSparkleObject());
            }

            // Pre-create 70 trail particles
            for (int i = 0; i < 70; i++)
            {
                _trailPool.Enqueue(CreateTrailObject());
            }
        }

        private RippleItem CreateRippleObject()
        {
            GameObject obj = new GameObject("RippleFX", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);

            Image img = obj.GetComponent<Image>();
            img.sprite = rippleSprite;
            img.raycastTarget = false;

            return new RippleItem
            {
                rt = obj.GetComponent<RectTransform>(),
                img = img
            };
        }

        private SparkleItem CreateSparkleObject()
        {
            GameObject obj = new GameObject("SparkleFX", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);

            Image img = obj.GetComponent<Image>();
            img.sprite = sparkleSprite != null ? sparkleSprite : rippleSprite;
            img.raycastTarget = false;

            return new SparkleItem
            {
                rt = obj.GetComponent<RectTransform>(),
                img = img
            };
        }

        private TrailItem CreateTrailObject()
        {
            GameObject obj = new GameObject("TrailFX", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);

            Image img = obj.GetComponent<Image>();
            img.sprite = trailSprite != null ? trailSprite : sparkleSprite;
            img.raycastTarget = false;

            return new TrailItem
            {
                rt = obj.GetComponent<RectTransform>(),
                img = img
            };
        }

        private void Update()
        {
            HandleInput();
            UpdateRipples();
            UpdateSparkles();
            UpdateTrails();
        }

        // ==========================================
        // INPUT DETECTION (CROSS-PLATFORM)
        // ==========================================

        private void HandleInput()
        {
#if ENABLE_INPUT_SYSTEM
            // 1. New Input System: Touchscreen
            var ts = UnityEngine.InputSystem.Touchscreen.current;
            if (ts != null)
            {
                var touches = ts.touches;
                for (int i = 0; i < touches.Count; i++)
                {
                    var touch = touches[i];
                    if (!touch.isInProgress) continue;

                    Vector2 screenPos = touch.position.ReadValue();
                    int id = touch.touchId.ReadValue();

                    if (touch.press.wasPressedThisFrame)
                    {
                        TriggerTouch(screenPos);
                        _lastPointerPositions[id] = screenPos;
                    }
                    else if (touch.press.isPressed)
                    {
                        if (_lastPointerPositions.TryGetValue(id, out Vector2 lastPos))
                        {
                            float dist = Vector2.Distance(lastPos, screenPos);
                            if (dist >= trailSpacing)
                            {
                                TriggerTrail(lastPos, screenPos, dist);
                                _lastPointerPositions[id] = screenPos;
                            }
                        }
                        else
                        {
                            _lastPointerPositions[id] = screenPos;
                        }
                    }
                    else if (touch.press.wasReleasedThisFrame)
                    {
                        _lastPointerPositions.Remove(id);
                    }
                }
            }

            // 2. New Input System: Mouse
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null)
            {
                Vector2 mousePos = mouse.position.ReadValue();
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    _isMouseDragging = true;
                    _lastMousePos = mousePos;
                    TriggerTouch(_lastMousePos);
                }
                else if (mouse.leftButton.isPressed && _isMouseDragging)
                {
                    float dist = Vector2.Distance(_lastMousePos, mousePos);
                    if (dist >= trailSpacing)
                    {
                        TriggerTrail(_lastMousePos, mousePos, dist);
                        _lastMousePos = mousePos;
                    }
                }
                else if (mouse.leftButton.wasReleasedThisFrame)
                {
                    _isMouseDragging = false;
                }
            }
#else
            // 1. Mobile Multi-Touch
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 screenPos = touch.position;
                    int id = touch.fingerId;

                    if (touch.phase == TouchPhase.Began)
                    {
                        TriggerTouch(screenPos);
                        _lastPointerPositions[id] = screenPos;
                    }
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (_lastPointerPositions.TryGetValue(id, out Vector2 lastPos))
                        {
                            float dist = Vector2.Distance(lastPos, screenPos);
                            if (dist >= trailSpacing)
                            {
                                TriggerTrail(lastPos, screenPos, dist);
                                _lastPointerPositions[id] = screenPos;
                            }
                        }
                        else
                        {
                            _lastPointerPositions[id] = screenPos;
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _lastPointerPositions.Remove(id);
                    }
                }
                return;
            }

            // 2. Mouse / Simulator Input
            if (Input.GetMouseButtonDown(0))
            {
                _isMouseDragging = true;
                _lastMousePos = Input.mousePosition;
                TriggerTouch(_lastMousePos);
            }
            else if (Input.GetMouseButton(0) && _isMouseDragging)
            {
                Vector2 currentPos = Input.mousePosition;
                float dist = Vector2.Distance(_lastMousePos, currentPos);
                if (dist >= trailSpacing)
                {
                    TriggerTrail(_lastMousePos, currentPos, dist);
                    _lastMousePos = currentPos;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isMouseDragging = false;
            }
#endif
        }

        // ==========================================
        // EFFECT EMITTERS
        // ==========================================

        public void TriggerTouch(Vector2 screenPos)
        {
            if (!ScreenPointToLocal(screenPos, out Vector2 localPos)) return;

            Color chosenColor = _fairyColors[Random.Range(0, _fairyColors.Length)];

            // 1. Spawn Expanding Ripple Ring
            SpawnRipple(localPos, chosenColor);

            // 2. Spawn 5~7 Sparkle Burst Stars
            int sparkleCount = Random.Range(5, 8);
            for (int i = 0; i < sparkleCount; i++)
            {
                Color spkColor = _fairyColors[Random.Range(0, _fairyColors.Length)];
                SpawnSparkle(localPos, spkColor);
            }
        }

        public void TriggerTrail(Vector2 startScreenPos, Vector2 endScreenPos, float dist)
        {
            int count = Mathf.Clamp(Mathf.FloorToInt(dist / trailSpacing), 1, 4);
            for (int i = 0; i < count; i++)
            {
                float t = (float)(i + 1) / count;
                Vector2 screenPos = Vector2.Lerp(startScreenPos, endScreenPos, t);
                if (ScreenPointToLocal(screenPos, out Vector2 localPos))
                {
                    // Slight random jitter for organic stardust feel
                    Vector2 jitter = Random.insideUnitCircle * 8f;
                    Color trailColor = _fairyColors[Random.Range(0, _fairyColors.Length)];
                    SpawnTrail(localPos + jitter, trailColor);
                }
            }
        }

        private void SpawnRipple(Vector2 localPos, Color col)
        {
            RippleItem item = _ripplePool.Count > 0 ? _ripplePool.Dequeue() : CreateRippleObject();
            item.rt.anchoredPosition = localPos;
            item.rt.localScale = Vector3.one * 0.2f;
            item.rt.sizeDelta = new Vector2(140f, 140f);
            item.time = 0f;
            item.baseColor = col;
            item.img.color = new Color(col.r, col.g, col.b, 0.85f);
            item.rt.gameObject.SetActive(true);

            _activeRipples.Add(item);
        }

        private void SpawnSparkle(Vector2 localPos, Color col)
        {
            SparkleItem item = _sparklePool.Count > 0 ? _sparklePool.Dequeue() : CreateSparkleObject();
            item.rt.anchoredPosition = localPos;

            // Random burst velocity
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float speed = Random.Range(160f, 340f);
            item.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            item.rotSpeed = Random.Range(-400f, 400f);
            item.initialSize = Random.Range(20f, 34f);
            item.rt.sizeDelta = new Vector2(item.initialSize, item.initialSize);
            item.rt.localScale = Vector3.one;
            item.rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            item.time = 0f;
            item.baseColor = col;
            item.img.color = col;
            item.rt.gameObject.SetActive(true);

            _activeSparkles.Add(item);
        }

        private void SpawnTrail(Vector2 localPos, Color col)
        {
            TrailItem item = _trailPool.Count > 0 ? _trailPool.Dequeue() : CreateTrailObject();
            item.rt.anchoredPosition = localPos;

            item.rotSpeed = Random.Range(-250f, 250f);
            item.initialSize = Random.Range(18f, 30f);
            item.drift = (Random.insideUnitCircle + Vector2.up * 0.5f) * 20f; // Gentle upward fairy float
            item.rt.sizeDelta = new Vector2(item.initialSize, item.initialSize);
            item.rt.localScale = Vector3.one;
            item.rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            item.time = 0f;
            item.baseColor = col;
            item.img.color = col;
            item.rt.gameObject.SetActive(true);

            _activeTrails.Add(item);
        }

        // ==========================================
        // UPDATE ANIMATIONS
        // ==========================================

        private void UpdateRipples()
        {
            float dt = Time.unscaledDeltaTime;
            for (int i = _activeRipples.Count - 1; i >= 0; i--)
            {
                RippleItem r = _activeRipples[i];
                r.time += dt;
                float progress = r.time / rippleDuration;

                if (progress >= 1f)
                {
                    r.rt.gameObject.SetActive(false);
                    _activeRipples.RemoveAt(i);
                    _ripplePool.Enqueue(r);
                }
                else
                {
                    // Juicy cubic ease-out expand: 0.2 -> 1.75
                    float scaleEase = 1f - Mathf.Pow(1f - progress, 3f);
                    float scale = Mathf.Lerp(0.2f, 1.75f, scaleEase);
                    r.rt.localScale = new Vector3(scale, scale, 1f);

                    // Smooth alpha fade
                    float alpha = Mathf.Lerp(0.85f, 0f, progress * progress);
                    r.img.color = new Color(r.baseColor.r, r.baseColor.g, r.baseColor.b, alpha);
                }
            }
        }

        private void UpdateSparkles()
        {
            float dt = Time.unscaledDeltaTime;
            for (int i = _activeSparkles.Count - 1; i >= 0; i--)
            {
                SparkleItem s = _activeSparkles[i];
                s.time += dt;
                float progress = s.time / sparkleDuration;

                if (progress >= 1f)
                {
                    s.rt.gameObject.SetActive(false);
                    _activeSparkles.RemoveAt(i);
                    _sparklePool.Enqueue(s);
                }
                else
                {
                    // Move & gentle deceleration
                    s.rt.anchoredPosition += s.velocity * dt;
                    s.velocity *= Mathf.Clamp01(1f - 4.5f * dt);

                    // Gentle fairy upward float
                    s.rt.anchoredPosition += Vector2.up * (25f * dt);

                    // Rotate
                    s.rt.Rotate(0, 0, s.rotSpeed * dt);

                    // Shrink & fade
                    float scale = 1f - Mathf.Pow(progress, 2f);
                    s.rt.localScale = new Vector3(scale, scale, 1f);
                    float alpha = Mathf.Clamp01(1f - progress);
                    s.img.color = new Color(s.baseColor.r, s.baseColor.g, s.baseColor.b, alpha);
                }
            }
        }

        private void UpdateTrails()
        {
            float dt = Time.unscaledDeltaTime;
            for (int i = _activeTrails.Count - 1; i >= 0; i--)
            {
                TrailItem t = _activeTrails[i];
                t.time += dt;
                float progress = t.time / trailDuration;

                if (progress >= 1f)
                {
                    t.rt.gameObject.SetActive(false);
                    _activeTrails.RemoveAt(i);
                    _trailPool.Enqueue(t);
                }
                else
                {
                    // Gentle drift
                    t.rt.anchoredPosition += t.drift * dt;
                    t.rt.Rotate(0, 0, t.rotSpeed * dt);

                    // Scale down to 0
                    float scale = Mathf.Lerp(1f, 0f, progress);
                    t.rt.localScale = new Vector3(scale, scale, 1f);

                    // Fade alpha
                    float alpha = Mathf.Clamp01(1f - progress);
                    t.img.color = new Color(t.baseColor.r, t.baseColor.g, t.baseColor.b, alpha);
                }
            }
        }

        private bool ScreenPointToLocal(Vector2 screenPos, out Vector2 localPos)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, screenPos, _uiCamera, out localPos);
        }
    }
}
