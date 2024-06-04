using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NeoForge.Utilities
{
    public static class PythonTools
    {
        public enum PythonFileType
        { 
            Script,
            Executable
        }
        
        // Check common installation paths for Python on Windows
        private static readonly string[] _commonPythonPaths = {
            @"C:\Python39\python.exe", // Python 3.9 default installation path
            @"C:\Python38\python.exe", // Python 3.8 default installation path
            @"C:\Python37\python.exe", // Python 3.7 default installation path
            @"C:\Users\" + Environment.UserName +
            @"\AppData\Local\Programs\Python\Python39\python.exe", // Python installed in user profile
            @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python38\python.exe",
            @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python37\python.exe",
            @"C:\Program Files\Python39\python.exe",
            @"C:\Program Files\Python38\python.exe",
            @"C:\Program Files\Python37\python.exe",
            @"C:\Program Files (x86)\Python39\python.exe",
            @"C:\Program Files (x86)\Python38\python.exe",
            @"C:\Program Files (x86)\Python37\python.exe"
        };

        /// <summary>
        /// Will execute a Python script or executable with the given arguments.
        /// </summary>
        /// <param name="pythonScriptPath">The path to the .py or python executable</param>
        /// <param name="args">The arguments to be given</param>
        /// <param name="fileType">Whether the user is giving a .py script or an executable</param>
        public static void CallPython(string pythonScriptPath, string[] args, PythonFileType fileType = PythonFileType.Script)
        {
            var arguments = fileType == PythonFileType.Script
                ? $"\"{pythonScriptPath}\"" + " " + string.Join(" ", args.Skip(1)).Trim()
                : string.Join(" ", args);
            
            var startInfo = new ProcessStartInfo
            {
                FileName = fileType == PythonFileType.Script ? FindPythonPath() : pythonScriptPath,
                Arguments = arguments,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Execute(startInfo);
        }
        
        private static string FindPythonPath()
        {
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            
            if (string.IsNullOrEmpty(pathEnv)) return _commonPythonPaths.FirstOrDefault(File.Exists);
            
            return pathEnv.Split(Path.PathSeparator)
                .Select(path => Path.Combine(path, "python.exe"))
                .FirstOrDefault(File.Exists) ?? _commonPythonPaths.FirstOrDefault(File.Exists);
        }
        
        private static void Execute(ProcessStartInfo startInfo)
        {
            using var process = Process.Start(startInfo);
            if (process == null) return;
            
            process.WaitForExit();

            var error = process.StandardError.ReadToEnd();
            UnityEngine.Debug.Log("Output: " + process.StandardOutput.ReadToEnd());
            UnityEngine.Debug.Assert(string.IsNullOrEmpty(error), $"Python error: {error}");
        }
    }
}