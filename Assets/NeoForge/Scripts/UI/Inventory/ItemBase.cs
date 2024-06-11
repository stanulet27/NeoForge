using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public abstract class ItemBase : ScriptableObject
    {
        [Tooltip("The cost of the item in the shop.")]
        public int Cost;
        
        /// <summary>
        /// The name that appears in the shop
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// The description that appears in the shop
        /// </summary>
        public abstract string Description { get; }
    }
}