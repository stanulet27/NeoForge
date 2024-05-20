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
using System.Linq;

namespace CustomInspectors.Editor
{
    /// <summary>
    /// Handles the Editor Logic for the ExpandedClassAttribute.cs
   /// </summary>
    [CustomPropertyDrawer(typeof(ExpandedClassAttribute))]
    public class ExpandedClassPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int i = 0;
            foreach (var child in property)
            {
                if (child is not SerializedProperty childProperty) continue;
                // Calculate rects
                var unitRect = new Rect(position.x, position.y + i * 22, position.width, 20);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(unitRect, childProperty, new GUIContent(childProperty.displayName));
                i++;
            }

            position.height = i * position.height;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int i = 0;
            
            foreach (var _ in property)
            {
                i++;
            }

            return 20 * i;
        }
    }
}