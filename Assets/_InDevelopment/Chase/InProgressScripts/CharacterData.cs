using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeoForge.UI.Tools
{
    [Serializable]
    public class CharacterData
    {
        private const string PRONOUN_THEY = "zey";
        private const string PRONOUN_THEIR = "zier";
        private const string PRONOUN_THEM = "zem";
        private const string PRONOUN_WERE = "zere";
        
        // Order and spelling matter for the enums - see BackstoryUI and CharacterSelectionUI
        public enum Trait { Friendly, Careful, Curious }
        public enum Pronoun { They, She, He }
        public enum Family { Blacksmiths, Merchants, Nobles }

        public string Name;
        public List<Pronoun> Pronouns;
        public Trait Personality;
        public Family Background;
        public Sprite Portrait;

        public string ReplacePronouns(string line)
        {
            var pronounToUse = Pronouns[Random.Range(0, Pronouns.Count)];
            var conversionTable = ConversionTable(pronounToUse);
            string ReplacePronoun(string current, string pronoun) => current.Replace(pronoun, conversionTable[pronoun]);
            
            return conversionTable.Keys.Aggregate(line, ReplacePronoun);
        }
        
        public string ReplaceName(string line)
        {
            return line.Replace(DialogueVariables.CHARACTER_NAME_MARKER, Name);
        }

        private static Dictionary<string, string> ConversionTable(Pronoun pronoun)
        {
            return new Dictionary<string, string>
            {
                [PRONOUN_THEM] = pronoun switch
                {
                    Pronoun.They => "them",
                    Pronoun.He => "him",
                    _ => "her"
                },
                [PRONOUN_THEIR] = pronoun switch
                {
                    Pronoun.They => "their",
                    Pronoun.He => "his",
                    _ => "her"
                },
                [PRONOUN_THEY] = pronoun switch
                {
                    Pronoun.They => "they",
                    Pronoun.He => "he",
                    _ => "she"
                },
                [PRONOUN_WERE] = pronoun switch
                {
                    Pronoun.They => "were",
                    Pronoun.He => "was",
                    _ => "was"
                }
            };
        }
    }
}