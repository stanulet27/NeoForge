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
        
        private float _maxForce;
        
        private IEnumerator SetupEnvironment()
        {
            //send environment data (all zeros is default)
            ///TODO: Select environment data and replace defaults
            EnvironmentChoices environmentChoices = new EnvironmentChoices(0, 0, 0, 0);
            yield return WebServerConnectionHandler.SendPutRequest(JsonUtility.ToJson(environmentChoices), "/init");

            yield return WebServerConnectionHandler.SendGetRequest("/starting-mesh");
            MeshData startingMeshData = JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData);
            _part.GetComponent<MeshFilter>().mesh = CreateMesh(startingMeshData);
            _part.GetComponent<MeshCollider>().sharedMesh = _part.GetComponent<MeshFilter>().mesh;

            yield return WebServerConnectionHandler.SendGetRequest("/target-mesh");
            MeshData targetMeshData = JsonUtility.FromJson<MeshData>(WebServerConnectionHandler.ReturnData);
            _target.GetComponent<MeshFilter>().mesh = CreateMesh(targetMeshData);
            _target.GetComponent<MeshCollider>().sharedMesh = _target.GetComponent<MeshFilter>().mesh; 
            
            yield return WebServerConnectionHandler.SendGetRequest("/hammer");
            HammerData hammerData = JsonUtility.FromJson<HammerData>(WebServerConnectionHandler.ReturnData);
            _selector.transform.localScale = new Vector3(hammerData.SizeY, 1, hammerData.SizeX);

            yield return WebServerConnectionHandler.SendGetRequest("/material");
            MaterialData materialData = JsonUtility.FromJson<MaterialData>(WebServerConnectionHandler.ReturnData);
            _maxForce =  materialData.MaximumDeformation;
            
            OnDeformationPerformed?.Invoke(); //invoke this to get initial score/heatmap
        }

        private void OnEnable()
        {
            ControllerManager.OnHit += HitIntersectedMeshes;
        }

        private Mesh CreateMesh(MeshData meshData)
        {
            float[] vertices = meshData.Vertices;
            int[] triangles = meshData.Triangles;
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
        
        private void OnDisable()
        {
            ControllerManager.OnHit -= HitIntersectedMeshes;
        }

        private void Start()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
            StartCoroutine(SetupEnvironment());
        }

        private void Update()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
        }

        private void HitIntersectedMeshes()
        {
            var meshesHit = _selector.GetContainedObjects<Deformable>().ToList();
            meshesHit.ForEach(x => StartCoroutine(HitIntersectedMesh(x)));
        }
        
        private IEnumerator HitIntersectedMesh(Deformable deformable)
        {
            _force = Mathf.Min(_force, _maxForce);    
            yield return deformable.PerformHitOperation(_force, _part.transform, _selector.Contains);
            OnDeformationPerformed?.Invoke();
        }
    }
}