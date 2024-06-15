#nullable enable
using Newtonsoft.Json;
using RuniEngine.Accounts;
using RuniEngine.Booting;
using RuniEngine.Settings;
using RuniEngine.Threading;
using System;
using System.Diagnostics;
using UnityEngine;

namespace RuniEngine
{
    public static partial class Kernel
    {
        [GlobalData]
        public struct GlobalData
        {
            public static Version lastRuniEngineVersion { get; set; } = runiEngineVersion;
        }

        public static Version runiEngineVersion { get; } = new Version(0, 0, 0);

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
        /// Application.platform
        /// </summary>
        public static RuntimePlatform platform { get; } = Application.platform;



        /// <summary>
        /// Application.internetReachability
        /// </summary>
        public static NetworkReachability internetReachability { get; private set; } = NetworkReachability.NotReachable;



#if UNITY_EDITOR
        /// <summary>
        /// Editor: ThreadTask.isMainThread && Application.isEditor
        /// /
        /// Build: const false
        /// </summary>
        public static bool isEditor => ThreadTask.isMainThread && Application.isEditor;

        /// <summary>
        /// Editor: !ThreadTask.isMainThread || Application.isPlaying
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlaying => !ThreadTask.isMainThread || Application.isPlaying;

        /// <summary>
        /// Editor: !ThreadTask.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlayingAndNotPaused => !ThreadTask.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused);
#else
        public const bool isEditor = false;
        public const bool isPlaying = true;
        public const bool isPlayingAndNotPaused = true;
#endif



        /// <summary>
        /// Application.quitting 이벤트랑 동일하지만 커널보다 먼저 실행되는 것을 보장하며 플레이 모드 해제 시 이벤트가 자동으로 초기화됩니다
        /// </summary>
        public static event Action? quitting;



        [Awaken]
        static void Awaken()
        {
            CustomPlayerLoopSetter.initEvent += Update;
            Application.quitting += Quitting;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
            UnityEditor.EditorApplication.pauseStateChanged += PauseStateChanged;
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            if (!isPlaying)
                UnityEditor.EditorApplication.update += Update;
        }
#endif

#if UNITY_EDITOR
        static void PauseStateChanged(UnityEditor.PauseState pauseState) => deltaTimeStopwatch.Restart();
#endif

        static readonly Stopwatch deltaTimeStopwatch = Stopwatch.StartNew();
        public static void Update()
        {
            TimeUpdate();
            internetReachability = Application.internetReachability;
        }

        static void Quitting()
        {
            quitting?.Invoke();
            quitting = null;

            Application.quitting -= Quitting;

            AsyncTask.AllAsyncTaskCancel();

            if (UserAccountManager.currentAccount != null)
                UserAccountManager.LogoutWithoutUnload();

            if (SettingManager.isDataLoaded)
                SettingManager.Save();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
            UnityEditor.EditorApplication.pauseStateChanged -= PauseStateChanged;
#endif
        }
    }
}
