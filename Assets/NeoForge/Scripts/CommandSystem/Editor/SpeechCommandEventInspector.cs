/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEditor;
using UnityEngine;

namespace CommandSystem.Editor
{
    /// <summary>
    /// Overrides the default property drawer for the SpeechCommandEvent.cs in the inspector to make it easier to read
    /// the enum values.
    /// </summary>
    [CustomPropertyDrawer(typeof(BasicCommandEvent))]
    public class SpeechCommandEventInspector : PropertyDrawer
    {
        private const string DEFAULT_LABEL = "None";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = GetLabel(property);
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        private static string GetLabel(SerializedProperty property)
        {
            var enumProperty = property.FindPropertyRelative("commandLabel");
            var enumIndex = enumProperty.enumValueIndex;
            return enumIndex == -1 ? DEFAULT_LABEL : enumProperty.enumDisplayNames[enumIndex];
        }

    }
}