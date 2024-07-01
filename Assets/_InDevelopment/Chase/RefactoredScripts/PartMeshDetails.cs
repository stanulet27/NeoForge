namespace NeoForge.Deformation.JSON
{
    public class PartMeshDetails
    {
        public readonly PartMeshDatabase.Shape Shape;
        public readonly PartMeshDatabase.Size Size;
        public readonly float Weight;
        
        public PartMeshDetails(PartMeshDatabase.Shape shape, PartMeshDatabase.Size size, float weight)
        {
            Shape = shape;
            Size = size;
            Weight = weight;
        }
    }
}