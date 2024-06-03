using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
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

        private void Update()
        {
            var moveDirection = GetDirection(_config.MovementBindings);
            var rotationDirection = GetDirection(_config.RotationBindings);

            if (Input.GetKey(_config.SpeedUpKey))
            {
                moveDirection *= _config.MovementSpeedUp;
                rotationDirection *= _config.RotationalSpeedUp;
            }

            if (!_positionLocked) ApplyMovement(moveDirection);
            if (!_rotationLocked) ApplyRotation(rotationDirection);
        }

        private static Vector3 GetDirection(IEnumerable<InputBinding> bindings)
        {
            return bindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(Vector3.zero, (current, binding) => current + binding.Direction);
        }

        private void ApplyMovement(Vector3 moveDirection)
        {
            if (!_positionLocked)
            {
                transform.position += moveDirection * (_config.MoveSpeed * Time.deltaTime);
            }
        }

        private void ApplyRotation(Vector3 rotationDirection)
        {
            if (!_rotationLocked)
            {
                transform.Rotate(rotationDirection * (_config.RotationSpeed * Time.deltaTime), Space.World);
            }
        }
    }
}