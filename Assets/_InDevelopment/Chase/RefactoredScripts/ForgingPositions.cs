using System;
using AYellowpaper.SerializedCollections;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    [Serializable]
    public class ForgingPositions
    {
        [Header("Part Positions")]
        [Tooltip("The transform for the part at each station")] 
        public SerializedDictionary<ForgeArea, Transform> PartPositions;
        [Tooltip("The position of the part when it is in the water")] 
        public Transform InWaterPosition;
        [Tooltip("The initial position of the part in the heating station")] 
        public Transform InitialFurnacePosition;
        
        public ForgingPositions(ForgingPositions positions)
        {
            PartPositions = new SerializedDictionary<ForgeArea, Transform>(positions.PartPositions);
            InWaterPosition = positions.InWaterPosition;
            InitialFurnacePosition = positions.InitialFurnacePosition;
        }
        
        /// <summary>
        /// Will set the initial location of the furnace spot locator
        /// </summary>
        public void Initialize()
        {
            FurnaceSpotLocator.SetInitialLocation(InitialFurnacePosition);
        }
    }
}