#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Jsons;
using RuniEngine.UI.Themes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuniEngine.Resource.Themes
{
    public sealed class ThemeLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        static Dictionary<string, Dictionary<string, ThemeStyle>> allStyles = new();



        public const string name = "styles";
        string IResourceElement.name => name;

        public ResourcePack? resourcePack { get; set; }



        public static ThemeStyle? GetStyle(IOHandler ioHandler, string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string path = PathUtility.Combine(ResourceManager.rootName, nameSpace, name, key);
                if (ResourceManager.FileExtensionExists(ioHandler, path, out string outPath, ExtensionFilter.jsonFileFilter))
                    return JsonManager.JsonRead<ThemeStyle>(outPath);

                return null;
            }
            else if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result) && result.TryGetValue(key, out ThemeStyle? result2))
                return result2;

            return null;
        }



        public static string[]? GetStyleKeys(IOHandler ioHandler, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string rootPath = PathUtility.Combine(ResourceManager.rootName, nameSpace, name);
                if (!ioHandler.DirectoryExists(rootPath))
                    return null;

                return ioHandler.GetAllFiles(rootPath, ExtensionFilter.jsonFileFilter).Select(x => PathUtility.GetPathWithoutExtension(x.Substring(rootPath.Length + 1))).ToArray();
            }

            if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result))
                return result.Keys.ToArray();

            return null;
        }



        public static string? GetStylePath(IOHandler ioHandler, string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string path = PathUtility.Combine(ResourceManager.rootName, nameSpace, name, key);
            if (ResourceManager.FileExtensionExists(ioHandler, path, out string outPath, ExtensionFilter.jsonFileFilter))
                return outPath;

            return null;
        }



        public UniTask Load() => Load(null);
        public async UniTask Load(IProgress<float>? progress)
        {
            if (resourcePack == null)
                return;

            NotPlayModeException.Exception();
            Dictionary<string, Dictionary<string, ThemeStyle>> tempAllStyles = new();

            await UniTask.SwitchToThreadPool();

            for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
            {
                string nameSpace = resourcePack.nameSpaces[i];
                string rootPath = PathUtility.Combine(ResourceManager.rootName, nameSpace, name);
                IOHandler root = resourcePack.ioHandler.CreateChild(rootPath);

                if (!root.DirectoryExists())
                {
                    ReportProgress();
                    continue;
                }

                foreach (string stylePath in root.GetAllFiles(ExtensionFilter.jsonFileFilter))
                {
                    ThemeStyle? style = JsonManager.JsonRead<ThemeStyle>(stylePath, "", root);
                    if (style == null)
                        continue;

                    string localStylePath = stylePath.Substring(rootPath.Length + 1).Replace("\\", "/");

                    tempAllStyles.TryAdd(nameSpace, new Dictionary<string, ThemeStyle>());
                    tempAllStyles[nameSpace].TryAdd(localStylePath, style);
                }

                ReportProgress();

                void ReportProgress() => progress?.Report((float)(i + 1) / resourcePack.nameSpaces.Count);
            }

            allStyles = tempAllStyles;
            isLoaded = true;
        }

        public async UniTask Unload()
        {
            allStyles.Clear();
            await UniTask.CompletedTask;
        }
    }
}
