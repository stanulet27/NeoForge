using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Deformation.JSON;
using SharedData;
using UnityEngine;
using Color = UnityEngine.Color;

namespace NeoForge.Deformation.Scoring
{
    public class MeshSimilarityCalculator : MonoBehaviour
    {
        // The number is irrelevant, but it must be a constant
        private const int SEED = 1987;
        private const int POINTS_TO_GENERATE = 10000;
        private const float MAX_SCORE = 100.0f;
        private static readonly Color _successColor = Color.green;
        private static readonly Color _failureColor = Color.red;
        
        [Tooltip("The shared float that will store the score of the part.")]
        [SerializeField] private SharedFloat _score;

        [Tooltip("The buffer that is used to expand the bounding box of the current mesh.")]
        [SerializeField] private float _buffer = 0.05f;
        
        [Tooltip("Determines which points to display in editor.")]
        [SerializeField] private RaycastPoint.Mode _displayMode = RaycastPoint.Mode.Undershot;

        private float _initialScore;
        private bool _hasPart;
        private List<RaycastPoint> _undershotPoints = new();
        private MeshFilter _userMeshFilter;
        private MeshFilter _desiredMeshFilter;
        private Renderer _heatMapRenderer;
        
        /// <summary>
        /// The current score of the part.
        /// </summary>
        public float Score => _score;
        
        /// <summary>
        /// The score for machine cost.
        /// </summary>
        public static float MachineCostScore => RaycastPoint.GetScore(RaycastPoint.Mode.Overshot);

        private void Start()
        {
            DeformationHandler.OnDeformationPerformed += OnDeformationPerformed;
        }

        private void OnDestroy()
        {
            DeformationHandler.OnDeformationPerformed -= OnDeformationPerformed;
        }

        private void OnDrawGizmosSelected()
        {
            var points = _displayMode == RaycastPoint.Mode.Undershot ? _undershotPoints : RaycastPoint.Points[_displayMode];
            
            foreach (var point in points)
            {
                Gizmos.color = point.DoesItScore() ? Color.green : Color.red;
                Gizmos.DrawSphere(point.Origin, 0.01f);
            }
        }
        
        /// <summary>
        /// Will send the score to the server
        /// </summary>
        public void PostScore()
        {
            StartCoroutine(SendScorePutRequest());
        }

        /// <summary>
        /// Will setup the calculator to be able to score the specified part. If the part is null, it will clear the part.
        /// </summary>
        public void SetPart(ScoringDetails scoringDetails)
        {
            _hasPart = scoringDetails != null;
            if (!_hasPart)
            {
                ClearPart();
                return;
            }
            
            _userMeshFilter = scoringDetails!.Part.PartMesh;
            _desiredMeshFilter = scoringDetails.Part.DesiredMesh;
            _heatMapRenderer = scoringDetails.Part.HeatmapRenderer;
            
            Debug.Assert(_userMeshFilter != null, "User mesh filter is null");
            Debug.Assert(_desiredMeshFilter != null, "Desired mesh filter is null");
            Debug.Assert(_heatMapRenderer != null, "Heat map renderer is null");

            _undershotPoints = scoringDetails.UndershotPoints;
        }
        
        private void ClearPart()
        {
            _hasPart = false;
        }
        
        /// <summary>
        /// Will refresh the score of the part and save it to the shared float, _score.
        /// Score can be retrieved by calling Score.
        /// </summary>
        public void UpdateScore()
        {
            if (!_hasPart) return;
            
            GeneratePoints(_userMeshFilter, RaycastPoint.Mode.Overshot);
            DisplayHeatMap();
            _score.Value = DetermineScore();
        }

        private static List<RaycastPoint> GeneratePoints(MeshFilter meshFilter, RaycastPoint.Mode mode)
        {
            var generator = new System.Random(SEED);
            var bounds = meshFilter.mesh.bounds;
            var min = bounds.min;
            var max = bounds.max;

            RaycastPoint.ClearPoints(mode);
            
            for (int i = 0; i < POINTS_TO_GENERATE; i++)
            {
                var x = GetRandomValue(generator, min.x, max.x);
                var y = GetRandomValue(generator, min.y, max.y);
                var z = GetRandomValue(generator, min.z, max.z);
                var _ = new RaycastPoint(new Vector3(x, y, z), meshFilter.transform, mode);
            }
            
            return RaycastPoint.Points[mode];
        }

        private IEnumerator SendScorePutRequest()
        {
            var json = JsonUtility.ToJson(new ScoreData(_score.Value));
            yield return WebServerConnectionHandler.SendPutRequest(json, "/post-score");
        }

        private void OnDeformationPerformed()
        {
            UpdateScore();
        }
        
        private static float GetRandomValue(System.Random generator, float min, float max)
        {
            return (float) generator.NextDouble() * (max - min) + min;
        }

        private void DisplayHeatMap()
        {
            var desiredMesh = _desiredMeshFilter.mesh;
            var desiredPoints = new List<Vector3>(desiredMesh.vertices);
            var triangles = ConvertMeshToTriangles(_userMeshFilter.mesh);
            var expandedBoxes = triangles.ConvertAll(x => x.GetBounds(_buffer));
            bool IsContained(Vector3 point) => expandedBoxes.Any(b => b.Contains(point));

            desiredMesh.colors = desiredPoints
                .Select(p => IsContained(p) ? _successColor : _failureColor)
                .ToArray();
        }

        private float DetermineScore()
        {
            var currentPercent = RaycastPoint.GetScore(_undershotPoints) * 0.8f +
                                 RaycastPoint.GetScore(RaycastPoint.Mode.Overshot) * 0.2f;
            
            return currentPercent / MAX_SCORE * 100.0f;
        }

        private static List<Triangle> ConvertMeshToTriangles(Mesh mesh)
        {
            var triangles = new List<Triangle>();
            var vertices = mesh.vertices;
            var meshTriangles = mesh.triangles;

            for (var i = 0; i < meshTriangles.Length; i += 3)
            {
                var x = vertices[meshTriangles[i]];
                var y = vertices[meshTriangles[i + 1]];
                var z = vertices[meshTriangles[i + 2]];
                triangles.Add(new Triangle(x, y, z));
            }

            return triangles;
        }

        private class Triangle
        {
            private readonly Vector3[] _vertices;
            
            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                _vertices = new[] { v1, v2, v3 };
            }

            public Bounds GetBounds(float buffer = 0.0f)
            {
                var bufferVector = new Vector3(buffer, buffer, buffer);
                var minCorner = Vector3.Min(Vector3.Min(_vertices[0], _vertices[1]), _vertices[2]) - bufferVector;
                var maxCorner = Vector3.Max(Vector3.Max(_vertices[0], _vertices[1]), _vertices[2]) + bufferVector;
                return new Bounds((minCorner + maxCorner) / 2, maxCorner - minCorner);
            }
        }

        public class ScoringDetails
        {
            /// <summary>
            /// The points used for determining the score of the part regarding points inside the desired mesh.
            /// </summary>
            public List<RaycastPoint> UndershotPoints { get; private set; }
            
            public PartMeshHandler Part { get; private set; }

            /// <summary>
            /// Will setup up UndershotPoints for the specified part.
            /// </summary>
            public IEnumerator Setup(PartMeshHandler part)
            {
                yield return null; // Let mesh initialize
                Part = part;
                UndershotPoints = new List<RaycastPoint>(GeneratePoints(part.DesiredMesh, RaycastPoint.Mode.Undershot));
            }
        }
    }
}
