using UnityEngine;

namespace NeoForge.Utilities.Movement
{
    public class PartBoundsLocker : MonoBehaviour
    {
        [Tooltip("The reference to the boundary setup for the bounds locker to use.")]
        [SerializeField] private BoundsConfig _bounds;

        private Vector3 _startingPosition;
        
        private void OnEnable()
        {
            _startingPosition = transform.position;
        }
        
        private void Update()
        {
            ClampPosition();
        }
        
        private void ClampPosition()
        {
            var position = transform.position;
            var offset = _bounds.FromStartingPosition ? _startingPosition : Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                position[i] = Mathf.Clamp(position[i], _bounds[i].Min + offset[i], _bounds[i].Max + offset[i]);
            }
            transform.position = position;
        }
    }
}