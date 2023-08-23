#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Json;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Resource
{
    public sealed class LanguageLoader : IResourceElement
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> loadedLanguages = new();

        public string name => "lang";

        [Awaken]
        public static void Awaken() => ResourceManager.ElementRegister(new LanguageLoader());

        public void Clear() => tempLanguages.Clear();

        Dictionary<string, Dictionary<string, Dictionary<string, string>>> tempLanguages = new();
        public async UniTask Refresh(string nameSpacePath, string nameSpace)
        {
            await UniTask.RunOnThreadPool(Thread);

            void Thread()
            {
                string langPath = Path.Combine(nameSpacePath, name);
                if (!Directory.Exists(langPath))
                    return;

                string[] filePaths = DirectoryUtility.GetFiles(langPath, ExtensionFilter.textFileFilter);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filePath = filePaths[i];
                    string fileName = Path.GetFileName(filePath);

                    Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(filePath);
                    if (lang == null)
                        continue;

                    foreach (var item in lang)
                    {
                        loadedLanguages.TryAdd(nameSpace, new Dictionary<string, Dictionary<string, string>>());
                        loadedLanguages[nameSpace].TryAdd(fileName, new Dictionary<string, string>());
                        loadedLanguages[nameSpace][fileName].TryAdd(item.Key, item.Value);
                    }
                }
            }
        }

        public void Apply() => loadedLanguages = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(tempLanguages);
    }
}
