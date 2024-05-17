#nullable enable
using RuniEngine.Resource;
using System.IO;
using UnityEditor;
using UnityEngine;

using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Install
{
    public sealed class PackageResourceScreen : IInstallerScreen
    {
        public InstallerMainWindow? installerMainWindow { get; set; }

        public string label => TryGetText("installer.package_resource.label");
        public bool headDisable { get; } = false;
        public int sort { get; } = 3;

        public void DrawGUI()
        {
            PackageImportButton(TryGetText("gui.streaming_assets"), packagePath, Kernel.streamingAssetsFolderName, Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, "runi"));
            PackageImportButton(TryGetText("gui.streaming_assets") + " (NBS)", packagePath + ".nbs", Kernel.streamingAssetsFolderName, Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, "runi-nbs"));
            PackageImportButton(TryGetText("gui.font"), packagePath, "Fonts", Path.Combine(assetsResourcePathParent, "Fonts"));
        }

        public void PackageImportButton(string title, string packagePath, string path, string existsPath)
        {
            string allPackagePath = Path.Combine(packagePath, packageResourcesPath, path + ".unitypackage");
            if (!File.Exists(allPackagePath))
            {
                EditorGUILayout.HelpBox(TryGetText("installer.package_resource.package_none").Replace("{name}", title), MessageType.Error);
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(TryGetText("installer.package_resource.package").Replace("{name}", title), GUILayout.ExpandWidth(false));

            if (Directory.Exists(existsPath))
            {
                if (GUILayout.Button(TryGetText("installer.package_resource.import_done"), GUILayout.ExpandWidth(false)))
                {
                    AssetDatabase.ImportPackage(allPackagePath, false);
                    GUILayout.EndHorizontal();

                    return;
                }
            }
            else
            {
                if (GUILayout.Button(TryGetText("installer.package_resource.import"), GUILayout.ExpandWidth(false)))
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
