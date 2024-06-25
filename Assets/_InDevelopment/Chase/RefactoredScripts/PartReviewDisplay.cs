using System;
using NeoForge.Stations.Warehosue;
using NeoForge.UI.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NeoForge.DaySystem
{
    public class PartReviewDisplay : MonoBehaviour
    {
        [Tooltip("The text field that will display the part name.")]
        [SerializeField] private TMP_Text _nameTextField;
        [Tooltip("The image renderer that will display the part mesh.")]
        [SerializeField] private RawImage _imageRenderer;
        [Tooltip("The part viewer that will record the part mesh.")]
        [SerializeField] private PartViewer _partViewer;

        private void Start()
        {
            _imageRenderer.texture = _partViewer.Setup();
        }
        
        /// <summary>
        /// Will display the given item in the review display and make the display visible.
        /// </summary>
        public void Display(CompletedItem item)
        {
            _nameTextField.text = item.Name;
            _partViewer.DisplayPart(item.Mesh);
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Will hide the review display and clear the displayed item.
        /// </summary>
        public void Hide()
        {
            _nameTextField.text = "";
            _partViewer.DisplayPart(null);
            gameObject.SetActive(false);
        }
    }
}