using System;
using System.Collections;
using AYellowpaper.SerializedCollections;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public enum TemperatureState { Heating, Cooling, Ambient }

    [RequireComponent(typeof(PartPositionHandler))]
    [RequireComponent(typeof(PartTemperatureHandler))]
    [RequireComponent(typeof(PartMeshHandler))]
    public class ForgedPart : MonoBehaviour
    {
        /// <summary>
        /// Will be called when the part is clicked
        /// </summary>
        public event Action<ForgedPart> OnPartClicked;
        
        private ForgeArea _currentForgeArea;
        private bool _isSelected;
        private PartPositionHandler _positionHandler;
        private PartTemperatureHandler _temperatureHandler;
        private PartMeshHandler _partMeshHandler;

        /// <summary>
        /// The details of the part
        /// </summary>
        public PartDetails Details { get; set; }

        /// <summary>
        /// The temperature of the part in kelvin
        /// </summary>
        public float Temperature => _temperatureHandler.Temperature;
        
        /// <summary>
        /// The current state of the part
        /// </summary>
        public TemperatureState CurrentState => _temperatureHandler.CurrentState;
        
        /// <summary>
        /// The Temperature Handler that is used to control the temperature of the part
        /// </summary>
        public PartTemperatureHandler TemperatureHandler => _temperatureHandler;

        /// <summary>
        /// The mesh that the user created through deformation
        /// </summary>
        public Mesh UserCreatedMesh => _partMeshHandler.PartMesh.mesh;

        /// <summary>
        /// The environment settings of the part to be used for JAX
        /// </summary>
        public JAXEnvironmentSettings EnvironmentSettings => new(transform, _partMeshHandler, Details);

        private void Awake()
        {
            _positionHandler = GetComponent<PartPositionHandler>();
            _temperatureHandler = GetComponent<PartTemperatureHandler>();
            _partMeshHandler = GetComponent<PartMeshHandler>();
        }

        private void OnEnable()
        {
            if (Details == null) gameObject.SetActive(false);
            
            _positionHandler.ToggleMovement(false);
            ToggleSelection(false);
            StartCoroutine(SetupPart(Details));
            DeformationHandler.OnHit += DeformationHandler_OnHit;
        }
        
        private void OnDisable()
        {
            DeformationHandler.OnHit -= DeformationHandler_OnHit;
        }

        /// <summary>
        /// Will setup the forging positions for the part
        /// </summary>
        public void SetPositions(ForgingPositions positions)
        {
            _positionHandler.InitializeForgingPositions(positions);
        }

        /// <summary>
        /// Will toggle the temperature state of the part. Will also move the part to the corresponding position for 
        /// the given state.
        /// </summary>
        public void SetTemperatureState(TemperatureState newState)
        {
            switch (newState)
            {
                case TemperatureState.Heating:
                    _positionHandler.PutIntoFurnace();
                    break;
                case TemperatureState.Cooling:
                    _positionHandler.SubmergeInWater();
                    break;
                case TemperatureState.Ambient:
                    _positionHandler.JumpToStation(_currentForgeArea);
                    break;
            }
            _temperatureHandler.SetState(newState);
            UpdateSelectionIndication();
        }

        /// <summary>
        /// Toggles whether the part is selected or not
        /// </summary>
        public void ToggleSelection(bool selected)
        {
            _isSelected = selected;
            UpdateSelectionIndication();
        }

        /// <summary>
        /// Switches the station of the part to the specified station
        /// </summary>
        public void SetStation(ForgeArea newArea)
        {
            var currentlyBeingHeated = newArea == ForgeArea.Heating 
                                       && _temperatureHandler.CurrentState == TemperatureState.Heating;
            var inValidMovementArea = newArea is ForgeArea.Heating or ForgeArea.Planning;
            
            _currentForgeArea = newArea;
            _temperatureHandler.SetStation(newArea);
            _positionHandler.JumpToStation(newArea);
            _positionHandler.ToggleMovement(inValidMovementArea);
            
            if (currentlyBeingHeated) _positionHandler.PutIntoFurnace();

            UpdateSelectionIndication();
        }
        
        /// <summary>
        /// Signal that the part has been Clicked
        /// </summary>
        public void ClickPart()
        {
            OnPartClicked?.Invoke(this);
        }

        private IEnumerator SetupPart(PartDetails details)
        {
            Details = details;
            yield return DeformationHandler.SetupPart(EnvironmentSettings);
            _positionHandler.SetupPosition();
            SetStation(ForgeArea.Heating);
        }
        
        private void UpdateSelectionIndication()
        {
            var needsToDisplaySelection = _isSelected && _currentForgeArea == ForgeArea.Heating;
            _positionHandler.ToggleVerticalOffset(needsToDisplaySelection);
        }
        
        private void DeformationHandler_OnHit()
        {
            if (!_isSelected) return;
            Details.Hits++;
        }
    }
}
