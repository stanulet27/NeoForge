using System;

namespace DeformationSystem
{
    [Serializable]
    public class MaterialData
    {
        public string Name;
        public float MaximumDeformation;
        public MaterialData(string name, float maximumDeformation)
        {
            Name = name;
            MaximumDeformation = maximumDeformation;
        }
    }
}