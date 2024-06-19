using System;

namespace NeoForge.Deformation.JSON
{
    [Serializable]
    public class JSONMaterial
    {
        public string Name;
        public float MaximumDeformation;
        public JSONMaterial(string name, float maximumDeformation)
        {
            Name = name;
            MaximumDeformation = maximumDeformation;
        }
    }
}