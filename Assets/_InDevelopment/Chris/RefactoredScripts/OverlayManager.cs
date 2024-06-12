using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.Input
{
    public class OverlayManager : MonoBehaviour
    {
        [Tooltip("The part that will be toggled between opaque and transparent")]
        [SerializeField] private GameObject _part;
        [Tooltip("The opaque material that will be applied to the part")]
        [SerializeField] private Material _opaqueMaterial;
        [Tooltip("The transparent material that will be applied to the part")]
        [SerializeField] private Material _transparentMaterial;
        
        private Camera _scoreCamera;
        private bool _isOverlayActive;
        
        private void Start()
        {
            _scoreCamera = GetComponent<Camera>();
            ControllerManager.OnOverlay += ToggleOverlay;
        }

        private void OnDestroy()
        {
            ControllerManager.OnOverlay -= ToggleOverlay;
        }

        private void ToggleOverlay()
        {
            _part.GetComponent<Renderer>().material = _isOverlayActive ? _opaqueMaterial : _transparentMaterial;
            var newMask = _scoreCamera.cullingMask ^ (1 << LayerMask.NameToLayer("Desired"));
            _scoreCamera.cullingMask = newMask;
            _isOverlayActive = !_isOverlayActive;
        }
    }
}