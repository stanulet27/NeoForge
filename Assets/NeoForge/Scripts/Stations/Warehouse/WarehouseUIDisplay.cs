﻿using System;
using NeoForge.Deformation;
using NeoForge.Input;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class WarehouseUIDisplay : MonoBehaviour
    {
        private const string RECIPE_LABEL = "Recipe: {0}";
        private const string EMPTY_RECIPE = "None";

        [Tooltip("The HUD that displays the recipe and craft button.")]
        [SerializeField] private GameObject _hud;
        [Tooltip("The UI that displays the recipe book.")]
        [SerializeField] private GameObject _ui;
        [Tooltip("The text label that displays the current recipe.")]
        [SerializeField] private TMP_Text _recipeLabel;
        [Tooltip("The button that allows the player to craft the selected recipe.")]
        [SerializeField] private SimpleButton _craftButton;
        [Tooltip("The starting part selection display")]
        [SerializeField] private WarehousePartSelector _partSelector;
        
        private Action<CraftableParts> _onRecipeSet;
        private Action<MaterialItem> _onPartSelected;
        private static string FormattedRecipeDisplay(string recipe) => string.Format(RECIPE_LABEL, recipe);

        /// <summary>
        /// Will open the UI.
        /// When the player selects a recipe, the onRecipeSet action will be invoked with the selected recipe.
        /// </summary>
        public void OpenUI(Action<CraftableParts> onRecipeSet, Action<MaterialItem> onPartSelected)
        {
            _hud.SetActive(true);
            _onRecipeSet = onRecipeSet;
            _onPartSelected = onPartSelected;
            _recipeLabel.text = FormattedRecipeDisplay(EMPTY_RECIPE);
        }

        /// <summary>
        /// Will open the recipe book.
        /// </summary>
        public void OpenRecipeBook()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _ui.SetActive(true);
            _hud.SetActive(false);
        }

        /// <summary>
        /// Will close the UI.
        /// </summary>
        public void CloseUI()
        {
            _ui.SetActive(false);
            _hud.SetActive(false);
        }

        /// <summary>
        /// Will set the craft button to be interactable or not.
        /// </summary>
        public void SetCanCraft(bool canCraft)
        {
            _craftButton.ToggleInteractable(canCraft);
        }

        /// <summary>
        /// Will select the recipe and close the recipe book.
        /// Will invoke the onRecipeSet action with the selected recipe.
        /// Will display the selected recipe in the HUD.
        /// </summary>
        public void SelectRecipe(CraftableParts recipe)
        {
            _onRecipeSet?.Invoke(recipe);
            _recipeLabel.text = FormattedRecipeDisplay(recipe.ToString());
            _ui.SetActive(false);
            _hud.SetActive(true);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }

        /// <summary>
        /// Opens the part selector. Will close the part selector when a part is selected.
        /// If a part is selected, the onPartSelected action will be invoked with the selected part.
        /// Will swap the controller mode to UI and hide the HUD.
        /// </summary>
        public void OpenPartSelector()
        {
            _partSelector.OpenDisplay(_onPartSelected, ClosePartSelector);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _hud.SetActive(false);
        }

        private void ClosePartSelector()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
            _hud.SetActive(true);
        }
    }
}