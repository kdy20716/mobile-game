using System;
using UnityEngine;

namespace HorrorEscape
{
    public class EscapeKeyItem : MonoBehaviour
    {
        public static event Action<int> OnKeyCollected;
        public static int CollectedKeysCount { get; private set; } = 0;
        public const int TotalKeysNeeded = 3;

        public static void ResetKeys() => CollectedKeysCount = 0;

        [SerializeField] private float rotationSpeed = 60f;
        [SerializeField] private Light keyGlowLight;

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            // Bobbing
            transform.position += Vector3.up * Mathf.Sin(Time.time * 3f) * 0.002f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<HorrorPlayerController>() != null)
            {
                CollectedKeysCount++;
                OnKeyCollected?.Invoke(CollectedKeysCount);
                Destroy(gameObject);
            }
        }
    }
}
