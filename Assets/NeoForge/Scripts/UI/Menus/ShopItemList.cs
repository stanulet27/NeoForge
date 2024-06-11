using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.UI.Inventory;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    [CreateAssetMenu(menuName = "Items/ShopItemList", fileName = "New ShopItemList")]
    public class ShopItemList : ScriptableObject
    {
        [Tooltip("The items that are available in this shop screen.")]
        [SerializeField] private List<ShopItem> _shopItems = new();
        
        /// <summary>
        /// Returns a list of items that are available in the shop. Items that have yet to be unlocked will not
        /// appear in this list.
        /// </summary>
        public List<ItemBase> ShopItems => _shopItems.Where(IsUnlocked).Select(x => x.Item).ToList();
        
        private static bool IsUnlocked(ShopItem item)
        {
            // Will be replaced with day checker
            return true;
        }
        
        [Serializable]
        private class ShopItem
        {
            [Tooltip("The item that will be available in the shop.")]
            public ItemBase Item;
            
            [Tooltip("The day the item will be unlocked.")]
            public int DayUnlocked;
        }
    }
}