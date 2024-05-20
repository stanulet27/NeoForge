using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpeechProcessing
{
    /// <summary>
    /// Adds SpeechCommandEvents to the game object that will listen to the scene's SpeechToCommandHandler.cs. Adds
    /// additional functionality to the SpeechCommandListener by allowing for the addition of advanced commands
    /// </summary>
    public class AdvancedSpeechCommandListener : SpeechCommandListener
    {
        [SerializeField] private List<AdvancedSpeechCommandEvent> commandEvents = new();

        protected override IEnumerable<BasicSpeechCommandEvent> CommandEvents 
            => commandEvents.Cast<BasicSpeechCommandEvent>().ToList();

        protected override void AddOptions()
        {
            commandEvents.ForEach(x => SpeechToCommandHandler.AddOption(x));
        }

        public override void AddCommand(BasicSpeechCommandEvent commandEvent)
        {
            var command = commandEvent as AdvancedSpeechCommandEvent ?? new AdvancedSpeechCommandEvent(commandEvent);
            commandEvents.Add(command);

            if (manuallyAdd)
            {
                SpeechToCommandHandler.AddOption(command);
            }
        }

        public override void RemoveCommand(BasicSpeechCommandEvent commandEvent)
        {
            var matchingCommand = commandEvents.Find(m => m.CommandLabel == commandEvent.CommandLabel && 
                                                          m.OptionNeeded == commandEvent.OptionNeeded);
            if (matchingCommand != null && commandEvents.Remove(matchingCommand) && manuallyAdd)
            {
                SpeechToCommandHandler.RemoveOption(commandEvent.CommandLabel, commandEvent.OptionNeeded);
            }
        }
    }
}