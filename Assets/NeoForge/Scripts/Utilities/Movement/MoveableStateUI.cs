using System.Collections;
using NeoForge.Input;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.Utilities.Movement
{
    public class MoveableStateUI : MonoBehaviour
    {
        [Tooltip("The image to display the state of the moveable object.")]
        [SerializeField] private Image _stateDisplay;
        [Tooltip("The sprite to display when the object is in move state.")]
        [SerializeField] private Sprite _moveStateSprite;
        [Tooltip("The sprite to display when the object is in rotate state.")]
        [SerializeField] private Sprite _rotateStateSprite;

        private void OnEnable()
        {
            ControllerManager.OnSwapMode += SwapMode;
        }
        
        private void OnDisable()
        {
            ControllerManager.OnSwapMode -= SwapMode;
        }
        
        private void SwapMode()
        {
            _stateDisplay.sprite = ControllerManager.Instance.InRotationMode ? _rotateStateSprite : _moveStateSprite;
        }
    }
}