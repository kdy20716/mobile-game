using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LethalCompany
{
    public class LethalInventory : MonoBehaviour
    {
        public const int MaxSlots = 4;
        private ScrapItem[] _slots = new ScrapItem[MaxSlots];
        public int CurrentSlotIndex { get; private set; } = 0;

        [Header("Hand Holder")]
        [SerializeField] private Transform handHolder;

        public event Action<int, ScrapItem[]> OnInventoryChanged; // (currentSlot, allSlots)
        public event Action<float> OnWeightChanged; // totalWeightLb

        public float TotalWeightLb { get; private set; } = 0f;

        private void Start()
        {
            UpdateWeight();
            OnInventoryChanged?.Invoke(CurrentSlotIndex, _slots);
        }

        private void Update()
        {
            // Number keys 1, 2, 3, 4 to switch slots
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.digit1Key.wasPressedThisFrame) SelectSlot(0);
                else if (kb.digit2Key.wasPressedThisFrame) SelectSlot(1);
                else if (kb.digit3Key.wasPressedThisFrame) SelectSlot(2);
                else if (kb.digit4Key.wasPressedThisFrame) SelectSlot(3);
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll > 0f) SelectSlot((CurrentSlotIndex + 1) % MaxSlots);
                else if (scroll < 0f) SelectSlot((CurrentSlotIndex - 1 + MaxSlots) % MaxSlots);
            }
#endif
        }

        public bool TryAddItem(ScrapItem item)
        {
            // First try current slot if empty
            if (_slots[CurrentSlotIndex] == null)
            {
                _slots[CurrentSlotIndex] = item;
                item.SetPickedUp(handHolder);
                UpdateWeight();
                OnInventoryChanged?.Invoke(CurrentSlotIndex, _slots);
                return true;
            }

            // Otherwise find first empty slot
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i] == null)
                {
                    _slots[i] = item;
                    item.SetPickedUp(handHolder);
                    item.SetStoredInInventory(); // Hide it until selected
                    UpdateWeight();
                    OnInventoryChanged?.Invoke(CurrentSlotIndex, _slots);
                    return true;
                }
            }

            return false; // Inventory full
        }

        public ScrapItem DropCurrentItem(Vector3 dropPos, Vector3 throwDirection)
        {
            ScrapItem item = _slots[CurrentSlotIndex];
            if (item != null)
            {
                _slots[CurrentSlotIndex] = null;
                item.SetDropped(dropPos, throwDirection * 4.5f + Vector3.up * 1.5f);
                UpdateWeight();
                OnInventoryChanged?.Invoke(CurrentSlotIndex, _slots);
                return item;
            }
            return null;
        }

        public void SelectSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots) return;

            // Hide previous item
            if (_slots[CurrentSlotIndex] != null)
            {
                _slots[CurrentSlotIndex].SetStoredInInventory();
            }

            CurrentSlotIndex = slotIndex;

            // Show new item
            if (_slots[CurrentSlotIndex] != null)
            {
                _slots[CurrentSlotIndex].SetPickedUp(handHolder);
            }

            OnInventoryChanged?.Invoke(CurrentSlotIndex, _slots);
        }

        private void UpdateWeight()
        {
            float w = 0f;
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i] != null) w += _slots[i].weightLb;
            }
            TotalWeightLb = w;
            OnWeightChanged?.Invoke(TotalWeightLb);
        }

        public void SetHandHolder(Transform holder)
        {
            handHolder = holder;
        }

        public ScrapItem GetItemAt(int index) => (index >= 0 && index < MaxSlots) ? _slots[index] : null;
        public ScrapItem GetCurrentItem() => _slots[CurrentSlotIndex];
    }
}
