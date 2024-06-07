using UnityEngine;

namespace NeoForge.Input
{
    public class HideOnMode : MonoBehaviour
    {
        [Tooltip("Which controller input mode should this object be hidden during.")]
        [SerializeField] private ControllerManager.Mode _mode;
        
        private void Awake()
        {
            ControllerManager.OnModeSwapped += ToggleDisplay;
            ToggleDisplay(ControllerManager.Instance.CurrentMode);
        }

        private void ToggleDisplay(ControllerManager.Mode mode)
        {
            if (mode != _mode) Show();
            else Hide();
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            if (ControllerManager.Instance == null) return;
            ControllerManager.OnModeSwapped -= ToggleDisplay;
        }
    }
}