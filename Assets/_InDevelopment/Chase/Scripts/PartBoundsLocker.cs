using System;
using DeformationSystem;
using UnityEngine;

namespace _InDevelopment.Chase.Scripts
{
    public class PartBoundsLocker : MonoBehaviour
    {
        [SerializeField] private FloatRange[] _bounds;

        private void Update()
        {
            ClampPosition();
        }

        public void SetClampedMovement(FloatRange[] clampedMovement)
        {
            _bounds = clampedMovement;
        }
        
        private void ClampPosition()
        {
            var position = transform.position;
            for (int i = 0; i < 3; i++)
            {
                position[i] = Mathf.Clamp(position[i], _bounds[i].Min, _bounds[i].Max);
            }
            transform.position = position;
        }
    }
}