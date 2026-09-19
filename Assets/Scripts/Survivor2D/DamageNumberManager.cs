using UnityEngine;
using UnityEngine.UI;

namespace Survivor2D
{
    public class DamageNumberManager : MonoBehaviour
    {
        private static DamageNumberManager _instance;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);
        }

        public static void SpawnDamage(Vector3 worldPos, int damage)
        {
            if (_instance == null) return;
            _instance.CreateFloatingText(worldPos, damage);
        }

        private void CreateFloatingText(Vector3 pos, int damage)
        {
            GameObject textObj = new GameObject("DmgText");
            textObj.transform.position = pos;

            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.text = damage.ToString();
            tm.fontSize = 24;
            tm.characterSize = 0.12f;
            tm.color = Color.yellow;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;

            var floater = textObj.AddComponent<FloatingDamageText>();
            floater.Init();
        }
    }

    public class FloatingDamageText : MonoBehaviour
    {
        private float _lifetime = 0.6f;
        private Vector3 _velocity;
        private TextMesh _tm;

        public void Init()
        {
            _velocity = new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(2f, 3.5f), 0f);
            _tm = GetComponent<TextMesh>();
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            _velocity.y -= 4f * Time.deltaTime; // Gravity fall

            if (_tm != null)
            {
                Color c = _tm.color;
                c.a = Mathf.Clamp01(c.a - Time.deltaTime * 1.5f);
                _tm.color = c;
            }
        }
    }
}
