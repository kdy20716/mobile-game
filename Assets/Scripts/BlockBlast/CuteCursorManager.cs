using UnityEngine;

namespace BlockBlast
{
    public class CuteCursorManager : MonoBehaviour
    {
        public static CuteCursorManager Instance { get; private set; }

        [Header("Cursor Textures")]
        [SerializeField] private Texture2D normalCursor;
        [SerializeField] private Texture2D clickCursor;
        [SerializeField] private Vector2 hotSpot = new Vector2(3f, 3f);

        private bool _isPressed = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            ApplyCursor(false);
        }

        private void Start()
        {
            ApplyCursor(false);
        }

        private void OnEnable()
        {
            ApplyCursor(false);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                ApplyCursor(_isPressed);
            }
        }

        public void Setup(Texture2D normal, Texture2D click, Vector2 spot)
        {
            normalCursor = normal;
            clickCursor = click;
            hotSpot = spot;
            ApplyCursor(false);
        }

        private void Update()
        {
            bool pressing = false;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                pressing = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
            }
#else
            pressing = Input.GetMouseButton(0);
#endif

            if (pressing != _isPressed)
            {
                _isPressed = pressing;
                ApplyCursor(_isPressed);
            }
        }

        public void ApplyCursor(bool isClick)
        {
            Texture2D tex = isClick ? (clickCursor != null ? clickCursor : normalCursor) : normalCursor;
            if (tex != null)
            {
                Cursor.SetCursor(tex, hotSpot, CursorMode.Auto);
            }
        }
    }
}
