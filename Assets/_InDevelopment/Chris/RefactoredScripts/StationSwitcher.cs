using SharedData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class StationSwitcher : MonoBehaviour
{
    [Tooltip("The event that is invoked when the station is changed. Should be found in Station Controller")]
    [SerializeField] private UnityEvent<Station> _changeStation;
    [Tooltip("The station that the player will switch to")]
    [SerializeField] private Station _station;
    
    /// <summary>
    /// Changes the station to the one specified in the inspector
    /// </summary>
    public void Invoke()
    {
        _changeStation.Invoke(_station);
    }
}
