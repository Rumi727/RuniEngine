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
                    _streamingAssetsPath = Path.Combine(persistentDataPath, streamingAssetsFolderName);

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
        /// PathUtility.Combine(Kernel.persistentDataPath, "Global Data")
        /// </summary>
        public static string globalDataPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_globalDataPath))
                    return _globalDataPath;
                else
                {
                    _globalDataPath = PathUtility.Combine(persistentDataPath, "Global Data");

                    if (!Directory.Exists(_globalDataPath))
                        Directory.CreateDirectory(_globalDataPath);

                    return _globalDataPath;
                }
            }
        }
        static string _globalDataPath = "";

        /// <summary>
        /// PathUtility.Combine(Kernel.persistentDataPath, "Resource Pack")
        /// </summary>
        public static string resourcePackPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_resourcePackPath))
                    return _resourcePackPath;
                else
                {
                    _resourcePackPath = PathUtility.Combine(persistentDataPath, "Resource Pack");

                    if (!Directory.Exists(_resourcePackPath))
                        Directory.CreateDirectory(_resourcePackPath);

                    return _resourcePackPath;
                }
            }
        }
        static string _resourcePackPath = "";

        /// <summary>
        /// PathUtility.Combine(Kernel.streamingAssetsPath, "setting")
        /// </summary>
        public static string projectSettingPath { get; } = PathUtility.Combine(streamingAssetsPath, "settings");

        public static void PathInitialize()
        {
            _ = dataPath;
            _ = streamingAssetsPath;
            _ = persistentDataPath;
            _ = temporaryCachePath;
            _ = globalDataPath;
            _ = resourcePackPath;

            _ = companyName;
            _ = productName;
            _ = version;

            _ = unityVersion;
        }
    }
}
