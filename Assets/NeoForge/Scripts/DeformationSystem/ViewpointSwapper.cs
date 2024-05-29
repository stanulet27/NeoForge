using System.Collections.Generic;
using UnityEngine;

namespace DeformationSystem
{
    public class ViewpointSwapper : MonoBehaviour
    {
        private readonly List<Camera> _cameras = new();
        private int _currentCameraIndex;
        
        private void Start()
        {
            GetComponentsInChildren(true, _cameras);
            _cameras.ForEach(camera => camera.enabled = false);
            _cameras[0].enabled = true;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) SwapToNextCamera();
        }

        private void SwapToNextCamera()
        {
            _cameras[_currentCameraIndex].enabled = false;
            _currentCameraIndex = (_currentCameraIndex + 1) % _cameras.Count;
            _cameras[_currentCameraIndex].enabled = true;
        }
    }
}