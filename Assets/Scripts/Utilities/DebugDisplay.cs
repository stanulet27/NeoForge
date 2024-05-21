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
 using TMPro;
 using UnityEngine;

 namespace Utilities
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
        
        [SerializeField] private TextMeshProUGUI textfield;

        private string log = "";

        private void Awake()
        {
            Application.logMessageReceived += UpdateLog;
        }
        
        private void UpdateLog(string logString, string stackTrace, LogType type)
        {
            textfield.text = logString + "\n" + textfield.text;
            log = TimeStamp + " " + type + " " + logString + "\n" + stackTrace + "\n\n" + log;
        }

        /// <summary>
        /// Will update the log file saved to the users file system.
        /// </summary>
        public void UpdateLogFile()
        {
            if (!Directory.Exists(SaveFolderPath))
                Directory.CreateDirectory(SaveFolderPath);
            
            File.WriteAllText(FilePath, log);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= UpdateLog;
            UpdateLogFile();
        }
    }
}