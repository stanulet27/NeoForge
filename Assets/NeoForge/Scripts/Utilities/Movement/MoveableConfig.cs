using UnityEngine;

namespace NeoForge.Utilities.Movement
{
    [CreateAssetMenu(fileName = "MoveableConfig", menuName = "DeformationSystem/MoveableConfig")]
    public class MoveableConfig : ScriptableObject
    {
        [Tooltip("The speed at which the object moves.")]
        [SerializeField] private float _moveSpeed = 1f;
        
        [Tooltip("The speed at which the object rotates around its pivot.")]
        [SerializeField] private float _rotationSpeed = 10f;
        
        [Tooltip("The amount move speed is sped up")]
        [SerializeField] private float _movementSpeedUp = 2f;
        [Tooltip("The amount rotational speed is sped up")]
        [SerializeField] private float _rotationalSpeedUp = 2f;

        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float MovementSpeedUp => _movementSpeedUp;
        public float RotationalSpeedUp => _rotationalSpeedUp;
    }
}