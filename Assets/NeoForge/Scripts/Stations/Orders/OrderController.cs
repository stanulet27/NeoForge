using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities;
using SharedData;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NeoForge.Stations.Orders
{
    public class OrderController : SingletonMonoBehaviour<OrderController>
    {
        private const string ORDER_FOLDER = "Orders";
        
        [Tooltip("The current day")]
        [SerializeField] private SharedInt _currentDay;
        
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
        /// to the active orders list. Requires that the event is in the format "GainOrder-GiverName-QuestNumber" and
        /// that the order exists in the game.
        /// </summary>
        public void OnDialogueEvent(string eventTriggered)
        {
            Debug.Assert(eventTriggered.StartsWith("GainOrder"));
            
            var giver = eventTriggered.Split("-")[1];
            var part = eventTriggered.Split("-")[2];

            var order = GetOrder(giver, part);
            order.DueDate = _currentDay + order.Time;
            _activeOrders.Add(order);
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
        public Order GetOrder(string giver, string number)
        {
            var partIndex = int.Parse(number) - 1;
            var giverOrders = _ordersInGame.Where(x => GiverNameMatches(x, giver)).ToList();
            Debug.Assert(giverOrders.Count > 0, $"No orders found for {giver}");
            Debug.Assert(giverOrders.Count > partIndex, $"Not enough orders found for {giver} and {number}");
            var order = giverOrders[partIndex];

            Debug.Assert(order != null, $"No order found for {giver} and {number}");
            return order;
        }

        private static bool GiverNameMatches(Order order, string giver)
        {
            return order.GiverName.Equals(giver, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}