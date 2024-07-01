using System.Collections.Generic;
using System.Linq;
using NeoForge.Input;
using UnityEngine;

namespace NeoForge.Stations.Orders
{
    public class OrderBoard : MonoBehaviour, IStation
    {
        [Tooltip("The position where the full order display will be shown.")]
        [SerializeField] private Transform _orderFullDisplayPosition;
        
        readonly List<OrderDisplay> _orderDisplays = new();
        private OrderDisplay _currentFullDisplay;
        private int _selectedDisplayIndex;
        
        private bool FullViewBeingDisplayed => _currentFullDisplay != null;
        private OrderDisplay SelectedDisplay => _orderDisplays[_selectedDisplayIndex];

        private void Awake()
        {
            foreach (var orderDisplay in GetComponentsInChildren<OrderDisplay>(true))
            {
                orderDisplay.OnOrderClicked += DisplayOrderFullView;
                _orderDisplays.Add(orderDisplay);
            }
        }
        
        private void Start()
        {
            UpdateOrders();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromController();
        }

        public void EnterStation()
        {
            UpdateOrders();
            _selectedDisplayIndex = 0;
            SelectedDisplay.SetHighlight(true);

            SubscribeToController();
        }

        public void ExitStation()
        {
            ResetFullDisplay();
            SelectedDisplay.SetHighlight(false);
            UnsubscribeFromController();
        }
        
        /// <summary>
        /// Will refresh the orders on the board. If there are less orders than displays, the rest will be hidden.
        /// </summary>
        public void UpdateOrders()
        {
            var orders = OrderController.Instance.GetActiveOrders();
            for (var i = 0; i < _orderDisplays.Count; i++)
            {
                _orderDisplays[i].SetOrder(i < orders.Count ? orders[i] : null);
            }
        }
        
        private void SubscribeToController()
        {
            ControllerManager.OnMouseClick += OnMouseClick;
            ControllerManager.OnMove += SwapDisplay;
            ControllerManager.OnInteract += SelectDisplay;
        }
        
        private void UnsubscribeFromController()
        {
            ControllerManager.OnMouseClick -= OnMouseClick;
            ControllerManager.OnMove -= SwapDisplay;
            ControllerManager.OnInteract -= SelectDisplay;
        }
        
        private void SelectDisplay()
        {
            if (FullViewBeingDisplayed)
            {
                ResetFullDisplay();
            }
            else if (_orderDisplays.Count > 0)
            {
                DisplayOrderFullView(SelectedDisplay);
            }
        }

        private void SwapDisplay(Vector2 direction)
        {
            if (direction.x == 0 || FullViewBeingDisplayed) return;
            
            var totalDisplays = _orderDisplays.Count(x => x.gameObject.activeInHierarchy);
            
            SelectedDisplay.SetHighlight(false);
            _selectedDisplayIndex = (_selectedDisplayIndex + (int)direction.x + totalDisplays) % totalDisplays;
            SelectedDisplay.SetHighlight(true);
        }

        private void DisplayOrderFullView(OrderDisplay order)
        {
            if (order == null || FullViewBeingDisplayed) return;
            
            SelectedDisplay.SetHighlight(false);
            order.JumpToPosition(_orderFullDisplayPosition.position);
            _currentFullDisplay = order;
        }

        private void OnMouseClick()
        {
            if (FullViewBeingDisplayed) ResetFullDisplay();
        }
        
        private void ResetFullDisplay()
        {
            if (!FullViewBeingDisplayed) return;
            
            _selectedDisplayIndex = _orderDisplays.IndexOf(_currentFullDisplay);
            _currentFullDisplay.SetHighlight(true);
            _currentFullDisplay.ReturnToInitialPosition();
            _currentFullDisplay = null;
        }
    }
}