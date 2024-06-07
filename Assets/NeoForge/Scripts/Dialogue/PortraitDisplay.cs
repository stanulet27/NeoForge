using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.Dialogue
{
    public class PortraitDisplay : MonoBehaviour
    {
        [Tooltip("What will display the portrait")]
        [SerializeField] Image _portrait;
        [Tooltip("What will display the name of the character")]
        [SerializeField] TextMeshProUGUI _textField;
        [Tooltip("The database that contains all the portraits and the characters they belong to")]
        [SerializeField] ImageDatabase _imageDatabase;

        /// <summary>
        /// Will show the portrait and name of the character. The portrait will be retrieved from the image database. If
        /// no portrait is found, the portrait will not be displayed.
        /// </summary>
        /// <param name="characterName">The name of the character to be displayed</param>
        public void Display(string characterName)
        {
            ToggleChildrenDisplay(true);
            _portrait.sprite = _imageDatabase.GetPortrait(characterName);
            _portrait.enabled = _portrait.sprite != null;
            _textField.text =  characterName[0].ToString().ToUpper() + characterName.Substring(1);
        }

        /// <summary>
        /// Will hide the portrait and name of the character.
        /// </summary>
        public void Hide()
        {
            ToggleChildrenDisplay(false);
        }
        
        private void ToggleChildrenDisplay(bool shouldDisplay)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldDisplay);
            }
        }
    }
}
