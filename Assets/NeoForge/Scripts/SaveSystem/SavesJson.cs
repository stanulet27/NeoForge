using System;
using System.Collections.Generic;

namespace NeoForge.SaveSystem
{
    [Serializable]
    internal class SavesJson
    {
        public List<Save> Saves;
        public SavesJson(List<Save> saves)
        {
            Saves = saves;
        }
    }
}