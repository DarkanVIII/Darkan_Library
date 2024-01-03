namespace Darkan.Grid
{
    using Darkan.RuntimeTools;
    using Sirenix.OdinInspector;
    using System.Runtime.CompilerServices;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider)), Searchable]
    [InfoBox("It is recommended to put this GameObject on it's own layer (for raycasts)")]
    public abstract class GridBase<T> : SerializedMonoBehaviour
    {
        enum Dimensions { XY, XZ }

        [Title("Grid Setup")]

        [SerializeField, EnumToggleButtons]
        Dimensions _dimensions = Dimensions.XY;

        [SerializeField]
        Vector2Int _gridSize;

        [SerializeField]
        float _tileSize;

        [SerializeField]
        Vector3 _origin;

        protected LayerMask GridLayer;

        public Vector2Int GridSize => _gridSize;
        public float TileSize => _tileSize;
        public Vector3 Origin => _origin;

#if UNITY_EDITOR
        [Title("Debug")]
        enum DebugTiles { Disabled, ShowValues, ShowIndices }

        [SerializeField, EnumToggleButtons]
        DebugTiles _debugTiles = DebugTiles.Disabled;

        [SerializeField, AssetsOnly]
        [LabelText("Text Prefab (optional)")]
        [HideIf("DebugTilesAreDisabled")]
        TextMeshPro _textPrefab;

        bool DebugTilesAreDisabled => _debugTiles is DebugTiles.Disabled;

        TextMeshPro[,] _textGrid;
#endif
        [ShowInInspector, ReadOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed, DrawPreview = true, PreviewAlignment = PreviewAlignment.Bottom, PreviewHeight = 150, Expanded = false)]
        Mesh _gridMesh;

        T[,] _grid;

        protected virtual void Awake()
        {
            GridLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
            _gridMesh = GetComponent<MeshFilter>().mesh;
        }

        protected virtual void Start()
        {
            BuildGrid();
        }

        protected abstract T SetInitialValue();

#if UNITY_EDITOR
        protected virtual void UpdateTileValue(T tile, TextMeshPro textMesh)
        {
            textMesh.text = tile.ToString();
        }

        public void UpdateTileValue(Vector2Int tileIndex)
        {
            if (_debugTiles is DebugTiles.ShowValues)
                UpdateTileValue(_grid[tileIndex.x, tileIndex.y], _textGrid[tileIndex.x, tileIndex.y]);
        }
#endif

        protected abstract void TileSetup(T tile, Vector2Int tileIndex);

        [Button("Rebuild Grid")]
        void BuildGrid()
        {
            if (!Application.isPlaying) return;

            if (_gridSize.x <= 0 || _gridSize.y <= 0) return;
            if (_tileSize <= 0) return;

            _grid = new T[_gridSize.x, _gridSize.y];

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    T tile = SetInitialValue();
                    _grid[x, y] = tile;
                    TileSetup(tile, new Vector2Int(x, y));
                }
            }

            BuildAndRenderGridMesh();

#if UNITY_EDITOR
            CreateDebugTileText();
#endif
        }

