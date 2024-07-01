using System.Linq;
using NeoForge.Dialogue;
using NeoForge.Dialogue.Editor;
using UnityEditor;
using UnityEngine;

public class MainMenuUI
{
    private ConversationDataHandler _dataHandler;
    private string _searchQuery = "";
    private SearchType _searchType = SearchType.ID;
    private Vector2 _scrollPosition;

    public MainMenuUI(ConversationDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public void DrawMainMenu(ref int currentNodeIndex)
    {
        DrawSearchBar();

        GUILayout.Label("Conversation Nodes:");
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
        if (GUILayout.Button("Validate Nodes"))
        {
            DialogueValidator.ValidateAllConversations(_dataHandler.ConversationDataSOList);
        }
        
        foreach (var dataSO in _dataHandler.ConversationDataSOList)
        {
            var displayName = $"{dataSO.Data.ID} #{dataSO.Data.Variation}";
            if (!string.IsNullOrEmpty(_searchQuery) && !IsMatch(dataSO))
            {
                continue;
            }

            if (GUILayout.Button(displayName))
            {
                currentNodeIndex = _dataHandler.ConversationDataSOList.IndexOf(dataSO);
                _dataHandler.BackupCurrentData(currentNodeIndex);
            }
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Add New Node"))
        {
            _dataHandler.CreateNewConversationData();
        }
    }

    private void DrawSearchBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.Width(50));
        _searchQuery = GUILayout.TextField(_searchQuery);
        GUILayout.Label("Type: ", GUILayout.Width(35));
        _searchType = (SearchType)EditorGUILayout.EnumPopup(_searchType, GUILayout.Width(100));
        GUILayout.EndHorizontal();
    }

    private bool IsMatch(ConversationDataSO dataSO)
    {
        var lowerQuery = _searchQuery.ToLower();
        switch (_searchType)
        {
            case SearchType.ID:
                return dataSO.Data.ID.ToLower().Contains(lowerQuery);
            case SearchType.Changes:
                return dataSO.Data.StateChanges.Any(change => change.State.ToLower().Contains(lowerQuery));
            case SearchType.Requirements:
                return dataSO.Data.StateRequirements.Any(requirement => requirement.State.ToLower().Contains(lowerQuery));
            case SearchType.LeadsTo:
                return dataSO.Data.LeadsTo.Any(leadsTo => leadsTo.NextID.ToLower().Contains(lowerQuery));
            default:
                return false;
        }
    }

    private enum SearchType
    {
        ID,
        Changes,
        Requirements,
        LeadsTo
    }
}