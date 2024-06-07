using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoForge.Input
{
    public abstract class ImageBasedOnInputSourceBase<T> : MonoBehaviour
    {
        [Tooltip("The sprite to display when a controller is connected.")]
        [SerializeField] private Sprite _controllerSprite;
        [Tooltip("The sprite to display when no controller is connected.")]
        [SerializeField] private Sprite _keyboardSprite;

        protected T Display;

        protected abstract void SetDisplay(Sprite sprite);

        private void Awake()
        {
            Display = GetComponent<T>();
        }

        private void Start()
        {
            SetDisplay(Gamepad.current != null ? _controllerSprite : _keyboardSprite);
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            SetDisplay(device is Gamepad ? _controllerSprite : _keyboardSprite);
        }
    }
}