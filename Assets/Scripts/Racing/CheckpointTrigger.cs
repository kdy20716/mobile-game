using UnityEngine;

namespace MobileRacing
{
    [RequireComponent(typeof(Collider))]
    public class CheckpointTrigger : MonoBehaviour
    {
        public int CheckpointIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<ArcadeCarController>() != null)
            {
                if (CheckpointTrackManager.Instance != null)
                {
                    CheckpointTrackManager.Instance.CheckpointTriggered(CheckpointIndex);
                }
            }
        }
    }
}
