namespace DeformationSystem
{
    public class MeshObject
    {
        public int ID;
        public MeshData meshData;

        public MeshObject(int ID, MeshData meshData)
        {
            this.ID = ID;
            this.meshData = meshData;
        }   
    }
}