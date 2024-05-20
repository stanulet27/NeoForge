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
using Microsoft.MixedReality.Toolkit.Audio;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace SpeechProcessing
{
    /// <summary>
    /// Handles processing speech input from the user and if the user gives a valid command, will broadcast the command
    /// to all of its listeners.
    /// </summary>
    [RequireComponent(typeof(TextToSpeech))]
    public class SpeechToCommandHandler : MonoBehaviour
    {
        [SerializeField] private DictationHandler dictationHandler;
        [SerializeField] private List<CommandData> validCommands;
        [SerializeField] private string testingString;
        [SerializeField] private UnityEvent<string> onCommandProcessed;
        [SerializeField] private GameObject jarvisIcon;
        
        [Header("Audio Settings")]
        [SerializeField] private TextToSpeech textToSpeech;
        [SerializeField] private List<string> startDialogueOptions;
        [SerializeField] private List<string> confirmationDialogueOptions;
        [SerializeField] private List<string> failureDialogueOptions;
        
        private string lastSpokenCommand;
        private string executionLog;
        private bool commandLineMessage;
        
        /// <summary>
        /// Broadcasted when a user gives a valid command.
        /// </summary>
        public static event Action<SpeechCommand> OnCommandReceived;
        public string ExecutionLog => executionLog;
        public string ValidCommands => validCommands.ConvertAll(x => x.ToString()).Aggregate((x, y) => x + "\n" + y);

        private void Awake()
        {
            textToSpeech = GetComponent<TextToSpeech>();
            lastSpokenCommand = "";
            var listeners = FindObjectsOfType<SpeechCommandListener>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Initialize(handler: this));
        }

        [ContextMenu("Convert to Command")]
        public void TestCommandConverter()
        {
            TestCommandConverter(testingString);
        }

        public void TestCommandConverter(string command)
        {
            lastSpokenCommand = command;
            commandLineMessage = true;
            ConvertToCommand();
        }

        [ContextMenu("Manually Start Listening")]
        public void StartListener()
        {
            jarvisIcon.SetActive(true);
            textToSpeech.StopSpeaking();
            textToSpeech.StartSpeaking(GetDialogueOption(startDialogueOptions));
            commandLineMessage = false;
            dictationHandler.StartRecording();
        }

        public void SetCommand(string userText)
        {
            lastSpokenCommand = userText;
            Debug.Log("Command Set: " + lastSpokenCommand);
        }

        public void ConvertToCommand()
        {
            if (lastSpokenCommand == "")
            {
                ResetHandler();
                return;
            }

            executionLog = "";
            lastSpokenCommand = Regex.Replace(lastSpokenCommand, @"(?![-])\p{P}", "").Trim().ToLower();

            var commandFound = SearchForValidCommand(lastSpokenCommand);
            ResetHandler();

            if (!commandLineMessage)
            {
                StartCoroutine(PlayAudioFeedback(commandFound));
            }
            
        }

        private IEnumerator PlayAudioFeedback(bool commandFound)
        {
            textToSpeech.StopSpeaking();
            var response = GetDialogueOption(commandFound ? confirmationDialogueOptions : failureDialogueOptions);
            textToSpeech.StartSpeaking(response);

            while (!textToSpeech.IsSpeaking()) yield return null;
            while (textToSpeech.IsSpeaking()) yield return null;

            if (!commandFound)
            {
                StartListener();
            }
        }

        private string GetDialogueOption(List<string> possibleOptions)
        {
            return possibleOptions.Count > 0 ? possibleOptions[UnityEngine.Random.Range(0, possibleOptions.Count)] : "";
        }

        private void ResetHandler()
        {
            jarvisIcon.SetActive(false);
        }

        private bool SearchForValidCommand(string userText)
        {
            var foundCommand = SpeechHelperClass.TryToFindCommand(userText, validCommands, out var processedCommand);
            executionLog = foundCommand ? $"Command Found:\n{processedCommand}" : "No Command Found";
            
            if (foundCommand)
            {
                onCommandProcessed?.Invoke(processedCommand.ToString());
                OnCommandReceived?.Invoke(processedCommand);
            }
            
            return foundCommand;
        }

        /// <summary>
        /// Adds a new option to the valid commands in the SpeechToCommandHandler.
        /// </summary>
        /// <param name="label">The command label to modify / add</param>
        /// <param name="newOption">The new option to add to the label</param>
        public void AddOption(SpeechLabel label, string newOption, List<string> variations = null, bool hasValue = false)
        {
            variations ??= new List<string>();
            var matchingOption = validCommands.Find(x => x.speechLabel == label);
            if(matchingOption != null)
            {
                matchingOption.Update(newOption, variations);
            }
            else
            {
                matchingOption = 
                    new CommandData { speechLabel = label, PossibleAlternatives = variations, 
                        ValidOptions = newOption != "" ? new List<string> { newOption } : new List<string>(),
                        HasNumericValue = hasValue
                    };
                validCommands.Add(matchingOption);
            }
        }
        
        public void AddOption(BasicSpeechCommandEvent commandEvent)
        {
            AddOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
        }
        
        public void AddOption(AdvancedSpeechCommandEvent commandEvent)
        {
            AddOption(commandEvent.CommandLabel, commandEvent.OptionNeeded, commandEvent.Variations, commandEvent.HasValue);
        }
        
        public void RemoveOption(SpeechLabel label, string option)
        {
            var matchingOption = validCommands.Find(x => x.speechLabel == label);
            matchingOption?.ValidOptions.Remove(option);
        }

        private void OnDestroy()
        {
            var listeners = FindObjectsOfType<SpeechCommandListener>(includeInactive: true);
            listeners.ToList().ForEach(x => x.Destroy());
        }
    }
}