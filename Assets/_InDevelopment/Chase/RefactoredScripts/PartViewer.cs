﻿using UnityEngine;

namespace NeoForge.Stations.Warehosue
{
    public class PartViewer : MonoBehaviour
    {
        [Tooltip("The speed at which the part mesh will rotate.")]
        [SerializeField] private Vector3 _rotationSpeed = new(4, 5, -6);
        [Tooltip("The mesh filter that will display the part mesh.")]
        [SerializeField] private MeshFilter _partMeshDisplay;
        
        private void Update()
        {
            _partMeshDisplay.transform.Rotate(_rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Will display the given mesh in the part mesh display. If the mesh is null, the display will be hidden.
        /// </summary>
        public void DisplayPart(Mesh mesh)
        {
            _partMeshDisplay.gameObject.SetActive(mesh != null);
            _partMeshDisplay.mesh = mesh;
        }
    }
}