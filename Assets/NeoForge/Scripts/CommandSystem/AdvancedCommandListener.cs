using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandSystem
{
    /// <summary>
    /// Adds CommandEvents to the game object that will listen to the scene's CommandHandler.cs. Adds
    /// additional functionality to the CommandListener by allowing for the addition of advanced commands
    /// </summary>
    public class AdvancedCommandListener : CommandListener
    {
        [SerializeField] private List<AdvancedCommandEvent> commandEvents = new();

        protected override IEnumerable<BasicCommandEvent> CommandEvents 
            => commandEvents.Cast<BasicCommandEvent>().ToList();

        protected override void AddOptions()
        {
            commandEvents.ForEach(x => CommandHandler.AddOption(x));
        }

        public override void AddCommand(BasicCommandEvent commandEvent)
        {
            var command = commandEvent as AdvancedCommandEvent ?? new AdvancedCommandEvent(commandEvent);
            commandEvents.Add(command);

            if (manuallyAdd)
            {
                CommandHandler.AddOption(command);
            }
        }

        public override void RemoveCommand(BasicCommandEvent commandEvent)
        {
            var matchingCommand = commandEvents.Find(m => m.CommandLabel == commandEvent.CommandLabel && 
                                                          m.OptionNeeded == commandEvent.OptionNeeded);
            if (matchingCommand != null && commandEvents.Remove(matchingCommand) && manuallyAdd)
            {
                CommandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
            }
        }
    }
}