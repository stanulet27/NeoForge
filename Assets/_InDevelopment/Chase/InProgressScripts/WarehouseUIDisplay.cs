using System;
using NeoForge.Deformation;
using NeoForge.Input;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Warehouse
{
    public class WarehouseUIDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _hud;
        [SerializeField] private GameObject _ui;
        [SerializeField] private TMP_Text _recipeLabel;
        [SerializeField] private SimpleButton _craftButton;
        
        private Action<CraftableParts> _onRecipeSet;

        public void OpenUI(Action<CraftableParts> onRecipeSet)
        {
            _hud.SetActive(true);
            _onRecipeSet = onRecipeSet;
            _recipeLabel.text = "Recipe: None";
        }

        public void OpenRecipeBook()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _ui.SetActive(true);
            _hud.SetActive(false);
        }

        public void CloseUI()
        {
            _ui.SetActive(false);
            _hud.SetActive(false);
        }
        
        public void SetCanCraft(bool canCraft)
        {
            _craftButton.ToggleInteractable(canCraft);
        }

        public void SelectRecipe(CraftableParts recipe)
        {
            _onRecipeSet?.Invoke(recipe);
            _recipeLabel.text = $"Recipe: {recipe}";
            _ui.SetActive(false);
            _hud.SetActive(true);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }
    }
}