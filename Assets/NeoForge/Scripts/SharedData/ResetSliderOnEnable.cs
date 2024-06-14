using UnityEngine;
using UnityEngine.UI;

namespace SharedData
{
    [RequireComponent(typeof(Slider))]
    public class ResetSliderOnEnable : MonoBehaviour
    {
        [Tooltip("The value that the slider will be set to when enabled.")]
        [SerializeField] private SharedFloat _sharedFloat;

        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        private void OnEnable()
        {
            _slider.value = _sharedFloat.Value;
        }
    }
}