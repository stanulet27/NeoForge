using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class DeformationHandler : MonoBehaviour
    {
        public static Action OnDeformationPerformed;
        
        [Tooltip("The trigger tracker that is used to determine parts and vertices that are hit.")]
        [SerializeField] private TriggerTracker _selector;

        [Tooltip("The camera that is used to determine the direction of the hit.")]
        [SerializeField] private Transform _camera;
        
        [Range(0, 10)]
        [Tooltip("The force that is applied by the hit.")]
        [SerializeField] private float _force = 1f;
        
        [Tooltip("The HUD that is used to display the force and size of the selector.")]
        [SerializeField] private ForgeHUD _hud;

        private void Start()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());
        }
        
        private void Update()
        {
            _hud.UpdateDisplay(force: _force, size: _selector.GetSize());

            if (Input.GetKeyDown(KeyCode.V)) HitIntersectedMeshes();
        }

        private void HitIntersectedMeshes()
        {
            var meshesHit = _selector.GetContainedObjects<Deformable>().ToList();
            meshesHit.ForEach(x => StartCoroutine(HitIntersectedMesh(x)));
        }
        
        private IEnumerator HitIntersectedMesh(Deformable deformable)
        {
            var direction = _camera.transform.forward;
            yield return deformable.PerformHitOperation(_force, direction, _selector.Contains);
            OnDeformationPerformed?.Invoke();
        }
    }
}