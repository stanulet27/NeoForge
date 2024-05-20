using System.Collections.Generic;
using System.Linq;
using MenuSystems.SpeechProcessing;
using UnityEngine;

namespace SpeechProcessing
{
    public abstract class SpeechCommandListener : MonoBehaviour
    {
        [SerializeField] protected bool manuallyAdd;
        
        protected SpeechToCommandHandler SpeechToCommandHandler;
        private bool isInitialized;

        protected abstract IEnumerable<BasicSpeechCommandEvent> CommandEvents { get; }
        
        public void Initialize(SpeechToCommandHandler handler = null)
        {
            if (isInitialized) return;
            isInitialized = true;
            SpeechToCommandHandler.OnCommandReceived += ProcessCommands;
            handler ??= FindObjectOfType<SpeechToCommandHandler>(true);
            
            Debug.Assert(handler != null, "Unable to find a speech to command handler");
            
            SpeechToCommandHandler = handler;
            if (manuallyAdd)
                AddOptions();
        }

        protected abstract void AddOptions();

        public void EnableManualSetting()
        {
            manuallyAdd = true;
        }

        private void ProcessCommands(SpeechCommand command)
        {
            foreach (var commandEvent in CommandEvents.Where(ValidateCommandIsActive))
            {
                commandEvent.TryToInvokeCommand(command);
            }
        }

        private bool ValidateCommandIsActive(BasicSpeechCommandEvent command)
        {
            try
            {
                return command.ProcessWhenDisabled || gameObject.activeInHierarchy;
            }
            catch // Fails if the object was destroyed before being enabled
            {
                SpeechToCommandHandler.OnCommandReceived -= ProcessCommands;
                return false;
            }
        }

        public void Destroy()
        {
            SpeechToCommandHandler.OnCommandReceived -= ProcessCommands;
        }

        /// <summary>
        /// Adds a command to the command events and if the SpeechCommandListener is set to manually add its command events,
        /// it will also add the option to the SpeechToCommandHandler
        /// </summary>
        /// <param name="commandEvent">The Command Event to add</param>
        public abstract void AddCommand(BasicSpeechCommandEvent commandEvent);

        public abstract void RemoveCommand(BasicSpeechCommandEvent commandEvent);
    }
}