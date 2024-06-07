using System.Collections;
using NeoForge.Input;
using NeoForge.UI.Buttons;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public class RemappingButton : UIButton
    {
        [Tooltip("The name of the key that will be remapped. Should match a key in the PlayerInput.")]
        [SerializeField] private string _keyName;
        [Tooltip("The text that will display the current key.")]
        [SerializeField] private TMP_Text _currentKeyText;
        [Tooltip("The text that will display the key name.")]
        [SerializeField] private TMP_Text _keyNameText;
        
        private ControllerManager _controller;
        
        protected override void Awake()
        {
            base.Awake();
            _controller = ControllerManager.Instance;
            _keyNameText.text = _keyName;
        }

        private void OnEnable()
        {
            ReplaceKey();
        }

        private void ReplaceKey()
        {
            var keyText = _controller.GetKey(_keyName);
            keyText = keyText.Replace("D-Pad/", "");
            _currentKeyText.text = keyText;
        }

        /// <summary>
        /// Will trigger a remapping of the key.
        /// </summary>
        public override void Use()
        {
            StartCoroutine(HandleKeyRemapping());
        }
        
        private IEnumerator HandleKeyRemapping()
        {
            _currentKeyText.text = "Press a key";
            yield return _controller.AllowUserToSetKey(_keyName);
            ReplaceKey();
        }
    }
}