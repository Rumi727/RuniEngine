#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Jsons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource
{
    public sealed class ResourcePack
    {
        ResourcePack() { }

        public static ResourcePack? defaultPack
        {
            get
            {
                _defaultPack ??= Create(Kernel.streamingAssetsPath);
                return _defaultPack;
            }
        }
        static ResourcePack? _defaultPack;

        [JsonIgnore] public string name => _name;
        [JsonProperty(nameof(name))] readonly string _name = "";

        [JsonIgnore] public string description => _description;
        [JsonProperty(nameof(description))] readonly string _description = "";

        [JsonIgnore] public Version version => _version;
        [JsonProperty(nameof(version))] readonly Version _version = Version.zero;

        [JsonIgnore] public VersionRange targetVersion => _targetVersion;
        [JsonProperty(nameof(targetVersion))] VersionRange _targetVersion = new VersionRange(Kernel.runiEngineVersion, Kernel.runiEngineVersion);

        [JsonIgnore] public string path { get; private set; } = "";
        [JsonIgnore] public string iconPatch { get; private set; } = "";

        [JsonIgnore] public IReadOnlyList<string> nameSpaces { get; private set; } = new List<string>();
        [JsonIgnore] public IReadOnlyDictionary<Type, IResourceElement> resourceElements { get; private set; } = new Dictionary<Type, IResourceElement>();

        [JsonIgnore] public bool isLoaded { get; private set; } = false;

        public static ResourcePack? Create(string path)
        {
            if (!ResourceManager.FileExtensionExists(Path.Combine(path, "pack"), out string jsonPath, ExtensionFilter.jsonFileFilter))
                return null;

            ResourcePack? resourcePack = JsonManager.JsonRead<ResourcePack>(jsonPath);
            if (resourcePack == null)
                return null;

            resourcePack.path = path;
            resourcePack.iconPatch = Path.Combine(path, "pack");

            {
                string[] nameSpacePaths = Directory.GetDirectories(Path.Combine(path, ResourceManager.rootName));
                List<string> nameSpaces = new List<string>();

                for (int j = 0; j < nameSpacePaths.Length; j++)
                {
                    string nameSpacePath = nameSpacePaths[j];
                    nameSpaces.Add(Path.GetFileName(nameSpacePath));
                }

                resourcePack.nameSpaces = nameSpaces;
            }

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

            return resourcePack;
        }

        public async UniTask Load(IProgress<float>? progress = null)
        {
            List<UniTask> cachedUniTasks = new List<UniTask>();
            SynchronizedCollection<float> progressLists = new SynchronizedCollection<float>();
            int progressIndex = 0;

            foreach (var item in resourceElements)
            {
                if (progress != null)
                {
                    int index = progressIndex;
                    IProgress<float> progress2 = Progress.Create<float>(x =>
                    {
                        progressLists[index] = x;
                        progress.Report(progressLists.Sum() / resourceElements.Count);
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
