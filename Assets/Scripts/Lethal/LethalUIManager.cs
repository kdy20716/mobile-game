using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LethalCompany
{
    public class LethalUIManager : MonoBehaviour
    {
        [Header("HUD Text & Weight")]
        [SerializeField] private Text weightText;
        [SerializeField] private Text promptText;

        [Header("4-Slot Inventory HUD")]
        [SerializeField] private Image[] slotFrames;
        [SerializeField] private Text[] slotItemNames;

        [Header("Scan Results Overlay")]
        [SerializeField] private GameObject scanPingPrefab;
        [SerializeField] private Transform scanContainer;

        private LethalInventory _inventory;
        private LethalInteraction _interaction;
        private Camera _mainCam;

        private void Start()
        {
            _inventory = Object.FindFirstObjectByType<LethalInventory>();
            _interaction = Object.FindFirstObjectByType<LethalInteraction>();
            _mainCam = Camera.main;

            if (_inventory != null)
            {
                _inventory.OnInventoryChanged += UpdateInventoryDisplay;
                _inventory.OnWeightChanged += UpdateWeightDisplay;
                UpdateWeightDisplay(_inventory.TotalWeightLb);
            }

            if (_interaction != null)
            {
                _interaction.OnFocusScrapChanged += UpdatePromptDisplay;
                _interaction.OnScannedScraps += HandleScannedScraps;
            }

            UpdatePromptDisplay(null);
        }

        private void UpdateWeightDisplay(float weight)
        {
            if (weightText != null)
            {
                weightText.text = $"WEIGHT: {Mathf.RoundToInt(weight)} lb";
                weightText.color = weight > 40f ? new Color(1f, 0.4f, 0.4f) : Color.white;
            }
        }

        private void UpdatePromptDisplay(ScrapItem scrap)
        {
            if (promptText != null)
            {
                if (scrap != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = $"[E] Grab {scrap.itemName}\n<color=#44ff44>${scrap.scrapValue}</color> ({scrap.weightLb} lb)";
                }
                else
                {
                    promptText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateInventoryDisplay(int selectedSlot, ScrapItem[] items)
        {
            for (int i = 0; i < LethalInventory.MaxSlots; i++)
            {
                if (i >= slotFrames.Length) break;

                // Highlight current slot
                bool isSelected = (i == selectedSlot);
                slotFrames[i].color = isSelected ? new Color(0.1f, 0.8f, 1f, 0.85f) : new Color(0.15f, 0.15f, 0.2f, 0.5f);

                // Show item name
                if (i < slotItemNames.Length && slotItemNames[i] != null)
                {
                    if (items[i] != null)
                    {
                        slotItemNames[i].text = items[i].itemName;
                        slotItemNames[i].color = Color.white;
                    }
                    else
                    {
                        slotItemNames[i].text = $"[ {i + 1} ]";
                        slotItemNames[i].color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
                    }
                }
            }
        }

        private void HandleScannedScraps(List<ScrapItem> scraps)
        {
            StopAllCoroutines();
            StartCoroutine(ShowScanPings(scraps));
        }

        private IEnumerator ShowScanPings(List<ScrapItem> scraps)
        {
            if (scanContainer == null || _mainCam == null) yield break;

            // Clear previous pings
            foreach (Transform child in scanContainer)
            {
                Destroy(child.gameObject);
            }

            // Create 3D world space text or screen space ping
            List<GameObject> pings = new List<GameObject>();
            foreach (var s in scraps)
            {
                if (s == null) continue;

                GameObject ping = new GameObject("ScanPing", typeof(RectTransform));
                ping.transform.SetParent(scanContainer);

                Text t = ping.AddComponent<Text>();
                t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                t.fontSize = 22;
                t.alignment = TextAnchor.MiddleCenter;
                t.text = $"◎ {s.itemName}\n<color=#66ff66>${s.scrapValue}</color>";
                t.color = new Color(0.2f, 0.85f, 1f, 0.95f);

                var tracker = ping.AddComponent<WorldToScreenTracker>();
                tracker.target = s.transform;
                tracker.cam = _mainCam;

                pings.Add(ping);
            }

            yield return new WaitForSeconds(3.0f);

            foreach (var p in pings)
            {
                if (p != null) Destroy(p);
            }
        }

        public void SetupReferences(Text weight, Text prompt, Image[] frames, Text[] names, Transform scanParent)
        {
            weightText = weight;
            promptText = prompt;
            slotFrames = frames;
            slotItemNames = names;
            scanContainer = scanParent;
        }
    }

    public class WorldToScreenTracker : MonoBehaviour
    {
        public Transform target;
        public Camera cam;
        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (target == null || cam == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 screenPos = cam.WorldToScreenPoint(target.position + Vector3.up * 0.3f);
            if (screenPos.z > 0)
            {
                gameObject.SetActive(true);
                _rt.position = screenPos;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
