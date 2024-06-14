using UnityEngine;
using NeoForge.Input;

namespace NeoForge.Utilities.Movement
{
    public class Moveable : MonoBehaviour
    {
        [Tooltip("Configuration for the moveable object.")]
        [SerializeField] private MoveableConfig _config;
        
        [Header("Overrides")]
        [Tooltip("If true, the object will not be able to rotate.")]
        [SerializeField] private bool _rotationLocked;
        [Tooltip("If true, the object will not be able to move.")]
        [SerializeField] private bool _positionLocked;

        public bool InRotationMode { get; private set; }

        private bool _beingSlowed;
        private Vector3 _moveDirection;
        private Vector3 _rotationDirection;
        
        private void OnEnable()
        {
            ControllerManager.OnMove += ApplyMovement;
            ControllerManager.OnRotate += ApplyRotation;
            ControllerManager.OnSwapMode += SwapMode;
            ControllerManager.OnSlowDown += SlowDown;
        }
        
        private void OnDisable()
        {
            ControllerManager.OnMove -= ApplyMovement;
            ControllerManager.OnRotate -= ApplyRotation;
            ControllerManager.OnSwapMode -= SwapMode;
            ControllerManager.OnSlowDown -= SlowDown;
        }
        
        private void SwapMode()
        {
            InRotationMode = !InRotationMode;
            _moveDirection = Vector2.zero;
            _rotationDirection = Vector3.zero;
        }
        
        private void SlowDown(bool spedUp)
        {
            _beingSlowed = spedUp;
        }

        private void Update()
        {
            var moveModifer = _beingSlowed ? _config.MovementSpeedUp : 1f;
            var rotationModifer = _beingSlowed ? _config.RotationalSpeedUp : 1f;
            transform.localPosition += _moveDirection * (_config.MoveSpeed * Time.deltaTime * moveModifer);
            transform.Rotate(_rotationDirection * (_config.RotationSpeed * Time.deltaTime * rotationModifer), Space.Self);
        }

        private void ApplyMovement(Vector2 moveDirection)
        {
            if (_positionLocked || InRotationMode) return;
            _moveDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
        }

        private void ApplyRotation(Vector3 rotationDirection)
        {
            if (_rotationLocked || !InRotationMode) return;
            _rotationDirection = rotationDirection;
        }
    }
}