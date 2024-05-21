using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DeformationSystem
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Deformable : MonoBehaviour
    {
        [SerializeField] private Collider selector;
        [SerializeField] private float force = 1f;
        [SerializeField] private Transform cam;

        private string pythonPath;
        private string pythonScriptPath;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        private void Awake()
        {
            pythonPath = PythonFinder.FindPythonPath();
            if (string.IsNullOrEmpty(pythonPath))
            {
                UnityEngine.Debug.LogError("Python executable not found.");
            }

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "mesh_modifier.py");
            if (!File.Exists(pythonScriptPath))
            {
                UnityEngine.Debug.LogError($"Python script not found at: {pythonScriptPath}");
                return;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                ScaleMeshVertices();
            }
        }

        [ContextMenu("Scale Mesh Vertices")]
        public void ScaleMeshVertices()
        {
            var mesh = meshFilter.mesh;
            var intersections = mesh.vertices.Where(IsHit).Distinct().ToArray();
            var meshData = new MeshData(mesh, intersections, force, transform.worldToLocalMatrix.MultiplyVector(cam.forward));
            var tempOutputPath = Path.GetTempFileName();
            try
            {
                var start = Time.realtimeSinceStartup;
                CallPythonScript(pythonPath, pythonScriptPath, JsonUtility.ToJson(meshData), tempOutputPath);
                var end = Time.realtimeSinceStartup;
                Debug.Log($"Output in {end - start} seconds");
                
                if (File.Exists(tempOutputPath))
                {
                    RebuildMesh(tempOutputPath, mesh);
                    meshCollider.sharedMesh = mesh;
                    meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
                }
                else
                {
                    Debug.LogError($"Output file not found: {tempOutputPath}");
                }
            }
            finally
            {
                if (File.Exists(tempOutputPath))
                {
                    File.Delete(tempOutputPath);
                }
            }
        }
        
        private bool IsHit(Vector3 vertex)
        {
            return selector.bounds.Contains(transform.localToWorldMatrix.MultiplyPoint(vertex));
        }

        private static void RebuildMesh(string tempOutputPath, Mesh mesh)
        {
            var outputJson = File.ReadAllText(tempOutputPath);
            var modifiedMeshData = JsonUtility.FromJson<MeshData>(outputJson);

            // Convert float array back to Vector3 array
            var modifiedVertices = new Vector3[modifiedMeshData.vertices.Length / 3];
            for (int i = 0; i < modifiedVertices.Length; i++)
            {
                modifiedVertices[i] = new Vector3(
                    modifiedMeshData.vertices[i * 3],
                    modifiedMeshData.vertices[i * 3 + 1],
                    modifiedMeshData.vertices[i * 3 + 2]
                );
            }

            if (modifiedVertices.Length == 0)
            {
                UnityEngine.Debug.LogError("Modified mesh vertices are not properly assigned or empty.");
                return;
            }

            if (modifiedMeshData.triangles == null || modifiedMeshData.triangles.Length == 0)
            {
                UnityEngine.Debug.LogError("Modified mesh triangles are not properly assigned or empty.");
                return;
            }

            mesh.vertices = modifiedVertices;
            mesh.triangles = modifiedMeshData.triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void CallPythonScript(string pythonExePath, string scriptPath, string inputFilePath,
            string outputFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{scriptPath}\" \"{inputFilePath.Replace('\"', '?')}\" \"{outputFilePath}\"",
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process.WaitForExit();

            string error = process.StandardError.ReadToEnd();
            Debug.Log("Output: " + process.StandardOutput.ReadToEnd());

            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError($"Python error: {error}");
            }
        }
    }
}