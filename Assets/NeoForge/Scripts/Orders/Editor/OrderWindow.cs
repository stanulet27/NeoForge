using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NeoForge.Orders.Editor
{
    public class OrderWindow : EditorWindow
    {
        [SerializeField, TextArea(10, 40)] private string _sectionToParse;

        [MenuItem("Tools/Order Window")]
        private static void OpenWindow()
        {
            GetWindow<OrderWindow>().Show();
        }

        private void OnGUI()
        {
            _sectionToParse = EditorGUILayout.TextArea(_sectionToParse, GUILayout.Height(position.height - 50));
            if (GUILayout.Button("Parse Section"))
            {
                ParseSection();
            }
        }

        private void ParseSection()
        {
            ConvertToJson(_sectionToParse);
        }
        
        public static void ConvertToJson(string box)
        {
            Debug.Log("Converting to JSON: " + box);
            var questLines = box.Split("Order: ").Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (var questLine in questLines)
            {
                ConvertToOrder(questLine);
            }
        }
        
        private static void ConvertToOrder(string text)
        {
            var order = CreateInstance<Order>();
            order.SetupOrder(text);
            SaveOrder(order);
        }
        
        /*
         * Will see if Resources/Orders has a Order with the same name as the order being passed in
         * If it does, it will update the existing Order with the new data and then mark it as dirty
         * If it does not, it will create a new Order and save it to Resources/Orders
         */
        private static void SaveOrder(Order order)
        {
            var filePath = $"Assets/Resources/Orders/{order.name}.asset";
            if (System.IO.File.Exists(filePath))
            {
                var file = AssetDatabase.LoadAssetAtPath(filePath, typeof(Order)) as Order;
                EditorUtility.CopySerialized(order, file);
                EditorUtility.SetDirty(file);
            }
            else
            {
                AssetDatabase.CreateAsset(order, filePath);
            }
        }
    }
    
}