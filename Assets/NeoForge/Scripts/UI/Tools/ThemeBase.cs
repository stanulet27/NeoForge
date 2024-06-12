using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public abstract class ThemeBase<T> : ScriptableObject
    {
        private static readonly List<string> _validStates = new() 
            { "Normal", "Highlighted", "Pressed", "Disabled", "Selected" };
        
        [Tooltip("Displayed when button in neutral state.")]
        public T Normal;
        
        [Tooltip("Displayed when button is hovered over.")]
        public T Highlighted;
        
        [Tooltip("Displayed when button is pressed.")]
        public T Pressed;
        
        [Tooltip("Displayed when button is disabled.")]
        public T Disabled;
        
        [Tooltip("Displayed when button is selected.")]
        public T Selected;

        /// <summary>
        /// Returns the value of the theme for the given state.
        /// </summary>
        public T GetValueFromState(string state)
        {
            Debug.Assert(IsValidState(state), $"Invalid state: {state}");
            
            return state switch
            {
                "Normal" => Normal,
                "Highlighted" => Highlighted,
                "Pressed" => Pressed,
                "Disabled" => Disabled,
                "Selected" => Selected,
                _ => Normal
            };
        }
        
        /// <summary>
        /// Returns whether the specified state is a valid state that the theme can be in.
        /// </summary>
        public static bool IsValidState(string state)
        {
            return _validStates.Contains(state);
        }
    }
}