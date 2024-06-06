using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    [CreateAssetMenu(fileName = "ColorTheme", menuName = "UI/ColorTheme")]
    public class ColorTheme : ScriptableObject
    {
        private static readonly List<string> _validStates = new() 
            { "Normal", "Highlighted", "Pressed", "Disabled", "Selected" };
        
        public Color NormalColor;
        public Color HighlightedColor;
        public Color PressedColor;
        public Color DisabledColor;
        public Color SelectedColor;

        /// <summary>
        /// Will return the matching color for the given state.
        /// If the state is not found, it will return the NormalColor.
        /// </summary>
        /// <param name="state">The state to get the color corresponding to it</param>
        public Color GetColorFromState(string state)
        {
            return state switch
            {
                "Normal" => NormalColor,
                "Highlighted" => HighlightedColor,
                "Pressed" => PressedColor,
                "Disabled" => DisabledColor,
                "Selected" => SelectedColor,
                _ => NormalColor
            };
        }
        
        /// <summary>
        /// Will return true if ColorTheme.cs has a corresponding color for the given state.
        /// </summary>
        /// <param name="state"></param>
        public static bool IsValidState(string state)
        {
            return _validStates.Contains(state);
        }
    }
}