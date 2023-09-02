#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Booting;
using RuniEngine.Data;
using RuniEngine.Json;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Resource
{
    public sealed class LanguageLoader : IResourceElement
    {
        public LanguageLoader() => refreshDelegates = new ResourceManager.RefreshDelegate[] { Refresh };

        [GlobalData]
        public struct GlobalData
        {
            [JsonProperty] public static string currentLanguage { get; set; } = "en_us";
        }



        public static bool isLoaded { get; private set; } = false;
        public static List<string> languageList { get; private set; } = new();



        public const string name = "lang";
        string IResourceElement.name => name;

        public ResourceManager.RefreshDelegate[] refreshDelegates { get; }



        [Awaken]
        static void Awaken() => ResourceManager.ElementRegister(new LanguageLoader());


        public static string TryGetText(string key, string nameSpace = "", string language = "")
        {
            string? value = GetText(key, nameSpace, language);
            if (value == null)
                return key;
            else
                return value;
        }

        /// <summary>
        /// string text = loadedLanguages[nameSpace][fileName][key];
        /// </summary>
        static Dictionary<string, Dictionary<string, Dictionary<string, string>>> loadedLanguages = new();
        public static string? GetText(string key, string nameSpace = "", string language = "")
        {
            NotPlayModeException.Exception();
            NotMainThreadException.Exception();
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (string.IsNullOrWhiteSpace(language))
                language = GlobalData.currentLanguage;

            if (!isLoaded)
            {
                if (ResourceManager.FileExtensionExists(Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name, language),
                    out string outPath,
                    ExtensionFilter.textFileFilter))
                {
                    Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(outPath);
                    if (lang != null && lang.TryGetValue(key, out string value))
                    {
                        loadedLanguages.TryAdd(nameSpace, new());
                        loadedLanguages[nameSpace].TryAdd(language, new());
                        loadedLanguages[nameSpace][language].TryAdd(key, value);
                    }
                }
            }

            if (loadedLanguages.TryGetValue(nameSpace, out var result))
            {
                if (result.TryGetValue(language, out var result2))
                {
                    result2.TryGetValue(key, out string result3);
                    return result3;
                }
            }

            return null;
        }



        public void Clear() => tempLoadedLanguages.Clear();

        readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempLoadedLanguages = new();
        readonly List<string> tempLanguageTypes = new();
        public async UniTask Refresh(string nameSpacePath, string nameSpace)
        {
            NotPlayModeException.Exception();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(Thread);
            else
                Thread();

            void Thread()
            {
                string langPath = Path.Combine(nameSpacePath, name);
                if (!Directory.Exists(langPath))
                    return;

                string[] filePaths = DirectoryUtility.GetFiles(langPath, ExtensionFilter.textFileFilter);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filePath = filePaths[i];
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
            }
        }

        public void Apply()
        {
            loadedLanguages = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(tempLoadedLanguages);
            languageList = new List<string>(tempLanguageTypes);

            isLoaded = true;
        }
    }
}
