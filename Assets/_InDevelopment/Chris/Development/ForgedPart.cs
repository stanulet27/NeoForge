using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgedPart : MonoBehaviour
{
    private enum PartState
    {
        Heating,
        Cooling,
        Ambient
    }

    private PartState _currentState;
    private float _temperature;
    private Material _material;
    
    private void Start()
    {
        _currentState = PartState.Ambient;
        _temperature = 290f; //room temperature in kelvin
        _material = GetComponent<MeshRenderer>().material;
        _material.SetFloat("_Temperature", _temperature);
    }
    [ContextMenu("Heat")]
    public void StartHeating()
    {
        _currentState = PartState.Heating;
        StartCoroutine(Heat());
    }
    [ContextMenu("Cool")]
    public void StartCooling()
    {
        _currentState = PartState.Cooling;
        StartCoroutine(Cool());
    }
    
    [ContextMenu("Stop")]
    public void SetToAmbient()
    {
        _currentState = PartState.Ambient;
        StopAllCoroutines();
    }    
    
    private IEnumerator Heat()
    {
        while (_currentState == PartState.Heating)
        {
            _temperature += 1f;
            _material.SetFloat("_Temperature", _temperature);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Cool()
    {
        while (_currentState == PartState.Cooling)
        {
            _temperature -= 1f;
            _material.SetFloat("_Temperature", _temperature);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
