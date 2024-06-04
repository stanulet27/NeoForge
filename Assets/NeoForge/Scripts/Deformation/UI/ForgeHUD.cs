using TMPro;
using UnityEngine;

namespace NeoForge.Deformation.UI
{
    public class ForgeHUD : MonoBehaviour
    {
        [Tooltip("Text field that displays the HUD for the score, force, and size.")]
        [SerializeField] private TMP_Text _hudDisplay;

        private float _score, _force, _size;

        private void Start()
        {
            RefreshDisplay();
        }

        /// <summary>
        /// Sets the score, force, and size of the HUD. If a value is not provided, the previous value is used.
        /// Requires: value >= 0
        /// </summary>
        /// <param name="score">Displayed as "Score: X%"</param>
        /// <param name="force">Displayed as "Force: X"</param>
        /// <param name="size">Displayed as "Size: X</param>
        public void UpdateDisplay(float score = -1f, float force = -1f, float size = -1f)
        {
            if (score >= 0) _score = score;
            if (force >= 0) _force = force;
            if (size >= 0) _size = size;
            
            RefreshDisplay();
        }
        
        private void RefreshDisplay()
        {
            _hudDisplay.text = $"Score: {_score:F2}%" + "\n" +
                               $"Force: {_force:F2}" + "\n" +
                               $"Size: {_size:F2}";
        }
    }
}
