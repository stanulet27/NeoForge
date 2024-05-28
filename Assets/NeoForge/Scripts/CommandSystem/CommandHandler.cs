/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections;
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
        [SerializeField] private List<CommandData> validCommands;
        [SerializeField] private UnityEvent<string> onCommandProcessed;

        private string lastSpokenCommand;
        private string executionLog;
        
        /// <summary>
        /// Broadcasted when a user gives a valid command.
        /// </summary>
        public static event Action<Command> OnCommandReceived;
        public string ExecutionLog => executionLog;
        public string ValidCommands => validCommands.ConvertAll(x => x.ToString()).Aggregate((x, y) => x + "\n" + y);

        private void Awake()
        {
            lastSpokenCommand = "";
            var listeners = FindObjectsOfType<CommandListener>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Initialize(handler: this));
        }

        public void TestCommandConverter(string command)
        {
            lastSpokenCommand = command;
            ConvertToCommand();
        }

        private void ConvertToCommand()
        {
            if (lastSpokenCommand == "")
            {
                return;
            }

            executionLog = "";
            lastSpokenCommand = Regex.Replace(lastSpokenCommand, @"(?![-])\p{P}", "").Trim().ToLower();

            SearchForValidCommand(lastSpokenCommand);
        }

        private void SearchForValidCommand(string userText)
        {
            var foundCommand = CommandHelperClass.TryToFindCommand(userText, validCommands, out var processedCommand);
            executionLog = foundCommand ? $"Command Found:\n{processedCommand}" : "No Command Found";
            
            if (foundCommand)
            {
                onCommandProcessed?.Invoke(processedCommand.ToString());
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
            var matchingOption = validCommands.Find(x => x.commandLabel == label);
            if(matchingOption != null)
            {
                matchingOption.Update(newOption, variations);
            }
            else
            {
                matchingOption = 
                    new CommandData { commandLabel = label, PossibleAlternatives = variations, 
                        ValidOptions = newOption != "" ? new List<string> { newOption } : new List<string>(),
                        HasNumericValue = hasValue
                    };
                validCommands.Add(matchingOption);
            }
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
            var matchingOption = validCommands.Find(x => x.commandLabel == label);
            matchingOption?.ValidOptions.Remove(option);
        }

        private void OnDestroy()
        {
            var listeners = FindObjectsOfType<CommandListener>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Destroy());
        }
    }
}