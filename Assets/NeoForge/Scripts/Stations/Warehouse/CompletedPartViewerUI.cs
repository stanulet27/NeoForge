using System.Collections.Generic;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class CompletedPartViewerUI : MonoBehaviour
    {
        [Tooltip("The display GameObject that shows the part viewer UI.")] 
        [SerializeField] private GameObject _display;

        [Tooltip("The TMP_Text component used to display the part name.")] 
        [SerializeField] private TMP_Text _partName;

        [Tooltip("The button to go to the next completed part.")] 
        [SerializeField] private HoverButton _nextButton;

        [Tooltip("The button to select the current part.")] 
        [SerializeField] private HoverButton _selectButton;

        [Tooltip("The component responsible for displaying the part's mesh.")] 
        [SerializeField] private PartViewer _partMeshDisplay;

        /// <summary>
        /// Toggles whether the display is able to be seen.
        /// </summary>
        public void SetDisplayActive(bool isActive)
        {
            _display.SetActive(isActive);
        }

        /// <summary>
        /// Updates the part name displayed on the UI.
        /// </summary>
        public void UpdatePartName(string name)
        {
            _partName.text = name;
        }
        
        /// <summary>
        /// Will set the mesh being displayed to the given mesh.
        /// </summary>
        public void DisplayPart(Mesh mesh)
        {
            _partMeshDisplay.DisplayPart(mesh);
        }

        /// <summary>
        /// Will update the visibility of the buttons based on the given parameters.
        /// </summary>
        /// <param name="nextButtonActive">Whether the next button should be visible.</param>
        /// <param name="selectButtonActive">Whether the select button should be visible.</param>
        public void UpdateButtons(bool nextButtonActive, bool selectButtonActive)
        {
            _nextButton.gameObject.SetActive(nextButtonActive);
            _selectButton.gameObject.SetActive(selectButtonActive);
        }
    }
}