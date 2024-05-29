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
    public class BasicCommandListener : CommandListenerBase
    {
        [SerializeField] private List<BasicCommandEvent> _commandEventElements = new();

        protected override IEnumerable<BasicCommandEvent> _commandEvents => _commandEventElements;

        protected override void AddOptions()
        {
            _commandEventElements.ForEach(x => _commandHandler.AddOption(x));
        }

        /// <summary>
        /// Adds a command to the command events and if the CommandListener is set to manually add its command events,
        /// it will also add the option to the ToCommandHandler
        /// </summary>
        /// <param name="commandEvent">The Command Event to add</param>
        public override void AddCommand(BasicCommandEvent commandEvent)
        {
            _commandEventElements.Add(commandEvent);
            if (_manuallyAdd) _commandHandler.AddOption(commandEvent);
        }

        /// <summary>
        /// Removes a command from the command events and if the CommandListener is set to manually add its command events,
        /// it will also remove the option from the CommandHandler
        /// </summary>
        /// <param name="commandEvent"></param>
        public override void RemoveCommand(BasicCommandEvent commandEvent)
        {
            var matchingCommand = _commandEventElements.Find(m => m.CommandLabel == commandEvent.CommandLabel &&
                m.OptionNeeded == commandEvent.OptionNeeded);
            if (matchingCommand != null && _commandEventElements.Remove(matchingCommand) && _manuallyAdd)
            {
                _commandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
            }
        }
    }
}