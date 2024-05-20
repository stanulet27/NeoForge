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

namespace MenuSystems.SpeechProcessing.Editor
{
    /// <summary>
    /// Modifies how CommandData.cs is displayed within the unity editor so that it is easier to read
    /// </summary>
    [CustomPropertyDrawer(typeof(CommandData))]
    public class CommandDataInspector : PropertyDrawer
    {
        private const string DEFAULT_LABEL = "None";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = GetLabel(property);
            EditorGUI.PropertyField(position, property, label, true);
        }

        private static string GetLabel(SerializedProperty property)
        {
            var enumProperty = property.FindPropertyRelative("speechLabel");
            var enumIndex = enumProperty.enumValueIndex;
            return enumIndex == -1 ? DEFAULT_LABEL : enumProperty.enumDisplayNames[enumIndex];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}