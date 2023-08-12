#nullable enable
using RuniEngine.Booting;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace RuniEngine
{
    public static class Kernel
    {
        public static Version runiEngineVersion { get; } = new Version(0, 0, 0);

        #region Info
        /// <summary>
        /// Application.dataPath
        /// </summary>
        public static string dataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_dataPath))
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
                if (string.IsNullOrEmpty(_streamingAssetsPath))
                    return _streamingAssetsPath;
                else
                    return _streamingAssetsPath = Application.streamingAssetsPath;
                ;
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
                if (string.IsNullOrEmpty(_persistentDataPath))
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
                if (string.IsNullOrEmpty(_temporaryCachePath))
                    return _temporaryCachePath;
                else
                    return _temporaryCachePath = Application.temporaryCachePath;
            }
        }
        static string _temporaryCachePath = "";

        /// <summary>
        /// PathTool.Combine(Kernel.persistentDataPath, "Save Data")
        /// </summary>
        public static string saveDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_saveDataPath))
                    return _saveDataPath;
                else
                {
                    _saveDataPath = Path.Combine(persistentDataPath, "Save Data");

                    if (!Directory.Exists(_saveDataPath))
                        Directory.CreateDirectory(_saveDataPath);

                    return _saveDataPath;
                }
            }
        }
        static string _saveDataPath = "";

        /// <summary>
        /// PathTool.Combine(Kernel.persistentDataPath, "Resource Pack")
        /// </summary>
        public static string resourcePackPath
        {
            get
            {
                if (string.IsNullOrEmpty(_resourcePackPath))
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

        /*/// <summary>
        /// PathTool.Combine(Kernel.streamingAssetsPath, "projectSettings")
        /// </summary>
        public static string projectSettingPath { get; } = Path.Combine(streamingAssetsPath, "projectSettings");*/



        /// <summary>
        /// Application.companyName
        /// </summary>
        public static string companyName
        {
            get
            {
                if (string.IsNullOrEmpty(_companyName))
                    return _companyName;
                else
                    return _companyName = Application.companyName;
            }
        }
        static string _companyName = "";

        /// <summary>
        /// Application.productName
        /// </summary>
        public static string productName
        {
            get
            {
                if (string.IsNullOrEmpty(_productName))
                    return _productName;
                else
                    return _productName = Application.productName;
            }
        }
        static string _productName = "";

        /// <summary>
        /// Application.version
        /// </summary>
        public static string version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                    return _version;
                else
                    return _version = Application.version;
            }
        }
        static string _version = "";



        /// <summary>
        /// Application.unityVersion
        /// </summary>
        public static string unityVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                    return _unityVersion;
                else
                    return _unityVersion = Application.unityVersion;
            }
        }
        static string _unityVersion = "";



        /// <summary>
        /// Application.platform
        /// </summary>
        public static RuntimePlatform platform { get; } = Application.platform;



        /// <summary>
        /// Application.internetReachability
        /// </summary>
        public static NetworkReachability internetReachability { get; private set; } = NetworkReachability.NotReachable;
        #endregion

        #region Time
        public static float fps { get; private set; } = 60;

        public static float deltaTime { get; private set; } = fps60second;
        public static float fpsDeltaTime { get; private set; } = 1;

        public static double deltaTimeDouble { get; private set; } = fps60second;
        public static double fpsDeltaTimeDouble { get; private set; } = 1;

        public static float smoothDeltaTime { get; private set; } = fps60second;
        public static float fpsSmoothDeltaTime { get; private set; } = fps60second;

        public static float unscaledDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledDeltaTime { get; private set; } = 1;

        public static double unscaledDeltaTimeDouble { get; private set; } = fps60second;
        public static double fpsUnscaledDeltaTimeDouble { get; private set; } = 1;

        public static float unscaledSmoothDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledSmoothDeltaTime { get; private set; } = fps60second;

        public static float fixedDeltaTime { get; set; } = fps60second;

        public const float fps60second = 1f / 60f;

        /// <summary>
        /// 게임의 전체 속도를 결정 합니다
        /// </summary>
        public static float gameSpeed { get; set; } = 1;
        #endregion

        #region Is Playing
