#nullable enable
using Newtonsoft.Json;
using RuniEngine.Jsons;
using System;
using System.Collections.Generic;
using System.IO;

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

        [JsonIgnore]
        public string name
        {
            get
            {
                if (isUnloaded)
                    throw new ObjectDisposedException(GetType().FullName);

                return _name;
            }
        }
        [JsonProperty(nameof(name))] readonly string _name = "";

        [JsonIgnore]
        public string description
        {
            get
            {
                if (isUnloaded)
                    throw new ObjectDisposedException(GetType().FullName);

                return _description;
            }
        }
        [JsonProperty(nameof(description))] readonly string _description = "";

        [JsonIgnore]
        public Version version
        {
            get
            {
                if (isUnloaded)
                    throw new ObjectDisposedException(GetType().FullName);

                return _version;
            }
        }
        [JsonProperty(nameof(version))] readonly Version _version = Version.zero;

        [JsonIgnore]
        public VersionRange targetVersion
        {
            get
            {
                if (isUnloaded)
                    throw new ObjectDisposedException(GetType().FullName);

                return _targetVersion;
            }
        }
        [JsonProperty(nameof(targetVersion))] private VersionRange _targetVersion = new VersionRange(Kernel.runiEngineVersion, Kernel.runiEngineVersion);

        [JsonIgnore] public string path { get; private set; } = "";
        [JsonIgnore] public string iconPatch { get; private set; } = "";

        [JsonIgnore] public IReadOnlyList<string> nameSpaces { get; private set; } = new List<string>();
        [JsonIgnore] public IReadOnlyDictionary<Type, IResourceElement> resourceElements { get; private set; } = new Dictionary<Type, IResourceElement>();

        [JsonIgnore] public bool isUnloaded { get; private set; } = false;

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

        public void UnloadCheck() => isUnloaded = true;
    }
}
