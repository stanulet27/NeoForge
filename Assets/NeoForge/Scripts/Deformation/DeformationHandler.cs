using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using NeoForge.Deformation.JSON;
using NeoForge.Deformation.UI;
using NeoForge.Input;

namespace NeoForge.Deformation
{
    public class DeformationHandler : MonoBehaviour
    {
        /// <summary>
        /// Occurs whenever a change is made to the mesh.
        /// </summary>
        public static Action OnDeformationPerformed;

        /// <summary>
        /// Occurs whenever a hit is performed.
        /// </summary>
        public static Action OnHit;

        [Tooltip("The trigger tracker that is used to determine parts and vertices that are hit.")]
        [SerializeField] private TriggerTracker _selector;

        [Range(0, 10)]
        [Tooltip("The force that is applied by the hit.")]
        [SerializeField] private float _force = 1f;
        
        [Tooltip("The HUD that is used to display the force and size of the selector.")]
        [SerializeField] private PlanningHUD _hud;
        
        private Transform _part;
        private MeshFilter _partMesh;
        private MeshCollider _targetCollider;

        private float _maxForce;
        
        private void OnEnable()
        {
            ControllerManager.OnHit += HitIntersectedMeshes;
        }
                
        private void OnDisable()
        {
            ControllerManager.OnHit -= HitIntersectedMeshes;
        }

        private void Start()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
        }

        private void Update()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
        }
        
        public void UndoLastDeformation()
        {
            StartCoroutine(UndoDeformation());
        }

        public static IEnumerator SetupPart(JAXEnvironmentSettings settings)
        {
            yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(settings.Environment), "/init");

            yield return WebServerConnectionHandler.SendGetRequest("/starting-mesh");
            var startingMeshData = CreateMesh(JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData));
            settings.MeshData.PartMesh.mesh = startingMeshData;
            settings.MeshData.PartCollider.sharedMesh = startingMeshData;

            yield return WebServerConnectionHandler.SendGetRequest("/target-mesh");
            var targetMeshData = CreateMesh(JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData));
            settings.MeshData.DesiredMesh.mesh = targetMeshData;
            settings.MeshData.DesiredCollider.sharedMesh = targetMeshData;

            yield return WebServerConnectionHandler.SendGetRequest("/material");
            var jsonMaterial = JsonUtility.FromJson<JSONMaterial>(WebServerConnectionHandler.ReturnData);
            settings.Details.SetMaterialData(jsonMaterial);
            
            yield return settings.Details.SetScoreDetails(settings.MeshData);
        }

        public IEnumerator PrepareEnvironment(JAXEnvironmentSettings settings)
        {
            settings.Environment.HammerID = 0;
            yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(settings.Environment), "/init");
            
            yield return WebServerConnectionHandler.SendGetRequest("/hammer");
            var hammerData = JsonUtility.FromJson<HammerData>(WebServerConnectionHandler.ReturnData);
            _selector.transform.localScale = new Vector3(hammerData.SizeY, 1, hammerData.SizeX);
            
            _maxForce = settings.Details.Material.MaximumForce;
            _partMesh = settings.MeshData.PartMesh;
            _targetCollider = settings.MeshData.DesiredCollider;
            _part = settings.Part;
        }

        private static Mesh CreateMesh(MeshData meshData)
        {
            var vertices = meshData.Vertices;
            var triangles = meshData.Triangles;
            var mesh = new Mesh();
            var meshVertices = new Vector3[vertices.Length / 3];
            for (var i = 0; i < vertices.Length; i += 3)
            {
                meshVertices[i / 3] = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
            }
            mesh.vertices = meshVertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.Optimize();
            return mesh;
        }
        
        private void HitIntersectedMeshes()
        {
            var meshesHit = _selector.GetContainedObjects<Deformable>().ToList();
            meshesHit.ForEach(x => StartCoroutine(HitIntersectedMesh(x)));
        }
        
        private IEnumerator UndoDeformation()
        {
            yield return WebServerConnectionHandler.SendGetRequest("/undo-strike");
            var meshData = CreateMesh(JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData));
            _partMesh.mesh = meshData;
            _targetCollider.sharedMesh = meshData;
            OnDeformationPerformed?.Invoke();
        }
        
        private IEnumerator HitIntersectedMesh(Deformable deformable)
        {
            _force = Mathf.Min(_force, _maxForce);    
            yield return deformable.PerformHitOperation(_force, _part, _selector.Contains);
            OnDeformationPerformed?.Invoke();
            OnHit?.Invoke();
        }
    }
}