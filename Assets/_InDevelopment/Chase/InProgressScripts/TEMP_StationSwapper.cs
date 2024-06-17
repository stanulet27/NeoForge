using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.Orders
{
    public class TEMP_StationSwapper : MonoBehaviour
    {
        [Serializable] public class StationSetup { public GameObject Station; public Camera Camera; }
        
        [SerializeField] private List<StationSetup> _stationSetups;
        
        private int _currentStationIndex;

        private void Start()
        {
            _stationSetups.ForEach(x => x.Camera.enabled = false);
            EnterStation(_stationSetups[_currentStationIndex]);
        }

        private void Update()
        {
            if (!UnityEngine.Input.GetKeyDown(KeyCode.T)) return;

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
    }
}