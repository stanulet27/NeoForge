using System.Collections.Generic;
using UnityEngine;

namespace DeformationSystem
{
    [CreateAssetMenu(fileName = "MoveableConfig", menuName = "DeformationSystem/MoveableConfig")]
    public class MoveableConfig : ScriptableObject
    {
        [Tooltip("The speed at which the object moves.")]
        [SerializeField] private float _moveSpeed = 1f;
        
        [Tooltip("The speed at which the object rotates around its pivot.")]
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("Speed Up")]
        [Tooltip("The key used to speed up the movement and rotation.")]
        [SerializeField] private KeyCode _speedUpKey = KeyCode.LeftShift;
        [Tooltip("The amount move speed is sped up")]
        [SerializeField] private float _movementSpeedUp = 2f;
        [Tooltip("The amount rotational speed is sped up")]
        [SerializeField] private float _rotationalSpeedUp = 2f;

        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public KeyCode SpeedUpKey => _speedUpKey;
        public float MovementSpeedUp => _movementSpeedUp;
        public float RotationalSpeedUp => _rotationalSpeedUp;
        public List<InputBinding> MovementBindings { get; } = new()
        {
            new InputBinding(KeyCode.W, Vector3.back),
            new InputBinding(KeyCode.S, Vector3.forward),
            new InputBinding(KeyCode.A, Vector3.right),
            new InputBinding(KeyCode.D, Vector3.left),
        };

        public List<InputBinding> RotationBindings { get; } = new()
        {
            new InputBinding(KeyCode.Q, Vector3.forward),
            new InputBinding(KeyCode.E, Vector3.back),
            new InputBinding(KeyCode.Z, Vector3.up),
            new InputBinding(KeyCode.X, Vector3.down),
            new InputBinding(KeyCode.R, Vector3.left),
            new InputBinding(KeyCode.T, Vector3.right),
        };
    }
}