/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Will add the first scene to the loaded scenes when the game starts up and is a build. Mainly meant to compensate
    /// for MRTK having the first scene in the build index be the SceneManager instead of the first scene that is meant
    /// to be loaded.
    /// </summary>
    public class LoadFirstScene : MonoBehaviour
    {
        #if !UNITY_EDITOR
        private void Awake()
        {
            SceneLoadingSystem.LoadFirstScene();
        }
        #endif
    }
}