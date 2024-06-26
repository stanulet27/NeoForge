using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Input;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NeoForge.UI.Tools
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject _display;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private GameObject _pronounButtonGroup;
        [SerializeField] private GameObject _traitButtonGroup;
        [SerializeField] private GameObject _familyButtonGroup;
        [SerializeField] private SharedCharacterData _sharedCharacterData;
        [SerializeField] private Image _portrait;
        [SerializeField] private Sprite[] _portraits;
        [SerializeField] private UnityEvent _onCharacterCreated;

        private readonly List<CharacterTraitTracker> _pronounButtons = new();
        private readonly List<CharacterTraitTracker> _traitButtons = new();
        private readonly List<CharacterTraitTracker> _familyButtons = new();

        private int _currentIndex;
        
        private void Awake()
        {
            _pronounButtonGroup.GetComponentsInChildren(_pronounButtons);
            _pronounButtons.ForEach(x => x.OnClicked += OnPronounClicked);
            _traitButtonGroup.GetComponentsInChildren(_traitButtons);
            _traitButtons.ForEach(x => x.OnClicked += OnTraitClicked);
            _familyButtonGroup.GetComponentsInChildren(_familyButtons);
            _familyButtons.ForEach(x => x.OnClicked += OnFamilyClicked);
        }

        private void Start()
        {
            _portrait.sprite = _portraits[0];
            Hide();
        }

        public void Display()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _display.SetActive(true);
            _nameInput.Select();
            _pronounButtons[0].Set(true);
            _traitButtons[0].Set(true);
            _familyButtons[0].Set(true);
        }
        
        public void Hide()
        {
            _display.SetActive(false);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }
        
        public void CreateCharacter()
        {
            if (string.IsNullOrWhiteSpace(_nameInput.text)) return;
            
            var character = new CharacterData
            {
                Name = _nameInput.text,
                Pronouns = _pronounButtons
                    .Select((button, index) => (button, index))
                    .Where(x => x.button.IsActive)
                    .Select(x => (CharacterData.Pronoun)x.index)
                    .ToList(),
                Personality = (CharacterData.Trait)_traitButtons.IndexOf(_traitButtons.Find(x => x.IsActive)),
                Background = (CharacterData.Family)_familyButtons.IndexOf(_familyButtons.Find(x => x.IsActive)),
                Portrait = _portraits[_currentIndex]
            };
            
            _sharedCharacterData.RewriteData(character);
            _onCharacterCreated.Invoke();
        }
        
        public void NextPortrait()
        {
            OffsetCurrentPortraitBy(1);
        }
        
        public void PreviousPortrait()
        {
            OffsetCurrentPortraitBy(-1);
        }

        private void OffsetCurrentPortraitBy(int offset)
        {
            _currentIndex = (_currentIndex + offset + _portraits.Length) % _portraits.Length;
            _portrait.sprite = _portraits[_currentIndex];
        }
        
        private void OnPronounClicked(CharacterTraitTracker tracker, bool isActive)
        {
            if (_pronounButtons.All(x => !x.IsActive)) tracker.Set(true);
        }
        
        private void OnTraitClicked(Object tracker, bool isActive)
        {
            _traitButtons.ForEach(x => x.Set(x == tracker));
        }
        
        private void OnFamilyClicked(Object tracker, bool isActive)
        {
            _familyButtons.ForEach(x => x.Set(x == tracker));
        }
    }
}