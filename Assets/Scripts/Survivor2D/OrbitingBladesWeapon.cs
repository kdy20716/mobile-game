using System.Collections.Generic;
using UnityEngine;

namespace Survivor2D
{
    public class OrbitingBladesWeapon : MonoBehaviour
    {
        [Header("Blade Settings")]
        [SerializeField] private int bladeCount = 2;
        [SerializeField] private float orbitRadius = 2.2f;
        [SerializeField] private float rotationSpeed = 220f;
        [SerializeField] private float baseDamage = 25f;

        private List<Transform> _activeBlades = new List<Transform>();
        private float _currentAngle = 0f;
        private SurvivorPlayer2D _player;

        private void Start()
        {
            _player = GetComponentInParent<SurvivorPlayer2D>();
            RebuildBlades();
        }

        private void Update()
        {
            float speedMult = _player != null ? _player.attackSpeedMultiplier : 1f;
            _currentAngle += rotationSpeed * speedMult * Time.deltaTime;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            float step = 360f / Mathf.Max(1, bladeCount);
            for (int i = 0; i < _activeBlades.Count; i++)
            {
                if (_activeBlades[i] == null) continue;
                float rad = (_currentAngle + i * step) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
                _activeBlades[i].position = transform.position + offset;
                _activeBlades[i].rotation = Quaternion.Euler(0f, 0f, _currentAngle + i * step + 90f);
            }
        }

        public void AddBlade()
        {
            bladeCount++;
            RebuildBlades();
        }

        public void IncreaseDamage(float percent)
        {
            baseDamage *= (1f + percent);
        }

        public void RebuildBlades()
        {
            // Clear existing
            foreach (var b in _activeBlades)
            {
                if (b != null) Destroy(b.gameObject);
            }
            _activeBlades.Clear();

            for (int i = 0; i < bladeCount; i++)
            {
                GameObject bladeObj = new GameObject($"Blade_{i}");
                bladeObj.transform.SetParent(transform);

                // Sprite
                SpriteRenderer sr = bladeObj.AddComponent<SpriteRenderer>();
                sr.sprite = CreateBladeSprite();
                sr.color = new Color(0.2f, 0.9f, 1f); // Cyan glowing blade
                sr.sortingOrder = 5;

                // Collider
                CircleCollider2D col = bladeObj.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.4f;

                // Hit trigger
                var hit = bladeObj.AddComponent<BladeHitTrigger>();
                hit.damage = baseDamage;
                hit.weapon = this;

                _activeBlades.Add(bladeObj.transform);
            }
        }

        public float GetCurrentDamage()
        {
            float mult = _player != null ? _player.damageMultiplier : 1f;
            return baseDamage * mult;
        }

        private Sprite CreateBladeSprite()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Diamond dagger shape
                    float dx = Mathf.Abs(x - size * 0.5f) / (size * 0.5f);
                    float dy = Mathf.Abs(y - size * 0.5f) / (size * 0.5f);
                    if (dx * 1.8f + dy <= 1f)
                    {
                        colors[y * size + x] = Color.white;
                    }
                    else
                    {
                        colors[y * size + x] = Color.clear;
                    }
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
        }
    }

    public class BladeHitTrigger : MonoBehaviour
    {
        public float damage;
        public OrbitingBladesWeapon weapon;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy2D>();
            if (enemy != null)
            {
                float finalDmg = weapon != null ? weapon.GetCurrentDamage() : damage;
                enemy.TakeDamage(finalDmg);
            }
        }
    }
}
