using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Darkan.UpgradeSystem.Ability
{
    public abstract class UpgradeDataAbilityBase<TAbility> : SerializedScriptableObject
    {
        [InfoBox("Warning: Changing the name of, or removing the enums used as Keys, will destroy all entries!")]
        [SerializeField]
        [PropertyOrder(-1)]
        Dictionary<TAbility, List<AbilityBase>> _upgrades = new();

        [SerializeField]
        [PropertyOrder(1)]
        bool _lockDictionary;

        public Dictionary<TAbility, List<AbilityBase>> Upgrades => _upgrades;

        [ButtonGroup("Upgrades")]
        [PropertyOrder(0)]
        void NewUpgradeDictionary()
        {
            if (_lockDictionary) return;
            _upgrades.Clear();

            foreach (TAbility tUpgrade in Enum.GetValues(typeof(TAbility)))
            {
                _upgrades.Add(tUpgrade, new List<AbilityBase>());
            }
        }

        [ButtonGroup("Upgrades")]
        [Button("Add Missing Entries")]
        [PropertyOrder(0)]
        void AddMissingEntriesUpgrades()
        {
            if (_lockDictionary) return;

            foreach (TAbility tUpgrade in Enum.GetValues(typeof(TAbility)))
            {
                if (_upgrades.ContainsKey(tUpgrade)) continue;

                _upgrades.Add(tUpgrade, new List<AbilityBase>());
            }
        }
    }
}
