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
    ///     This serves as the interface for a scriptable object data type that allows for sharing of variables
    ///     between game objects.
    /// </summary>
    public abstract class SharedDataBase<T> : ScriptableObject, IShareableData
    {
        public abstract T Value { get; set; }
        public virtual event Action OnValueChanged;

        public void Invoke()
        {
            BroadcastValueChanged();
        }

        public virtual string ValueAsText()
        {
            return Value.ToString();
        }

        public static implicit operator T(SharedDataBase<T> x)
        {
            return x.Value;
        }

        protected void BroadcastValueChanged()
        {
            OnValueChanged?.Invoke();
        }
    }
}