using System.Collections.Generic;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class AreaSwapper : MonoBehaviour
    {
        [SerializeField] private GameObject _overview;
        [SerializeField] private GameObject _navigator;
        [SerializeField] private List<ForgeArea> _areaOrder;
        [SerializeField] private ForgeStationManager _stationManager;
        
        private ForgeArea _currentArea;

        private void OnEnable()
        {
            _stationManager.OnChangeStation += OnAreaChanged;
        }
        
        private void OnDisable()
        {
            _stationManager.OnChangeStation -= OnAreaChanged;
        }

        /// <summary>
        /// Move to the next area in the list
        /// </summary>
        public void NextArea()
        {
            ShiftAreaBy(1);
        }
        
        /// <summary>
        /// Move to the previous area in the list
        /// </summary>
        public void PreviousArea()
        {
            ShiftAreaBy(-1);
        }

        private void ShiftAreaBy(int offset)
        {
            if (!_areaOrder.Contains(_currentArea)) return;
            
            var newIndex = (_areaOrder.IndexOf(_currentArea) + offset + _areaOrder.Count) % _areaOrder.Count;
            _stationManager.ChangeArea(_areaOrder[newIndex]);
        }

        private void OnAreaChanged(ForgeArea newArea)
        {
            _overview.SetActive(newArea == ForgeArea.Overview);
            _navigator.SetActive(newArea != ForgeArea.Overview);
            
            _currentArea = newArea;
        }
    }
}