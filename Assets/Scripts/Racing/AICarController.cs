using System.Collections.Generic;
using UnityEngine;

namespace MobileRacing
{
    [RequireComponent(typeof(Rigidbody))]
    public class AICarController : MonoBehaviour
    {
        [Header("AI Performance")]
        [SerializeField] private float accelerationPower = 32f;
        [SerializeField] private float maxSpeed = 38f;
        [SerializeField] private float targetLaneX = 0f;
        [SerializeField] private float laneChangeSpeed = 4f;

        [Header("Wheel Visuals")]
        [SerializeField] private Transform frontLeftWheel;
        [SerializeField] private Transform frontRightWheel;
        [SerializeField] private Transform rearLeftWheel;
        [SerializeField] private Transform rearRightWheel;

        private Rigidbody _rb;
        private bool _canRace = false;
        private float _laneChangeTimer = 0f;
        private float _laneInterval = 3.5f;

        public float CurrentSpeedKmh => _rb != null ? _rb.linearVelocity.magnitude * 3.6f : 0f;
        public float DistanceTraveled => transform.position.z;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
        }

        public void SetCanRace(bool canRace) => _canRace = canRace;

        public void SetPerformance(float speed, float accel, float initialLaneX)
        {
            maxSpeed = speed;
            accelerationPower = accel;
            targetLaneX = initialLaneX;
            _laneInterval = Random.Range(3f, 6f);
        }

        private void FixedUpdate()
        {
            if (!_canRace) return;

            // 1. Forward Acceleration
            if (_rb.linearVelocity.magnitude < maxSpeed)
            {
                _rb.AddForce(Vector3.forward * accelerationPower, ForceMode.Acceleration);
            }

            // 2. Lane Keeping & Smooth Lane Changing (5-lane highway: -14, -7, 0, 7, 14)
            _laneChangeTimer += Time.fixedDeltaTime;
            if (_laneChangeTimer >= _laneInterval)
            {
                _laneChangeTimer = 0f;
                _laneInterval = Random.Range(4f, 8f);
                // Pick a lane: -14, -7, 0, 7, 14
                int[] lanes = new int[] { -14, -7, 0, 7, 14 };
                targetLaneX = lanes[Random.Range(0, lanes.Length)];
            }

            // Move towards target lane
            float currentX = transform.position.x;
            float newX = Mathf.MoveTowards(currentX, targetLaneX, laneChangeSpeed * Time.fixedDeltaTime);
            Vector3 pos = transform.position;
            pos.x = newX;
            transform.position = pos;

            // Keep facing forward
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // Downforce
            _rb.AddForce(Vector3.down * 40f, ForceMode.Acceleration);
        }

        private void Update()
        {
            float rotationAmount = CurrentSpeedKmh * Time.deltaTime * 20f;
            if (frontLeftWheel != null) frontLeftWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
            if (frontRightWheel != null) frontRightWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
            if (rearLeftWheel != null) rearLeftWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
            if (rearRightWheel != null) rearRightWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }

        public void AssignWheels(Transform fl, Transform fr, Transform rl, Transform rr)
        {
            frontLeftWheel = fl;
            frontRightWheel = fr;
            rearLeftWheel = rl;
            rearRightWheel = rr;
        }
    }
}
