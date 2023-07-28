#nullable enable
using System;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class NullableEnabler
    {
        public static string nullableCode { get; } = "#nullable enable";

        [MenuItem("Runi Engine/Nullable Enable")]
        static void NullableEnableMenuButton()
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (string.IsNullOrWhiteSpace(path))
                return;
            else if (!EditorTool.PathIsProjectPath(path))
            {
                EditorUtility.DisplayDialog("Error", "To be safe, please select it as the project path", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Warning", $"Are you sure you want to enable nullable for all cs files in folder '{path}'?", "Yes", "No"))
                NullableEnable(path);
        }

        public static void NullableEnable(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] csFilePaths = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
                for (int i = 0; i < csFilePaths.Length; i++)
                {
                    string csFilePath = csFilePaths[i];
                    string text = File.ReadAllText(csFilePath).TrimStart();

                    if (!text.StartsWith(nullableCode))
                        File.WriteAllText(csFilePath, nullableCode + Environment.NewLine + text);
                }
            }
        }
    }
}
