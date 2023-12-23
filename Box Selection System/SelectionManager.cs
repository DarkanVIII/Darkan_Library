namespace Darkan.Selection
{
    using Darkan.RuntimeTools;
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class SelectionManager : MonoBehaviour
    {
        #region Inspector

        enum RaycastOptions { Only2D, Only3D, Both }
        enum CollisionCheckOptions { CollisionObject, RigidbodyObject, Both }

        [SerializeField]
        RaycastOptions _raycastOptions = RaycastOptions.Both;

        [SerializeField]
        [Tooltip("Marks objects (IMarkable) inside the selection box while dragging.")]
        bool _useMarking;

        [Title("Performance Options")]

        [SerializeField]
        [Tooltip("Improve performance by only checking the relevant layers.")]
        LayerMask _layersToCheck;

        [SerializeField]
        [Tooltip("After collision, which objects to check for selection or marking. Using only 1 improves performance.")]
        CollisionCheckOptions _collisionCheckOptions = CollisionCheckOptions.Both;

        [SerializeField]
        [Tooltip("To remove allocation, set the max amount of hits that can be detected each cast. Only needed for 3D")]
        int _maxHits3D = 100;

        #endregion

        #region Private Variables

        const float TIME_BEFORE_DRAG = .1f;

        Vector3 _startPointLocal;
        Vector3 _endPointLocal;
        Vector3 _startPointWorld;
        Vector3 _endPointWorld;
        Vector3 _startMousePos;
        Vector3 _lastCamPos;

        SpriteRenderer _boxRenderer;
        Transform _transform;
        bool _isDragging;
        private bool _buttonPressed;
        float _currDragTime;

        RaycastHit[] _raycastHits3D;
        readonly List<RaycastHit2D> _raycastHits2D = new();
        readonly List<ISelectable> _activeSelection = new(32);
        readonly List<IMarkable> _activeMarks = new(32);

        // Needed to compare new hits with already marked objects
        readonly List<IMarkable> _lastMarks = new(32);
        readonly List<int> _activeMarksColliderID = new(32);
        readonly List<int> _lastMarksColliderID = new(32);

        readonly Vector2 POINT_VECTOR = new(.01f, .01f);
        ContactFilter2D _contactFilter2D;

#if UNITY_EDITOR // Used by Gizmos

        [Title("Gizmos")]

        [SerializeField]
        Color _gizmoColor = Color.yellow;

        [SerializeField]
        [Tooltip("How many boxes to draw in the Scene View.")]
        int _gizmoBoxes = 10;

        [SerializeField]
        [Tooltip("Distance between gizmo boxes.")]
        int _boxDistance = 5;


        Vector3 _wireBoxCenter;
        Vector3 _wireBoxExtents;
        Quaternion _wireBoxRot = Quaternion.identity;

#endif

        #endregion

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _boxRenderer = _transform.GetChild(0).GetComponent<SpriteRenderer>();

            _raycastHits3D = new RaycastHit[_maxHits3D];

            _contactFilter2D = new()
            {
                useLayerMask = true,
                useTriggers = true,
                layerMask = _layersToCheck
            };
        }

        void Start()
        {
            _lastCamPos = GameHelper.MainCamera.transform.position;
        }

        void Update()
        {
            if (_buttonPressed)
            {
                _currDragTime -= Time.deltaTime;

                if (_currDragTime <= 0)
                {
                    _isDragging = true;
                }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    ClearCurrSelection();
                }

                GetStartScreenPosition();
                GetStartPositions();

                _currDragTime = TIME_BEFORE_DRAG;
                _buttonPressed = true;
                _boxRenderer.enabled = true;
            }

            if (_buttonPressed)
            {
                GetEndPositions();
                DrawBoxSprite();
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (_isDragging)
                {
                    ClearActiveMarks();
                    UpdateStartPoint();
                    CastSelectionBox();

                    _isDragging = false;
                    _boxRenderer.enabled = false;
                }
                else
                {
                    SingleRaycast();
                }

                _buttonPressed = false;
            }
        }

        void FixedUpdate()
        {
            if (!_useMarking) return;

            if (_isDragging)
            {
                UpdateStartPoint();
                MoveActiveMarksToLastMarks();
                CastMarkingBox();
                ClearLastMarks();
            }
        }

        void CastMarkingBox()
        {
            Vector3 boxCenter = (_startPointWorld + _endPointWorld) / 2;

            Vector3 halfExtents;
            halfExtents.x = Mathf.Abs(_endPointLocal.x - _startPointLocal.x) / 2;
            halfExtents.y = Mathf.Abs(_endPointLocal.y - _startPointLocal.y) / 2;
            halfExtents.z = Mathf.Abs(_endPointLocal.z - _startPointLocal.z) / 2;

            Quaternion orientation = GameHelper.MainCamera.transform.rotation;

            Vector3 direction = GameHelper.MainCamera.transform.forward;

#if UNITY_EDITOR // Used by Gizmos

            _wireBoxCenter = boxCenter;
            _wireBoxExtents = halfExtents * 2;
            _wireBoxRot = orientation;

#endif

            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only3D)
            {
                int hits;

                hits = Physics.BoxCastNonAlloc(boxCenter, halfExtents, direction, _raycastHits3D,
                    orientation, Mathf.Infinity, _layersToCheck, QueryTriggerInteraction.Collide);

                MarkAllHits3D(hits);
            }

            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only2D)
            {
                int hits;

                hits = Physics2D.BoxCast(boxCenter, halfExtents * 2, 0, Vector2.zero, _contactFilter2D, _raycastHits2D);

                MarkAllHits2D(hits);
            }
        }

        void MoveActiveMarksToLastMarks()
        {
            foreach (IMarkable marker in _activeMarks)
            {
                _lastMarks.Add(marker);
            }

            _activeMarks.Clear();

            foreach (int id in _activeMarksColliderID)
            {
                _lastMarksColliderID.Add(id);
            }

            _activeMarksColliderID.Clear();
        }

        /// <summary>
        /// Updates the start point in local and world space (needed when the camera moves)
        /// </summary>
        void UpdateStartPoint()
        {
            Vector3 currCamPos = GameHelper.MainCamera.transform.position;

            if (_lastCamPos == currCamPos)
                return;
            else
                _lastCamPos = currCamPos;

            _startPointWorld = GameHelper.MainCamera.ScreenToWorldPoint(_startMousePos);
            _startPointLocal = _transform.InverseTransformPoint(_startPointWorld);
        }

        void GetStartPositions()
        {
            _startPointWorld = GameHelper.MainCamera.ScreenToWorldPoint(_startMousePos);
            _startPointLocal = _transform.InverseTransformPoint(_startPointWorld);
        }

        void ClearCurrSelection()
        {
            foreach (ISelectable selector in _activeSelection)
            {
                selector.Deselect();
            }

            _activeSelection.Clear();
        }

        void ClearLastMarks()
        {
            foreach (IMarkable mark in _lastMarks)
            {
                mark.Unmark();
            }

            _lastMarks.Clear();
            _lastMarksColliderID.Clear();
        }

        void ClearActiveMarks()
        {
            foreach (IMarkable mark in _activeMarks)
            {
                mark.Unmark();
            }

            _activeMarks.Clear();
            _activeMarksColliderID.Clear();
        }

        void SingleRaycast()
        {
            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only3D)
            {
                Ray ray = GameHelper.MainCamera.ScreenPointToRay(Mouse.current.position.value);

                int hits = Physics.RaycastNonAlloc(ray, _raycastHits3D, Mathf.Infinity, _layersToCheck, QueryTriggerInteraction.Collide);

                SelectAllHits3D(hits);
            }

            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only2D)
            {
                Ray ray = GameHelper.MainCamera.ScreenPointToRay(Mouse.current.position.value);

                int hits = Physics2D.Raycast(ray.origin, ray.direction, _contactFilter2D, _raycastHits2D);

                SelectAllHits2D(hits);
            }
        }

        void CastSelectionBox()
        {
            Vector3 boxCenter = (_startPointWorld + _endPointWorld) / 2;

            Vector3 halfExtents;
            halfExtents.x = Mathf.Abs(_endPointLocal.x - _startPointLocal.x) / 2;
            halfExtents.y = Mathf.Abs(_endPointLocal.y - _startPointLocal.y) / 2;
            halfExtents.z = Mathf.Abs(_endPointLocal.z - _startPointLocal.z) / 2;

            Quaternion orientation = GameHelper.MainCamera.transform.rotation;

            Vector3 direction = GameHelper.MainCamera.transform.forward;

#if UNITY_EDITOR // Used by Gizmos

            _wireBoxCenter = boxCenter;
            _wireBoxExtents = halfExtents * 2;
            _wireBoxRot = orientation;

#endif

            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only3D)
            {
                int hits = Physics.BoxCastNonAlloc(boxCenter, halfExtents, direction, _raycastHits3D,
                    orientation, Mathf.Infinity, _layersToCheck, QueryTriggerInteraction.Collide);

                SelectAllHits3D(hits);
            }

            if (_raycastOptions is RaycastOptions.Both || _raycastOptions is RaycastOptions.Only2D)
            {
                int hits = Physics2D.BoxCast(boxCenter, halfExtents * 2, 0, Vector2.zero, _contactFilter2D, _raycastHits2D);

                SelectAllHits2D(hits);
            }
        }

        void SelectAllHits3D(int hits)
        {
            int currHit = 0;

            foreach (RaycastHit hit in _raycastHits3D)
            {
                currHit++;
                if (currHit > hits) break;

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.CollisionObject)
                {
                    if (hit.collider.TryGetComponent(out ISelectable selector))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeSelection.Contains(selector))
                        {
                            selector.Deselect();
                            _activeSelection.Remove(selector);
                        }
                        else
                        {
                            selector.Select();
                            _activeSelection.Add(selector);
                        }
                        continue;
                    }
                }

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.RigidbodyObject)
                {
                    if (hit.rigidbody == null) continue;

                    if (hit.rigidbody.TryGetComponent(out ISelectable selector))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeSelection.Contains(selector))
                        {
                            selector.Deselect();
                            _activeSelection.Remove(selector);
                        }
                        else
                        {
                            selector.Select();
                            _activeSelection.Add(selector);
                        }
                    }
                }
            }
        }

        void SelectAllHits2D(int hits)
        {
            int currHit = 0;

            foreach (RaycastHit2D hit in _raycastHits2D)
            {
                currHit++;
                if (currHit > hits) break;

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.CollisionObject)
                {
                    if (hit.collider.TryGetComponent(out ISelectable selector))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeSelection.Contains(selector))
                        {
                            selector.Deselect();
                            _activeSelection.Remove(selector);
                        }
                        else
                        {
                            selector.Select();
                            _activeSelection.Add(selector);
                        }
                        continue;
                    }
                }

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.RigidbodyObject)
                {
                    if (hit.rigidbody == null) continue;

                    if (hit.rigidbody.TryGetComponent(out ISelectable selector))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeSelection.Contains(selector))
                        {
                            selector.Deselect();
                            _activeSelection.Remove(selector);
                        }
                        else
                        {
                            selector.Select();
                            _activeSelection.Add(selector);
                        }
                    }
                }
            }
        }

        void MarkAllHits3D(int hits)
        {
            int currHit = 0;

            foreach (RaycastHit hit in _raycastHits3D)
            {
                currHit++;
                if (currHit > hits) break;

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.CollisionObject)
                {
                    if (hit.collider.TryGetComponent(out IMarkable marker))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeMarks.Contains(marker))
                        {
                            marker.Unmark();
                            _activeMarks.Remove(marker);
                        }
                        else
                        {
                            if (_lastMarksColliderID.Contains(hit.colliderInstanceID)) //If target was already marked, keep it marked
                            {
                                _lastMarks.Remove(marker);

                                _activeMarks.Add(marker);
                                _activeMarksColliderID.Add(hit.colliderInstanceID);
                            }
                            else
                            {
                                marker.Mark();
                                _activeMarks.Add(marker);
                                _activeMarksColliderID.Add(hit.colliderInstanceID);
                            }
                        }
                        continue;
                    }
                }

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.RigidbodyObject)
                {
                    if (hit.rigidbody == null) continue;

                    if (hit.rigidbody.TryGetComponent(out IMarkable marker))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _activeMarks.Contains(marker))
                        {
                            marker.Unmark();
                            _activeMarks.Remove(marker);
                        }
                        else
                        {
                            if (_lastMarksColliderID.Contains(hit.colliderInstanceID)) //If target was already marked, keep it marked
                            {
                                _lastMarks.Remove(marker);

                                _activeMarks.Add(marker);
                                _activeMarksColliderID.Add(hit.colliderInstanceID);
                            }
                            else
                            {
                                marker.Mark();
                                _activeMarks.Add(marker);
                                _activeMarksColliderID.Add(hit.colliderInstanceID);
                            }
                        }
                    }
                }
            }
        }

        void MarkAllHits2D(int hits)
        {
            int currHit = 0;

            foreach (RaycastHit2D hit in _raycastHits2D)
            {
                currHit++;
                if (currHit > hits) break;

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.CollisionObject)
                {
                    if (hit.collider.TryGetComponent(out IMarkable marker))
                    {
                        int colliderID = hit.collider.GetInstanceID();

                        if (_lastMarksColliderID.Contains(colliderID)) //If target was already marked, keep it marked
                        {
                            _lastMarks.Remove(marker);

                            _activeMarks.Add(marker);
                            _activeMarksColliderID.Add(colliderID);
                        }
                        else
                        {
                            marker.Mark();
                            _activeMarks.Add(marker);
                            _activeMarksColliderID.Add(colliderID);
                        }

                        continue;
                    }
                }

                if (_collisionCheckOptions is CollisionCheckOptions.Both || _collisionCheckOptions is CollisionCheckOptions.RigidbodyObject)
                {
                    if (hit.rigidbody == null) continue;

                    if (hit.rigidbody.TryGetComponent(out IMarkable marker))
                    {
                        int colliderID = hit.collider.GetInstanceID();

                        if (_lastMarksColliderID.Contains(colliderID)) //If target was already marked, keep it marked
                        {
                            _lastMarks.Remove(marker);

                            _activeMarks.Add(marker);
                            _activeMarksColliderID.Add(colliderID);
                        }
                        else
                        {
                            marker.Mark();
                            _activeMarks.Add(marker);
                            _activeMarksColliderID.Add(colliderID);
                        }
                    }
                }
            }
        }

        void GetStartScreenPosition()
        {
            _startMousePos = Mouse.current.position.value;
        }

        void GetEndPositions()
        {
            _endPointWorld = GameHelper.MainCamera.ScreenToWorldPoint(Mouse.current.position.value);
            _endPointLocal = _transform.InverseTransformPoint(_endPointWorld);
        }

        void DrawBoxSprite()
        {
            Vector3 boxCenter = (_startPointLocal + _endPointLocal) / 2;
            boxCenter.z = 1;

            Vector2 size;
            size.x = Mathf.Abs(_endPointLocal.x - _startPointLocal.x);
            size.y = Mathf.Abs(_endPointLocal.y - _startPointLocal.y);

            _boxRenderer.transform.localPosition = boxCenter;
            _boxRenderer.size = size;
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = _gizmoColor;
                Gizmos.matrix = Matrix4x4.TRS(_wireBoxCenter, _wireBoxRot, Vector3.one);

                Gizmos.DrawLine(Vector3.zero, Vector3.forward * 100);

                for (int i = 0; i < _gizmoBoxes; i++)
                {
                    Gizmos.DrawWireCube(Vector3.zero + i * _boxDistance * Vector3.forward, _wireBoxExtents);
                }
            }
        }

#endif

    }
}