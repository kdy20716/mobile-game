using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LethalCompany
{
    [RequireComponent(typeof(CharacterController))]
    public class LethalPlayerController : MonoBehaviour
    {
        public static LethalPlayerController Instance { get; private set; }

        [Header("Movement Settings")]
        [SerializeField] private float baseWalkSpeed = 4.2f;
        [SerializeField] private float baseRunSpeed = 7.0f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = 20f;

        [Header("Look Sensitivity")]
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float touchSensitivity = 0.18f;
        [SerializeField] private Transform playerCamera;

        [Header("Head Bobbing")]
        [SerializeField] private float bobbingSpeed = 11f;
        [SerializeField] private float bobbingAmount = 0.05f;

        private CharacterController _cc;
        private LethalInventory _inventory;
        private float _verticalVelocity = 0f;
        private float _cameraPitch = 0f;
        private Vector3 _defaultCamPos;
        private float _bobTimer = 0f;
        private bool _isSprinting = false;

        public float CurrentWeightLb => _inventory != null ? _inventory.TotalWeightLb : 0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _cc = GetComponent<CharacterController>();
            _inventory = GetComponent<LethalInventory>();
            if (playerCamera != null) _defaultCamPos = playerCamera.localPosition;
        }

        private void Start()
        {
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

#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse != null)
            {
                lookInput += mouse.delta.ReadValue() * mouseSensitivity;
            }
#endif

            if (HorrorEscape.TouchLookPanel.Instance != null)
            {
                lookInput += HorrorEscape.TouchLookPanel.Instance.LookDelta * touchSensitivity;
            }

            transform.Rotate(Vector3.up * lookInput.x);

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

#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed) moveDir.y += 1f;
                if (kb.sKey.isPressed) moveDir.y -= 1f;
                if (kb.aKey.isPressed) moveDir.x -= 1f;
                if (kb.dKey.isPressed) moveDir.x += 1f;

                _isSprinting = kb.leftShiftKey.isPressed && moveDir.sqrMagnitude > 0.01f;

                if (kb.spaceKey.wasPressedThisFrame && _cc.isGrounded)
                {
                    _verticalVelocity = Mathf.Sqrt(2f * jumpHeight * gravity);
                }
            }
#endif

            var joystick = Survivor2D.VirtualJoystick.Instance;
            if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
            {
                moveDir = joystick.Direction;
            }

            moveDir.Normalize();

            // Weight Penalty calculation (Every 10 lb reduces speed by 5%, up to 50% max reduction)
            float weightPenalty = Mathf.Clamp(CurrentWeightLb * 0.005f, 0f, 0.5f);
            float currentSpeed = (_isSprinting ? baseRunSpeed : baseWalkSpeed) * (1f - weightPenalty);

            Vector3 worldMove = transform.right * moveDir.x + transform.forward * moveDir.y;

            if (_cc.isGrounded)
            {
                if (_verticalVelocity < 0f) _verticalVelocity = -1f;
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

        public void SetSprinting(bool s) => _isSprinting = s;
    }
}
