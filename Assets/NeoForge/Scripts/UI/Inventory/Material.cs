using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Material")]
    public class Material : ItemBase
    {
        [Tooltip("The data for this material that represents its characteristics.")]
        public MaterialData Data;
        
        [Tooltip("The model that will be used in the forging game")]
        public GameObject Model;
        
        [Tooltip("The weight of the material in pounds.")]
        public float Weight;
        
        public override int Cost => Data.Cost * (int)(Weight / 100f);
        public override string Name => $"{Data.Name} ({Weight}lbs)";
        public override string Description => Data.Description;
    }
}