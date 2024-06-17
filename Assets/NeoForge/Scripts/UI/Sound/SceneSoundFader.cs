using System;
using NeoForge.UI.Scenes;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SceneSoundFader : MonoBehaviour
    {
        private AudioSource _audioSource;
        private VolumeMatcher _volumeMatcher;
        private bool _volumeMatcherFound;
        private float _startVolume;
        private float _lastFadeAmount;

        private void Awake()
        {
            Debug.Assert(FadeToBlackSystem.Instance != null, "No FadeToBlackSystem found, SceneSoundFader will not work");
            
            _audioSource = GetComponent<AudioSource>();
            _volumeMatcherFound = TryGetComponent(out _volumeMatcher);
        }

        private void Start()
        {
            _startVolume = _volumeMatcherFound ? _volumeMatcher.Volume : _audioSource.volume;
            _lastFadeAmount = FadeToBlackSystem.Instance.CurrentFadeAmount;
            UpdateVolume();
        }

        private void Update()
        {
            if (Math.Abs(FadeToBlackSystem.Instance.CurrentFadeAmount - _lastFadeAmount) < 0.001f)
            {
                return;
            }
            _lastFadeAmount = FadeToBlackSystem.Instance.CurrentFadeAmount;
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            var newVolume = (1 - _lastFadeAmount) * _startVolume;
            if (_volumeMatcherFound)
            {
                _volumeMatcher.SetVolume(newVolume);
            }
            else
            {
                _audioSource.volume = newVolume;
            }
        }
    }
}