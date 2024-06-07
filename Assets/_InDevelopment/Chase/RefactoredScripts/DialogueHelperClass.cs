using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspectors;
using UnityEngine;

namespace NeoForge.Dialogue
{
    public static class DialogueHelperClass
    {
        public static readonly string ID_MARKER = "ID: ";
        public static readonly string CONDITIONAL_MARKER = "Conditions";
        public static readonly string CHANGES_MARKER = "Changes";
        public static readonly string DIALOGUE_MARKER = "Dialogue";
        public static readonly string PLAYER_MARKER = "Player: ";
        public static readonly string VOICE_MARKER = "Voice: ";
        public static readonly string LEADS_TO_MARKER = "Leads To:";
        public static readonly string EVENT_MARKER = "*";
        public static readonly string VARIATION_MARKER = "Variation";
        public static readonly string MUSIC_MARKER = "Cue Audio";

        [Serializable]
        public class DialogueData
        {
            public ConversantType Speaker;
            public string SpeakerName;
            [SerializeField, TextArea] public string Dialogue;
            
            public DialogueData(string line)
            {
                if (line.StartsWith(PLAYER_MARKER)) Speaker = ConversantType.Player;
                else if (line.StartsWith(VOICE_MARKER)) Speaker = ConversantType.Voice;
                else Speaker = ConversantType.Conversant;
                
                SpeakerName = Speaker switch
                {
                    ConversantType.Player => PLAYER_MARKER.Split(':')[0],
                    ConversantType.Conversant => line.Split(':')[0],
                    _ => ""
                };
                
                Dialogue = Speaker == ConversantType.Voice 
                    ? line[VOICE_MARKER.Length..] 
                    : line[SpeakerName.Length..].Split(':')[1].Trim();
            }
            
            public void AppendDialogue(string line)
            {
                Dialogue += "\n" + line;
            }
        }
    
        public enum ConversantType
        {
            Player,
            Conversant,
            Voice,
        }

        [Serializable]
        public class ConversationData
        {
            public string ID;
            public List<DialogueChain> DialoguesSeries = new();
            public List<LeadsToPath> LeadsTo = new();
            public List<StateChange> StateChanges = new();
            public List<StateRequirement> StateRequirements = new();
            public string Variation;
            public string AudioCue;
            public bool HasChoice => LeadsTo.Count > 0 && LeadsTo[0].Prompt != "";
        }
        
        [Serializable]
        public class StateChange
        {
            [SerializeField, HideInInspector] List<string> _components;
            [SerializeField, ReadOnly] private string _change;
            [HideInInspector] public string State;

            public Func<int, int> Modifier
            {
                get
                {
                    if (_components == null) return null;
                    switch (_components.Count)
                    {
                        case 1:
                            var hasPrefix = _components[0].StartsWith("!");
                            return hasPrefix ? _ => 0 : _ => 1;
                        case 3:
                            return _components[1] switch
                            {
                                "=" => _ => int.Parse(_components[2]),
                                "+=" => x => x + int.Parse(_components[2]),
                                "-=" => x => x - int.Parse(_components[2]),
                                _ => throw new ArgumentException("Invalid operator")
                            };
                        default:
                            throw new ArgumentException("Invalid condition");
                    }
                }
            }

            public StateChange(string conditionLine)
            {
                _components = conditionLine.Split(" ").ToList();
                
                switch (_components.Count)
                {
                    case 1:
                        var hasPrefix = _components[0].StartsWith("!");
                        State = hasPrefix ? _components[0][1..] : _components[0];
                        break;
                    case 3:
                        State = _components[0];
                        break;
                    default:
                        throw new ArgumentException("Invalid condition");
                }
                
                _change = conditionLine;
            }
        }
        
        [Serializable]
        public class StateRequirement
        {
            [SerializeField, HideInInspector] private List<string> _components;
            [SerializeField, ReadOnly] private string _requirement;
            [HideInInspector] public string State;
            
            public Predicate<int> IsMet
            {
                get
                {
                    if (_components == null) return null;
                    switch (_components.Count)
                    {
                        case 1:
                            var hasPrefix = _components[0].StartsWith("!");
                            State = hasPrefix ? _components[0][1..] : _components[0];
                            return hasPrefix ? x => x == 0 : x => x != 0;
                        case 3:
                            State = _components[0];
                            return GetOperatorPredicate(_components[1], int.Parse(_components[2]));
                        default:
                            throw new ArgumentException($"Invalid condition: {_requirement} has {_components.Count}");
                    }
                }
            }
            
            public StateRequirement(string conditionLine)
            {
                _components = conditionLine.Split(" ").ToList();

                switch (_components.Count)
                {
                    case 1:
                        var hasPrefix = _components[0].StartsWith("!");
                        State = hasPrefix ? _components[0][1..] : _components[0];
                        break;
                    case 3:
                        State = _components[0];
                        break;
                    default:
                        throw new ArgumentException("Invalid condition");
                }
                
                _requirement = conditionLine;
            }

            private static Predicate<int> GetOperatorPredicate(string op, int state)
            {
                return op switch
                {
                    "==" => x => x == state,
                    "!=" => x => x != state,
                    ">" => x => x > state,
                    "<" => x => x < state,
                    ">=" => x => x >= state,
                    "<=" => x => x <= state,
                    _ => throw new ArgumentException("Invalid operator")
                };
            }
        }
        
        [Serializable]
        public class LeadsToPath
        {
            [SerializeField, ReadOnly] private string _path;
            [HideInInspector] public string Prompt;
            [HideInInspector] public string NextID;
            [HideInInspector] public bool IsEvent;
            
            public LeadsToPath(string prompt, string nextID, bool isEvent)
            {
                this.Prompt = prompt;
                this.NextID = nextID;
                this.IsEvent = isEvent;
                _path = isEvent 
                    ? "Prompt: " + prompt + " Triggers Event: " + nextID
                    : "Prompt: " + prompt + " Leads to: " + nextID;
            }
        }

        [Serializable]
        public class DialogueChain
        {
            public List<DialogueData> dialogues = new();
        }
    }
}