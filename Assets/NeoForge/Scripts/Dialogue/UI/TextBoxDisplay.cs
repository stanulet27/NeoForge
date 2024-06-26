using NeoForge.Dialogue.Helper;
using TMPro;
using UnityEngine;

namespace NeoForge.Dialogue.UI
{
    public class TextBoxDisplay : MonoBehaviour
    {
        [Tooltip("The text field where the dialogue will be displayed")]
        [SerializeField] private TMP_Text _dialogueTextField;
        [Tooltip("The text field where the speaker's name will be displayed")]
        [SerializeField] private TMP_Text _nameTextField;
        [Tooltip("The object that contains the speaker's name")]
        [SerializeField] private GameObject _nameField;
        [Tooltip("The object that indicates that the player can continue the dialogue")]
        [SerializeField] protected GameObject _continueIndicator;

        private ConversantType _conversant;

        public void Display()
        {
            ToggleChildrenDisplay(true);
        }

        public virtual void SetDialogueText(string speakerName, string dialogue)
        {
            _nameTextField.text = speakerName;
            _nameField.SetActive(_nameTextField.text != "");
            _dialogueTextField.maxVisibleCharacters = 0;
            _dialogueTextField.text = dialogue;
            _continueIndicator.SetActive(false);
        }
        
        public virtual void UpdateDialogueText(string text)
        {
            _dialogueTextField.maxVisibleCharacters = text.Length;
            _continueIndicator.SetActive(_dialogueTextField.text != "" &&
                _dialogueTextField.maxVisibleCharacters >= _dialogueTextField.text.Length);
        }
        
        public void Hide() => ToggleChildrenDisplay(false);

        private void ToggleChildrenDisplay(bool shouldDisplay)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldDisplay);
            }
        }
    }
}