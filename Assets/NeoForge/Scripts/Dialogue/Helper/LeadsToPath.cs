using System;
using CustomInspectors;
using UnityEngine;

namespace NeoForge.Dialogue.Helper
{
    [Serializable]
    public class LeadsToPath
    {
        [SerializeField, ReadOnly] private string _path;
        [HideInInspector] public string Prompt;
        [HideInInspector] public string NextID;
        [HideInInspector] public bool IsEvent;
            
        public LeadsToPath(string prompt, string nextID, bool isEvent)
        {
            this.Prompt = prompt;
            this.NextID = nextID;
            this.IsEvent = isEvent;
            _path = isEvent 
                ? "Prompt: " + prompt + " Triggers Event: " + nextID
                : "Prompt: " + prompt + " Leads to: " + nextID;
        }
    }
}