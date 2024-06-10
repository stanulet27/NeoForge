using System.Collections.Generic;
using System.Linq;
using NeoForge.Input;
using UnityEngine;
using NeoForge.Dialogue;

namespace NeoForge.Orders
{
    public class OrderController : MonoBehaviour
    {
        [Tooltip("Quests that are available in the game")]
        [SerializeField] private List<Order> _orders = new();
        [Tooltip("The display to open / close to display quest info")]
        [SerializeField] OrderUIDisplay _orderUIDisplay;
        
        private Order _currentOrder;
        private int _currentOrderID;

        private void Awake()
        {
            _orderUIDisplay.Hide();
        }

        private void OnEnable()
        {
            WorldState.OnWorldStateChanged += HandleWorldStateChanged;
        }
        
        private void OnDisable()
        {
            WorldState.OnWorldStateChanged -= HandleWorldStateChanged;
            ControllerManager.OnClose -= CloseUI;
        }
        
        /// <summary>
        /// Will display the current quest status
        /// </summary>
        public void OpenUI()
        {
            if (_currentOrder == null)
            {
                _orderUIDisplay.Display();
            }
            else
            {
                _orderUIDisplay.Display(_currentOrder.GetTaskDescription());
            }
            
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            ControllerManager.OnClose += CloseUI;
        }
        
        /// <summary>
        /// Will hide the current quest status
        /// </summary>
        public void CloseUI()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
            _orderUIDisplay.Hide();
            ControllerManager.OnClose -= CloseUI;
        }
        
        private void HandleWorldStateChanged()
        {
            if (TryHandleNewOrder()) return;
            if (NeedResetOrder()) return;

            while (_currentOrder != null
                   && _currentOrder.CheckNextCompletionStatus(WorldState.GetState))
            {
                _currentOrder.CompleteCurrentTask();
            }
        }

        private bool NeedResetOrder()
        {
            if (!WorldState.InState("orderNeedsReset")) return false;
            WorldState.SetState("orderNeedsReset", 0);
            _currentOrder.Reset();
            return true;
        }
        
        private bool TryHandleNewOrder()
        {
            if (_currentOrderID == WorldState.GetState("orderType")) return false;
            if (_orders.All(x => x.TriggerEvent != WorldState.GetState("orderType"))) return false;
            _currentOrderID = WorldState.GetState("orderType");
            _currentOrder = _orders.Find(x => x.TriggerEvent == _currentOrderID);
            Debug.Log("Swapping to order " + _currentOrderID);
            _currentOrder.Reset();

            return true;
        }
    }
}