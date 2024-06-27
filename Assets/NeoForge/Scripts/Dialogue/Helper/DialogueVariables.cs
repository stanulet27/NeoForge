using System.Collections.Generic;

namespace NeoForge.Dialogue.Helper
{
    public static class DialogueVariables
    {
        public enum Pronoun { They, She, He }

        private const string PRONOUN_THEY = "zey";
        private const string PRONOUN_THEIR = "zier";
        private const string PRONOUN_THEM = "zem";
        private const string PRONOUN_WERE = "zere";
        
        public static readonly string ID_MARKER = "ID: ";
        public static readonly string CONDITIONAL_MARKER = "Requires";
        public static readonly string CHANGES_MARKER = "Changes";
        public static readonly string DIALOGUE_MARKER = "Dialogue";
        public static readonly string PLAYER_MARKER = "Player: ";
        public static readonly string VOICE_MARKER = "Voice: ";
        public static readonly string LEADS_TO_MARKER = "Leads To:";
        public static readonly string EVENT_MARKER = "*";
        public static readonly string VARIATION_MARKER = "Variation";
        public static readonly string MUSIC_MARKER = "Cue Audio";
        public static readonly string CHARACTER_NAME_MARKER = "Zoey";
        
        public static Dictionary<string, string> ConversionTable(Pronoun pronoun)
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