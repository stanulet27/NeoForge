using System;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class GenericMenu : MenuBase
    {
        [SerializeField] private bool _startsOpen;
        protected override bool StartOpen => _startsOpen;
        protected override Action OpenTrigger { get ; set; }
        protected override Action CloseTrigger { get; set; }
    }
}