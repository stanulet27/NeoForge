using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspectors;
using UnityEngine;

namespace NeoForge.Dialogue.Helper
{
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
}