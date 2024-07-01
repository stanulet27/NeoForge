using System.Linq;
using NeoForge.Dialogue;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace NeoForge.Dialogue.Character
{
    [CreateAssetMenu(menuName = "CharacterData", fileName = "SO_NewProfile_CharacterData")]
    public class SharedCharacterData : ScriptableObject
    {
        [SerializeField] public CharacterData Data;

        public void RewriteData(CharacterData data)
        {
            Data = data;
            Debug.Log($"Rewrote data to {data.Name} with " +
                      $"pronouns {data.Pronouns.Select(x => x.ToString()).Aggregate((x, y) => $"{x}/{y}")} " +
                      $"and personality {data.Personality} from {data.Background} background.");
            WorldState.SetState(Data.Personality.ToString().ToLower(), true);
            WorldState.SetState("Trait", (int)Data.Personality);
            WorldState.SetState(Data.Background.ToString().ToLower(), true);
            WorldState.SetState("Family", (int) Data.Background);
        }
    }
}