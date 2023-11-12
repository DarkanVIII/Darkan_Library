namespace Darkan.InstanceHelpers
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;

    public class PopupSpawner : MonoBehaviour
    {
        [SerializeField] Transform _textPopupPrefab;
        [SerializeField] Transform _spawnPosition;

        readonly Queue<TextPopup> _queuedPopups = new();
        ObjectPool<TextPopup> _popupPool;

        void Awake()
        {
            _popupPool = new(OnCreatePopup, OnGetPopup, OnReleasePopup, OnDestroyPopup, false, 0);
        }

        TextPopup OnCreatePopup()
        {
            TextPopup popup = Instantiate(_textPopupPrefab, transform).GetComponent<TextPopup>();
            popup.Init(_popupPool, _spawnPosition);
            popup.gameObject.SetActive(false);
            return popup;
        }
        void OnDestroyPopup(TextPopup popup)
        {
            Destroy(popup.gameObject);
        }

        void OnGetPopup(TextPopup popup)
        {
            _queuedPopups.Enqueue(popup);

            if (_queuedPopups.Count == 1)
            {
                popup.gameObject.SetActive(true);
                popup.PlayPopup();
            }
        }

        void OnReleasePopup(TextPopup popup)
        {
            popup.gameObject.SetActive(false);
            _queuedPopups.Dequeue();

            if (_queuedPopups.Count > 0)
            {
                TextPopup textPopup = _queuedPopups.Peek();
                textPopup.gameObject.SetActive(true);
                textPopup.PlayPopup();
            }
        }

        /// <summary>
        /// Uses Object Pooling and has no allocation, except when changing params a lot
        /// </summary>
        public void SpawnPopup(TextPopupParams popupParams)
        {
            _popupPool.Get().ChangePopupParams(popupParams);
        }
    }
}
