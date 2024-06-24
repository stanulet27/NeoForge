using SharedData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NeoForge.Deformation
{
    public class StationSwitcher : MonoBehaviour
    {
        [Tooltip("The event that is invoked when the station is changed. Should be found in Station Controller")]
        [SerializeField] private UnityEvent<ForgeArea> _changeStation;

        [Tooltip("The station that the player will switch to")] 
        [SerializeField] private ForgeArea _forgeArea;

        /// <summary>
        /// Changes the station to the one specified in the inspector
        /// </summary>
        public void Invoke()
        {
            _changeStation.Invoke(_forgeArea);
        }
    }
}