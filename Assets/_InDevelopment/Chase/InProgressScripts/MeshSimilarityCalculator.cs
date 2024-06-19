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

        [Header("References")]
        [Tooltip("The mesh filter of the current mesh.")]
        [SerializeField] private MeshFilter _userMeshFilter;
        [Tooltip("The mesh filter of the desired mesh.")]
        [SerializeField] private MeshFilter _desiredMeshFilter;
        [Tooltip("The renderer to display the heat map of successful points.")]
        [SerializeField] private Renderer _heatMapRenderer;
        [Tooltip("A scriptable object that contains the current score.")]
        [SerializeField] private SharedFloat _score;

        [Header("Settings")]
        [Tooltip("The buffer that is used to expand the bounding box of the current mesh.")]
        [SerializeField] private float _buffer = 0.05f;
        [Tooltip("The material that is used to color the vertices of the desired mesh to show score")]
        [SerializeField] private Material _vertexColorMaterial;
        
        [Header("Debug Display")]
        [Tooltip("Determines which points to display in editor.")]
        [SerializeField] private RaycastPoint.Mode _displayMode = RaycastPoint.Mode.Undershot;

        private float _initialScore;
        private ForgedPart _part;
        private List<RaycastPoint> _undershotPoints = new();
        
        public float Score => _score;
        public float OvershotScore => RaycastPoint.GetScore(RaycastPoint.Mode.Overshot);

        private void Awake()
        {
            _heatMapRenderer.material = _vertexColorMaterial;
        }

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
        
        public void PostScore()
        {
            StartCoroutine(SendScorePutRequest());
        }

        public void SetPart(ForgedPart part)
        {
            _part = part;
            foreach (Transform child in part.transform)
            {
                if (child.gameObject.TryGetComponent(out Deformable deformable))
                {
                    Debug.Log("found user mesh");
                    _userMeshFilter = deformable.GetComponent<MeshFilter>();
                }
                else
                {
                    Debug.Log("found desired mesh");
                    _desiredMeshFilter = child.GetComponent<MeshFilter>();
                    _heatMapRenderer = child.GetComponent<Renderer>();
                }
            }
            _undershotPoints = _part.Details.ScoreDetails.UndershotPoints;
        }
        
        public void CalculateScore()
        {
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
            if (_part != null) _part.Details.Hits++;
            CalculateScore();
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

        private float DetermineScore(bool raw = false)
        {
            var currentPercent = RaycastPoint.GetScore(_undershotPoints) * 0.8f +
                                 RaycastPoint.GetScore(RaycastPoint.Mode.Overshot) * 0.2f;
            
            return (currentPercent) / (MAX_SCORE) * 100.0f;
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
            public List<RaycastPoint> UndershotPoints { get; private set; }

            public IEnumerator Setup(ForgedPart part)
            {
                yield return null;
                
                UndershotPoints = new List<RaycastPoint>(GeneratePoints(part.DesiredMesh, RaycastPoint.Mode.Undershot));
            }
        }
    }
}
