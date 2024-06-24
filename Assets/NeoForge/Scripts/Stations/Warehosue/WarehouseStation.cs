using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Deformation;
using NeoForge.UI.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Stations.Warehosue
{
    public class WarehouseStation : MonoBehaviour, IStation
    {
        [Tooltip("The parent object that contains all the crates.")]
        [SerializeField] private GameObject _cratesParent;
        
        [Tooltip("The UI display for the warehouse station.")]
        [SerializeField] private WarehouseUIDisplay _uiDisplay;
        

        private List<ItemCrate> _crates;
        private ItemCrate _materialCrate;
        private ItemCrate _bonusCrate;
        private CraftableParts _selectedRecipe = CraftableParts.None;
        
        private void Start()
        {
            _crates = _cratesParent.GetComponentsInChildren<ItemCrate>().ToList();
            _uiDisplay.CloseUI();
        }

        public void EnterStation()
        {
            _crates.ForEach(crate => crate.RefreshDisplay());
            _crates.ForEach(crate => crate.OnSelected += OnCrateSelected);
            _uiDisplay.OpenUI(SelectedRecipe);
            RefreshCraftability();
        }

        public void ExitStation()
        {
            _crates.ForEach(crate => crate.OnSelected -= OnCrateSelected);
            _uiDisplay.CloseUI();
            _materialCrate = null;
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
            
            if (_materialCrate.Item is not MaterialItem m || _bonusCrate.Item is not ItemWithBonus b) return;
            if (forgedPart == default) return;
            
            var partDetails = new PartDetails(m.StartingMesh, m.Data, _selectedRecipe, b);
            
            forgedPart.Details = partDetails;
            forgedPart.gameObject.SetActive(true);
            
            InventorySystem.Instance.RemoveItem(m);
            InventorySystem.Instance.RemoveItem(b);
            ClearCrateSelection();
            RefreshCraftability();
        }
        
        private void OnCrateSelected(ItemCrate crate)
        {
            var crateItem = crate.Item;
            switch (crateItem)
            {
                case MaterialItem:
                {
                    if (_materialCrate != crate && _materialCrate != null) _materialCrate.Deselect();
                    _materialCrate = crate;
                    break;
                }
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
            _materialCrate.RefreshDisplay(shouldDeselect: true);
            _bonusCrate = null;
            _materialCrate = null;
        }
        
        private void SelectedRecipe(CraftableParts recipe)
        {
            _selectedRecipe = recipe;
            RefreshCraftability();
        }

        private bool CanCraft()
        {
            return _materialCrate != null && _bonusCrate != null && _selectedRecipe != CraftableParts.None;
        }
        
        private void RefreshCraftability()
        {
            _uiDisplay.SetCanCraft(CanCraft());
        }
    }
}