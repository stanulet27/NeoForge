using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeoForge.Dialogue.Character
{
    [Serializable]
    public class CharacterData
    {
        // Order and spelling matter for the enums - see BackstoryUI and CharacterSelectionUI
        public enum Trait { Friendly, Careful, Curious }
        public enum Family { Blacksmiths, Merchants, Nobles }
        
        [Tooltip("The name of the character. Will be used in dialogue and UI.")]
        [SerializeField] private string _name;

        [Tooltip("The pronouns of the character. Will be used in dialogue.")]
        [SerializeField] private List<DialogueVariables.Pronoun> _pronouns;

        [Tooltip("The personality of the character.")]
        [SerializeField] private Trait _personality;

        [Tooltip("The family background of the character.")]
        [SerializeField] private Family _background;

        [Tooltip("The portrait of the character.")] 
        [SerializeField] private Sprite _portrait;

        /// The name the player choose
        public string Name => _name;
        
        /// The pronouns the character uses
        public List<DialogueVariables.Pronoun> Pronouns => _pronouns;
        
        /// The personality of the character
        public Trait Personality => _personality;
        
        /// The family background of the character
        public Family Background => _background;
        
        /// The portrait of the character
        public Sprite Portrait => _portrait;
        
        public CharacterData(string name, List<DialogueVariables.Pronoun> pronouns, 
            Trait personality, Family background, Sprite portrait)
        {
            _name = name;
            _pronouns = pronouns;
            _personality = personality;
            _background = background;
            _portrait = portrait;
        }

        /// Will replace the default character name, DialogueVariables.CHARACTER_NAME_MARKER, in the line with
        /// the player chosen character's name and return the modified string.
        public string ReplaceName(string line)
        {
            return line.Replace(DialogueVariables.CHARACTER_NAME_MARKER, Name);
        }
        
        /// Will replace the default pronouns in the line with the player chosen character's pronouns
        /// and return the modified string. The pronoun conversions are stored in DialogueVariables.
        public string ReplacePronouns(string line)
        {
            var pronounToUse = Pronouns[Random.Range(0, Pronouns.Count)];
            var conversionTable = DialogueVariables.ConversionTable(pronounToUse);

            return conversionTable.Keys.Aggregate(line, (x, y) => ReplaceCaseIncluded(x, y, conversionTable[y]));
        }

        private string ReplaceCaseIncluded(string line, string pronoun, string replacement)
        {
            return line.Replace(pronoun, replacement).Replace(ToTitleCase(pronoun), ToTitleCase(replacement));
        }
        
        private string ToTitleCase(string input)
        {
            return char.ToUpper(input[0]) + input[1..];
        }
    }
}