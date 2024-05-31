#nullable enable
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class StreamingAssetsExporter
    {
        [MenuItem("Runi Engine/Streaming Assets Export (Sounds)")]
        public static void StreamingAssetsExport()
        {
            {
                string[] path = new string[]
                {
                    Path.Combine("Assets", Kernel.streamingAssetsFolderName, ResourceManager.rootName, AudioLoader.soundsNameSpace)
                };

                AssetDatabase.ExportPackage(path,
                    Path.Combine(EditorTool.packagePath + ".sounds", EditorTool.packageResourcesPath, Kernel.streamingAssetsFolderName + ".unitypackage"),
                    ExportPackageOptions.Recurse);
            }
        }
    }
}
