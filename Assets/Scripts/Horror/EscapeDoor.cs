using System;
using UnityEngine;

namespace HorrorEscape
{
    public class EscapeDoor : MonoBehaviour
    {
        public static event Action OnEscaped;

        [SerializeField] private Light doorLight;
        [SerializeField] private Transform doorMesh;

        private bool _isUnlocked = false;

        private void Start()
        {
            if (doorLight != null) doorLight.color = Color.red;
            EscapeKeyItem.OnKeyCollected += CheckKeys;
        }

        private void OnDestroy()
        {
            EscapeKeyItem.OnKeyCollected -= CheckKeys;
        }

        private void CheckKeys(int count)
        {
            if (count >= EscapeKeyItem.TotalKeysNeeded && !_isUnlocked)
            {
                _isUnlocked = true;
                if (doorLight != null) doorLight.color = Color.green; // Ready to open
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<HorrorPlayerController>() != null)
            {
                if (_isUnlocked)
                {
                    OnEscaped?.Invoke();
                }
            }
        }

        public void SetupDoor(Light l, Transform mesh)
        {
            doorLight = l;
            doorMesh = mesh;
        }
    }
}
