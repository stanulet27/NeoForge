/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.IO;
using UnityEngine;
using TMPro;

namespace Utilities
{
    /// <summary>
    /// Handles displaying a console to the user that shows all of the debug logs.
    /// </summary>
    public class DebugDisplayHandler : MonoBehaviour
    {
        private const string EXTENSION = ".txt";

#if UNITY_EDITOR
        private static string _saveFolderPath => $"{Application.dataPath}//Debug//";
#else
        private static string _saveFolderPath => $"{Application.persistentDataPath}//Debug//";
#endif
        private static string _filePath => _saveFolderPath + _saveName;
        private static string _saveName => $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year} {_timeStamp}" + EXTENSION;
        
        private static string _timeStamp => $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
        
        [SerializeField] private TMP_Text _textfield;

        private string _log = "";

        private void Awake()
        {
            Application.logMessageReceived += UpdateLog;
        }

        /// <summary>
        /// Will update the log file saved to the users file system.
        /// </summary>
        public void UpdateLogFile()
        {
            if (!Directory.Exists(_saveFolderPath))
            {
                Directory.CreateDirectory(_saveFolderPath);
            }
            File.WriteAllText(_filePath, _log);
        }

        private void UpdateLog(string logString, string stackTrace, LogType type)
        {
            _textfield.text = _textfield.text + "\n" + logString;
            _log = _timeStamp + " " + type + " " + logString + "\n" + stackTrace + "\n\n" + _log;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= UpdateLog;
            UpdateLogFile();
        }
    }
}