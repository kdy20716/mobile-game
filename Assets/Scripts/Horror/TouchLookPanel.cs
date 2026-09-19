using UnityEngine;
using UnityEngine.EventSystems;

namespace HorrorEscape
{
    public class TouchLookPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static TouchLookPanel Instance { get; private set; }

        public Vector2 LookDelta { get; private set; }
        private int _touchPointerId = -1;
        private Vector2 _lastPointerPos;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_touchPointerId == -1)
            {
                _touchPointerId = eventData.pointerId;
                _lastPointerPos = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == _touchPointerId)
            {
                LookDelta = eventData.position - _lastPointerPos;
                _lastPointerPos = eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == _touchPointerId)
            {
                _touchPointerId = -1;
                LookDelta = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            // Reset delta each frame so it only reports active movement
            LookDelta = Vector2.zero;
        }
    }
}
