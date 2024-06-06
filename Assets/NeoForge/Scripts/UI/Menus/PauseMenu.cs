using System;
using NeoForge.Input;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class PauseMenu : MenuBase
    {
        protected override Action OpenTrigger { get => ControllerManager.OnPause; set => ControllerManager.OnPause = value; }
        protected override Action CloseTrigger { get => ControllerManager.OnClose; set => ControllerManager.OnClose = value; }
        
        [ContextMenu("Open Menu")]
        private void DEBUG_OpenMenu()
        {
            OpenMenu();
        }
    }
}