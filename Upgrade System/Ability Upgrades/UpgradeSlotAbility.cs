namespace Darkan.UpgradeSystem.Ability
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UpgradeSlotAbility : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _description;



        public void Populate(string description)
        {
            _description.text = description;
        }
    }
}
