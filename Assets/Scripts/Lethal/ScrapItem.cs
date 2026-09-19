using UnityEngine;

namespace LethalCompany
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class ScrapItem : MonoBehaviour
    {
        [Header("Scrap Properties")]
        public string itemName = "Scrap Piece";
        public int scrapValue = 35; // in Dollars ($)
        public float weightLb = 12f; // in Pounds (lb)
        public bool isTwoHanded = false;

        [Header("Visual Orientation in Hand")]
        public Vector3 handHoldOffset = new Vector3(0.35f, -0.3f, 0.6f);
        public Vector3 handHoldRotation = Vector3.zero;
        public Vector3 handHoldScale = Vector3.one;

        private Rigidbody _rb;
        private Collider _collider;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void SetPickedUp(Transform handHolder)
        {
            _rb.isKinematic = true;
            _collider.enabled = false;
            transform.SetParent(handHolder);
            transform.localPosition = handHoldOffset;
            transform.localRotation = Quaternion.Euler(handHoldRotation);
            transform.localScale = handHoldScale;
            gameObject.SetActive(true);
        }

        public void SetDropped(Vector3 dropPos, Vector3 throwVelocity)
        {
            transform.SetParent(null);
            transform.position = dropPos;
            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.linearVelocity = throwVelocity;
            _rb.angularVelocity = Random.insideUnitSphere * 5f;
            gameObject.SetActive(true);
        }

        public void SetStoredInInventory()
        {
            gameObject.SetActive(false);
        }
    }
}
