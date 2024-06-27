using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NeoForge.Dialogue;
using NeoForge.Dialogue.Helper;

public class ConversationDataHandler
{
    private const string FOLDER_PATH = "Assets/Resources/Dialogue";
    private ConversationData _backupConversationData;
    private NodeEditorUI _nodeEditorUI;

    public List<ConversationDataSO> ConversationDataSOList { get; private set; }
    
    public void SetNodeEditorUI(NodeEditorUI nodeEditorUI)
    {
        _nodeEditorUI = nodeEditorUI;
    }

    public void LoadExistingNodes()
    {
        ConversationDataSOList = new List<ConversationDataSO>();
        var guids = AssetDatabase.FindAssets("t:ConversationDataSO", new[] { FOLDER_PATH });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var conversationDataSO = AssetDatabase.LoadAssetAtPath<ConversationDataSO>(path);
            if (conversationDataSO != null)
            {
                ConversationDataSOList.Add(conversationDataSO);
            }
        }
    }

    public void CreateNewConversationData()
    {
        var newConversationDataSO = ScriptableObject.CreateInstance<ConversationDataSO>();
        var newConversationData = new ConversationData { ID = "emptyDialogue" };
        newConversationData.Variation = GetNextVariation(newConversationData);
        newConversationDataSO.SetConversation(newConversationData);

        if (!Directory.Exists(FOLDER_PATH))
        {
            Directory.CreateDirectory(FOLDER_PATH);
        }

        var assetPath = $"{FOLDER_PATH}/{newConversationDataSO.Data.ID}{newConversationDataSO.Data.Variation}.asset";
        AssetDatabase.CreateAsset(newConversationDataSO, assetPath);
        AssetDatabase.SaveAssets();

        ConversationDataSOList.Add(newConversationDataSO);
    }

    public void Save(int currentNodeIndex)
    {
        var conversationData = ConversationDataSOList[currentNodeIndex].Data;
        if (conversationData == null) return;

        // Apply temporary state changes and requirements before saving
        _nodeEditorUI.ApplyTemporaryStrings(currentNodeIndex);

        conversationData.Variation = GetNextVariation(conversationData);

        // Update the name of the ScriptableObject
        var newName = $"{conversationData.ID}{conversationData.Variation}";
        EditorUtility.SetDirty(ConversationDataSOList[currentNodeIndex]);
        AssetDatabase.SaveAssets();
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(ConversationDataSOList[currentNodeIndex]), newName);

        BackupCurrentData(currentNodeIndex); // Update backup with current data after saving
    }

    public void RevertChanges(int currentNodeIndex)
    {
        if (_backupConversationData != null)
        {
            var conversationData = ConversationDataSOList[currentNodeIndex].Data;
            conversationData.ID = _backupConversationData.ID;
            conversationData.Variation = _backupConversationData.Variation;
            conversationData.AudioCue = _backupConversationData.AudioCue;
            conversationData.StateChanges = new List<StateChange>(_backupConversationData.StateChanges);
            conversationData.StateRequirements = new List<StateRequirement>(_backupConversationData.StateRequirements);
            conversationData.DialoguesSeries = new List<DialogueChain>(_backupConversationData.DialoguesSeries.Select(dc => new DialogueChain { dialogues = new List<DialogueData>(dc.dialogues) }));
            conversationData.LeadsTo = new List<LeadsToPath>(_backupConversationData.LeadsTo);
        }
    }

    public void BackupCurrentData(int currentNodeIndex)
    {
        var conversationData = ConversationDataSOList[currentNodeIndex].Data;
        if (conversationData != null)
        {
            _backupConversationData = new ConversationData
            {
                ID = conversationData.ID,
                Variation = conversationData.Variation,
                AudioCue = conversationData.AudioCue,
                StateChanges = new List<StateChange>(conversationData.StateChanges),
                StateRequirements = new List<StateRequirement>(conversationData.StateRequirements),
                DialoguesSeries = new List<DialogueChain>(conversationData.DialoguesSeries.Select(dc => new DialogueChain { dialogues = new List<DialogueData>(dc.dialogues) })),
                LeadsTo = new List<LeadsToPath>(conversationData.LeadsTo)
            };
        }
        
        _nodeEditorUI.LoadTemporaryStrings(currentNodeIndex);
    }

    private string GetNextVariation(ConversationData data)
    {
        var maxVariation = 0;
        foreach (var dataSO in ConversationDataSOList)
        {
            var isMatch = dataSO.Data.ID == data.ID && dataSO.Data != data;
            var isIntVariation = int.TryParse(dataSO.Data.Variation, out var variation);
            if (isMatch && isIntVariation && variation > maxVariation)
            {
                maxVariation = variation;
            }
        }
        return (maxVariation + 1).ToString();
    }
}
