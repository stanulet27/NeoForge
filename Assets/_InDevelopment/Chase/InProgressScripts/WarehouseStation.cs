using System.Collections.Generic;
using System.Linq;
using NeoForge.Deformation;
using NeoForge.Orders;
using NeoForge.UI.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.UI.Warehouse
{
    public class WarehouseStation : MonoBehaviour, IStation
    {
        [SerializeField] private GameObject _cratesParent;
        [SerializeField] private List<ForgedPart> _forgedPartsPool;
        [SerializeField] private WarehouseUIDisplay _uiDisplay;

        private List<ItemCrate> _crates;
        private ItemCrate _selectedMaterialCrate;
        private ItemCrate _selectedBonusCrate;
        private PartDetails.DesiredOption _selectedRecipe = PartDetails.DesiredOption.None;
        
        private void Start()
        {
            _crates = _cratesParent.GetComponentsInChildren<ItemCrate>().ToList();
            _forgedPartsPool.ForEach(part => part.gameObject.SetActive(false));
            _uiDisplay.CloseUI();
        }

        private void OnCrateSelected(ItemCrate crate)
        {
            var crateItem = crate.Item;
            switch (crateItem)
            {
                case MaterialItem item:
                {
                    if (_selectedMaterialCrate != null) _selectedMaterialCrate.Deselect();
                    _selectedMaterialCrate = crate;
                    break;
                }
                case ItemWithBonus item:
                {
                    if (_selectedBonusCrate != null) _selectedBonusCrate.Deselect();
                    _selectedBonusCrate = crate;
                    break;
                }
            }
            RefreshCraftability();
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
            _selectedMaterialCrate = null;
            _selectedBonusCrate = null;
            _selectedRecipe = PartDetails.DesiredOption.None;
        }

        [Button]
        public void SendItemToForge()
        {
            var forgedPart = _forgedPartsPool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (_selectedMaterialCrate.Item is not MaterialItem m || _selectedBonusCrate.Item is not ItemWithBonus b 
                                                                  || forgedPart == default) return;
            
            var partDetails = new PartDetails(MaterialItem.StartingOption.BasicBar, m.Data, 
                _selectedRecipe, b);
            
            forgedPart.Details = partDetails;
            forgedPart.gameObject.SetActive(true);
            
            InventorySystem.Instance.RemoveItem(m);
            InventorySystem.Instance.RemoveItem(b);
            ClearCrateSelection();
            RefreshCraftability();
        }

        private void ClearCrateSelection()
        {
            _selectedBonusCrate.Deselect();
            _selectedMaterialCrate.Deselect();
            _selectedBonusCrate.RefreshDisplay();
            _selectedMaterialCrate.RefreshDisplay();
            _selectedBonusCrate = null;
            _selectedMaterialCrate = null;
        }
        
        private void SelectedRecipe(PartDetails.DesiredOption recipe)
        {
            _selectedRecipe = recipe;
            RefreshCraftability();
        }

        private bool CanCraft()
        {
            return _selectedMaterialCrate != null && _selectedBonusCrate != null && _selectedRecipe != PartDetails.DesiredOption.None;
        }
        
        private void RefreshCraftability()
        {
            _uiDisplay.SetCanCraft(CanCraft());
        }
    }
}