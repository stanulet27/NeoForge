using System;
using NeoForge.Input;
using UnityEngine.Device;

namespace UI.Menus
{
    public class PauseMenu : MenuBase
    {
        protected override Action OpenTrigger { get => ControllerManager.OnPause; set => ControllerManager.OnPause = value; }
        protected override Action CloseTrigger { get => ControllerManager.OnClose; set => ControllerManager.OnClose = value; }
        
        private void DEBUG_OpenMenu()
        {
            OpenMenu();
        }
        
        
    
        public void ExitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}