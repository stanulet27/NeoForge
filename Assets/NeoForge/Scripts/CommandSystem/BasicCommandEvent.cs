/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using UnityEngine;
using UnityEngine.Events;

namespace CommandSystem
{
    /// <summary>
    /// Represents a speech command event that is triggered when a specified command label is processed by the
    /// SpeechToCommandHandler.cs and will invoke a UnityEvent given. If the command has a value, the value will also
    /// be sent through the UnityEvent
    /// </summary>
    [Serializable]
    public class BasicCommandEvent
    { 
        private Action _nonValueCommand;
        private Action<int> _valueCommand;

        [SerializeField] private CommandLabel _commandLabel;
        [SerializeField] private string _optionNeeded;
        [SerializeField] private bool _processWhenDisabled;
        [SerializeField] private UnityEvent<int> _onCommand;

        public bool ProcessWhenDisabled => _processWhenDisabled;
        public CommandLabel CommandLabel => _commandLabel;
        public string OptionNeeded => _optionNeeded.ToLower();
        
        public BasicCommandEvent(   CommandLabel commandLabel,
                                    string optionNeeded, 
                                    bool processWhenDisabled,
                                    Action nonValueCommand = null, 
                                    Action<int> valueCommand = null)
        {
            _commandLabel = commandLabel;
            _optionNeeded = optionNeeded;
            _processWhenDisabled = processWhenDisabled;
            _nonValueCommand = nonValueCommand;
            _valueCommand = valueCommand;
        }
        
        public void TryToInvokeCommand(Command command)
        {
            var validLabel = command.Label == _commandLabel;
            var needsOption = _optionNeeded != "";
            var hasGenericOption = command.HasOption && command.OptionsSelected.Contains("all") &&
                command.OptionsSelected.Count == 1;
            var hasNeededOption = command.HasOption && command.OptionsSelected.Contains(OptionNeeded);
            var validOption = !needsOption || hasGenericOption || hasNeededOption;

            if (validLabel && validOption)
            {
                _nonValueCommand?.Invoke();
                _valueCommand?.Invoke(command.NumberValue);
                _onCommand?.Invoke(command.NumberValue);
            }
        }
    }
}