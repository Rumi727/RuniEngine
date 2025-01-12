#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Accounts;
using RuniEngine.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuniEngine.Resource.Texts
{
    public sealed class LanguageLoader : IResourceElement
    {
        [UserData]
        public struct GlobalData
        {
            public static string currentLanguage { get; set; } = "en_us";
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
                if (!ResourceManager.FileExtensionExists(StreamingIOHandler.instance, PathUtility.Combine(ResourceManager.rootName, nameSpace, name, language), out string outPath, ExtensionFilter.jsonFileFilter))
                    return result4;

                Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(outPath, "", StreamingIOHandler.instance);
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

            await UniTask.SwitchToThreadPool();

            Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempLoadedLanguages = new();
            List<string> tempLanguageTypes = new();

            for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
            {
                string nameSpace = resourcePack.nameSpaces[i];
                using IOHandler root = resourcePack.ioHandler.CreateChild(PathUtility.Combine(nameSpace, name));

                if (!root.DirectoryExists())
                {
                    ReportProgress();
                    continue;
                }

                foreach (string filePath in root.GetFiles(ExtensionFilter.jsonFileFilter))
                {
                    string fileName = PathUtility.GetFileNameWithoutExtension(filePath);

                    Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(filePath, "", root);
                    if (lang == null)
                        continue;

                    tempLoadedLanguages.TryAdd(nameSpace, new Dictionary<string, Dictionary<string, string>>());
                    tempLoadedLanguages[nameSpace].TryAdd(fileName, lang);
                }

                ReportProgress();

                void ReportProgress() => progress?.Report((float)(i + 1) / resourcePack.nameSpaces.Count);
            }

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            loadedLanguages = tempLoadedLanguages;
            languageList = tempLanguageTypes;

            isLoaded = true;
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
