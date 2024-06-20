using NeoForge.Deformation;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Warehouse
{
    public class RecipeEvent : MonoBehaviour
    {
        [SerializeField] CraftableParts _recipe;
        [SerializeField] UnityEvent<CraftableParts> _onRecipeSelected;
        
        public void SelectRecipe()
        {
            _onRecipeSelected.Invoke(_recipe);
        }
    }
}