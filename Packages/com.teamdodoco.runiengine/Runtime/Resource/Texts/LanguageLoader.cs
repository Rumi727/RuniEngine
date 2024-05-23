#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Datas;
using RuniEngine.Jsons;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource.Texts
{
    public sealed class LanguageLoader : IResourceElement
    {
        [GlobalData]
        public struct GlobalData
        {
            [JsonProperty] public static string currentLanguage { get; set; } = "en_us";
        }



        public bool isLoaded { get; private set; } = false;
        public ResourcePack? resourcePack { get; set; } = null;



        /// <summary>
        /// string text = loadedLanguages[nameSpace][fileName][key];
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> loadedLanguages = new();
        List<string> languageList = new();



        public const string name = "lang";
        string IResourceElement.name => name;



        public static string TryGetText(string key, string nameSpace = "", string language = "")
        {
            string? value = GetText(key, nameSpace, language);
            if (value == null)
                return key;
            else
                return value;
        }



        public static string? GetText(string key, string nameSpace = "", string language = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (string.IsNullOrWhiteSpace(language))
                language = GlobalData.currentLanguage;

            string? result4 = null;
            if (!ResourceManager.ResourceElementLoop<LanguageLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                if (x.loadedLanguages.TryGetValue(nameSpace, out var result))
                {
                    if (result.TryGetValue(language, out var result2))
                    {
                        result2.TryGetValue(key, out string result3);
                        result4 = result3;

                        return true;
                    }
                }

                return false;
            }))
            {
                if (!ResourceManager.FileExtensionExists(Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name, language), out string outPath, ExtensionFilter.jsonFileFilter))
                    return result4;

                Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(outPath);
                if (lang == null || !lang.TryGetValue(key, out string value))
                    return result4;

                result4 = value;

                ResourcePack? pack = ResourcePack.defaultPack;
                if (pack == null)
                    return result4;

                LanguageLoader loader = (LanguageLoader)pack.resourceElements[typeof(LanguageLoader)];

                loader.loadedLanguages.TryAdd(nameSpace, new());
                loader.loadedLanguages[nameSpace].TryAdd(language, new());
                loader.loadedLanguages[nameSpace][language].TryAdd(key, value);

                return result4;
            }    

            return result4;
        }

        public static IReadOnlyCollection<string> GetLanguageList()
        {
            List<string> languageList = new List<string>();

            ResourceManager.ResourceElementLoop<LanguageLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                languageList.Union(x.languageList);
                return true;
            });

            return languageList;
        }



        public UniTask Load() => Load(null);
        public async UniTask Load(IProgress<float>? progress)
        {
            if (resourcePack == null)
                return;

            Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempLoadedLanguages = new();
            List<string> tempLanguageTypes = new();

            if (ThreadTask.isMainThread)
                await UniTask.RunOnThreadPool(Thread);
            else
                await Thread();

            loadedLanguages = tempLoadedLanguages;
            languageList = tempLanguageTypes;

            isLoaded = true;
            
            async UniTask Thread()
            {
                for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
                {
                    string nameSpace = resourcePack.nameSpaces[i];
                    string langPath = Path.Combine(resourcePack.path, ResourceManager.rootName, nameSpace, name);
                    if (!Directory.Exists(langPath))
                        return;

                    string[] filePaths = DirectoryUtility.GetFiles(langPath, ExtensionFilter.jsonFileFilter);
                    for (int j = 0; j < filePaths.Length; j++)
                    {
                        string filePath = filePaths[j];
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(filePath);
                        if (lang == null)
                            continue;

                        foreach (var item in lang)
                        {
                            tempLoadedLanguages.TryAdd(nameSpace, new Dictionary<string, Dictionary<string, string>>());
                            tempLoadedLanguages[nameSpace].TryAdd(fileName, new Dictionary<string, string>());
                            tempLoadedLanguages[nameSpace][fileName].TryAdd(item.Key, item.Value);

                            if (!tempLanguageTypes.Contains(fileName))
                                tempLanguageTypes.Add(fileName);
                        }
                    }

                    progress?.Report((float)i / resourcePack.nameSpaces.Count);
                }

                await UniTask.CompletedTask;
            }
        }

        public async UniTask Unload()
        {
            loadedLanguages = new();
            languageList = new();

            isLoaded = false;

            await UniTask.CompletedTask;
        }
    }
}
