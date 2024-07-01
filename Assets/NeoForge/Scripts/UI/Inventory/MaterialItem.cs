using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Material")]
    public class MaterialItem : ItemBase
    {
        public enum StartingOption
        {
            BasicCube = 0,
            BasicBar = 1,
        }
        
        [Tooltip("The data for this material that represents its characteristics.")]
        public MaterialData Data;
        
        [Tooltip("The mesh that will be obtained from the database")]
        public StartingOption StartingMesh;
        
        [Tooltip("The weight of the material in pounds.")]
        public float Weight;
        
        public override int Cost => Data.Cost * (int)(Weight / 100f);
        public override string Name => $"{Data.Name} ({Weight}lbs)";
        public override string Description => Data.Description;
    }
}