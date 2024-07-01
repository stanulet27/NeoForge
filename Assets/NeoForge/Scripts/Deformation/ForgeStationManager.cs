using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Deformation.Scoring;
using NeoForge.Input;
using NeoForge.Stations;
using NeoForge.Stations.Warehouse;
using NeoForge.UI.Inventory;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class ForgeStationManager : MonoBehaviour, IStation
    {
        /// <summary>
        /// Invoked when the player changes the station they are in.
        /// </summary>
        public event Action<ForgeArea> OnChangeStation;
        
        [Header("References")]
        [Tooltip("The mesh similarity calculator that is used to compare the current mesh to the target mesh")]
        [SerializeField] private MeshSimilarityCalculator _meshSimilarityCalculator;
        [Tooltip("The deformation handler")]
        [SerializeField] private DeformationHandler _deformationHandler;
        [Tooltip("Will handle displaying UI Information")]
        [SerializeField] private ForgingHUD _forgingHUD;
        [Tooltip("Handles the camera movement between stations")]
        [SerializeField] private ForgingCamera _forgingCamera;
        
        private bool _aPartIsActive;
        private ForgedPart _activePart;
        private ForgeArea _currentArea;

        private void OnDestroy()
        {
            ForgePartPool.Instance.ForgedPartsPool.ForEach(p => p.OnPartClicked -= OnPartClicked);
            ControllerManager.OnChangeStation -= ChangeArea;
        }
        
        public void EnterStation()
        {
            ForgePartPool.Instance.ForgedPartsPool.ForEach(p => p.OnPartClicked += OnPartClicked);
            ControllerManager.OnChangeStation += ChangeArea;
            _forgingHUD.OpenUI();

            ChangeArea(ForgeArea.Overview);
        }

        public void ExitStation()
        {
            ForgePartPool.Instance.ForgedPartsPool.ForEach(p => p.OnPartClicked -= OnPartClicked);
            ControllerManager.OnChangeStation -= ChangeArea;
            
            if (_aPartIsActive) ReturnPartToHeating();
            ChangeArea(ForgeArea.Overview);
            _forgingHUD.CloseUI();
        }

        /// <summary>
        /// Changes the current station the player is in and swaps the camera and UI to match.
        /// </summary>
        public void ChangeArea(ForgeArea newArea)
        {
            Debug.Log("Changing Area to: " + newArea);
            if (newArea == ForgeArea.Planning) _meshSimilarityCalculator.UpdateScore();
            if (_aPartIsActive) _activePart.SetStation(newArea);
            
            _currentArea = newArea;
            _forgingHUD.ChangeUI(newArea, _aPartIsActive);
            _forgingCamera.ChangeCameraView(newArea);
            OnChangeStation?.Invoke(newArea);
        }

        /// <summary>
        /// Sends the player and the part back to the heating table
        /// </summary>
        public void ReturnPartToHeating()
        {
            _activePart.SetStation(ForgeArea.Heating);
            SetActivePart(null);
            ChangeArea(ForgeArea.Heating);
            _forgingHUD.ToggleReturnUI(false);
        }

        /// <summary>
        /// Toggles the part state between heating/cooling and ambient depending on the current station of the user.
        /// Also handles button text changes in the UI.
        /// </summary>
        public void ToggleHeatingOrCooling()
        {
            var partIsAmbient = _activePart.CurrentState == TemperatureState.Ambient;
            var atHeatingStation = _currentArea == ForgeArea.Heating;
            var nextState = !partIsAmbient ? TemperatureState.Ambient : 
                atHeatingStation ? TemperatureState.Heating 
                : TemperatureState.Cooling;
            
            _activePart.SetTemperatureState(nextState);
        }

        /// <summary>
        /// Will submit the part to be reviewed and scored. The player will be shown the results of the part. The
        /// inventory will be updated with the new part.
        /// </summary>
        public void SubmitPart()
        {
            var results = new ForgingResults(_meshSimilarityCalculator, _activePart.Details, _activePart.UserCreatedMesh);
            CompletedItem.CreateItem(results);
            
            _forgingHUD.DisplayCompletionScreen(results, OnPartReviewed);
        }

        private void OnPartReviewed()
        {
            _activePart.gameObject.SetActive(false);
            SetActivePart(null);
            ChangeArea(ForgeArea.Overview);
            _meshSimilarityCalculator.PostScore();
        }

        private void SetActivePart(ForgedPart part)
        {
            if (_activePart != null) _activePart.ToggleSelection(false);
            
            _activePart = part;
            _aPartIsActive = part != null;
            _meshSimilarityCalculator.SetPart(_aPartIsActive ? part.Details.ScoreDetails : null);
            
            if (_aPartIsActive)
            {
                _activePart.ToggleSelection(true);
                StartCoroutine(_deformationHandler.PrepareEnvironment(part.EnvironmentSettings));
            }

            _forgingHUD.SetPart(part);
            _forgingHUD.ChangeUI(_currentArea, _aPartIsActive);
        }

        private void OnPartClicked(ForgedPart part)
        {
            var newPartSelected = part != _activePart && _currentArea == ForgeArea.Heating;
            if (newPartSelected) SetActivePart(part);
            else if (_currentArea != ForgeArea.Heating) _forgingHUD.ToggleReturnUI(true);
        }
    }
}