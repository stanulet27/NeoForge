using UnityEngine;

namespace NeoForge.Dialogue
{
    public class TriggerDialogue : MonoBehaviour
    {
        public void StartDialogue(string dialogue)
        {
            DialogueManager.Instance.StartDialogueName(dialogue);
        }
    }
}