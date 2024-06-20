using UnityEngine;

namespace NeoForge.UI.Warehouse
{
    public class PartViewer : MonoBehaviour
    {
        [SerializeField] Vector3 _rotationSpeed = new(4, 5, -6);
        [SerializeField] MeshFilter _partMeshDisplay;
        
        private void Update()
        {
            _partMeshDisplay.transform.Rotate(_rotationSpeed * Time.deltaTime);
        }
        
        public void DisplayPart(Mesh mesh)
        {
            _partMeshDisplay.gameObject.SetActive(mesh != null);
            _partMeshDisplay.mesh = mesh;
        }
    }
}