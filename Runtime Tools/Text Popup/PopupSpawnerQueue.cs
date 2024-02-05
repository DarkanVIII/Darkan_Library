namespace Darkan.RuntimeTools
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public class PopupSpawnerQueue : MonoBehaviour
    {
        [SerializeField]
        [Required]
        Transform _textPopupPrefab;

        [SerializeField]
        [Tooltip("Uses this transform if left empty")]
        Transform _origin;

        [SerializeField]
        [Tooltip("Optional")]
        Transform _lookAtTarget;

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
            _cachedPopup.Init(OnCompletedPopup, _origin, _lookAtTarget);
            _cachedPopup.gameObject.SetActive(false);
        }

        void OnCompletedPopup(TextPopup popup)
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
            _cachedPopup.ChangePopupParams(_queuedParams.Dequeue()).Play();
        }
    }
}
