#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Booting;
using RuniEngine.Data;
using RuniEngine.Resource.Texts;
using RuniEngine.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;

namespace RuniEngine.Resource
{
    public static class ResourceManager
    {
        public delegate UniTask RefreshDelegate(string nameSpacePath, string nameSpace);

        [GlobalData]
        public struct GlobalData
        {
            [JsonProperty] public static List<string> resourcePacks { get; set; } = new List<string>();
        }

        public const string rootName = "assets";
        public const string defaultNameSpace = "runi";

        public static List<Object> allLoadedResources { get; } = new();
        public static SynchronizedCollection<Object> garbages { get; } = new SynchronizedCollection<Object>();



        static List<IResourceElement> allResourceElements { get; } = new List<IResourceElement>();

        public static void ElementRegister(IResourceElement element) => allResourceElements.Add(element);
        public static void ElementUnregister(IResourceElement element) => allResourceElements.Remove(element);

        public static UniTask Refresh() => Refresh(allResourceElements);
        public static UniTask Refresh(params IResourceElement[] resourceElements) => Refresh((IList<IResourceElement>)resourceElements);

        public static async UniTask Refresh(IList<IResourceElement> resourceElements)
        {
            NotPlayModeException.Exception();
            NotMainThreadException.Exception();

            UniTask[] cachedUniTasks = new UniTask[resourceElements.Count];
            for (int i = 0; i < cachedUniTasks.Length; i++)
                cachedUniTasks[i] = resourceElements[i].Load();

            await UniTask.WhenAll(cachedUniTasks);
            if (!Kernel.isPlaying)
                return;

            GarbageRemoval();
        }

        public delegate UniTask ResourcePackLoopFunc(string nameSpacePath, string nameSpace);
        public static async UniTask ResourcePackLoop(ResourcePackLoopFunc func)
        {
            //기본 에셋도 포함시켜야하기 때문에 리소스팩 길이를 1 늘린다
            for (int i = 0; i < GlobalData.resourcePacks.Count + 1; i++)
            {
                //현재 인덱스가 리소스팩의 길이를 벗어나면 기본 에셋으로 판정 (반복문이 리소스팩 길이 + 1 까지 반복하기 때문에 가능함)
                string path;
                if (i < GlobalData.resourcePacks.Count)
                    path = GlobalData.resourcePacks[i];
                else
                    path = Kernel.streamingAssetsPath;

                path = Path.Combine(path, rootName);

                string[] nameSpaces = Directory.GetDirectories(path);
                for (int j = 0; j < nameSpaces.Length; j++)
                {
                    string nameSpacePath = nameSpaces[j];
                    try
                    {
                        await func.Invoke(nameSpacePath, Path.GetFileName(nameSpacePath));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.ForceLogError(LanguageLoader.TryGetText("resource_manager.throw").Replace("{type}", Debug.NameOfCallingClass()).Replace("{namespace}", nameSpacePath), nameof(ResourceManager));
                    }
                }
            }
        }

        public static void GarbageRemoval()
        {
            NotMainThreadException.Exception();

            for (int i = 0; i < garbages.Count; i++)
                Object.DestroyImmediate(garbages[i]);

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object resource = allLoadedResources[i];
                if (resource == null)
                {
                    allLoadedResources.RemoveAt(i);
                    i--;
                }
            }

            garbages.Clear();
            GC.Collect();
        }



        /// <summary>
        /// 모든 리소스를 삭제합니다
        /// </summary>
        public static void AllDestroy()
        {
            GarbageRemoval();

            List<Sprite> allLoadedSprite = allLoadedResources.OfType<Sprite>().ToList();
            for (int i = 0; i < allLoadedSprite.Count; i++)
            {
                Sprite sprite = allLoadedSprite[i];
                if (sprite != null)
                    Object.DestroyImmediate(sprite);
            }

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object resource = allLoadedResources[i];
                if (resource != null)
                    Object.DestroyImmediate(resource);
            }

