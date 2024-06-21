using System.Collections;
using System.Collections.Generic;
using NeoForge.Stations.Customers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Stations.Orders
{
    [CreateAssetMenu(fileName = "DailyOrders", menuName = "Orders/DailyOrders")]
    public class DailyOrders : ScriptableObject
    {
        [Tooltip("The customers that can arrive on the given day")]
        [SerializeField] private List<CustomerOrder> _orders = new();
        [Tooltip("The amount of orders to give out during the course of the day")]
        [SerializeField] private int _ordersToGive;
        [Tooltip("If the order should be randomized or not")]
        [SerializeField, ValueDropdown("_onOffBools")] private bool _randomizeOrder;

        List<CustomerOrder> _todaysOrders = new();
        
        private IEnumerable _onOffBools = new ValueDropdownList<bool>
        {
            { "On", true },
            { "Off", false }
        };

        /// <summary>
        /// Will see if there are any orders left to give out. If there are, it will return the order and remove
        /// it from the list. If there are no orders left, it will return false and set order to null.
        /// </summary>
        /// <param name="order">The order found, null if none left</param>
        public bool TryGetOrder(out CustomerOrder order)
        {
            var ordersLeft = _todaysOrders.Count > 0;
            order = ordersLeft ? _todaysOrders[0] : null;
            if (ordersLeft) _todaysOrders.RemoveAt(0);
            
            return ordersLeft;
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