#if UNITY_EDITOR // For Debugging Tile values or indices in the Editor
        void CreateDebugTileText()
        {
            if (_textGrid != null)
            {
                foreach (var txtMesh in _textGrid)
                {
                    if (txtMesh != null)
                        Destroy(txtMesh.gameObject);
                }
            }

            if (_debugTiles is DebugTiles.Disabled)
            {
                _textGrid = null;
                return;
            }

            _textGrid = new TextMeshPro[_gridSize.x, _gridSize.y];

            Vector2Int tileIndex = Vector2Int.zero;

            foreach (T tile in _grid)
            {
                if (tileIndex.y == _gridSize.y)
                {
                    tileIndex.y = 0;
                    tileIndex.x++;
                }

                string debugText = string.Empty;

                switch (_debugTiles)
                {
                    case DebugTiles.ShowValues:
                        debugText = tile.ToString();
                        break;
                    case DebugTiles.ShowIndices:
                        debugText = $"{tileIndex.x},{tileIndex.y}";
                        break;
                }

                switch (_dimensions)
                {
                    case Dimensions.XY:
                        _textGrid[tileIndex.x, tileIndex.y] = CreateWorldText(debugText,
                            GetLocalPositionByTileIndex(tileIndex) + new Vector3(_tileSize, _tileSize, 0) * 0.5f, tileIndex);
                        break;

                    case Dimensions.XZ:
                        _textGrid[tileIndex.x, tileIndex.y] = CreateWorldText(debugText,
                            GetLocalPositionByTileIndex(tileIndex) + new Vector3(_tileSize, 0, _tileSize) * 0.5f, tileIndex);
                        break;
                }

                tileIndex.y++;
            }
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(Vector2Int tileIndex)
        {
            if (tileIndex.x < 0 || tileIndex.y < 0 || tileIndex.x >= _gridSize.x || tileIndex.y >= _gridSize.y)
                return false;

            return true;
        }

        void BuildAndRenderGridMesh()
        {
            if (_gridMesh != null)
                _gridMesh.Clear();

            Vector3[] vertices = new Vector3[4 * _grid.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[6 * _grid.Length];

            int indexCurrVertex = 0;
            int indexCurrTriangle = 0;

            Vector2Int currTileIndex = Vector2Int.zero;

            for (currTileIndex.y = 0; currTileIndex.y < _grid.GetLength(1); currTileIndex.y++)
            {
                for (currTileIndex.x = 0; currTileIndex.x < _grid.GetLength(0); currTileIndex.x++)
                {
                    Vector3 lowerLeftVertexPos = GetLocalPositionByTileIndex(currTileIndex);

                    uvs[indexCurrVertex] = new(0, 0);

                    triangles[indexCurrTriangle++] = indexCurrVertex;
                    triangles[indexCurrTriangle++] = indexCurrVertex + 1;
                    triangles[indexCurrTriangle++] = indexCurrVertex + 2;
                    triangles[indexCurrTriangle++] = indexCurrVertex;
                    triangles[indexCurrTriangle++] = indexCurrVertex + 2;
                    triangles[indexCurrTriangle++] = indexCurrVertex + 3;

                    vertices[indexCurrVertex++] = lowerLeftVertexPos;

                    uvs[indexCurrVertex] = new(0, 1);
                    uvs[indexCurrVertex + 1] = new(1, 1);
                    uvs[indexCurrVertex + 2] = new(1, 0);

                    switch (_dimensions)
                    {
                        case Dimensions.XY:
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(0, _tileSize, 0);
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_tileSize, _tileSize, 0);
                            break;

                        case Dimensions.XZ:
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(0, 0, _tileSize);
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_tileSize, 0, _tileSize);
                            break;
                    }

                    vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_tileSize, 0, 0);
                }
            }

            _gridMesh.SetVertices(vertices);
            _gridMesh.SetUVs(0, uvs);
            _gridMesh.SetTriangles(triangles, 0);

            GetComponent<MeshCollider>().sharedMesh = _gridMesh;
        }

#if UNITY_EDITOR
        TextMeshPro CreateWorldText(string text, Vector3 worldPos, Vector2Int tileIndex)
        {
            GameObject go;
            if (_textPrefab != null)
                go = Instantiate(_textPrefab.gameObject);
            else
                go = new($"Grid_Text {tileIndex}", typeof(TextMeshPro));

            Transform transform = go.transform;
            transform.position = worldPos;
            transform.localScale *= _tileSize;

            if (_dimensions == Dimensions.XZ)
                transform.rotation = Quaternion.Euler(90, 0, 0);

            transform.SetParent(this.transform, false);

            TextMeshPro textMesh = go.GetComponent<TextMeshPro>();
            textMesh.text = text;

            if (_textPrefab != null) return textMesh;

            textMesh.color = Color.white;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 4;
            return textMesh;
        }
