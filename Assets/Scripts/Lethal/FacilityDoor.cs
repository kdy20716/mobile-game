using UnityEngine;

namespace LethalCompany
{
    public class FacilityDoor : MonoBehaviour
    {
        [Header("Door Settings")]
        [SerializeField] private string doorActionName = "Enter Facility";
        [SerializeField] private Transform destinationPoint;
        [SerializeField] private bool isExit = false;

        public string PromptText => $"[E] {doorActionName}";

        public void SetDestination(Transform dest, bool exit = false)
        {
            destinationPoint = dest;
            isExit = exit;
            doorActionName = isExit ? "Exit to Ship" : "Enter Facility";
        }

        public void Interact(GameObject player)
        {
            if (destinationPoint == null)
            {
                Debug.LogWarning("[FacilityDoor] Destination point is null!");
                return;
            }

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // Move player to destination point slightly in front
            Vector3 destPos = destinationPoint.position + destinationPoint.forward * 1.2f;
            player.transform.position = destPos;
            player.transform.rotation = Quaternion.Euler(0f, destinationPoint.eulerAngles.y, 0f);

            if (cc != null) cc.enabled = true;

            Debug.Log($"[FacilityDoor] Player transitioned through door to: {destPos}");
        }
    }
}
