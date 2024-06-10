using UnityEngine;

namespace NeoForge.Dialogue.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogueAudioCueHandler : MonoBehaviour
    {
        [Tooltip("The audio database containing all audio cues.")]
        [SerializeField] AudioDatabase _audioDatabase;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            DialogueManager.OnAudioCue += PlayAudioCue;
        }
        
        private void OnDisable()
        {
            DialogueManager.OnAudioCue -= PlayAudioCue;
        }

        /// <summary>
        /// Will trigger the audio cue to play by finding the audio clip in the audio database.
        /// </summary>
        /// <param name="audioCue">The name corresponding to a clip in the audio database</param>
        public void PlayAudioCue(string audioCue)
        {
            if (!_audioDatabase.AudioClips.TryGetValue(audioCue, out var clip)) return;
            _audioSource.PlayOneShot(clip);
        }
    }
}