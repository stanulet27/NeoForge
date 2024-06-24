using UnityEngine;

namespace NeoForge.Input
{
    public class OverlayManager : MonoBehaviour
    {
        [Tooltip("The part that will be toggled between opaque and transparent")]
        [SerializeField] private Renderer _part;
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
            _part.material = _isOverlayActive ? _opaqueMaterial : _transparentMaterial;
             
            _scoreCamera.cullingMask ^= 1 << LayerMask.NameToLayer("Desired");
            _isOverlayActive = !_isOverlayActive;
        }
    }
}