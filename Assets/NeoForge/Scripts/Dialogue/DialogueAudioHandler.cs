using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace NeoForge.Dialogue
{
    public class DialogueAudioHandler : MonoBehaviour
    {
        [Tooltip("Audio source to play the sound")]
        [SerializeField] private AudioSource _audioSource;
        [Tooltip("Mapping of character names to their respective pitch values")]
        [SerializeField] private SerializedDictionary<string, float> _characterPitches;
        [Tooltip("Default pitch value for characters not in the dictionary")]
        [SerializeField] private float _defaultPitch = 1;
        [Tooltip("Blip sound to play")]
        [SerializeField] private AudioClip _blipSound;
        [Tooltip("Variation in pitch for the blip sound")]
        [SerializeField] private float _variation = 0.2f;
        [Tooltip("Frequency of blip sound, every nth character will play the sound")]
        [SerializeField] private int _frequency = 3;

        private float _pitch = 1;

        private void OnEnable()
        {
            DialogueManager.OnTextSet += SetBlip;
            SetBlip(DialogueManager.Instance.CurrentDialogue);
            DialogueManager.OnTextUpdated += PlayBlip;
        }
        
        private void OnDisable()
        {
            DialogueManager.OnTextSet -= SetBlip;
            DialogueManager.OnTextUpdated -= PlayBlip;
        }
        
        [ContextMenu("Create Audio Dictionary")]
        public void EditorCreateAudioDictionary()
        {
            _characterPitches = new SerializedDictionary<string, float>();
            Resources.LoadAll<ConversationDataSO>("Dialogue")
                .SelectMany(x => x.Data.DialoguesSeries)
                .SelectMany(y => y.dialogues)
                .Select(z => z.SpeakerName)
                .Distinct()
                .ToList().ForEach(x => _characterPitches.Add(x, _defaultPitch));
        }
        
        private void SetBlip(DialogueHelperClass.DialogueData dialogue)
        {
            if (dialogue == null) return;
            var characterName = dialogue.SpeakerName;
            _pitch = _characterPitches.GetValueOrDefault(characterName, _defaultPitch);
        }
        
        private void PlayBlip(string text)
        {
            if (text.Length % _frequency != 0) return;
            _audioSource.pitch = Random.Range(-1f, 1f) * _variation + _pitch;
            _audioSource.PlayOneShot(_blipSound);
        }
    }
}