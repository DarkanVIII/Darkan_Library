namespace Darkan.Grid
{
    using Darkan.RuntimeTools;
    using Sirenix.OdinInspector;
    using System.Runtime.CompilerServices;
    using TMPro;
    using UnityEngine;

    [Searchable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public abstract class GridBase<TCell> : SerializedMonoBehaviour
    {
        enum Dimensions { XY, XZ }
        [InfoBox("Put this GameObject on a separate grid layer or exclude layers in the mesh collider (for raycasts)")]
        [Title("Grid Setup")]

        [SerializeField, EnumToggleButtons]
        [OnValueChanged("UpdateGridInEditor")]
        Dimensions _dimensions = Dimensions.XY;

        [SerializeField]
        [OnValueChanged("UpdateGridInEditor")]
        Vector2Int _gridSize;

        [SerializeField]
        [OnValueChanged("UpdateGridInEditor")]
        float _cellSize;

        [SerializeField]
        bool _displayGridIngame;

        [Title("Debug")]
        enum DebugCells { Disabled, ShowValues, ShowIndices }

        [SerializeField, EnumToggleButtons]
        [OnValueChanged("UpdateGridInEditor")]
        DebugCells _debugCells = DebugCells.Disabled;

        [SerializeField, AssetsOnly]
        [LabelText("Text Prefab (optional)")]
        [ShowIf("DebugCellsAreEnabled")]
        [OnValueChanged("UpdateGridInEditor")]
        TextMeshPro _textPrefab;

        bool DebugCellsAreEnabled => _debugCells is not DebugCells.Disabled;

        protected LayerMask GridLayer;
        public Vector2Int GridSize => _gridSize;
        public float CellSize => _cellSize;
        protected TCell[,] Grid => _grid;
        protected Transform Transform => _transform;

        [ShowInInspector, ReadOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed, DrawPreview = true, PreviewAlignment = PreviewAlignment.Bottom, PreviewHeight = 150, Expanded = false)]
        Mesh _gridMesh;
        Transform _transform;
        TextMeshPro[,] _textGrid;
        TCell[,] _grid;

        protected virtual void Awake()
        {
            _transform = GetComponent<Transform>();
            GridLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
            _gridMesh = GetComponent<MeshFilter>().sharedMesh;

            GetComponent<MeshCollider>().sharedMesh = _gridMesh;

            if (_gridSize.x <= 0 || _gridSize.y <= 0) return;
            if (_cellSize <= 0) return;

            BuildGrid();
        }

        protected virtual void Start()
        {
            if (_gridSize.x <= 0 || _gridSize.y <= 0) return;
            if (_cellSize <= 0) return;

            if (_displayGridIngame)
            {
                BuildGridMesh();
                CreateDebugCellText();
            }
            else
            {
                ClearTextGrid();
                if (_gridMesh != null)
                    _gridMesh.Clear();
            }
        }

        // Used by Inspector variables to update the grid on inspector values changed
        void UpdateGridInEditor()
        {
            if (_gridSize.x <= 0 || _gridSize.y <= 0) return;
            if (_cellSize <= 0) return;

            if (Application.isPlaying)
            {
                BuildGrid();

                if (_displayGridIngame)
                {
                    BuildGridMesh();
                    CreateDebugCellText();
                }
            }
            else
            {
                BuildGridMesh();
            }
        }

        /// <summary>
        /// Set the initial value of each cell.
        /// </summary>
        protected abstract TCell CellSetup(Vector2Int cellIndex);

        protected virtual void UpdateCellValue(TCell cell, TextMeshPro textMesh)
        {
            textMesh.text = cell.ToString();
        }

        public void UpdateCellValue(Vector2Int cellIndex)
        {
            if (_debugCells is DebugCells.ShowValues)
                UpdateCellValue(_grid[cellIndex.x, cellIndex.y], _textGrid[cellIndex.x, cellIndex.y]);
        }

        void BuildGrid()
        {
            _grid = new TCell[_gridSize.x, _gridSize.y];

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    TCell tile = CellSetup(new Vector2Int(x, y));
                    _grid[x, y] = tile;
                }
            }
        }

        void CreateDebugCellText()
        {
            ClearTextGrid();

            if (_debugCells is DebugCells.Disabled) return;
            if (_debugCells is DebugCells.ShowValues && !Application.isPlaying)
            {
                Debug.LogWarning("Debug Show Values only works while playing");
                return;
            }

            _textGrid = new TextMeshPro[_gridSize.x, _gridSize.y];

            Vector2Int cellIndex = Vector2Int.zero;

            for (cellIndex.y = 0; cellIndex.y < _gridSize.y; cellIndex.y++)
            {
                for (cellIndex.x = 0; cellIndex.x < _gridSize.x; cellIndex.x++)
                {
                    string debugText = string.Empty;

                    switch (_debugCells)
                    {
                        case DebugCells.ShowValues:
                            debugText = _grid[cellIndex.x, cellIndex.y].ToString();
                            break;
                        case DebugCells.ShowIndices:
                            debugText = $"[{cellIndex.x},{cellIndex.y}]";
                            break;
                    }

                    switch (_dimensions)
                    {
                        case Dimensions.XY:
                            _textGrid[cellIndex.x, cellIndex.y] = CreateWorldText(debugText,
                                GetLocalPositionByCellIndex(cellIndex) + new Vector3(_cellSize, _cellSize, 0) * 0.5f, cellIndex);
                            break;

                        case Dimensions.XZ:
                            _textGrid[cellIndex.x, cellIndex.y] = CreateWorldText(debugText,
                                GetLocalPositionByCellIndex(cellIndex) + new Vector3(_cellSize, 0, _cellSize) * 0.5f, cellIndex);
                            break;
                    }
                }
            }
        }

        void ClearTextGrid()
        {
            if (_textGrid is null) return;

            foreach (TextMeshPro txtMesh in _textGrid)
            {
                if (txtMesh != null)
                {
                    if (Application.isPlaying)
                        Destroy(txtMesh.gameObject);
                    else
                        DestroyImmediate(txtMesh.gameObject);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(Vector2Int cellIndex)
        {
            if (cellIndex.x < 0 || cellIndex.y < 0 || cellIndex.x >= _gridSize.x || cellIndex.y >= _gridSize.y)
                return false;

            return true;
        }

        void BuildGridMesh()
        {
            if (_gridMesh != null)
                _gridMesh.Clear();
            else
            {
                _gridMesh = new();
            }

            Vector3[] vertices = new Vector3[4 * _gridSize.x * _gridSize.y];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[6 * _gridSize.x * _gridSize.y];

            int indexCurrVertex = 0;
            int indexCurrTriangle = 0;

            Vector2Int currTileIndex = Vector2Int.zero;

            for (currTileIndex.y = 0; currTileIndex.y < _gridSize.y; currTileIndex.y++)
            {
                for (currTileIndex.x = 0; currTileIndex.x < _gridSize.x; currTileIndex.x++)
                {
                    Vector3 lowerLeftVertexPos = GetLocalPositionByCellIndex(currTileIndex);

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
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(0, _cellSize, 0);
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_cellSize, _cellSize, 0);
                            break;

                        case Dimensions.XZ:
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(0, 0, _cellSize);
                            vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_cellSize, 0, _cellSize);
                            break;
                    }

                    vertices[indexCurrVertex++] = lowerLeftVertexPos + new Vector3(_cellSize, 0, 0);
                }
            }

            _gridMesh.SetVertices(vertices);
            _gridMesh.SetUVs(0, uvs);
            _gridMesh.SetTriangles(triangles, 0);

            _gridMesh.Optimize();

#if !UNITY_EDITOR
            _gridMesh.UploadMeshData(true);
#endif
            GetComponent<MeshFilter>().sharedMesh = _gridMesh;
        }

        TextMeshPro CreateWorldText(string text, Vector3 worldPos, Vector2Int cellIndex)
        {
            GameObject go;

            if (_textPrefab != null)
                go = Instantiate(_textPrefab.gameObject);
            else
                go = new($"Grid_Text {cellIndex}", typeof(TextMeshPro));

            Transform transform = go.transform;
            transform.position = worldPos;
            transform.localScale *= _cellSize;

            if (_dimensions == Dimensions.XZ)
                transform.rotation = Quaternion.Euler(90, 0, 0);

            transform.SetParent(_transform, false);

            TextMeshPro textMesh = go.GetComponent<TextMeshPro>();
            textMesh.text = text;

            if (_textPrefab != null) return textMesh;

            textMesh.color = Color.white;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 3.5f;
            return textMesh;
        }

        internal Vector3 GetLocalPositionByCellIndex(Vector2Int cellIndex)
        {
            Vector3 worldPos = Vector3.zero;

            switch (_dimensions)
            {
                case Dimensions.XY:
                    worldPos = new Vector3(cellIndex.x, cellIndex.y, 0) * _cellSize;
                    break;

                case Dimensions.XZ:
                    worldPos = new Vector3(cellIndex.x, 0, cellIndex.y) * _cellSize;
                    break;
            }
            return worldPos;
        }

        public bool GetWorldPositionByCellIndex(Vector2Int cellIndex, out Vector3 worldPos, bool middleOfCell = false, bool checkIfInBounds = true)
        {
            worldPos = default;

            if (checkIfInBounds && !InBounds(cellIndex))
            {
                return false;
            }

            switch (_dimensions)
            {
                case Dimensions.XY:
                    worldPos = new Vector3(cellIndex.x, cellIndex.y, 0) * _cellSize + _transform.position;
                    if (middleOfCell) worldPos += new Vector3(_cellSize * .5f, _cellSize * .5f);
                    break;
                case Dimensions.XZ:
                    worldPos = new Vector3(cellIndex.x, 0, cellIndex.y) * _cellSize + _transform.position;
                    if (middleOfCell) worldPos += new Vector3(_cellSize * .5f, 0, _cellSize * .5f);
                    break;
            }

            return true;
        }

        public bool GetCellIndex(Vector3 worldPos, out Vector2Int cellIndex)
        {
            cellIndex = default;

            cellIndex.x = Mathf.FloorToInt((worldPos.x - _transform.position.x) / _cellSize);

            if (cellIndex.x < 0 || cellIndex.x >= _gridSize.x)
            {
                cellIndex.y = default;
                return false;
            }

            switch (_dimensions)
            {
                case Dimensions.XY:
                    cellIndex.y = Mathf.FloorToInt((worldPos.y - _transform.position.y) / _cellSize);
                    break;

                case Dimensions.XZ:
                    cellIndex.y = Mathf.FloorToInt((worldPos.z - _transform.position.z) / _cellSize);
                    break;
            }

            if (cellIndex.y < 0 || cellIndex.y >= _gridSize.y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to set the cell at the given index. Only needed for struct cells.
        /// </summary>
        public void SetCell(Vector2Int cellIndex, TCell cell)
        {
            if (!InBounds(cellIndex)) return;

            _grid[cellIndex.x, cellIndex.y] = cell;

            if (_debugCells == DebugCells.ShowValues)
                UpdateCellValue(_grid[cellIndex.x, cellIndex.y], _textGrid[cellIndex.x, cellIndex.y]);
        }

        /// <summary>
        /// Tries to set the cell at the given world position. Only needed for struct cells.
        /// </summary>
        public void SetCell(Vector3 worldPosition, TCell cell)
        {
            if (GetCellIndex(worldPosition, out Vector2Int cellIndex))
                SetCell(cellIndex, cell);
        }

        public bool GetCell(Vector2Int cellIndex, out TCell cell)
        {
            if (InBounds(cellIndex))
            {
                cell = _grid[cellIndex.x, cellIndex.y];
                return true;
            }
            else
            {
                cell = default;
                return false;
            }
        }

        public bool GetCell(Vector3 worldPos, out TCell cell)
        {
            if (GetCellIndex(worldPos, out Vector2Int cellIndex))
            {
                cell = _grid[cellIndex.x, cellIndex.y];
                return true;
            }
            else
            {
                cell = default;
                return false;
            }
        }

        /// <summary>
        /// Does a raycast with the screen mouse position and returns the cell index if a cell was hit
        /// </summary>
        public bool GetCellIndexWithMousePosition(out Vector2Int cellIndex)
        {
            if (RaycastMousePositionOnGrid(out RaycastHit hit))
            {
                if (GetCellIndex(hit.point, out cellIndex))
                {
                    return true;
                }
            }

            cellIndex = default;
            return false;
        }

        /// <summary>
        /// Does a raycast with the screen mouse position for the grid layer
        /// </summary>
        /// <returns>The hit</returns>
        public bool RaycastMousePositionOnGrid(out RaycastHit hit)
        {
            Ray ray = GameHelper.MainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GridLayer))
                return true;
            else
                return false;
        }
    }
}