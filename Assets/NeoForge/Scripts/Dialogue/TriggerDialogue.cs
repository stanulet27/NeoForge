using UnityEngine;

namespace NeoForge.Dialogue
{
    public class TriggerDialogue : MonoBehaviour
    {
        /// <summary>
        /// Will trigger a dialogue conversation of the given name to occur. Useful for unity events set in the
        /// inspector since DialogueManager is a singleton.
        /// </summary>
        /// <param name="dialogue">The dialogue conversation to begin, case-insensitive</param>
        public void StartDialogue(string dialogue)
        {
            DialogueManager.Instance.StartDialogueName(dialogue);
        }
    }
}