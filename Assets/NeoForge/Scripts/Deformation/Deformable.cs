using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoForge.Deformation.JSON;
using UnityEngine;

namespace NeoForge.Deformation
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Deformable : MonoBehaviour
    {

        public Action<ForgedPart> Clicked;
        
        private ForgedPart _part;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _part = transform.parent.gameObject.GetComponent<ForgedPart>();
        }
        
        private void OnMouseDown()
        {
            Clicked?.Invoke(_part);
        }

        [ContextMenu("Export Mesh as JSON")]
        public void ExportMeshAsJson()
        {
            var mesh = _meshFilter.mesh;
            var meshData = new MeshData(mesh);
            var json = JsonUtility.ToJson(meshData);
            var tempOutputPath = "./mesh.json";
            File.WriteAllText(tempOutputPath, json);
            Debug.Log($"Mesh data exported to {tempOutputPath}");
        }

        [ContextMenu("Scale Mesh Vertices")]
        public IEnumerator PerformHitOperation(float force, Transform part, Predicate<Vector3> isHit)
        {
            var mesh = _meshFilter.mesh;
            var intersections = FindIntersections(mesh, isHit);
    
            var hitRequest = new HitData(force, part.rotation, part.position, intersections, new int[] { });
            yield return SendHitRequest(hitRequest);
        }

        private int[] FindIntersections(Mesh mesh, Predicate<Vector3> isHit)
        {
            return mesh.vertices
                .Select((v, i) => new {v, i})
                .Where(vertex => isHit(transform.localToWorldMatrix.MultiplyPoint(vertex.v)))
                .Distinct()
                .Select(vertex => vertex.i)
                .ToArray();
        }

        private IEnumerator SendHitRequest(HitData hitRequest)
        {
              var start = Time.realtimeSinceStartup;
              yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(hitRequest), "/press");
              Debug.Log($"Request took {Time.realtimeSinceStartup - start} seconds.");     
              HandleResponse(WebServerConnectionHandler.ReturnData);
        }
        
        private void HandleResponse(string tempOutputPath)
        {
            var newMeshes = JsonUtility.FromJson<ResultData>(tempOutputPath);
            StartCoroutine(SwapMeshes(newMeshes.ExtractMeshes()));
        }
        
        private IEnumerator SwapMeshes(List<Mesh> meshes)
        {
            foreach (var mesh in meshes)
            {
                yield return new WaitForSeconds(0.1f);
                _meshFilter.mesh = mesh;
                _meshCollider.sharedMesh = mesh;
                _meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
            }

        }
    }
}