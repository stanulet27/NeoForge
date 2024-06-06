using System;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UITools
{
    public class AnimatorEventListener : MonoBehaviour
    {
        public event Action<string> OnStateEnter;
        public event Action<string> OnStateExit;
        
        [SerializeField] private UnityEvent<string> _onStateEnter;
        [SerializeField] private UnityEvent<string> _onStateExit;

        public void StateEnter(string state)
        {
            _onStateEnter.Invoke(state);
            OnStateEnter?.Invoke(state);
        }

        public void StateExit(string state)
        {
            _onStateExit.Invoke(state);
            OnStateExit?.Invoke(state);
        }
    }
}