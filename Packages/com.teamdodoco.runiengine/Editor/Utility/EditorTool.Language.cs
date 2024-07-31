#nullable enable
using RuniEngine.Datas;
using RuniEngine.Jsons;
using RuniEngine.Resource;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        [ProjectData]
        public struct ProjectData
        {
            public static string currentLanguage { get; set; } = "en_us";
        }

        public const string packageLanguagePath = packagePath + "/" + packageEditorPath + "/Languages";

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
                /* 
                 * 원래 에디터 모드에서는 저장 가능한 클래스는 null이 아니여도 외부에서 값이 바뀔 수 있으니 항상 불러와야하나
                 * 이 클래스는 외부에서 값을 바꿀 이유가 전혀 없기 때문에 최적화를 위해서 예외로 null 값일때만 불러오게 설정
                 */
                if (storableClass == null)
                {
                    storableClass = new StorableClass(typeof(ProjectData));
                    storableClass.AutoNameLoad(Kernel.projectSettingPath);
                }

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
