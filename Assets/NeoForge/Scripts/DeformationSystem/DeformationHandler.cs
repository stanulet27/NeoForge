using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeformationSystem
{
    public class DeformationHandler : MonoBehaviour
    {
        [Tooltip("The trigger tracker that is used to determine parts and vertices that are hit.")]
        [SerializeField] private TriggerTracker _selector;
        
        [FormerlySerializedAs("_cam")]
        [Tooltip("The camera that is used to determine the direction of the hit.")]
        [SerializeField] private Transform _camera;
        
        [Tooltip("The force that is applied by the hit.")]
        [SerializeField, Range(0, 10)] private float _force = 1f;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) ModifyMeshesHit();
        }
        
        private void ModifyMeshesHit()
        {
            _selector.GetContainedObjects<Deformable>().ToList().ForEach(ModifyMesh);
        }
        
        private void ModifyMesh(Deformable deformable)
        {
            var direction = transform.localToWorldMatrix.MultiplyVector(_camera.forward);
            StartCoroutine(deformable.ScaleMeshVertices(_force, direction, _selector.Contains));
        }
    }
}