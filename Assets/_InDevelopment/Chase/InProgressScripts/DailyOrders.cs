using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Orders
{
    [CreateAssetMenu(fileName = "DailyOrders", menuName = "Orders/DailyOrders")]
    public class DailyOrders : ScriptableObject
    {
        [SerializeField] private List<CustomerOrder> _orders = new();
        [SerializeField] private int _ordersToGive;
        [SerializeField, ValueDropdown("_onOffBools")] private bool _randomizeOrder;

        List<CustomerOrder> _todaysOrders = new();
        
        private IEnumerable _onOffBools = new ValueDropdownList<bool>
        {
            { "On", true },
            { "Off", false }
        };

        /// <summary>
        /// Will see if there are any orders left to give out. If there are, it will return the order and remove
        /// it from the list. If there are no orders left, it will return false.
        /// </summary>
        /// <param name="order">The order found</param>
        public bool TryGetOrder(out CustomerOrder order)
        {
            if (_todaysOrders.Count == 0)
            {
                order = null;
                return false;
            }

            order = _todaysOrders[0];
            _todaysOrders.RemoveAt(0);
            return true;
        }
        
        /// <summary>
        /// Will prepare the day by randomizing the orders if needed and setting the orders to give out.
        /// </summary>
        public void PrepareDay()
        {
            _todaysOrders.Clear();
            if (_randomizeOrder)
            {
                var orders = new List<CustomerOrder>(_orders);
                for (int i = 0; i < _ordersToGive; i++)
                {
                    var order = orders[Random.Range(0, orders.Count)];
                    _todaysOrders.Add(order);
                    orders.Remove(order);
                }
            }
            else
            {
                for (int i = 0; i < _ordersToGive; i++)
                {
                    _todaysOrders.Add(_orders[i]);
                }
            }
        }
    }
}