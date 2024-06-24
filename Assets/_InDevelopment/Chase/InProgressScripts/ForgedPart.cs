using System;
using System.Collections;
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
        private Station _currentStation;
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
        /// The mesh that the user created through deformation
        /// </summary>
        public Mesh UserCreatedMesh => _partMeshHandler.PartMesh.mesh;

        /// <summary>
        /// The environment settings of the part to be used for JAX
        /// </summary>
        public JAXEnvironmentSettings EnvironmentSettings => new(transform, _partMeshHandler, Details);
        
        ///<summary>
        /// The position of the part when it is out of the furnace
        /// </summary>
        public Transform OutFurnacePosition => _positionHandler.OutFurnacePosition;

        private Transform InFurnacePosition => _positionHandler.InFurnacePosition;

        private void Awake()
        {
            _positionHandler = GetComponent<PartPositionHandler>();
            _temperatureHandler = GetComponent<PartTemperatureHandler>();
            _partMeshHandler = GetComponent<PartMeshHandler>();
        }

        private void OnEnable()
        {
            if (Details == null) return;
            ToggleMovement(false);
            ToggleSelection(false);
            SetStation(Station.Heating);
            StartCoroutine(SetupPart(Details));
            DeformationHandler.OnHit += DeformationHandler_OnHit;
        }
        
        private void OnDisable()
        {
            DeformationHandler.OnHit -= DeformationHandler_OnHit;
        }


        /// <summary>
        /// Changes the position of the part to be inside the furnace and starts heating the part
        /// </summary>
        public void StartHeating()
        {
            UpdateSelectionIndication();
            _positionHandler.JumpToPosition(InFurnacePosition);
            _temperatureHandler.SetState(TemperatureState.Heating);
        }
        
        /// <summary>
        /// Changes the position of the part to be inside the water and starts cooling the part
        /// </summary>
        /// <param name="position">The transform that is inside the water</param>
        public void StartCooling(Transform position)
        {
            _positionHandler.JumpToPosition(position);
            _temperatureHandler.SetState(TemperatureState.Cooling);
        }

        /// <summary>
        /// Removes the part from either the furnace or the water and stops heating and cooling
        /// </summary>
        /// <param name="coolPosition"></param>
        public void SetToAmbient(Transform coolPosition)
        {
            var nextPosition = _temperatureHandler.CurrentState == TemperatureState.Heating ? 
                OutFurnacePosition : coolPosition;
            _positionHandler.JumpToPosition(nextPosition);
            _temperatureHandler.SetState(TemperatureState.Ambient);
            UpdateSelectionIndication();
        }
        
        /// <summary>
        /// Toggles whether the part can be moved or not by user input
        /// </summary>
        public void ToggleMovement(bool canMove)
        {
            _positionHandler.ToggleMovement(canMove);
        }
        
        /// <summary>
        /// Teleports the part to the target transform details
        /// </summary>
        public void ChangePosition(Transform target)
        {
            _positionHandler.JumpToPosition(target);
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
        public void SetStation(Station station)
        {
            _currentStation = station;
            _temperatureHandler.SetStation(station);

            UpdateSelectionIndication();
        }
        
        private IEnumerator SetupPart(PartDetails details)
        {
            Details = details;
            yield return DeformationHandler.SetupPart(EnvironmentSettings);
            _positionHandler.SetupPosition();
        }
        
        private void UpdateSelectionIndication()
        {
            var needsToDisplaySelection = _isSelected && _currentStation == Station.Heating;
            _positionHandler.ToggleVerticalOffset(needsToDisplaySelection);
        }
        
        private void DeformationHandler_OnHit()
        {
            if (!_isSelected) return;
            Details.Hits++;
        }
    }
}
