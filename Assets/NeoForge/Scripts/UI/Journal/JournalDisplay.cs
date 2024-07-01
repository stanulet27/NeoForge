using System.Collections.Generic;
using NeoForge.Input;
using NeoForge.UI.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NeoForge.UI.Journal
{
    public class JournalDisplay : MonoBehaviour
    {
        [Tooltip("The journal cover that will display the table of contents")]
        [SerializeField] private JournalCover _cover;
        [Tooltip("The journal page that will display the material information")]
        [SerializeField] private JournalPage _page;
        [Tooltip("The materials that will be displayed in the table of contents")]
        [SerializeField] private List<MaterialData> _materials;
        [Tooltip("The display object that will be toggled on and off for hiding and showing the journal menu")]
        [SerializeField] private GameObject _display;
        [Tooltip("Will trigger when the journal is opened")]
        [SerializeField] private UnityEvent _onOpen;
        [Tooltip("Will trigger when the journal is closed")]
        [SerializeField] private UnityEvent _onClose;

        private bool _onCover;
        
        private void Start()
        {
            HideMenu();
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        /// <summary>
        /// Will open the journal menu and swap to UI mode.
        /// </summary>
        [Button]
        public void OpenMenu()
        {
            _display.SetActive(true);
            ReturnToTitle();
            _onOpen.Invoke();
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            ControllerManager.OnClose += CloseMenu;
            ControllerManager.OnGoBack += GoBackAPage;
        }
        
        /// <summary>
        /// Will close the journal menu and swap to gameplay mode.
        /// </summary>
        [Button]
        public void CloseMenu()
        {
            HideMenu();
            _onClose.Invoke();
            UnsubscribeToEvents();
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }
        
        private void UnsubscribeToEvents()
        {
            ControllerManager.OnClose -= CloseMenu;
            ControllerManager.OnGoBack -= GoBackAPage;
        }

        private void GoBackAPage()
        {
            if (_onCover) CloseMenu();
            else ReturnToTitle();
        }
        
        private void HideMenu()
        {
            _display.SetActive(false);
            _page.Hide();
            _cover.Hide();
        }

        private void ReturnToTitle()
        {
            _cover.Display(_materials, DisplayMaterial);
            _page.Hide();
            _onCover = true;
        }

        private void DisplayMaterial(MaterialData data)
        {
            _cover.Hide();
            _page.DisplayMaterial(data);
            ControllerManager.OnGoBack += GoBackAPage;
            _onCover = false;
        }
    }
}