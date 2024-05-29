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

namespace CustomInspectors
{
    /// <summary>
    /// This attribute allows the user to make a property read-only in the unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // DO NOT REMOVE
        // This is a marker attribute the logic behind this is located in the ReadOnlyPropertyDrawer editor script.
    }
}