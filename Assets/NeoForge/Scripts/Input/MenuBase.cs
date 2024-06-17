using System;
using System.Collections.Generic;
using NeoForge.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public abstract class MenuBase : MonoBehaviour
    {
        protected abstract Action OpenTrigger { get; set; }
        protected abstract Action CloseTrigger { get; set; }
        
        [SerializeField] private GameObject _display;
        [SerializeField] private List<Page> _pages;
        
        protected virtual bool StartOpen => false;
        protected bool OnFirstPage => _currentPage == 0;
        private int _currentPage;

        protected virtual void Awake()
        {
            if (_display == null) _display = transform.GetChild(0).gameObject;
            if (_pages.Count == 0) _pages.Add(new Page(_display));
            _pages.ForEach(x => x.ToggleDisplay(false));
        }
        
        private void OnEnable()
        {
            if(StartOpen) OpenMenu(); 
            else CloseMenu();
            
            OpenTrigger += OpenMenu;
        }
        
        private void JumpToStartPage() => JumpToPage(0);

        /// <summary>
        /// Will open the menu and swap to UI mode. Will jump to the first page if there are multiple pages.
        /// </summary>
        [Button]
        public virtual void OpenMenu()
        {
            _display.SetActive(true);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            if (_pages.Count > 1) ControllerManager.OnGoBack += JumpToStartPage;
            JumpToPage(0);
            CloseTrigger += CloseMenu;
        }

        /// <summary>
        /// Will swap the current page to the given index.
        /// </summary>
        /// <param name="pageIndex">Index of the new page</param>
        public virtual void JumpToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _pages.Count)
            {
                Debug.LogError("Invalid page index");
                return;
            }

            _pages[_currentPage].ToggleDisplay(false);
            _pages[pageIndex].ToggleDisplay(true);
            _currentPage = pageIndex;
        }
    
        /// <summary>
        /// Will swap the current page to the given page.
        /// </summary>
        /// <param name="page">References to the gameobject of the other display</param>
        public void JumpToPage(GameObject page)
        {
            JumpToPage(_pages.FindIndex(p => p.IsMatch(page)));
        }

        private void HideAllPages()
        {
            _pages.ForEach(p => p.ToggleDisplay(false));
        }

        /// <summary>
        /// Will close the menu and return to gameplay mode.
        /// </summary>
        [Button]
        public virtual void CloseMenu()
        {
            _display.SetActive(false);
            HideAllPages();
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
            if (_pages.Count > 1) ControllerManager.OnGoBack -= JumpToStartPage;
            CloseTrigger -= CloseMenu;
        }

        private void OnDisable()
        {
            OpenTrigger -= OpenMenu;
            CloseTrigger -= CloseMenu;
        }

        private void OnDestroy()
        {
            ControllerManager.OnGoBack -= JumpToStartPage;
        }
    }
}