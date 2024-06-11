using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.Dialogue
{
    public class DialogueEventReceiver : MonoBehaviour
    {
        [Tooltip("Determines how to determine if an event matches the criteria")]
        [SerializeField] private EventFormat _eventFormat = EventFormat.Full;
        [Tooltip("The criteria for match")]
        [SerializeField, HideIf("_eventFormat", EventFormat.All)] private string _eventToReceive;
        [Tooltip("The event to trigger when the criteria is met")]
        [SerializeField] private UnityEvent<string> _onEventReceived;
        
        private enum EventFormat {Prefix, Suffix, Full, All}
        
        private void OnEnable()
        {
            DialogueManager.OnEventTriggered += OnEventTriggered;
        }
        
        private void OnEventTriggered(string eventToReceive)
        {
            Dictionary<EventFormat, Predicate<string>> eventFormatPredicates = new()
            {
                {EventFormat.All, _ => true},
                {EventFormat.Prefix, x => x.StartsWith(_eventToReceive, StringComparison.CurrentCultureIgnoreCase)},
                {EventFormat.Suffix, x => x.EndsWith(_eventToReceive, StringComparison.CurrentCultureIgnoreCase)},
                {EventFormat.Full, x => x.Equals(_eventToReceive, StringComparison.CurrentCultureIgnoreCase)}
            };
            
            if (eventFormatPredicates[_eventFormat](eventToReceive))
            {
                TriggerEvent(eventToReceive);
            }
        }
        
        [Button]
        private void TriggerEvent(string eventLabel)
        {
            _onEventReceived?.Invoke(eventLabel);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnEventTriggered -= OnEventTriggered;
        }
    }
}