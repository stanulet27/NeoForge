using System.Collections.Generic;
using System.Linq;
using NeoForge.Input;
using UnityEngine;

namespace NeoForge.Orders
{
    public class OrderBoard : MonoBehaviour, IStation
    {
        [SerializeField] private Transform _orderFullDisplayPosition;
        
        readonly List<OrderDisplay> _orderDisplays = new();
        
        private OrderDisplay _currentFullDisplay;
        private int _currentDisplayIndex;

        private void Awake()
        {
            var orderDisplays = GetComponentsInChildren<OrderDisplay>(true);
            foreach (var orderDisplay in orderDisplays)
            {
                orderDisplay.OnOrderClicked += OnOrderClicked;
                _orderDisplays.Add(orderDisplay);
            }
        }
        
        private void Start()
        {
            UpdateOrders();
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnMouseClick -= OnMouseClick;
            ControllerManager.OnMove -= SwapDisplay;
            ControllerManager.OnInteract -= SelectDisplay;
        }
        
        private void SelectDisplay()
        {
            if (_currentFullDisplay != null)
            {
                _currentDisplayIndex = _orderDisplays.IndexOf(_currentFullDisplay);
                _currentFullDisplay.SetHighlight(true);
                _currentFullDisplay.ReturnToInitialPosition();
                _currentFullDisplay = null;
            }
            else if (_orderDisplays.Count > 0)
            {
                OnOrderClicked(_orderDisplays[_currentDisplayIndex]);
            }
        }

        private void SwapDisplay(Vector2 direction)
        {
            if (direction.x == 0 || _currentFullDisplay != null) return;
            _orderDisplays[_currentDisplayIndex].SetHighlight(false);
            var displays = _orderDisplays.Count(x => x.gameObject.activeInHierarchy);
            _currentDisplayIndex = (_currentDisplayIndex + (int)direction.x + displays) % displays;
            _orderDisplays[_currentDisplayIndex].SetHighlight(true);
        }
        
        public void UpdateOrders()
        {
            var orders = OrderController.Instance.GetActiveOrders();
            for (var i = 0; i < _orderDisplays.Count; i++)
            {
                _orderDisplays[i].SetOrder(i < orders.Count ? orders[i] : null);
            }
        }
        
        private void OnOrderClicked(OrderDisplay order)
        {
            if (order == null || _currentFullDisplay != null) return;
            _orderDisplays[_currentDisplayIndex].SetHighlight(false);
            order.JumpToPosition(_orderFullDisplayPosition.position);
            _currentFullDisplay = order;
        }

        private void OnMouseClick()
        {
            if (_currentFullDisplay == null) return;
            _currentFullDisplay.SetHighlight(true);
            _currentDisplayIndex = _orderDisplays.IndexOf(_currentFullDisplay);
            _currentFullDisplay.ReturnToInitialPosition();
            _currentFullDisplay = null;
        }

        public void EnterStation()
        {
            _currentDisplayIndex = 0;
            _orderDisplays[_currentDisplayIndex].SetHighlight(true);
            
            ControllerManager.OnMouseClick += OnMouseClick;
            ControllerManager.OnMove += SwapDisplay;
            ControllerManager.OnInteract += SelectDisplay;
        }

        public void ExitStation()
        {
            if (_currentFullDisplay != null)
            {
                _currentFullDisplay.ReturnToInitialPosition();
                _currentFullDisplay = null;
            }
            
            _orderDisplays[_currentDisplayIndex].SetHighlight(false);
            ControllerManager.OnMouseClick -= OnMouseClick;
            ControllerManager.OnMove -= SwapDisplay;
            ControllerManager.OnInteract -= SelectDisplay;
        }
    }
}