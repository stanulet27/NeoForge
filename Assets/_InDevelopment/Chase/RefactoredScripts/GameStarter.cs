using System.Collections.Generic;
using NeoForge.Dialogue.Character;
using NeoForge.Dialogue;
using NeoForge.UI.Inventory;
using NeoForge.UI.Scenes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Utilities
{
    public class GameStarter : SerializedMonoBehaviour
    {
        [Header("Starting Gear")] 
        [Tooltip("The amount of gold the player starts a new game with")]
        [SerializeField] private int _startingGold;
        [Tooltip("The amount of renown the player starts a new game with")]
        [SerializeField] private int _startingRenown;
        [Tooltip("The amount of gold the player starts a new game with if they are of nobility")]
        [SerializeField] private int _nobilityGold;
        [Tooltip("The amount of renown the player starts a new game with if they are of nobility")]
        [SerializeField] private int _nobilityRenown;
        [Tooltip("The items the player starts a new game with")]
        [SerializeField] private Dictionary<ItemBase, int> _startingItems;

        /// <summary>
        /// Will start a new game file and set up initial game state
        /// </summary>
        public void StartGame()
        {
            var isNoble = WorldState.InState(CharacterData.Family.Nobles);
            var startingGold = isNoble ? _nobilityGold : _startingGold;
            var startingRenown = isNoble ? _nobilityRenown : _startingRenown;
            
            InventorySystem.Instance.ResetInventory(startingGold, startingRenown, _startingItems);
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneWrapped));
        }
    }
}