using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeoForge.UI.Buttons
{
    public class ButtonGroup : MonoBehaviour
    {
        private List<IButton> _buttons;
    
        private void Awake()
        {
            _buttons = GetComponentsInChildren<IButton>().ToList();
        }
        
        private void OnEnable()
        {
            SetButton(_buttons.FindIndex(x => x.IsInteractable()));
        }
        
        /// <summary>
        /// Will select the first child that implements IButton.
        /// </summary>
        public void SelectFirst()
        {
            GetComponentInChildren<IButton>().Select();
        }

        private void SetButton(int index)
        {
            Debug.Assert(index >= 0 && index < _buttons.Count, "Invalid button index");
            _buttons[index].Select();
        }
    }
}
