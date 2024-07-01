using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public abstract class ItemBase : ScriptableObject
    {
        public abstract int Cost { get; }
        
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