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
        
        /// <summary>
        /// Will be called when the temperature of the part changes
        /// </summary>
        public event Action<float> OnTemperatureChanged;
        
        /// <summary>
        /// Will be called when the state of the part changes
        /// </summary>
        public event Action<TemperatureState> OnStateChanged;

        [Tooltip("The temperature display for the part, must have a shader with a _Temperature property")]
        [SerializeField] private MeshRenderer _temperatureDisplay;
        [Tooltip("The current temperature of the furnace")]
        [SerializeField] private SharedFloat _furnaceTemperature;
        [Tooltip("Flames that will be displayed when the part is heating")]
        [SerializeField] private ParticleSystem _heatingFlames;
        [Tooltip("Smoke that will be displayed when the part is cooling")]
        [SerializeField] private ParticleSystem _coolingSmoke;
        
        private ForgeArea _currentForgeArea;
        
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
            UpdateParticles();
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
        public void SetStation(ForgeArea forgeArea)
        {
            _currentForgeArea = forgeArea;
            UpdateTemperatureDisplay();
            
            var canKeepHeating = CurrentState == TemperatureState.Heating 
                                 && forgeArea is ForgeArea.Heating or ForgeArea.Planning;
            
            var canKeepCooling = CurrentState == TemperatureState.Cooling 
                                 && forgeArea is ForgeArea.Cooling or ForgeArea.Planning;
            
            if (!canKeepHeating && !canKeepCooling)
            {
                SetState(TemperatureState.Ambient);
            }

            UpdateParticles();
        }
        
        /// <summary>
        /// Will swap the current state of the part. When the state is set to Heating the temperature will increase by
        /// TEMPERATURE_CHANGE_RATE. When the state is set to Cooling the temperature will decrease by TEMPERATURE_CHANGE_RATE.
        /// The Temperature will be clamped between ROOM_TEMPERATURE_KELVIN and the current furnace temperature.
        /// </summary>
        public void SetState(TemperatureState state)
        {
            CurrentState = state;
            UpdateParticles();
            OnStateChanged?.Invoke(CurrentState);
        }

        private void UpdateTemperatureDisplay()
        {
            var temperatureToDisplay = _currentForgeArea != ForgeArea.Planning ? Temperature : 0f;
            _temperatureDisplay.material.SetFloat(_temperatureProperty, temperatureToDisplay);
            UpdateParticles();
            OnTemperatureChanged?.Invoke(Temperature);
        }
        
        private void UpdateParticles()
        {
            if (CurrentState == TemperatureState.Heating && _currentForgeArea is ForgeArea.Heating)
            {
                if (_heatingFlames.isPlaying == false)
                {
                    _heatingFlames.Play();
                }
            }
            else
            {
                _heatingFlames.Stop();
            }
            
            if (CurrentState == TemperatureState.Cooling && _currentForgeArea is ForgeArea.Cooling 
                                                         && Temperature > ROOM_TEMPERATURE_KELVIN)
            {
                if (_coolingSmoke.isPlaying == false)
                {
                    _coolingSmoke.Play();
                }
            }
            else
            {
                _coolingSmoke.Stop();
            }
        }
    }
}