using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHelper : MonoBehaviour
{
#if UNITY_EDITOR
    public static Camera MainCamera { get; private set; }
#else
    public static Camera MainCamera;
#endif

    void Awake()
    {
        DisableLoggingForBuilds();

        MainCamera = Camera.main;

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
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

    void Update()
    {
        if (_calculateAvgFrameRate)
            CalculateAvgFrameRate();
    }


    #region Fps Calculator

    [SerializeField] bool _calculateAvgFrameRate;
    [SerializeField] float _fpsUpdateRate = 0.5f;

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
    /// Caches each WaitForSeconds to reduce garbage allocation. Only useful when using the same time value with each call.
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
        _pointerEventData.position = Mouse.current.position.value;
        _results.Clear();

        foreach (var rayCaster in _graphicRaycasters)
        {
            rayCaster.Raycast(_pointerEventData, _results);
            if (_results.Count > 0) return true;
        }
        return false;
    }

    /// <summary>
    /// Uses the provided raycaster to do a raycast with the current screen position and checks for any hits.
    /// </summary>
    /// <returns>True if anything was hit</returns>
    public static bool IsOverSpecificUI(GraphicRaycaster raycaster)
    {
        _pointerEventData.position = Mouse.current.position.value;
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

    void DisableLoggingForBuilds()
    {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }
}
