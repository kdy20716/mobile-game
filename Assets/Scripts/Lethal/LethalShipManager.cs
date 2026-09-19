using System;
using System.Collections.Generic;
using UnityEngine;

namespace LethalCompany
{
    public class LethalShipManager : MonoBehaviour
    {
        public static LethalShipManager Instance { get; private set; }

        [Header("Cargo Zone")]
        [SerializeField] private Collider cargoCollider;
        private readonly HashSet<ScrapItem> itemsInShip = new HashSet<ScrapItem>();

        [Header("Ship Lever")]
        [SerializeField] private Transform leverHandle;
        private bool isLeverPulled = false;

        public event Action<int> OnCargoValueChanged;
        public int CurrentCargoValue { get; private set; } = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var scrap = other.GetComponent<ScrapItem>();
            if (scrap != null && !itemsInShip.Contains(scrap))
            {
                itemsInShip.Add(scrap);
                RecalculateCargoValue();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var scrap = other.GetComponent<ScrapItem>();
            if (scrap != null && itemsInShip.Contains(scrap))
            {
                itemsInShip.Remove(scrap);
                RecalculateCargoValue();
            }
        }

        public void RecalculateCargoValue()
        {
            itemsInShip.RemoveWhere(item => item == null);

            int sum = 0;
            foreach (var item in itemsInShip)
            {
                // Only count items currently on the floor in the ship (not held in hands)
                if (item != null && item.transform.parent == null)
                {
                    sum += item.scrapValue;
                }
            }

            CurrentCargoValue = sum;
            OnCargoValueChanged?.Invoke(CurrentCargoValue);
        }

        public List<ScrapItem> GetCargoScraps()
        {
            itemsInShip.RemoveWhere(item => item == null);
            var validList = new List<ScrapItem>();
            foreach (var item in itemsInShip)
            {
                if (item != null && item.transform.parent == null)
                {
                    validList.Add(item);
                }
            }
            return validList;
        }

        public void ClearCargoScraps()
        {
            var list = GetCargoScraps();
            foreach (var item in list)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            itemsInShip.Clear();
            RecalculateCargoValue();
        }

        public void PullLever()
        {
            if (isLeverPulled) return;
            isLeverPulled = true;

            if (leverHandle != null)
            {
                leverHandle.localRotation = Quaternion.Euler(45f, 0f, 0f);
            }

            Debug.Log("[LethalShip] Lever pulled! Ship taking off into orbit...");
            if (LethalGameManager.Instance != null)
            {
                LethalGameManager.Instance.EndDay();
            }
        }

        public void ResetLever()
        {
            isLeverPulled = false;
            if (leverHandle != null)
            {
                leverHandle.localRotation = Quaternion.Euler(-45f, 0f, 0f);
            }
        }
    }
}
