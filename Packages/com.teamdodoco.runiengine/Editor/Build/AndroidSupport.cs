#nullable enable

//얘는 빌드 코드기 때문에 에디터에서도 작동해야함
#if UNITY_ANDROID || ENABLE_ANDROID_SUPPORT || UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace RuniEngine.Editor.Build
{
    static class AndroidSupport
    {
        static readonly string tempStreamingAssetsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp" + Kernel.streamingAssetsFolderName);
        static readonly string zipPath = Path.Combine(Kernel.streamingAssetsPath, Kernel.streamingAssetsFolderName + ".zip");

        [InitializeOnLoadMethod]
        static void Init()
        {
            BuildHandler.onBuildStarted += OnPreprocessBuild;
            BuildHandler.onBuildFailure += OnPostprocessBuild;
            BuildHandler.onBuildSuccess += OnPostprocessBuild;
        }

        static bool start = false;
        static bool OnPreprocessBuild()
        {
            if (!Directory.Exists(Kernel.streamingAssetsPath))
                return true;
            else if (!EditorUtility.DisplayDialog(EditorTool.TryGetText("gui.warning"), EditorTool.TryGetText("android_build.warning"), EditorTool.TryGetText("android_build.yes"), EditorTool.TryGetText("android_build.no")))
                return false;

            start = true;

            Debug.Log($"{nameof(Kernel.streamingAssetsPath)} : {Kernel.streamingAssetsPath}");
            Debug.Log($"{nameof(tempStreamingAssetsFolderPath)} : {tempStreamingAssetsFolderPath}");

            if (Directory.Exists(tempStreamingAssetsFolderPath))
            {
                EditorUtility.DisplayDialog(EditorTool.TryGetText("gui.warning"), EditorTool.TryGetText("gui.folder_exists").Replace("{path}", tempStreamingAssetsFolderPath), EditorTool.TryGetText("android_build.cancel"));
                start = false;

                return false;
            }

            if (!File.Exists(zipPath))
            {
                File.Delete(zipPath);
                AssetDatabase.Refresh();
            }

            Directory.Move(Kernel.streamingAssetsPath, tempStreamingAssetsFolderPath);
            Directory.CreateDirectory(Kernel.streamingAssetsPath);

            CompressFileManager.CompressZipFile(tempStreamingAssetsFolderPath, zipPath, "", x => Path.GetExtension(x) != ".meta");
            AssetDatabase.Refresh();

            return true;
        }

        static void OnPostprocessBuild()
        {
            if (!start)
                return;

            start = false;

            if (Directory.Exists(tempStreamingAssetsFolderPath))
            {
                Directory.Delete(Kernel.streamingAssetsPath, true);
                Directory.Move(tempStreamingAssetsFolderPath, Kernel.streamingAssetsPath);

                AssetDatabase.Refresh();
            }
        }
    }
}
#endif