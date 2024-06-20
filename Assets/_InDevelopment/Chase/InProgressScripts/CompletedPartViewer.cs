using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue;
using NeoForge.Input;
using NeoForge.Orders;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Warehouse
{
    public class CompletedPartViewer : MonoBehaviour
    {
        [SerializeField] private GameObject _display;
        [SerializeField] private TMP_Text _partName;
        [SerializeField] private HoverButton _nextButton;
        [SerializeField] private HoverButton _selectButton;
        [SerializeField] private HoverButton _closeButton;
        [SerializeField] private PartViewer _partMeshDisplay;

        private readonly List<HoverButton> _buttons = new();
        private List<CompletedItem> _completedItems = new();
        private int _currentIndex;
        private Order _order;
        private string _dialogueToTrigger = "";

        private void Awake()
        {
            _buttons.Add(_nextButton);
            _buttons.Add(_selectButton);
            _buttons.Add(_closeButton);
        }

        private void Start()
        {
            Hide();
        }
        
        public void View()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _completedItems = InventorySystem.Instance.GetItems<CompletedItem>()
                .Where(x => x.Value > 0).Select(x => x.Key).ToList();
            
            _buttons[0].gameObject.SetActive(_completedItems.Count > 1);
            _buttons[1].gameObject.SetActive(false);
            _order = null;
            _dialogueToTrigger = "";
            
            DisplayItem(0);
            _display.SetActive(true);
        }

        public void ViewSelection(string dialogueEvent)
        {
            View();
            _buttons[1].gameObject.SetActive(_completedItems.Count > 0);
            _order = OrderController.Instance.GetOrder(dialogueEvent.Split("-")[1], dialogueEvent.Split("-")[2]);
            _dialogueToTrigger = $"{_order.GiverName}-Cancel";
        }
        
        public void Hide()
        {
            _display.SetActive(false);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
            if (_dialogueToTrigger != "") DialogueManager.Instance.StartDialogueName(_dialogueToTrigger);
        }
        
        public void Next()
        { 
            var nextIndex = (_currentIndex + 1 + _completedItems.Count) % _completedItems.Count;
            DisplayItem(nextIndex);
        }
        
        public void Select()
        {
            if (_order == null) return;
            var item = _completedItems[_currentIndex];
            var correctPart = item.Goal == _order.ObjectToCraft;
            var success = correctPart && item.Score <= _order.MinimumScore;
            var bonusReached = correctPart && item.Score <= _order.BonusScore;
            
            var suffix = bonusReached ? "SuccessBonus" : success ? "Success" : "Failure";
            _dialogueToTrigger = $"{_order.GiverName}-{suffix}";
            var payment = bonusReached ? _order.PaymentWithBonus : success ? _order.PaymentAmount : 0;
            
            InventorySystem.Instance.CurrentGold += payment;
            if (success) InventorySystem.Instance.RemoveItem(item);
            if (success) OrderController.Instance.CompleteOrder(_order);

            Debug.Log("Selected " + _completedItems[_currentIndex].Name);
            Hide();
        }

        private void DisplayItem(int index)
        {
            if (index < 0 || index >= _completedItems.Count)
            {
                DisplayBlank();
                return;
            }
            
            _currentIndex = index;
            var item = _completedItems[_currentIndex];
            _partName.text = item.Name;
            _partMeshDisplay.DisplayPart(item.Mesh);
        }
        
        private void DisplayBlank()
        {
            _partName.text = "No Parts Made";
            _partMeshDisplay.DisplayPart(null);
        }
    }
}