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

namespace CommandSystem
{
    /// <summary>
    /// Adds CommandEvents to the game object that will listen to the scene's ToCommandHandler.cs
    /// </summary>
    public class BasicCommandListener : CommandListener
    {
        [SerializeField] private List<BasicCommandEvent> commandEvents = new();

        protected override IEnumerable<BasicCommandEvent> CommandEvents => commandEvents;

        protected override void AddOptions()
        {
            commandEvents.ForEach(x => CommandHandler.AddOption(x));
        }
        
        /// <summary>
        /// Adds a command to the command events and if the CommandListener is set to manually add its command events,
        /// it will also add the option to the ToCommandHandler
        /// </summary>
        /// <param name="commandEvent">The Command Event to add</param>
        public override void AddCommand(BasicCommandEvent commandEvent)
        {
            commandEvents.Add(commandEvent);
            
            if (manuallyAdd) 
                CommandHandler.AddOption(commandEvent);
        }

        /// <summary>
        /// Removes a command from the command events and if the CommandListener is set to manually add its command events,
        /// it will also remove the option from the CommandHandler
        /// </summary>
        /// <param name="commandEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void RemoveCommand(BasicCommandEvent commandEvent)
        {
            var matchingCommand = commandEvents.Find(m => m.CommandLabel == commandEvent.CommandLabel && 
                                                          m.OptionNeeded == commandEvent.OptionNeeded);
            if(matchingCommand != null && commandEvents.Remove(matchingCommand) && manuallyAdd)
                CommandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
        }
    }
}