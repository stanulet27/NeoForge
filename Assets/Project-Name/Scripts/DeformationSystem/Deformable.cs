using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Deformable : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        [ContextMenu("Scale Mesh Vertices")]
        public IEnumerator PerformHitOperation(float force, Vector3 direction, Predicate<Vector3> isHit)
        {
            var mesh = _meshFilter.mesh;
            var localHitDirection = transform.worldToLocalMatrix.MultiplyVector(direction);
            var intersections = FindIntersections(mesh, isHit);
    
            var meshData = new MeshData(mesh, intersections, force, localHitDirection);
            yield return SendHitRequest(meshData);

            UpdateMeshFromJson(WebServerConnectionHandler.ReturnData, mesh);
            ReplaceMesh(mesh);
        }

        private IEnumerable<Vector3> FindIntersections(Mesh mesh, Predicate<Vector3> isHit)
        {
            return mesh.vertices
                .Where(vertex => isHit(transform.localToWorldMatrix.MultiplyPoint(vertex)))
                .Distinct()
                .ToArray();
        }

        private static IEnumerator SendHitRequest(MeshData meshData)
        {
            var tempOutputPath = Path.GetTempFileName();
            try
            {
                var start = Time.realtimeSinceStartup;
                yield return WebServerConnectionHandler.SendRequest(JsonUtility.ToJson(meshData));
                Debug.Log($"Request took {Time.realtimeSinceStartup - start} seconds.");
            }
            finally
            {
                CleanupTempFile(tempOutputPath);
            }
        }

        private static void CleanupTempFile(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        private static void UpdateMeshFromJson(string jsonText, Mesh mesh)
        {
            var modifiedMeshData = JsonUtility.FromJson<MeshData>(jsonText);
            var modifiedVertices = ConvertToVector3Array(modifiedMeshData.Vertices);
            if (TryValidateMeshData(modifiedVertices, modifiedMeshData.Triangles))
            {
                UpdateMesh(mesh, modifiedVertices, modifiedMeshData.Triangles);
            }
        }

        private static Vector3[] ConvertToVector3Array(float[] vertices)
        {
            return Enumerable.Range(0, vertices.Length / 3)
                .Select(i => new Vector3(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]))
                .ToArray();
        }

        private static bool TryValidateMeshData(Vector3[] vertices, int[] triangles)
        {
            Debug.Assert(vertices.Length > 0, "Modified mesh vertices are not properly assigned or empty.");
            Debug.Assert(triangles is { Length: > 0 }, "Modified mesh triangles are not properly assigned or empty.");
            
            return vertices.Length > 0 && triangles is { Length: > 0 };
        }

        private static void UpdateMesh(Mesh mesh, Vector3[] vertices, int[] triangles)
        {
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        
        private void ReplaceMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
        }
    }
}