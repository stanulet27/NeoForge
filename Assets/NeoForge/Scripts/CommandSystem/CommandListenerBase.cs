using System.Collections.Generic;
using System.Linq;
using MenuSystems.SpeechProcessing;
using UnityEngine;

namespace CommandSystem
{
    public abstract class CommandListenerBase : MonoBehaviour
    {
        [SerializeField] protected bool _manuallyAdd;

        protected CommandHandler _commandHandler;

        protected abstract IEnumerable<BasicCommandEvent> _commandEvents { get; }

        private bool _isInitialized;

        public void Initialize(CommandHandler handler = null)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            CommandHandler.OnCommandReceived += ProcessCommands;
            handler ??= FindObjectOfType<CommandHandler>(true);

            Debug.Assert(handler != null, "Unable to find a speech to command handler");

            _commandHandler = handler;
            if (_manuallyAdd)
                AddOptions();
        }

        public void EnableManualSetting()
        {
            _manuallyAdd = true;
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

        protected abstract void AddOptions();

        private void ProcessCommands(Command command)
        {
            foreach (var commandEvent in _commandEvents.Where(ValidateCommandIsActive))
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
    }
}