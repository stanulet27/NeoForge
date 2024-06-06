using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Buttons
{
    public class EventButton : UIButton
    {
        [Tooltip("The event to trigger when the button is clicked.")] [SerializeField]
        UnityEvent _eventToTrigger;

        /// <summary>
        /// Trigger the action that this button is link to.
        /// </summary>
        public override void Use()
        {
            _eventToTrigger?.Invoke();
        }
    }
}
