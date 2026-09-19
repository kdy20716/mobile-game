using UnityEngine;

namespace LethalCompany
{
    public class ShipLever : MonoBehaviour
    {
        [SerializeField] private string actionName = "Start Ship / Take Off";

        public string PromptText => $"[E] {actionName}";

        public void PullLever()
        {
            if (LethalShipManager.Instance != null)
            {
                LethalShipManager.Instance.PullLever();
            }
            else
            {
                Debug.LogWarning("[ShipLever] LethalShipManager Instance not found!");
            }
        }
    }
}
