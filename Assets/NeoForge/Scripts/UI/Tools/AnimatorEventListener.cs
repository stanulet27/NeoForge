using System;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Tools
{
    public class AnimatorEventListener : MonoBehaviour
    {
        public event Action<string> OnStateEnter;
        public event Action<string> OnStateExit;
        
        [Tooltip("Will invoke the following events when a state is entered and give the state name.")]
        [SerializeField] private UnityEvent<string> _onStateEnter;
        
        [Tooltip("Will invoke the following events when a state is exited and give the state name.")]
        [SerializeField] private UnityEvent<string> _onStateExit;
        
        /// <summary>
        /// Will invoke the OnStateEnter event and the _onStateEnter UnityEvent triggering any listeners.
        /// </summary>
        /// <param name="state">The name of the state that was entered</param>
        public void StateEnter(string state)
        {
            _onStateEnter.Invoke(state);
            OnStateEnter?.Invoke(state);
        }

        /// <summary>
        /// Will invoke the OnStateExit event and the _onStateExit UnityEvent triggering any listeners.
        /// </summary>
        /// <param name="state">The name of the state that was exited</param>
        public void StateExit(string state)
        {
            _onStateExit.Invoke(state);
            OnStateExit?.Invoke(state);
        }
    }
}