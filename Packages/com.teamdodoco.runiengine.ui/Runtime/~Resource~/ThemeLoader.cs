#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Json;
using RuniEngine.Threading;
using RuniEngine.UI.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource.Themes
{
    public sealed class ThemeLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        static Dictionary<string, Dictionary<string, ThemeStyle>> allStyles = new();



        public const string name = "styles";
        string IResourceElement.name => name;



        [Awaken]
        static void Awaken() => ResourceManager.ElementRegister(new ThemeLoader());



        public static ThemeStyle? GetStyle(string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name, key);
                if (ResourceManager.FileExtensionExists(path, out string outPath, ExtensionFilter.jsonFileFilter))
                    return JsonManager.JsonRead<ThemeStyle>(outPath);

                return null;
            }
            else if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result) && result.TryGetValue(key, out ThemeStyle? result2))
                return result2;

            return null;
        }



        public static string[] GetStyleKeys(string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (!isLoaded)
            {
                string rootPath = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                if (!Directory.Exists(rootPath))
                    return Array.Empty<string>();

                string[] paths = DirectoryUtility.GetFiles(rootPath, ExtensionFilter.jsonFileFilter, SearchOption.AllDirectories);
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = paths[i].Substring(rootPath.Length + 1).Replace("\\", "/");

                return paths;
            }

            if (allStyles.TryGetValue(nameSpace, out Dictionary<string, ThemeStyle>? result))
                return result.Keys.ToArray();

            return Array.Empty<string>();
        }



        public static string? GetStylePath(string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name, key);
            if (ResourceManager.FileExtensionExists(path, out string outPath, ExtensionFilter.jsonFileFilter))
                return outPath;

            return null;
        }



        public async UniTask Load()
        {
            NotPlayModeException.Exception();
            Dictionary<string, Dictionary<string, ThemeStyle>> tempAllStyles = new();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(() => ResourceManager.ResourcePackLoop(Thread));
            else
                await ResourceManager.ResourcePackLoop(Thread);

            allStyles = tempAllStyles;
            isLoaded = true;

            async UniTask Thread(string nameSpacePath, string nameSpace)
            {
                string rootPath = Path.Combine(nameSpacePath, name);
                if (!Directory.Exists(rootPath))
                    return;

                string[] stylePaths = DirectoryUtility.GetFiles(rootPath, ExtensionFilter.jsonFileFilter, SearchOption.AllDirectories);
                for (int i = 0; i < stylePaths.Length; i++)
                {
                    string stylePath = stylePaths[i];
                    ThemeStyle? style = JsonManager.JsonRead<ThemeStyle>(stylePath);
                    if (style == null)
                        continue;

                    string localStylePath = stylePath.Substring(rootPath.Length + 1).Replace("\\", "/");

                    tempAllStyles.TryAdd(nameSpace, new Dictionary<string, ThemeStyle>());
                    tempAllStyles[nameSpace].TryAdd(localStylePath, style);
                }

                await UniTask.CompletedTask;
            }
        }
    }
}
