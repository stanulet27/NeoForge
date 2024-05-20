using System.Collections;
using System.Collections.Generic;
using MenuSystems.SpeechProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechProcessing
{
    /// <summary>
    /// When a new command is processed, this popup will appear to show the user what command was processed. After it
    /// pops up it will fade away after a set amount of time.
    /// </summary>
    public class CommandPopup : MonoBehaviour
    {
        [SerializeField] private GameObject display;
        [SerializeField] private TextMeshProUGUI commandTextField;
        [SerializeField] private float popupDuration = 1.5f;
        [SerializeField] private float popupFadeDuration = 1.0f;
        
        private readonly List<Image> imagesToFade = new();

        private void Start()
        {
            SpeechToCommandHandler.OnCommandReceived += OnCommandReceived;
            display.GetComponentsInChildren(true, imagesToFade);
            display.SetActive(false);
        }
        
        private void OnCommandReceived(SpeechCommand commandData)
        {
            commandTextField.text = commandData.ToString();
            StartCoroutine(DisplayCommandPopup());
        }
        
        private IEnumerator DisplayCommandPopup()
        {
            display.SetActive(true);
            SetColorAlphaTo(1.0f);
            
            yield return new WaitForSeconds(popupDuration);
            yield return FadeOut();
        }

        private IEnumerator FadeOut()
        {
            var remainingTime = popupFadeDuration;
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                remainingTime = Mathf.Max(0, remainingTime);
                var alpha = remainingTime / popupFadeDuration;
                SetColorAlphaTo(alpha);

                yield return null;
            }
            
            display.SetActive(false);
        }

        private void SetColorAlphaTo(float alpha)
        {
            imagesToFade.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, alpha));

            var temp = commandTextField.color;
            temp.a = alpha;
            commandTextField.color = temp;
        }

        private void OnDestroy()
        {
            SpeechToCommandHandler.OnCommandReceived -= OnCommandReceived;
        }
    }
}