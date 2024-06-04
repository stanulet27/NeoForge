namespace DeformationSystem
{
    [System.Serializable]
    public class EnvironmentOptions
    {
        public MaterialData[] Materials;
        public MeshData[] StartingMeshes;
        public MeshData[] TargetMeshes;
        public HammerData[] Hammers;

        public EnvironmentOptions(MaterialData[] materials, MeshData[] startingMeshes, MeshData[] targetMeshes, HammerData[] hammers)
        {
            Materials = materials;
            StartingMeshes = startingMeshes;
            TargetMeshes = targetMeshes;
            Hammers = hammers;
        }
    }
}