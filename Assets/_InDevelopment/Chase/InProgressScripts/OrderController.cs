using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NeoForge.Orders
{
    public class OrderController : SingletonMonoBehaviour<OrderController>
    {
        [SerializeField, ReadOnly] private List<Order> _ordersInGame;
        [SerializeField, ReadOnly] private List<Order> _activeOrders;

        private void Start()
        {
            _ordersInGame = new List<Order>(Resources.LoadAll<Order>("Orders"));
        }

        public void OnDialogueEvent(string eventTriggered)
        {
            Debug.Assert(eventTriggered.StartsWith("GainOrder"));
            
            var giver = eventTriggered.Split("-")[1];
            var part = eventTriggered.Split("-")[2];
            var order = _ordersInGame.FirstOrDefault(x => Matches(x, giver, part));
            
            Debug.Assert(order != null, $"No order found for {giver} and {part}");
            
            _activeOrders.Add(order);
        }
        
        private bool Matches(Order order, string giver, string part)
        {
            return order.GiverName.Equals(giver, StringComparison.InvariantCultureIgnoreCase) 
                   && order.ObjectToCraft.ToString().Equals(part, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}