namespace DeformationSystem
{
    [System.Serializable]
    public class HammerData 
    {
        public string Name;
        public float SizeX;
        public float SizeY;

        public HammerData(string name, float sizeX, float sizeY)
        { 
            Name = name;
            SizeX = sizeX;
            SizeY = sizeY;
        }
    }
}