using System;
using UnityEngine;

namespace NeoForge.Input
{
    public class InputModeEvents : MonoBehaviour
    {
        [Tooltip("The events to trigger when the mode changes.")]
        [SerializeField] private ModeEvent[] _modeEvents;
        
        private ControllerManager.Mode _currentMode;
        
        private void Start()
        {
            ControllerManager.OnModeSwapped += TriggerEvents;
            _currentMode = ControllerManager.Instance.CurrentMode;
        }

        private void OnDestroy()
        {
            ControllerManager.OnModeSwapped -= TriggerEvents;
        }
        
        private void TriggerEvents(ControllerManager.Mode newMode)
        {
            foreach (var modeEvent in _modeEvents)
            {
                var triggeringMode = modeEvent.Mode;
                var isEnteringMode = newMode == triggeringMode && _currentMode != triggeringMode;
                var isExitingMode = newMode != triggeringMode && _currentMode == triggeringMode;
                switch (modeEvent.Interaction)
                {
                    case ModeEvent.InteractionType.OnEnter when isEnteringMode:
                        modeEvent.EventToTrigger.Invoke();
                        break;
                    case ModeEvent.InteractionType.OnExit when isExitingMode:
                        modeEvent.EventToTrigger.Invoke();
                        break;
                    case ModeEvent.InteractionType.OnBoth when isEnteringMode || isExitingMode:
                        modeEvent.EventToTrigger.Invoke();
                        break;
                }
            }
            
            _currentMode = newMode;
        }

        [Serializable]
        private class ModeEvent
        {
            public enum InteractionType { OnEnter, OnExit, OnBoth }
            
            [Tooltip("The mode to trigger the event on.")]
            public ControllerManager.Mode Mode;
            [Tooltip("When should the event be triggered in relation to the mode.")]
            public InteractionType Interaction;
            [Tooltip("The event to trigger.")]
            public UnityEngine.Events.UnityEvent EventToTrigger;
        }
    }
}