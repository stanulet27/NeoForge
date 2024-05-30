using UnityEngine;

namespace DeformationSystem
{
    public class PartBoundsLocker : MonoBehaviour
    {
        [Tooltip("X, Y, Z bounds. Will prevent the part from moving outside of these bounds. Based in world origin")]
        [SerializeField] private FloatRange[] _bounds;
        
        [Tooltip("If true, the bounds will be based on the starting position of the object.")]
        [SerializeField] private bool _fromStartingPosition;

        private Vector3 _startingPosition;
        
        private void Awake()
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
            var offset = _fromStartingPosition ? _startingPosition : Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                position[i] = Mathf.Clamp(position[i], _bounds[i].Min + offset[i], _bounds[i].Max + offset[i]);
            }
            transform.position = position;
        }
    }
}