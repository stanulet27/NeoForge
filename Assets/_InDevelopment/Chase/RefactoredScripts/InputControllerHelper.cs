using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeoForge.UI.Tools
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputControllerHelper : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        /// <summary>
        /// Will be invoked when the object is selected.
        /// </summary>
        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(UnFocusByDefault());
        }

        private IEnumerator UnFocusByDefault()
        {
            yield return new WaitForEndOfFrame();
            _inputField.DeactivateInputField();
        }
 
        /// <summary>
        /// Will be invoked when the submit button is pressed while the object is selected.
        /// </summary>
        public void OnSubmit(BaseEventData eventData)
        {
            TouchScreenKeyboard.Open(_inputField.text, _inputField.keyboardType);
        }
    }
}