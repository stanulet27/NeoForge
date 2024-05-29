using System.Collections.Generic;
using DeformationSystem;
using UnityEngine;

namespace _InDevelopment.Chase.Scripts
{
    public class MeshSimilarityCalculator : MonoBehaviour
    {
        [SerializeField] private MeshFilter _currentMeshFilter; // Assign in the Inspector
        [SerializeField] private MeshFilter _desiredMeshFilter; // Assign in the Inspector
        [SerializeField] private float _buffer = 0.05f;
        [SerializeField] private Material _vertexColorMaterial; // Assign the VertexColorShader material
        [SerializeField] private ForgeHUD _hud; // Assign in the Inspector

        public class Triangle
        {
            public Vector3[] Vertices { get; }

            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                Vertices = new[] { v1, v2, v3 };
            }
        }

        private static Bounds GetExpandedBox(Triangle triangle, float buffer)
        {
            var minCorner = Vector3.Min(Vector3.Min(triangle.Vertices[0], triangle.Vertices[1]), triangle.Vertices[2]) - new Vector3(buffer, buffer, buffer);
            var maxCorner = Vector3.Max(Vector3.Max(triangle.Vertices[0], triangle.Vertices[1]), triangle.Vertices[2]) + new Vector3(buffer, buffer, buffer);
            return new Bounds((minCorner + maxCorner) / 2, maxCorner - minCorner);
        }

        private static List<Triangle> ConvertMeshToTriangles(Mesh mesh)
        {
            var triangles = new List<Triangle>();
            var vertices = mesh.vertices;
            var meshTriangles = mesh.triangles;

            for (var i = 0; i < meshTriangles.Length; i += 3)
            {
                triangles.Add(new Triangle(vertices[meshTriangles[i]], vertices[meshTriangles[i + 1]], vertices[meshTriangles[i + 2]]));
            }

            return triangles;
        }

        private void Start()
        {
            CalculateScore();
            DeformationHandler.OnDeformationPerformed += CalculateScore;
        }

        private void CalculateScore()
        {
            // Get the current and desired meshes
            var currentMesh = _currentMeshFilter.mesh;
            var desiredMesh = _desiredMeshFilter.mesh;

            // Convert meshes to lists of triangles and points
            var currentTriangles = ConvertMeshToTriangles(currentMesh);
            var desiredPoints = new List<Vector3>(desiredMesh.vertices);

            // Create expanded bounding boxes
            var expandedBoxes = currentTriangles.ConvertAll(triangle => GetExpandedBox(triangle, _buffer));

            // Determine vertex colors based on containment
            var colors = new Color[desiredPoints.Count];
            var matches = 0f;
            for (int i = 0; i < desiredPoints.Count; i++)
            {
                var point = desiredPoints[i];
                bool contained = expandedBoxes.Exists(box => box.Contains(point));
                colors[i] = contained ? Color.green : Color.red;
                if (contained) matches++;
            }
            
            _hud.UpdateDisplay(matches / desiredPoints.Count * 100f);

            // Assign vertex colors to the desired mesh
            desiredMesh.colors = colors;

            // Apply the vertex color shader material to the desired mesh
            _desiredMeshFilter.GetComponent<Renderer>().material = _vertexColorMaterial;
            
        }
    }
}