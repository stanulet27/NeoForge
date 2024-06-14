using System;
using UnityEngine;


namespace SharedData
{
    /// <summary>
    /// The different stations that the player can be in when in the forging area
    /// </summary>
    public enum Station
    {
        Overview,
        Heating,
        Forging,
        Cooling,
        Planning
    }

    /// <summary>
    /// A shared data object that holds the current station that the player is in
    /// </summary>
    [CreateAssetMenu(menuName = "Shared Data/Station", fileName = "New Shared State")]
    public class SharedState : SharedDataBase<Station>
    {
        [SerializeField] private Station _currentState;

        public override Station Value
        {
            get => _currentState;
            set => _currentState = value;
        }
        
    }
}
