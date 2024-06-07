using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NeoForge.SaveSystem
{
    public class SaveDisplayButtonUI : MonoBehaviour
    {
        private static readonly Dictionary<int, string> _sceneNames = new() {{1, "Workshop"}, {2, "Night Market"}};

        [Tooltip("Used to display the scene name.")]
        [SerializeField] private TMP_Text _sceneName;
        [Tooltip("When true, the text will have separate sizes for day and scene.")]
        [SerializeField] private bool _textIsSkewed;
        
        [Header("Non-Skew Text Properties")]
        [Tooltip("Used to display the day number. If not set, the day will be displayed in the scene name.")]
        [SerializeField] private TMP_Text _day;
        
        [Header("Skew Text Properties")]
        [Tooltip("The size of the day text when skewing.")]
        [SerializeField] private float _daySize = 80f;
        [Tooltip("The size of the scene text when skewing.")]
        [SerializeField] private float _sceneSize = 60f;

        /// <summary>
        /// Will update the text fields to display the save information.
        /// </summary>
        public void Display(Save save)
        {
            if (_day != null)
            {
                FormatAsSeparateTextFields(save);
            }
            else
            {
                FormatAsCombinedTextFields(save);
            }
        }
        
        private void FormatAsSeparateTextFields(Save save)
        {
            _day.text = save.IsEmpty ? "Empty" : $"Day {ConvertNumToText(save.Day)}";
            _sceneName.text = save.IsEmpty ? "Empty" : _sceneNames[save.SceneIndex];
            _sceneName.enabled = !save.IsEmpty;
        }

        private void FormatAsCombinedTextFields(Save save)
        {
            _sceneName.text = save.IsEmpty ? "Empty" :
                _textIsSkewed ? ApplySkewedFormat(save) : ApplyCombinedFormat(save);
        }

        private static string ApplyCombinedFormat(Save save)
        {
            return $"Day {ConvertNumToText(save.Day)}\n{_sceneNames[save.SceneIndex]}";
        }

        private string ApplySkewedFormat(Save save)
        {
            return $"<size={_daySize}>Day {ConvertNumToText(save.Day)}</size>\n" +
                   $"<size={_sceneSize}>{_sceneNames[save.SceneIndex]}</size>";
        }

        private static string ConvertNumToText(int num)
        {
            return num switch
            {
                1 => "One",
                2 => "Two",
                3 => "Three",
                4 => "Four",
                5 => "Five",
                6 => "Six",
                7 => "Seven",
                8 => "Eight",
                9 => "Nine",
                10 => "Ten",
                _ => "Unknown"
            };
        }
    }
}