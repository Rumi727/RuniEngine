#nullable enable
using Newtonsoft.Json;
using RuniEngine.Account;
using RuniEngine.Booting;
using RuniEngine.Data;
using RuniEngine.Resource;
using RuniEngine.Threading;
using System.Diagnostics;
using UnityEngine;

namespace RuniEngine
{
    public static partial class Kernel
    {
        [UserData]
        public struct UserData
        {
            [JsonProperty] public static Version lastRuniEngineVersion { get; set; } = runiEngineVersion;
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




        [Awaken]
        static void Awaken()
        {
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
            TimeUpdate();
            internetReachability = Application.internetReachability;
        }

        static void Qutting()
        {
            ResourceManager.AllDestroy();

            if (UserAccountManager.currentAccount != null)
                UserAccountManager.Logout();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.pauseStateChanged -= PauseStateChanged;
#endif
        }
    }
}
