using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Utilities;
using SharedData;
using Sirenix.Utilities;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class InventorySystem : SingletonMonoBehaviour<InventorySystem>
    {
        /// <summary>
        /// Invoked when the player's gold amount changes.
        /// Will pass the amount of gold gained or lost as a parameter.
        /// </summary>
        public static event Action<int> OnGoldChanged;
        
        /// <summary>
        /// Invoked when the player's renown amount changes.
        /// Will pass the amount of renown gained or lost as a parameter.
        /// </summary>
        public static event Action<int> OnRenownChanged;

        /// <summary>
        /// Will be invoked when an item is added to the inventory.
        /// The item that was added will be passed as a parameter.
        /// </summary>
        public static event Action<ItemBase> OnItemAdded;
        
        /// <summary>
        /// Will be invoked when an item is removed from the inventory.
        /// The item that was removed will be passed as a parameter.
        /// </summary>
        public static event Action<ItemBase> OnItemRemoved;

        [Tooltip("The current inventory of the player.")]
        [SerializeField] private SerializedDictionary<ItemBase, int> _inventory = new();
        [Tooltip("The current amount of gold the player has.")]
        [SerializeField] private int _currentGold;
        [Tooltip("The current renown the player has.")]
        [SerializeField] private int _currentRenown;
        
        private List<CompletedItem> _completedItems = new();
        
        /// <summary>
        /// The current total amount of gold the player has. This value will not be negative.
        /// </summary>
        public int CurrentGold 
        {
            get => _currentGold;
            set
            {
                OnGoldChanged?.Invoke(value - _currentGold);
                _currentGold = Mathf.Max(value, 0); 
            }
        }
        
        /// <summary>
        /// The current total amount of renown the player has. This value will not be negative.
        /// </summary>
        public int CurrentRenown 
        {
            get => _currentRenown;
            set
            {
                OnRenownChanged?.Invoke(value - _currentRenown);
                _currentRenown = Mathf.Max(value, 0); 
            }
        }

        /// <summary>
        /// Will clear the inventory and use the starting equipment to fill it.
        /// </summary>
        public void ResetInventory(int startingGold, int startingRenown, Dictionary<ItemBase, int> startingItems)
        {
            _currentGold = startingGold;
            _currentRenown = startingRenown;
            _inventory.Clear();
            startingItems.ForEach(x => _inventory.Add(x.Key, x.Value));
        }
        
        /// <summary>
        /// Will add the item to the inventory.
        /// If the item is already in the inventory, the quantity will be increased by 1.
        /// </summary>
        public void AddItem(ItemBase item)
        {
            if (_inventory.ContainsKey(item))
            {
                _inventory[item]++;
            }
            else
            {
                _inventory.Add(item, 1);
                if (item is CompletedItem completedItem)
                {
                    _completedItems.Add(completedItem);
                }
            }
            
            OnItemAdded?.Invoke(item);
        }
        
        /// <summary>
        /// Will decrease the quantity of the item in the inventory by 1.
        /// If the quantity reaches 0, the item will be removed from the inventory.
        /// </summary>
        public void RemoveItem(ItemBase item)
        {
            if (!_inventory.ContainsKey(item)) return;
            
            if (_inventory[item] <= 1)
            {
                _inventory.Remove(item);
            }
            else
            {
                _inventory[item]--;
            }
            
            OnItemRemoved?.Invoke(item);
        }

        /// <summary>
        /// Will count the amount of copies of the supplied item are in the inventory and return the result.
        /// </summary>
        public int CountItems(ItemBase item)
        {
            return _inventory.TryGetValue(item, out var value) ? value : 0;
        }
        
        /// <summary>
        /// Will return a dictionary of all items in the inventory that are of the specified type.
        /// </summary>
        /// <typeparam name="T">The item type to filter by</typeparam>
        public Dictionary<T, int> GetItems<T>() where T : ItemBase
        {
            return _inventory.Where(x => x.Key is T).ToDictionary(x => (T)x.Key, x => x.Value);
        }
    }
}