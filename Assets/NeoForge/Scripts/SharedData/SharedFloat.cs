/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared float between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Float", menuName = "Shared Data/Normalizable/Float")]
    public class SharedFloat : SharedDataBase<float>, INormalizable
    {
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _value;
        [SerializeField] private int _sigFigs = -1;

        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                BroadcastValueChanged();
            }
        }


        public void SetFromNormal(float normalizedValue)
        {
            Value = _minValue + normalizedValue * (_maxValue - _minValue);
        }

        public float GetNormal()
        {
            return (Value - _minValue) / (_maxValue - _minValue);
        }

        public string DebugInformation()
        {
            return $"Min Value: {_minValue} | Max Value: {_maxValue} | Value: {Value} | Normalized Value: {GetNormal()}";
        }

        public void SetMax(float newMax)
        {
            _maxValue = newMax;
        }

        public override string ToString()
        {
            return _sigFigs == -1 ? _value.ToString() : _value.ToString($"F{_sigFigs}");
        }
    }
}