using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class Moveable : MonoBehaviour
    {
        [Tooltip("Configuration for the moveable object.")]
        [SerializeField] private MoveableConfig _config;
        
        [SerializeField] private bool _rotationLocked;
        [SerializeField] private bool _positionLocked;

        private void Update()
        {
            var moveDirection = Vector3.zero;
            var rotationDirection = Vector3.zero;

            moveDirection = _config.MovementBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(moveDirection, (current, binding) => current + binding.Direction);

            rotationDirection = _config.RotationBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(rotationDirection, (current, binding) => current + binding.Direction);

            if (Input.GetKey(_config.SpeedUpKey))
            {
                moveDirection *= _config.MovementSpeedUp;
                rotationDirection *= _config.RotationalSpeedUp;
            }

            if (!_positionLocked) transform.position += moveDirection * (_config.MoveSpeed * Time.deltaTime);
            if (!_rotationLocked) transform.Rotate(rotationDirection * (_config.RotationSpeed * Time.deltaTime));
        }
    }
    
    public class InputBinding
    {
        public KeyCode KeyCode { get; }
        public Vector3 Direction { get; }
            
        public InputBinding(KeyCode keyCode, Vector3 direction)
        {
            KeyCode = keyCode;
            Direction = direction;
        }
    }
}