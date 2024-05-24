using UnityEngine;

namespace DeformationSystem
{
    public class ComparisonHandler : MonoBehaviour
    {
        [SerializeField] private MeshFilter playerMesh;
        [SerializeField] private MeshFilter goalMesh;

        private void Start()
        {
            DisplayScore();
        }

        private void DisplayScore()
        {
            var matchScore = MeshComparer.CalculateMatchScore(playerMesh.mesh, goalMesh.mesh);
            Debug.Log($"Match score: {matchScore}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayScore();
            }
        }
    }
}