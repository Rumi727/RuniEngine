#nullable enable
using RuniEngine.Resource;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class StreamingAssetsExporter
    {
        [MenuItem("Runi Engine/Streaming Assets Export (NBS)")]
        public static void StreamingAssetsExport()
        {
            {
                string[] path = new string[]
                {
                    Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, "nbs")
                };

                AssetDatabase.ExportPackage(path,
                    Path.Combine(EditorTool.packagePath + ".nbs", EditorTool.packageResourcesPath, Kernel.streamingAssetsFolderName + ".unitypackage"),
                    ExportPackageOptions.Recurse);
            }
        }
    }
}
