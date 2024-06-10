using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NeoForge.Dialogue.Helper;
using UnityEngine;
using UnityEngine.UI;
using static NeoForge.Dialogue.Helper.DialogueVariables;

namespace NeoForge.Dialogue.UI
{
    public class DialogueBoxColorHandler : MonoBehaviour
    {
        [Tooltip("The dialogue box to change the color of")]
        [SerializeField] private Image _dialogueBox;
        [Tooltip("The colors for each character")]
        [SerializeField] private SerializedDictionary<string, Color> _colors;
        [Tooltip("The default color for the dialogue box when no match is found")]
        [SerializeField] private Color _defaultColor = Color.white;
        
        [ContextMenu("Create Color Dictionary")]
        private void EditorCreateColorDictionary()
        {
            _colors = new SerializedDictionary<string, Color>();
            Resources.LoadAll<ConversationDataSO>("Dialogue")
                    .SelectMany(x => x.Data.DialoguesSeries)
                    .SelectMany(y => y.dialogues)
                    .Select(z => z.SpeakerName)
                    .Distinct()
                    .ToList().ForEach(x => _colors.Add(x, _defaultColor));
        }
        
        private void OnEnable()
        {
            DialogueManager.OnTextSet += SetColor;
            DialogueManager.OnChoiceMenuOpen += SetColorForChoices;
            SetColor(DialogueManager.Instance.CurrentDialogue);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnTextSet -= SetColor;
            DialogueManager.OnChoiceMenuOpen -= SetColorForChoices;
        }
        
        private void SetColorForChoices(List<string> choices)
        {
            _dialogueBox.color = _colors.GetValueOrDefault(PLAYER_MARKER.Split(":")[0], _defaultColor);
        }
        
        private void SetColor(DialogueData dialogue)
        {
            if (dialogue == null) return;
            var characterName = dialogue.SpeakerName;
            _dialogueBox.color = _colors.GetValueOrDefault(characterName, _defaultColor);
        }
    }
}