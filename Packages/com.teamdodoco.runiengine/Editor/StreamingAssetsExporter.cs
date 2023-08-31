#nullable enable
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class StreamingAssetsExporter
    {
        [MenuItem("Runi Engine/Streaming Assets Export")]
        public static void StreamingAssetsExport()
        {
            AssetDatabase.ExportPackage(Path.Combine("Assets", Kernel.streamingAssetsFolderName),
                Path.Combine(EditorTool.packageResourcesPath, Kernel.streamingAssetsFolderName + ".unitypackage"),
                ExportPackageOptions.Recurse);
        }
    }
}
