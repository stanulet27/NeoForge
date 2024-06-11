using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CustomInspectors;
using UnityEngine;

namespace NeoForge.Orders
{
    [CreateAssetMenu(fileName = "New Order", menuName = "Orders/Order")]
    public class Order : ScriptableObject
    {
        public enum CraftableObjects { BasicBar = 0, Sphere = 1 }
        
        [Tooltip("The unique ID of the order")]
        [SerializeField] private int _id;
        
        [Tooltip("The object that the player needs to craft")]
        [SerializeField] private CraftableObjects _objectToCraft;
        
        [Tooltip("The name of the npc giver of the order")]
        [SerializeField] private string _giverName;
        
        [Tooltip("The amount of money the player will receive for completing the order")]
        [SerializeField] private int _paymentAmount;
        
        [Tooltip("The time in days the player has to complete the order")]
        [SerializeField] private int _time;
        
        [Tooltip("Not used yet")]
        [SerializeField, TextArea(1, 4), ReadOnly] private string _requirements;

        [Tooltip("The flavor text of the order")]
        [SerializeField, TextArea(1, 4)] private string _flavorText;

        /// <summary>
        /// The unique ID of the order
        /// </summary>
        public int ID => _id;
        
        /// <summary>
        /// The object that the player needs to craft
        /// </summary>
        public CraftableObjects ObjectToCraft => _objectToCraft;
        
        /// <summary>
        /// The name of the npc giver of the order
        /// </summary>
        public string GiverName => _giverName;
        
        /// <summary>
        /// The amount of money the player will receive for completing the order
        /// </summary>
        public int PaymentAmount => _paymentAmount;
        
        /// <summary>
        /// The time in days the player has to complete the order
        /// </summary>
        public int Time => _time;
        
        /// <summary>
        /// The requirements of the order
        /// </summary>
        public string Requirements => _requirements;
        
        /// <summary>
        /// The flavor text of the order
        /// </summary>
        public string FlavorText => _flavorText;
        
        /// <summary>
        /// Takes in a csv line in the format of "ID, ObjectToCraft, GiverName, PaymentAmount, Time, Requirements, FlavorText"
        /// and sets up the order scriptable object
        /// </summary>
        public void Setup(string input)
        {
            var components = ParseCSVLine(input);
            _id = int.Parse(components[0]);
            _objectToCraft = (CraftableObjects) Enum.Parse(typeof(CraftableObjects), components[1].Replace(" ", ""));
            _giverName = components[2];
            _paymentAmount = int.Parse(components[3]);
            _time = int.Parse(components[4]);
            _requirements = components[5];
            _flavorText = components[6];
            
            name = $"SO_{_giverName}_{_objectToCraft}_Order";
        }
        
        private static List<string> ParseCSVLine(string line)
        {
            var matches = Regex.Matches(line, @"(?<=^|,)\s*""([^""]*)""\s*|(?<=^|,)\s*([^,""]*)\s*");
            var result = new List<string>();
        
            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
            }

            return result;
        }
    }
}