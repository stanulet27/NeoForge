using NeoForge.UI.Scenes;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public class CommonUIFunctions : MonoBehaviour
    {
        /// <summary>
        /// Will transition to the scene with the given index. Must match the build index.
        /// </summary>
        /// <param name="index">The index of the desired scene to goto</param>
        public void GoToScene(int index)
        {
            StartCoroutine(SceneTools.TransitionToScene(index));
        }
        
        /// <summary>
        /// Will transition to the scene at build index 0.
        /// </summary>
        public void ReturnToTitle()
        {
            StartCoroutine(SceneTools.TransitionToScene(0));
        }
        
        /// <summary>
        /// Will transition to the current scene, effectively restarting it.
        /// </summary>
        public void RestartScene()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.CurrentSceneIndex));
        }

        /// <summary>
        /// Will quit the game.
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}