using SharedData;
using TMPro;
using UnityEngine;

namespace NeoForge.Deformation.UI
{
    public class ForgeHUD : MonoBehaviour
    {
        [Tooltip("Text field that displays the HUD for the score, force, and size.")]
        [SerializeField] private TMP_Text _hudDisplay;
        [SerializeField] private SharedFloat _score;

        private float _force, _size;

        private void Start()
        {
            RefreshDisplay();
            _score.OnValueChanged += RefreshDisplay;
        }
        private void OnDestroy()
        {
            _score.OnValueChanged -= RefreshDisplay;
        }

        /// <summary>
        /// Sets the force, and size of the HUD. If a value is not provided, the previous value is used.
        /// Requires: value >= 0
        /// </summary>
        /// <param name="force">Displayed as "Force: X"</param>
        /// <param name="size">Displayed as "Size: X</param>
        public void UpdateDisplay(float force = -1f, float size = -1f)
        {
            if (force >= 0) _force = force;
            if (size >= 0) _size = size;
            
            RefreshDisplay();
        }
        
        private void RefreshDisplay()
        {
            _hudDisplay.text = $"Score: {_score.Value:F2}%" + "\n" +
                               $"Force: {_force:F2}" + "\n" +
                               $"Size: {_size:F2}";
        }
    }
}
