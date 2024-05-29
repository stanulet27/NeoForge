/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.Editor
{
    /// <summary>
    /// Provides a custom unity editor window to facilitate quick transitioning between scenes.
    /// Included Options:
    ///     - Ability to click button to jump to that scene
    ///     - A List of all scenes located in Assets/Scenes
    ///     - Ability to refresh the scene list
    /// </summary>
    public class SceneSwapperWindow : EditorWindow
    {
        private enum UserPath
        {
            Main,
            Chase,
            Chris
        }

        private const string SCENE_FOLDER = "Assets/NeoForge/Scenes/";
        private const string FILE_EXTENSION = ".unity";
        
        [SerializeField] private UserPath _path = UserPath.Main;

        private readonly List<string> _currentScenes = new();
        
        [MenuItem("Tools/Scene Swapper")]
        public static void ShowWindow()
        {
            var window = (SceneSwapperWindow)GetWindow(typeof(SceneSwapperWindow), false, "Scene Swapper");
            window.UpdateSceneList();
        }
        
        private void SwapToScene(string sceneName)
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(GetNameWithoutExtension(sceneName));
            }
            else if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(GetPath() + sceneName, OpenSceneMode.Single);
            }
        }

        private void OnGUI()
        {
            var lastPath = _path;
            _path = (UserPath)EditorGUILayout.EnumPopup("User Path", _path);
            if (lastPath != _path)
            {
                UpdateSceneList();
            }
            
            foreach (var scene in _currentScenes.Where(scene => GUILayout.Button(GetNameWithoutExtension(scene))))
            {
                SwapToScene(scene);
            }
            
            if (GUILayout.Button("Update Scene List"))
            {
                UpdateSceneList();
            }
        }

        private void UpdateSceneList()
        {
            var info = new DirectoryInfo(GetPath());
            var fileInfo = info.GetFiles();
            _currentScenes.Clear();
            foreach (var file in fileInfo.Where(x => x.Name.EndsWith(FILE_EXTENSION)))
            {
                _currentScenes.Add(file.Name);
            }
        }
        
        private string GetPath()
        {
            return _path switch
            {
                UserPath.Chase => "Assets/_InDevelopment/Chase/Scenes/",
                UserPath.Chris => "Assets/_InDevelopment/Chris/Scenes/",
                UserPath.Main => SCENE_FOLDER,
                _ => "Assets/"
            };
        }

        private static string GetNameWithoutExtension(string name)
        {
            return name.Remove(name.Length - FILE_EXTENSION.Length);
        }
    }
}