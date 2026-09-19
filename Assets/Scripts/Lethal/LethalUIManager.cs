using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LethalCompany
{
    public class LethalUIManager : MonoBehaviour
    {
        public static LethalUIManager Instance { get; private set; }

        [Header("HUD Text & Weight")]
        [SerializeField] private Text weightText;
        [SerializeField] private Text promptText;

        [Header("Time & Quota HUD")]
        [SerializeField] private Text timeText;
        [SerializeField] private Text quotaText;
        [SerializeField] private Text dayText;
        [SerializeField] private Text cargoText;

        [Header("4-Slot Inventory HUD")]
        [SerializeField] private Image[] slotFrames;
        [SerializeField] private Text[] slotItemNames;

        [Header("Scan Results Overlay")]
        [SerializeField] private Transform scanContainer;

        [Header("Day Summary Modal")]
        [SerializeField] private GameObject summaryPanel;
        [SerializeField] private Text summaryTitleText;
        [SerializeField] private Text summaryDetailsText;
        [SerializeField] private Button continueButton;

        private LethalInventory _inventory;
        private LethalInteraction _interaction;
        private Camera _mainCam;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

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
                _interaction.OnFocusPromptChanged += UpdatePromptDisplay;
                _interaction.OnScannedScraps += HandleScannedScraps;
            }

            if (LethalGameManager.Instance != null)
            {
                LethalGameManager.Instance.OnTimeChanged += UpdateTimeDisplay;
                LethalGameManager.Instance.OnQuotaUpdated += UpdateQuotaDisplay;
                UpdateQuotaDisplay(LethalGameManager.Instance.FulfilledQuota, LethalGameManager.Instance.TargetQuota, LethalGameManager.Instance.DaysLeft);
            }

            if (LethalShipManager.Instance != null)
            {
                LethalShipManager.Instance.OnCargoValueChanged += UpdateCargoDisplay;
                UpdateCargoDisplay(LethalShipManager.Instance.CurrentCargoValue);
            }

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(() =>
                {
                    if (LethalGameManager.Instance != null)
                    {
                        LethalGameManager.Instance.StartNextDay();
                    }
                });
            }

            if (summaryPanel != null) summaryPanel.SetActive(false);
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

        private void UpdatePromptDisplay(string prompt)
        {
            if (promptText != null)
            {
                if (!string.IsNullOrEmpty(prompt))
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = prompt;
                }
                else
                {
                    promptText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateTimeDisplay(float hour)
        {
            if (timeText != null && LethalGameManager.Instance != null)
            {
                timeText.text = LethalGameManager.Instance.GetFormattedTime();
            }
        }

        private void UpdateQuotaDisplay(int fulfilled, int target, int daysLeft)
        {
            if (quotaText != null)
            {
                quotaText.text = $"PROFIT QUOTA:\n<color=#44ff44>${fulfilled}</color> / ${target}";
            }

            if (dayText != null && LethalGameManager.Instance != null)
            {
                dayText.text = $"DAY {LethalGameManager.Instance.CurrentDay}\n<color=#ffcc00>{daysLeft} DAYS LEFT</color>";
            }
        }

        private void UpdateCargoDisplay(int cargoValue)
        {
            if (cargoText != null)
            {
                cargoText.text = $"SHIP CARGO: <color=#44ff44>${cargoValue}</color>";
            }
        }

        private void UpdateInventoryDisplay(int selectedSlot, ScrapItem[] items)
        {
            if (slotFrames == null) return;

            for (int i = 0; i < LethalInventory.MaxSlots; i++)
            {
                if (i >= slotFrames.Length) break;

                bool isSelected = (i == selectedSlot);
                slotFrames[i].color = isSelected ? new Color(0.1f, 0.8f, 1f, 0.85f) : new Color(0.15f, 0.15f, 0.2f, 0.5f);

                if (i < slotItemNames.Length && slotItemNames[i] != null)
                {
                    if (items != null && items[i] != null)
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

            foreach (Transform child in scanContainer)
            {
                Destroy(child.gameObject);
            }

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

        public void ShowDaySummary(List<ScrapItem> scraps, int earned, int totalFulfilled, int targetQuota, int daysLeft, bool isQuotaMet, bool isGameOver)
        {
            if (summaryPanel == null) return;
            summaryPanel.SetActive(true);

            // Build Summary Text
            string details = $"<b>[ SCRAP CARGO SOLD ]</b>\n";
            if (scraps.Count == 0)
            {
                details += "<color=#888888>(No scrap brought into ship)</color>\n";
            }
            else
            {
                foreach (var s in scraps)
                {
                    if (s != null)
                        details += $"• {s.itemName}  :  <color=#44ff44>+${s.scrapValue}</color>\n";
                }
            }

            details += $"\n<b>TODAY'S EARNINGS:</b> <color=#44ff44>+${earned}</color>\n";
            details += $"<b>TOTAL QUOTA:</b> ${totalFulfilled} / ${targetQuota}\n";
            details += $"<b>DAYS REMAINING:</b> {Mathf.Max(0, daysLeft)}\n\n";

            if (isGameOver)
            {
                if (summaryTitleText != null) summaryTitleText.text = "<color=#ff2222>YOU ARE FIRED</color>";
                details += "<color=#ff4444><b>PERFORMANCE REPORT:</b>\nYou failed to reach the company's profit quota.\nYour employment has been terminated.</color>";
            }
            else if (isQuotaMet)
            {
                if (summaryTitleText != null) summaryTitleText.text = "<color=#44ff44>QUOTA FULFILLED!</color>";
                details += "<color=#88ff88><b>GREAT WORK ASSET:</b>\nThe company is satisfied with your performance.\nA new, higher quota has been issued.</color>";
            }
            else
            {
                if (summaryTitleText != null) summaryTitleText.text = "<color=#ffcc00>SHIP IN ORBIT - DAY SUMMARY</color>";
                details += "<color=#ffcc66>Proceed to the next landing day to collect more scrap.</color>";
            }

            if (summaryDetailsText != null) summaryDetailsText.text = details;
        }

        public void HideDaySummary()
        {
            if (summaryPanel != null) summaryPanel.SetActive(false);
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
