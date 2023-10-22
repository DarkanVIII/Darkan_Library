namespace Darkan.UpgradeSystem.Ability
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public class UpgradeManagerAbilityBase<TAbility> : SerializedMonoBehaviour where TAbility : System.Enum
    {
        [SerializeField] UpgradeDataAbilityBase<TAbility> _upgradeData;
        [SerializeField] Canvas _upgradeCanvas;
        [SerializeField] GameObject _slotPrefab;

        readonly List<TAbility> _availableUpgradeTypes = new();
        readonly Stack<TAbility> _cachedUpgradeTypes = new();
        readonly LinkedList<Transform> _slots = new();
        readonly Stack<Transform> _cachedSlots = new();
        readonly Dictionary<TAbility, int> _abilityLevels = new();

        public struct AbilityUpgrade
        {
            public TAbility UpgradeType;
            public AbilityBase Ability;
            public int AbilityLevel;
            public override readonly string ToString()
            {
                return $"{UpgradeType} Level {AbilityLevel}";
            }
        }

        protected virtual void Awake()
        {
            foreach (TAbility abilityType in System.Enum.GetValues(typeof(TAbility)))
            {
                _availableUpgradeTypes.Add(abilityType);
                _abilityLevels.Add(abilityType, 0);
            }
        }

        public AbilityUpgrade GetRandomUpgrade()
        {
            int rand = Random.Range(0, _availableUpgradeTypes.Count);

            TAbility abilityType = _availableUpgradeTypes[rand];

            AbilityBase ability;

            int abilityLevel = 0;

            if (_upgradeData.Upgrades.ContainsKey(abilityType))
            {
                abilityLevel = _abilityLevels[abilityType] + 1;

                ability = _upgradeData.Upgrades[abilityType][abilityLevel - 1];

                int maxLevel = _upgradeData.Upgrades[abilityType].Count;

                if (abilityLevel == maxLevel)
                    _availableUpgradeTypes.Remove(abilityType);
            }
            else
            {
                Debug.LogWarning($"Upgrade with Key {abilityType} could not be found. Returned null.");
                ability = null;
            }

            return new AbilityUpgrade()
            {
                UpgradeType = abilityType,
                Ability = ability,
                AbilityLevel = abilityLevel
            };
        }

        public void CreateUpgradeSelection(int amount)
        {
            _upgradeCanvas.enabled = true;

            Transform parent = _upgradeCanvas.transform.GetChild(0);

            int neededSlots = amount - _slots.Count;

            if (neededSlots < 0)
            {
                for (int i = 1; i <= -neededSlots; i++)
                {
                    Transform slot = _slots.Last.Value;
                    slot.gameObject.SetActive(false);
                    _cachedSlots.Push(slot);
                    _slots.RemoveLast();
                }
            }
            else if (neededSlots > 0)
            {
                for (int i = 1; i <= neededSlots; i++)
                {
                    if (_cachedSlots.TryPop(out var slot))
                    {
                        _slots.AddLast(slot);
                        slot.gameObject.SetActive(true);
                    }
                    else
                        _slots.AddLast(Instantiate(_slotPrefab, parent).transform);
                }
            }

            int counter = -1;

            foreach (Transform transform in _slots)
            {
                counter++;

                AbilityUpgrade upgrade = GetRandomUpgrade();

                _availableUpgradeTypes.Remove(upgrade.UpgradeType);
                _cachedUpgradeTypes.Push(upgrade.UpgradeType);

                string description = CreateDescription(upgrade);

                transform.GetComponent<UpgradeSlotAbility>().Populate(description);
            }

            _availableUpgradeTypes.AddRange(_cachedUpgradeTypes);
            _cachedUpgradeTypes.Clear();
        }

        /// <summary>
        /// Pass in each selected Upgrade to Update the internal Ability Levels
        /// </summary>
        public void UpgradeSelected(AbilityUpgrade upgrade)
        {
            if (_abilityLevels.ContainsKey(upgrade.UpgradeType))
            {
                _abilityLevels[upgrade.UpgradeType] = upgrade.AbilityLevel;
            }
        }

        protected virtual string CreateDescription(AbilityUpgrade upgrade)
        {
            return upgrade.ToString();
        }
    }
}


