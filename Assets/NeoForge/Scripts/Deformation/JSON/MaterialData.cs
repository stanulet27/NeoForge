using System;

namespace NeoForge.Deformation.JSON
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