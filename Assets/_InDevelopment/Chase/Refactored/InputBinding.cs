using UnityEngine;

namespace DeformationSystem
{
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