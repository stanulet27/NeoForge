﻿using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Utilities;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class InventorySystem : SingletonMonoBehaviour<InventorySystem>
    {
        /// <summary>
        /// Will be invoked when an item is added to the inventory.
        /// The item that was added will be passed as a parameter.
        /// </summary>
        public Action<ItemBase> OnItemAdded;
        
        /// <summary>
        /// Will be invoked when an item is removed from the inventory.
        /// The item that was removed will be passed as a parameter.
        /// </summary>
        public Action<ItemBase> OnItemRemoved;

        [Tooltip("The current inventory of the player.")]
        [SerializeField] private SerializedDictionary<ItemBase, int> _inventory = new();
        [Tooltip("The current amount of gold the player has.")]
        [SerializeField] private int _currentGold;
        
        /// <summary>
        /// The current total amount of gold the player has. This value will not be negative.
        /// </summary>
        public int CurrentGold {get => _currentGold ; set => _currentGold = Mathf.Max(value, 0); }
        
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