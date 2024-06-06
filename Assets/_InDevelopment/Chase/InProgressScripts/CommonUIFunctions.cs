using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeoForge.UITools
{
    public class CommonUIFunctions : MonoBehaviour
    {
        public void GoToScene(int index)
        {
            StartCoroutine(SceneTools.TransitionToScene(index));
        }
        
        public void ReturnToTitle()
        {
            StartCoroutine(SceneTools.TransitionToScene(0));
        }
        
        public void RestartScene()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.CurrentSceneIndex));
        }

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