#if UNITY_EDITOR
        /// <summary>
        /// Editor: ThreadManager.isMainThread && Application.isEditor
        /// /
        /// Build: const false
        /// </summary>
        public static bool isEditor => ThreadManager.isMainThread && Application.isEditor;

        /// <summary>
        /// Editor: !ThreadManager.isMainThread || Application.isPlaying
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlaying => !ThreadManager.isMainThread || Application.isPlaying;

        /// <summary>
        /// Editor: !ThreadManager.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlayingAndNotPaused => !ThreadManager.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused);
#else
        public const bool isEditor = false;
        public const bool isPlaying = true;
        public const bool isPlayingAndNotPaused = true;
#endif
        #endregion




        [Awaken]
        public static void Awaken()
        {
            _ = dataPath;
            _ = streamingAssetsPath;
            _ = persistentDataPath;
            _ = temporaryCachePath;
            _ = saveDataPath;
            _ = resourcePackPath;

            _ = companyName;
            _ = productName;
            _ = version;

            _ = unityVersion;

            CustomPlayerLoopSetter.initEvent += Update;
            Application.quitting += Qutting;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.pauseStateChanged += PauseStateChanged;
#endif
        }

#if UNITY_EDITOR
        static void PauseStateChanged(UnityEditor.PauseState pauseState) => deltaTimeStopwatch.Restart();
#endif

        static readonly Stopwatch deltaTimeStopwatch = Stopwatch.StartNew();
        static float lastUnscaledDeltaTime = fps60second;
        public static void Update()
        {
            double realDeltaTime = deltaTimeStopwatch.Elapsed.TotalSeconds;
            deltaTimeStopwatch.Restart();

            //게임 속도를 0에서 100 사이로 정하고, 타임 스케일을 게임 속도로 정합니다
            gameSpeed = gameSpeed.Clamp(0, 100);
            Time.timeScale = gameSpeed;

            //유니티의 내장 변수들은 테스트 결과, 약간의 성능을 더 먹는것으로 확인되었기 때문에
            //이렇게 관리 스크립트가 변수를 할당하고 다른 스크립트가 그 변수를 가져오는것이 성능에 더 도움 되는것을 확인하였습니다
            deltaTime = (float)realDeltaTime * gameSpeed;
            fpsDeltaTime = (float)(deltaTime * VideoManager.standardFPS);

            deltaTimeDouble = realDeltaTime * gameSpeed;
            fpsDeltaTimeDouble = deltaTimeDouble * VideoManager.standardFPS;

            unscaledDeltaTime = (float)realDeltaTime;
            fpsUnscaledDeltaTime = (float)(unscaledDeltaTime * VideoManager.standardFPS);

            unscaledDeltaTimeDouble = realDeltaTime;
            fpsUnscaledDeltaTimeDouble = unscaledDeltaTimeDouble * VideoManager.standardFPS;

            fixedDeltaTime = (float)(1d / VideoManager.standardFPS);
            Time.fixedDeltaTime = fixedDeltaTime;

            fps = 1f / unscaledDeltaTime;

            //Smooth Delta Time
            //테스트 결과, 게임 속도 영향을 제외하면 유니티 내부 구현이랑 정확히 같습니다
            {
                unscaledSmoothDeltaTime += (unscaledDeltaTime - lastUnscaledDeltaTime) * 0.2f;
                smoothDeltaTime = unscaledDeltaTime * gameSpeed;

                fpsSmoothDeltaTime = (float)(smoothDeltaTime * VideoManager.standardFPS);
                fpsUnscaledSmoothDeltaTime = (float)(unscaledSmoothDeltaTime * VideoManager.standardFPS);

                lastUnscaledDeltaTime = unscaledSmoothDeltaTime;
            }

            internetReachability = Application.internetReachability;
        }

        static void Qutting()
        {
            ResourceManager.AllDestroy();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.pauseStateChanged -= PauseStateChanged;
#endif
        }
    }
}
