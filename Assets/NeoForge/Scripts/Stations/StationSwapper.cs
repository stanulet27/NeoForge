using System;
using System.Collections.Generic;
using NeoForge.Input;
using UnityEngine;

namespace NeoForge.Stations
{
    public class StationSwapper : MonoBehaviour
    {
        [Tooltip("The list of stations to swap between.")]
        [SerializeField] private List<StationSetup> _stationSetups;
        
        private int _currentStationIndex;

        private void Start()
        {
            _stationSetups.ForEach(x => x.Camera.enabled = false);
            EnterStation(_stationSetups[_currentStationIndex]);
            ControllerManager.OnSwapArea += OnSwapStation;
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnSwapArea -= OnSwapStation;
        }

        private void OnSwapStation()
        {
            ExitStation(_stationSetups[_currentStationIndex]);
            _currentStationIndex = (_currentStationIndex + 1) % _stationSetups.Count;
            EnterStation(_stationSetups[_currentStationIndex]);
        }

        private static void EnterStation(StationSetup station)
        {
            var stationComponent = station.Station.GetComponent<IStation>();
            station.Camera.enabled = true;
            stationComponent?.EnterStation();
        }
        
        private static void ExitStation(StationSetup station)
        {
            var stationComponent = station.Station.GetComponent<IStation>();
            station.Camera.enabled = false;
            stationComponent?.ExitStation();
        }

        [Serializable]
        public class StationSetup
        {
            [Tooltip("The IStation GameObject to swap to.")]
            public GameObject Station; 
            [Tooltip("The camera used to view this station.")]
            public Camera Camera;
        }
    }
}
