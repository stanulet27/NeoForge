using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace NeoForge.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/ImageDatabase")]
    public class ImageDatabase : ScriptableObject
    {
        [Tooltip("The portraits of the characters. The key is the character name in lowercase.")] [SerializeField]
        SerializedDictionary<string, Sprite> _portraits;

        /// <summary>
        /// Converts the character name to lowercase and returns the corresponding portrait.
        /// Returns null if the character name is not found.
        /// </summary>
        public Sprite GetPortrait(string characterName)
        {
            if (_portraits.TryGetValue(characterName.ToLower(), out var sprite))
            {
                return sprite;
            }

            Debug.Log("Could not find " + characterName);
            return null;
        }
    }
}