using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NeoForge.Utilities;

namespace NeoForge.UI.Scenes
{
    public class FadeToBlackSystem : SingletonMonoBehaviour<FadeToBlackSystem>
    {
        /// <summary>
        /// Returns true when the fade out system is fully transparent.
        /// </summary>
        public static bool FadeOutComplete => Instance == null || Instance._image.color.a == 0;

        [Tooltip("When true, the screen will start black. When false, the screen will start transparent.")]
        [SerializeField] bool _startFullAlpha;

        [Tooltip("When true, the screen will fade out automatically when the scene is loaded.")]
        [SerializeField] bool _autoFadeOut;

        [Tooltip("The image that will be faded in and out.")] 
        [SerializeField] private Image _image;
        
        /// <summary>
        /// Returns the current fade amount.
        /// </summary>
        public float CurrentFadeAmount => _image.color.a;

        private void Start()
        {
            SetFadePercentage(_startFullAlpha ? 1 : 0);
            CheckForAutoFadeOut(default, default);
            SceneManager.sceneLoaded += CheckForAutoFadeOut;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= CheckForAutoFadeOut;
        }

        /// <summary>
        /// Will trigger a fading to black effect, setting the alpha from 0 => 1.
        /// Remember to put inside a StartCoroutine call.
        /// </summary>
        /// <param name="duration">How long it will take to transition</param>
        public static IEnumerator TryCueFadeInToBlack(float duration)
        {
            if (Instance != null)
            {
                yield return Instance.Transition(0, 1, duration);
            }
        }

        /// <summary>
        /// Will trigger a fading from black effect, setting the alpha from 1 => 0.
        /// Remember to put inside a StartCoroutine call.
        /// </summary>
        /// <param name="duration">How long it will take to transition</param>
        public static IEnumerator TryCueFadeOutOfBlack(float duration)
        {
            if (Instance != null)
            {
                yield return Instance.Transition(1, 0, duration);
            }
        }

        private void CheckForAutoFadeOut(Scene _, LoadSceneMode __)
        {
            if (!_autoFadeOut) return;

            SetFadePercentage(1);
            StartCoroutine(HandleStartOfScene());
        }

        private void SetFadePercentage(float percentage)
        {
            var temp = _image.color;
            temp.a = percentage;
            _image.color = temp;
        }

        private IEnumerator HandleStartOfScene()
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(TryCueFadeOutOfBlack(1.5f));
        }

        private IEnumerator Transition(float start, float end, float transitionDuration)
        {
            SetFadePercentage(start);

            var startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < transitionDuration)
            {
                SetFadePercentage(Mathf.Lerp(start, end, (Time.realtimeSinceStartup - startTime) / transitionDuration));
                yield return null;
            }

            SetFadePercentage(end);
        }
    }
}