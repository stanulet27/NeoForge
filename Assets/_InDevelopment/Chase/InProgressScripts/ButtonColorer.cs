using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.UITools
{
    [RequireComponent(typeof(Animator))]
    public class ButtonColorer : MonoBehaviour
    {
        private static readonly List<string> _validStates = new() 
            { "Normal", "Highlighted", "Pressed", "Disabled", "Selected" };

        [SerializeField] private SerializedDictionary<MaskableGraphic, ColorTheme> _graphics;

        private bool _isTransitioning;
        
        public void Awake()
        {
            SetColor("Normal");
        }

        public void OnAnimatorStateEnter(string stateName)
        {
            if (!_validStates.Contains(stateName)) return;
            SetColor(stateName);
        }

        private void SetColor(string state)
        {
            foreach (var graphic in _graphics.Keys)
            {
                graphic.color = _graphics[graphic].GetColorFromState(state);
            }
        }
    }
}