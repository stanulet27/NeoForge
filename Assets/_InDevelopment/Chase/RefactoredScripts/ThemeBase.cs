using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public abstract class ThemeBase<T> : ScriptableObject
    {
        private static readonly List<string> _validStates = new() 
            { "Normal", "Highlighted", "Pressed", "Disabled", "Selected" };
        
        public T Normal;
        public T Highlighted;
        public T Pressed;
        public T Disabled;
        public T Selected;

        public T GetValueFromState(string state)
        {
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
        
        public static bool IsValidState(string state)
        {
            return _validStates.Contains(state);
        }
    }
}