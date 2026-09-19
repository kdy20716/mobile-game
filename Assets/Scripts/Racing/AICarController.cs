using System.Collections.Generic;
using UnityEngine;

namespace MobileRacing
{
    [RequireComponent(typeof(Rigidbody))]
    public class AICarController : MonoBehaviour
    {
        [Header("AI Performance")]
        [SerializeField] private float accelerationPower = 30f;
        [SerializeField] private float maxSpeed = 36f; // Slightly tuned per car
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float brakePower = 40f;
        [SerializeField] private float reachThreshold = 12f;
        [SerializeField] private float downforce = 50f;
        [SerializeField] private float driftFriction = 0.90f;

        [Header("Waypoints")]
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        public int CurrentWaypointIndex { get; private set; } = 0;
        public int LapsCompleted { get; private set; } = 0;

        [Header("Wheel Visuals")]
        [SerializeField] private Transform frontLeftWheel;
        [SerializeField] private Transform frontRightWheel;
        [SerializeField] private Transform rearLeftWheel;
        [SerializeField] private Transform rearRightWheel;

        private Rigidbody _rb;
        private bool _isGrounded;
        private bool _canRace = false;

        public float CurrentSpeedKmh => _rb != null ? _rb.linearVelocity.magnitude * 3.6f : 0f;
        public Rigidbody Rb => _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
        }

        public void SetCanRace(bool canRace) => _canRace = canRace;

        public void SetWaypoints(List<Transform> list, float speed = 36f, float accel = 30f)
        {
            waypoints = list;
            maxSpeed = speed;
            accelerationPower = accel;
        }

        private void FixedUpdate()
        {
            CheckGround();

            if (!_canRace || waypoints == null || waypoints.Count == 0) return;

            Transform targetWp = waypoints[CurrentWaypointIndex];
            Vector3 toTarget = targetWp.position - transform.position;
            toTarget.y = 0f;

            // Check if reached waypoint
            if (toTarget.magnitude < reachThreshold)
            {
                CurrentWaypointIndex++;
                if (CurrentWaypointIndex >= waypoints.Count)
                {
                    CurrentWaypointIndex = 0;
                    LapsCompleted++;
                }
                targetWp = waypoints[CurrentWaypointIndex];
                toTarget = targetWp.position - transform.position;
                toTarget.y = 0f;
            }

            Vector3 localTarget = transform.InverseTransformPoint(targetWp.position);
            float steer = Mathf.Clamp(localTarget.x / 10f, -1f, 1f);

            // Cornering speed regulation
            float targetSpeed = maxSpeed;
            if (Mathf.Abs(steer) > 0.4f)
            {
                targetSpeed *= 0.65f; // Slow down in sharp turns
            }

            // Obstacle / Car avoidance with front raycasts
            steer = ApplyObstacleAvoidance(steer);

            if (_isGrounded)
            {
                // Acceleration & Braking
                float currentSpeed = _rb.linearVelocity.magnitude;
                if (currentSpeed < targetSpeed)
                {
                    _rb.AddForce(transform.forward * accelerationPower, ForceMode.Acceleration);
                }
                else if (currentSpeed > targetSpeed + 2f)
                {
                    _rb.AddForce(-transform.forward * brakePower, ForceMode.Acceleration);
                }

                // Steering
                float speedRatio = Mathf.Clamp01(currentSpeed / 10f);
                float turn = steer * turnSpeed * speedRatio * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                _rb.MoveRotation(_rb.rotation * turnRotation);

                // Side friction
                Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
                localVel.x *= (1f - (1f - driftFriction) * 0.5f);
                _rb.linearVelocity = transform.TransformDirection(localVel);

                // Downforce
                _rb.AddForce(-transform.up * (downforce * (_rb.linearVelocity.magnitude / 20f)), ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(Vector3.down * 30f, ForceMode.Acceleration);
            }
        }

        private float ApplyObstacleAvoidance(float currentSteer)
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.4f;

            // Check straight ahead
            if (Physics.Raycast(origin, transform.forward, out hit, 12f))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    // Steer away
                    Vector3 normal = hit.normal;
                    float cross = Vector3.Cross(transform.forward, normal).y;
                    return cross > 0 ? 0.9f : -0.9f;
                }
            }

            return currentSteer;
        }

        private void CheckGround()
        {
            RaycastHit hit;
            _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, -transform.up, out hit, 1.2f);
        }

        private void Update()
        {
            // Wheel visual rotation
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
