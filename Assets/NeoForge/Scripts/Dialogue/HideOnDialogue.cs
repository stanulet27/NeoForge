using UnityEngine;

namespace NeoForge.Dialogue
{
    public class HideOnDialogue : MonoBehaviour
    {
        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += Hide;
            DialogueManager.OnDialogueEnded += Show;
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= Hide;
            DialogueManager.OnDialogueEnded -= Show;
        }
        
        private void Show()
        {
            foreach (Transform child in transform) child.gameObject.SetActive(true);
        }
        
        private void Hide(DialogueHelperClass.ConversationData _)
        {
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }
    }
}
