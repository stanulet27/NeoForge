using System;
using System.Collections.Generic;
using System.Linq;
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
            _completedItems = InventorySystem.Instance.GetItems<CompletedItem>()
                .Where(x => x.Value > 0).Select(x => x.Key).ToList();
            
            _buttons[0].gameObject.SetActive(_completedItems.Count > 1);
            _buttons[1].gameObject.SetActive(false);
            
            DisplayItem(0);
            _display.SetActive(true);
        }

        public void ViewSelection(string dialogueEvent)
        {
            View();
            _buttons[1].gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _display.SetActive(false);
        }
        
        public void Next()
        { 
            var nextIndex = (_currentIndex + 1 + _completedItems.Count) % _completedItems.Count;
            DisplayItem(nextIndex);
        }
        
        public void Select()
        {
            InventorySystem.Instance.RemoveItem(_completedItems[_currentIndex]);
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