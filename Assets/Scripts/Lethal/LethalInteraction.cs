using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LethalCompany
{
    public class LethalInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float reachDistance = 3.2f;
        [SerializeField] private LayerMask interactLayer = ~0;
        [SerializeField] private Transform playerCamera;

        [Header("Scanner Settings")]
        [SerializeField] private float scanRange = 16f;
        [SerializeField] private float scanCooldown = 1.2f;

        private LethalInventory _inventory;
        private float _lastScanTime = -10f;

        public ScrapItem HighlightedScrap { get; private set; }
        public FacilityDoor HighlightedDoor { get; private set; }
        public ShipLever HighlightedLever { get; private set; }

        public event Action<ScrapItem> OnFocusScrapChanged; // null if not looking at scrap
        public event Action<string> OnFocusPromptChanged; // General prompt text (e.g. "[E] Enter Facility")
        public event Action<List<ScrapItem>> OnScannedScraps; // list of detected scraps

        private void Awake()
        {
            _inventory = GetComponent<LethalInventory>();
        }

        private void Update()
        {
            CheckFocusItem();

            // E key: Grab scrap or Interact with Door/Lever
            // G key: Drop scrap
            // Right Click: Scan
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            if (kb != null)
            {
                if (kb.eKey.wasPressedThisFrame) PerformPrimaryInteraction();
                if (kb.gKey.wasPressedThisFrame) DropCurrentItem();
            }

            if (mouse != null)
            {
                if (mouse.rightButton.wasPressedThisFrame) TriggerScan();
            }
#endif
        }

        private void CheckFocusItem()
        {
            if (playerCamera == null) return;

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            ScrapItem foundScrap = null;
            FacilityDoor foundDoor = null;
            ShipLever foundLever = null;

            if (Physics.Raycast(ray, out hit, reachDistance, interactLayer))
            {
                foundScrap = hit.collider.GetComponentInParent<ScrapItem>();
                foundDoor = hit.collider.GetComponentInParent<FacilityDoor>();
                foundLever = hit.collider.GetComponentInParent<ShipLever>();
            }

            if (foundScrap != HighlightedScrap)
            {
                HighlightedScrap = foundScrap;
                OnFocusScrapChanged?.Invoke(HighlightedScrap);
            }

            HighlightedDoor = foundDoor;
            HighlightedLever = foundLever;

            // Update prompt text
            if (foundScrap != null)
            {
                OnFocusPromptChanged?.Invoke($"[E] Grab {foundScrap.itemName} (${foundScrap.scrapValue})");
            }
            else if (foundDoor != null)
            {
                OnFocusPromptChanged?.Invoke(foundDoor.PromptText);
            }
            else if (foundLever != null)
            {
                OnFocusPromptChanged?.Invoke(foundLever.PromptText);
            }
            else
            {
                OnFocusPromptChanged?.Invoke(null);
            }
        }

        public void PerformPrimaryInteraction()
        {
            if (HighlightedScrap != null)
            {
                GrabFocusedItem();
            }
            else if (HighlightedDoor != null)
            {
                HighlightedDoor.Interact(gameObject);
            }
            else if (HighlightedLever != null)
            {
                HighlightedLever.PullLever();
            }
        }

        public void GrabFocusedItem()
        {
            if (HighlightedScrap != null && _inventory != null)
            {
                if (_inventory.TryAddItem(HighlightedScrap))
                {
                    HighlightedScrap = null;
                    OnFocusScrapChanged?.Invoke(null);
                    OnFocusPromptChanged?.Invoke(null);
                }
            }
        }

        public void DropCurrentItem()
        {
            if (_inventory != null && playerCamera != null)
            {
                Vector3 dropPos = playerCamera.position + playerCamera.forward * 0.8f;
                _inventory.DropCurrentItem(dropPos, playerCamera.forward);
            }
        }

        public void TriggerScan()
        {
            if (Time.time - _lastScanTime < scanCooldown || playerCamera == null) return;
            _lastScanTime = Time.time;

            List<ScrapItem> detected = new List<ScrapItem>();
            var allScraps = UnityEngine.Object.FindObjectsByType<ScrapItem>(FindObjectsSortMode.None);

            foreach (var scrap in allScraps)
            {
                if (scrap == null || !scrap.gameObject.activeInHierarchy || scrap.transform.parent != null) continue;

                Vector3 toScrap = scrap.transform.position - playerCamera.position;
                if (toScrap.magnitude <= scanRange)
                {
                    float angle = Vector3.Angle(playerCamera.forward, toScrap);
                    if (angle <= 65f) // Cone of view
                    {
                        detected.Add(scrap);
                    }
                }
            }

            OnScannedScraps?.Invoke(detected);
        }

        public void SetCamera(Transform cam)
        {
            playerCamera = cam;
        }
    }
}
