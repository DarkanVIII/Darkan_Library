namespace Darkan.UpgradeSystem.Value
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class UpgradeDataBase<TUpgrade, TRarety> : SerializedScriptableObject where TUpgrade : Enum where TRarety : Enum
    {
        [Title("Upgrades")]
        [InfoBox("Warning: Changing the name of, or removing the enums used as Keys, will destroy all entries!")]
        [SerializeField]
        [PropertyOrder(-1)]
        Dictionary<(TUpgrade, TRarety), float> _upgrades = new();

        [Title("Rarety Chances")]
        [InfoBox("Warning: Changing the name of, or removing the enums used as Keys, will destroy all entries!")]
        [SerializeField]
        [PropertyOrder(2)]
        Dictionary<TRarety, float> _chances = new();

        [Title("Rarety Colors")]
        [InfoBox("Warning: Changing the name of, or removing the enums used as Keys, will destroy all entries!")]
        [SerializeField]
        [PropertyOrder(5)]
        Dictionary<TRarety, Color> _raretyColors = new();

        [SerializeField]
        [PropertyOrder(1)]
        bool _lockUpgradeDictionary;

        [SerializeField]
        [PropertyOrder(4)]
        bool _lockChancesDictionary;

        [SerializeField]
        [PropertyOrder(7)]
        bool _lockColorsDictionary;

        public Dictionary<(TUpgrade, TRarety), float> Upgrades => _upgrades;
        public Dictionary<TRarety, float> Chances => _chances;
        public Dictionary<TRarety, Color> RaretyColors => _raretyColors;


        [ButtonGroup("Upgrades")]
        [PropertyOrder(0)]
        void NewUpgradeDictionary()
        {
            if (_lockUpgradeDictionary) return;

            _upgrades.Clear();

            foreach (TUpgrade tUpgrade in Enum.GetValues(typeof(TUpgrade)))
            {
                foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
                {
                    _upgrades.Add((tUpgrade, tRarety), 0);
                }
            }
        }

        [ButtonGroup("Upgrades")]
        [Button("Add Missing Entries")]
        [PropertyOrder(0)]
        void AddMissingEntriesUpgrades()
        {
            if (_lockUpgradeDictionary) return;

            foreach (TUpgrade tUpgrade in Enum.GetValues(typeof(TUpgrade)))
            {
                foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
                {
                    if (_upgrades.ContainsKey((tUpgrade, tRarety))) continue;

                    _upgrades.Add((tUpgrade, tRarety), 0);
                }
            }
        }

        [ButtonGroup("Chances")]
        [PropertyOrder(3)]
        void NewChancesDictionary()
        {
            if (_lockChancesDictionary) return;

            _chances.Clear();

            foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
            {
                _chances.Add(tRarety, 0);
            }
        }

        [ButtonGroup("Chances")]
        [PropertyOrder(3)]
        [Button("Add Missing Entries")]
        void AddMissingEntriesChances()
        {
            if (_lockChancesDictionary) return;

            foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
            {
                if (_chances.ContainsKey(tRarety)) continue;

                _chances.Add(tRarety, 0);
            }
        }

        [ButtonGroup("Colors")]
        [PropertyOrder(6)]
        void NewColorsDictionary()
        {
            if (_lockColorsDictionary) return;

            _raretyColors.Clear();

            foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
            {
                _raretyColors.Add(tRarety, Color.black);
            }
        }

        [ButtonGroup("Colors")]
        [PropertyOrder(6)]
        [Button("Add Missing Entries")]
        void AddMissingEntriesColors()
        {
            if (_lockColorsDictionary) return;

            foreach (TRarety tRarety in Enum.GetValues(typeof(TRarety)))
            {
                if (_raretyColors.ContainsKey(tRarety)) continue;

                _raretyColors.Add(tRarety, Color.black);
            }
        }
    }
}
