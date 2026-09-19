using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Survivor2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SurvivorPlayer2D : MonoBehaviour
    {
        public static SurvivorPlayer2D Instance { get; private set; }

        [Header("Stats")]
        public float maxHp = 100f;
        public float currentHp = 100f;
        public float moveSpeed = 5.5f;
        public float magnetRadius = 3.5f;
        public float damageMultiplier = 1.0f;
        public float attackSpeedMultiplier = 1.0f;

        [Header("Progression")]
        public int currentLevel = 1;
        public int currentExp = 0;
        public int expToNextLevel = 10;

        [Header("Visuals & Feedback")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private float _invincibilityTimer = 0f;
        private bool _isDead = false;

        public event Action<float, float> OnHpChanged; // cur, max
        public event Action<int, int, int> OnExpChanged; // cur, max, level
        public event Action<int> OnLevelUp;
        public event Action OnPlayerDied;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.freezeRotation = true;

            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            currentHp = maxHp;
        }

        private void Start()
        {
            OnHpChanged?.Invoke(currentHp, maxHp);
            OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
        }

        private void Update()
        {
            if (_isDead) return;

            // 1. Process Movement Inputs (Joystick or Keyboard)
            Vector2 input = Vector2.zero;

            // Virtual Joystick
            if (VirtualJoystick.Instance != null && VirtualJoystick.Instance.Direction.sqrMagnitude > 0.01f)
            {
                input = VirtualJoystick.Instance.Direction;
            }
            else
            {
                // Keyboard Input System
#if ENABLE_INPUT_SYSTEM
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
                    if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
                    if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
                    if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
                }
#endif
            }

            _moveInput = Vector2.ClampMagnitude(input, 1f);

            // Flip sprite based on direction
            if (_moveInput.x != 0 && spriteRenderer != null)
            {
                spriteRenderer.flipX = _moveInput.x < 0;
            }

            // Invincibility frame timer
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (spriteRenderer != null)
                {
                    // Flash effect
                    float alpha = Mathf.PingPong(Time.time * 15f, 1f) > 0.5f ? 0.3f : 1f;
                    Color c = spriteRenderer.color;
                    c.a = alpha;
                    spriteRenderer.color = c;
                }
            }
            else if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }
        }

        private void FixedUpdate()
        {
            if (_isDead) return;
            _rb.linearVelocity = _moveInput * moveSpeed;
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || _invincibilityTimer > 0f) return;

            currentHp -= damage;
            _invincibilityTimer = 0.5f; // 0.5s i-frames
            OnHpChanged?.Invoke(currentHp, maxHp);

            if (currentHp <= 0f)
            {
                Die();
            }
        }

        public void AddExp(int amount)
        {
            if (_isDead) return;

            currentExp += amount;
            while (currentExp >= expToNextLevel)
            {
                currentExp -= expToNextLevel;
                currentLevel++;
                expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.35f);
                OnLevelUp?.Invoke(currentLevel);
            }
            OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
        }

        public void Heal(float amount)
        {
            currentHp = Mathf.Min(maxHp, currentHp + amount);
            OnHpChanged?.Invoke(currentHp, maxHp);
        }

        private void Die()
        {
            _isDead = true;
            _rb.linearVelocity = Vector2.zero;
            OnPlayerDied?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
