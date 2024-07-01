using System;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Journal
{
    public class JournalNavigationButton : HoverButton
    {
        [Tooltip("The title of the page to be navigated to")]
        [SerializeField] private TMP_Text _pageTitle;
        [Tooltip("The page number of the page to be navigated to")]
        [SerializeField] private TMP_Text _pageNumber;
        
        private Action _onClick;
        
        /// <summary>
        /// Will display a table of contents entry. The title will be displayed in the _pageTitle text field,
        /// the page number will be displayed in the _pageNumber text field, and the onClick action will be invoked
        /// upon being pressed.
        /// </summary>
        public void SetButton(string title, int pageNumber, Action onClick)
        {
            _onClick = onClick;
            _pageTitle.text = title;
            _pageNumber.text = $"Pg {pageNumber}";
        }

        public override void Use()
        {
            _onClick?.Invoke();
        }
    }
}