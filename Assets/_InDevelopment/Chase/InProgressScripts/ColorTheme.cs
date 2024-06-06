using UnityEngine;

namespace NeoForge.UITools
{
    [CreateAssetMenu(fileName = "ColorTheme", menuName = "UI/ColorTheme")]
    public class ColorTheme : ScriptableObject
    {
        public Color NormalColor;
        public Color HighlightedColor;
        public Color PressedColor;
        public Color DisabledColor;
        public Color SelectedColor;

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
    }
}