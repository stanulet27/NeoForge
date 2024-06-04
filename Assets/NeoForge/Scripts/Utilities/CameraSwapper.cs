using System.Collections.Generic;
using NeoForge.Input;
using UnityEngine;

namespace NeoForge.Utilities
{
    public class CameraSwapper : MonoBehaviour
    {
        private readonly List<Camera> _cameras = new();
        private int _currentCameraIndex;

        private void OnEnable()
        {
            ControllerManager.OnSwapCamera += SwapToNextCamera;
        }
        
        private void OnDisable()
        {
            ControllerManager.OnSwapCamera -= SwapToNextCamera;
        }

        private void Start()
        {
            GetComponentsInChildren(true, _cameras);
            _cameras.ForEach(camera => camera.enabled = false);
            _cameras[0].enabled = true;
        }

        private void SwapToNextCamera()
        {
            _cameras[_currentCameraIndex].enabled = false;
            _currentCameraIndex = (_currentCameraIndex + 1) % _cameras.Count;
            _cameras[_currentCameraIndex].enabled = true;
        }

        private void SwapPerspective()
        {
            _cameras[_currentCameraIndex].orthographic = !_cameras[_currentCameraIndex].orthographic;
        }
    }
}