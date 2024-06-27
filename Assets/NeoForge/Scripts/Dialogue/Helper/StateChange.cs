using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspectors;
using UnityEngine;

namespace NeoForge.Dialogue.Helper
{
    [Serializable]
    public class StateChange
    {
        [SerializeField, HideInInspector] List<string> _components;
        [SerializeField, ReadOnly] private string _change;
        [HideInInspector] public string State;

        public string Change => _change;
        
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
}