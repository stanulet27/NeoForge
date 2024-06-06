using System;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class UIButton : MonoBehaviour, IButton
    {
        public event Action<IButton> OnClick;
        
        private Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Click);
        }

        /// <summary>
        /// Invokes the OnClick event and performs the use method of the given implementation of UIButton.
        /// </summary>
        public void Click()
        {
            OnClick?.Invoke(this);
            Use();
        }

        /// <summary>
        /// Will cause the button to be selected.
        /// </summary>
        public virtual void Select()
        {
            _button.Select();
        }

        public abstract void Use();
    }
}