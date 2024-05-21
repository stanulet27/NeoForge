using System;
using System.IO;

namespace DeformationSystem
{
    public static class PythonFinder
    {
        public static string FindPythonPath()
        {
            // Check the PATH environment variable for Python installations
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                string[] paths = pathEnv.Split(Path.PathSeparator);
                foreach (string path in paths)
                {
                    string pythonPath = Path.Combine(path, "python.exe");
                    if (File.Exists(pythonPath))
                    {
                        return pythonPath;
                    }
                }
            }

            // Check common installation paths for Python on Windows
            string[] commonPythonPaths = {
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

            foreach (string commonPath in commonPythonPaths)
            {
                if (File.Exists(commonPath))
                {
                    return commonPath;
                }
            }

            return null;
        }
    }
}