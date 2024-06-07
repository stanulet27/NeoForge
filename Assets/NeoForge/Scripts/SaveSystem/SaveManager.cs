using System.Collections.Generic;
using NeoForge.UI.Scenes;
using NeoForge.Utilities;
using NeoForge.Dialogue;
using UnityEngine;

namespace NeoForge.SaveSystem
{
    public class SaveManager : SingletonMonoBehaviour<SaveManager>
    {
        private Save _savePrepared = new();
        private int _currentSaveIndex;
        public List<Save> Saves { get; } = new () {new Save(), new Save(), new Save()};

        private void Start()
        {
            SceneTools.OnSceneTransitionStart += PrepareSave;
            ReadSavesFromFile();
        }

        private void OnDestroy()
        {
            SceneTools.OnSceneTransitionStart -= PrepareSave;
            if (Instance == this)
            {
                WriteSavesToFile();
            }
        }

        /// <summary>
        /// Will perform the save operation and apply the new save to the specified slot index.
        /// </summary>
        /// <param name="slotIndex">The slot to store new save in</param>
        public void Save(int slotIndex)
        {
            Saves[slotIndex] = new Save(_savePrepared);
            Debug.Log("Saved to slot " + slotIndex);
        }
        
        /// <summary>
        /// Will perform the load operation using the data from the specified save slot index.
        /// </summary>
        /// <param name="saveIndex">The slot to load the save information from</param>
        public void Load(int saveIndex)
        {
            _currentSaveIndex = saveIndex;
            WorldState.LoadWorldState(Saves[_currentSaveIndex].GetWorldState());
            StartCoroutine(SceneTools.TransitionToScene(Saves[_currentSaveIndex].SceneIndex));
            Debug.Log("Loaded from slot " + saveIndex);
        }

        /// <summary>
        /// Will write the current save data to a file in a persistant location.
        /// </summary>
        [ContextMenu("Write Saves To File")]
        public void WriteSavesToFile()
        {
            var jsonData = JsonUtility.ToJson(new SavesJson(Saves), true);
            Debug.Log(jsonData);
            if (Application.isEditor)
            {
                System.IO.File.WriteAllText(Application.dataPath + "/saves.json", jsonData);
            }
            else
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json", jsonData);
            }
        }

        /// <summary>
        /// Will clear the save data from the specified save slot index.
        /// </summary>
        /// <param name="saveIndex">The save slot to clear the info from.</param>
        public void DeleteSave(int saveIndex)
        {
            Saves[saveIndex] = null;
        }

        /// <summary>
        /// Will reset the games save data to default values. Will also clear the save file.
        /// </summary>
        [ContextMenu("Clear Saves")]
        public void ClearSaves()
        {
            Saves.Clear();
            Saves.AddRange(new List<Save> {new(), new(), new()});
            
            WriteSavesToFile();
        }
        
        private void PrepareSave(int sceneIndex)
        {
            _savePrepared = new Save(WorldState.GetWorldState(), sceneIndex, WorldState.GetState("saveDay"));
        }
        
        private void ReadSavesFromFile()
        {
            var path = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
            if (!System.IO.File.Exists(path + "/saves.json")) return;
            
            var jsonData = System.IO.File.ReadAllText(path + "/saves.json");
            var loadedData = JsonUtility.FromJson<SavesJson>(jsonData);
            Saves.Clear();
            Saves.AddRange(loadedData.Saves);
        }
    }
}