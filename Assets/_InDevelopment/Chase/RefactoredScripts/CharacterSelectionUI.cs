using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue.Helper;
using NeoForge.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NeoForge.Dialogue.Character
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _display;
        [SerializeField] private SharedCharacterData _sharedCharacterData;
        [SerializeField] private Sprite[] _portraits;
        
        [Header("Character Details")]
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private GameObject _pronounButtonGroup;
        [SerializeField] private GameObject _traitButtonGroup;
        [SerializeField] private GameObject _familyButtonGroup;
        [SerializeField] private Image _portrait;
        
        [Header("Events")]
        [SerializeField] private UnityEvent _onCharacterCreated;

        private readonly List<CharacterTraitTracker> _pronounButtons = new();
        private readonly List<CharacterTraitTracker> _traitButtons = new();
        private readonly List<CharacterTraitTracker> _familyButtons = new();

        private int _currentIndex;

        private void Awake()
        {
            SetupButtons(_pronounButtonGroup, OnPronounClicked, _pronounButtons);
            SetupButtons(_traitButtonGroup, OnTraitClicked, _traitButtons);
            SetupButtons(_familyButtonGroup, OnFamilyClicked, _familyButtons);
            _portrait.sprite = _portraits[0];
        }

        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// Will display the character creation UI. This will also swap the controller mode to UI.
        /// This will also set the default values for the character creation UI.
        /// </summary>
        public void Display()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _display.SetActive(true);
            _nameInput.Select();
            _pronounButtons[0].Set(true);
            _traitButtons[0].Set(true);
            _familyButtons[0].Set(true);
        }
        
        /// <summary>
        /// Will hide the character creation UI. This will also swap the controller mode to Gameplay.
        /// </summary>
        public void Hide()
        {
            _display.SetActive(false);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }
        
        /// <summary>
        /// Will create a character with the current values in the character creation UI.
        /// Will not create a character if the name field is empty.
        /// Will set the global character data to the created character.
        /// Will invoke the OnCharacterCreated event.
        /// </summary>
        public void CreateCharacter()
        {
            if (string.IsNullOrWhiteSpace(_nameInput.text)) return;
            
            var character = new CharacterData
            (
                name: _nameInput.text,
                pronouns: _pronounButtons
                    .Select((button, index) => (button, index))
                    .Where(x => x.button.IsActive)
                    .Select(x => (DialogueVariables.Pronoun)x.index)
                    .ToList(),
                personality: (CharacterData.Trait)_traitButtons.IndexOf(_traitButtons.Find(x => x.IsActive)),
                background: (CharacterData.Family)_familyButtons.IndexOf(_familyButtons.Find(x => x.IsActive)),
                portrait: _portraits[_currentIndex]
            );
            
            _sharedCharacterData.RewriteData(character);
            _onCharacterCreated.Invoke();
        }
        
        /// <summary>
        /// Will cycle to the next portrait in the list.
        /// </summary>
        public void NextPortrait()
        {
            OffsetCurrentPortraitBy(1);
        }
        
        /// <summary>
        /// Will cycle to the previous portrait in the list.
        /// </summary>
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

        private static void SetupButtons(GameObject container, 
            Action<CharacterTraitTracker, bool> clickAction, List<CharacterTraitTracker> buttons)
        {
            container.GetComponentsInChildren(buttons);
            buttons.ForEach(x => x.OnClicked += clickAction);
        }
    }
}