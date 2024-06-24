using UnityEngine;

namespace NeoForge.Deformation
{
    public class PartMeshHandler : MonoBehaviour
    {
        [Tooltip("The users part mesh")]
        [SerializeField] private MeshFilter _partMesh;
        [Tooltip("The desired part mesh")]
        [SerializeField] private MeshFilter _desiredMesh;
        
        /// <summary>
        /// The mesh of the part that is being deformed
        /// </summary>
        public MeshFilter PartMesh => _partMesh;
        
        /// <summary>
        /// The desired mesh of the part
        /// </summary>
        public MeshFilter DesiredMesh => _desiredMesh;
        
        /// <summary>
        /// The renderer for the heatmap score of the part
        /// </summary>
        public MeshRenderer HeatmapRenderer { get; private set; }
        
        /// <summary>
        /// The collider for the user made part mesh
        /// </summary>
        public MeshCollider PartCollider { get; private set; }
        
        /// <summary>
        /// The collider for the desired part mesh
        /// </summary>
        public MeshCollider DesiredCollider { get; private set; }

        private void Awake()
        {
            HeatmapRenderer = _desiredMesh.GetComponent<MeshRenderer>();
            PartCollider = _partMesh.GetComponent<MeshCollider>();
            DesiredCollider = _desiredMesh.GetComponent<MeshCollider>();
        }
    }
}