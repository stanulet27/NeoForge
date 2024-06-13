﻿using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NeoForge.Utilities;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class InventorySystem : SingletonMonoBehaviour<InventorySystem>
    {
        public Action<ItemBase> OnItemAdded;
        public Action<ItemBase> OnItemRemoved;

        [SerializeField] private SerializedDictionary<ItemBase, int> _inventory = new();
        [SerializeField] private int _currentGold;
        
        public int CurrentGold {get => _currentGold ; set => _currentGold = Mathf.Max(value, 0); }
        
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
    }
}