namespace DeformationSystem
{
    public class MeshObject
    {
        public int ID;
        public MeshData MeshData;

        public MeshObject(int id, MeshData meshData)
        {
            ID = id;
            MeshData = meshData;
        }   
    }
}