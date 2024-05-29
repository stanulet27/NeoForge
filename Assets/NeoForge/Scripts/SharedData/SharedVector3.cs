/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a vector 3 of shared ints between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Int Vector3", menuName = "Shared Data/Collection/Int Vector3")]
    public class SharedVector3 : SharedDataBase<SharedIntVector3>
    {
        [SerializeField] private SharedIntVector3 _value;

        public override SharedIntVector3 Value
        {
            get => _value;
            set
            {
                _value = value;
                BroadcastValueChanged();
            }
        }

        public override event Action OnValueChanged
        {
            add
            {
                base.OnValueChanged += value;
                _value.x.OnValueChanged += value;
                _value.y.OnValueChanged += value;
                _value.z.OnValueChanged += value;
            }
            remove
            {
                base.OnValueChanged -= value;
                _value.x.OnValueChanged -= value;
                _value.y.OnValueChanged -= value;
                _value.z.OnValueChanged -= value;
            }
        }

        public override string ToString()
        {
            return $"({_value.x}, {_value.y}, {_value.z})";
        }
    }
}