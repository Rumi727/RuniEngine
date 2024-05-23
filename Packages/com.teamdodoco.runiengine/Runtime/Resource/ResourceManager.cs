#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Datas;
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

        [UserData]
        public struct UserData
        {
            [JsonProperty] public static List<string> resourcePacks { get; set; } = new List<string>();
        }

        public const string rootName = "assets";
        public const string defaultNameSpace = "runi";



        public static IReadOnlyList<ResourcePack> resourcePacks => _resourcePacks;
        static readonly List<ResourcePack> _resourcePacks = new List<ResourcePack>();



        public static List<Object?> allLoadedResources { get; } = new();
        public static SynchronizedCollection<Object?> garbages { get; } = new SynchronizedCollection<Object?>();



#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeOnLoadMethod() => UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += AllDestroy;
#endif

        public static async UniTask<ResourcePack?> Load(string path, IProgress<float>? progress = null)
        {
            ResourcePack? resourcePack = ResourcePack.Create(path);
            if (resourcePack == null)
                return null;

            List<UniTask> cachedUniTasks = new List<UniTask>();
            SynchronizedCollection<float> progressLists = new SynchronizedCollection<float>();
            int progressIndex = 0;

            foreach (var item in resourcePack.resourceElements)
            {
                if (progress != null)
                {
                    int index = progressIndex;
                    IProgress<float> progress2 = Progress.Create<float>(x =>
                    {
                        progressLists[index] = x;
                        progress.Report(progressLists.Sum() / resourcePack.resourceElements.Count);
                    });

                    progressLists.Add(0);
                    progressIndex++;

                    cachedUniTasks.Add(item.Value.Load(progress2));
                }
                else
                    cachedUniTasks.Add(item.Value.Load());
            }

            await UniTask.WhenAll(cachedUniTasks);

            _resourcePacks.Add(resourcePack);
            return resourcePack;
        }

        public static async UniTask Unload(ResourcePack resourcePack)
        {
            _resourcePacks.Remove(resourcePack);
            
            List<UniTask> cachedUniTasks = new List<UniTask>();
            foreach (var item in resourcePack.resourceElements)
                cachedUniTasks.Add(item.Value.Unload());

            await UniTask.WhenAll(cachedUniTasks);
        }

        public static async UniTask Refresh(IProgress<float>? progress = null)
        {
            NotMainThreadException.Exception();

            List<UniTask> cachedUniTasks = new List<UniTask>();
            SynchronizedCollection<float> progressLists = new SynchronizedCollection<float>();
            int progressIndex = 0;

            ResourcePackLoop(x =>
            {
                foreach (var item in x.resourceElements)
                {
                    if (progress != null)
                    {
                        int index = progressIndex;
                        IProgress<float> progress2 = Progress.Create<float>(y =>
                        {
                            progressLists[index] = y;
                            progress.Report(progressLists.Sum() / x.resourceElements.Count);
                        });

                        progressLists.Add(0);
                        progressIndex++;

                        cachedUniTasks.Add(item.Value.Load(progress2));
                    }
                    else
                        cachedUniTasks.Add(item.Value.Load());
                }

                return true;
            }, out _);

            await UniTask.WhenAll(cachedUniTasks);
            GarbageRemoval();
        }

        public static bool ResourcePackLoop<T>(Func<ResourcePack, T> func, out T? result)
        {
            //기본 에셋도 포함시켜야하기 때문에 리소스팩 길이를 1 늘린다
            for (int i = 0; i < resourcePacks.Count + 1; i++)
            {
                //현재 인덱스가 리소스팩의 길이를 벗어나면 기본 에셋으로 판정 (반복문이 리소스팩 길이 + 1 까지 반복하기 때문에 가능함)
                ResourcePack pack;
                if (i < resourcePacks.Count)
                    pack = resourcePacks[i];
                else
                {
                    if (ResourcePack.defaultPack != null)
                        pack = ResourcePack.defaultPack;
                    else
                        continue;
                }

                try
                {
                    if (func != null)
                    {
                        result = func.Invoke(pack);
                        return true;
                    }

                    result = default;
                    return false;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.ForceLogError(LanguageLoader.TryGetText("resource_manager.throw").Replace("{type}", Debug.NameOfCallingClass()).Replace("{path}", pack.path), nameof(ResourceManager));
                }
            }

            result = default;
            return false;
        }

        public static bool ResourceElementLoop<T>(Func<T, bool> action) where T : IResourceElement
        {
            if (!ResourcePackLoop(x => action.Invoke((T)x.resourceElements[typeof(T)]), out bool result))
                return false;

            return result;
        }

        public static void GarbageRemoval()
        {
            NotMainThreadException.Exception();

            for (int i = 0; i < garbages.Count; i++)
            {
                Object? resource = garbages[i];
                if (resource != null)
                    Object.DestroyImmediate(resource);
            }

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object? resource = allLoadedResources[i];
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
                {
                    Debug.Log(sprite);
                    Object.DestroyImmediate(sprite);
                }
            }

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object? resource = allLoadedResources[i];
                if (resource != null)
                {
                    Debug.Log(resource);
                    Object.DestroyImmediate(resource);
                }
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
            IEnumerable<string> nameSpaces = new string[0];
            ResourcePackLoop(x => nameSpaces = nameSpaces.Union(x.nameSpaces), out _);

            return nameSpaces.ToArray();
        }
    }
}
