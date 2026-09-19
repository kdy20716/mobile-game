using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
            float keySteer = 0f;
            float keyThrottle = 0f;
            bool keyHandbrake = false;
            bool keyBoost = false;

            // 1. New Input System Support
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) keySteer -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) keySteer += 1f;
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) keyThrottle += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) keyThrottle -= 1f;
                keyHandbrake = keyboard.spaceKey.isPressed;
                keyBoost = keyboard.leftShiftKey.isPressed;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            keySteer = Input.GetAxisRaw("Horizontal");
            keyThrottle = Input.GetAxisRaw("Vertical");
            keyHandbrake = Input.GetKey(KeyCode.Space);
            keyBoost = Input.GetKey(KeyCode.LeftShift);
#endif

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
