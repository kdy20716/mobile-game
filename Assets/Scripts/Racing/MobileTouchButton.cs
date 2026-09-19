using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MobileRacing
{
    public class MobileTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonType
        {
            SteerLeft,
            SteerRight,
            Accelerate,
            Brake,
            Boost,
            Handbrake
        }

        public ButtonType type;

        public void OnPointerDown(PointerEventData eventData)
        {
            SetState(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetState(false);
        }

        private void SetState(bool isPressed)
        {
            if (MobileInputManager.Instance == null) return;

            switch (type)
            {
                case ButtonType.SteerLeft:
                    MobileInputManager.Instance.SetSteerLeft(isPressed);
                    break;
                case ButtonType.SteerRight:
                    MobileInputManager.Instance.SetSteerRight(isPressed);
                    break;
                case ButtonType.Accelerate:
                    MobileInputManager.Instance.SetAccelerate(isPressed);
                    break;
                case ButtonType.Brake:
                    MobileInputManager.Instance.SetBrake(isPressed);
                    break;
                case ButtonType.Boost:
                    MobileInputManager.Instance.SetBoost(isPressed);
                    break;
                case ButtonType.Handbrake:
                    MobileInputManager.Instance.SetHandbrake(isPressed);
                    break;
            }
        }

        private void OnDisable()
        {
            SetState(false);
        }
    }
}
