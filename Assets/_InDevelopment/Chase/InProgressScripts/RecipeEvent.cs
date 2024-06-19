using NeoForge.Deformation;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Warehouse
{
    public class RecipeEvent : MonoBehaviour
    {
        [SerializeField] PartDetails.DesiredOption _recipe;
        [SerializeField] UnityEvent<PartDetails.DesiredOption> _onRecipeSelected;
        
        public void SelectRecipe()
        {
            _onRecipeSelected.Invoke(_recipe);
        }
    }
}