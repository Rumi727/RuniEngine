#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Jsons;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuniEngine.Resource
{
    public sealed class ResourcePack
    {
        ResourcePack() { }
        ResourcePack(string name, string description, Version version, VersionRange targetRuniEngineVersion)
        {
            _name = name;
            _description = description;
            _version = version;
            _targetRuniEngineVersion = targetRuniEngineVersion;
        }

        public static ResourcePack defaultPack
        {
            get
            {
                _defaultPack ??= Create(StreamingIOHandler.instance);
                if (_defaultPack == null)
                    return new ResourcePack();

                return _defaultPack;
            }
        }
        static ResourcePack? _defaultPack;

        public static VersionRange compatibleRuniEngineVersion => new VersionRange(Version.zero, Kernel.runiEngineVersion);

        [JsonIgnore] public string name => _name;
        [JsonProperty(nameof(name))] readonly string _name = "";

        [JsonIgnore] public string description => _description;
        [JsonProperty(nameof(description))] readonly string _description = "";

        [JsonIgnore] public Version version => _version;
        [JsonProperty(nameof(version))] readonly Version _version = Version.zero;

        [JsonIgnore] public VersionRange targetRuniEngineVersion => _targetRuniEngineVersion;
        [JsonProperty(nameof(targetRuniEngineVersion))] VersionRange _targetRuniEngineVersion = new VersionRange(Kernel.runiEngineVersion, Kernel.runiEngineVersion);

        [JsonIgnore] public IOHandler ioHandler { get; private set; } = IOHandler.empty;

        [JsonIgnore] public IReadOnlyList<string> nameSpaces { get; private set; } = Array.Empty<string>();
        [JsonIgnore] public IReadOnlyDictionary<Type, IResourceElement> resourceElements { get; private set; } = new Dictionary<Type, IResourceElement>();

        [JsonIgnore] public bool isValid { get; private set; } = false;
        [JsonIgnore] public bool isLoaded { get; private set; } = false;

        public static ResourcePack? Create(string path) => Create(new FileIOHandler(path));

        public static ResourcePack? Create(IOHandler ioHandler)
        {
            if (!ioHandler.FileExists("pack", out string jsonPath, ExtensionFilter.jsonFileFilter))
                return null;

            ResourcePack? resourcePack = JsonManager.JsonRead<ResourcePack>(jsonPath, "", ioHandler);
            if (resourcePack == null)
                return null;

            return Create(ioHandler, resourcePack);
        }

        public static ResourcePack? Create(IOHandler ioHandler, string name, string description, Version version) => Create(ioHandler, new ResourcePack(name, description, version, Kernel.runiEngineVersion));

        static ResourcePack Create(IOHandler ioHandler, ResourcePack resourcePack)
        {
            resourcePack.ioHandler = ioHandler.CreateChild(ResourceManager.rootName);

            {
                resourcePack.nameSpaces = ioHandler.GetDirectories(ResourceManager.rootName)
                                                   .Select(static nameSpacePath => PathUtility.GetFileName(nameSpacePath))
                                                   .ToArray();
            }

            /*
             * 좀 더 확장성 있게 수정해야함
             * 실시간 로드/언로드 기능 구현해라!!!!!!!!
             */
            {
                Dictionary<Type, IResourceElement> resourceElements = new Dictionary<Type, IResourceElement>();

                IReadOnlyList<Type> types = ReflectionManager.types;
                for (int i = 0; i < types.Count; i++)
                {
                    Type type = types[i];
                    if (type.IsSubtypeOf<IResourceElement>())
                    {
                        IResourceElement resourceElement = (IResourceElement)Activator.CreateInstance(type);
                        resourceElement.resourcePack = resourcePack;

                        resourceElements.Add(type, resourceElement);
                    }
                }

                resourcePack.resourceElements = resourceElements;
            }

            resourcePack.isValid = true;

            return resourcePack;
        }

        public async UniTask Load(IProgress<float>? progress = null)
        {
            NotMainThreadException.Exception();

            try
            {
                List<UniTask> cachedUniTasks = new List<UniTask>();
                SynchronizedCollection<float> progressLists = new SynchronizedCollection<float>();
                int progressIndex = 0;

                foreach (var item in resourceElements)
                {
                    if (progress != null)
                    {
                        int index = progressIndex;
                        IProgress<float> progress2 = Progress.Create<float>(y =>
                        {
                            try
                            {
                                ThreadTask.Lock(ref progressLists.internalSync);

                                progressLists.internalList[index] = y;
                                progress.Report(progressLists.internalList.Sum() / resourceElements.Count);

                            }
                            finally
                            {
                                ThreadTask.Unlock(ref progressLists.internalSync);
                            }
                        });

                        progressLists.Add(0);
                        progressIndex++;

                        cachedUniTasks.Add(item.Value.Load(progress2));
                    }
                    else
                        cachedUniTasks.Add(item.Value.Load());
                }

                await UniTask.WhenAll(cachedUniTasks);
                isLoaded = true;
            }
            finally
            {
                ResourceManager.GarbageRemoval();
            }
        }

        public async UniTask Unload()
        {
            isLoaded = false;

            List<UniTask> cachedUniTasks = new List<UniTask>();
            foreach (var item in resourceElements)
                cachedUniTasks.Add(item.Value.Unload());

            await UniTask.WhenAll(cachedUniTasks);
        }
    }
}
