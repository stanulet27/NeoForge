using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NeoForge.Orders
{
    [CreateAssetMenu(fileName = "New Order", menuName = "Orders/Order")]
    public class Order : ScriptableObject
    {
        public enum CraftableObjects { BasicBar = 0, Sphere = 1 }
        
        [SerializeField] private int _id;
        [SerializeField] private CraftableObjects _objectToCraft;
        [SerializeField] private string _giverName;
        [SerializeField] private int _paymentAmount;
        [SerializeField] private int _time;
        [SerializeField, TextArea(1, 4)] private string _requirements;
        [SerializeField, TextArea(1, 4)] private string _flavorText;
        
        [Tooltip("The event that triggers the order")]
        [SerializeField] private int _triggerEvent;
        
        public int ID => _id;
        public CraftableObjects ObjectToCraft => _objectToCraft;
        public string GiverName => _giverName;
        public int PaymentAmount => _paymentAmount;
        public int Time => _time;
        public string Requirements => _requirements;
        public string FlavorText => _flavorText;
        
        public int TriggerEvent => _triggerEvent;
        
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
        
        static List<string> ParseCSVLine(string line)
        {
            var matches = Regex.Matches(line, @"(?<=^|,)\s*""([^""]*)""\s*|(?<=^|,)\s*([^,""]*)\s*");
            var result = new List<string>();
        
            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
            }

            return result;
        }

        /// <summary>
        /// Converts the order to a string format to be displayed
        /// </summary>
        public string GetTaskDescription()
        {
            return "";
        }
    }
}