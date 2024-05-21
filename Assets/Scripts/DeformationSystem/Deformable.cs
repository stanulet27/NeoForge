using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace DeformationSystem
{
    public static class PythonFinder
    {
        public static string FindPythonPath()
        {
            // Check the PATH environment variable for Python installations
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                string[] paths = pathEnv.Split(Path.PathSeparator);
                foreach (string path in paths)
                {
                    string pythonPath = Path.Combine(path, "python.exe");
                    if (File.Exists(pythonPath))
                    {
                        return pythonPath;
                    }
                }
            }

            // Check common installation paths for Python on Windows
            string[] commonPythonPaths = {
                @"C:\Python39\python.exe", // Python 3.9 default installation path
                @"C:\Python38\python.exe", // Python 3.8 default installation path
                @"C:\Python37\python.exe", // Python 3.7 default installation path
                @"C:\Users\" + Environment.UserName +
                @"\AppData\Local\Programs\Python\Python39\python.exe", // Python installed in user profile
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python38\python.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python37\python.exe",
                @"C:\Program Files\Python39\python.exe",
                @"C:\Program Files\Python38\python.exe",
                @"C:\Program Files\Python37\python.exe",
                @"C:\Program Files (x86)\Python39\python.exe",
                @"C:\Program Files (x86)\Python38\python.exe",
                @"C:\Program Files (x86)\Python37\python.exe"
            };

            foreach (string commonPath in commonPythonPaths)
            {
                if (File.Exists(commonPath))
                {
                    return commonPath;
                }
            }

            return null;
        }
    }

    [RequireComponent(typeof(MeshFilter))]
    public class Deformable : MonoBehaviour
    {
        MeshFilter meshFilter;

        [ContextMenu("Deform")]
        public void Deform()
        {
            meshFilter = GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;

            var vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= 1.5f;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void Update()
        {
            // On Q Pressed execute Scale Mesh Vertices
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ScaleMeshVertices();
            }
        }

        [ContextMenu("Scale Mesh Vertices")]
    public void ScaleMeshVertices()
    {
        string pythonExePath = PythonFinder.FindPythonPath();
        if (string.IsNullOrEmpty(pythonExePath))
        {
            UnityEngine.Debug.LogError("Python executable not found.");
            return;
        }
        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            UnityEngine.Debug.LogError("MeshFilter component not found on this GameObject.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        if (mesh == null)
        {
            UnityEngine.Debug.LogError("Mesh is not assigned to the MeshFilter.");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        if (vertices == null || vertices.Length == 0)
        {
            UnityEngine.Debug.LogError("Mesh vertices are not properly assigned or empty.");
            return;
        }

        if (triangles == null || triangles.Length == 0)
        {
            UnityEngine.Debug.LogError("Mesh triangles are not properly assigned or empty.");
            return;
        }

        // Convert Vector3 array to float array
        float[] flattenedVertices = new float[vertices.Length * 3];
        for (int i = 0; i < vertices.Length; i++)
        {
            flattenedVertices[i * 3] = vertices[i].x;
            flattenedVertices[i * 3 + 1] = vertices[i].y;
            flattenedVertices[i * 3 + 2] = vertices[i].z;
        }

        // Create the data structure to send to Python
        MeshData meshData = new MeshData
        {
            vertices = flattenedVertices,
            triangles = triangles
        };

        string inputJson = JsonUtility.ToJson(meshData);
        string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "mesh_modifier.py");

        // Ensure the paths are correct
        if (!File.Exists(pythonExePath))
        {
            UnityEngine.Debug.LogError($"Python executable not found at: {pythonExePath}");
            return;
        }

        if (!File.Exists(pythonScriptPath))
        {
            UnityEngine.Debug.LogError($"Python script not found at: {pythonScriptPath}");
            return;
        }

        // Create temporary files for input and output
        string tempInputPath = Path.GetTempFileName();
        string tempOutputPath = Path.GetTempFileName();

        try
        {
            // Write input JSON to temporary file
            File.WriteAllText(tempInputPath, inputJson);

            // Call the Python script with the temporary files
            string output = CallPythonScript(pythonExePath, pythonScriptPath, tempInputPath, tempOutputPath);

            // Debug log the output from the Python script
            UnityEngine.Debug.Log(output);

            // Read the output JSON from the temporary file
            if (File.Exists(tempOutputPath))
            {
                string outputJson = File.ReadAllText(tempOutputPath);

                MeshData modifiedMeshData = JsonUtility.FromJson<MeshData>(outputJson);

                // Convert float array back to Vector3 array
                Vector3[] modifiedVertices = new Vector3[modifiedMeshData.vertices.Length / 3];
                for (int i = 0; i < modifiedVertices.Length; i++)
                {
                    modifiedVertices[i] = new Vector3(
                        modifiedMeshData.vertices[i * 3],
                        modifiedMeshData.vertices[i * 3 + 1],
                        modifiedMeshData.vertices[i * 3 + 2]
                    );
                }

                if (modifiedVertices == null || modifiedVertices.Length == 0)
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
            else
            {
                UnityEngine.Debug.LogError($"Output file not found: {tempOutputPath}");
            }
        }
        finally
        {
            // Clean up temporary files
            if (File.Exists(tempInputPath))
            {
                File.Delete(tempInputPath);
            }
            if (File.Exists(tempOutputPath))
            {
                File.Delete(tempOutputPath);
            }
        }
    }

    private string CallPythonScript(string pythonExePath, string scriptPath, string inputFilePath, string outputFilePath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonExePath,
            Arguments = $"\"{scriptPath}\" {inputFilePath} {outputFilePath}",
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError($"Python error: {error}");
            }

            return output;
        }
    }

    [System.Serializable]
    public class MeshData
    {
        public float[] vertices;
        public int[] triangles;
    }
    }
}