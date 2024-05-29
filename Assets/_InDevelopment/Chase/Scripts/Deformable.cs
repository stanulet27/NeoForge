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
        public IEnumerator ScaleMeshVertices(float force, Vector3 direction, Predicate<Vector3> isHit)
        {
            var mesh = _meshFilter.mesh;
            var intersections = mesh.vertices
                .Where(x => isHit(transform.localToWorldMatrix.MultiplyPoint(x)))
                .Distinct()
                .ToArray();
            var localHitDirection = transform.worldToLocalMatrix.MultiplyVector(direction);
            
            var meshData = new MeshData(mesh, intersections, force, localHitDirection);
            var tempOutputPath = Path.GetTempFileName();
            try
            {
                var start = Time.realtimeSinceStartup;
                yield return WebServerConnectionHandler.SendRequest(JsonUtility.ToJson(meshData));
                Debug.Log($"Request took {Time.realtimeSinceStartup - start} seconds.");
                
                RebuildMesh(WebServerConnectionHandler.ReturnData, mesh);
                SwapMush(mesh);
            }
            finally
            {
                if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
            }
        }

        private static void RebuildMesh(string tempOutputPath, Mesh mesh)
        {
            var modifiedMeshData = JsonUtility.FromJson<MeshData>(tempOutputPath);

            var vertices = modifiedMeshData.Vertices;
            var modifiedVertices = Enumerable.Range(0, vertices.Length / 3)
                .Select(i => new Vector3(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]))
                .ToArray();
            
            Debug.Assert(modifiedVertices.Length > 0, "Modified mesh vertices are not properly assigned or empty.");
            Debug.Assert(modifiedMeshData.Triangles is {Length: > 0}, "Modified mesh triangles are not properly assigned or empty.");

            mesh.vertices = modifiedVertices;
            mesh.triangles = modifiedMeshData.Triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        
        private void SwapMush(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
        }
    }
}