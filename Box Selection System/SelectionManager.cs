namespace Darkan.Selection
{
    using Darkan.RuntimeTools;
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public class SelectionManager : MonoBehaviour
    {
        enum RaycastOptions { Only2D, Only3D, Both }

        [SerializeField] RaycastOptions _raycastOptions = RaycastOptions.Both;
        [Title("Performance Options")]
        [Tooltip("Improve performance by only checking the relevant layers.")]
        [SerializeField] LayerMask _layersToCheck;
        [Tooltip("To remove allocation, set the max amount of hits that can be detected each cast. Only needed for 3D")]
        [SerializeField] int _maxHits3D = 100;

        SpriteRenderer _spriteRenderer;
        Vector3 _startPointLocal;
        Vector3 _endPointLocal;
        Vector3 _startPointWorld;
        Vector3 _endPointWorld;
        Vector3 _startMousePos;
        bool _isDragging;
        RaycastHit[] _raycastHits3D;
        readonly List<RaycastHit2D> _raycastHits2D = new();
        readonly List<ISelectable> _currSelection = new(32);
        readonly Vector2 POINT_VECTOR = new(.01f, .01f);
        Transform _transform;
        ContactFilter2D _contactFilter2D;

#if UNITY_EDITOR // Used by Gizmos
        [Title("Gizmos")]
        [SerializeField] Color _gizmoColor = Color.yellow;
        [Tooltip("How many boxes to draw in the Scene View.")]
        [Min(0)][SerializeField] int _gizmoBoxes = 10;

        Vector3 _wireBoxCenter;
        Vector3 _wireBoxExtents;
        Quaternion _wireBoxRot = Quaternion.identity;
#endif

        void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _transform = GetComponent<Transform>();

            _raycastHits3D = new RaycastHit[_maxHits3D];

            _contactFilter2D = new()
            {
                useLayerMask = true,
                useTriggers = true,
                layerMask = _layersToCheck
            };
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    ClearCurrSelection();
                }

                GetStartScreenPoint();
                GetStartPoints();

                _isDragging = true;
                _spriteRenderer.enabled = true;
            }

            if (_isDragging)
            {
                GetEndPoints();
                DrawBoxSprite();
            }

            if (Input.GetMouseButtonUp(0))
            {
                GetStartPoints();
                CastSelectionBox();

                _isDragging = false;
                _spriteRenderer.enabled = false;
            }
        }

        void GetStartPoints()
        {
            _startPointWorld = GameHelper.MainCamera.ScreenToWorldPoint(_startMousePos);
            _startPointLocal = _transform.InverseTransformPoint(_startPointWorld);
        }

        void ClearCurrSelection()
        {
            foreach (ISelectable selector in _currSelection)
            {
                selector.Deselect();
            }
            _currSelection.Clear();
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

            if (_raycastOptions is not RaycastOptions.Only2D)
            {
                int collisions3D = 0;

                collisions3D = Physics.BoxCastNonAlloc(boxCenter, halfExtents, direction, _raycastHits3D,
                    orientation, Mathf.Infinity, _layersToCheck, QueryTriggerInteraction.Collide);

                SelectAllHits3D(collisions3D);
            }

            if (_raycastOptions is not RaycastOptions.Only3D)
            {
                int collisions2D = 0;

                if (halfExtents == Vector3.zero) //Allows single clicking instead of dragging
                    halfExtents = POINT_VECTOR;

                collisions2D = Physics2D.BoxCast(boxCenter, halfExtents * 2, 0, Vector2.zero, _contactFilter2D, _raycastHits2D);

                SelectAllHits2D(collisions2D);
            }
        }

        void SelectAllHits3D(int collisions3D)
        {

            if (_raycastOptions is not RaycastOptions.Only2D)
            {
                int currCollision = 0;
                foreach (RaycastHit hit in _raycastHits3D)
                {
                    currCollision++;
                    if (currCollision > collisions3D) break;

                    if (hit.transform.TryGetComponent(out ISelectable selector))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) && _currSelection.Contains(selector))
                        {
                            selector.Deselect();
                            _currSelection.Remove(selector);
                        }
                        else
                        {
                            selector.Select();
                            _currSelection.Add(selector);
                        }
                    }
                }
            }
        }

        void SelectAllHits2D(int collisions)
        {
            int currCollision = 0;
            foreach (RaycastHit2D hit in _raycastHits2D)
            {
                currCollision++;
                if (currCollision > collisions) break;

                if (hit.transform.TryGetComponent(out ISelectable selector))
                {
                    if (Input.GetKey(KeyCode.LeftControl) && _currSelection.Contains(selector))
                    {
                        selector.Deselect();
                        _currSelection.Remove(selector);
                    }
                    else
                    {
                        selector.Select();
                        _currSelection.Add(selector);
                    }
                }
            }
        }

        void GetStartScreenPoint()
        {
            _startMousePos = Input.mousePosition;
        }

        void GetEndPoints()
        {
            _endPointWorld = GameHelper.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            _endPointLocal = _transform.InverseTransformPoint(_endPointWorld);
        }

        void DrawBoxSprite()
        {
            Vector3 boxCenter = (_startPointLocal + _endPointLocal) / 2;
            boxCenter.z = 1;

            Vector2 size;
            size.x = Mathf.Abs(_endPointLocal.x - _startPointLocal.x);
            size.y = Mathf.Abs(_endPointLocal.y - _startPointLocal.y);

            _spriteRenderer.transform.localPosition = boxCenter;
            _spriteRenderer.size = size;
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
                    Gizmos.DrawWireCube(Vector3.zero + i * 5 * Vector3.forward, _wireBoxExtents);
                }
            }
        }
#endif
    }
}