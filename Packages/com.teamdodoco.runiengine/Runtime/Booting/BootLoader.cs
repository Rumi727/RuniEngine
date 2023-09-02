#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Account;
using RuniEngine.Data;
using RuniEngine.Resource;
using RuniEngine.Splash;
using RuniEngine.Threading;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

namespace RuniEngine.Booting
{
    public static class BootLoader
    {
        public static StorableClass[] projectData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return _projectData;
            }
        }
        internal static StorableClass[] _projectData = null!;

        public static StorableClass[] globalData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return _globalData;
            }
        }
        internal static StorableClass[] _globalData = null!;

        public static bool basicDataLoaded { get; set; } = false;
        public static bool allLoaded { get; set; } = false;

        public static bool splashAniPlaying { get; set; } = false;

        [RuntimeInitializeOnLoadMethod]
        public static async void Booting()
        {
            NotPlayModeException.Exception();
            NotMainThreadException.Exception();

#if UNITY_WEBGL && !UNITY_EDITOR
            //CS0162 접근할 수 없는 코드 경고를 비활성화 하기 위해 변수로 우회합니다
            bool warningDisable = true;
            if (warningDisable)
                throw new NotSupportedException(LanguageLoader.TryGetText("boot_loader.webgl"));
#endif

            //Path Init
            Kernel.PathInitialize();

            //Player Loop Setting
            Debug.Log("Player Loop Setting...", nameof(BootLoader));
            {
                PlayerLoopSystem loopSystems = PlayerLoop.GetCurrentPlayerLoop();

                //UniTask Setting
                PlayerLoopHelper.Initialize(ref loopSystems);

                //Awaken Invoke
                AttributeInvoke<AwakenAttribute>();

                //Custom Update Setting
                CustomPlayerLoopSetter.EventRegister(ref loopSystems);
                PlayerLoop.SetPlayerLoop(loopSystems);
            }
            Debug.Log("Player Loop Setting End", nameof(BootLoader));

            await UniTask.Delay(100);
            if (!Kernel.isPlaying)
                return;

            //Splash Screen Play
            SplashScreen.isPlaying = true;

            //Storable Class
            Debug.Log("Storable Class Loading...", nameof(BootLoader));
            {
                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => _projectData = StorableClassUtility.AutoInitialize<ProjectDataAttribute>()),
                    UniTask.RunOnThreadPool(() => _globalData = StorableClassUtility.AutoInitialize<GlobalDataAttribute>()),
                    UniTask.RunOnThreadPool(UserAccountManager.UserDataInit)
                    );
                if (!Kernel.isPlaying)
                    return;

                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => StorableClassUtility.LoadAll(_projectData, Kernel.projectDataPath)),
                    UniTask.RunOnThreadPool(() => StorableClassUtility.LoadAll(_globalData, Kernel.globalDataPath))
                    );
                if (!Kernel.isPlaying)
                    return;

                basicDataLoaded = true;
            }
            Debug.Log("Storable Class Loaded", nameof(BootLoader));

            Debug.Log("Resource Loading...", nameof(BootLoader));

            await ResourceManager.Refresh();
            if (!Kernel.isPlaying)
                return;

            Debug.Log("Resource Loaded", nameof(BootLoader));

            //All Loading End
            allLoaded = true;

            //Splash Screen Stop Wait...
            await UniTask.WaitUntil(() => !SplashScreen.isPlaying);
            if (!Kernel.isPlaying)
                return;

            await SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        }

        static void AttributeInvoke<T>() where T : Attribute
        {
            Type[] types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
            {
                MethodInfo[] methodInfos = types[typesIndex].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int methodInfoIndex = 0; methodInfoIndex < methodInfos.Length; methodInfoIndex++)
                {
                    MethodInfo methodInfo = methodInfos[methodInfoIndex];
                    if (methodInfo.AttributeContains<T>() && methodInfo.GetParameters().Length <= 0)
                    {
                        try
                        {
                            methodInfo.Invoke(null, null);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
    }
}
