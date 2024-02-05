namespace Darkan.RuntimeTools
{
    using Darkan.Pooling;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class PopupSpawner : MonoBehaviour
    {
        [SerializeField]
        [Required]
        Transform _textPopupPrefab;

        [SerializeField]
        [Tooltip("Optional")]
        Transform _lookAtTarget;

        ObjectPooler<TextPopup> _popupPooler;

        void Awake()
        {
            _popupPooler = new ObjectPooler<TextPopup>(OnCreate, OnRelease, OnGet, null, 5);
        }

        #region Object Pooling

        TextPopup OnCreate()
        {
            TextPopup popup = Instantiate(_textPopupPrefab).GetComponent<TextPopup>();
            popup.Init(OnCompletedPopup, _lookAtTarget);
            popup.gameObject.SetActive(false);

            return popup;
        }

        void OnRelease(TextPopup popup)
        {
            popup.gameObject.SetActive(false);
        }

        void OnGet(TextPopup popup)
        {
            popup.gameObject.SetActive(true);
        }

        #endregion

        public void SpawnPopup(TextPopupParams popupParams)
        {
            _popupPooler.Get().ChangePopupParams(popupParams).Play();
        }

        void OnCompletedPopup(TextPopup popup)
        {
            _popupPooler.Release(popup);
        }
    }
}
