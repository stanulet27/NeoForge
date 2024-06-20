using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using NeoForge.Deformation.JSON;
using NeoForge.Deformation.Scoring;
using NeoForge.Deformation.UI;
using NeoForge.Input;

namespace NeoForge.Deformation
{
    public class DeformationHandler : MonoBehaviour
    {
        public static Action OnDeformationPerformed;
        
        [Tooltip("The trigger tracker that is used to determine parts and vertices that are hit.")]
        [SerializeField] private TriggerTracker _selector;

        [Tooltip("This is the part that is being deformed.")]
        [SerializeField] private GameObject _part;

        [Tooltip("This is the target that the part is being deformed to.")]
        [SerializeField] private GameObject _target;
        
        [Range(0, 10)]
        [Tooltip("The force that is applied by the hit.")]
        [SerializeField] private float _force = 1f;
        
        [Tooltip("The HUD that is used to display the force and size of the selector.")]
        [SerializeField] private ForgeHUD _hud;
        
        
        private MeshFilter _partMesh => _part.GetComponent<MeshFilter>();
        private MeshCollider _partMeshCollider => _target.GetComponent<MeshCollider>();
        
        private MeshFilter _targetMesh => _target.GetComponent<MeshFilter>();
        private MeshCollider _targetMeshCollider => _target.GetComponent<MeshCollider>();
        
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

        public static IEnumerator SetupPart(ForgedPart part)
        {
            var environmentChoices = new EnvironmentChoices(part.Details);
            yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(environmentChoices), "/init");

            yield return WebServerConnectionHandler.SendGetRequest("/starting-mesh");
            var startingMeshData = JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData);
            part.PartMesh.mesh = CreateMesh(startingMeshData);
            part.PartCollider.sharedMesh = part.PartMesh.mesh;

            yield return WebServerConnectionHandler.SendGetRequest("/target-mesh");
            var targetMeshData = JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData);
            part.DesiredMesh.mesh = CreateMesh(targetMeshData);
            part.DesiredCollider.sharedMesh = part.DesiredMesh.mesh;

            yield return WebServerConnectionHandler.SendGetRequest("/material");
            var jsonMaterial = JsonUtility.FromJson<JSONMaterial>(WebServerConnectionHandler.ReturnData);
            part.Details.SetMaterialData(jsonMaterial);
            
            yield return part.Details.SetScoreDetails(part);
        }

        public IEnumerator PrepareEnvironment(ForgedPart part)
        {
            var environmentChoices = new EnvironmentChoices(part.Details) { HammerID = 0 };
            yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(environmentChoices), "/init");
            
            yield return WebServerConnectionHandler.SendGetRequest("/hammer");
            var hammerData = JsonUtility.FromJson<HammerData>(WebServerConnectionHandler.ReturnData);
            _selector.transform.localScale = new Vector3(hammerData.SizeY, 1, hammerData.SizeX);
            
            _maxForce = part.Details.Material.MaximumForce;
            _target = part.DesiredMesh.gameObject;
            _part = part.gameObject;
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
            var meshData = JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData);
            _partMesh.mesh = CreateMesh(meshData);
            _partMeshCollider.sharedMesh = _partMesh.mesh;
            OnDeformationPerformed?.Invoke();
        }
        
        private IEnumerator HitIntersectedMesh(Deformable deformable)
        {
            _force = Mathf.Min(_force, _maxForce);    
            yield return deformable.PerformHitOperation(_force, _part.transform, _selector.Contains);
            OnDeformationPerformed?.Invoke();
        }
    }
}