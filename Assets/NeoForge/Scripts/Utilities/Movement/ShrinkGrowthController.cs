using UnityEngine;

namespace NeoForge.Utilities.Movement
{
    public class ShrinkGrowthController : MonoBehaviour
    {
        [Header("Shrink")]
        [Tooltip("Key to start shrinking when pressed")]
        [SerializeField] private KeyCode _shrinkKey = KeyCode.LeftBracket;
        
        [Tooltip("Rate at which the object shrinks")]
        [SerializeField] private Vector3 _shrinkRate = Vector3.one - Vector3.up;
        
        [Header("Growth")]
        [Tooltip("Key to start growing when pressed")]
        [SerializeField] private KeyCode _growKey = KeyCode.RightBracket;
        
        [Tooltip("Rate at which the object grows")]
        [SerializeField] private Vector3 _growthRate = Vector3.one - Vector3.up;
        
        [Header("Bounds")]
        [Tooltip("The minimum scale of the object")]
        [SerializeField] private Vector3 _minScale = Vector3.one;
        
        [Tooltip("The maximum scale of the object")]
        [SerializeField] private Vector3 _maxScale = Vector3.one * 10;

        /*
        private void Update()
        {
            var scale = transform.localScale;
            if (Input.GetKey(_growKey))
            {
                scale += _growthRate * Time.deltaTime;
            }
            else if (Input.GetKey(_shrinkKey))
            {
                scale -= _shrinkRate * Time.deltaTime;
            }
            
            scale = ClampScale(scale);
            transform.localScale = scale;
        }
        */
        
        private Vector3 ClampScale(Vector3 scale)
        {
            for (int i = 0; i < 3; i++)
            {
                scale[i] = Mathf.Clamp(scale[i], _minScale[i], _maxScale[i]);
            }
            return scale;
        }
    }
}