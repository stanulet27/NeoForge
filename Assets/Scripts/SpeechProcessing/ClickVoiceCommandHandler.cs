using System;
using MenuSystems.SpeechProcessing;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;

namespace SpeechProcessing
{
    [RequireComponent(typeof(BasicSpeechCommandListener))]
    public class ClickVoiceCommandHandler : MonoBehaviour, IMixedRealityFocusHandler
    {
        [SerializeField] private UnityEvent onClick;
        [SerializeField] private bool invokeInteractable = true;
        
        [Header("Optional")]
        [Tooltip("This is the keyword the user can use to click the object without focus")]
        [SerializeField] private string clickKeyword = "";
        [SerializeField] private SpeechLabel label = SpeechLabel.Click;

        private bool isFocused;
        private BasicSpeechCommandListener listener;
        private Interactable interactable;
        
        protected UnityEvent OnClick => onClick;
        protected string ClickKeyword => clickKeyword;
        
        protected virtual void Start()
        {
            if (invokeInteractable)
            {
                interactable = GetComponent<Interactable>();
                if (interactable != null) onClick.AddListener(interactable.OnClick.Invoke);
            }
            
            listener = GetComponent<BasicSpeechCommandListener>();
            listener.EnableManualSetting();
            if (clickKeyword != "")
                listener.AddCommand(new BasicSpeechCommandEvent(label, clickKeyword, false, HandleClick));
            listener.AddCommand(new BasicSpeechCommandEvent(label, "", false, OnGenericClick));
        }
        
        protected void SwapKeyword(string newKeyword)
        {
            var lastWord = clickKeyword;
            clickKeyword = newKeyword;
            if (listener == null) return;
            
            listener.RemoveCommand(new BasicSpeechCommandEvent(label, lastWord, false, HandleClick));
            listener.AddCommand(new BasicSpeechCommandEvent(label, clickKeyword, false, HandleClick));
        }

        [ContextMenu("Find Label")]
        private void FindLabel()
        {
            var label = GetComponentInChildren<TMPro.TMP_Text>();
            if (label == null) label = transform.parent.GetComponentInChildren<TMPro.TMP_Text>();
            
            if (label != null)
            {
                clickKeyword = label.text.ToLower();
            }
        }

        private void OnGenericClick()
        {
            if (isFocused) HandleClick();
        }
        
        private void HandleClick() => onClick?.Invoke();
        public void OnFocusEnter(FocusEventData eventData) => isFocused = true;
        public void OnFocusExit(FocusEventData eventData) => isFocused = false;

        private void OnDisable()
        {
            isFocused = false;
        }
    }
}