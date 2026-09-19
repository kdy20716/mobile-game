using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor2D
{
    public class LevelUpManager : MonoBehaviour
    {
        public static LevelUpManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject levelUpPanel;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private Text[] choiceTitleTexts;
        [SerializeField] private Text[] choiceDescTexts;

        public enum UpgradeType
        {
            AddBlade,
            DamageBoost,
            AttackSpeed,
            MoveSpeed,
            MagnetRange,
            HealHp
        }

        private UpgradeType[] _currentChoices = new UpgradeType[3];

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                SurvivorPlayer2D.Instance.OnLevelUp += TriggerLevelUp;
            }

            if (levelUpPanel != null) levelUpPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (SurvivorPlayer2D.Instance != null)
            {
                SurvivorPlayer2D.Instance.OnLevelUp -= TriggerLevelUp;
            }
        }

        public void TriggerLevelUp(int newLevel)
        {
            Time.timeScale = 0f; // Pause game
            if (levelUpPanel != null) levelUpPanel.SetActive(true);

            // Roll 3 distinct upgrades
            List<UpgradeType> all = new List<UpgradeType>
            {
                UpgradeType.AddBlade,
                UpgradeType.DamageBoost,
                UpgradeType.AttackSpeed,
                UpgradeType.MoveSpeed,
                UpgradeType.MagnetRange,
                UpgradeType.HealHp
            };

            for (int i = 0; i < 3; i++)
            {
                int r = UnityEngine.Random.Range(0, all.Count);
                _currentChoices[i] = all[r];
                all.RemoveAt(r);

                SetupCard(i, _currentChoices[i]);
            }
        }

        private void SetupCard(int index, UpgradeType type)
        {
            if (index >= choiceButtons.Length) return;

            string title = "";
            string desc = "";

            switch (type)
            {
                case UpgradeType.AddBlade:
                    title = "🗡️ 수호 칼날 추가 (+1)";
                    desc = "플레이어 주위를 회전하는 칼날이 1개 추가됩니다.";
                    break;
                case UpgradeType.DamageBoost:
                    title = "💥 공격력 대폭 상승 (+30%)";
                    desc = "모든 무기의 위력이 30% 증가합니다.";
                    break;
                case UpgradeType.AttackSpeed:
                    title = "⚡ 공격 속도 가속 (+25%)";
                    desc = "칼날 회전 속도와 마법 미사일 발사 주기가 빨라집니다.";
                    break;
                case UpgradeType.MoveSpeed:
                    title = "👟 이동 속도 증가 (+20%)";
                    desc = "플레이어의 이동 속도가 20% 빨라집니다.";
                    break;
                case UpgradeType.MagnetRange:
                    title = "🧲 자석 흡수 반경 (+50%)";
                    desc = "경험치 보석을 더 먼 거리에서 자동으로 끌어당깁니다.";
                    break;
                case UpgradeType.HealHp:
                    title = "❤️ 생명력 회복 (+40 HP)";
                    desc = "체력을 즉시 40 회복합니다.";
                    break;
            }

            if (choiceTitleTexts != null && index < choiceTitleTexts.Length) choiceTitleTexts[index].text = title;
            if (choiceDescTexts != null && index < choiceDescTexts.Length) choiceDescTexts[index].text = desc;

            int choiceIndex = index;
            choiceButtons[index].onClick.RemoveAllListeners();
            choiceButtons[index].onClick.AddListener(() => ApplyChoice(choiceIndex));
        }

        private void ApplyChoice(int index)
        {
            UpgradeType selected = _currentChoices[index];
            var player = SurvivorPlayer2D.Instance;

            if (player != null)
            {
                switch (selected)
                {
                    case UpgradeType.AddBlade:
                        var bladeWeapon = player.GetComponentInChildren<OrbitingBladesWeapon>();
                        if (bladeWeapon != null) bladeWeapon.AddBlade();
                        break;
                    case UpgradeType.DamageBoost:
                        player.damageMultiplier += 0.3f;
                        break;
                    case UpgradeType.AttackSpeed:
                        player.attackSpeedMultiplier += 0.25f;
                        break;
                    case UpgradeType.MoveSpeed:
                        player.moveSpeed *= 1.2f;
                        break;
                    case UpgradeType.MagnetRange:
                        player.magnetRadius *= 1.5f;
                        break;
                    case UpgradeType.HealHp:
                        player.Heal(40f);
                        break;
                }
            }

            // Resume game
            if (levelUpPanel != null) levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void SetupUIReferences(GameObject panel, Button[] btns, Text[] titles, Text[] descs)
        {
            levelUpPanel = panel;
            choiceButtons = btns;
            choiceTitleTexts = titles;
            choiceDescTexts = descs;
        }
    }
}
