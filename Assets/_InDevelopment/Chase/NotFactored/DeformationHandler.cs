using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _InDevelopment.Chase.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeformationSystem
{
    public class DeformationHandler : MonoBehaviour
    {
        public static Action OnDeformationPerformed;
        
        [Tooltip("The trigger tracker that is used to determine parts and vertices that are hit.")]
        [SerializeField] private TriggerTracker _selector;
        
        [FormerlySerializedAs("_cam")]
        [Tooltip("The camera that is used to determine the direction of the hit.")]
        [SerializeField] private Transform _camera;
        
        [Tooltip("The force that is applied by the hit.")]
        [SerializeField, Range(0, 10)] private float _force = 1f;
        
        [SerializeField] private ForgeHUD _hud;
        
        private void Start()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) ModifyMeshesHit();
        }
        
        private void ModifyMeshesHit()
        {
            _selector.GetContainedObjects<Deformable>().ToList().ForEach(x => StartCoroutine(PerformDeformation(x)));
        }
        
        private IEnumerator PerformDeformation(Deformable deformable)
        {
            var direction = _camera.transform.forward;
            yield return deformable.ScaleMeshVertices(_force, direction, _selector.Contains);
            OnDeformationPerformed?.Invoke();
        }
    }
}