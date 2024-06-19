namespace NeoForge.Deformation.JSON
{
    [System.Serializable]
    public class EnvironmentOptions
    {
        public JSONMaterial[] Materials;
        public MeshData[] StartingMeshes;
        public MeshData[] TargetMeshes;
        public HammerData[] Hammers;

        public EnvironmentOptions(JSONMaterial[] materials, MeshData[] startingMeshes, MeshData[] targetMeshes, HammerData[] hammers)
        {
            Materials = materials;
            StartingMeshes = startingMeshes;
            TargetMeshes = targetMeshes;
            Hammers = hammers;
        }
    }
}