using System;
using UnityEditor;
using UnityEngine;

namespace NeoForge.Input
{
    public class OverlayManager : MonoBehaviour
    {
        //private static const int = b10000000;
        
        [SerializeField] private GameObject _part;
        [SerializeField] private Material _opaqueMaterial;
        [SerializeField] private Material _transparentMaterial;
        
        private Camera _scoreCamera;
        private bool _isOverlayActive;
        
        private void Start()
        {
            _scoreCamera = Camera.main;
            ControllerManager.OnOverlay += ToggleOverlay;
        }

        private void OnDestroy()
        {
            ControllerManager.OnOverlay -= ToggleOverlay;
        }

        private void ToggleOverlay()
        {
            //toggle part material (transparent/opaque)
            _part.GetComponent<Renderer>().material = _isOverlayActive ? _opaqueMaterial : _transparentMaterial;
            //toggle score culling mask
            var newMask = _scoreCamera.cullingMask ^ (1 << 9);
            _scoreCamera.cullingMask = newMask;
            _isOverlayActive = !_isOverlayActive;
            
        }
    }
}