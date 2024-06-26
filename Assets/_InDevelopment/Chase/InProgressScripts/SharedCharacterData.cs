using System.Linq;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    [CreateAssetMenu(menuName = "CharacterData", fileName = "SO_NewProfile_CharacterData")]
    public class SharedCharacterData : ScriptableObject
    {
        [SerializeField] private CharacterData _characterData;
        
        public CharacterData Data => _characterData;
        
        public void RewriteData(CharacterData data)
        {
            _characterData = data;
            Debug.Log($"Rewrote data to {data.Name} with " +
                      $"pronouns {data.Pronouns.Select(x => x.ToString()).Aggregate((x, y) => $"{x}/{y}")} " +
                      $"and personality {data.Personality} from {data.Background} background.");
        }
    }
}