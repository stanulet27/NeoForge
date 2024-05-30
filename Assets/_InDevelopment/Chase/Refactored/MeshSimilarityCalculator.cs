using System.Collections.Generic;
using DeformationSystem;
using UnityEngine;

namespace _InDevelopment.Chase.Scripts
{
    public class MeshSimilarityCalculator : MonoBehaviour
    {
        [Header("Meshes")]
        [Tooltip("The mesh filter of the current mesh.")]
        [SerializeField] private MeshFilter _userMeshFilter;
        [Tooltip("The mesh filter of the desired mesh.")]
        [SerializeField] private MeshFilter _desiredMeshFilter;
        
        [Header("Settings")]
        [Tooltip("The buffer that is used to expand the bounding box of the current mesh.")]
        [SerializeField] private float _buffer = 0.05f;
        [Tooltip("The material that is used to color the vertices of the desired mesh to show score")]
        [SerializeField] private Material _vertexColorMaterial;
        [Tooltip("The HUD that is used to display the score.")]
        [SerializeField] private ForgeHUD _hud;
        
        private void Start()
        {
            _desiredMeshFilter.GetComponent<Renderer>().material = _vertexColorMaterial;
            CalculateScore();
            DeformationHandler.OnDeformationPerformed += CalculateScore;
        }
        
        private void CalculateScore()
        {
            var desiredMesh = _desiredMeshFilter.mesh;
            var desiredPoints = new List<Vector3>(desiredMesh.vertices);
            var expandedBoxes = GetTrianglesBoundingBoxes(_userMeshFilter);

            DetermineAndDisplayScore(desiredPoints, expandedBoxes, desiredMesh);
        }
        
        private List<Bounds> GetTrianglesBoundingBoxes(MeshFilter meshFilter)
        {
            var triangles = ConvertMeshToTriangles(meshFilter.mesh);
            return triangles.ConvertAll(triangle => triangle.GetBounds(_buffer));
        }

        private void DetermineAndDisplayScore(List<Vector3> desiredPoints, List<Bounds> expandedBoxes, Mesh desiredMesh)
        {
            var colors = new Color[desiredPoints.Count];
            var matches = 0f;
            
            for (int i = 0; i < desiredPoints.Count; i++)
            {
                var point = desiredPoints[i];
                var pointIsContained = expandedBoxes.Exists(box => box.Contains(point));
                
                if (pointIsContained)
                {
                    colors[i] = Color.green;
                    matches += 1;
                }
                else
                {
                    colors[i] = Color.red;
                }
            }
            
            desiredMesh.colors = colors;
            _hud.UpdateDisplay(matches / desiredPoints.Count * 100f);
        }

        private static List<Triangle> ConvertMeshToTriangles(Mesh mesh)
        {
            var triangles = new List<Triangle>();
            var vertices = mesh.vertices;
            var meshTriangles = mesh.triangles;

            for (var i = 0; i < meshTriangles.Length; i += 3)
            {
                var x = vertices[meshTriangles[i]];
                var y = vertices[meshTriangles[i + 1]];
                var z = vertices[meshTriangles[i + 2]];
                triangles.Add(new Triangle(x, y, z));
            }

            return triangles;
        }

        private class Triangle
        {
            private readonly Vector3[] _vertices;
            
            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                _vertices = new[] { v1, v2, v3 };
            }
            
            public Bounds GetBounds(float buffer = 0.0f)
            {
                var bufferVector = new Vector3(buffer, buffer, buffer);
                var minCorner = Vector3.Min(Vector3.Min(_vertices[0], _vertices[1]), _vertices[2]) - bufferVector;
                var maxCorner = Vector3.Max(Vector3.Max(_vertices[0], _vertices[1]), _vertices[2]) + bufferVector;
                return new Bounds((minCorner + maxCorner) / 2, maxCorner - minCorner);
            }
        }
    }
}