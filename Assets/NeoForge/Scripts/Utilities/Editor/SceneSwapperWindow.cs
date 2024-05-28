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
        private const string SCENE_FOLDER = "Assets/NeoForge/Scenes/";
        private const string FILE_EXTENSION = ".unity";

        enum UserPath
        {
            CHASE,
            CHRIS,
            MAIN
        }
        
        [SerializeField] private UserPath path = UserPath.MAIN;

        private List<string> currentScenes = new();
        
        private void SwapToScene(string sceneName)
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(NameWithoutExtension(sceneName));
            }
            else if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(GetPath() + sceneName, OpenSceneMode.Additive);
            }
        }

        [MenuItem("Tools/Scene Swapper")]
        public static void ShowWindow()
        {
            var window = (SceneSwapperWindow)GetWindow(typeof(SceneSwapperWindow), false, "Scene Swapper");
            window.UpdateSceneList();
        }

        private void OnGUI()
        {
            var lastPath = path;
            path = (UserPath)EditorGUILayout.EnumPopup("User Path", path);
            if (lastPath != path)
            {
                UpdateSceneList();
            }
            
            foreach (var scene in currentScenes)
            {
                if (GUILayout.Button(NameWithoutExtension(scene)))
                {
                    SwapToScene(scene);
                }
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
            currentScenes.Clear();
            foreach (var file in fileInfo.Where(x => x.Name.EndsWith(FILE_EXTENSION)))
            {
                currentScenes.Add(file.Name);
            }
        }
        
        private string GetPath()
        {
            return path switch
            {
                UserPath.CHASE => "Assets/_InDevelopment/Chase/Scenes/",
                UserPath.CHRIS => "Assets/_InDevelopment/Chris/Scenes/",
                UserPath.MAIN => SCENE_FOLDER,
                _ => "Assets/"
            };
        }

        private static string NameWithoutExtension(string name)
        {
            return name.Remove(name.Length - FILE_EXTENSION.Length);
        }
    }
}