using System.Collections.Generic;
using System.Linq;
using NeoForge.Stations.Orders;
using NeoForge.UI.Inventory;
using NeoForge.UI.Scenes;
using NeoForge.Utilities;
using SharedData;
using UnityEngine;

namespace NeoForge.DaySystem
{
    public class DayManager : SingletonMonoBehaviour<DayManager>
    {
        [Tooltip("The shared int that tracks the current day.")]
        [SerializeField] private SharedInt _dateTracker;
        [Tooltip("If true, the day will not reset to 1 on start.")]
        [SerializeField] private bool _lockDayResetOnStart;

        /// <summary>
        /// The total renown earned since the start of the day.
        /// </summary>
        public int RenownEarnedToday { get; private set; }
        
        /// <summary>
        /// The total gold earned since the start of the day.
        /// </summary>
        public int GoldEarnedToday { get; private set; }
        
        /// <summary>
        /// The finished parts that have been completed today.
        /// </summary>
        public List<CompletedItem> CompletedPartsMadeToday { get; } = new();

        private void Start()
        {
            if (!_lockDayResetOnStart) ResetDay();

            SceneTools.OnSceneTransitionStart += SceneTools_OnSceneTransitionStart;
            InventorySystem.OnGoldChanged += InventorySystem_OnGoldChanged;
            InventorySystem.OnRenownChanged += InventorySystem_OnRenownChanged;
            InventorySystem.OnItemAdded += InventorySystem_OnItemAdded;
        }

        private void OnDestroy()
        {
            SceneTools.OnSceneTransitionStart -= SceneTools_OnSceneTransitionStart;
            InventorySystem.OnGoldChanged -= InventorySystem_OnGoldChanged;
            InventorySystem.OnRenownChanged -= InventorySystem_OnRenownChanged;
            InventorySystem.OnItemAdded -= InventorySystem_OnItemAdded;
        }

        private void SceneTools_OnSceneTransitionStart(int sceneIndex)
        {
            switch (sceneIndex)
            {
                case 0:
                    ResetDay();
                    break;
                case 1:
                    StartNewDay();
                    break;
            }
        }
        
        private void InventorySystem_OnGoldChanged(int gold)
        {
            Debug.Log("Gold changed: " + gold);
            GoldEarnedToday += gold;
        }
        
        private void InventorySystem_OnRenownChanged(int renown)
        {
            Debug.Log("Renown changed: " + renown);
            RenownEarnedToday += renown;
        }
        
        private void InventorySystem_OnItemAdded(ItemBase item)
        {
            Debug.Log("Item added: " + item.name);
            if (item is CompletedItem completedItem)
            {
                CompletedPartsMadeToday.Add(completedItem);
            }
        }
        
        private void StartNewDay()
        {
            Debug.Log("Starting new day");
            CompletedPartsMadeToday.Clear();
            RenownEarnedToday = 0;
            GoldEarnedToday = 0;
            
            OrderController.Instance.GetActiveOrders()
                .Where(x => x.DueDate == _dateTracker.Value)
                .ToList()
                .ForEach(x => OrderController.Instance.CompleteOrder(x));
            
            _dateTracker.Value++;
        }

        private void ResetDay()
        {
            StartNewDay();
            _dateTracker.Value = Mathf.Min(SceneTools.CurrentSceneIndex, 1);
        }
    }
}