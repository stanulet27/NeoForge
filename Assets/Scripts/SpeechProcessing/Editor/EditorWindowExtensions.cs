/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace MenuSystems.SpeechProcessing.Editor
{
    /// <summary>
    /// Adds a new extension so that you can check whether a key was pressed or not
    /// </summary>
    internal static class EditorWindowExtensions
    {
        /// <summary>
        /// Will return the value that was passed as the self parameter unmodified.
        /// </summary>
        /// <param name="self">The object that was being received from the editor action</param>
        /// <param name="controlName">The name of the controller that you want to check is currently active when the key is pressed</param>
        /// <param name="key">The key you want to check to see if it was pressed</param>
        /// <param name="success">True if the key was pressed, false if not</param>
        /// <typeparam name="T">The type of the object that was passed in</typeparam>
        /// <returns></returns>
        public static T KeyPressed<T>(this T self, string controlName, KeyCode key, out bool success)
        {
            
            success = GUI.GetNameOfFocusedControl() == controlName 
                      && Event.current.type == EventType.KeyUp 
                      && Event.current.keyCode == key;
            return self;
        }
    }
}