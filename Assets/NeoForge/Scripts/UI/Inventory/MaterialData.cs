using System;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Material", menuName = "Items/Material")]
    public class MaterialData : ScriptableObject 
    {
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
    }
}