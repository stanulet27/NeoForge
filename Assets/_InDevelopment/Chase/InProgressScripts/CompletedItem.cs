using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class CompletedItem : ItemBase
    {
        private string _name;
        private string _description;
        private PartCompletionScreen.ForgingResults _results;

        public Mesh Mesh => _results.PartMade;
        public override string Name => _name;
        public override string Description => "";
        public override int Cost => 0;
        
        public void SetItem(PartCompletionScreen.ForgingResults results)
        {
            _name = $"{results.PartName} ({results.Score})";
            _results = results;
        }
        
        public static void CreateItem(PartCompletionScreen.ForgingResults results)
        {
            var item = CreateInstance<CompletedItem>();
            item.SetItem(results);
            InventorySystem.Instance.AddItem(item);
        }
    }
}