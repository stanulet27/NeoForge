using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Scenes
{
    public class SceneEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onSceneStart;
        [SerializeField] private UnityEvent _onFadeToBlackCompleted;
        
        private void Start()
        {
            _onSceneStart?.Invoke();
            StartCoroutine(AwaitFadeToBlack());
        }
        
        private IEnumerator AwaitFadeToBlack()
        {
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);
            _onFadeToBlackCompleted?.Invoke();
        }
    }
}