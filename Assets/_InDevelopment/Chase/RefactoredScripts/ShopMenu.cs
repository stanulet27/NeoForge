using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Dialogue;
using NeoForge.Input;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class ShopMenu : MenuBase
    {
        const string MAIN_PAGE = "Market";
        
        protected override Action OpenTrigger { get; set; }
        protected override Action CloseTrigger { get => ControllerManager.OnClose; set => ControllerManager.OnClose = value; }
        
        [Tooltip("The camera that is used for the shop")]
        [SerializeField] private Camera _shopCamera;
        
        [Tooltip("The text field that displays the title of the shop page")]
        [SerializeField] private TMP_Text _titleField;
        
        [Tooltip("The text field that displays the player's current gold")]
        [SerializeField] private TMP_Text _goldField;
        
        [Tooltip("The text field that displays the description of the item")]
        [SerializeField] private TMP_Text _descriptionField;
        
        [Tooltip("The container for the description field")]
        [SerializeField] private GameObject _descriptionContainer;
        
        [Tooltip("The dialogue that will play when the player exits the shop")]
        [SerializeField] private string _exitDialogue;
        
        [Tooltip("The parent object that holds all of the shop buttons")]
        [SerializeField] private ButtonGroup _buttonParent;
        
        [Tooltip("The pages of the shop and the items that are available on those pages")]
        [SerializeField] private SerializedDictionary<string, ShopItemList> _shopPages = new();

        private readonly List<ShopButton> _buttons = new();
        private string _currentShopPage;

        protected override void Awake()
        {
            base.Awake();
            _buttonParent.GetComponentsInChildren(_buttons);
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnGoBack -= TryGoBack;
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            _shopCamera.rect = new Rect(0, 0, 0.65f, 1);
            GotoPage(MAIN_PAGE);
            ControllerManager.OnGoBack += TryGoBack;
        }
        
        public override void CloseMenu()
        {
            base.CloseMenu();
            _shopCamera.rect = new Rect(0, 0, 1, 1);
            ControllerManager.OnGoBack -= TryGoBack;
            if (!string.IsNullOrWhiteSpace(_exitDialogue)) DialogueManager.Instance.StartDialogueName(_exitDialogue);
        }
        
        private void TryGoBack()
        {
            if (_currentShopPage == MAIN_PAGE) CloseMenu();
            else GotoPage(MAIN_PAGE);
        }

        private void GotoPage(string page)
        {
            _titleField.text = page;
            _currentShopPage = page;
            RefreshPage();
            _buttonParent.SelectFirst();
        }

        private void RefreshPage()
        {
            UpdateGoldField();
            
            if (_currentShopPage == MAIN_PAGE) SetupPage(_shopPages.Keys.ToList(), SetupButtonsForNavigation, "Exit");
            else SetupPage(_shopPages[_currentShopPage].ShopItems, SetupButtonsForPurchase, "Back");
        }
        
        private void UpdateGoldField()
        {
            _goldField.text = $"Current GP: {InventorySystem.Instance.CurrentGold.ToString()}";
        }
        
        private void SetupPage<T>(List<T> entries, Action<List<T>> setupAction, string exitText)
        {
            while (_buttons.Count <= entries.Count) CreateNewButton();
            setupAction(entries);
            _descriptionContainer.SetActive(_currentShopPage != MAIN_PAGE);
            _buttons[entries.Count].SetupNavigate(exitText, TryGoBack);
            for (var i = entries.Count + 1; i < _buttons.Count; i++) _buttons[i].Hide();
        }
        
        private void SetupButtonsForNavigation(List<string> pages)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                var page = pages[i];
                _buttons[i].SetupNavigate(page, () => GotoPage(page));
            }
        }

        private void SetupButtonsForPurchase(List<ItemBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                _buttons[i].SetupBuy(item.Name, item.Cost, 
                    () => AttemptPurchase(item), () => _descriptionField.text = item.Description);
            }
        }

        private void AttemptPurchase(ItemBase item)
        {
            if (InventorySystem.Instance.CurrentGold < item.Cost) return;
            InventorySystem.Instance.CurrentGold -= item.Cost;
            InventorySystem.Instance.AddItem(item);
            RefreshPage();
        }

        private void CreateNewButton()
        {
            var prefab = _buttons[0].gameObject;
            var newButton = Instantiate(prefab, _buttonParent.transform).GetComponent<ShopButton>(); 
            _buttons.Add(newButton);
            newButton.Hide();
        }
    }
}