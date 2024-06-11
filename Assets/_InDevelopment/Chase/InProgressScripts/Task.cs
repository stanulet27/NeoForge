using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CustomInspectors;

namespace NeoForge.Orders
{
    [Serializable]
    public class Task
    {
        public enum ComparisonType { GreaterThan, LessThan, EqualTo, NotEqualTo, GreaterThanOrEqualTo, LessThanOrEqualTo }
        
        [SerializeField, ReadOnly] string _completionCondition;
        [SerializeField, ReadOnly] ComparisonType _comparisonType;
        [SerializeField, ReadOnly] private int _comparisonValue;
        
        public string Label;
        
        private Dictionary<string, ComparisonType> _comparisonTypeMap = new()
        {
            {" > ", ComparisonType.GreaterThan},
            {" < ", ComparisonType.LessThan},
            {" == ", ComparisonType.EqualTo},
            {" != ", ComparisonType.NotEqualTo},
            {" >= ", ComparisonType.GreaterThanOrEqualTo},
            {" <= ", ComparisonType.LessThanOrEqualTo}
        };
        
        /*
         * This constructor is used by the Unity Editor to create new tasks
         * The format of the text is
         * {Task Label} => {Completion Condition} {Comparison Type} {Comparison Value}
         * where comparison type is one of the following: >, <, ==, !=, >=, <=
         * Example: "Befriend the shopkeeper | ShopkeeperFriendship >= 100"
         * or
         * {Task Label} | {Completion Condition}
         * Example: "Drink some milk | MilkDrank"
         */
        public Task(string line)
        {
            var components = line.Split("=>");
            Label = components[0].Trim();
            var condition = components[1].Trim();
            var comparisionFound = _comparisonTypeMap.Select(x => x.Key).FirstOrDefault(x => condition.Contains(x));

            if (comparisionFound == null)
            {
                _comparisonType = ComparisonType.GreaterThanOrEqualTo;
                _comparisonValue = 1;
                _completionCondition = condition;
            }
            else
            {
                _comparisonType = _comparisonTypeMap[comparisionFound];
                var comparisonComponents = condition.Split(comparisionFound);
                Debug.Log(condition);
                _completionCondition = comparisonComponents[0].Trim();
                _comparisonValue = int.Parse(comparisonComponents[1].Trim());
            }
        }

        /// <summary>
        /// Returns true if the task is completed based on the current state.
        /// </summary>
        /// <param name="getState">The method to get the new state</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If the task is setup incorrectly</exception>
        public bool IsCompleted(Func<string, int> getState)
        {
            var value = getState(_completionCondition);
            Debug.Log("Checking if " + _completionCondition + " is " + _comparisonType + " " + _comparisonValue);
            return _comparisonType switch
            {
                ComparisonType.GreaterThan => value > _comparisonValue,
                ComparisonType.LessThan => value < _comparisonValue,
                ComparisonType.EqualTo => value == _comparisonValue,
                ComparisonType.NotEqualTo => value != _comparisonValue,
                ComparisonType.GreaterThanOrEqualTo => value >= _comparisonValue,
                ComparisonType.LessThanOrEqualTo => value <= _comparisonValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}