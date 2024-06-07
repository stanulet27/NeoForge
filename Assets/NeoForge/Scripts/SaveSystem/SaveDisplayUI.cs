using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.SaveSystem
{
    public class SaveDisplayUI : MonoBehaviour
    {
        [Tooltip("List of buttons that will display the save information.")]
        [SerializeField] private List<SaveDisplayButtonUI> _saveDisplayButtons;

        private void OnEnable()
        {
            if (SaveManager.Instance == null)
            {
                Debug.Log("SaveManager not found. SaveDisplayUI disabled.");
                enabled = false;
                return;
            }
            
            Display(SaveManager.Instance.Saves);
        }

        /// <summary>
        /// Will go through and update each save slot display with the corresponding save data 
        /// </summary>
        public void Display(List<Save> saves)
        {
            for (var i = 0; i < _saveDisplayButtons.Count; i++)
            {
                _saveDisplayButtons[i].Display(saves[i]);
            }
        }
        
        /// <summary>
        /// Will perform the save operation and apply the new save to the specified slot index.
        /// </summary>
        /// <param name="saveIndex">The save slot to update</param>
        public void Save(int saveIndex)
        {
            SaveManager.Instance.Save(saveIndex);
            Display(SaveManager.Instance.Saves);
        }
        
        /// <summary>
        /// Will perform the load operation using the data from the specified save slot index.
        /// </summary>
        /// <param name="saveIndex">The save slot to load</param>
        public void Load(int saveIndex)
        {
            SaveManager.Instance.Load(saveIndex);
        }
        
        /// <summary>
        /// Will clear the save data from the specified save slot index.
        /// </summary>
        /// <param name="saveIndex">The save slot to clear</param>
        public void Delete(int saveIndex)
        {
            SaveManager.Instance.DeleteSave(saveIndex);
            Display(SaveManager.Instance.Saves);
        }
    }
}