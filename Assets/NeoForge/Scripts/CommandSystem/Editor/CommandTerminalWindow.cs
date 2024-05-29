/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections.Generic;
using System.Text.RegularExpressions;
using MenuSystems.SpeechProcessing.Editor;
using UnityEditor;
using UnityEngine;

namespace CommandSystem.Editor
{
    /// <summary>
    /// Adds a new window to unity to be able to invoke Speech Commands from the editor. It is in the style of a
    /// command terminal but will only execute commands that are present within the SpeechToCommandHandler.cs. Essentially
    /// it acts as a stand in for voice commands since those can't be given when using HoloRemoting
    /// </summary>
    public class CommandTerminalWindow : EditorWindow
    {
        public static string CommandLineArguments
        {
            get => EditorPrefs.GetString(nameof(CommandLineArguments), "");
            set => EditorPrefs.SetString(nameof(CommandLineArguments), value);
        }

        private string _commandTerminalExecutionLog = "";
        private bool _isPlayingTracker;
        private Vector2 _currentScrollPosition;
        private CommandHandler _commandHandler;
        private static Dictionary<string, string> _commandShortCuts = new ()
            {
                {"#setup", "open ros => download => open thickness setter => create"}
            };

        private Regex _multiplesPattern = new("x(\\d+)");
        
        
        [MenuItem("Tools/Command Terminal")]
        public static void ShowWindow()
        {
            GetWindow<CommandTerminalWindow>(false, "Command Terminal", true);
        }

        private static void DisplayClearButton()
        {
            if (GUILayout.Button("Clear"))
            {
                CommandLineArguments = "";
            }
        }

        private void OnGUI()
        {
            UpdateIsPlayingTracker();
            DisplayGUI();
        }

        private void UpdateIsPlayingTracker()
        {
            if (_isPlayingTracker == Application.isPlaying) return;
            
            _isPlayingTracker = Application.isPlaying;
            _commandHandler = _isPlayingTracker ? FindObjectOfType<CommandHandler>() : null;
            _commandTerminalExecutionLog = _isPlayingTracker ? "" : "Can only execute commands in play mode!";
        }
        
        private void DisplayGUI()
        {
            DisplayExecutionLog();
            DisplayCommandLineArguments();
            DisplaySubmitButton();
            DisplayOptionsButton();
            DisplayClearButton();
        }

        private void DisplayExecutionLog()
        {

            EditorGUILayout.LabelField("Command Terminal Execution Log");
            _currentScrollPosition = EditorGUILayout.BeginScrollView(_currentScrollPosition, GUILayout.Height(100));
            GUI.enabled = false;
            EditorGUILayout.TextArea(_commandTerminalExecutionLog, GUILayout.ExpandHeight(true));
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();

        }
        
        private void DisplayCommandLineArguments()
        {
            const string label = "Command Line Arguments";
            GUI.SetNextControlName(label);

            CommandLineArguments =
                EditorGUILayout.TextField(label, CommandLineArguments)
                    .KeyPressed(label, KeyCode.Return, out bool enterWasPressed);
            
            if(enterWasPressed && _isPlayingTracker)
            {
                ActivateCommand();
                CommandLineArguments = "";
                EditorGUI.FocusTextInControl(label);
                Repaint();
            }
        }

        private void ActivateCommand()
        {
            if (_commandHandler == null) _commandHandler = FindObjectOfType<CommandHandler>();
            if(CommandLineArguments == "") return;

            if(_commandShortCuts.ContainsKey(CommandLineArguments.Trim())) 
                CommandLineArguments = _commandShortCuts[CommandLineArguments.Trim()];

            _commandTerminalExecutionLog = "";
            foreach (var command in CommandLineArguments.Split("=>"))
            {
                var multiples = _multiplesPattern.IsMatch(command) ? int.Parse(_multiplesPattern.Split(command)[1]) : 1;
                
                for (int i = 0; i < multiples; i++)
                {
                    _commandHandler.TestCommandConverter(command);
                    _commandTerminalExecutionLog += _commandHandler.ExecutionLog + "\n";
                }
            }
        }

        private void DisplaySubmitButton()
        {
            GUI.enabled = _isPlayingTracker;

            if (GUILayout.Button("Submit"))
            {
                ActivateCommand();
            }

            GUI.enabled = true;
        }

        private void DisplayOptionsButton()
        {
            GUI.enabled = _isPlayingTracker;

            if (GUILayout.Button("Display Options"))
            {
                _commandTerminalExecutionLog = _commandHandler.ValidCommands;
            }
        }
    }
}
