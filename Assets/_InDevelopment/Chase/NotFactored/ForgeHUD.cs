using System;
using TMPro;
using UnityEngine;

namespace _InDevelopment.Chase.Scripts
{
    public class ForgeHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hudDisplay;

        private float _score, _force, _size;

        private void Start()
        {
            RefreshDisplay();
        }

        public void UpdateDisplay(float score = -1f, float force = -1f, float size = -1f)
        {
            _score = score < 0 ? _score : score;
            _force = force < 0 ? _force : force;
            _size = size < 0 ? _size : size;
            
            RefreshDisplay();
        }
        
        private void RefreshDisplay()
        {
            _hudDisplay.text = $"Score: {_score:F2}%\nForce: {_force}\nSize: {_size}";
        }
    }
}