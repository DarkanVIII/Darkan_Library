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

        public static event System.Action<AbilityDataBase<TAbility>, UpgradeManagerBase<TAbility>.UpgradeKey> OnFinishedSelection;

        [SerializeField] TextMeshProUGUI _description;
        [SerializeField] Button _button;

        [SerializeField]
        [TextArea]
        [Tooltip("Slots will show this text when not enough unique upgrades are available.")]
        string _emptySlotDescription = "No more Upgrades available";

        protected TextMeshProUGUI Description => _description;
        protected UpgradeManagerBase<TAbility> UpgradeManager => _upgradeManager;
        protected UpgradeManagerBase<TAbility>.UpgradeKey UpgradeKey => _upgradeKey;
        protected AbilityDataBase<TAbility> AbilityData => _abilityData;

        UpgradeManagerBase<TAbility> _upgradeManager;
        UpgradeManagerBase<TAbility>.UpgradeKey _upgradeKey;

        AbilityDataBase<TAbility> _abilityData;

        public void Select()
        {
            _upgradeManager.GetAbilityData(_upgradeKey);

            OnFinishedSelection?.Invoke(_abilityData, _upgradeKey);
        }

        public void Setup(UpgradeManagerBase<TAbility>.UpgradeKey key, AbilityDataBase<TAbility> abilityData)
        {
            _upgradeKey = key;
            _abilityData = abilityData;

            Populate();
        }

        public void EmptySlot()
        {
            PopulateEmpty();
        }

        protected virtual void PopulateEmpty()
        {
            _button.interactable = false;
            _description.text = _emptySlotDescription;
        }

        protected virtual void Populate()
        {
            _button.interactable = true;
            _description.text = _abilityData.GetDescription(_upgradeKey);
        }

        public void OnPointerDown(PointerEventData eventData) { }
    }
}
