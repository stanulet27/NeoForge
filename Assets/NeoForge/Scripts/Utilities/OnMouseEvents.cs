using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.Utilities
{
    public class OnMouseEvents : MonoBehaviour
    {
        [Tooltip("Triggers when the mouse is clicked on the object.")]
        [SerializeField] private UnityEvent _onMouseClicked;
        [Tooltip("Triggers when the mouse hovers over the object.")]
        [SerializeField] private UnityEvent _onMouseEnter;
        [Tooltip("Triggers when the mouse stops hovering over the object.")]
        [SerializeField] private UnityEvent _onMouseExit;

        private bool _isMouseOver;
        
        private void OnMouseDown()
        {
            if (enabled) _onMouseClicked.Invoke();
        }
        
        private void OnMouseEnter()
        {
            if (!enabled) return;
            
            _onMouseEnter.Invoke();
            _isMouseOver = true;
        }
        
        private void OnMouseExit()
        {
            if (!enabled) return;
            
            _onMouseExit.Invoke();
            _isMouseOver = false;
        }

        private void OnDisable()
        {
            if (!_isMouseOver) return;
            
            _onMouseExit.Invoke();
            _isMouseOver = false;
        }
    }
}