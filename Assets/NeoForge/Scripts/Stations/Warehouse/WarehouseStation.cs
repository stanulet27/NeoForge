using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Deformation;
using NeoForge.UI.Inventory;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class WarehouseStation : MonoBehaviour, IStation
    {
        [Tooltip("The parent object that contains all the crates.")]
        [SerializeField] private GameObject _cratesParent;
        
        [Tooltip("The part selection crate that will display current part selected")]
        [SerializeField] private TMP_Text _partSelectionLabel;
        
        [Tooltip("The UI display for the warehouse station.")]
        [SerializeField] private WarehouseUIDisplay _uiDisplay;

        private List<ItemCrate> _crates;
        private MaterialItem _materialItem;
        private ItemCrate _bonusCrate;
        private CraftableParts _selectedRecipe = CraftableParts.None;
        
        private void Start()
        {
            _crates = _cratesParent.GetComponentsInChildren<ItemCrate>().ToList();
            _partSelectionLabel.text = "Click to select an item to forge";
            _uiDisplay.CloseUI();
        }

        public void EnterStation()
        {
            _crates.ForEach(crate => crate.RefreshDisplay());
            _crates.ForEach(crate => crate.OnSelected += OnCrateSelected);
            _uiDisplay.OpenUI(SelectedRecipe, SetMaterialItem);
            RefreshCraftability();
        }

        public void ExitStation()
        {
            _crates.ForEach(crate => crate.OnSelected -= OnCrateSelected);
            _uiDisplay.CloseUI();
            SetMaterialItem(null);
            _bonusCrate = null;
            _selectedRecipe = CraftableParts.None;
        }

        /// <summary>
        /// Will craft the part using the selected resources, if possible.
        /// Will update the inventory and the forge pool.
        /// Will send the crafted part to the forge.
        /// </summary>
        [Button]
        public void SendItemToForge()
        {
            var forgedPart = ForgePartPool.Instance.GetPart();
            
            if (_materialItem == null || _bonusCrate.Item is not ItemWithBonus bonusItem) return;
            if (forgedPart == default) return;
            
            var partDetails = new PartDetails(_materialItem.Mesh, _materialItem.Data, _selectedRecipe, bonusItem);
            
            forgedPart.Details = partDetails;
            forgedPart.gameObject.SetActive(true);
            
            InventorySystem.Instance.RemoveItem(_materialItem);
            InventorySystem.Instance.RemoveItem(bonusItem);
            ClearCrateSelection();
            RefreshCraftability();
        }
        
        private void OnCrateSelected(ItemCrate crate)
        {
            var crateItem = crate.Item;
            switch (crateItem)
            {
                case ItemWithBonus:
                {
                    if (_bonusCrate != crate && _bonusCrate != null) _bonusCrate.Deselect();
                    _bonusCrate = crate;
                    break;
                }
            }
            RefreshCraftability();
        }

        private void ClearCrateSelection()
        {
            _bonusCrate.RefreshDisplay(shouldDeselect: true);
            _bonusCrate = null;
            SetMaterialItem(null);
        }
        
        private void SelectedRecipe(CraftableParts recipe)
        {
            _selectedRecipe = recipe;
            RefreshCraftability();
        }

        private bool CanCraft()
        {
            return _materialItem != null && _bonusCrate != null && _selectedRecipe != CraftableParts.None;
        }
        
        private void RefreshCraftability()
        {
            _uiDisplay.SetCanCraft(CanCraft());
        }
        
        private void SetMaterialItem(MaterialItem item)
        {
            _materialItem = item;
            RefreshCraftability();
            _partSelectionLabel.text = item == null ? "Click to select an item to forge" : $"Selected: {item.Name}";
        }
    }
}