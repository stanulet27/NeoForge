using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.UI.Inventory;
using NeoForge.Utilities.Movement;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class ForgingHUD : MonoBehaviour
    {
        [Header("UI Screens")]
        [Tooltip("The main UI elements that are displayed when the player is in a specific station")]
        [SerializeField] private SerializedDictionary<ForgeArea, GameObject> _uiElements;
        [Tooltip("A UI screen that is displayed after the user selects a part in the main heating UI")] 
        [SerializeField] private GameObject _heatingUI;
        [Tooltip("A UI screen that prompts the user to return the part to the heating station")] 
        [SerializeField] private GameObject _returnUI;
        [Tooltip("A UI screen that contains the heating and cooling buttons and a temperature display")]
        [SerializeField] private TemperatureHUD _temperatureHUD;
        [Tooltip("Will handle displaying the results for a part")]
        [SerializeField] private PartCompletionScreen _partCompletionScreen;

        private GameObject _activeUI;
        
        private void Awake()
        {
            _activeUI = _uiElements[ForgeArea.Overview];
        }

        /// <summary>
        /// Will open the main HUD for the forging station
        /// </summary>
        public void OpenUI()
        {
            _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));
        }

        /// <summary>
        /// Will swap the hud to match the new station.
        /// </summary>
        /// <param name="newForgeArea">Used for determining which HUD to display</param>
        /// <param name="hasActivePart">Used for determining selection HUD visibility</param>
        public void ChangeUI(ForgeArea newForgeArea, bool hasActivePart)
        {
            var atHeatingStation = newForgeArea == ForgeArea.Heating;
            var atCoolingStation = newForgeArea == ForgeArea.Cooling;
            var inTemperatureStation = atHeatingStation || atCoolingStation;
            var awaitingPartSelection = !atHeatingStation && !hasActivePart;
            var trackingPartTemperature = hasActivePart && inTemperatureStation;
            
            _activeUI.SetActive(false);
            _heatingUI.SetActive(false);

            _activeUI = awaitingPartSelection ? _uiElements[ForgeArea.Overview] : _uiElements[newForgeArea];
            
            if (trackingPartTemperature)
            {
                _activeUI = atHeatingStation ? _heatingUI : _activeUI;
                _temperatureHUD.Show(newForgeArea);
            }
            else
            {
                _temperatureHUD.Hide();
            }

            _activeUI.SetActive(true);
        }
        
        /// <summary>
        /// Will close all UI elements for the HUD
        /// </summary>
        public void CloseUI()
        {
            _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));
            _activeUI.SetActive(false);
        }
        
        /// <summary>
        /// Will set the part that the HUD is currently tracking
        /// </summary>
        /// <param name="part"></param>
        public void SetPart(ForgedPart part)
        {
            _temperatureHUD.UpdatePart(part);
        }
        
        /// <summary>
        /// Will display the completion screen for the part. Upon completion, the onReviewed action will be invoked.
        /// </summary>
        public void DisplayCompletionScreen(ForgingResults results, Action onReviewed)
        {
            _partCompletionScreen.Display(results, onReviewed);
        }
        
        /// <summary>
        /// Will toggle the return UI for the player to return the part to the heating station
        /// </summary>
        public void ToggleReturnUI(bool active)
        {
            _returnUI.SetActive(active);
        }
    }
}