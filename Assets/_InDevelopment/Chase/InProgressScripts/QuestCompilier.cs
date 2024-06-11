using System;
using System.Linq;
using NeoForge.Dialogue;
using NeoForge.Dialogue.Helper;
using NeoForge.Orders;
using UnityEditor;
using UnityEngine;

namespace _InDevelopment.Chase.InProgressScripts
{
    public class TaskCompilier : EditorWindow
    {
        [SerializeField] private TextAsset tasksFile;
        
        [MenuItem("Tools/Task Compilier")]
        public static void ShowWindow()
        {
            GetWindow<TaskCompilier>("Task Compilier");
        }
        
        private void OnGUI()
        {
            tasksFile = (TextAsset) EditorGUILayout.ObjectField("Tasks File", tasksFile, typeof(TextAsset), false);
            
            if (GUILayout.Button("Compile"))
            {
                Compile();
            }

            if (GUILayout.Button("Verify"))
            {
                Verify();
            }
        }
        
        private void Compile()
        {
            var tasks = tasksFile.text.Split('\n');
            var currentOrders = Resources.LoadAll<Order>("Orders");
            foreach (var task in tasks[1..])
            {
                var newTask = currentOrders.FirstOrDefault(x => int.TryParse(task.Split(",")[0], out var y) && y == x.ID);
                var taskAlreadyExists = newTask != default;
                if (!taskAlreadyExists)
                {
                    newTask = CreateInstance<Order>();
                }
                
                newTask.Setup(task);
                if (!taskAlreadyExists)
                    AssetDatabase.CreateAsset(newTask, $"Assets/Resources/Orders/{newTask.name}.asset");
                else
                    EditorUtility.SetDirty(newTask);
            }
        }

        private void Verify()
        {
            var dialogueNodes = Resources.LoadAll<ConversationDataSO>("Dialogue");
            var orders = Resources.LoadAll<Order>("Orders");

            var dialogueEvents = dialogueNodes
                .SelectMany(x => x.Data.LeadsTo)
                .Where(x => x.IsEvent)
                .Select(x => x.NextID)
                .Where(x => x.StartsWith("GainOrder"))
                .ToList();
            
            var ordersEvents = orders.Select(x => new {giver = x.GiverName, craft = x.ObjectToCraft.ToString()}).ToList();
            
            foreach (var dialogueEvent in dialogueEvents)
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
                var dialogueEvent = dialogueEvents.FirstOrDefault(x => CheckIfMatch(x, order.giver, order.craft));
                if (dialogueEvent == null)
                {
                    Debug.LogError($"No dialogue event found for SO_{order.giver}_{order.craft}_Order");
                }
            }
        }
        
        private bool CheckIfMatch(string eventTriggered, string giver, string part)
        {
            return eventTriggered.StartsWith("GainOrder") 
                   && eventTriggered.Split("-")[1].Equals(giver, StringComparison.InvariantCultureIgnoreCase) 
                   && eventTriggered.Split("-")[2].Equals(part, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}