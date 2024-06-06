using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace NeoForge.UI.Scenes
{
    public static class SceneTools
    {
        public static Action<int> OnSceneTransitionStart;
        
        /// <summary>
        /// Returns true if the scene is currently in the process of transitioning.
        /// </summary>
        public static bool Transitioning = false;

        /// <summary>
        /// Returns true if there is a scene after the current scene in the build index.
        /// </summary>
        public static bool NextSceneExists =>
            NextSceneIndex < SceneManager.sceneCountInBuildSettings;

        /// <summary>
        /// Returns the index of the scene after the current scene in the build index.
        /// </summary>
        public static int NextSceneIndex =>
            CurrentSceneIndex + 1;

        /// <summary>
        /// Returns the index of the current scene in the build index.
        /// </summary>
        public static int CurrentSceneIndex =>
            SceneManager.GetActiveScene().buildIndex;

        /// <summary>
        /// Will transition to the scene with the given index. Must match the build index.
        /// </summary>
        /// <param name="sceneIndex">The desired scene's build index</param>
        public static IEnumerator TransitionToScene(int sceneIndex)
        {
            if (Transitioning) yield break;
            
            Transitioning = true;
            yield return HandleTransitionTo(sceneIndex);
            Transitioning = false;
        }
        
        private static IEnumerator HandleTransitionTo(int sceneIndex)
        {    
            OnSceneTransitionStart?.Invoke(sceneIndex);
            yield return FadeToBlackSystem.TryCueFadeInToBlack(1f);
            SceneManager.LoadScene(sceneIndex);
        }
    }
}