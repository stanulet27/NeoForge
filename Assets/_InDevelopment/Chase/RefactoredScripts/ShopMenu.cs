using System;
using System.Collections.Generic;
using NeoForge.Dialogue;
using NeoForge.Input;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class ShopMenu : MenuBase
    {
        private enum ShopPage { Main, Materials, Equipment }
        protected override Action OpenTrigger { get; set; }
        protected override Action CloseTrigger { get; set; }
        
        [SerializeField] private Camera _shopCamera;
        [SerializeField] private TMP_Text _titleField;
        [SerializeField] private string _exitDialogue;
        [SerializeField] private GameObject _buttonParent;
        
        private List<ShopButton> _buttons = new();
        private ShopPage _currentShopPage;
        
        protected override void Awake()
        {
            base.Awake();
            _buttonParent.GetComponentsInChildren(_buttons);
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            //Set the camera width to 0.65f
            _shopCamera.rect = new Rect(0, 0, 0.65f, 1);
            GotoPage(ShopPage.Main);
            ControllerManager.OnCancel += TryClose;
        }

        private void TryClose()
        {
            if (_currentShopPage == ShopPage.Main) CloseMenu();
            else GotoPage(ShopPage.Main);
        }

        private void GotoPage(ShopPage page)
        {
            _titleField.text = page.ToString();
            switch (page)
            {
                case ShopPage.Main:
                    SetupMainButtons();
                    break;
                case ShopPage.Materials:
                    SetupDemo();
                    break;
                case ShopPage.Equipment:
                    SetupDemo();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }

        private void SetupMainButtons()
        {
            _buttons[0].SetupNavigate("Materials", () => GotoPage(ShopPage.Materials));
            _buttons[1].SetupNavigate("Equipment", () => GotoPage(ShopPage.Equipment));
            _buttons[2].SetupNavigate("Exit", TryClose);
            for (int i = 3; i < _buttons.Count; i++) _buttons[i].Hide();
        }

        private void SetupDemo()
        {
            _buttons[0].SetupBuy("Sword", 100, () => Debug.Log("Bought sword"));
            _buttons[1].SetupBuy("Shield", 50, () => Debug.Log("Bought shield"));
            _buttons[2].SetupBuy("Potion", 10, () => Debug.Log("Bought potion"));
            for (int i = 3; i < _buttons.Count; i++) _buttons[i].Hide();
        }
        
        public override void CloseMenu()
        {
            base.CloseMenu();
            //Set the camera width to 1f
            _shopCamera.rect = new Rect(0, 0, 1, 1);
            ControllerManager.OnCancel -= TryClose;
            if (!string.IsNullOrWhiteSpace(_exitDialogue)) DialogueManager.Instance.StartDialogueName(_exitDialogue);
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnCancel -= TryClose;
        }
    }
}