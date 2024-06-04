namespace DeformationSystem
{
    [System.Serializable]
    public class EnvironmentChoices
    {
        public int StartMeshID;
        public int EndMeshID;
        public int MaterialID;
        public int HammerID;

        public EnvironmentChoices(int startMeshID, int endMeshID, int materialID, int hammerID)
        {
            StartMeshID = startMeshID;
            EndMeshID = endMeshID;
            MaterialID = materialID;
            HammerID = hammerID;
        }

    }
}