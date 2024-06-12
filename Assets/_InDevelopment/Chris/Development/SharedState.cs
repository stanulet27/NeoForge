using System;
using UnityEngine;


namespace SharedData
{
    [Serializable]
    public enum StationState
    {
        Overview,
        Planning,
        Forging,
        Heating,
        Cooling
    }

    [CreateAssetMenu(menuName = "Shared Data/Station", fileName = "New Shared State")]
    public class SharedState : SharedDataBase<StationState>
    {
        
        [SerializeField] private StationState _currentState;

        public override StationState Value
        {
            get => _currentState;
            set => _currentState = value;
        }
        
    }
}
