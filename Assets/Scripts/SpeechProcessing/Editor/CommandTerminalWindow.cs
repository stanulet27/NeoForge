/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MenuSystems.SpeechProcessing.Editor;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;

namespace SpeechProcessing.Editor
{
    /// <summary>
    /// Adds a new window to unity to be able to invoke Speech Commands from the editor. It is in the style of a
    /// command terminal but will only execute commands that are present within the SpeechToCommandHandler.cs. Essentially
    /// it acts as a stand in for voice commands since those can't be given when using HoloRemoting
    /// </summary>
    public class CommandTerminalWindow : EditorWindow
    {
        private string commandTerminalExecutionLog = "";
        private bool isPlayingTracker;
        private Vector2 currentScrollPosition;
        private SpeechToCommandHandler commandHandler;
        private static Dictionary<string, string> commandShortCuts = new ()
            {
                {"#setup", "open ros => download => open thickness setter => create"}
            };

        private Regex multiplesPattern = new("x(\\d+)");
        
        public static string CommandLineArguments
        {
            get => EditorPrefs.GetString(nameof(CommandLineArguments), "");
            set => EditorPrefs.SetString(nameof(CommandLineArguments), value);
        }
        
        [MenuItem("Tools/Command Terminal")]
        public static void ShowWindow()
        {
            GetWindow<CommandTerminalWindow>(false, "Command Terminal", true);
        }

        private void OnGUI()
        {
            UpdateIsPlayingTracker();
            DisplayGUI();
        }

        private void UpdateIsPlayingTracker()
        {
            if (isPlayingTracker == Application.isPlaying) return;
            
            isPlayingTracker = Application.isPlaying;
            commandHandler = isPlayingTracker ? FindObjectOfType<SpeechToCommandHandler>() : null;
            commandTerminalExecutionLog = isPlayingTracker ? "" : "Can only execute commands in play mode!";
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
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition, GUILayout.Height(100));
            GUI.enabled = false;
            EditorGUILayout.TextArea(commandTerminalExecutionLog, GUILayout.ExpandHeight(true));
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
            
            if(enterWasPressed && isPlayingTracker)
            {
                ActivateCommand();
                CommandLineArguments = "";
                EditorGUI.FocusTextInControl(label);
                Repaint();
            }
        }

        private void ActivateCommand()
        {
            if (commandHandler == null) commandHandler = FindObjectOfType<SpeechToCommandHandler>();
            if(CommandLineArguments == "") return;

            if(commandShortCuts.ContainsKey(CommandLineArguments.Trim())) 
                CommandLineArguments = commandShortCuts[CommandLineArguments.Trim()];

            commandTerminalExecutionLog = "";
            foreach (var command in CommandLineArguments.Split("=>"))
            {
                var multiples = multiplesPattern.IsMatch(command) ? int.Parse(multiplesPattern.Split(command)[1]) : 1;
                
                for (int i = 0; i < multiples; i++)
                {
                    commandHandler.TestCommandConverter(command);
                    commandTerminalExecutionLog += commandHandler.ExecutionLog + "\n";
                }
            }
        }

        private void DisplaySubmitButton()
        {
            GUI.enabled = isPlayingTracker;

            if (GUILayout.Button("Submit"))
            {
                ActivateCommand();
            }

            GUI.enabled = true;
        }

        private static void DisplayClearButton()
        {
            if (GUILayout.Button("Clear"))
            {
                CommandLineArguments = "";
            }
        }

        private void DisplayOptionsButton()
        {
            GUI.enabled = isPlayingTracker;

            if (GUILayout.Button("Display Options"))
            {
                commandTerminalExecutionLog = commandHandler.ValidCommands;
            }
        }
    }
}
