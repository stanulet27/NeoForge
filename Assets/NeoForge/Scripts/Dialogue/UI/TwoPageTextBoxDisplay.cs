using TMPro;
using UnityEngine;

namespace NeoForge.Dialogue.UI
{
    public class TwoPageTextBoxDisplay : TextBoxDisplay
    {
        [Tooltip("The maximum number of characters that can be displayed on the first page of dialogue")]
        [SerializeField] private int _firstPageCharacterLimit;
        [Tooltip("The text field where the second page of dialogue will be displayed")]
        [SerializeField] private TMP_Text _dialogueTextField2;

        private int _splitPoint;
        
        public override void SetDialogueText(string speakerName, string dialogue)
        {
            _splitPoint = FindSlitPoint(dialogue);
            if (_splitPoint == 0)
            {
                base.SetDialogueText(speakerName, dialogue);
                _dialogueTextField2.maxVisibleCharacters = 0;
                _dialogueTextField2.text = "";
            }
            else
            {
                base.SetDialogueText(speakerName, dialogue[.._splitPoint]);
                _dialogueTextField2.maxVisibleCharacters = 0;
                _dialogueTextField2.text = dialogue[(_splitPoint + 1)..];
            }
        }
        
        public override void UpdateDialogueText(string text)
        {
            base.UpdateDialogueText(text);
            if (text.Length < _splitPoint && _splitPoint != 0) return;
            
            _dialogueTextField2.maxVisibleCharacters = text.Length - (_splitPoint + 1);
            _continueIndicator.SetActive(_continueIndicator.activeInHierarchy 
                                         && _dialogueTextField2.text != "" && _dialogueTextField2.maxVisibleCharacters 
                                         >= _dialogueTextField2.text.Length); 
        }

        /// <summary>
        /// Will find the point in the string where the dialogue should be split, this will be at the end of the
        /// last word that fits within the character limit.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int FindSlitPoint(string line)
        {
            if (line.Length <= _firstPageCharacterLimit) return 0;
            var splitPoint = _firstPageCharacterLimit;
            while (splitPoint > 0 && line[splitPoint] != ' ') splitPoint--;
            return splitPoint;
        }
    }
}