﻿/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

namespace NeoForge.Utilities
{
    /// <summary>
    /// Handles displaying a console to the user that shows all of the debug logs.
    /// </summary>
    public class DebugDisplayHandler : MonoBehaviour
    {
        private const string EXTENSION = ".txt";

#if UNITY_EDITOR
        private static string SaveFolderPath => $"{Application.dataPath}//Debug//";
#else
        private static string SaveFolderPath => $"{Application.persistentDataPath}//Debug//";
#endif
        private static string FilePath => SaveFolderPath + SaveName;
        private static string SaveName => $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year} {TimeStamp}" + EXTENSION;
        
        private static string TimeStamp => $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
        
        [SerializeField] private TMP_Text _textfield;

        private string _log = "";

        private void Awake()
        {
            Application.logMessageReceived += UpdateLog;
        }

        /// <summary>
        /// Will update the log file saved to the users file system.
        /// </summary>
        private void UpdateLogFile()
        {
            if (string.IsNullOrWhiteSpace(_log)) return;
            
            if (!Directory.Exists(SaveFolderPath))
            {
                Directory.CreateDirectory(SaveFolderPath);
            }
            
            var files = Directory.GetFiles(SaveFolderPath).OrderByDescending(f => f).ToList();
            while (files.Count > 18)
            {
                File.Delete(files[^1]);
                files.RemoveAt(files.Count - 1);
            }
            
            File.WriteAllText(FilePath, _log);
        }

        private void UpdateLog(string logString, string stackTrace, LogType type)
        {
            _textfield.text = _textfield.text + "\n" + logString;
            _log = TimeStamp + " " + type + " " + logString + "\n" + stackTrace + "\n\n" + _log;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= UpdateLog;
            UpdateLogFile();
        }
    }
}