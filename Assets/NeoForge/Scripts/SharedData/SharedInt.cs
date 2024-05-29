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
    ///     This serves as scriptable object that can be used to have a shared int between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Int", menuName = "Shared Data/Normalizable/Integer")]
    public class SharedInt : SharedDataBase<int>, INormalizable, IBoundable<int>
    {
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;
        [SerializeField] private int _value;

        public override int Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                BroadcastValueChanged();
            }
        }

        public int LowerBound => _minValue;
        public int UpperBound => _maxValue;

        public void SetFromNormal(float normalizedValue)
        {
            Value = Mathf.RoundToInt(_minValue + normalizedValue * (_maxValue - _minValue));
        }

        public float GetNormal()
        {
            return (_value - _minValue) / (float)(_maxValue - _minValue);
        }

        public string DebugInformation()
        {
            return $"Min Value: {_minValue} | Max Value: {_maxValue} | Value: {_value} | Normalized Value: {GetNormal()}";
        }
    }
}