using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HorrorEscape
{
    public class FlashlightController : MonoBehaviour
    {
        [Header("Flashlight Settings")]
        [SerializeField] private Light spotLight;
        [SerializeField] private float maxBattery = 100f;
        public float currentBattery = 100f;
        [SerializeField] private float drainRate = 1.8f; // ~55 seconds of full light

        public bool IsOn { get; private set; } = true;

        private float _defaultIntensity;
        private float _flickerTimer = 0f;

        private void Start()
        {
            if (spotLight == null) spotLight = GetComponentInChildren<Light>();
            if (spotLight != null) _defaultIntensity = spotLight.intensity;
            currentBattery = maxBattery;
        }

        private void Update()
        {
            // F key toggle on PC
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null && kb.fKey.wasPressedThisFrame)
            {
                Toggle();
            }
#endif

            if (IsOn && spotLight != null)
            {
                currentBattery = Mathf.Max(0f, currentBattery - drainRate * Time.deltaTime);

                // Low battery flickering
                if (currentBattery < 25f && currentBattery > 0f)
                {
                    _flickerTimer += Time.deltaTime;
                    if (_flickerTimer > Random.Range(0.05f, 0.25f))
                    {
                        _flickerTimer = 0f;
                        spotLight.enabled = Random.value > 0.4f;
                    }
                }
                else if (currentBattery <= 0f)
                {
                    spotLight.enabled = false;
                }
                else
                {
                    spotLight.enabled = true;
                    // Slight dimming with battery level
                    spotLight.intensity = _defaultIntensity * Mathf.Lerp(0.5f, 1f, currentBattery / maxBattery);
                }
            }
            else if (spotLight != null)
            {
                spotLight.enabled = false;
            }
        }

        public void Toggle()
        {
            if (currentBattery <= 0f) return;
            IsOn = !IsOn;
            if (spotLight != null) spotLight.enabled = IsOn;
        }

        public void Recharge(float amount)
        {
            currentBattery = Mathf.Min(maxBattery, currentBattery + amount);
            if (!IsOn && currentBattery > 0f) Toggle();
        }

        public void AssignLight(Light l)
        {
            spotLight = l;
            if (spotLight != null) _defaultIntensity = spotLight.intensity;
        }
    }
}
