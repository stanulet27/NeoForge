using System;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class ShopButton : HoverButton
    {
        [SerializeField] TMP_Text _leftText;
        [SerializeField] TMP_Text _rightText;
        [SerializeField] TMP_Text _centerText;

        private Action _action;
        
        public void SetupNavigate(string page, Action action)
        {
            gameObject.SetActive(true);
            _leftText.text = "";
            _rightText.text = "";
            _centerText.text = page;
            _action = action;
        }
        
        public void SetupBuy(string itemName, int cost, Action action)
        {
            gameObject.SetActive(true);
            _leftText.text = itemName;
            _rightText.text = $"GP: {cost}";
            _centerText.text = "";
            _action = action;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Use()
        {
            _action?.Invoke();
        }
    }
}