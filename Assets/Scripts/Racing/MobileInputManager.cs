using UnityEngine;

namespace MobileRacing
{
    public class MobileInputManager : MonoBehaviour
    {
        public static MobileInputManager Instance { get; private set; }

        public float ThrottleInput { get; private set; } // -1 (Reverse) ~ 1 (Forward)
        public float SteerInput { get; private set; }    // -1 (Left) ~ 1 (Right)
        public bool HandbrakeInput { get; private set; }
        public bool BoostInput { get; private set; }

        // Touch button flags
        private bool _isSteerLeft;
        private bool _isSteerRight;
        private bool _isAccelerating;
        private bool _isBraking;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            // 1. Keyboard Input (PC / Editor Testing)
            float keySteer = Input.GetAxisRaw("Horizontal");
            float keyThrottle = Input.GetAxisRaw("Vertical");
            bool keyHandbrake = Input.GetKey(KeyCode.Space);
            bool keyBoost = Input.GetKey(KeyCode.LeftShift);

            // 2. Mobile Touch Buttons
            float touchSteer = 0f;
            if (_isSteerLeft) touchSteer -= 1f;
            if (_isSteerRight) touchSteer += 1f;

            float touchThrottle = 0f;
            if (_isAccelerating) touchThrottle += 1f;
            if (_isBraking) touchThrottle -= 1f;

            // Combine Inputs
            SteerInput = Mathf.Clamp(keySteer + touchSteer, -1f, 1f);
            ThrottleInput = Mathf.Clamp(keyThrottle + touchThrottle, -1f, 1f);
            HandbrakeInput = keyHandbrake;
            BoostInput = keyBoost;
        }

        // Mobile UI Callback Methods
        public void SetSteerLeft(bool isPressed) => _isSteerLeft = isPressed;
        public void SetSteerRight(bool isPressed) => _isSteerRight = isPressed;
        public void SetAccelerate(bool isPressed) => _isAccelerating = isPressed;
        public void SetBrake(bool isPressed) => _isBraking = isPressed;
        public void SetBoost(bool isPressed) => BoostInput = isPressed;
        public void SetHandbrake(bool isPressed) => HandbrakeInput = isPressed;
    }
}