#endif  

        Vector3 GetLocalPositionByTileIndex(Vector2Int tileIndex)
        {
            Vector3 worldPos = Vector3.zero;

            switch (_dimensions)
            {
                case Dimensions.XY:
                    worldPos = new Vector3(tileIndex.x, tileIndex.y, 0) * _tileSize + _origin;
                    break;

                case Dimensions.XZ:
                    worldPos = new Vector3(tileIndex.x, 0, tileIndex.y) * _tileSize + _origin;
                    break;
            }
            return worldPos;
        }

        public bool TryGetWorldPositionByTileIndex(Vector2Int tileIndex, out Vector3 worldPos, bool middleOfTile = false)
        {
            worldPos = default;

            if (!InBounds(tileIndex))
                return false;

            switch (_dimensions)
            {
                case Dimensions.XY:
                    worldPos = new Vector3(tileIndex.x, tileIndex.y, 0) * _tileSize + _origin + transform.position;
                    if (middleOfTile) worldPos += new Vector3(_tileSize * .5f, _tileSize * .5f);
                    break;
                case Dimensions.XZ:
                    worldPos = new Vector3(tileIndex.x, 0, tileIndex.y) * _tileSize + _origin + transform.position;
                    if (middleOfTile) worldPos += new Vector3(_tileSize * .5f, 0, _tileSize * .5f);
                    break;
            }

            return true;
        }

        public bool TryGetTileIndex(Vector3 worldPos, out Vector2Int tileIndex)
        {
            tileIndex = default;

            tileIndex.x = Mathf.FloorToInt((worldPos.x - _origin.x - transform.position.x) / _tileSize);

            if (tileIndex.x < 0 || tileIndex.x >= _gridSize.x)
            {
                tileIndex.y = default;
                return false;
            }

            switch (_dimensions)
            {
                case Dimensions.XY:
                    tileIndex.y = Mathf.FloorToInt((worldPos.y - _origin.y - transform.position.y) / _tileSize);
                    break;

                case Dimensions.XZ:
                    tileIndex.y = Mathf.FloorToInt((worldPos.z - _origin.z - transform.position.z) / _tileSize);
                    break;
            }

            if (tileIndex.y < 0 || tileIndex.y >= _gridSize.y)
            {
                return false;
            }

            return true;
        }

        public void SetTileObject(Vector2Int tileIndex, T tile)
        {
            if (!InBounds(tileIndex)) return;

            _grid[tileIndex.x, tileIndex.y] = tile;

#if UNITY_EDITOR
            if (_debugTiles == DebugTiles.ShowValues)
                UpdateTileValue(_grid[tileIndex.x, tileIndex.y], _textGrid[tileIndex.x, tileIndex.y]);
#endif
        }

        public void SetTileObject(Vector3 worldPosition, T tile)
        {
            if (TryGetTileIndex(worldPosition, out Vector2Int tileIndex))
                SetTileObject(tileIndex, tile);
        }

        public bool TryGetTile(Vector2Int tileIndex, out T tile)
        {
            if (!InBounds(tileIndex))
            {
                tile = default;
                return false;
            }

            tile = _grid[tileIndex.x, tileIndex.y];
            return true;
        }

        public bool TryGetTile(Vector3 worldPos, out T tile)
        {
            if (TryGetTileIndex(worldPos, out Vector2Int tileIndex))
            {
                tile = _grid[tileIndex.x, tileIndex.y];
                return true;
            }
            else
            {
                tile = default;
                return false;
            }
        }

        public bool TryGetTileIndexByMousePosition(out Vector2Int tileIndex)
        {
            if (RaycastMousePositionOnGrid(out Vector3 mousePos))
            {
                if (TryGetTileIndex(mousePos, out Vector2Int tileObjIndex))
                {
                    tileIndex = tileObjIndex;
                    return true;
                }
            }
            tileIndex = Vector2Int.zero;
            return false;
        }

        bool RaycastMousePositionOnGrid(out Vector3 mousePos)
        {
            Ray ray = GameHelper.MainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GridLayer))
            {
                mousePos = hit.point;
                return true;
            }
            else
            {
                mousePos = Vector3.zero;
                return false;
            }
        }
    }
}