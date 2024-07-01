using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using NeoForge.Dialogue.Helper;
using UnityEngine;

namespace NeoForge.Dialogue.Editor
{
    public static class DialogueValidator
    {
        private static readonly List<string> _requirementExceptions = new()
        {
            "curious",
            "careful",
            "friendly",
            "blacksmiths",
            "merchants",
            "nobles"
        };

        private static readonly List<string> _namesUsed = new();
        private static readonly List<string> _events = new();
        private static List<string> _dialogueIds = new();
        private static List<string> _changes = new();
        private static List<string> _requirements = new();
        private static List<(string id, int variation)> _dialogueVariations = new();
        
        public static bool ValidateAllConversations(List<ConversationDataSO> conversations)
        {
            Debug.Log("Validating all conversations...");
            
            _namesUsed.Clear();
            _events.Clear();
            _dialogueIds = conversations.Select(x => x.Data.ID).ToList();
            _dialogueIds.Add("exit");
            _changes = conversations.SelectMany(x => x.Data.StateChanges.Select(y => y.State)).ToList();
            _requirements = conversations.SelectMany(x => x.Data.StateRequirements.Select(y => y.State)).ToList();
            _changes.AddRange(_requirementExceptions);
            _dialogueVariations = conversations.Where(x => x.Data.Variation != "")
                .Select(x => (x.Data.ID, int.Parse(x.Data.Variation))).ToList();

            var allValid = conversations.Select(ValidateConversation).Aggregate(true, (current, isValid) => current & isValid);

            Debug.Log("Validation complete");
            Debug.Log("Spoken names:");
            foreach (var name in _namesUsed)
            {
                Debug.Log(name);
            }
            
            Debug.Log("--------------------------------------------------");
            
            Debug.Log("Events:");
            foreach (var eventId in _events)
            {
                Debug.Log(eventId);
            }
            Debug.Log("--------------------------------------------------");
            
            ValidateVariations();
            Debug.Log("--------------------------------------------------");

            return allValid;
        }

        private static void ValidateVariations()
        {
            var variations = _dialogueVariations.OrderBy(x => x.id).ThenBy(x => x.variation).ToList();

            for (var i = 1; i < variations.Count; i++)
            {
                var previous = variations[i - 1];
                var current = variations[i];
                Debug.Log(current);
                var sameID = current.id == previous.id;
                if (!sameID) continue;
                
                if (previous.variation + 1 == current.variation) continue;
                
                Debug.LogError($"Variation {current.id} - {current.variation} is not in sequence");
            }
        }

        private static bool ValidateConversation(ConversationDataSO conversation)
        {
            var isValid = true;
            isValid &= ValidateDialogueModifiers(conversation);
            isValid &= ValidateDialogue(conversation);
            isValid &= ValidateLeadsToPaths(conversation);
            
            return isValid;
        }

        private static bool ValidateDialogueModifiers(ConversationDataSO conversation)
        {
            var conversationData = conversation.Data;
            
            var warnings = conversationData.StateChanges.Select(x => x.State).Where(x => !_requirements.Contains(x));
            foreach (var warning in warnings)
            {
                Debug.LogWarning($"Change {warning} in {GetDebugLabel(conversation)} has no matching requirement");
            }
            
            warnings = conversationData.StateRequirements.Select(x => x.State).Where(x => !_changes.Contains(x));
            foreach (var warning in warnings)
            {
                Debug.LogWarning($"Requirement {warning} in {GetDebugLabel(conversation)} has no matching change");
            }
            
            return true;
        }

        /// <summary>
        /// Currently nothing to validate so will just gather the names used
        /// </summary>
        private static bool ValidateDialogue(ConversationDataSO conversation)
        {
            var dialogueChains = conversation.Data.DialoguesSeries;
            var isValid = true;

            foreach (var line in dialogueChains.SelectMany(chain => chain.dialogues))
            {
                if (!_namesUsed.Contains(line.SpeakerName))
                {
                    _namesUsed.Add(line.SpeakerName);
                }

                if (System.Enum.IsDefined(typeof(ConversantType), line.Speaker)) continue;
                
                Debug.LogError($"Dialogue speaker {line.Speaker} is invalid option for {GetDebugLabel(conversation)}");
                isValid = false;
            }
            
            return isValid;
        }

        private static bool ValidateLeadsToPaths(ConversationDataSO conversation)
        {
            var isValid = true;
            var invalidPath = conversation.Data.LeadsTo.Where(path => !path.IsEvent && !_dialogueIds.Contains(path.NextID));
            
            _events.AddRange(conversation.Data.LeadsTo
                .Where(path => path.IsEvent)
                .Select(path => $"{path.NextID} - {conversation.Data.ID} - {conversation.Data.Variation}"));

            foreach (var path in invalidPath)
            {
                Debug.LogError($"Path {path.NextID} in {GetDebugLabel(conversation)} does not lead to a valid conversation");
                isValid = false;
            }
            
            return isValid;
        }
        
        private static string GetDebugLabel(ConversationDataSO conversation)
        {
            return $"Conversation {conversation.Data.ID} - {conversation.Data.Variation}";
        }
    }
}