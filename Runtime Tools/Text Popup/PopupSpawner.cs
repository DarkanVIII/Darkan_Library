namespace Darkan.RuntimeTools
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public class PopupSpawner : SerializedMonoBehaviour
    {
        [SerializeField]
        [Required]
        Transform _textPopupPrefab;
        [SerializeField]
        [Tooltip("Uses this transform if left empty")]
        Transform _origin;

        readonly Queue<TextPopupParams> _queuedParams = new();

        int _popupsToSpawn;
        TextPopup _cachedPopup;

        void Awake()
        {
            if (_origin == null) _origin = transform;

            CreatePopup();
        }

        /// <summary>
        ///Has little to no allocation, except when changing params a lot
        /// </summary>
        public void SpawnPopup(TextPopupParams popupParams)
        {
            _popupsToSpawn++;
            _queuedParams.Enqueue(popupParams);

            if (_popupsToSpawn == 1)
                PlayNextPopup();
        }

        void CreatePopup()
        {
            _cachedPopup = Instantiate(_textPopupPrefab).GetComponent<TextPopup>();
            _cachedPopup.Init(OnCompletedPopup, _origin);
            _cachedPopup.gameObject.SetActive(false);
        }

        void OnCompletedPopup()
        {
            _popupsToSpawn--;

            if (_popupsToSpawn > 0)
                PlayNextPopup();
            else
                _cachedPopup.gameObject.SetActive(false);
        }

        void PlayNextPopup()
        {
            _cachedPopup.gameObject.SetActive(true);
            _cachedPopup.ChangePopupParams(_queuedParams.Dequeue());
            _cachedPopup.Play();
        }
    }
}
