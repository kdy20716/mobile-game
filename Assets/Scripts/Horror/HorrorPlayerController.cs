using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HorrorEscape
{
    [RequireComponent(typeof(CharacterController))]
    public class HorrorPlayerController : MonoBehaviour
    {
        public static HorrorPlayerController Instance { get; private set; }

        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.8f;
        [SerializeField] private float runSpeed = 6.5f;
        [SerializeField] private float gravity = 20f;

        [Header("Stamina")]
        public float maxStamina = 100f;
        public float currentStamina = 100f;
        [SerializeField] private float staminaDrain = 25f;
        [SerializeField] private float staminaRegen = 15f;

        [Header("Look Sensitivity")]
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float touchSensitivity = 0.18f;
        [SerializeField] private Transform playerCamera;

        [Header("Head Bobbing")]
        [SerializeField] private float bobbingSpeed = 10f;
        [SerializeField] private float bobbingAmount = 0.05f;

        private CharacterController _cc;
        private float _verticalVelocity = 0f;
        private float _cameraPitch = 0f;
        private Vector3 _defaultCamPos;
        private float _bobTimer = 0f;
        private bool _isSprinting = false;

        public bool IsRunning => _isSprinting;
        public bool IsMoving => _cc != null && _cc.velocity.sqrMagnitude > 0.1f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _cc = GetComponent<CharacterController>();
            if (playerCamera != null) _defaultCamPos = playerCamera.localPosition;
            currentStamina = maxStamina;
        }

        private void Start()
        {
            // Lock cursor on PC only when playing
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }

        private void Update()
        {
            HandleRotation();
            HandleMovement();
            HandleHeadBob();
        }

        private void HandleRotation()
        {
            Vector2 lookInput = Vector2.zero;

            // 1. Mouse Look (PC)
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse != null)
            {
                lookInput += mouse.delta.ReadValue() * mouseSensitivity;
            }
#endif

            // 2. Mobile Touch Look
            if (TouchLookPanel.Instance != null)
            {
                lookInput += TouchLookPanel.Instance.LookDelta * touchSensitivity;
            }

            // Horizontal Look (Player Body)
            transform.Rotate(Vector3.up * lookInput.x);

            // Vertical Look (Camera Pitch)
            if (playerCamera != null)
            {
                _cameraPitch -= lookInput.y;
                _cameraPitch = Mathf.Clamp(_cameraPitch, -80f, 80f);
                playerCamera.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            Vector2 moveDir = Vector2.zero;

            // Keyboard WASD
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed) moveDir.y += 1f;
                if (kb.sKey.isPressed) moveDir.y -= 1f;
                if (kb.aKey.isPressed) moveDir.x -= 1f;
                if (kb.dKey.isPressed) moveDir.x += 1f;

                _isSprinting = kb.leftShiftKey.isPressed && currentStamina > 5f && moveDir.sqrMagnitude > 0.01f;
            }
#endif

            // Mobile Virtual Joystick (if present in scene)
            var joystick = Survivor2D.VirtualJoystick.Instance;
            if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
            {
                moveDir = joystick.Direction;
            }

            moveDir.Normalize();

            // Stamina Handling
            if (_isSprinting)
            {
                currentStamina = Mathf.Max(0f, currentStamina - staminaDrain * Time.deltaTime);
                if (currentStamina <= 0f) _isSprinting = false;
            }
            else
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegen * Time.deltaTime);
            }

            float currentSpeed = _isSprinting ? runSpeed : walkSpeed;
            Vector3 worldMove = transform.right * moveDir.x + transform.forward * moveDir.y;

            // Gravity
            if (_cc.isGrounded)
            {
                _verticalVelocity = -1f;
            }
            else
            {
                _verticalVelocity -= gravity * Time.deltaTime;
            }

            worldMove *= currentSpeed;
            worldMove.y = _verticalVelocity;

            _cc.Move(worldMove * Time.deltaTime);
        }

        private void HandleHeadBob()
        {
            if (playerCamera == null) return;

            if (_cc.isGrounded && _cc.velocity.sqrMagnitude > 0.2f)
            {
                float speedMultiplier = _isSprinting ? 1.4f : 1f;
                _bobTimer += Time.deltaTime * bobbingSpeed * speedMultiplier;
                float newY = _defaultCamPos.y + Mathf.Sin(_bobTimer) * bobbingAmount;
                playerCamera.localPosition = new Vector3(_defaultCamPos.x, newY, _defaultCamPos.z);
            }
            else
            {
                _bobTimer = 0f;
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, _defaultCamPos, Time.deltaTime * 6f);
            }
        }

        public void SetCamera(Transform cam)
        {
            playerCamera = cam;
            if (playerCamera != null) _defaultCamPos = playerCamera.localPosition;
        }

        public void SetSprinting(bool sprinting)
        {
            _isSprinting = sprinting && currentStamina > 5f;
        }
    }
}
