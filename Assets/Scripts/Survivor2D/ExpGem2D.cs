using UnityEngine;

namespace Survivor2D
{
    public class ExpGem2D : MonoBehaviour
    {
        public int expValue = 1;

        private Transform _playerTransform;
        private bool _isAttracted = false;
        private float _flySpeed = 12f;

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

            float dist = Vector2.Distance(transform.position, _playerTransform.position);
            float magnet = SurvivorPlayer2D.Instance != null ? SurvivorPlayer2D.Instance.magnetRadius : 3.5f;

            if (dist <= magnet)
            {
                _isAttracted = true;
            }

            if (_isAttracted)
            {
                transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, _flySpeed * Time.deltaTime);
                _flySpeed += Time.deltaTime * 20f; // Accelerate towards player

                if (dist < 0.4f)
                {
                    // Collected!
                    if (SurvivorPlayer2D.Instance != null)
                    {
                        SurvivorPlayer2D.Instance.AddExp(expValue);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
