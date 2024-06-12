using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.Dialogue.UI
{
    public class ChoicesDisplay : MonoBehaviour
    {
        [Tooltip("The prefab of the choices to instantiate")]
        [SerializeField] private GameObject _choiceTemplate;

        private readonly List<RectTransform> _choices = new();
        private readonly List<UIButton> _choiceButtons = new();

        private Action<int> _onClick;

        /// <summary>
        /// Will display the choices to the player. It will create a button for each choice and call the onClick action
        /// with the index of the choice selected upon user selection.
        /// </summary>
        /// <param name="validChoices">The choices available to the user</param>
        /// <param name="onClick">What to do upon selection of a choice</param>
        public void Display(List<string> validChoices, Action<int> onClick)
        {
            foreach(var choiceOption in validChoices)
            {
                var instance = Instantiate(_choiceTemplate, transform);
                var textBox = instance.transform.GetComponentInChildren<TextMeshProUGUI>();
                var uiButton = instance.transform.GetComponent<UIButton>();
                
                textBox.text = choiceOption;
                uiButton.OnClick += UiButton_OnClick;
                _choices.Add(instance.GetComponent<RectTransform>());
                _choiceButtons.Add(uiButton);
            }
            
            _choiceButtons[0].Select();
            _onClick = onClick;
        }
        
        /// <summary>
        /// Will select the choice at the index.
        /// </summary>
        /// <param name="index">The index of the choice to select</param>
        public void SelectChoice(int index)
        {
            _choiceButtons[index].Select();
        }

        public void Hide()
        {
            DestroyChildren();
        }

        private void UiButton_OnClick(IButton obj)
        {
            _onClick(_choices.Select(x => x.GetComponent<IButton>()).ToList().IndexOf(obj));
        }

        private void DestroyChildren()
        {
            var children = transform.childCount - 1;
            while(children >= 0)
            {
                _choiceButtons[children].OnClick -= UiButton_OnClick;
                Destroy(transform.GetChild(children).gameObject);
                children--;
            }
            _choices.Clear();
            _choiceButtons.Clear();
        }
    }
}
