using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeformationSystem
{
    public class MoveableStateUI : MonoBehaviour
    {
        [Tooltip("The moveable object to display the state of.")]
        [SerializeField] private Moveable _moveable;
        [Tooltip("The image to display the state of the moveable object.")]
        [SerializeField] private Image _stateDisplay;
        [Tooltip("The sprite to display when the object is in move state.")]
        [SerializeField] private Sprite _moveStateSprite;
        [Tooltip("The sprite to display when the object is in rotate state.")]
        [SerializeField] private Sprite _rotateStateSprite;

        private void OnEnable()
        {
            StartCoroutine(ObserveState());
        }
        
        private IEnumerator ObserveState()
        {
            while (true)
            {
                var lastState = _moveable.InRotationMode;
                _stateDisplay.sprite = _moveable.InRotationMode ? _rotateStateSprite : _moveStateSprite;
                yield return new WaitUntil(() => lastState != _moveable.InRotationMode);
            }
        }
    }
}