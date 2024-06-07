using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static NeoForge.Dialogue.DialogueHelperClass;

namespace NeoForge.Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {
        [Tooltip("The text box to display the dialogue in")]
        [SerializeField] private TextBoxDisplay _textBoxDisplay;
        [Tooltip("The choices display to show the player's choices")]
        [SerializeField] private ChoicesDisplay _choicesDisplay;
        
        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += DisplayUI;
            DialogueManager.OnDialogueEnded += HideUI;
            HideUI();
        }
        
        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= DisplayUI;
            DialogueManager.OnDialogueEnded -= HideUI;
            DialogueManager.OnTextSet -= SetDialogue;
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
        }

        private void HideUI()
        {
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        
            _textBoxDisplay.Hide();
        }

        private void DisplayUI(ConversationData conversation)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            _textBoxDisplay.Display();
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnTextSet -= SetDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
            
            DialogueManager.OnTextUpdated += UpdateDialogue;
            DialogueManager.OnTextSet += SetDialogue;
            DialogueManager.OnChoiceMenuOpen += DisplayChoices;
        }

        private void SetDialogue(DialogueData dialogue)
        {
            _textBoxDisplay.UpdateDialogueText("");
            _textBoxDisplay.SetDialogueText(dialogue.SpeakerName, dialogue.Dialogue);
        }
        
        private void UpdateDialogue(string text)
        {
            _textBoxDisplay.UpdateDialogueText(text);
        }
        
        private void DisplayChoices(List<string> choices)
        {
            _choicesDisplay.Display(choices, OnChoiceSelected);
            _textBoxDisplay.SetDialogueText("Choose", "");
        }
        
        private void OnChoiceSelected(int index)
        {
            DialogueManager.Instance.SelectChoice(index);
            _choicesDisplay.Hide();
        }

        [System.Serializable]
        public class DialogueDisplay
        {
            public TMP_Text textField;
            public GameObject background;
        }
    }
}