﻿using System;
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
            
        public void AppendDialogue(string line)
        {
            Dialogue += "\n" + line;
        }
    }
}