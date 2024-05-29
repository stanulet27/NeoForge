using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandSystem
{
    /// <summary>
    /// Adds CommandEvents to the game object that will listen to the scene's CommandHandler.cs. Adds
    /// additional functionality to the CommandListener by allowing for the addition of advanced commands
    /// </summary>
    public class AdvancedCommandListener : CommandListenerBase
    {
        [SerializeField] private List<AdvancedCommandEvent> _commandEventElements = new();

        protected override IEnumerable<BasicCommandEvent> _commandEvents 
            => _commandEventElements.Cast<BasicCommandEvent>().ToList();

        public override void AddCommand(BasicCommandEvent commandEvent)
        {
            var command = commandEvent as AdvancedCommandEvent ?? new AdvancedCommandEvent(commandEvent);
            _commandEventElements.Add(command);

            if (_manuallyAdd)
            {
                _commandHandler.AddOption(command);
            }
        }

        public override void RemoveCommand(BasicCommandEvent commandEvent)
        {
            var matchingCommand = _commandEventElements.Find(m => m.CommandLabel == commandEvent.CommandLabel && 
                                                          m.OptionNeeded == commandEvent.OptionNeeded);
            if (matchingCommand != null && _commandEventElements.Remove(matchingCommand) && _manuallyAdd)
            {
                _commandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
            }
        }

        protected override void AddOptions()
        {
            _commandEventElements.ForEach(x => _commandHandler.AddOption(x));
        }
    }
}