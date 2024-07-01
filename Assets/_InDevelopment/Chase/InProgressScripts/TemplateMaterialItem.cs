using NeoForge.Deformation.JSON;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(menuName = "Items/Template Material", fileName = "New Template Material", order = 0)]
    public class TemplateMaterialItem : ItemBase
    {
        [SerializeField] private PartMeshDatabase.Shape _shape;
        [SerializeField] private PartMeshDatabase.Size _size;
        [SerializeField] private MaterialData _materialData;
        
        public PartMeshDetails PartMeshDetails(float weight) => new (_shape, _size, weight);
        public MaterialData MaterialData => _materialData;
        public override int Cost => _materialData.Cost;
        public override string Name => $"{_size} {_shape}";
        public override string Description => _materialData.Description + $"\nPrice is displayed per pound.";
    }
}