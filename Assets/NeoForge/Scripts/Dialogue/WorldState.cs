using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Dialogue
{
    public static class WorldState
    {
        public static Action OnWorldStateChanged;
        
        private static readonly Dictionary<string, int> _currentState = new();
        
        /// <summary>
        /// Will return the current world state
        /// </summary>
        public static Dictionary<string, int> GetWorldState()
        {
            return _currentState;
        }
        
        /// <summary>
        /// Will load the world state from the dictionary passed in
        /// </summary>
        /// <param name="worldState">The new world state</param>
        public static void LoadWorldState(Dictionary<string, int> worldState)
        {
            _currentState.Clear();
            foreach (var (key, value) in worldState)
            {
                _currentState[key] = value;
            }
            OnWorldStateChanged?.Invoke();
        }

        /// <summary>
        /// Will assign a new value to a property of the world state
        /// </summary>
        /// <param name="key">The property of the world state.</param>
        /// <param name="value">The new value of the world state</param>
        [Button]
        public static void SetState(string key, int value)
        {
            _currentState[key] = value;
            OnWorldStateChanged?.Invoke();
        }
        
        /// <summary>
        /// Will perform: CurrentState[key] = valueCalculator(CurrentState[key])
        /// Requires that both the key and valueCalculator are not null
        /// </summary>
        /// <param name="key">The world state property</param>
        /// <param name="valueCalculator">The function to apply to the world state property's current value</param>
        public static void SetState(string key, Func<int, int> valueCalculator)
        {
            Debug.Assert(key != null, nameof(key) + " != null");
            Debug.Assert(valueCalculator != null, nameof(valueCalculator) + " != null for key: " + key);
            
            _currentState[key] = valueCalculator(_currentState.TryGetValue(key, out var currentValue) ? currentValue : 0);
            OnWorldStateChanged?.Invoke();
        }
        
        /// <summary>
        /// Will set the value of the world state property to 1 if value is true, 0 otherwise
        /// </summary>
        /// <param name="key">The world state property</param>
        /// <param name="value">The desired new state</param>
        public static void SetState(string key, bool value)
        {
            _currentState[key] = value ? 1 : 0;
            OnWorldStateChanged?.Invoke();
        }
        
        /// <summary>
        /// Will return the current state of the world state property
        /// </summary>
        public static int GetState(string key)
        {
            return _currentState.TryGetValue(key, out var value) ? value : 0;
        }

        /// <summary>
        /// Will return true if the world state property is greater than 0 / is true
        /// </summary>
        public static bool InState(string key)
        {
            return _currentState.ContainsKey(key) && _currentState[key] > 0;
        }
        
        /// <summary>
        /// Will return true if the world state property is set to the given enum value
        /// </summary>
        public static bool InState<T>(T key) where T : Enum
        {
            return InState(key.ToString().ToLower());
        }
        
        /// <summary>
        /// Will reset the world state property to 0 / false
        /// </summary>
        /// <param name="key">The world state property</param>
        [Button]
        public static void ClearState(string key)
        {
            _currentState.Remove(key);
        }
        
        /// <summary>
        /// Will reset all world state properties to 0 / false
        /// </summary>
        [Button]
        public static void ClearAllStates()
        {
            _currentState.Clear();
        }

        /// <summary>
        /// Will return a string representation of the current world state
        /// </summary>
        public static string GetCurrentWorldState()
        {
            return string.Join("\n", _currentState.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
    }
}