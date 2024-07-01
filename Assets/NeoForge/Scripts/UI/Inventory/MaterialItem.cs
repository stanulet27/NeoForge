using NeoForge.Deformation.JSON;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Material")]
    public class MaterialItem : ItemBase
    {
        [Tooltip("The data for this material that represents its characteristics.")]
        public MaterialData Data;
        
        [Tooltip("The weight of the material in pounds.")]
        public float Weight;

        [Tooltip("The mesh that will be used to represent the material.")]
        public Mesh Mesh;

        private string _name;
        [SerializeField] private PartMeshDatabase.Shape TEMP_shape;
        [SerializeField] private PartMeshDatabase.Size TEMP_size;

        public override int Cost => Data.Cost * (int)(Weight / 100f);
        public override string Name => _name;
        public override string Description => Data.Description;

        [Button]
        private void Setup()
        {
            var values = CreateMaterial(Data, new PartMeshDetails(TEMP_shape, TEMP_size, Weight));
            Data = values.Data;
            Weight = values.Weight;
            Mesh = values.Mesh;
            _name = values._name;
        }
        
        public static MaterialItem CreateMaterial(MaterialData data, PartMeshDetails purchaseDetails)
        {
            var material = CreateInstance<MaterialItem>();
            material.Data = data;
            material.Weight = purchaseDetails.Weight;
            material.Mesh = PartMeshDatabase.Instance.GetPartMesh(purchaseDetails);
            material._name = $"{purchaseDetails.Size} {purchaseDetails.Weight}lbs {purchaseDetails.Shape} {data.Name} Bar";
            return material;
        }
    }
}