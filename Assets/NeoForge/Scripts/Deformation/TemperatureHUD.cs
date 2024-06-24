using CustomInspectors;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class TemperatureHUD : MonoBehaviour
    {
        [Header("Temperature Displays")]
        [Tooltip("The text display that shows the temperature of the part")]
        [SerializeField] private TMPro.TMP_Text _temperatureDisplay;
        [Tooltip("The button that toggles the part between heating and cooling")] 
        [SerializeField] private TMPro.TMP_Text _heatingCoolingButton;
        
        private ForgedPart _activePart;
        private ForgeArea _currentForgeArea;
        
        /// <summary>
        /// Will show the temperature HUD for the given station
        /// </summary>
        public void Show(ForgeArea forgeArea)
        {
            _currentForgeArea = forgeArea;
            UpdateHeatButtonText();
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Will hide the temperature HUD
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Will start tracking the temperature of the given part and display it in the HUD
        /// </summary>
        public void UpdatePart(ForgedPart part)
        {
            TearDownActivePart();
            SetupActivePart(part);
        }

        private void TearDownActivePart()
        {
            if (_activePart == null) return;
            
            _activePart.TemperatureHandler.OnTemperatureChanged -= UpdateTemperatureDisplays;
            _activePart.TemperatureHandler.OnStateChanged -= TemperatureHandler_OnStateChanged;
            _activePart = null;
        }

        private void SetupActivePart(ForgedPart part)
        {
            if (part == null) return;
            
            _activePart = part;
            
            _activePart.TemperatureHandler.OnTemperatureChanged += UpdateTemperatureDisplays;
            _activePart.TemperatureHandler.OnStateChanged += TemperatureHandler_OnStateChanged;
            UpdateTemperatureDisplays(_activePart.Temperature);
            UpdateHeatButtonText();
        }
        
        private void UpdateHeatButtonText()
        {
            if (_activePart == null) return;
            
            var awaitingStationAction = _activePart.CurrentState == TemperatureState.Ambient;
            var stationActionAvailable = _currentForgeArea == ForgeArea.Heating ? "Place in Furnace" : "Place in Water";
            
            _heatingCoolingButton.text = awaitingStationAction ? stationActionAvailable : "Remove";
        }
        
        private void TemperatureHandler_OnStateChanged(TemperatureState _)
        {
            UpdateHeatButtonText();
        }

        private void UpdateTemperatureDisplays(float newTemperature)
        {
            float ConvertKelvinToCelsius(float x) => x - 273;
            
            _temperatureDisplay.text = $"Part Temperature: {ConvertKelvinToCelsius(_activePart.Temperature)}C";
        }
    }
}