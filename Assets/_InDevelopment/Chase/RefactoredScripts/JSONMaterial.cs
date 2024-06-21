using System;
using UnityEngine;

namespace NeoForge.Deformation.JSON
{
    [Serializable]
    public class JSONMaterial
    {
        [Tooltip("Name of the material")]
        public string Name;
        [Tooltip("The maximum deformation the material can take")]
        public float MaximumDeformation;
        public JSONMaterial(string name, float maximumDeformation)
        {
            Name = name;
            MaximumDeformation = maximumDeformation;
        }
    }
}