using NeoForge.Deformation.JSON;
using UnityEditor;
using UnityEngine;

namespace NeoForge.Deformation.Editor
{
    public class MeshExporter : EditorWindow
    {
        [SerializeField] private Mesh _target;
        [SerializeField] private float _scale = 1f;
        
        [MenuItem("NeoForge/Export Mesh")]
        private static void ShowWindow()
        {
            GetWindow<MeshExporter>("Export Mesh");
        }
        
        private void OnGUI()
        {
            _target = EditorGUILayout.ObjectField("Target", _target, typeof(Mesh), true) as Mesh;
            _scale = EditorGUILayout.FloatField("Scale", _scale);
            
            if (GUILayout.Button("Export Mesh"))
            {
                ExportMeshAsJson();
            }
        }
        
        private void ExportMeshAsJson()
        {
            var meshData = new MeshData(_target, _scale);
            var json = JsonUtility.ToJson(meshData);
            var tempOutputPath = $"{Application.dataPath}/outputMesh.json";
            System.IO.File.WriteAllText(tempOutputPath, json);
            Debug.Log($"Mesh data exported to {tempOutputPath}");
        }
    }
}