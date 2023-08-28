#nullable enable
using System.IO;
using UnityEngine;

namespace RuniEngine
{
    public static partial class Kernel
    {
        /// <summary>
        /// Application.dataPath
        /// </summary>
        public static string dataPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_dataPath))
                    return _dataPath;
                else
                    return _dataPath = Application.dataPath;
            }
        }
        static string _dataPath = "";

        /// <summary>
        /// Application.streamingAssetsPath
        /// </summary>
        public static string streamingAssetsPath
        {
            get
            {
#if (UNITY_ANDROID || ENABLE_ANDROID_SUPPORT) && !UNITY_EDITOR
                if (_streamingAssetsPath != "")
                    return _streamingAssetsPath;
                else
                {
                    _streamingAssetsPath = PathUtility.Combine(persistentDataPath, streamingAssetsFolderName);

                    if (!Directory.Exists(_streamingAssetsPath))
                        Directory.CreateDirectory(_streamingAssetsPath);

                    return _streamingAssetsPath;
                }
#else
                if (!string.IsNullOrEmpty(_streamingAssetsPath))
                    return _streamingAssetsPath;
                else
                    return _streamingAssetsPath = Application.streamingAssetsPath;
#endif
            }
        }
        static string _streamingAssetsPath = "";

        public const string streamingAssetsFolderName = "StreamingAssets";

        /// <summary>
        /// Application.persistentDataPath
        /// </summary>
        public static string persistentDataPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_persistentDataPath))
                    return _persistentDataPath;
                else
                    return _persistentDataPath = Application.persistentDataPath;
            }
        }
        static string _persistentDataPath = "";

        /// <summary>
        /// Application.temporaryCachePath
        /// </summary>
        public static string temporaryCachePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_temporaryCachePath))
                    return _temporaryCachePath;
                else
                    return _temporaryCachePath = Application.temporaryCachePath;
            }
        }
        static string _temporaryCachePath = "";

        /// <summary>
        /// PathTool.Combine(Kernel.persistentDataPath, "Resource Pack")
        /// </summary>
        public static string resourcePackPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_resourcePackPath))
                    return _resourcePackPath;
                else
                {
                    _resourcePackPath = Path.Combine(persistentDataPath, "Resource Pack");

                    if (!Directory.Exists(_resourcePackPath))
                        Directory.CreateDirectory(_resourcePackPath);

                    return _resourcePackPath;
                }
            }
        }
        static string _resourcePackPath = "";

        /// <summary>
        /// PathTool.Combine(Kernel.streamingAssetsPath, "projectData")
        /// </summary>
        public static string projectDataPath { get; } = Path.Combine(streamingAssetsPath, "projectData");

        public static void PathInitialize()
        {
            _ = dataPath;
            _ = streamingAssetsPath;
            _ = persistentDataPath;
            _ = temporaryCachePath;
            _ = resourcePackPath;

            _ = companyName;
            _ = productName;
            _ = version;

            _ = unityVersion;
        }
    }
}
