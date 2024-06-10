using System;
using System.Collections.Generic;

namespace NeoForge.Dialogue.Helper
{
    [Serializable]
    public class ConversationData
    {
        public string ID;
        public List<DialogueChain> DialoguesSeries = new();
        public List<LeadsToPath> LeadsTo = new();
        public List<StateChange> StateChanges = new();
        public List<StateRequirement> StateRequirements = new();
        public string Variation;
        public string AudioCue;
        public bool HasChoice => LeadsTo.Count > 0 && LeadsTo[0].Prompt != "";
    }
}