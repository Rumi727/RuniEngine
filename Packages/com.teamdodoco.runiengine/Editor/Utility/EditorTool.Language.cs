#nullable enable
using Newtonsoft.Json;
using RuniEngine.Data;
using RuniEngine.Json;
using RuniEngine.Resource;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        [ProjectData]
        public struct ProjectData
        {
            [JsonProperty] public static string currentLanguage { get; set; } = "en_us";
        }

        public const string packageLanguagePath = packageEditorPath + "/Languages";

        /// <summary>
        /// value = loadedLanguages[language][key];
        /// </summary>
        static readonly Dictionary<string, Dictionary<string, string>> loadedLanguages = new();

        public static string TryGetText(string key, string lauguage = "")
        {
            string? result = GetText(key, lauguage);
            if (result == null)
                return key;

            return result;
        }

        static StorableClass? storableClass;
        public static string? GetText(string key, string language = "")
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                storableClass ??= new StorableClass(typeof(ProjectData));
                storableClass.AutoNameLoad(Kernel.projectDataPath);

                language = ProjectData.currentLanguage;
            }

            if (loadedLanguages.TryGetValue(language, out var result) && result.TryGetValue(key, out string value))
                return value;
            else if (ResourceManager.FileExtensionExists(Path.Combine(packageLanguagePath, language), out string outPath, ExtensionFilter.textFileFilter))
            {
                Dictionary<string, string>? lang = JsonManager.JsonRead<Dictionary<string, string>>(outPath);
                if (lang == null)
                    return null;

                if (lang.TryGetValue(key, out value))
                {
                    loadedLanguages.TryAdd(language, new());
                    loadedLanguages[language].TryAdd(key, value);

                    return value;
                }
            }

            return null;
        }
    }
}
