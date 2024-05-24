using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public static class MeshComparer
    {
        private const float MAX_ALLOWED_DISTANCE = 10.0f; // Adjust based on your game requirements
        private const float MAX_ALLOWED_AREA_DIFFERENCE = 100.0f; // Adjust based on your game requirements

        public static float CalculateMatchScore(Mesh playerMesh, Mesh goalMesh)
        {
            var vertexDistanceScore = CalculateAverageDistance(playerMesh, goalMesh);
            var surfaceAreaScore = CompareSurfaceAreas(playerMesh, goalMesh);

            var normalizedVertexDistanceScore = NormalizeScore(vertexDistanceScore, MAX_ALLOWED_DISTANCE);
            var normalizedSurfaceAreaScore = NormalizeScore(surfaceAreaScore, MAX_ALLOWED_AREA_DIFFERENCE);

            var finalScore = (normalizedVertexDistanceScore * 0.7f) + (normalizedSurfaceAreaScore * 0.3f);
            return finalScore;
        }

        private static float CalculateAverageDistance(Mesh meshA, Mesh meshB)
        {
            var verticesA = meshA.vertices;
            var verticesB = meshB.vertices;

            var totalDistance = verticesA.Sum(v => FindClosestVertexDistance(v, verticesB)) +
                                verticesB.Sum(v => FindClosestVertexDistance(v, verticesA));
            var totalCount = verticesA.Length + verticesB.Length;

            return totalDistance / totalCount;
        }

        private static float FindClosestVertexDistance(Vector3 vertex, Vector3[] vertices)
        {
            return vertices.Min(v => Vector3.Distance(vertex, v));
        }

        private static float CalculateSurfaceArea(Mesh mesh)
        {
            var area = 0f;
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var v0 = vertices[triangles[i]];
                var v1 = vertices[triangles[i + 1]];
                var v2 = vertices[triangles[i + 2]];
                area += Vector3.Cross(v1 - v0, v2 - v0).magnitude * 0.5f;
            }

            return area;
        }

        private static float CompareSurfaceAreas(Mesh playerMesh, Mesh goalMesh)
        {
            var playerArea = CalculateSurfaceArea(playerMesh);
            var goalArea = CalculateSurfaceArea(goalMesh);
            return Mathf.Abs(playerArea - goalArea);
        }

        private static float NormalizeScore(float score, float maxAllowed)
        {
            return Mathf.Clamp01(1.0f - (score / maxAllowed));
        }
    }
}