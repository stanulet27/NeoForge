using UnityEditor;
using System.IO;
using System.Linq;

public class BuildScript
{
    public static void PerformBuild()
    {
        // Define the path to the Scenes folder
        string projectScenesPath = "Assets/NeoForge/Scenes";
        
        // Find all scene files in the defined folder
        string[] scenes = Directory.GetFiles(projectScenesPath, "*.unity", SearchOption.AllDirectories);

        // Define the build target and output path
        string buildPath = "Build/StandaloneWindows64/YourGame.exe";

        // Perform the build
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
}