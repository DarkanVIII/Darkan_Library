namespace Darkan.UpgradeSystem.Ability
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class UpgradeManagerBase<TAbility> : SerializedMonoBehaviour where TAbility : System.Enum
    {
        [SerializeField] UpgradeDataBase<TAbility> _upgradeData;
        [SerializeField] Canvas _upgradeCanvas;
        [SerializeField] Transform _slotParent;
        [SerializeField] GameObject _slotPrefab;

        readonly List<TAbility> _availableUpgradeTypes = new();
        readonly Stack<TAbility> _cachedUpgradeTypes = new();
        readonly LinkedList<Transform> _slots = new();
        readonly Stack<Transform> _cachedSlots = new();
        readonly Dictionary<TAbility, int> _abilityLevels = new();

        public struct UpgradeKey
        {
            public TAbility AbilityType;
            public int AbilityLevel;
        }

        void Awake()
        {
            foreach (TAbility abilityType in System.Enum.GetValues(typeof(TAbility)))
            {
                _availableUpgradeTypes.Add(abilityType);
                _abilityLevels.Add(abilityType, 0);
            }
        }

        bool TryGetRandomUpgrade(out UpgradeKey upgradeKey)
        {
            if (_availableUpgradeTypes.Count == 0)
            {
                upgradeKey = default;
                return false;
            }

            int rand = Random.Range(0, _availableUpgradeTypes.Count);

            TAbility abilityType = _availableUpgradeTypes[rand];

            int abilityLevel;

            if (_upgradeData.Upgrades.ContainsKey(abilityType))
            {
                abilityLevel = _abilityLevels[abilityType] + 1;
            }
            else
            {
                Debug.LogWarning($"Upgrade with Key {abilityType} could not be found. Returned empty.");
                upgradeKey = default;
                return default;
            }

            upgradeKey = new UpgradeKey()
            {
                AbilityType = abilityType,
                AbilityLevel = abilityLevel
            };

            return true;
        }


        /// <returns>False if no Upgrades are available or the amount is unresonable (smaller 1, bigger 50)</returns>
        [Button("Debug Create Upgrade Selection (Use Ingame to avoid cleanup)")]
        public bool TryCreateUpgradeSelection(int amount)
        {
            if (amount < 1 || amount > 50) return false;

            if (_availableUpgradeTypes.Count == 0)
            {
                Debug.LogWarning("No available Upgrades found");
                return false;
            }

            _upgradeCanvas.enabled = true;

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
                    {
                        Transform transform = Instantiate(_slotPrefab, _slotParent).transform;
                        transform.GetComponent<UpgradeSlotBase<TAbility>>().Init(this);
                        _slots.AddLast(transform);
                    }
                }
            }

            int counter = -1;

            foreach (Transform transform in _slots)
            {
                counter++;

                if (TryGetRandomUpgrade(out UpgradeKey upgradeKey))
                {
                    _availableUpgradeTypes.Remove(upgradeKey.AbilityType);
                    _cachedUpgradeTypes.Push(upgradeKey.AbilityType);

                    transform.GetComponent<UpgradeSlotBase<TAbility>>().Setup(upgradeKey);
                }
                else
                    transform.GetComponent<UpgradeSlotBase<TAbility>>().Setup(upgradeKey, true);
            }

            _availableUpgradeTypes.AddRange(_cachedUpgradeTypes);
            _cachedUpgradeTypes.Clear();

            return true;
        }

        /// <summary>
        /// Get Ability Data by Key. Also updates the current Ability Levels.
        /// </summary>
        public AbilityDataBase GetAbilityData(UpgradeKey key)
        {

            if (_abilityLevels.ContainsKey(key.AbilityType))
            {
                _upgradeCanvas.enabled = false;
                _abilityLevels[key.AbilityType] = key.AbilityLevel;

                int maxLevel = _upgradeData.Upgrades[key.AbilityType].Count;

                if (key.AbilityLevel == maxLevel)
                    _availableUpgradeTypes.Remove(key.AbilityType);

                return _upgradeData.Upgrades[key.AbilityType][key.AbilityLevel - 1];
            }
            else
                return null;
        }
    }
}