/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using MenuSystems.SpeechProcessing;
using UnityEngine;
using UnityEngine.Events;

namespace SpeechProcessing
{
    
    /// <summary>
    /// Represents a speech command event that is triggered when a specified command label is processed by the
    /// SpeechToCommandHandler.cs and will invoke a UnityEvent given. If the command has a value, the value will also
    /// be sent through the UnityEvent
    /// </summary>
    [Serializable]
    public class BasicSpeechCommandEvent
    {
        [SerializeField] private SpeechLabel commandLabel;
        [SerializeField] private string optionNeeded;
        [SerializeField] private bool processWhenDisabled;
        [SerializeField] private UnityEvent<int> onCommand;
        
        private Action nonValueCommand;
        private Action<int> valueCommand;

        public bool ProcessWhenDisabled => processWhenDisabled;
        public SpeechLabel CommandLabel => commandLabel;
        public string OptionNeeded => optionNeeded.ToLower();
        
        public BasicSpeechCommandEvent(SpeechLabel commandLabel, string optionNeeded, bool processWhenDisabled,
            Action nonValueCommand = null, Action<int> valueCommand = null)
        {
            this.commandLabel = commandLabel;
            this.optionNeeded = optionNeeded;
            this.processWhenDisabled = processWhenDisabled;
            this.nonValueCommand = nonValueCommand;
            this.valueCommand = valueCommand;
        }
        
        public void TryToInvokeCommand(SpeechCommand command)
        {
            var validLabel = command.Label == commandLabel;
            var needsOption = optionNeeded != "";
            var hasGenericOption = command.HasOption && command.OptionsSelected.Contains("all") &&
                                   command.OptionsSelected.Count == 1;
            var hasNeededOption = command.HasOption && command.OptionsSelected.Contains(OptionNeeded);
            var validOption = !needsOption || hasGenericOption || hasNeededOption;

            if (validLabel && validOption)
            {
                nonValueCommand?.Invoke();
                valueCommand?.Invoke(command.NumberValue);
                onCommand?.Invoke(command.NumberValue);
            }
        }
    }
}