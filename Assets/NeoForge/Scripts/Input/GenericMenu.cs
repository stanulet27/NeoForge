using System;
using NeoForge.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.UI.Menus
{
    public class GenericMenu : MenuBase
    {
        [SerializeField, EnumToggleButtons] private OnOff _startsOpen;
        protected override bool StartOpen => _startsOpen == OnOff.On;
        protected override Action OpenTrigger { get ; set; }
        protected override Action CloseTrigger { get; set; }
    }
}