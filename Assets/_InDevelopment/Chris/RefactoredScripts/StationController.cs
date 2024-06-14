using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Deformation;
using NeoForge.Deformation.Scoring;
using NeoForge.Input;
using SharedData;
using UnityEngine;
using UnityEngine.Events;

public class StationController : MonoBehaviour
{
    [SerializeField] private UnityEvent<Station> _stationChanged;
    
    [SerializeField] private Animator animator;
    [SerializeField] private SharedState _currentStation;
    [SerializeField] private MeshSimilarityCalculator _meshSimilarityCalculator;

    [Header("UI Screens")]
    [SerializeField] private SerializedDictionary<Station, GameObject> _uiElements;
    [SerializeField] private GameObject _heatingUI;
    [SerializeField] private GameObject _returnUI;
    [SerializeField] private GameObject _temperatureUI;

    [Header("Part Positions")]
    [SerializeField] private SerializedDictionary<Station, Transform> _partPositions;
    [SerializeField] private Transform _inWaterPosition;
    [SerializeField] private Transform _initialFurnacePosition;

    [Header("Temperature Displays")]
    [SerializeField] private TMPro.TextMeshProUGUI _temperatureDisplay;
    [SerializeField] private TMPro.TextMeshProUGUI _heatingCoolingButton;
    
    private GameObject _activeUI;
    private ForgedPart _activePart;

    private void Awake()
    {
        FurnaceSpotLocator.SetInitialLocation(_initialFurnacePosition);
    }

    private void Start()
    {
        FindObjectsOfType<Deformable>().ToList().ForEach(deformable => deformable.Clicked += OnPartSelected );
        ControllerManager.OnChangeStation += ChangeCamera;
        _uiElements.Values.ToList().ForEach(ui => ui.SetActive(false));
        _activeUI = _uiElements[Station.Overview];
        _activeUI.SetActive(true);
        _currentStation.Value = Station.Overview;
    }

    private void OnDestroy()
    {
        FindObjectsOfType<Deformable>().ToList().ForEach(deformable => deformable.Clicked -= OnPartSelected );
    }

    private void Update()
    {
        if(_activePart == null) return;
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
        //move the part
        _activePart.ChangePosition(_partPositions[newStation]);
        //move the camera
        _currentStation.Value = newStation;
        ChangeCameraView(newStation);
        //get new UI
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
        ChangeStation(Station.Heating);
        _activePart = null;
        CloseReturnUI();
    }
    
    /// <summary>
    /// Toggles the part state betweeen heating/cooling and ambient depending on the current station of the user.
    /// Also handles button text changes in the UI.
    /// </summary>
    public void ToggleHeatingOrCooling()
    {
        if(_activePart.CurrentState != ForgedPart.PartState.Ambient)
        {   if(_currentStation.Value == Station.Heating) _heatingCoolingButton.text = "Place in Furnace";
            else _heatingCoolingButton.text = "Place in Water";
            _activePart.SetToAmbient(_partPositions[Station.Cooling]);
        }
        else
        {
            if (_currentStation.Value == Station.Cooling)
            {
                _activePart.StartCooling(_inWaterPosition);
                _heatingCoolingButton.text = "Remove from Water";
            }
            else
            {
                _activePart.StartHeating();
                _heatingCoolingButton.text = "Remove from Furnace";
            }
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
        //new part has been selected, can only happen in heating screen
        _activePart = part;
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
        }
        else if (_currentStation.Value == Station.Cooling)
        {
            _temperatureUI.SetActive(true);
        }
        _activeUI.SetActive(true);
    }

    private void ChangeCameraView(Station station)
    {
        switch (station)
        {
            case Station.Cooling:
                animator.Play("Cooling Position");
                break;
            case Station.Forging:
                animator.Play("Forging Position");
                break;
            case Station.Heating:
                animator.Play("Heating Position");
                break;
            case Station.Overview:
                animator.Play("Overview Position");
                break;
            case Station.Planning:
                animator.Play("Planning Position");
                break;
            default:
                Debug.LogError("Station not found!");
                break;
        }
    }
}
