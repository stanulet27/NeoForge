using System.Collections.Generic;
using UnityEngine;

namespace DeformationSystem
{
    public class ViewpointSwapper : MonoBehaviour
    {
        private List<Camera> cameras = new();

        int currentCameraIndex = 0;
        
        private void Start()
        {
            GetComponentsInChildren(true, cameras);
            cameras.ForEach(camera => camera.enabled = false);
            cameras[0].enabled = true;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                cameras[currentCameraIndex].enabled = false;
                currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
                cameras[currentCameraIndex].enabled = true;
            }
        }
    }
}