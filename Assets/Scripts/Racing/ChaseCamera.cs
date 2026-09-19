using UnityEngine;

namespace MobileRacing
{
    public class ChaseCamera : MonoBehaviour
    {
        [Header("Target & Offsets")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -5.5f);

        [Header("Follow Dynamics")]
        [SerializeField] private float positionSmoothTime = 0.12f;
        [SerializeField] private float rotationSmoothSpeed = 8f;

        [Header("Dynamic FOV with Speed")]
        [SerializeField] private Camera cam;
        [SerializeField] private float baseFov = 60f;
        [SerializeField] private float maxFov = 78f;
        [SerializeField] private float maxFovSpeedKmh = 140f;

        private Rigidbody _targetRb;
        private Vector3 _currentVelocity;

        private void Start()
        {
            if (cam == null) cam = GetComponent<Camera>();
            if (target != null) _targetRb = target.GetComponent<Rigidbody>();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null) _targetRb = target.GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. Calculate Target Position behind the car
            Vector3 desiredPosition = target.TransformPoint(offset);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, positionSmoothTime);

            // 2. Smoothly rotate to look at target slightly above center
            Vector3 lookTarget = target.position + Vector3.up * 1.2f;
            Vector3 direction = (lookTarget - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
            }

            // 3. Dynamic FOV depending on speed
            if (cam != null && _targetRb != null)
            {
                float speedKmh = _targetRb.linearVelocity.magnitude * 3.6f;
                float fovRatio = Mathf.Clamp01(speedKmh / maxFovSpeedKmh);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(baseFov, maxFov, fovRatio), Time.deltaTime * 5f);
            }
        }
    }
}
