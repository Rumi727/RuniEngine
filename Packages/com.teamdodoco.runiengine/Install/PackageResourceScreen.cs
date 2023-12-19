#nullable enable
using RuniEngine.Editor;
using RuniEngine.Resource;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Install
{
    public sealed class PackageResourceScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label => EditorTool.TryGetText("installer.package_resource.label");
        public bool headDisable { get; } = false;
        public int sort { get; } = 3;

        public void DrawGUI()
        {
            PackageImportButton(EditorTool.TryGetText("gui.streaming_assets"), EditorTool.packagePath, Kernel.streamingAssetsFolderName, Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, "runi"));
            PackageImportButton(EditorTool.TryGetText("gui.streaming_assets") + " (NBS)", EditorTool.packagePath + ".nbs", Kernel.streamingAssetsFolderName, Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, "nbs"));
            PackageImportButton(EditorTool.TryGetText("gui.font"), EditorTool.packagePath, "Fonts", Path.Combine(EditorTool.assetsResourcePathParent, "Fonts"));
        }

        public void PackageImportButton(string title, string packagePath, string path, string existsPath)
        {
            string allPackagePath = Path.Combine(packagePath, EditorTool.packageResourcesPath, path + ".unitypackage");
            if (!File.Exists(allPackagePath))
            {
                EditorGUILayout.HelpBox(EditorTool.TryGetText("installer.package_resource.package_none").Replace("{name}", title), MessageType.Error);
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorTool.TryGetText("installer.package_resource.package").Replace("{name}", title), GUILayout.ExpandWidth(false));

            if (Directory.Exists(existsPath))
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.package_resource.import_done"), GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(allPackagePath, false);
                    GUILayout.EndHorizontal();

                    return;
                }
            }
            else
            {
                if (GUILayout.Button(EditorTool.TryGetText("installer.package_resource.import"), GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(allPackagePath, false);
                    GUILayout.EndHorizontal();

                    return;
                }
            }

            GUILayout.EndHorizontal();
            return;
        }
    }
}
