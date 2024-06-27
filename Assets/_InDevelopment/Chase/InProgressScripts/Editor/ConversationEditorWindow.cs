using UnityEditor;
using UnityEngine;

public class ConversationEditorWindow : EditorWindow
{
    private ConversationDataHandler _dataHandler;
    private MainMenuUI _mainMenuUI;
    private NodeEditorUI _nodeEditorUI;
    private Validator _validator;
    
    private int _currentNodeIndex = -1; // -1 indicates the main menu

    [MenuItem("Tools/Conversation Editor")]
    public static void ShowWindow()
    {
        GetWindow<ConversationEditorWindow>("Conversation Editor");
    }

    private void OnEnable()
    {
        _dataHandler = new ConversationDataHandler();
        _mainMenuUI = new MainMenuUI(_dataHandler);
        _nodeEditorUI = new NodeEditorUI(_dataHandler);
        _validator = new Validator(_dataHandler);
        
        _dataHandler.SetNodeEditorUI(_nodeEditorUI);
        _dataHandler.LoadExistingNodes();
    }

    private void OnGUI()
    {
        if (_dataHandler.ConversationDataSOList == null || _dataHandler.ConversationDataSOList.Count == 0)
        {
            if (GUILayout.Button("Create New Conversation Data"))
            {
                _dataHandler.CreateNewConversationData();
            }
            return;
        }

        if (_currentNodeIndex == -1)
        {
            _mainMenuUI.DrawMainMenu(ref _currentNodeIndex);
        }
        else
        {
            _nodeEditorUI.DrawNodeEditor(_currentNodeIndex);

            if (GUILayout.Button("Back to Main Menu"))
            {
                GUI.FocusControl(null); // Remove the selection of the current text field
                _dataHandler.RevertChanges(_currentNodeIndex);
                _currentNodeIndex = -1;
            }

            if (GUILayout.Button("Save"))
            {
                if (_validator.ValidateLeadsTo(_currentNodeIndex))
                {
                    GUI.FocusControl(null); // Remove the selection of the current text field
                    _dataHandler.Save(_currentNodeIndex);
                }
                else
                {
                    EditorUtility.DisplayDialog("Validation Error", "One or more 'Leads To' entries point to invalid conversation IDs.", "OK");
                }
            }
        }
    }
}