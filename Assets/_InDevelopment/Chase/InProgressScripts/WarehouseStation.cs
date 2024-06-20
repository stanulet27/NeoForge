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
        private ItemCrate _materialCrate;
        private ItemCrate _bonusCrate;
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
                case MaterialItem:
                {
                    if (_materialCrate != null && _materialCrate != crate) _materialCrate.Deselect();
                    _materialCrate = crate;
                    break;
                }
                case ItemWithBonus:
                {
                    if (_bonusCrate != null && _bonusCrate != crate) _bonusCrate.Deselect();
                    _bonusCrate = crate;
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
            _materialCrate = null;
            _bonusCrate = null;
            _selectedRecipe = PartDetails.DesiredOption.None;
        }

        [Button]
        public void SendItemToForge()
        {
            var forgedPart = _forgedPartsPool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (_materialCrate.Item is not MaterialItem m || _bonusCrate.Item is not ItemWithBonus b 
                                                                  || forgedPart == default) return;
            
            var partDetails = new PartDetails(m.StartingMesh, m.Data, _selectedRecipe, b);
            
            forgedPart.Details = partDetails;
            forgedPart.gameObject.SetActive(true);
            
            InventorySystem.Instance.RemoveItem(m);
            InventorySystem.Instance.RemoveItem(b);
            ClearCrateSelection();
            RefreshCraftability();
        }

        private void ClearCrateSelection()
        {
            _bonusCrate.Deselect();
            _materialCrate.Deselect();
            _bonusCrate.RefreshDisplay();
            _materialCrate.RefreshDisplay();
            _bonusCrate = null;
            _materialCrate = null;
        }
        
        private void SelectedRecipe(PartDetails.DesiredOption recipe)
        {
            _selectedRecipe = recipe;
            RefreshCraftability();
        }

        private bool CanCraft()
        {
            return _materialCrate != null && _bonusCrate != null && _selectedRecipe != PartDetails.DesiredOption.None;
        }
        
        private void RefreshCraftability()
        {
            _uiDisplay.SetCanCraft(CanCraft());
        }
    }
}