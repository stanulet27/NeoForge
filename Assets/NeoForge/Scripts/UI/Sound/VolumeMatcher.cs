using System;
using CustomInspectors;
using SharedData;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class VolumeMatcher : MonoBehaviour
    {
        [Tooltip("The game's master volume.")]
        [SerializeField] private SharedFloat _masterVolume;
        [Tooltip("The specific volume for this audio source. (Ex. music, sound effects, etc.)")]
        [SerializeField] private SharedFloat _specificVolume;
        [Tooltip("The volume of the audio source w/o modifiers.")]
        [SerializeField, ReadOnly] private float _volume;
        
        private AudioSource _audioSource;
        
        /// <summary>
        /// The volume of the audio source w/o modifiers. Clamped between 0 and 1.
        /// </summary>
        public float Volume { get => _volume; private set => _volume = Mathf.Clamp01(value); }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            Volume = _audioSource.volume;
            UpdateVolume();
        }

        private void OnEnable()
        {
            _masterVolume.OnValueChanged += UpdateVolume;
            _specificVolume.OnValueChanged += UpdateVolume;
        }
        
        private void OnDisable()
        {
            _masterVolume.OnValueChanged -= UpdateVolume;
            _specificVolume.OnValueChanged -= UpdateVolume;
        }
        
        public void SetVolume(float volume)
        {
            Volume = volume;
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            _audioSource.volume = Volume * _specificVolume * _masterVolume;
        }
    }
}