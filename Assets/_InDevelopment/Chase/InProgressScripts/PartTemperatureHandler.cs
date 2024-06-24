using System;
using System.Collections;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class PartTemperatureHandler : MonoBehaviour
    {
        private const float ROOM_TEMPERATURE_KELVIN = 290f;
        private const float TEMPERATURE_CHANGE_RATE = 1f;
        private const string TEMPERATURE = "_Temperature";
        private static readonly int _temperatureProperty = Shader.PropertyToID(TEMPERATURE);
        
        [Tooltip("The temperature display for the part, must have a shader with a _Temperature property")]
        [SerializeField] private MeshRenderer _temperatureDisplay;
        [Tooltip("The current temperature of the furnace")]
        [SerializeField] private SharedFloat _furnaceTemperature;
        
        private Station _currentStation;
        
        /// <summary>
        /// The temperature of the part in kelvin
        /// </summary>
        public float Temperature { get; private set; }

        /// <summary>
        /// The current state of the part
        /// </summary>
        public TemperatureState CurrentState { get; private set; }
        
        private void OnEnable()
        {
            CurrentState = TemperatureState.Ambient;
            Temperature = ROOM_TEMPERATURE_KELVIN;
            _temperatureDisplay.material.SetFloat(_temperatureProperty, Temperature);
        }
        
        private void Update()
        {
            if (CurrentState == TemperatureState.Ambient) return;
            Temperature += CurrentState == TemperatureState.Heating ? TEMPERATURE_CHANGE_RATE : -TEMPERATURE_CHANGE_RATE;
            Temperature = Mathf.Clamp(Temperature, ROOM_TEMPERATURE_KELVIN, _furnaceTemperature);
            UpdateTemperatureDisplay();
        }
        
        /// <summary>
        /// Will swap the current station of the part. When the station is set to Planning the temperature display will
        /// be hidden from the user. When the station is not a temperature station nor planning, the part will enter
        /// the Ambient state.
        /// </summary>
        /// <param name="station"></param>
        public void SetStation(Station station)
        {
            _currentStation = station;
            UpdateTemperatureDisplay();
            
            var canKeepHeating = CurrentState == TemperatureState.Heating && station is Station.Heating or Station.Planning;
            var canKeepCooling = CurrentState == TemperatureState.Cooling && station is Station.Cooling or Station.Planning;
            if (!canKeepHeating && !canKeepCooling)
            {
                SetState(TemperatureState.Ambient);
            }
        }
        
        /// <summary>
        /// Will swap the current state of the part. When the state is set to Heating the temperature will increase by
        /// TEMPERATURE_CHANGE_RATE. When the state is set to Cooling the temperature will decrease by TEMPERATURE_CHANGE_RATE.
        /// The Temperature will be clamped between ROOM_TEMPERATURE_KELVIN and the current furnace temperature.
        /// </summary>
        public void SetState(TemperatureState state)
        {
            CurrentState = state;
        }

        private void UpdateTemperatureDisplay()
        {
            _temperatureDisplay.material.SetFloat(_temperatureProperty, _currentStation != Station.Planning ? Temperature : 0);
        }
    }
}