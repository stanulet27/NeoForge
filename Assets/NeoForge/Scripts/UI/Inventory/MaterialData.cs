using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Material", menuName = "Items/Material")]
    public class MaterialData : ScriptableObject 
    {
        [Tooltip("The name of the material.")]
        public string Name;
        
        [Tooltip("The description of the material.")]
        public string Description;
        
        [Tooltip("The icon that will be used in the journal.")]
        public Sprite Icon;
        
        [Tooltip("The minimum temperature that this material must be heated at.")]
        public float MinTemperature;
        
        [Tooltip("The maximum temperature that this material can be heated at before degrading.")]
        public float MaxTemperature;
        
        [Tooltip("The time it takes to cook this material when at desired temperature.")]
        public float CookingTime;
        
        [Tooltip("The rate at which this material cools down.")]
        public float CoolingRate;
        
        [Tooltip("The strength of the material when at normal temperature.")]
        public float NormalStrength;
        
        [Tooltip("The strength of the material when heated.")]
        public float HeatedStrength;
    }
}