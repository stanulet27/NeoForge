using System;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Material", menuName = "Items/Material")]
    public class MaterialData : ScriptableObject 
    {
        public enum JSONType
        {
            Copper = 0,
            Iron = 1,
        }

        [Tooltip("The name of the material.")]
        public string Name;
        
        [Tooltip("The description of the material.")]
        [TextArea(2, 5)]
        public string Description;

        [Tooltip("The icon that will be used in the journal.")]
        public Sprite Icon;
        
        [Tooltip("The temperature information for the material.")]
        public TemperatureInfo[] TemperatureInfos;
        
        [Tooltip("The strength of the material when at normal temperature.")]
        public float NormalStrength;
        
        [Tooltip("The strength of the material when heated.")]
        public float HeatedStrength;
        
        [Tooltip("The cost per 100lbs of the material.")]
        public int Cost;
        
        [Tooltip("The flavor text of the material.")]
        [TextArea(2, 5)]
        public string FlavorText;
        
        [Tooltip("The corresponding type in the database.")]
        public JSONType MaterialType;
        
        [Tooltip("The max amount of force that can be applied to the material.")]
        public float MaximumForce;
        
        [Serializable]
        public class TemperatureInfo
        {
            [Tooltip("The desired temperature for the entry.")]
            public float Temperature;
            [Tooltip("The minimum time to reach the desired temperature.")]
            public int MinimumTime;
            [Tooltip("The maximum time to reach the desired temperature.")]
            public int MaximumTime;
        }

        public static MaterialData CreateDefault()
        {
            var material = CreateInstance<MaterialData>();
            material.Name = "Material";
            material.Description = "Description";
            material.Icon = null;
            material.TemperatureInfos = Array.Empty<TemperatureInfo>();
            material.NormalStrength = 10;
            material.HeatedStrength = 5;
            material.Cost = 50;
            material.FlavorText = "Flavor Text";
            material.MaterialType = JSONType.Copper;
            material.MaximumForce = 10;
            return material;
        }
    }
}