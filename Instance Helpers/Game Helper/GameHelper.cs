namespace Darkan.GameHelper
{
    using Darkan.ObjectPooling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameHelper : MonoBehaviour
    {
#if !RELEASE
        public static GameHelper I { get; private set; }
        public static Camera MainCamera { get; private set; }
#else
        public static GameHelper I;
        public static Camera MainCamera;
#endif

        void Awake()
        {
            DisableLoggingForBuilds();

            I = this;
            MainCamera = Camera.main;

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

            if (_useUIHelpers)
            {
                UpdateEventSystem();
                UpdateGraphicRaycasters();
            }

            _popupPool = new(_textPopupPrefab.GetComponent<TextPopup>());
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        void Update()
        {
            if (_calculateAvgFrameRate)
                CalculateAvgFrameRate();
        }

        void SceneManager_activeSceneChanged(Scene current, Scene next)
        {
            if (_useUIHelpers)
            {
                UpdateEventSystem();
                UpdateGraphicRaycasters();
            }

            MainCamera = Camera.main;
        }

        #region Fps Calculator

        [SerializeField] bool _calculateAvgFrameRate;
        [SerializeField] float _fpsUpdateRate = .5f;

        public static event Action<int> OnUpdateFPS;
        float _fpsTimer;
        readonly List<float> _cachedFrameRates = new(30);

        void CalculateAvgFrameRate()
        {
            _fpsTimer += Time.deltaTime;
            _cachedFrameRates.Add(1 / Time.deltaTime);

            if (_fpsTimer >= _fpsUpdateRate)
            {
                float averageFps = 0;
                int count = 0;
                foreach (float fps in _cachedFrameRates)
                {
                    count++;
                    averageFps += fps;
                }
                averageFps /= count;
                _cachedFrameRates.Clear();

                OnUpdateFPS?.Invoke((int)averageFps);
                _fpsTimer = 0;
            }
        }

        #endregion

        #region WaitForSeconds Dictionary

        static readonly Dictionary<float, WaitForSeconds> WaitDict = new();

        /// <summary>
        /// Caches each WaitForSeconds to reduce garbage allocation.<br/>
        /// Only use when using the same or very limited time values with each call.
        /// </summary>
        /// <returns>Cached WaitForSeconds with the specified time</returns>
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (WaitDict.TryGetValue(seconds, out var wait)) return wait;

            WaitForSeconds waitFor = new(seconds);
            WaitDict[seconds] = waitFor;
            return waitFor;
        }

        #endregion

        #region UI Helpers

        [SerializeField] bool _useUIHelpers;

        static List<GraphicRaycaster> _graphicRaycasters;
        static PointerEventData _pointerEventData;
        static EventSystem _eventSystem;
        static List<RaycastResult> _results = new();

        /// <summary>
        /// Checks from the current screen position if any graphicRaycaster has a hit.
        /// GraphicRaycasters get updated OnEnable. Use <see cref="UpdateGraphicRaycasters"/> to do it manually.
        /// </summary>
        /// <returns>True if anything was hit</returns>
        public static bool IsOverUI()
        {
            _pointerEventData.position = _eventSystem.currentInputModule.input.mousePosition;
            _results.Clear();

            foreach (var rayCaster in _graphicRaycasters)
            {
                rayCaster.Raycast(_pointerEventData, _results);
                if (_results.Count > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Uses the provided raycaster to do a raycast on the current mouse position and checks for any hits.
        /// </summary>
        /// <returns>True if anything was hit</returns>
        public static bool IsOverSpecificUI(GraphicRaycaster raycaster)
        {
            _pointerEventData.position = _eventSystem.currentInputModule.input.mousePosition;
            _results.Clear();

            raycaster.Raycast(_pointerEventData, _results);
            if (_results.Count > 0) return true;
            return false;
        }

        /// <summary>
        /// Enable or disable all graphic raycasters.
        /// GraphicRaycasters get updated OnEnable. Use <see cref="UpdateGraphicRaycasters"/> to do it manually.
        /// </summary>
        public static void SetUIInteractionActive(bool b)
        {
            foreach (var raycaster in _graphicRaycasters)
            {
                raycaster.enabled = b;
            }
        }

        /// <summary>
        /// Finds all objects of type GraphicRaycaster and caches them.
        /// </summary>
        public static void UpdateGraphicRaycasters()
        {
            _graphicRaycasters = FindObjectsByType<GraphicRaycaster>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        }

        public static void UpdateEventSystem()
        {
            _eventSystem = EventSystem.current;
            _pointerEventData = new(_eventSystem);
        }

        #endregion

        #region Text Popup

        [SerializeField] Transform _textPopupPrefab;
        SimpleObjectPool<TextPopup> _popupPool;

        /// <summary>
        /// Uses Object Pooling and has no allocation, except when changing params a lot
        /// </summary>
        public void SpawnTextPopup(string text, Vector3 worldPosition, Color color, int fontSize, float distance = 1, float duration = 1, float fadeTime = .35f)
        {
            TextPopup textPopup = _popupPool.Take();

            textPopup.transform.position = worldPosition;
            textPopup.PlayPopup(text, color, fontSize, distance, duration, fadeTime);
        }

        #endregion

        void DisableLoggingForBuilds()
        {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
        }
    }
}
