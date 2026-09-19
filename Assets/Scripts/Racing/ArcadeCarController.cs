using UnityEngine;

namespace MobileRacing
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArcadeCarController : MonoBehaviour
    {
        [Header("Car Physics Settings")]
        [SerializeField] private float accelerationPower = 35f;
        [SerializeField] private float reversePower = 18f;
        [SerializeField] private float maxSpeed = 40f; // m/s (~144 km/h)
        [SerializeField] private float maxBoostSpeed = 55f;
        [SerializeField] private float turnSpeed = 90f; // degrees/sec
        [SerializeField] private float brakePower = 45f;
        [SerializeField] private float downforce = 50f;
        [SerializeField] private float driftFriction = 0.88f;

        [Header("Ground Check")]
        [SerializeField] private float groundRayDistance = 1.2f;
        [SerializeField] private LayerMask groundLayer = ~0;

        [Header("Wheel Visuals")]
        [SerializeField] private Transform frontLeftWheel;
        [SerializeField] private Transform frontRightWheel;
        [SerializeField] private Transform rearLeftWheel;
        [SerializeField] private Transform rearRightWheel;
        [SerializeField] private float maxSteerAngle = 30f;

        private Rigidbody _rb;
        private bool _isGrounded;
        private float _currentSteerAngle;

        public float CurrentSpeedKmh => _rb != null ? _rb.linearVelocity.magnitude * 3.6f : 0f;
        public bool IsGrounded => _isGrounded;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0f, -0.4f, 0f); // Low center of mass for stability
        }

        private void Update()
        {
            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            CheckGround();

            float throttle = MobileInputManager.Instance != null ? MobileInputManager.Instance.ThrottleInput : Input.GetAxis("Vertical");
            float steer = MobileInputManager.Instance != null ? MobileInputManager.Instance.SteerInput : Input.GetAxis("Horizontal");
            bool boost = MobileInputManager.Instance != null && MobileInputManager.Instance.BoostInput;
            bool handbrake = MobileInputManager.Instance != null && MobileInputManager.Instance.HandbrakeInput;

            if (_isGrounded)
            {
                ApplyAcceleration(throttle, boost);
                ApplySteering(steer);
                ApplySideFriction(handbrake);
                ApplyDownforce();
            }
            else
            {
                // Extra gravity when airborne to prevent floating
                _rb.AddForce(Vector3.down * 30f, ForceMode.Acceleration);
            }
        }

        private void CheckGround()
        {
            RaycastHit hit;
            _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, -transform.up, out hit, groundRayDistance, groundLayer);
        }

        private void ApplyAcceleration(float throttle, bool boost)
        {
            float targetMaxSpeed = boost ? maxBoostSpeed : maxSpeed;
            float currentSpeed = _rb.linearVelocity.magnitude;

            if (throttle > 0f && currentSpeed < targetMaxSpeed)
            {
                float power = accelerationPower * (boost ? 1.6f : 1f);
                _rb.AddForce(transform.forward * (throttle * power), ForceMode.Acceleration);
            }
            else if (throttle < 0f && currentSpeed < targetMaxSpeed * 0.5f)
            {
                // If moving forward, brake; otherwise reverse
                float forwardDot = Vector3.Dot(_rb.linearVelocity, transform.forward);
                if (forwardDot > 1f)
                {
                    _rb.AddForce(-transform.forward * brakePower, ForceMode.Acceleration);
                }
                else
                {
                    _rb.AddForce(transform.forward * (throttle * reversePower), ForceMode.Acceleration);
                }
            }
        }

        private void ApplySteering(float steer)
        {
            float speedRatio = Mathf.Clamp01(_rb.linearVelocity.magnitude / 10f); // Require movement to steer
            float forwardDot = Vector3.Dot(_rb.linearVelocity, transform.forward);
            float direction = forwardDot >= -0.1f ? 1f : -1f; // Invert steering when reversing

            float turn = steer * turnSpeed * speedRatio * direction * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }

        private void ApplySideFriction(bool handbrake)
        {
            Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
            // Cancel lateral velocity (side slipping) based on drift grip
            float lateralGrip = handbrake ? 0.95f : driftFriction;
            localVel.x *= (1f - (1f - lateralGrip) * 0.5f);
            _rb.linearVelocity = transform.TransformDirection(localVel);
        }

        private void ApplyDownforce()
        {
            _rb.AddForce(-transform.up * (downforce * (_rb.linearVelocity.magnitude / 20f)), ForceMode.Acceleration);
        }

        private void UpdateWheelVisuals()
        {
            float steer = MobileInputManager.Instance != null ? MobileInputManager.Instance.SteerInput : Input.GetAxis("Horizontal");
            _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, steer * maxSteerAngle, Time.deltaTime * 10f);

            if (frontLeftWheel != null)
                frontLeftWheel.localRotation = Quaternion.Euler(frontLeftWheel.localEulerAngles.x, _currentSteerAngle, 0f);
            if (frontRightWheel != null)
                frontRightWheel.localRotation = Quaternion.Euler(frontRightWheel.localEulerAngles.x, _currentSteerAngle, 0f);

            // Rotate wheels forward with speed
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
