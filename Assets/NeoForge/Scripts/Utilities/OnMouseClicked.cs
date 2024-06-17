using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.Scripts.Utilities
{
    public class OnMouseClicked : MonoBehaviour
    {
        [Tooltip("Triggers when the mouse is clicked on the object.")]
        [SerializeField] private UnityEvent _onMouseClicked;
        
        private void OnMouseDown()
        {
            _onMouseClicked.Invoke();
        }
    }
}