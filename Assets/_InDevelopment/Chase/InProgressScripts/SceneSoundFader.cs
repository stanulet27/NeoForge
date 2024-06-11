using System;
using System.Collections;
using UnityEngine;

namespace NeoForge.UI.Scenes
{
    [RequireComponent(typeof(AudioSource))]
    public class SceneSoundFader : MonoBehaviour
    {
        private AudioSource _audioSource;
        private float _startVolume;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _startVolume = _audioSource.volume;

            if (FadeToBlackSystem.Instance == null)
            {
                Debug.LogWarning("No FadeToBlackSystem found, SceneSoundFader will not work");
                enabled = false;
            }
        }

        private void Update()
        {
            _audioSource.volume = (1 - FadeToBlackSystem.Instance.CurrentFadeAmount) * _startVolume;
        }
    }
}