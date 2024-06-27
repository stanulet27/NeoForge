using System;
using NeoForge.Dialogue.Character;
using NeoForge.UI.Tools;
using UnityEngine;
using static NeoForge.Dialogue.Helper.DialogueVariables;

namespace NeoForge.Dialogue.Helper
{
    [Serializable]
    public class DialogueData
    {
        public ConversantType Speaker;
        public string SpeakerName;
        [SerializeField, TextArea] public string Dialogue;
            
        public DialogueData()
        {
            Speaker = ConversantType.Conversant;
            SpeakerName = "";
            Dialogue = "";
        }
        
        public DialogueData(DialogueData dialogueData)
        {
            Speaker = dialogueData.Speaker;
            SpeakerName = dialogueData.SpeakerName;
            Dialogue = dialogueData.Dialogue;
        }
        
        public DialogueData(string line)
        {
            if (line.StartsWith(PLAYER_MARKER)) Speaker = ConversantType.Player;
            else if (line.StartsWith(VOICE_MARKER)) Speaker = ConversantType.Voice;
            else Speaker = ConversantType.Conversant;
                
            SpeakerName = Speaker switch
            {
                ConversantType.Player => PLAYER_MARKER.Split(':')[0],
                ConversantType.Conversant => line.Split(':')[0],
                _ => ""
            };
                
            Dialogue = Speaker == ConversantType.Voice 
                ? line[VOICE_MARKER.Length..] 
                : line[SpeakerName.Length..].Split(':')[1].Trim();
        }
        
        public DialogueData GetCharacterDialogue(CharacterData characterData)
        {
            var dialogue = new DialogueData(this);
            if (string.Equals(dialogue.SpeakerName, CHARACTER_NAME_MARKER, StringComparison.OrdinalIgnoreCase))
            {
                dialogue.SpeakerName = characterData.Name;
            }
            dialogue.Dialogue = characterData.ReplacePronouns(dialogue.Dialogue);
            dialogue.Dialogue = characterData.ReplaceName(dialogue.Dialogue);
            return dialogue;
        }
            
        public void AppendDialogue(string line)
        {
            Dialogue += "\n" + line;
        }
    }
}