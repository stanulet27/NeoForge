using System.Collections.Generic;
using System.Linq;
using MenuSystems.SpeechProcessing;
using UnityEngine;

namespace CommandSystem
{
    public abstract class CommandListener : MonoBehaviour
    {
        [SerializeField] protected bool manuallyAdd;
        
        protected CommandHandler CommandHandler;
        private bool isInitialized;

        protected abstract IEnumerable<BasicCommandEvent> CommandEvents { get; }
        
        public void Initialize(CommandHandler handler = null)
        {
            if (isInitialized) return;
            isInitialized = true;
            CommandHandler.OnCommandReceived += ProcessCommands;
            handler ??= FindObjectOfType<CommandHandler>(true);
            
            Debug.Assert(handler != null, "Unable to find a speech to command handler");
            
            CommandHandler = handler;
            if (manuallyAdd)
                AddOptions();
        }

        protected abstract void AddOptions();

        public void EnableManualSetting()
        {
            manuallyAdd = true;
        }

        private void ProcessCommands(Command command)
        {
            foreach (var commandEvent in CommandEvents.Where(ValidateCommandIsActive))
            {
                commandEvent.TryToInvokeCommand(command);
            }
        }

        private bool ValidateCommandIsActive(BasicCommandEvent command)
        {
            try
            {
                return command.ProcessWhenDisabled || gameObject.activeInHierarchy;
            }
            catch // Fails if the object was destroyed before being enabled
            {
                CommandHandler.OnCommandReceived -= ProcessCommands;
                return false;
            }
        }

        public void Destroy()
        {
            CommandHandler.OnCommandReceived -= ProcessCommands;
        }

        /// <summary>
        /// Adds a command to the command events and if the SpeechCommandListener is set to manually add its command events,
        /// it will also add the option to the SpeechToCommandHandler
        /// </summary>
        /// <param name="commandEvent">The Command Event to add</param>
        public abstract void AddCommand(BasicCommandEvent commandEvent);

        public abstract void RemoveCommand(BasicCommandEvent commandEvent);
    }
}