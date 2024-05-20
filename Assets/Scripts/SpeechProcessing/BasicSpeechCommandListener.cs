/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections.Generic;
using UnityEngine;

namespace SpeechProcessing
{
    /// <summary>
    /// Adds SpeechCommandEvents to the game object that will listen to the scene's SpeechToCommandHandler.cs
    /// </summary>
    public class BasicSpeechCommandListener : SpeechCommandListener
    {
        [SerializeField] private List<BasicSpeechCommandEvent> commandEvents = new();

        protected override IEnumerable<BasicSpeechCommandEvent> CommandEvents => commandEvents;

        protected override void AddOptions()
        {
            commandEvents.ForEach(x => SpeechToCommandHandler.AddOption(x));
        }
        
        /// <summary>
        /// Adds a command to the command events and if the SpeechCommandListener is set to manually add its command events,
        /// it will also add the option to the SpeechToCommandHandler
        /// </summary>
        /// <param name="commandEvent">The Command Event to add</param>
        public override void AddCommand(BasicSpeechCommandEvent commandEvent)
        {
            commandEvents.Add(commandEvent);
            
            if (manuallyAdd) 
                SpeechToCommandHandler.AddOption(commandEvent);
        }

        /// <summary>
        /// Removes a command from the command events and if the SpeechCommandListener is set to manually add its command events,
        /// it will also remove the option from the SpeechToCommandHandler
        /// </summary>
        /// <param name="commandEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void RemoveCommand(BasicSpeechCommandEvent commandEvent)
        {
            var matchingCommand = commandEvents.Find(m => m.CommandLabel == commandEvent.CommandLabel && 
                                                          m.OptionNeeded == commandEvent.OptionNeeded);
            if(matchingCommand != null && commandEvents.Remove(matchingCommand) && manuallyAdd)
                SpeechToCommandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
        }
    }
}