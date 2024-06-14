using SharedData;
using UnityEngine;
using UnityEngine.Events;

public class StationSwitcher : MonoBehaviour
{
    [SerializeField] private UnityEvent<Station> _changeStation;
    [SerializeField] private Station _station;
    
    /// <summary>
    /// Changes the station to the one specified in the inspector
    /// </summary>
    public void Invoke()
    {
        _changeStation.Invoke(_station);
    }
}
