using UnityEngine;

namespace NeoForge.UI.Scenes
{
    public class SceneTransistor : MonoBehaviour
    {
        public void GoToNextScene()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneWrapped));
        }

        public void GoToScene(int index)
        {
            StartCoroutine(SceneTools.TransitionToScene(index));
        }
    }
}