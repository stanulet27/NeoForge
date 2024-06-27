using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NeoForge.Dialogue;
using NeoForge.Dialogue.Helper;

public class NodeEditorUI
{
    private ConversationDataHandler _dataHandler;
    private string _tempStateChanges = "";
    private string _tempStateRequirements = "";
    private Vector2 _scrollPosition;

    public NodeEditorUI(ConversationDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public void DrawNodeEditor(int currentNodeIndex)
    {
        var conversationData = _dataHandler.ConversationDataSOList[currentNodeIndex].Data;
        
        if (conversationData == null)
        {
            GUILayout.Label("No node data found.");
            return;
        }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
        GUILayout.Label($"Editing Node {conversationData.ID} #{conversationData.Variation}");

        conversationData.ID = EditorGUILayout.TextField("Conversation ID", conversationData.ID);
        conversationData.AudioCue = EditorGUILayout.TextField("Audio Cue", conversationData.AudioCue);

        GUILayout.Label("State Changes (use one line per change)");
        _tempStateChanges = EditorGUILayout.TextArea(_tempStateChanges);

        GUILayout.Label("State Requirements (use one line per requirement)");
        _tempStateRequirements = EditorGUILayout.TextArea(_tempStateRequirements);

        GUILayout.Label("Dialogue Chains");
        conversationData.DialoguesSeries ??= new List<DialogueChain>();
        foreach (var chain in conversationData.DialoguesSeries)
        {
            GUILayout.BeginVertical("box");
            DrawDialogueChain(chain);
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Add Dialogue Chain"))
        {
            conversationData.DialoguesSeries.Add(new DialogueChain());
        }

        GUILayout.Label("Leads To");
        conversationData.LeadsTo ??= new List<LeadsToPath>();

        for (int i = 0; i < conversationData.LeadsTo.Count; i++)
        {
            var path = conversationData.LeadsTo[i];
            path.Prompt = EditorGUILayout.TextField("Prompt", path.Prompt);
            path.NextID = EditorGUILayout.TextField("Next ID", path.NextID);
            
            GUILayout.BeginHorizontal();
            path.IsEvent = EditorGUILayout.Toggle("Is Event", path.IsEvent);
            
            conversationData.LeadsTo[i] = path;
            
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                conversationData.LeadsTo.RemoveAt(i);
                i--; // Adjust index after removal
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Leads To Path"))
        {
            conversationData.LeadsTo.Add(new LeadsToPath("", "", false));
        }

        GUILayout.Space(20);
        
        GUILayout.EndScrollView();
    }

    private void DrawDialogueChain(DialogueChain dialogueChain)
    {
        dialogueChain.dialogues ??= new List<DialogueData>();

        for (int i = 0; i < dialogueChain.dialogues.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box");
            DrawDialogueData(dialogueChain.dialogues[i]);
            GUILayout.EndVertical();
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                dialogueChain.dialogues.RemoveAt(i);
                i--; // Adjust index after removal
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Dialogue Line"))
        {
            dialogueChain.dialogues.Add(new DialogueData());
        }
    }

    private void DrawDialogueData(DialogueData dialogueData)
    {
        dialogueData.Speaker = (ConversantType)EditorGUILayout.EnumPopup("Speaker", dialogueData.Speaker);
        if (dialogueData.Speaker == ConversantType.Conversant)
        {
            dialogueData.SpeakerName = EditorGUILayout.TextField("Speaker Name", dialogueData.SpeakerName);
        }

        var textAreaStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = true
        };
        dialogueData.Dialogue = EditorGUILayout.TextArea(dialogueData.Dialogue, textAreaStyle);
    }
    
    public void LoadTemporaryStrings(int currentNodeIndex)
    {
        var conversationData = _dataHandler.ConversationDataSOList[currentNodeIndex].Data;
        if (conversationData != null)
        {
            _tempStateChanges = string.Join("\n", conversationData.StateChanges.Select(sc => sc.Change));
            _tempStateRequirements = string.Join("\n", conversationData.StateRequirements.Select(sr => sr.Requirement));
        }
    }

    public void ApplyTemporaryStrings(int currentNodeIndex)
    {
        var conversationData = _dataHandler.ConversationDataSOList[currentNodeIndex].Data;
        if (conversationData != null)
        {
            conversationData.StateChanges = _tempStateChanges.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(line => new StateChange(line)).ToList();
            conversationData.StateRequirements = _tempStateRequirements.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(line => new StateRequirement(line)).ToList();
        }
    }
}
