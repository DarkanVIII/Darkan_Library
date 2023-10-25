namespace Darkan.UpgradeSystem.Ability
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public abstract class UpgradeSlotBase<TAbility> : MonoBehaviour where TAbility : System.Enum
    {
        public void Init(UpgradeManagerBase<TAbility> upgradeManager)
        {
            _upgradeManager = upgradeManager;
        }

        public static event System.Action<AbilityDataBase, UpgradeManagerBase<TAbility>.UpgradeKey> OnFinishedSelection;

        [SerializeField] TextMeshProUGUI _description;
        [SerializeField] Button _button;

        [SerializeField]
        [TextArea]
        [Tooltip("Slots will show this text when not enough unique upgrades are available.")]
        string _emptySlotDescription = "No more Upgrades available";

        protected TextMeshProUGUI Description => _description;
        protected UpgradeManagerBase<TAbility> UpgradeManager => _upgradeManager;
        protected UpgradeManagerBase<TAbility>.UpgradeKey UpgradeKey => _upgradeKey;
        protected bool NoData => _noData;

        UpgradeManagerBase<TAbility> _upgradeManager;
        UpgradeManagerBase<TAbility>.UpgradeKey _upgradeKey;

        bool _noData;

        public void Select()
        {
            AbilityDataBase data = _upgradeManager.GetAbilityData(_upgradeKey);

            OnFinishedSelection?.Invoke(data, _upgradeKey);
        }

        public void Setup(UpgradeManagerBase<TAbility>.UpgradeKey key, bool noData = false)
        {
            _noData = noData;
            _upgradeKey = key;

            _button.interactable = !_noData;

            Populate();
        }

        protected virtual void Populate()
        {
            if
                (_noData) _description.text = _emptySlotDescription;
            else
                _description.text = $"{_upgradeKey.AbilityType} Level {_upgradeKey.AbilityLevel}";
        }

        public void OnPointerDown(PointerEventData eventData) { }
    }
}
