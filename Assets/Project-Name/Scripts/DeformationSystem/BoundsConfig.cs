using UnityEngine;

namespace DeformationSystem
{
    [CreateAssetMenu(fileName = "BoundsConfig", menuName = "DeformationSystem/BoundsConfig")]
    public class BoundsConfig : ScriptableObject
    {
        [Tooltip("X, Y, Z bounds. Will prevent the part from moving outside of these bounds. Based in world origin")]
        [SerializeField] private FloatRange[] _bounds;
        
        [Tooltip("If true, the bounds will be based on the starting position of the object.")]
        [SerializeField] private bool _fromStartingPosition;
        
        public FloatRange this[int index] => _bounds[index];
        public bool FromStartingPosition => _fromStartingPosition;
    }
}