using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue.Helper;
using NeoForge.Input;
using NeoForge.UI.Scenes;
using NeoForge.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace NeoForge.Dialogue
{
    public class DialogueManager : SingletonMonoBehaviour<DialogueManager>
    {
        public static Action<ConversationData> OnDialogueStarted;
        public static Action OnDialogueEnded;
        public static Action<DialogueData> OnTextSet;
        public static Action<string> OnTextUpdated;
        public static Action<List<string>> OnChoiceMenuOpen;
        public static Action OnChoiceMenuClose;
        public static Action<string> OnAudioCue;
        
        // Triggered when an event dialogue is reached, will exit the dialogue first then trigger the event
        public static Action<string> OnEventTriggered;

        [Tooltip("Chars/Second")]
        [SerializeField] float _dialogueSpeed;
        [Tooltip("Chars/Second")]
        [SerializeField] float _dialogueFastSpeed;
        [Tooltip("Loaded in from resources"), ReadOnly]
        [SerializeField] List<ConversationDataSO> _conversationGroup;

        private readonly Dictionary<string, int> _dialogueProgress = new();
    
        private float _currentDialogueSpeed;
        private bool _inDialogue;
        private bool _continueInputReceived;
        private bool _abortDialogue;
        private int _choiceSelected;
        private DialogueData _currentDialogue;
        public bool InDialogue => _inDialogue;
        public bool InInternalDialogue { get; private set; }

        public bool ValidateID(string id) => _conversationGroup.Find(data => data.Data.ID.ToLower().Equals(id.ToLower()));
        private int _playersReady;

        public DialogueData CurrentDialogue => _currentDialogue;
        
        [ContextMenu("Display World State")]
        private void DisplayWorldState() => Debug.Log(WorldState.GetCurrentWorldState());
        private void SetWorldState(string key, int value) => WorldState.SetState(key, _ => value);
        [ContextMenu("Clear World State")]
        private void ClearWorldState() => WorldState.ClearAllStates();

        [Button]
        private void DisplayConversants() =>
            Resources.LoadAll<ConversationDataSO>("Dialogue")
                .SelectMany(x => x.Data.DialoguesSeries)
                .SelectMany(y => y.dialogues)
                .Select(z => z.SpeakerName)
                .Distinct()
                .ToList().ForEach(Debug.Log);

        protected override void Awake()
        {
            base.Awake();
            _conversationGroup = Resources.LoadAll<ConversationDataSO>("Dialogue").ToList();
            _conversationGroup.Sort((x, y) => x.Data.StateRequirements.Count > y.Data.StateRequirements.Count ? -1 : 1);
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += DestroyOnStart;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= DestroyOnStart;
        }
        
        private void DestroyOnStart(Scene _, Scene __)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                _dialogueProgress.Clear();
                WorldState.ClearAllStates();
            }
        }

        /// <summary>
        /// Will start a new dialogue conversation using the conversation of the specified name that also is currently
        /// valid based on the world state.
        /// </summary>
        /// <param name="dialogueId">Name of the conversation</param>
        [Button]
        public void StartDialogueName(string dialogueId)
        {
            if (_inDialogue) return;
            
            _inDialogue = true;
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            
            AdvanceDialogue(dialogueId);
            StartDialogue(dialogueId);
        }
        
        /// <summary>
        /// Will continue the dialogue using the choice selected
        /// </summary>
        /// <param name="choice">The index of the choice selected</param>
        public void SelectChoice(int choice) => _choiceSelected = choice;

        private void StartDialogue(string dialogueId)
        {
            if (dialogueId == null || dialogueId.ToLowerInvariant().Equals("exit"))
            {
                ExitDialogue();
                return;
            }

            var conversationDataPointer = _conversationGroup.Find(data => data.Data.ID.ToLower().Equals(dialogueId.ToLower()) && CheckStateRequirements(data.Data));
            if (conversationDataPointer == null)
            {
                Debug.Log("Could not find " + dialogueId + " in database with valid condition");
                ExitDialogue();
                return;
            }

            Debug.Log("Starting conversation: " + conversationDataPointer.Data.ID);
            StartCoroutine(HandleConversation(conversationDataPointer.Data));
        }

        private void ExitDialogue()
        {
            _inDialogue = false;
            OnDialogueEnded?.Invoke();
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
        }
        
        private void AdvanceDialogue(string data)
        { 
            if (_dialogueProgress.ContainsKey(data))
            {
                _dialogueProgress[data]++;
            }
            else
            {
                _dialogueProgress.Add(data, 0);
            }
        }

        private void OnAbort() 
        {
            _abortDialogue = true;
            OnContinueInput();
        }

        private IEnumerator HandleConversation(ConversationData data)
        {
            OnDialogueStarted?.Invoke(data);
            if (data.DialoguesSeries.Count > 0) yield return DisplayDialogue(data);
            UpdateWorldState(data);
            yield return ProceedToNextDialogue(data);
        }

        private IEnumerator ProceedToNextDialogue(ConversationData data)
        {
            yield return AwaitChoice(data);
            var nextDialogue = _choiceSelected == -1 ? "end" : 
                data.LeadsTo.Where(x => CheckStateRequirements(x.NextID)).ToList()[_choiceSelected].NextID;
            
            if (nextDialogue.ToLower().Equals("end"))
            {
                Debug.Log("Exiting dialogue");
                ExitDialogue();
            }
            else if (data.LeadsTo.Where(x => CheckStateRequirements(x.NextID)).ToList()[_choiceSelected].IsEvent)
            {
                ExitDialogue();
                OnEventTriggered?.Invoke(nextDialogue);
                Debug.Log("Firing event: " + nextDialogue);
            }
            else
                StartDialogue(nextDialogue);
        }

        private static void UpdateWorldState(ConversationData data)
        {
            foreach (var change in data.StateChanges)
            {
                WorldState.SetState(change.State, change.Modifier);
                Debug.Log("Updating " + change.State + " to " + WorldState.GetState(change.State));
            }
        }

        private bool CheckStateRequirements(string dialogueID)
        {
            var data = _conversationGroup.Where(x => x.Data.ID == dialogueID).ToList();
            if (!data.Any()) return true;

            return data
                .Select(x => x.Data.StateRequirements)
                .Any(y => y.All(requirement => requirement.IsMet(WorldState.GetState(requirement.State))));
        }

        private bool CheckStateRequirements(ConversationData data)
        {
            return data.StateRequirements.All(requirement => requirement.IsMet(WorldState.GetState(requirement.State)));
        }
        
        private IEnumerator AwaitChoice(ConversationData data)
        {
            _choiceSelected = -1;
            if (!data.HasChoice)
            {
                _choiceSelected = data.LeadsTo.FindIndex(x => CheckStateRequirements(x.NextID));
            }
            else
            {
                var choices = data.LeadsTo.Where(x => CheckStateRequirements(x.NextID)).ToList();
                OnChoiceMenuOpen?.Invoke(choices.Select(x => x.Prompt).ToList());
                yield return new WaitUntil(() => _choiceSelected != -1);
                OnChoiceMenuClose?.Invoke();
            }
        }

        private IEnumerator DisplayDialogue(ConversationData data)
        {
            _abortDialogue = false;
            ControllerManager.OnSkipDialogue += OnAbort;
            if (!string.IsNullOrEmpty(data.AudioCue))
            {
                OnAudioCue?.Invoke(data.AudioCue);
            }
            
            var dialogueIndex = _dialogueProgress.TryGetValue(data.ID, out var progress)
                ? Mathf.Min(progress, data.DialoguesSeries.Count - 1)
                : 0;
            var dialogues = data.DialoguesSeries[dialogueIndex].dialogues;

            foreach (var dialogue in dialogues.TakeWhile(_ => !_abortDialogue))
            {
                yield return ProcessDialogue(dialogue);
            }

            ControllerManager.OnSkipDialogue -= OnAbort;
        }

        private void OnContinueInput() => _continueInputReceived = true;

        private IEnumerator ProcessDialogue(DialogueData dialogue)
        {
            var speakerName = dialogue.SpeakerName;
            
            _currentDialogue = dialogue;
            OnTextSet?.Invoke(dialogue);
            OnTextUpdated?.Invoke("");
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);

            _continueInputReceived = false;

            ControllerManager.OnNextDialogue += SpeedUpText;
            yield return TypewriterDialogue(speakerName, dialogue);
            ControllerManager.OnNextDialogue -= SpeedUpText;
            
            ControllerManager.OnNextDialogue += OnContinueInput;
            yield return new WaitUntil(() => _continueInputReceived);
            ControllerManager.OnNextDialogue -= OnContinueInput;
        }

        private IEnumerator TypewriterDialogue(string name, DialogueData dialogue)
        {
            _currentDialogueSpeed = _dialogueSpeed;
            var loadedText = "";
            var atSpecialCharacter = false;
            var line = dialogue.Dialogue;
            
            foreach (var letter in line)
            {
                loadedText += letter;
                atSpecialCharacter = letter == '<' || atSpecialCharacter;
                if (atSpecialCharacter && letter != '>') continue;
                atSpecialCharacter = false;
                OnTextUpdated?.Invoke(loadedText);
                yield return new WaitForSeconds(1 / _currentDialogueSpeed);

                if (_abortDialogue)
                {
                    OnTextUpdated?.Invoke(name + line);
                    break;
                }
            }
            
            _playersReady++;
        }

        private void SpeedUpText() => _currentDialogueSpeed = Math.Abs(_currentDialogueSpeed - _dialogueFastSpeed) < Mathf.Epsilon 
            ? _dialogueFastSpeed * 10 : _dialogueFastSpeed;
    }
}
