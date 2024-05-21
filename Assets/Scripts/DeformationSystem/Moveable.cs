using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class Moveable : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotationSpeed = 10f;
        
        private void Update()
        {
            var moveDirection = Vector3.zero;
            var rotationDirection = Vector3.zero;

            var movementBindings = new List<InputBinding>
            {
                new(KeyCode.W, Vector3.back),
                new(KeyCode.S, Vector3.forward),
                new(KeyCode.A, Vector3.right),
                new(KeyCode.D, Vector3.left),
            };
            
            var rotationBindings = new List<InputBinding>
            {
                new(KeyCode.Q, Vector3.forward),
                new(KeyCode.E, Vector3.back),
                new(KeyCode.Z, Vector3.up),
                new(KeyCode.X, Vector3.down),
                new(KeyCode.R, Vector3.left),
                new(KeyCode.T, Vector3.right),
            };

            moveDirection = movementBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(moveDirection, (current, binding) => current + binding.Direction);
            
            rotationDirection = rotationBindings.Where(binding => Input.GetKey(binding.KeyCode))
                .Aggregate(rotationDirection, (current, binding) => current + binding.Direction);
            
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            transform.Rotate(rotationDirection * (rotationSpeed * Time.deltaTime));
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