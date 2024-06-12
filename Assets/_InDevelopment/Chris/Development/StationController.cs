using NeoForge.Input;
using SharedData;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SharedState _currentStation;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject _overviewUI;
    [SerializeField] private GameObject _planningUI;
    [SerializeField] private GameObject _forgingUI;
    [SerializeField] private GameObject _heatingUI;
    [SerializeField] private GameObject _coolingUI;

    private GameObject _activeUI;
    
    private void Start()
    {
        ControllerManager.OnChangeStation += ChangeStation;
        _activeUI = _overviewUI;
        _overviewUI.SetActive(true);
        _planningUI.SetActive(false);
        _forgingUI.SetActive(false);
        _heatingUI.SetActive(false);
        _coolingUI.SetActive(false);
    }

    private void ChangeStation(int station)
    {
        _activeUI.SetActive(false);
        switch (station)
        {
            case 1:
                animator.Play("Overview Position");
                _currentStation.Value = StationState.Overview;
                _activeUI = _overviewUI;
                break;
            case 2:
                animator.Play("Heating Position");
                _currentStation.Value = StationState.Heating;
                _activeUI = _heatingUI;
                break;
            case 3:
                animator.Play("Forging Position");
                _currentStation.Value = StationState.Forging;
                _activeUI = _forgingUI;
                break;
            case 4:
                animator.Play("Cooling Position");
                _currentStation.Value = StationState.Cooling;
                _activeUI = _coolingUI;
                break;
            case 5:
                animator.Play("Planning Position");
                _currentStation.Value = StationState.Planning;
                _activeUI = _planningUI;
                break;
        }
        _activeUI.SetActive(true);
    }
}
