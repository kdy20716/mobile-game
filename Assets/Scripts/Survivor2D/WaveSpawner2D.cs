using UnityEngine;

namespace Survivor2D
{
    public class WaveSpawner2D : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float baseSpawnInterval = 1.2f;
        [SerializeField] private float spawnRadius = 11f;
        public Sprite enemySprite;

        private float _timer = 0f;
        private float _elapsedGameTime = 0f;
        private Transform _playerTransform;

        private void Start()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                _playerTransform = SurvivorPlayer2D.Instance.transform;
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            _elapsedGameTime += Time.deltaTime;

            // Interval decreases as time goes on (more enemies spawn)
            float currentInterval = Mathf.Max(0.25f, baseSpawnInterval - (_elapsedGameTime / 120f) * 0.8f);
            _timer += Time.deltaTime;

            if (_timer >= currentInterval)
            {
                _timer = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            // Pick a random angle around player
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 spawnPos = _playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;

            GameObject enemy = new GameObject("Enemy_Bat");
            enemy.transform.position = spawnPos;

            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            sr.sprite = enemySprite != null ? enemySprite : CreateEnemySprite();
            sr.color = Color.white;
            sr.sortingOrder = 2;

            CircleCollider2D col = enemy.AddComponent<CircleCollider2D>();
            col.radius = 0.45f;

            var enemyComp = enemy.AddComponent<Enemy2D>();

            // Scale stats with elapsed time
            float difficultyMult = 1f + (_elapsedGameTime / 60f) * 0.5f;
            enemyComp.SetStats(25f * difficultyMult, 2.6f + Random.Range(-0.3f, 0.5f), 10f * difficultyMult, 1);
        }

        private Sprite CreateEnemySprite()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Bat silhouette
                    float dx = Mathf.Abs(x - size * 0.5f) / (size * 0.5f);
                    float dy = Mathf.Abs(y - size * 0.5f) / (size * 0.5f);
                    if (dx * dx + dy * dy <= 0.8f)
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
}
