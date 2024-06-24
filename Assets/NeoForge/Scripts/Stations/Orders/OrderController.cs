using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NeoForge.Stations.Orders
{
    public class OrderController : SingletonMonoBehaviour<OrderController>
    {
        private const string ORDER_FOLDER = "Orders";
        
        [Tooltip("The orders in the game")]
        [SerializeField, ReadOnly] private List<Order> _ordersInGame;
        
        [Tooltip("The active orders the player currently has")]
        [SerializeField, ReadOnly] private List<Order> _activeOrders;

        /// <summary>
        /// The active orders the player currently has
        /// </summary>
        public List<Order> GetActiveOrders() => _activeOrders;

        private void Start()
        {
            _ordersInGame = new List<Order>(Resources.LoadAll<Order>(ORDER_FOLDER));
        }

        /// <summary>
        /// Parses the incoming event and the matching order in the game. Then, it adds the order
        /// to the active orders list. Requires that the event is in the format "GainOrder-GiverName-PartName" and
        /// that the order exists in the game.
        /// </summary>
        public void OnDialogueEvent(string eventTriggered)
        {
            Debug.Assert(eventTriggered.StartsWith("GainOrder"));
            
            var giver = eventTriggered.Split("-")[1];
            var part = eventTriggered.Split("-")[2];

            _activeOrders.Add(GetOrder(giver, part));
        }
        
        /// <summary>
        /// Will remove the order from the active orders list
        /// </summary>
        public void CompleteOrder(Order order)
        {
            _activeOrders.Remove(order);
        }

        /// <summary>
        /// Will return the first order that matches the giver and part
        /// </summary>
        public Order GetOrder(string giver, string part)
        {
            var order = _ordersInGame.FirstOrDefault(x => Matches(x, giver, part));
            
            Debug.Assert(order != null, $"No order found for {giver} and {part}");
            return order;
        }
        
        private static bool Matches(Order order, string giver, string part)
        {
            return order.GiverName.Equals(giver, StringComparison.InvariantCultureIgnoreCase) 
                   && order.ObjectToCraft.ToString().Equals(part, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}