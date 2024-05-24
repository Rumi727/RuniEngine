#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Accounts;
using RuniEngine.Datas;
using RuniEngine.Resource;
using RuniEngine.Splashs;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
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

        public static bool isLoadingStart { get; private set; } = false;
        public static bool isDataLoaded { get; private set; } = false;
        public static bool isAllLoaded { get; private set; } = false;

        public static AsyncTask? resourceTask { get; private set; } = null;

        //UniTask는 BeforeSplashScreen 단계에서부터 사용 가능
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static async UniTaskVoid Boot()
        {
#if UNITY_WEBGL
            //CS0162 접근할 수 없는 코드 경고를 비활성화 하기 위해 변수로 우회합니다
            bool warningDisable = true;
            if (warningDisable)
                throw new NotSupportedException(LanguageLoader.TryGetText("boot_loader.webgl"));
#endif
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

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

            //Resource Setup
            TryLoad().Forget();

            await UniTask.Delay(100, true);
            if (!Kernel.isPlaying)
                return;
            
            //Splash Screen Play
            SplashScreen.isPlaying = true;

            await UniTask.WaitUntil(() => isDataLoaded);
            if (!Kernel.isPlaying)
                return;

            //Starten Invoke
            AttributeInvoke<StartenAttribute>();

            //Splash Screen Stop Wait...
            await UniTask.WaitUntil(() => !SplashScreen.isPlaying);
            if (!Kernel.isPlaying)
                return;

            if (SplashScreen.ProjectData.startSceneIndex >= 0)
                await SceneManager.LoadSceneAsync(SplashScreen.ProjectData.startSceneIndex, LoadSceneMode.Single);
            else
                await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }

        public static async UniTask<bool> TryLoad()
        {
            NotMainThreadException.Exception();

            if (isLoadingStart || isAllLoaded)
                return false;

            isLoadingStart = true;

            //Storable Class
            Debug.Log("Storable Class Loading...", nameof(BootLoader));
            {
                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => _projectData = StorableClassUtility.AutoInitialize<ProjectDataAttribute>()),
                    UniTask.RunOnThreadPool(() => _globalData = StorableClassUtility.AutoInitialize<GlobalDataAttribute>()),
                    UniTask.RunOnThreadPool(UserAccountManager.UserDataInit)
                    );

                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => StorableClassUtility.LoadAll(_projectData, Kernel.projectSettingPath)),
                    UniTask.RunOnThreadPool(() => StorableClassUtility.LoadAll(_globalData, Kernel.globalDataPath))
                    );

                isDataLoaded = true;
            }
            Debug.Log("Storable Class Loaded", nameof(BootLoader));

            Debug.Log("Resource Loading...", nameof(BootLoader));

            await ResourceManager.Refresh(resourceTask = new AsyncTask("resource_manager.load.name"));
            resourceTask.Dispose();
            resourceTask = null;

            Debug.Log("Resource Loaded", nameof(BootLoader));

            //All Loading End
            isAllLoaded = true;
            isLoadingStart = false;

            return true;
        }

        public static bool TryUnload()
        {
            if (!isLoadingStart)
                return false;

            ResourceManager.AllDestroy();

            isAllLoaded = false;
            isLoadingStart = false;

            return true;
        }

        static void AttributeInvoke<T>() where T : Attribute
        {
            IReadOnlyList<Type> types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Count; typesIndex++)
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

        /// <summary>
        /// Static Reset 어트리뷰트가 붙은 모든 프로퍼티 및 필드를 기본값으로 초기화합니다
        /// </summary>
        public static void StaticReset()
        {
            IReadOnlyList<Type> types = ReflectionManager.types;
            for (int i = 0; i < types.Count; i++)
            {
                PropertyInfo[] propertyInfos = types[i].GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int j = 0; j < propertyInfos.Length; j++)
                {
                    PropertyInfo propertyInfo = propertyInfos[j];
                    StaticResetAttribute attribute = propertyInfo.GetCustomAttribute<StaticResetAttribute>();

                    if (attribute != null)
                    {
                        try
                        {
                            if (attribute.value != null)
                                propertyInfo.SetValue(null, attribute.value);
                            else
                            {
                                if (attribute.isNullable)
                                    propertyInfo.SetValue(null, propertyInfo.PropertyType.GetDefaultValue());
                                else
                                    propertyInfo.SetValue(null, propertyInfo.PropertyType.GetDefaultValueNotNull());
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }

                FieldInfo[] fieldInfos = types[i].GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    FieldInfo fieldInfo = fieldInfos[j];
                    StaticResetAttribute attribute = fieldInfo.GetCustomAttribute<StaticResetAttribute>();

                    if (attribute != null)
                    {
                        try
                        {
                            if (attribute.value != null)
                                fieldInfo.SetValue(null, attribute.value);
                            else
                            {
                                if (attribute.isNullable)
                                    fieldInfo.SetValue(null, fieldInfo.FieldType.GetDefaultValue());
                                else
                                    fieldInfo.SetValue(null, fieldInfo.FieldType.GetDefaultValueNotNull());
                            }
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
