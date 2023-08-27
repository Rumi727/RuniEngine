#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Booting;
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
        public sealed class UserData
        {
            [JsonProperty] public static List<string> resourcePacks { get; set; } = new List<string>();
        }

        public const string rootName = "assets";
        public const string defaultNameSpace = "runi";

        public static List<Object> allLoadedResources { get; } = new();



        static List<IResourceElement> allResourceElements { get; } = new List<IResourceElement>();

        public static void ElementRegister(IResourceElement element) => allResourceElements.Add(element);
        public static void ElementUnregister(IResourceElement element) => allResourceElements.Remove(element);

        public static UniTask Refresh() => Refresh(allResourceElements);
        public static UniTask Refresh(params IResourceElement[] resourceElements) => Refresh((IList<IResourceElement>)resourceElements);

        public static async UniTask Refresh(IList<IResourceElement> resourceElements)
        {
            NotPlayModeException.Exception();
            NotMainThreadException.Exception();

            for (int i = 0; i < resourceElements.Count; i++)
                resourceElements[i].Clear();

            UniTask[] cachedUniTasks = new UniTask[resourceElements.Count];

            //기본 에셋도 포함시켜야하기 때문에 리소스팩 길이를 1 늘린다
            for (int i = 0; i < UserData.resourcePacks.Count + 1; i++)
            {
                //현재 인덱스가 리소스팩의 길이를 벗어나면 기본 에셋으로 판정 (반복문이 리소스팩 길이 + 1 까지 반복하기 때문에 가능함)
                string resourcePack;
                if (i < UserData.resourcePacks.Count)
                    resourcePack = UserData.resourcePacks[i];
                else
                    resourcePack = Kernel.streamingAssetsPath;

                string rootPath = Path.Combine(resourcePack, rootName);
                if (!Directory.Exists(rootPath))
                    continue;

                string[] paths = Directory.GetDirectories(rootPath);
                for (int j = 0; j < paths.Length; j++)
                {
                    string path = paths[j];
                    string nameSpace = Path.GetFileName(path);

                    for (int k = 0; k < resourceElements.Count; k++)
                    {
                        cachedUniTasks[k] = Method(path, nameSpace, k);
                        async UniTask Method(string nameSpacePath, string nameSpace, int index)
                        {
                            Type? type = null;

                            try
                            {
                                IResourceElement resourceElement = resourceElements[index];
                                type = resourceElement.GetType();

                                await resourceElement.Refresh(nameSpacePath, nameSpace);
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                                Debug.LogError($"{type?.Name} 리소스 요소가 \"{nameSpacePath}\" 네임스페이스에 있는 리소스들을 불러오다 예외가 발생 했습니다", nameof(ResourceManager));
                            }
                        }
                    }

                    await UniTask.WhenAll(cachedUniTasks);
                }
            }

            for (int i = 0; i < resourceElements.Count; i++)
                resourceElements[i].Apply();
        }



        /// <summary>
        /// 모든 리소스를 삭제합니다
        /// </summary>
        public static void AllDestroy()
        {
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
    }
}
