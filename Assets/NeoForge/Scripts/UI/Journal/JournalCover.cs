using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.UI.Buttons;
using NeoForge.UI.Inventory;
using NeoForge.UI.Journal;
using UnityEngine;

namespace NeoForge.UI.Journal
{
    public class JournalCover : MonoBehaviour
    {
        [Tooltip("The button group that will contain the navigation buttons")]
        [SerializeField] private ButtonGroup _buttonGroup;
        
        /// <summary>
        /// Will handle displaying the table of contents for the journal. As well as the action to take when an
        /// option is selected.
        /// </summary>
        public void Display(List<MaterialData> materials, Action<MaterialData> displayMaterial)
        {
            gameObject.SetActive(true);
            var buttons = _buttonGroup.GetComponentsInChildren<JournalNavigationButton>(true).ToList();
            buttons.ForEach(x => x.gameObject.SetActive(false));
            for (var i = 0; i < materials.Count; i++)
            {
                var button = buttons[i];
                var material = materials[i];
                button.gameObject.SetActive(true);
                button.SetButton(material.Name, i + 1, () => displayMaterial(material));
            }
            buttons[0].Select();
        }

        /// <summary>
        /// Will hide the journal cover.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}