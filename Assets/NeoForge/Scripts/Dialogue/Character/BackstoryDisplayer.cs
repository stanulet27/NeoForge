using NeoForge.DaySystem;
using NeoForge.Dialogue;
using NeoForge.UI.Scenes;
using NeoForge.UI.Sound;
using NeoForge.Utilities;
using UnityEngine;

namespace NeoForge.Dialogue.Character
{
    public class BackstoryDisplayer : MonoBehaviour
    {
        private const string BACKSTORY_START = "Backstory-Start";
        
        [Tooltip("The display that will show the backstory.")]
        [SerializeField] private GameObject _display;
        [Tooltip("The sound that will play when the backstory is being written.")]
        [SerializeField] private AudioClip _writingSound;
        [Tooltip("The frequency at which the writing sound will play. Every x characters will play the sound.")]
        [SerializeField] private int _writingSoundFrequency;
        [Tooltip("The song that will play during the backstory.")]
        [SerializeField] private AudioClip _backstorySong;
        [Tooltip("The audio source that will play the background music.")]
        [SerializeField] private AudioSource _backgroundMusicPlayer;
        [Tooltip("The game starter that will start the game after the backstory concludes.")]
        [SerializeField] private GameStarter _gameStarter;
        
        private void Start()
        {
            _display.SetActive(false);
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueEnded -= DialogueManager_OnDialogueEnded;
        }

        /// <summary>
        /// Will display the backstory and start the game when the backstory is finished.
        /// </summary>
        public void Display()
        {
            _display.SetActive(true);
            SetupAudio();
            DialogueManager.OnDialogueEnded += DialogueManager_OnDialogueEnded;
            DialogueManager.Instance.StartDialogueName(BACKSTORY_START);
        }

        private void SetupAudio()
        {
            _backgroundMusicPlayer.clip = _backstorySong;
            _backgroundMusicPlayer.Play();
            _backgroundMusicPlayer.loop = true;
            UISounds.Instance.AdjustDialogueSound(_writingSound, _writingSoundFrequency);
        }

        private void DialogueManager_OnDialogueEnded()
        {
            DialogueManager.OnDialogueEnded -= DialogueManager_OnDialogueEnded;
            UISounds.Instance.AdjustDialogueSound(default, default);
            _gameStarter.StartGame();
        }
    }
}