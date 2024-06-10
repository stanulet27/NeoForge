using NeoForge.Dialogue.Helper;
using UnityEngine;

namespace NeoForge.Dialogue
{
    [CreateAssetMenu(fileName = "New Data", menuName = "Dialogue/Data")]
    public class ConversationDataSO : ScriptableObject
    {
        [Tooltip("The conversation stored in this object")]
        [SerializeField] ConversationData _conversationData;

        /// <summary>
        /// Sets the conversation data for this object. Will also set the name of the object to the conversation ID
        /// and variation.
        /// </summary>
        /// <param name="conversation">The conversation to set the object to</param>
        public void SetConversation(ConversationData conversation)
        {
            _conversationData = conversation;
            name = _conversationData.ID + _conversationData.Variation;
        }

        public ConversationData Data => _conversationData;
    }
}
