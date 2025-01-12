#nullable enable
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor
{
    public static class AssetBundlesBuilder
    {
        [MenuItem("Runi Engine/Build Asset Bundles")]
        public static void Build()
        {
            string path = Path.Combine("Assets", "Asset Bundles");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.NoTarget);
        }
    }
}
