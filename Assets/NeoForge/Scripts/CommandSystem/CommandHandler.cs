/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MenuSystems.SpeechProcessing;
using UnityEngine;
using UnityEngine.Events;

namespace CommandSystem
{
    /// <summary>
    /// Handles processing speech input from the user and if the user gives a valid command, will broadcast the command
    /// to all of its listeners.
    /// </summary>
    public class CommandHandler : MonoBehaviour
    {
        /// <summary>
        /// Broadcasted when a user gives a valid command.
        /// </summary>
        public static event Action<Command> OnCommandReceived;

        [SerializeField] private List<CommandData> _validCommands;
        [SerializeField] private UnityEvent<string> _onCommandProcessed;

        public string ExecutionLog => _executionLog;
        public string ValidCommands => _validCommands.ConvertAll(x => x.ToString()).Aggregate((x, y) => x + "\n" + y);

        private string _lastSpokenCommand;
        private string _executionLog;
       

        private void Awake()
        {
            _lastSpokenCommand = "";
            var listeners = FindObjectsOfType<CommandListenerBase>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Initialize(handler: this));
        }
        private void OnDestroy()
        {
            var listeners = FindObjectsOfType<CommandListenerBase>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Destroy());
        }

        public void TestCommandConverter(string command)
        {
            _lastSpokenCommand = command;
            ConvertToCommand();
        }

        private void ConvertToCommand()
        {
            if (_lastSpokenCommand == "") return;

            _executionLog = "";
            _lastSpokenCommand = Regex.Replace(_lastSpokenCommand, @"(?![-])\p{P}", "").Trim().ToLower();

            SearchForValidCommand(_lastSpokenCommand);
        }

        public void AddOption(BasicCommandEvent commandEvent)
        {
            AddOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
        }

        public void AddOption(AdvancedCommandEvent commandEvent)
        {
            AddOption(commandEvent.CommandLabel, commandEvent.OptionNeeded, commandEvent.Variations, commandEvent.HasValue);
        }

        public void RemoveOption(CommandLabel label, string option)
        {
            var matchingOption = _validCommands.Find(x => x.CommandLabel == label);
            matchingOption?.ValidOptions.Remove(option);
        }

        private void SearchForValidCommand(string userText)
        {
            var foundCommand = CommandHelperClass.TryToFindCommand(userText, _validCommands, out var processedCommand);
            _executionLog = foundCommand ? $"Command Found:\n{processedCommand}" : "No Command Found";
            
            if (foundCommand)
            {
                _onCommandProcessed?.Invoke(processedCommand.ToString());
                OnCommandReceived?.Invoke(processedCommand);
            }
        }

        /// <summary>
        /// Adds a new option to the valid commands in the SpeechToCommandHandler.
        /// </summary>
        /// <param name="label">The command label to modify / add</param>
        /// <param name="newOption">The new option to add to the label</param>
        private void AddOption(CommandLabel label, string newOption, List<string> variations = null, bool hasValue = false)
        {
            variations ??= new List<string>();
            var matchingOption = _validCommands.Find(x => x.CommandLabel == label);
            if(matchingOption != null)
            {
                matchingOption.Update(newOption, variations);
            }
            else
            {
                matchingOption = 
                    new CommandData 
                    { 
                        CommandLabel = label, 
                        PossibleAlternatives = variations, 
                        ValidOptions = newOption != "" ? new List<string> { newOption } : new List<string>(),
                        HasNumericValue = hasValue
                    };
                _validCommands.Add(matchingOption);
            }
        }
       
    }
}