using UnityEditor;
using UnityEngine;

namespace NeoForge.Dialogue.Editor
{
    public class JsonDialogueConverterWindow : EditorWindow
    {
        [SerializeField, TextArea(10, 40)] string _box;

        [MenuItem("Tools/Json Converter")]
        private static void OpenWindow()
        {
            GetWindow<JsonDialogueConverterWindow>().Show();
        }

        // ReSharper disable Unity.PerformanceAnalysis - Calls TryToConvert() method only on button click
        private void OnGUI()
        {
            _box = EditorGUILayout.TextArea(_box, GUILayout.Height(position.height - 50));
            if (GUILayout.Button("Convert"))
            {
                TryToConvert();
            }
        }
        
        private void TryToConvert()
        {
            JsonDialogueConverter.ConvertToJson(_box);
        }
    }
}
