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
            Debug.Assert(FadeToBlackSystem.Instance != null, "No FadeToBlackSystem found, SceneSoundFader will not work");
            
            _audioSource = GetComponent<AudioSource>();
            _startVolume = _audioSource.volume;
        }

        private void Update()
        {
            _audioSource.volume = (1 - FadeToBlackSystem.Instance.CurrentFadeAmount) * _startVolume;
        }
    }
}