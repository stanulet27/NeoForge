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

namespace NeoForge.Utilities
{
    /// <summary>
    ///     This will add the ability to swap a game object from off to on and visa versa
    /// </summary>
    public class Toggleable : MonoBehaviour
    {
        public event Action<bool> OnStateChanged;
        public bool IsEnabled => gameObject.activeSelf;

        public void Toggle()
        {
            Toggle(!gameObject.activeSelf);
        }

        public void Toggle(bool shouldEnable)
        {
            gameObject.SetActive(shouldEnable);
            OnStateChanged?.Invoke(IsEnabled);
        }
    }
}

