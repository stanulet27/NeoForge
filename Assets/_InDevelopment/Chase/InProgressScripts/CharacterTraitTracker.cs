using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeoForge.UI.Tools
{
    public class CharacterTraitTracker : MonoBehaviour
    {
        public event Action<CharacterTraitTracker, bool> OnClicked; 

        [Tooltip("The graphic that will have its color changed")]
        [SerializeField] private Graphic _graphic;
        [Tooltip("The color the graphic will have when active")]
        [SerializeField] private Color _onActivate;
        [Tooltip("The color the graphic will have when inactive")]
        [SerializeField] private Color _onDeactivate;

        private void OnEnable()
        {
            Set(isActive: false);
        }

        /// <summary>
        /// The current state of the object
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Will set the state of the object to the given value
        /// </summary>
        public void Set(bool isActive)
        {
            IsActive = isActive;
            _graphic.color = IsActive ? _onActivate : _onDeactivate;
        }
        
        /// <summary>
        /// Will swap the state of the object
        /// </summary>
        public void Click()
        {
            Set(!IsActive);
            OnClicked?.Invoke(this, IsActive);
        }
    }
}