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
using MenuSystems.SpeechProcessing;

namespace SpeechProcessing
{
    /// <summary>
    /// Provides helper methods for speech processing.
    /// </summary>
    public static class SpeechHelperClass
    {
        private static readonly Dictionary<string, int> NUMBERS = new()
        {  
            {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},{"five",5},{"six",6},  
            {"seven",7},{"eight",8},{"nine",9},{"ten",10}
        };
        
        /// <summary>
        /// Will convert a speech label to a string. (Removes underscores and converts to lower case)
        /// </summary>
        /// <param name="speechLabel">Speech label to convert</param>
        /// <returns>Converted speech label</returns>
        public static string SpeechLabelToString(SpeechLabel speechLabel)
        {
            return speechLabel.ToString().ToLowerInvariant().Replace("_", " ");
        }

        /// <summary>
        /// Will attempt to convert a string to an int. (ex. "one" -> 1), only valid for numbers 0 - 10
        /// </summary>
        /// <param name="numberString">The string to convert</param>
        /// <param name="value">The value the string converted to, 0 if unsuccessful</param>
        /// <returns>Whether the conversion was successful</returns>
        public static bool TryToConvertToInt(string numberString, out int value)
        {
            return NUMBERS.TryGetValue(numberString, out value);
        }
        
        /// <summary>
        /// Will try to find a command in the command data base provided from the given text.
        /// </summary>
        /// <param name="text">The command given by the user</param>
        /// <param name="validCommands">The command database of valid commands to search through</param>
        /// <param name="commandData">The command found, an invalid command if not found</param>
        /// <returns>True if the command was found, false otherwise</returns>
        public static bool TryToFindCommand(string text, IEnumerable<CommandData> validCommands, out SpeechCommand commandData)
        {
            commandData = new SpeechCommand();
            var match = string.Empty;
            
            foreach (var validCommand in validCommands.Where(x => CheckForMatchingLabel(text, x, out match)))
            {
                commandData = ProcessCommand(RemoveString(text, match), validCommand);
                if (commandData.IsValid) return true;
            }
            
            return false;
        }
        
        private static bool CheckForMatchingLabel(string userText, CommandData commandData, out string match)
        {
            match = userText.Contains(commandData.Label) ? 
                commandData.Label : 
                commandData.PossibleAlternatives.Find(userText.Contains);

            return match != null;
        }

        private static SpeechCommand ProcessCommand(string userText, CommandData commandData)
        {
            var command = new SpeechCommand
            {
                IsValid = true,
                Label = commandData.speechLabel
            };
            
            userText = RemoveString(userText, command.Label.ToString());
            
            if (commandData.HasOptions)
            {
                var optionsSelected = new List<string>();
                var options = commandData.ValidOptions.OrderByDescending(x => x.Length);
                foreach (var option in options)
                {
                    if(!userText.Contains(option)) continue;
                    
                    optionsSelected.Add(option);
                    if (option != "") userText = userText.Replace(option, "").Trim();
                }

                command.OptionsSelected = optionsSelected;
                command.HasOption = command.OptionsSelected.Count > 0;
                command.IsValid = command.HasOption;
            }
            
            if (commandData.HasNumericValue)
            {
                var sign = userText.Contains("-") ? -1 : 1;
                userText = userText.Replace("-", "").Trim();

                foreach (var word in userText.Split())
                {
                    if (!int.TryParse(word, out command.NumberValue) && !TryToConvertToInt(word, out command.NumberValue))
                    {
                        continue;
                    }
                    
                    command.HasNumber = true;
                    break;
                }
                
                command.NumberValue = sign * command.NumberValue;
                command.IsValid = command.IsValid && command.HasNumber;
            }

            return command;
        }
        
        private static string RemoveString(string stringToModify, string stringToRemove)
        {
            if (!stringToModify.Contains(stringToRemove)) return stringToModify;

            var lastCharacterIndex = stringToModify.IndexOf(stringToRemove, StringComparison.InvariantCulture) +
                                     stringToRemove.Length;
            
            return stringToModify[lastCharacterIndex..].Trim();
        }
    }
}