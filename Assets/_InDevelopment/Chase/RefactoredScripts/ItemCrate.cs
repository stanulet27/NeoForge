using System;
using NeoForge.UI.Inventory;
using NeoForge.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehosue
{
    public class ItemCrate : MonoBehaviour
    {
        public Action<ItemCrate> OnSelected;

        [Tooltip("The item that this crate displays.")]
        [SerializeField] private ItemBase _item;
        [Tooltip("The text label that displays the item name and quantity, Format: {ItemName} (x{Quantity})")]
        [SerializeField] private TMP_Text _label;
        [Tooltip("The mesh renderer that displays the highlight effect.")]
        [SerializeField] private MeshRenderer _highlighter;
        [Tooltip("The material to use when the item is not selected.")]
        [SerializeField] private Material _normalMaterial;
        [Tooltip("The material to use when the item is selected.")]
        [SerializeField] private Material _highlightMaterial;
        [Tooltip("The material to use when the item is empty.")]
        [SerializeField] private Material _emptyMaterial;
        [Tooltip("The mouse events component that allows the item to be selected by the mouse.")]
        [SerializeField] private OnMouseEvents _onMouseEvents;

        private Material _unselectedMaterial;
        
        /// <summary>
        /// The item that this crate displays.
        /// </summary>
        public ItemBase Item => _item;

        /// <summary>
        /// Will refresh the crate's display of the item. This will update the quantity and name of the item.
        /// Also will update the material of the highlighter to reflect if the item is empty or not.
        /// </summary>
        /// <param name="shouldDeselect">If set to true, will also call Deselect()</param>
        public void RefreshDisplay(bool shouldDeselect = false)
        {
            var total = InventorySystem.Instance.CountItems(_item);
            _label.text = $"{_item.Name}\n(x{total})";
            _unselectedMaterial = total > 0 ? _normalMaterial : _emptyMaterial;
            if (_highlightMaterial != _highlighter.material) _highlighter.material = _unselectedMaterial;
            if (shouldDeselect) Deselect();
            _onMouseEvents.enabled = total > 0;
        }

        /// <summary>
        /// Will invoke the OnSelected event and change the highlight material to the selected material.
        /// </summary>
        public void Select()
        {
            _highlighter.material = _highlightMaterial;
            OnSelected?.Invoke(this);
        }
        
        /// <summary>
        /// Will remove the selection highlight and change the highlight material to the unselected material.
        /// </summary>
        public void Deselect()
        {
            _highlighter.material = _unselectedMaterial;
        }
    }
}