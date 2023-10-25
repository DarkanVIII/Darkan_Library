namespace Darkan.UpgradeSystem.Value
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class UpgradeManagerBase<TUpgrade, TRarety> : SerializedMonoBehaviour where TUpgrade : Enum where TRarety : Enum
    {
        [SerializeField] UpgradeDataBase<TUpgrade, TRarety> _upgradeData;
        [SerializeField] Canvas _upgradeCanvas;
        [SerializeField] GameObject _slotPrefab;

        float _raretyTotal;
        TUpgrade[] _upgradeTypes;
        readonly LinkedList<Transform> _slots = new();
        readonly Stack<Transform> _cachedSlots = new();
        readonly List<Upgrade> _cachedUpgrades = new();

        public struct Upgrade
        {
            public TUpgrade UpgradeType;
            public TRarety Rarety;
            public float Value;
            public override readonly string ToString()
            {
                return $"{Rarety} {UpgradeType} {Value}";
            }
        }

        protected virtual void Awake()
        {
            SetRaretyTotal();
            _upgradeTypes = (TUpgrade[])Enum.GetValues(typeof(TUpgrade));
        }

        public Upgrade GetRandomUpgrade()
        {
            float total = _raretyTotal;
            float randRarety = UnityEngine.Random.Range(0, total);

            TRarety rarety = default;

            foreach (var keyValuePair in _upgradeData.Chances)
            {
                if (randRarety <= keyValuePair.Value)
                {
                    rarety = keyValuePair.Key;
                    break;
                }

                randRarety -= keyValuePair.Value;
            }

            int randUpgrade = UnityEngine.Random.Range(0, _upgradeTypes.Length);

            TUpgrade upgradeType = _upgradeTypes[randUpgrade];

            if (!_upgradeData.Upgrades.TryGetValue((upgradeType, rarety), out float value))
                Debug.LogWarning($"Upgrade with Keys {upgradeType} {rarety} could not be found. Returned Value 0.");

            return new Upgrade()
            {
                UpgradeType = upgradeType,
                Rarety = rarety,
                Value = value
            };
        }

        public List<Upgrade> GetRandomSetOfUniqueUpgrades(int amount, List<Upgrade> set)
        {
            set.Clear();
            set.Capacity = amount;

            for (int i = 1; i <= amount; i++)
            {
                Upgrade upgrade = GetRandomUpgrade();

                if (set.Contains(upgrade))
                    i--;
                else
                    set.Add(upgrade);
            }

            return set;
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
            GetRandomSetOfUniqueUpgrades(amount, _cachedUpgrades);

            foreach (Transform transform in _slots)
            {
                counter++;

                Upgrade upgrade = _cachedUpgrades[counter];
                Color raretyColor = _upgradeData.RaretyColors[upgrade.Rarety];

                string description = CreateDescription(upgrade);

                transform.GetComponent<UpgradeSlot>().Populate(raretyColor, description);
            }
        }

        protected virtual string CreateDescription(Upgrade upgrade)
        {
            return $"Increase {upgrade.UpgradeType} by {upgrade.Value}";
        }

        void SetRaretyTotal()
        {
            foreach (float value in _upgradeData.Chances.Values)
            {
                _raretyTotal += value;
            }
        }
    }
}
