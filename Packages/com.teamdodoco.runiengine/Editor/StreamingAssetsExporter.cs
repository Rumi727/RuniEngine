#nullable enable
using RuniEngine.Resource;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class StreamingAssetsExporter
    {
        [MenuItem("Runi Engine/Streaming Assets Export")]
        public static void StreamingAssetsExport()
        {
            string[] path = new string[]
            {
                Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, "runi"),
                Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, "minecraft"),
                Path.Combine("Assets", Kernel.streamingAssetsFolderName, "projectData")
            };

            AssetDatabase.ExportPackage(path,
                Path.Combine(EditorTool.packageResourcesPath, Kernel.streamingAssetsFolderName + ".unitypackage"),
                ExportPackageOptions.Recurse);
        }
    }
}
