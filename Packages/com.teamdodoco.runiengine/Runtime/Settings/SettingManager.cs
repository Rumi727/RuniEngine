#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Datas;
using RuniEngine.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Settings
{
    public static class SettingManager
    {
        public const string rootName = "settings";

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

        public static IReadOnlyDictionary<string, IReadOnlyList<StorableClass>> nameSpaceProjectData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return (IReadOnlyDictionary<string, IReadOnlyList<StorableClass>>)_nameSpaceProjectData;
            }
        }
        static readonly Dictionary<string, StorableClass[]> _nameSpaceProjectData = new Dictionary<string, StorableClass[]>();



        [StaticResettable] static readonly List<UniTask> initTasks = new List<UniTask>();
        public static void SettingInitRegister(Action task) => initTasks.Add(UniTask.RunOnThreadPool(task));
        public static void SettingInitRegister(Func<UniTask> task) => initTasks.Add(UniTask.RunOnThreadPool(task));

        [StaticResettable] static readonly List<UniTask> initLoadTasks = new List<UniTask>();
        public static void SettingInitLoadRegister(IEnumerable<StorableClass> storableClass, string path) => initLoadTasks.Add(UniTask.RunOnThreadPool(() => StorableClassUtility.LoadAll(storableClass, path)));
        public static void SettingInitLoadRegister(Func<UniTask> task) => initLoadTasks.Add(UniTask.RunOnThreadPool(task));

        [StaticResettable(true)] static Action? loadTasks;
        public static void SettingLoadRegister(IEnumerable<StorableClass> storableClass, string path) => loadTasks += () => StorableClassUtility.LoadAll(storableClass, path);
        public static void SettingLoadRegister(Action task) => loadTasks += task;

        [StaticResettable(true)] static Action? saveTasks;
        public static void SettingSaveRegister(IEnumerable<StorableClass> storableClass, string path) => saveTasks += () => StorableClassUtility.LoadAll(storableClass, path);
        public static void SettingSaveRegister(Action task) => saveTasks += task;

        [Awaken]
        public static void Awaken()
        {
            SettingInitRegister(() => _projectData = StorableClassUtility.AutoInitialize<ProjectDataAttribute>());
            SettingInitRegister(() => _globalData = StorableClassUtility.AutoInitialize<GlobalDataAttribute>());
            SettingInitRegister(() =>
            {
                string[] paths = Directory.GetDirectories(Kernel.projectSettingPath);
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    string nameSpace = Path.GetFileName(path);

                    _nameSpaceProjectData.Add(nameSpace, StorableClassUtility.AutoInstanceInitialize<NameSpaceProjectDataAttribute>());
                }
            });

            SettingInitLoadRegister(_projectData, Kernel.projectSettingPath);
            SettingInitLoadRegister(_globalData, Kernel.globalDataPath);
            foreach (var item in _nameSpaceProjectData)
                SettingInitLoadRegister(item.Value, Path.Combine(Kernel.projectSettingPath, item.Key));

            SettingLoadRegister(_globalData, Kernel.globalDataPath);
            foreach (var item in _nameSpaceProjectData)
                SettingLoadRegister(item.Value, Path.Combine(Kernel.projectSettingPath, item.Key));

            SettingSaveRegister(() =>
            {
                Kernel.GlobalData.lastRuniEngineVersion = Kernel.runiEngineVersion;
                StorableClassUtility.SaveAll(_globalData, Kernel.globalDataPath);
            });
            foreach (var item in _nameSpaceProjectData)
                SettingSaveRegister(item.Value, Path.Combine(Kernel.projectSettingPath, item.Key));
        }


        public static bool isDataLoaded { get; private set; } = false;

        public static async UniTask Init()
        {
            await UniTask.WhenAll(initTasks);
            await UniTask.WhenAll(initLoadTasks);

            isDataLoaded = true;
        }

        public static void Load()
        {
            BasicDataNotLoadedException.Exception();
            loadTasks?.Invoke();
        }

        public static void Save()
        {
            BasicDataNotLoadedException.Exception();
            saveTasks?.Invoke();
        }

        public static T? GetProjectSetting<T>(string nameSpace) => GetProjectSetting<T>(nameSpace, out _);

        public static T? GetProjectSetting<T>(string nameSpace, out StorableClass? storableClass)
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);
            if (nameSpaceProjectData.TryGetValue(nameSpace, out var value))
            {
                storableClass = value.First(x => x.type == typeof(T));
                return (T?)storableClass.instance;
            }

            storableClass = null;
            return default;
        }

        public static string[] GetNameSpaces()
        {
            string[] realNameSpaces = Directory.GetDirectories(Kernel.projectSettingPath);
            string[] nameSpaces = new string[realNameSpaces.Length];
            for (int i = 0; i < realNameSpaces.Length; i++)
                nameSpaces[i] = PathUtility.GetRelativePath(Kernel.projectSettingPath, realNameSpaces[i].UniformDirectorySeparatorCharacter());

            return nameSpaces;
        }
    }
}
