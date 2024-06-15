using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Deformation;
using NeoForge.Deformation.Scoring;
using NeoForge.Input;
using SharedData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class StationController : MonoBehaviour
{
    [Tooltip(("Event that is called when the station is changed"))]
    [SerializeField] private UnityEvent<Station> _stationChanged;
    
    [FormerlySerializedAs("animator")]
    [Tooltip("The animator that controls the camera movement between stations")]
    [SerializeField] private Animator _animator;
    [Tooltip("The shared state that holds the current station of the player")]
    [SerializeField] private SharedState _currentStation;
    [Tooltip("The mesh similarity calculator that is used to compare the current mesh to the target mesh")]
    [SerializeField] private MeshSimilarityCalculator _meshSimilarityCalculator;

    [Header("UI Screens")]
    [Tooltip("The main UI elements that are displayed when the player is in a specific station")]
    [SerializeField] private SerializedDictionary<Station, GameObject> _uiElements;
    [Tooltip("A UI screen that is displayed after the user selects a part in the main heating UI")]
    [SerializeField] private GameObject _heatingUI;
    [Tooltip("A UI screen that prompts the user to return the part to the heating station")]
    [SerializeField] private GameObject _returnUI;
    [Tooltip("A UI screen that contains the heating and cooling buttons and a temperature dislpay")]    
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
    private ForgedPart _activePart;

    private void Awake()
    {
        FurnaceSpotLocator.SetInitialLocation(_initialFurnacePosition);
    }

    private void Start()
    {
        FindObjectsOfType<Deformable>().ToList().ForEach(deformable => deformable.Clicked += OnPartSelected);
        ControllerManager.OnChangeStation += ChangeCamera;
        _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));
        _activeUI = _uiElements[Station.Overview];
        _activeUI.SetActive(true);
        _currentStation.Value = Station.Overview;
    }

    private void OnDestroy()
    {
        FindObjectsOfType<Deformable>().ToList().ForEach(deformable => deformable.Clicked -= OnPartSelected);
    }

    private void Update()
    {
        if (_activePart == null) return;
        if (_activePart.CurrentState != ForgedPart.PartState.Ambient)
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
        _activePart.ChangePosition(_partPositions[newStation]);
        _currentStation.Value = newStation;
        ChangeCameraView(newStation);
        ChangeUI();
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
        _activePart = null;
        _currentStation.Value = Station.Heating;
        ChangeCameraView(Station.Heating);
        ChangeUI();
        CloseReturnUI();
    }
    
    /// <summary>
    /// Toggles the part state betweeen heating/cooling and ambient depending on the current station of the user.
    /// Also handles button text changes in the UI.
    /// </summary>
    public void ToggleHeatingOrCooling()
    {
        if(_activePart.CurrentState != ForgedPart.PartState.Ambient)
        {  
            _activePart.SetToAmbient(_partPositions[Station.Cooling]);
        }
        else if (_currentStation.Value == Station.Cooling)
        {
            _activePart.StartCooling(_inWaterPosition);
        }
        else
        {
            _activePart.StartHeating();
        }
        UpdateHeatButtonText();
    }

    private void UpdateHeatButtonText()
    {
        if(_activePart.CurrentState == ForgedPart.PartState.Ambient)
        {  
            _heatingCoolingButton.text = (_currentStation.Value == Station.Heating) 
                ?  "Place in Furnace"
                : "Place in Water";
        }
        else
        {

            _heatingCoolingButton.text = "Remove";
        }   
    }

    private void UpdateTemperatureDisplays()
    {
        //convert from k to c
        _temperatureDisplay.text = $"Part Temperature: {_activePart.Temperature - 273}C";
    }

    private void OnPartSelected(ForgedPart part)
    {
        //TODO: Raise part to show its been selected.
        //if the part is already active, open the send to UI
        if (part == _activePart && _currentStation.Value != Station.Heating)
        {
            OpenReturnUI();
            return;
        }
        
        if (_currentStation.Value != Station.Heating) return;
        //new part has been selected, can only happen in heating screen
        if (part != _activePart && _currentStation.Value == Station.Heating)
        {
            _activePart = part;
            _partPositions[Station.Heating] = _activePart.OutFurnacePosition;
            _meshSimilarityCalculator.SetPart(part);
            ChangeUI();
            UpdateTemperatureDisplays();
        }
    }

    private void OpenReturnUI()
    {
        _returnUI.SetActive(true);
    }
    
    private void ChangeCamera(Station station)
    {
        if (_currentStation.Value == Station.Overview) _currentStation.Value = station;
        ChangeUI();
        ChangeCameraView(station);
    }
    
    private void ChangeUI()
    {
        _activeUI.SetActive(false);
        _heatingUI.SetActive(false);
        _temperatureUI.SetActive(false);
        _activeUI = _uiElements[_currentStation.Value];
        if (_currentStation.Value == Station.Heating && _activePart != null)
        {
            _activeUI = _heatingUI;
            _temperatureUI.SetActive(true);
            UpdateHeatButtonText();
        }
        else if (_currentStation.Value == Station.Cooling)
        {
            _temperatureUI.SetActive(true);
            UpdateHeatButtonText();
        }
        _activeUI.SetActive(true);
    }

    private void ChangeCameraView(Station station)
    {
        switch (station)
        {
            case Station.Cooling:
                _animator.Play("Cooling Position");
                break;
            case Station.Forging:
                _animator.Play("Forging Position");
                break;
            case Station.Heating:
                _animator.Play("Heating Position");
                break;
            case Station.Overview:
                _animator.Play("Overview Position");
                break;
            case Station.Planning:
                _animator.Play("Planning Position");
                break;
            default:
                Debug.LogError("Station not found!");
                break;
        }
    }
}
