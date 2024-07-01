using NeoForge.Dialogue.Audio;
using NeoForge.Utilities;
using SharedData;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(DialogueAudioHandler))]
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
        private DialogueAudioHandler _dialogueAudioHandler;
        

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _dialogueAudioHandler = GetComponent<DialogueAudioHandler>();
        }
        
        /// <summary>
        /// Will trigger the normal click sound.
        /// </summary>
        public void Play()
        {
            _audioSource.PlayOneShot(_sfx, _volume);
        }
        
        /// <summary>
        /// Will trigger the success sound.
        /// </summary>
        public void PlaySuccess()
        {
            _audioSource.PlayOneShot(_success, _volume);
        }
        
        /// <summary>
        /// Will trigger the failure sound.
        /// </summary>
        public void PlayFailure()
        {
            _audioSource.PlayOneShot(_failure, _volume);
        }

        /// <summary>
        /// Will adjust the sound of the dialogue audio handler. If no clip is provided, the default clip will be used.
        /// If no frequency is provided, the default frequency will be used.
        /// </summary>
        /// <param name="clip">The audio clip to play when the dialogue clip is triggered.</param>
        /// <param name="frequency">The amount of characters per triggering the clip.</param>
        public void AdjustDialogueSound(AudioClip clip = default, int frequency = 0)
        {
            _dialogueAudioHandler.SetSound(clip, frequency);
        }
    }
}