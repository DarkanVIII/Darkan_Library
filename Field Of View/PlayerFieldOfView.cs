namespace Darkan.FieldOfView
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfView : SerializedMonoBehaviour
    {
        [SerializeField]
        [Tooltip("Layers to check for obstacles")]
        LayerMask _obstacleMask;

        [SerializeField]
        [Range(0f, 360f)]
        float _fov = 360;

        [SerializeField]
        [Range(3, 5000)]
        int _rayCount;

        [SerializeField]
        [Range(0f, 20f)]
        float _viewDistance;

        Mesh _mesh;
        new Transform transform;

        void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            transform = GetComponent<Transform>();
        }

        void Update()
        {
            _mesh.Clear();

            float angle = _fov * 0.5f + 90;
            float angleIncrease = _fov / _rayCount;

            Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[_rayCount * 3];

            vertices[0] = Vector3.zero;

            int vertexIndex = 1;
            int triangleIndex = 0;

            for (int currRay = 0; currRay <= _rayCount; currRay++)
            {
                Vector3 vertex;

                RaycastHit2D rayCastHit = Physics2D.Raycast(transform.position, GetPositionFromAngle(angle), _viewDistance, _obstacleMask);

                if (rayCastHit.collider == null)
                    vertex = GetPositionFromAngle(angle) * _viewDistance;
                else
                    vertex = (Vector3)rayCastHit.point;

                vertices[vertexIndex] = vertex;

                if (currRay > 0)
                {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;

                    triangleIndex += 3;
                }
                vertexIndex++;
                angle -= angleIncrease;
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;

            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
            _mesh.MarkDynamic();
            _mesh.Optimize();
            _mesh.UploadMeshData(false);
        }

        public void SetParams(LayerMask obstacleMask, float fov, int rayCount, float viewDistance)
        {
            _obstacleMask = obstacleMask;
            _fov = fov;
            _rayCount = rayCount;
            _viewDistance = viewDistance;
        }

        Vector3 GetPositionFromAngle(float angle)
        {
            float angleRad = angle * Mathf.PI / 180f;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
    }
}
