namespace Darkan.UpgradeSystem.Value
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UpgradeSlot : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _description;

        Image _imgRarety;

        void Awake()
        {
            _imgRarety = GetComponent<Image>();
        }

        public void Populate(Color raretyColor, string description)
        {
            _imgRarety.color = raretyColor;
            _description.text = description;
        }
    }
}


