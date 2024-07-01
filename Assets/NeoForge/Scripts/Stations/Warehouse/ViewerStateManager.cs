using System;

namespace NeoForge.Stations.Warehouse
{
    public class ViewerStateManager
    {
        /// <summary>
        /// Will be called when the state of the viewer changes.
        /// </summary>
        public event Action<ViewerState> OnStateChanged;
        
        /// <summary>
        /// Will return the current state of the viewer.
        /// </summary>
        public ViewerState CurrentState { get; private set; }
        
        /// <summary>
        /// Will change the state of the viewer and notify any listeners.
        /// </summary>
        public void ChangeState(ViewerState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}