using System;

namespace NeoForge.SaveSystem
{
    [Serializable]
    internal class WorldStateEntry
    {
        public string EntryName;
        public int EntryValue;
            
        public WorldStateEntry(string entryName, int entryValue)
        {
            EntryName = entryName;
            EntryValue = entryValue;
        }
    }
}