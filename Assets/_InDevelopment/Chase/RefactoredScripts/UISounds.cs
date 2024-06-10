using System;
using NeoForge.Utilities;
using SharedData;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class UISounds : SingletonMonoBehaviour<UISounds>
    {
        [Tooltip("The volume to play the audio clip at")]
        [SerializeField] private SharedFloat _volume;
        [Tooltip("The audio clip to play when the button is triggered.")]
        [SerializeField] private AudioClip _sfx;
        [Tooltip("The audio clip to play when the button is triggered successfully.")]
        [SerializeField] private AudioClip _success;
        [Tooltip("The audio clip to play when the button is triggered unsuccessfully.")]
        [SerializeField] private AudioClip _failure;
        
        private AudioSource _audioSource;
        
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void Play()
        {
            _audioSource.PlayOneShot(_sfx, _volume);
        }
        
        public void PlaySuccess()
        {
            _audioSource.PlayOneShot(_success, _volume);
        }
        
        public void PlayFailure()
        {
            _audioSource.PlayOneShot(_failure, _volume);
        }
    }
}