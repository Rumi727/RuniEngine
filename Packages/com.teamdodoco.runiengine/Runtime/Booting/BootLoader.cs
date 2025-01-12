#nullable enable
using Cysharp.Threading.Tasks;
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
        public static bool isLoadingStart { get; private set; } = false;
        public static bool isDataLoaded { get; private set; } = false;
        public static bool isAllLoaded { get; private set; } = false;

        public static AsyncTask? resourceTask { get; private set; } = null;

        public static IReadOnlyList<StorableClass> projectData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return _projectData;
            }
        }
        static StorableClass[] _projectData = null!;

        public static IReadOnlyList<StorableClass> globalData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return _globalData;
            }
        }
        static StorableClass[] _globalData = null!;

        //UniTask는 BeforeSplashScreen 단계에서부터 사용 가능
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static async UniTaskVoid Boot()
        {
#if UNITY_WEBGL
            //CS0162 접근할 수 없는 코드 경고를 비활성화 하기 위해 변수로 우회합니다
            bool warningDisable = true;
            if (warningDisable)
                throw new NotSupportedException(Resource.Texts.LanguageLoader.TryGetText("boot_loader.webgl"));
#endif
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

#if UNITY_EDITOR
            StaticReset();
#endif

            //Path Init
            Kernel.PathInitialize();

            //Player Loop Setting
            Debug.Log("Player Loop Setting...");
            {
                PlayerLoopSystem loopSystems = PlayerLoop.GetDefaultPlayerLoop();

                //UniTask Setting
                PlayerLoopHelper.Initialize(ref loopSystems);

                //Awaken Invoke
                await AttributeInvoke<AwakenAttribute>();

                //Custom Update Setting
                CustomPlayerLoopSetter.EventRegister(ref loopSystems);
            }
            Debug.Log("Player Loop Setting End");

            await UniTask.Delay(100, true);
            if (!Kernel.isPlaying)
                return;

            //Splash Screen Play
            SplashScreen.isPlaying = true;

            await UniTask.WaitUntil(() => SplashScreen.resourceLoadable || !Kernel.isPlaying);
            if (!Kernel.isPlaying)
                return;

            //Resource Setup
            TryLoad().Forget();

            await UniTask.WaitUntil(() => isDataLoaded || !Kernel.isPlaying);
            if (!Kernel.isPlaying)
                return;

            //Starten Invoke
            await AttributeInvoke<StartenAttribute>();

            //Splash Screen Stop Wait...
            await UniTask.WaitUntil(() => (!SplashScreen.isPlaying && isAllLoaded) || !Kernel.isPlaying);
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
            Debug.Log("Storable Class Loading...");

            _projectData = StorableClassUtility.AutoInitialize<ProjectDataAttribute>();
            _globalData = StorableClassUtility.AutoInitialize<GlobalDataAttribute>();

            StorableClassUtility.LoadAll(_projectData, Kernel.projectSettingPath);
            StorableClassUtility.LoadAll(_globalData, Kernel.globalDataPath);

            isDataLoaded = true;

            Debug.Log("Storable Class Loaded");

            Debug.Log("Resource Loading...");

            await ResourceManager.Refresh(resourceTask = new AsyncTask("resource_manager.load.name"));
            resourceTask.Dispose();
            resourceTask = null;

            Debug.Log("Resource Loaded");

            //All Loading End
            isAllLoaded = true;
            isLoadingStart = false;

            return true;
        }
        static async UniTask AttributeInvoke<T>() where T : Attribute
        {
            //GC 이슈로 인해 스레드로 전환하지 않으면 메인 스레드에서 프레임 드랍이 일어남
            await UniTask.SwitchToThreadPool();

            List<MethodInfo> methods = new List<MethodInfo>();
            IReadOnlyList<Type> types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Count; typesIndex++)
            {
                MethodInfo[] methodInfos = types[typesIndex].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int methodInfoIndex = 0; methodInfoIndex < methodInfos.Length; methodInfoIndex++)
                {
                    MethodInfo methodInfo = methodInfos[methodInfoIndex];
                    if (methodInfo.AttributeContains<T>() && methodInfo.GetParameters().Length <= 0)
                        methods.Add(methodInfo);
                }
            }

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            for (int i = 0; i < methods.Count; i++)
            {
                try
                {
                    methods[i].Invoke(null, null);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// <see cref="StaticResettableAttribute"/> 어트리뷰트가 붙은 모든 프로퍼티 및 필드를 기본값으로 초기화합니다
        /// <para></para>
        /// <see cref="System.Diagnostics.ConditionalAttribute"/> 어트리뷰트 사용해서 에디터에서만 되게할 수 있지만, 런타임에서도 사용할 수 있으니 굳이 막진 않았습니다
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
                    StaticResettableAttribute attribute = propertyInfo.GetCustomAttribute<StaticResettableAttribute>();

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
                    StaticResettableAttribute attribute = fieldInfo.GetCustomAttribute<StaticResettableAttribute>();

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
