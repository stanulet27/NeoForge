using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class WarehousePartSelector : MonoBehaviour
    {
        [SerializeField] private GameObject _display;
        [SerializeField] private TMP_Text _partName;
        [SerializeField] private PartViewer _partViewer;
        [SerializeField] private HoverButton _nextButton;
        [SerializeField] private HoverButton _selectButton;

        private int _index = 0;
        private List<MaterialItem> _inventory;
        private Action<MaterialItem> _onSelect;
        private Action _onClose;

        private void Start()
        {
            Close();
        }

        public void OpenDisplay(Action<MaterialItem> onSelect, Action onClose)
        {
            _inventory = InventorySystem.Instance.GetItems<MaterialItem>().Select(x => x.Key).ToList();
            var inventoryEmpty = _inventory.Count == 0;
            var multipleItems = _inventory.Count > 1;
            _index = 0;
            _partViewer.DisplayPart(inventoryEmpty ? null : _inventory[_index].Mesh);
            _nextButton.gameObject.SetActive(multipleItems);
            _selectButton.gameObject.SetActive(!inventoryEmpty);
            _partName.text = inventoryEmpty ? "No items" : _inventory[_index].Name;
            _onSelect = onSelect;
            _onClose = onClose;
            _display.SetActive(true);
        }

        public void Close()
        {
            _display.SetActive(false);
            _onClose?.Invoke();
        }
        
        public void SelectAndClose()
        {
            if (_inventory.Count > 0) _onSelect?.Invoke(_inventory[_index]);

            Close();
        }

        public void NextItem()
        {
            MoveIndexBy(1);
            DisplayItem(_inventory[_index]);
        }
        
        public void PreviousItem()
        {
            MoveIndexBy(-1);
            DisplayItem(_inventory[_index]);
        }
        
        private void DisplayItem(MaterialItem item)
        {
            _partName.text = item.Name;
            _partViewer.DisplayPart(item.Mesh);
        }
        
        private void MoveIndexBy(int i)
        {
            _index = (_index + i + _inventory.Count) % _inventory.Count;
        }
    }
}