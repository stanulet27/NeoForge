using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NeoForge.UI.Scenes
{
    public class SceneEvents : MonoBehaviour
    {
        [Tooltip("Will trigger when the scene starts.")]
        [SerializeField] private UnityEvent _onSceneStart;
        [FormerlySerializedAs("_onFadeToBlackCompleted")]
        [Tooltip("Will trigger when the scene finishes fading out (Alpha goes from 1 - 0).")]
        [SerializeField] private UnityEvent _onFadeOutCompleted;
        
        private void Start()
        {
            _onSceneStart?.Invoke();
            StartCoroutine(AwaitFadeToBlack());
        }
        
        private IEnumerator AwaitFadeToBlack()
        {
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);
            _onFadeOutCompleted?.Invoke();
        }
    }
}