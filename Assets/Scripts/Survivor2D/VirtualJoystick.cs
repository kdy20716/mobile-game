using UnityEngine;
using UnityEngine.EventSystems;

namespace Survivor2D
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static VirtualJoystick Instance { get; private set; }

        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 60f;

        public Vector2 Direction { get; private set; } = Vector2.zero;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (background == null) background = GetComponent<RectTransform>();
            if (handle == null && transform.childCount > 0) handle = transform.GetChild(0).GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position))
            {
                position = Vector2.ClampMagnitude(position, handleRange);
                if (handle != null) handle.anchoredPosition = position;
                Direction = position / handleRange;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            if (handle != null) handle.anchoredPosition = Vector2.zero;
        }

        public void SetupReferences(RectTransform bg, RectTransform hd, float range = 60f)
        {
            background = bg;
            handle = hd;
            handleRange = range;
        }
    }
}
