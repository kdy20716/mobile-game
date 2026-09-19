using System;
using UnityEngine;

namespace Survivor2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class Enemy2D : MonoBehaviour
    {
        public static event Action OnEnemyKilled;

        [Header("Stats")]
        [SerializeField] private float maxHp = 30f;
        [SerializeField] private float moveSpeed = 2.8f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private int expReward = 1;

        private float _currentHp;
        private Rigidbody2D _rb;
        private Transform _playerTransform;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _sr = GetComponentInChildren<SpriteRenderer>();
            _currentHp = maxHp;
        }

        private void Start()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                _playerTransform = SurvivorPlayer2D.Instance.transform;
            }
        }

        private void FixedUpdate()
        {
            if (_playerTransform == null) return;

            Vector2 dir = (_playerTransform.position - transform.position).normalized;
            _rb.linearVelocity = dir * moveSpeed;

            if (_sr != null && dir.x != 0)
            {
                _sr.flipX = dir.x < 0;
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHp -= damage;
            DamageNumberManager.SpawnDamage(transform.position + Vector3.up * 0.3f, Mathf.RoundToInt(damage));

            if (_currentHp <= 0f)
            {
                Die();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<SurvivorPlayer2D>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }

        private void Die()
        {
            OnEnemyKilled?.Invoke();
            DropExpGem();
            Destroy(gameObject);
        }

        private void DropExpGem()
        {
            GameObject gem = new GameObject("ExpGem");
            gem.transform.position = transform.position;

            SpriteRenderer sr = gem.AddComponent<SpriteRenderer>();
            sr.sprite = CreateGemSprite();
            sr.color = new Color(0.2f, 0.7f, 1f); // Blue gem
            sr.sortingOrder = 3;

            var exp = gem.AddComponent<ExpGem2D>();
            exp.expValue = expReward;
        }

        private Sprite CreateGemSprite()
        {
            int size = 16;
            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - size * 0.5f) / (size * 0.5f);
                    float dy = Mathf.Abs(y - size * 0.5f) / (size * 0.5f);
                    colors[y * size + x] = (dx + dy <= 1f) ? Color.white : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
        }

        public void SetStats(float hp, float speed, float dmg, int exp)
        {
            maxHp = hp;
            _currentHp = hp;
            moveSpeed = speed;
            attackDamage = dmg;
            expReward = exp;
        }
    }
}
