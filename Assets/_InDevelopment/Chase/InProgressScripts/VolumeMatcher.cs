using System;
using CustomInspectors;
using SharedData;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class VolumeMatcher : MonoBehaviour
    {
        [SerializeField] private SharedFloat _masterVolume;
        [SerializeField] private SharedFloat _specificVolume;
        [SerializeField, ReadOnly] private float _volume;
        
        private AudioSource _audioSource;
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