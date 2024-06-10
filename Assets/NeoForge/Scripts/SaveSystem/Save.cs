using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeoForge.SaveSystem
{
    [Serializable]
    public class Save
    {
        [SerializeField] private List<WorldStateEntry> _worldState;
        public DateTime SaveTime;
        public int SceneIndex;
        public bool IsEmpty;
        public int Day;

        public Save()
        {
            IsEmpty = true;
            SceneIndex = 1;
            SaveTime = DateTime.Now;
            _worldState = new List<WorldStateEntry>();
            Day = 0;
        }
        
        public Save(Save save)
        {
            _worldState = new List<WorldStateEntry>(save._worldState);
            SaveTime = save.SaveTime;
            SceneIndex = save.SceneIndex;
            IsEmpty = save.IsEmpty;
            Day = save.Day + 1;
        }
        
        public Save(Dictionary<string, int> currentWorldState, int sceneIndex, int day)
        {
            UpdateWorldState(currentWorldState);
            SaveTime = DateTime.Now;
            SceneIndex = sceneIndex;
            IsEmpty = false;
            Day = day;
        }
        
        /// <summary>
        /// Returns the world state that the the save was created with.
        /// </summary>
        public Dictionary<string, int> GetWorldState()
        {
            return _worldState.ToDictionary(entry => entry.EntryName, entry => entry.EntryValue);
        }
        
        private void UpdateWorldState(Dictionary<string, int> currentWorldState)
        {
            _worldState = currentWorldState.Select(state => new WorldStateEntry(state.Key, state.Value)).ToList();
        }
    }
}