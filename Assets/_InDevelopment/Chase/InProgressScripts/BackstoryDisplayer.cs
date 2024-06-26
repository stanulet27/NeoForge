using NeoForge.Dialogue;
using NeoForge.UI.Scenes;
using NeoForge.UI.Sound;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public class BackstoryDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject _display;
        [SerializeField] private SharedCharacterData _sharedCharacterData;
        [SerializeField] private AudioClip _writingSound;
        [SerializeField] private int _writingSoundFrequency;
        [SerializeField] private AudioClip _backstorySong;
        [SerializeField] private AudioSource _backgroundMusicPlayer;

        private bool _traitsDisplayed;
        
        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueEnded -= OnDialogueEnded;
        }

        public void Display()
        {
            _display.SetActive(true);
            _traitsDisplayed = false;
            _backgroundMusicPlayer.clip = _backstorySong;
            _backgroundMusicPlayer.Play();
            _backgroundMusicPlayer.loop = true;
            UISounds.Instance.AdjustDialogueSound(_writingSound, _writingSoundFrequency);
            DialogueManager.OnDialogueEnded += OnDialogueEnded;
            DialogueManager.Instance.StartDialogueName("Backstory-" + _sharedCharacterData.Data.Background);
        }

        private void Hide()
        {
            _display.SetActive(false);
        }
        
        private void OnDialogueEnded()
        {
            if (!_traitsDisplayed)
            {
                DisplayTraitBackstory();
            }
            else
            {
                GoToNextScene();
            }
        }

        private void DisplayTraitBackstory()
        {
            _traitsDisplayed = true;
            DialogueManager.Instance.StartDialogueName("Backstory-" + _sharedCharacterData.Data.Personality);
        }

        private void GoToNextScene()
        {
            DialogueManager.OnDialogueEnded -= OnDialogueEnded;
            UISounds.Instance.AdjustDialogueSound(default, default);
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneWrapped));
        }
    }
}