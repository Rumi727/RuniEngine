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



        public static ThemeStyle? GetStyle(ResourcePack resourcePack, string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string rootPath = PathUtility.Combine(nameSpace, name, key);
                if (resourcePack.ioHandler.CreateChild(rootPath).FileExists(out IOHandler outHandler, ExtensionFilter.jsonFileFilter))
                    return JsonManager.JsonRead<ThemeStyle>(outHandler);

                return null;
            }
            else if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result) && result.TryGetValue(key, out ThemeStyle? result2))
                return result2;

            return null;
        }



        public static string[]? GetStyleKeys(ResourcePack resourcePack, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string rootPath = PathUtility.Combine(nameSpace, name);
                IOHandler rootHandler = resourcePack.ioHandler.CreateChild(rootPath);

                if (!rootHandler.DirectoryExists())
                    return null;

                return rootHandler.GetAllFiles(ExtensionFilter.jsonFileFilter).ToArray();
            }

            if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result))
                return result.Keys.ToArray();

            return null;
        }



        public static IOHandler? GetStylePath(ResourcePack resourcePack, string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            IOHandler rootHandler = resourcePack.ioHandler.CreateChild(PathUtility.Combine(nameSpace, name, key));
            if (rootHandler.FileExists(out IOHandler outPath, ExtensionFilter.jsonFileFilter))
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
                string rootPath = PathUtility.Combine(nameSpace, name);
                IOHandler root = resourcePack.ioHandler.CreateChild(rootPath);

                if (!root.DirectoryExists())
                {
                    ReportProgress();
                    continue;
                }

                foreach (string stylePath in root.GetAllFiles(ExtensionFilter.jsonFileFilter))
                {
                    IOHandler styleHandler = root.CreateChild(stylePath);
                    ThemeStyle? style = JsonManager.JsonRead<ThemeStyle>(styleHandler);
                    if (style == null)
                        continue;

                    tempAllStyles.TryAdd(nameSpace, new Dictionary<string, ThemeStyle>());
                    tempAllStyles[nameSpace].TryAdd(stylePath, style);
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
