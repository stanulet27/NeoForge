using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class Moveable : MonoBehaviour
    {
        [Tooltip("The speed at which the object moves.")]
        [SerializeField] private float _moveSpeed = 1f;
        
        [Tooltip("The speed at which the object rotates around its pivot.")]
        [SerializeField] private float _rotationSpeed = 10f;

        private readonly List<InputBinding> _movementBindings = new()
        {
            new InputBinding(KeyCode.W, Vector3.back),
            new InputBinding(KeyCode.S, Vector3.forward),
            new InputBinding(KeyCode.A, Vector3.right),
            new InputBinding(KeyCode.D, Vector3.left),
        };

        private readonly List<InputBinding> _rotationBindings = new()
        {
            new InputBinding(KeyCode.Q, Vector3.forward),
            new InputBinding(KeyCode.E, Vector3.back),
            new InputBinding(KeyCode.Z, Vector3.up),
            new InputBinding(KeyCode.X, Vector3.down),
            new InputBinding(KeyCode.R, Vector3.left),
            new InputBinding(KeyCode.T, Vector3.right),
        };
        
        private void Update()
        {
            var moveDirection = Vector3.zero;
            var rotationDirection = Vector3.zero;

            moveDirection = _movementBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(moveDirection, (current, binding) => current + binding.Direction);
            
            rotationDirection = _rotationBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(rotationDirection, (current, binding) => current + binding.Direction);
            
            transform.position += moveDirection * (_moveSpeed * Time.deltaTime);
            transform.Rotate(rotationDirection * (_rotationSpeed * Time.deltaTime));
        }

        private class InputBinding
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
}