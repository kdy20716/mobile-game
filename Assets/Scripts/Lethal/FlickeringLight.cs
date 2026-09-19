using UnityEngine;

namespace LethalCompany
{
    public class FlickeringLight : MonoBehaviour
    {
        [SerializeField] private Light targetLight;
        [SerializeField] private float minIntensity = 0.2f;
        [SerializeField] private float maxIntensity = 1.6f;
        [SerializeField] private float flickerSpeed = 0.08f;
        [SerializeField] private float blackoutChance = 0.04f;

        private float _baseIntensity;
        private float _timer;

        private void Start()
        {
            if (targetLight == null) targetLight = GetComponent<Light>();
            if (targetLight != null) _baseIntensity = targetLight.intensity;
        }

        private void Update()
        {
            if (targetLight == null) return;

            _timer += Time.deltaTime;
            if (_timer >= flickerSpeed)
            {
                _timer = 0f;

                if (Random.value < blackoutChance)
                {
                    targetLight.intensity = 0.02f; // Sudden dark blink
                }
                else
                {
                    targetLight.intensity = Random.Range(minIntensity, maxIntensity);
                }
            }
        }
    }
}
