using NeoForge.Input;
using NeoForge.UI.Scenes;
using UnityEngine;

namespace NeoForge.Orders
{
    public class AwaitGoToNextDay : MonoBehaviour
    {
        private void Start()
        {
            ControllerManager.OnNextDay += OnNextDay;
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnNextDay -= OnNextDay;
        }
        
        private void OnNextDay()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneWrapped));
        }
    }
}