using System;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using NeoForge.UI.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NeoForge.UI.Menus
{
    public class ShopButton : HoverButton, ISelectHandler
    {
        [Tooltip("The text that appears on the left side of the button")]
        [SerializeField] TMP_Text _leftText;
        
        [Tooltip("The text that appears on the right side of the button")]
        [SerializeField] TMP_Text _rightText;
        
        [Tooltip("The text that appears in the center of the button")]
        [SerializeField] TMP_Text _centerText;
        
        [Tooltip("The color theme to use when the button is interactable")]
        [SerializeField] ButtonColorer _themeWhenInteractable;
        
        [Tooltip("The color theme to use when the button is not interactable")]
        [SerializeField] ButtonColorer _themeWhenNotInteractable;

        private Action _action;
        private Action _onSelected;
        private bool _isInteractable;
        
        /// <summary>
        /// Sets up the button to be used to navigate to a different page
        /// </summary>
        /// <param name="text">The string to display in the center of the button</param>
        /// <param name="action">The method to invoke upon being clicked</param>
        public void SetupNavigate(string text, Action action)
        {
            gameObject.SetActive(true);
            _leftText.text = "";
            _rightText.text = "";
            _centerText.text = text;
            _action = action;
            
            SetClickable(true);
        }

        /// <summary>
        /// Sets up the button to be used to buy an item. Will grey out if the player does not have enough gold.
        /// </summary>
        /// <param name="itemName">The name of the item, will be displayed to the left of the button</param>
        /// <param name="cost">The amount it costs, will be displayed to the right of the button</param>
        /// <param name="action">The method to invoke upon being clicked</param>
        /// <param name="onSelected">The method to invoke when the button is selected in the game</param>
        public void SetupBuy(string itemName, int cost, Action action, Action onSelected)
        {
            gameObject.SetActive(true);
            _leftText.text = itemName;
            _rightText.text = $"GP: {cost}";
            _centerText.text = "";
            _action = action;
            _onSelected = onSelected;
            
            SetClickable(cost <= InventorySystem.Instance.CurrentGold);
        }

        /// <summary>
        /// Will cause the button to no longer be visible or interactable
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Will invoke the action assigned to the button
        /// </summary>
        public override void Use()
        {
            _action?.Invoke();
        }
        
        /// <summary>
        /// Invoked when the button is selected in the game
        /// </summary>
        public void OnSelect(BaseEventData eventData)
        {
            _onSelected?.Invoke();
        }

        private void SetClickable(bool isClickable)
        {
            _isInteractable = isClickable;
            _themeWhenInteractable.enabled = isClickable;
            _themeWhenNotInteractable.enabled = !isClickable;
        }
    }
}