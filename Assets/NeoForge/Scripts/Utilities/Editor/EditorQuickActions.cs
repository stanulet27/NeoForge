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

namespace Utilities.Editor
{
    /// <summary>
    /// Provides a custom editor to execute common time consuming unity tasks.
    /// Included Tasks:
    ///     - Destroy Meshes in Children
    ///     - Destroy Colliders
    ///     - Destroy Rigidbodies
    ///     - Clear Tags
    /// </summary>
    public class EditorQuickActions : EditorWindow
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private bool _includeChildren;
        
        [MenuItem("Tools/Editor Actions")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EditorQuickActions), false, "Editor Tools");
        }

        private void OnGUI()
        {
            _target = EditorGUILayout.ObjectField("Target: ", _target, typeof(GameObject), true) as GameObject;
            _includeChildren = EditorGUILayout.Toggle("Include children: ", _includeChildren);
            
            if (GUILayout.Button("Destroy Meshes"))
            {
                DestroyComponent<MeshRenderer>(_target, _includeChildren);
            }
            
            if (GUILayout.Button("Destroy Colliders"))
            {
                DestroyComponent<Collider>(_target, _includeChildren);
            }

            if (GUILayout.Button("Destroy Rigidbodies"))
            {
                DestroyComponent<Rigidbody>(_target, _includeChildren);
            }

            if (GUILayout.Button("Clear Tags"))
            {
                SetTags(_target, _includeChildren);
            }
        }

        private static void DestroyComponent<T>(GameObject gameObject, bool includeChildren) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                DestroyImmediate(component);
            }
            
            if (!includeChildren) return;

            foreach (var child in gameObject.transform.GetComponentsInChildren<T>(true))
            {
                DestroyImmediate(child);
            }
        }

        private static void SetTags(GameObject gameObject, bool includeChildren = true)
        {
            gameObject.tag = "Untagged";
            
            if (!includeChildren) return;
            foreach (Transform child in gameObject.transform)
            {
                child.tag = "Untagged";
            }
        }
    }
}