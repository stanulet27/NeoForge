using System;
using NeoForge.UI.Inventory;
using NeoForge.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Warehouse
{
    public class ItemCrate : MonoBehaviour
    {
        public Action<ItemCrate> OnSelected;

        [SerializeField] private ItemBase _item;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private MeshRenderer _highlighter;
        [SerializeField] private Material _normalMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Material _emptyMaterial;
        [SerializeField] private OnMouseEvents _onMouseEvents;

        private Material _unselectedMaterial;
        
        public ItemBase Item => _item;

        public void RefreshDisplay()
        {
            var total = InventorySystem.Instance.CountItems(_item);
            _label.text = $"{_item.Name}\n(x{total})";
            _unselectedMaterial = total > 0 ? _normalMaterial : _emptyMaterial;
            _highlighter.material = _unselectedMaterial;
            _onMouseEvents.enabled = total > 0;
        }

        public void Select()
        {
            _highlighter.material = _highlightMaterial;
            OnSelected?.Invoke(this);
        }
        
        public void Deselect()
        {
            _highlighter.material = _unselectedMaterial;
        }
    }
}