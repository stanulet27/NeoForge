using NeoForge.Deformation.JSON;
using UnityEditor;
using UnityEngine;

namespace NeoForge.Deformation.Editor
{
    public class MeshExporter : EditorWindow
    {
        private const string DEFAULT_FILE_NAME = "outputMesh.json";
        
        [Tooltip("The mesh to export")]
        [SerializeField] private Mesh _target;
        [Tooltip("The amount to scale the mesh's vertices by before exporting")]
        [SerializeField] private float _scale = 1f;
        [Tooltip("The name of the output file")]
        [SerializeField] private string _outputFileName = DEFAULT_FILE_NAME;
        
        [MenuItem("Tools/Export Mesh")]
        private static void ShowWindow()
        {
            var x = GetWindow<MeshExporter>("Export Mesh");
            x._outputFileName = DEFAULT_FILE_NAME;
        }
        
        private void OnGUI()
        {
            _target = EditorGUILayout.ObjectField("Target", _target, typeof(Mesh), true) as Mesh;
            _scale = EditorGUILayout.FloatField("Scale", _scale);
            _outputFileName = EditorGUILayout.TextField("Output File Name", _outputFileName);
            
            if (GUILayout.Button("Export Mesh"))
            {
                ExportMeshAsJson();
            }
        }
        
        private void ExportMeshAsJson()
        {
            var meshData = new MeshData(_target, _scale);
            var json = JsonUtility.ToJson(meshData);
            var outputFile = _outputFileName.EndsWith(".json") ? _outputFileName : $"{_outputFileName}.json";
            var tempOutputPath = $"{Application.dataPath}/{outputFile}";
            System.IO.File.WriteAllText(tempOutputPath, json);
            Debug.Log($"Mesh data exported to {tempOutputPath}");
        }
    }
}