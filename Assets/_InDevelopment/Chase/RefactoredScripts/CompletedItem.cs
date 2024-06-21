using NeoForge.Deformation;
using NeoForge.Deformation.JSON;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class CompletedItem : ItemBase
    {
        private string _name;
        private string _description;
        private ForgingResults _results;

        /// <summary>
        /// The desired part to be crafted
        /// </summary>
        public CraftableParts Goal => _results.PartGoal;
        
        /// <summary>
        /// The users score, note that lower int value means higher score
        /// </summary>
        public PartScore Score => _results.Score;
        
        /// <summary>
        /// The final mesh after performing the user specified hits
        /// </summary>
        public Mesh Mesh => _results.PartMade;
        
        public override string Name => _name;
        public override string Description => "";
        public override int Cost => 0;

        /// <summary>
        /// Will create a new scriptable object that will be added to the inventory.
        /// </summary>
        /// <param name="results">The results to use to define the objects properties</param>
        public static void CreateItem(ForgingResults results)
        {
            var item = CreateInstance<CompletedItem>();
            item._name = $"{results.PartName} ({results.Score})";
            item._results = results;
            InventorySystem.Instance.AddItem(item);
        }
    }
}