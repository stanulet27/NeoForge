using NeoForge.Deformation;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.Stations.Warehouse
{
    public class RecipeEvent : MonoBehaviour
    {
        [Tooltip("The recipe that this event is associated with.")]
        [SerializeField] private CraftableParts _recipe;
        [Tooltip("The event that will be invoked when this recipe is selected.")]
        [SerializeField] private UnityEvent<CraftableParts> _onRecipeSelected;
        
        /// <summary>
        /// Will invoke the OnRecipeSelected event with the recipe that this event is associated with.
        /// </summary>
        public void SelectRecipe()
        {
            _onRecipeSelected.Invoke(_recipe);
        }
    }
}