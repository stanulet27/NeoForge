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
        [Tooltip("The display that will show the part selector.")]
        [SerializeField] private GameObject _display;
        [Tooltip("The text label that will display the part name.")]
        [SerializeField] private TMP_Text _partName;
        [Tooltip("The part viewer that will display the part mesh.")]
        [SerializeField] private PartViewer _partViewer;
        [Tooltip("The button that will navigate to the next item.")]
        [SerializeField] private HoverButton _nextButton;
        [Tooltip("The button that will select the current item.")]
        [SerializeField] private HoverButton _selectButton;

        private int _index;
        private List<MaterialItem> _inventory;
        private Action<MaterialItem> _onSelect;
        private Action _onClose;

        private void Start()
        {
            Close();
        }

        /// <summary>
        /// Will open the part selector display.
        /// </summary>
        /// <param name="onSelect">
        /// The method to invoke when an item is selected, will pass the item selected
        /// </param>
        /// <param name="onClose">
        /// The method to invoke when the menu is close, note will invoke no matter if a part is selected
        /// </param>
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

        /// <summary>
        /// Will hide the display from the user and invoke the on close event
        /// </summary>
        public void Close()
        {
            _display.SetActive(false);
            _onClose?.Invoke();
        }
        
        /// <summary>
        /// Will select the current item and close the display. Will invoke the onSelect event
        /// </summary>
        public void SelectAndClose()
        {
            if (_inventory.Count > 0) _onSelect?.Invoke(_inventory[_index]);

            Close();
        }

        /// <summary>
        /// Will navigate to the next item in the inventory
        /// </summary>
        public void NextItem()
        {
            MoveIndexBy(1);
            DisplayItem(_inventory[_index]);
        }
        
        /// <summary>
        /// Will navigate to the previous item in the inventory
        /// </summary>
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
            if (_inventory.Count < 2) return;
            
            _index = (_index + i + _inventory.Count) % _inventory.Count;
        }
    }
}