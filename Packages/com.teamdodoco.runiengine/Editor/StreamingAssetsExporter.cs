#nullable enable
using RuniEngine.Resource;
using RuniEngine.Settings;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class StreamingAssetsExporter
    {
        [MenuItem("Runi Engine/Streaming Assets Export")]
        public static void StreamingAssetsExport()
        {
            {
                string[] path = new string[]
                {
                    Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, "runi"),
                    Path.Combine("Assets", Kernel.streamingAssetsFolderName, SettingManager.rootName, "runi-ui")
                };

                AssetDatabase.ExportPackage(path,
                    Path.Combine(EditorTool.packagePath, EditorTool.packageResourcesPath, Kernel.streamingAssetsFolderName + ".unitypackage"),
                    ExportPackageOptions.Recurse);
            }
        }
    }
}
