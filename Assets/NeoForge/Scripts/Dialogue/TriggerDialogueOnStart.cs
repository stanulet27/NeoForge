using UnityEngine;

namespace NeoForge.Dialogue
{
    public class TriggerDialogueOnStart : MonoBehaviour
    {
        [Tooltip("The name of the conversation to start")]
        [SerializeField] private string _conversation;
        
        private void Start()
        {
            DialogueManager.Instance.StartDialogueName(_conversation);
        }
    }
}