using System;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    [Serializable]
    public class Page
    {
        [SerializeField] private GameObject _display;

        public Page(GameObject display)
        {
            _display = display;
        }
        
        /// <summary>
        /// Returns true if the gameobject of the page is the same as the one passed in.
        /// </summary>
        public bool IsMatch(GameObject page)
        {
            return page.Equals(_display);
        }
    
        /// <summary>
        /// Will toggle the display of the page.
        /// </summary>
        public void ToggleDisplay(bool shouldDisplay)
        {
            _display.SetActive(shouldDisplay);
        }
    }
}