using System;
using System.Linq;
using NeoForge.Dialogue;
using UnityEditor;
using UnityEngine;

namespace NeoForge.Orders.Editor
{
    public class OrderCompilier : EditorWindow
    {
        [SerializeField] private TextAsset tasksFile;
        
        [MenuItem("Tools/Task Compilier")]
        public static void ShowWindow()
        {
            GetWindow<OrderCompilier>("Task Compilier");
        }
        
        private void OnGUI()
        {
            tasksFile = (TextAsset) EditorGUILayout.ObjectField("Tasks File", tasksFile, typeof(TextAsset), false);
            
            if (GUILayout.Button("Compile")) Compile();
            if (GUILayout.Button("Verify")) Verify();
        }
        
        private void Compile()
        {
            var lines = tasksFile.text.Split('\n');
            var currentOrders = Resources.LoadAll<Order>("Orders");
            foreach (var line in lines[1..])
            {
                var task = currentOrders.FirstOrDefault(x => ContainsMatchingID(line, x));
                var taskAlreadyExists = task != default;
                if (!taskAlreadyExists)
                {
                    task = CreateInstance<Order>();
                }
                
                task.Setup(line);
                
                if (!taskAlreadyExists) AssetDatabase.CreateAsset(task, $"Assets/Resources/Orders/{task.name}.asset");
                else EditorUtility.SetDirty(task);
            }
        }

        private static bool ContainsMatchingID(string line, Order order)
        {
            return int.TryParse(line.Split(",")[0], out var id) && id == order.ID;
        }

        private void Verify()
        {
            var dialogueNodes = Resources.LoadAll<ConversationDataSO>("Dialogue");
            var orders = Resources.LoadAll<Order>("Orders");

            var orderDialogueEvents = dialogueNodes
                .SelectMany(x => x.Data.LeadsTo)
                .Where(x => x.IsEvent)
                .Select(x => x.NextID)
                .Where(x => x.StartsWith("GainOrder"))
                .ToList();
            
            var ordersEvents = orders.Select(x => new {giver = x.GiverName, craft = x.ObjectToCraft.ToString()}).ToList();
            
            foreach (var dialogueEvent in orderDialogueEvents)
            {
                var giver = dialogueEvent.Split("-")[1];
                var craft = dialogueEvent.Split("-")[2];
                var order = ordersEvents.FirstOrDefault(x => CheckIfMatch(dialogueEvent, x.giver, x.craft));
                if (order == null)
                {
                    Debug.LogError($"No order found for GainOrder-{giver}-{craft}");
                }
            }
            
            foreach (var order in ordersEvents)
            {
                var dialogueEvent = orderDialogueEvents.FirstOrDefault(x => CheckIfMatch(x, order.giver, order.craft));
                if (dialogueEvent == null)
                {
                    Debug.LogError($"No dialogue event found for SO_{order.giver}_{order.craft}_Order");
                }
            }
        }
        
        private bool CheckIfMatch(string eventTriggered, string giver, string part)
        {
            return eventTriggered.Split("-")[1].Equals(giver, StringComparison.InvariantCultureIgnoreCase) 
                   && eventTriggered.Split("-")[2].Equals(part, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}