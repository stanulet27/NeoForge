using System.Collections;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class DeformationHandler : MonoBehaviour
    {

        [Tooltip("The game object that represents the hammer")]
        [SerializeField] private GameObject _selector;

        [Tooltip("The game object that represents the part that will be struck")]
        [SerializeField] private GameObject _part;

        [Tooltip("The mesh that represents the target geometry")]
        [SerializeField] private GameObject _target;

        [Tooltip("The force that is applied by the hit.")]
        [SerializeField, Range(0, 10)] private float _force = 1f;

        private float _maxForce;
        private TriggerTracker _triggerTracker;

        private void Start()
        {
            //Setup the connection
            _triggerTracker = _selector.GetComponent<TriggerTracker>();
            StartCoroutine(SetupEnvironment());
        }

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
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) ModifyMeshesHit();
        }

        private void ModifyMeshesHit()
        {
            _triggerTracker.GetContainedObjects<Deformable>().ToList().ForEach(ModifyMesh);
        }

        private void ModifyMesh(Deformable deformable)
        {
            if(_force > _maxForce) _force = _maxForce;    
            StartCoroutine(deformable.ScaleMeshVertices(_force, _part.transform.position ,_part.transform.rotation, _triggerTracker.Contains));
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
    }
}