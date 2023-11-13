namespace Darkan.RuntimeTools
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TextPopupPreset", menuName = "Data/TextPopupPreset")]
    public class TextPopupPresetData : ScriptableObject
    {
        [SerializeField] TextPopupParams _popupParams = TextPopupParams.BasicWhite;

        public TextPopupParams Params => _popupParams;
    }
}
