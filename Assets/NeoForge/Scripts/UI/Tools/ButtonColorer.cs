﻿using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.UI.Tools
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimatorEventListener))]
    public class ButtonColorer : MonoBehaviour
    {
        [Tooltip("The graphics to change the color of. The key is the graphic and the value is the color theme.")]
        [SerializeField] private SerializedDictionary<MaskableGraphic, ColorTheme> _graphics;

        private bool _isTransitioning;
        private AnimatorEventListener _broadcaster;
        
        public void Awake()
        {
            SetColor("Normal");
            _broadcaster = GetComponent<AnimatorEventListener>();
        }

        private void OnEnable()
        {
            _broadcaster.OnStateEnter += OnAnimatorStateEnter;
            OnAnimatorStateEnter(_broadcaster.CurrentState);
        }

        private void OnDisable()
        {
            _broadcaster.OnStateEnter -= OnAnimatorStateEnter;
        }

        /// <summary>
        /// Will change the color of each of its graphics to their corresponding theme color for the given state.
        /// </summary>
        /// <param name="stateName">The name of the state to transition to</param>
        public void OnAnimatorStateEnter(string stateName)
        {
            if (!ColorTheme.IsValidState(stateName)) return;
            SetColor(stateName);
        }

        private void SetColor(string state)
        {
            foreach (var graphic in _graphics.Keys)
            {
                graphic.color = _graphics[graphic].GetValueFromState(state);
            }
        }
    }
}