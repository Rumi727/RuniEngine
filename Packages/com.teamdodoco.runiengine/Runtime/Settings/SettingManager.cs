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



        [StaticResettable] static readonly List<Action> initTasks = new List<Action>();
        public static void SettingInitRegister(Action task) => initTasks.Add(task);

        [StaticResettable] static readonly List<Action> initLoadTasks = new List<Action>();
        public static void SettingInitLoadRegister(Action task) => initLoadTasks.Add(task);

        [StaticResettable(true)] static Action? loadTasks;
        public static void SettingLoadRegister(Action task) => loadTasks += task;

        [StaticResettable(true)] static Action? saveTasks;
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

            SettingInitLoadRegister(() => StorableClassUtility.LoadAll(_projectData, Kernel.projectSettingPath));
            SettingInitLoadRegister(() => StorableClassUtility.LoadAll(_globalData, Kernel.globalDataPath));
            SettingInitLoadRegister(() =>
            {
                foreach (var item in _nameSpaceProjectData)
                    StorableClassUtility.LoadAll(item.Value, Path.Combine(Kernel.projectSettingPath, item.Key));
            });

            SettingLoadRegister(() => _globalData.LoadAll(Kernel.globalDataPath));
            foreach (var item in _nameSpaceProjectData)
                SettingLoadRegister(() => item.Value.LoadAll(Path.Combine(Kernel.projectSettingPath, item.Key)));

            SettingSaveRegister(() =>
            {
                Kernel.GlobalData.lastRuniEngineVersion = Kernel.runiEngineVersion;
                StorableClassUtility.SaveAll(_globalData, Kernel.globalDataPath);
            });
            foreach (var item in _nameSpaceProjectData)
                SettingSaveRegister(() => StorableClassUtility.SaveAll(item.Value, Path.Combine(Kernel.projectSettingPath, item.Key)));
        }


        public static bool isDataLoaded { get; private set; } = false;

        public static async UniTask Init()
        {
            UniTask[] uniTasks = new UniTask[initTasks.Count];
            for (int i = 0; i < uniTasks.Length; i++)
                uniTasks[i] = UniTask.RunOnThreadPool(initTasks[i]);

            await UniTask.WhenAll(uniTasks);

            uniTasks = new UniTask[initLoadTasks.Count];
            for (int i = 0; i < uniTasks.Length; i++)
                uniTasks[i] = UniTask.RunOnThreadPool(initLoadTasks[i]);

            await UniTask.WhenAll(uniTasks);

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
