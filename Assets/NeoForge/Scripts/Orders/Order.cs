using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeoForge.Orders
{
    [CreateAssetMenu(fileName = "New Order", menuName = "Orders/Order")]
    public class Order : ScriptableObject
    {
        [Tooltip("The name of the order")]
        [SerializeField] private string _orderName;
        [Tooltip("The event that triggers the order")]
        [SerializeField] private int _triggerEvent;
        [Tooltip("The tasks that need to be completed")]
        [SerializeField] private List<Task> _tasks = new();
        
        private int _currentTaskIndex;
        public int TriggerEvent => _triggerEvent;

        /// <summary>
        /// Will parse the section and set up the order with the following format:
        /// TriggerEvent
        /// Name
        /// Task 1
        /// Task 2
        /// ...
        /// </summary>
        /// <param name="sectionToParse"></param>
        public void SetupOrder(string sectionToParse)
        {
            var lines = sectionToParse.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            _triggerEvent = int.Parse(lines[0].Trim());
            _orderName = lines[1].Trim();
            name = "Order" + _triggerEvent;
            _tasks.Clear();
            for (int i = 2; i < lines.Length; i++)
            {
                _tasks.Add(new Task(lines[i]));
            }
        }

        /// <summary>
        /// Resets the order to the first task
        /// </summary>
        public void Reset()
        {
            _currentTaskIndex = 0;
        }

        /// <summary>
        /// Converts the order to a string format to be displayed
        /// </summary>
        public string GetTaskDescription()
        {
            var description = _orderName;
            for (int i = 0; i < _currentTaskIndex; i++)
            {
                description += $"\n    -<s>{_tasks[i].Label}</s>";
            }
            if (_currentTaskIndex < _tasks.Count)
            {
                description += $"\n    -{_tasks[_currentTaskIndex].Label}";
            }

            return description;
        }
        
        /// <summary>
        /// Will check if the next task is completed based on the world state
        /// </summary>
        /// <param name="getWorldState">The method to determine the current world state</param>
        public bool CheckNextCompletionStatus(Func<string, int> getWorldState)
        {
            return _currentTaskIndex < _tasks.Count && _tasks[_currentTaskIndex].IsCompleted(getWorldState);
        }

        /// <summary>
        /// Will complete the current task and move to the next one
        /// </summary>
        public void CompleteCurrentTask()
        {
            _currentTaskIndex++;
        }
    }
}