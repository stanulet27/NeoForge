using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Deformation.Scoring;
using NeoForge.Input;
using NeoForge.Orders;
using SharedData;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.Deformation
{
    public class StationController : MonoBehaviour, IStation
    {
        [Header("Events")]
        [Tooltip("Event that is called when the station is changed")] 
        [SerializeField] private UnityEvent<Station> _stationChanged;
        
        [Header("References")]
        [Tooltip("The animator that controls the camera movement between stations")]
        [SerializeField] private Animator _animator;
        [Tooltip("The shared state that holds the current station of the player")] 
        [SerializeField] private SharedState _currentStation;
        [Tooltip("The mesh similarity calculator that is used to compare the current mesh to the target mesh")]
        [SerializeField] private MeshSimilarityCalculator _meshSimilarityCalculator;
        [Tooltip("Will handle displaying the results for a part")]
        [SerializeField] private PartCompletionScreen _partCompletionScreen;
        [Tooltip("The deformation handler")]
        [SerializeField] private DeformationHandler _deformationHandler;

        [Header("UI Screens")]
        [Tooltip("The main UI elements that are displayed when the player is in a specific station")]
        [SerializeField] private SerializedDictionary<Station, GameObject> _uiElements;
        [Tooltip("A UI screen that is displayed after the user selects a part in the main heating UI")] 
        [SerializeField] private GameObject _heatingUI;
        [Tooltip("A UI screen that prompts the user to return the part to the heating station")] 
        [SerializeField] private GameObject _returnUI;
        [Tooltip("A UI screen that contains the heating and cooling buttons and a temperature display")]
        [SerializeField] private GameObject _temperatureUI;

        [Header("Part Positions")]
        [Tooltip("The transform for the part at each station")] 
        [SerializeField] private SerializedDictionary<Station, Transform> _partPositions;
        [Tooltip("The position of the part when it is in the water")] 
        [SerializeField] private Transform _inWaterPosition;
        [Tooltip("The initial position of the part in the heating station")] 
        [SerializeField] private Transform _initialFurnacePosition;
        
        [Header("Temperature Displays")]
        [Tooltip("The text display that shows the temperature of the part")]
        [SerializeField] private TMPro.TMP_Text _temperatureDisplay;
        [Tooltip("The button that toggles the part between heating and cooling")] 
        [SerializeField] private TMPro.TMP_Text _heatingCoolingButton;

        private GameObject _activeUI;
        private bool _aPartIsActive;
        private ForgedPart _activePart;
        private List<Deformable> _parts;

        private void Awake()
        {
            FurnaceSpotLocator.SetInitialLocation(_initialFurnacePosition);
            _parts = FindObjectsOfType<Deformable>(true).ToList();
            _activeUI = _uiElements[Station.Overview];
        }

        private void OnDestroy()
        {
            _parts.ForEach(part => part.Clicked -= OnPartSelected);
            ControllerManager.OnChangeStation -= ChangeCamera;
        }

        private void Update()
        {
            if (_aPartIsActive && _activePart.CurrentState != ForgedPart.PartState.Ambient)
            {
                UpdateTemperatureDisplays();
            }
        }

        /// <summary>
        /// Changes the current station the player is in and swaps the camera and UI to match.
        /// </summary>
        /// <param name="newStation"></param>
        public void ChangeStation(Station newStation)
        {
            if (newStation == Station.Planning) _meshSimilarityCalculator.CalculateScore();
            _activePart.ChangePosition(_partPositions[newStation]);
            _activePart.ToggleMovement(newStation is Station.Forging or Station.Planning);
            _activePart.SetStation(newStation);
            _currentStation.Value = newStation;
            ChangeCameraView(newStation);
            ChangeUI();
            UpdateHeatButtonText();
        }


        /// <summary>
        /// Closes the UI screen that prompts the user to return the part to the heating station
        /// </summary>
        public void CloseReturnUI()
        {
            _returnUI.SetActive(false);
        }

        /// <summary>
        /// Sends the player and the part back to the heating table
        /// </summary>
        public void ReturnPartToHeating()
        {
            _activePart.ChangePosition(_partPositions[Station.Heating]);
            _activePart.SetStation(Station.Heating);
            SetActivePart(null);
            ChangeCamera(Station.Heating);
            CloseReturnUI();
        }

        /// <summary>
        /// Toggles the part state between heating/cooling and ambient depending on the current station of the user.
        /// Also handles button text changes in the UI.
        /// </summary>
        public void ToggleHeatingOrCooling()
        {
            var partIsAmbient = _activePart.CurrentState == ForgedPart.PartState.Ambient;
            var atHeatingStation = _currentStation.Value == Station.Heating;
            
            if (!partIsAmbient) _activePart.SetToAmbient(_partPositions[Station.Cooling]);
            else if (atHeatingStation) _activePart.StartHeating();
            else _activePart.StartCooling(_inWaterPosition);

            UpdateHeatButtonText();
        }
        
        public void EnterStation()
        {
            _parts.ForEach(deformable => deformable.Clicked += OnPartSelected);
            ControllerManager.OnChangeStation += ChangeCamera;
            _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));

            ChangeCamera(Station.Overview);
        }

        public void ExitStation()
        {
            _parts.ForEach(deformable => deformable.Clicked -= OnPartSelected);
            ControllerManager.OnChangeStation -= ChangeCamera;
            _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));
            
            if (_aPartIsActive) ReturnPartToHeating();
            ChangeCamera(Station.Overview);
            _activeUI.SetActive(false);
        }
        
        public void SubmitPart()
        {
            var part = _activePart;
            var results = new PartCompletionScreen.ForgingResults(_meshSimilarityCalculator, part.Details, part.Mesh);
            
            _partCompletionScreen.Display(results, OnPartReviewed);
        }

        private void OnPartReviewed()
        {
            _activePart.gameObject.SetActive(false);
            SetActivePart(null);
            ChangeCamera(Station.Overview);
            _meshSimilarityCalculator.PostScore();
        }
        
        private void SetActivePart(ForgedPart part)
        {
            if (_activePart != null) _activePart.ToggleSelection(false);
            _activePart = part;
            _aPartIsActive = part != null;
            if (_aPartIsActive) _activePart.ToggleSelection(true);
            if (_aPartIsActive) StartCoroutine(_deformationHandler.PrepareEnvironment(part));
            UpdateHeatButtonText();
        }

        private void UpdateHeatButtonText()
        {
            if (!_aPartIsActive) return;
            var stationActionAvailable = _currentStation.Value == Station.Heating ? "Place in Furnace" : "Place in Water";
            var awaitingStationAction = _activePart.CurrentState == ForgedPart.PartState.Ambient;
            
            _heatingCoolingButton.text = awaitingStationAction ? stationActionAvailable : "Remove";
        }

        private void UpdateTemperatureDisplays()
        {
            float ConvertKelvinToCelsius(float x) => x - 273;
            
            _temperatureDisplay.text = $"Part Temperature: {ConvertKelvinToCelsius(_activePart.Temperature)}C";
        }

        private void OnPartSelected(ForgedPart part)
        {
            var newPartSelected = part != _activePart && _currentStation.Value == Station.Heating;
            if (newPartSelected) SwapToPart(part);
            else if (_currentStation != Station.Heating) OpenReturnUI();
        }

        private void SwapToPart(ForgedPart part)
        {
            //TODO: Raise part to show its been selected.
            SetActivePart(part);
            _partPositions[Station.Heating] = _activePart.OutFurnacePosition;
            _meshSimilarityCalculator.SetPart(part);
            ChangeUI();
            UpdateTemperatureDisplays();
        }

        private void OpenReturnUI()
        {
            _returnUI.SetActive(true);
        }

        private void ChangeCamera(Station station)
        {
            if (_currentStation.Value == Station.Overview) _currentStation.Value = station;
            if (!_aPartIsActive) _currentStation.Value = station;
            ChangeUI();
            ChangeCameraView(station);
        }

        private void ChangeUI()
        {
            var atHeatingStation = _currentStation.Value == Station.Heating;
            var atCoolingStation = _currentStation.Value == Station.Cooling;
            var inTemperatureStation = atHeatingStation || atCoolingStation;
            var awaitingPartSelection = !atHeatingStation && !_aPartIsActive;
            var trackingPartTemperature = _aPartIsActive && inTemperatureStation;
            
            _activeUI.SetActive(false);
            _temperatureUI.SetActive(trackingPartTemperature);
            _heatingUI.SetActive(false);

            _activeUI = awaitingPartSelection ? _uiElements[Station.Overview] : _uiElements[_currentStation.Value];

            if (trackingPartTemperature)
            {
                _activeUI = atHeatingStation ? _heatingUI : _activeUI;
                UpdateTemperatureDisplays();
            }

            _activeUI.SetActive(true);
        }

        private void ChangeCameraView(Station station)
        {
            _animator.Play($"{station.ToString()} Position");
        }
    }
}