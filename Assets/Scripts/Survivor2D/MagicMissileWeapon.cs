using UnityEngine;

namespace Survivor2D
{
    public class MagicMissileWeapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] private float fireInterval = 1.2f;
        [SerializeField] private float missileSpeed = 9f;
        [SerializeField] private float baseDamage = 35f;
        [SerializeField] private float detectionRange = 14f;

        private float _timer = 0f;
        private SurvivorPlayer2D _player;

        private void Start()
        {
            _player = GetComponentInParent<SurvivorPlayer2D>();
        }

        private void Update()
        {
            float speedMult = _player != null ? _player.attackSpeedMultiplier : 1f;
            _timer += Time.deltaTime * speedMult;

            if (_timer >= fireInterval)
            {
                _timer = 0f;
                ShootMissileAtNearestEnemy();
            }
        }

        private void ShootMissileAtNearestEnemy()
        {
            // Find nearest enemy
            Enemy2D nearest = null;
            float minDist = float.MaxValue;

            var allEnemies = Object.FindObjectsByType<Enemy2D>(FindObjectsSortMode.None);
            foreach (var e in allEnemies)
            {
                if (e == null || !e.gameObject.activeInHierarchy) continue;
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < minDist && d <= detectionRange)
                {
                    minDist = d;
                    nearest = e;
                }
            }

            if (nearest != null)
            {
                Vector2 dir = (nearest.transform.position - transform.position).normalized;
                SpawnMissile(dir);
            }
        }

        private void SpawnMissile(Vector2 direction)
        {
            GameObject missile = new GameObject("MagicMissile");
            missile.transform.position = transform.position;

            SpriteRenderer sr = missile.AddComponent<SpriteRenderer>();
            sr.sprite = CreateMissileSprite();
            sr.color = new Color(1f, 0.4f, 0.9f); // Pink magical energy
            sr.sortingOrder = 6;

            CircleCollider2D col = missile.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;

            var proj = missile.AddComponent<MagicMissileProjectile>();
            float dmg = baseDamage * (_player != null ? _player.damageMultiplier : 1f);
            proj.Setup(direction, missileSpeed, dmg);
        }

        private Sprite CreateMissileSprite()
        {
            int size = 24;
            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f)) / (size * 0.5f);
                    colors[y * size + x] = dist <= 1f ? Color.white : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 24);
        }
    }

    public class MagicMissileProjectile : MonoBehaviour
    {
        private Vector2 _dir;
        private float _speed;
        private float _damage;
        private float _lifetime = 3.5f;

        public void Setup(Vector2 direction, float speed, float damage)
        {
            _dir = direction;
            _speed = speed;
            _damage = damage;
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy2D>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
