using System;
using System.Collections.Generic;
using MenuSystems.SpeechProcessing;
using UnityEngine;
using UnityEngine.Events;

namespace CommandSystem
{
    /// <summary>
    /// Represents a speech command event that is triggered when a specified command label is processed by the
    /// SpeechToCommandHandler.cs and will invoke a UnityEvent given. If the command has a value, the value will also
    /// be sent through the UnityEvent. This version of the SpeechCommandEvent adds additional functionality to the
    /// base SpeechCommandEvent by allowing the user to specify a list of variations that the command can have. Also
    /// allows users to specify whether a command has a value or not.s
    /// </summary>
    [Serializable]
    public class AdvancedCommandEvent : BasicCommandEvent
    {
        [SerializeField] private List<string> _variations;
        [SerializeField] private bool _hasValue;
        
        public List<string> Variations => _variations;
        public bool HasValue => _hasValue;
        
        public AdvancedCommandEvent(CommandLabel commandLabel,
                                    string optionNeeded, 
                                    bool hasValue, 
                                    List<string> variations,
                                    bool processWhenDisabled, 
                                    Action nonValueCommand = null, 
                                    Action<int> valueCommand = null) 
            : base(commandLabel, optionNeeded, processWhenDisabled, nonValueCommand, valueCommand)
        {
            _variations = variations;
            _hasValue = hasValue;
        }
        
        public AdvancedCommandEvent(BasicCommandEvent commandEvent) 
            : base(commandEvent.CommandLabel, commandEvent.OptionNeeded, commandEvent.ProcessWhenDisabled)
        {
            _variations = new List<string>();
            _hasValue = false;
        }
    }
}