            allLoadedResources.Clear();
        }



        public static void SetDefaultNameSpace(ref string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = defaultNameSpace;
        }

        /// <summary>
        /// 파일들에 특정 확장자가 있으면 true를 반환합니다
        /// Returns true if files have a specific extension
        /// </summary>
        /// <param name="path">
        /// 파일의 경로
        /// Path
        /// </param>
        /// <param name="outPath">
        /// 검색한 확장자를 포함한 전체 경로
        /// Full path including searched extension
        /// </param>
        /// <param name="extensions">
        /// 확장자 리스트
        /// extension list
        /// </param>
        /// <returns></returns>
        public static bool FileExtensionExists(string path, out string outPath, ExtensionFilter extensionFilter)
        {
            for (int i = 0; i < extensionFilter.extensions.Length; i++)
            {
                string extension = extensionFilter.extensions[i];
                if (File.Exists(path + extension))
                {
                    outPath = path + extension;
                    return true;
                }
            }

            outPath = "";
            return false;
        }

        /// <summary>
        /// 텍스트에서 네임스페이스를 분리하고 네임스페이스를 반환합니다.
        /// Detach a namespace from text and return the namespace.
        /// </summary>
        /// <param name="text">
        /// 분리할 텍스트
        /// text to split
        /// </param>
        /// <param name="value">
        /// 분리되고 남은 텍스트
        /// Remaining Text
        /// </param>
        /// <returns></returns>
        public static string GetNameSpace(string text, out string value)
        {
            if (text == null)
            {
                value = "";
                return "";
            }

            if (text.Contains(":"))
            {
                int index = text.IndexOf(":");
                value = text.Substring(index + 1);

                return text.Remove(index);
            }
            else
            {
                value = text;
                return "";
            }
        }

        /// <summary>
        /// 텍스트에서 텍스쳐 타입을 분리하고 타입을 반환합니다.
        /// Detach a namespace from text and return the namespace.
        /// </summary>
        /// <param name="text">
        /// 분리할 텍스트
        /// text to split
        /// </param>
        /// <param name="value">
        /// 분리되고 남은 텍스트
        /// Remaining Text
        /// </param>
        /// <returns></returns>
        public static string GetTextureType(string text, out string value)
        {
            if (text == null)
            {
                value = "";
                return "";
            }

            if (text.Contains("/"))
            {
                int index = text.LastIndexOf("/");
                value = text.Substring(index + 1);

                return text.Remove(index);
            }
            else
            {
                value = text;
                return "";
            }
        }

        /// <summary>
        /// 리소스팩에서 네임스페이스 리스트를 가져옵니다
        /// </summary>
        /// <returns></returns>
        public static string[] GetNameSpaces()
        {
            if (BootLoader.basicDataLoaded)
            {
                IEnumerable<string> nameSpaces = new string[0];

                //기본 에셋도 포함시켜야하기 때문에 리소스팩 길이를 1 늘린다
                for (int i = 0; i < GlobalData.resourcePacks.Count + 1; i++)
                {
                    //현재 인덱스가 리소스팩의 길이를 벗어나면 기본 에셋으로 판정 (반복문이 리소스팩 길이 + 1 까지 반복하기 때문에 가능함)
                    string path;
                    if (i < GlobalData.resourcePacks.Count)
                        path = GlobalData.resourcePacks[i];
                    else
                        path = Kernel.streamingAssetsPath;

                    nameSpaces = nameSpaces.Union(GetResult(path));
                }

                return nameSpaces.ToArray();
            }
            else
                return GetResult(Kernel.streamingAssetsPath);

            static string[] GetResult(string path)
            {
                path = Path.Combine(path, rootName);

                string[] nameSpaces = Directory.GetDirectories(path);
                for (int j = 0; j < nameSpaces.Length; j++)
                    nameSpaces[j] = Path.GetFileName(nameSpaces[j]);

                return nameSpaces;
            }
        }
    }
}
