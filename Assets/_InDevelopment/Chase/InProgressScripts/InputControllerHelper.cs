using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeoForge.UI.Tools
{
    public class InputControllerHelper : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        [SerializeField] TouchScreenKeyboardType _keyboardType;
        [SerializeField] TMP_InputField _inputField;

        private void Reset()
        {
            _inputField = GetComponent<TMP_InputField>();
        }
 
        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(UnFocusByDefault());
        }

        private IEnumerator UnFocusByDefault()
        {
            yield return new WaitForEndOfFrame();
            _inputField.DeactivateInputField();
        }
 
        public void OnSubmit(BaseEventData eventData)
        {
            TouchScreenKeyboard.Open(_inputField.text, _keyboardType);
        }
    }
}