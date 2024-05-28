/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace CustomInspectors
{
    /// <summary>
    /// This attribute will cause a serialized property to be expanded.
    /// Useful particularly for properties that are custom classes containing only one serialized field in them.
    /// </summary>
    public class ExpandedClassAttribute : PropertyAttribute
    {
        
    }
}