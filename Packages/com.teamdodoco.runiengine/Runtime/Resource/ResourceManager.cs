#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Accounts;
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

        public static void PackRegister(ResourcePack resourcePack)
        {
            if (resourcePack != ResourcePack.defaultPack)
                _resourcePacks.Add(resourcePack);
        }

        public static void PackUnregister(ResourcePack resourcePack) => _resourcePacks.Remove(resourcePack);

        public static async UniTask Refresh(IProgress<float>? progress = null)
        {
            NotMainThreadException.Exception();

            try
            {
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
                                try
                                {
                                    ThreadTask.Lock(ref progressLists.internalSync);

                                    progressLists.internalList[index] = y;
                                    progress.Report(progressLists.internalList.Sum() / x.resourceElements.Count);

                                }
                                finally
                                {
                                    ThreadTask.Unlock(ref progressLists.internalSync);
                                }
                            });

                            progressLists.Add(0);
                            progressIndex++;

                            cachedUniTasks.Add(item.Value.Load(progress2));
                        }
                        else
                            cachedUniTasks.Add(item.Value.Load());
                    }

                    return false;
                });

                await UniTask.WhenAll(cachedUniTasks);
            }
            finally
            {
                GarbageRemoval();
            }
        }

        /// <summary>
        /// 등록된 리소스팩을 등록 순서에 따라 루프합니다
        /// <para></para>
        /// <see langword="true"/>를 반환하면 true를 반환받은 시점에 리소스팩 루프를 중지합니다!!
        /// <para></para>
        /// 어떤 경우던 리소스팩을 전부 루프하고 싶은 경우에는 <see langword="false"/>"만" 반환해주세요
        /// </summary>
        /// <param name="func"></param>
        public static void ResourcePackLoop(Func<ResourcePack, bool> func)
        {
            //루프 안에서 리소스팩의 등록을 해제할 수 있기 때문에 리스트를 복사합니다
            ResourcePack[] resourcePacks = ResourceManager.resourcePacks.ToArray();

            //기본 에셋도 포함시켜야하기 때문에 리소스팩 길이를 1 늘린다
            for (int i = 0; i < resourcePacks.Length + 1; i++)
            {
                //현재 인덱스가 리소스팩의 길이를 벗어나면 기본 에셋으로 판정 (반복문이 리소스팩 길이 + 1 까지 반복하기 때문에 가능함)
                ResourcePack pack;
                if (i < resourcePacks.Length)
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
                    if (func.Invoke(pack))
                        return;
                }
                catch (Exception e)
                {
                    ExceptionLog(e, pack.path);
                }
            }
        }

        /// <summary>
        /// <see langword="true"/> 반환하면 리소스팩 루프를 중지합니다
        /// 즉, 리소스를 성공적으로 불러왔을 경우, 중복 할당을 방지하기 위해 <see langword="true"/> 반환하고 리소스를 찾지 못했다면 <see langword="false"/> 반환해주세요
        /// </summary>
        /// <typeparam name="T">리소스 요소</typeparam>
        /// <returns>
        /// 등록된 모든 리소스팩에서 요소가 전부 <see langword="false"/> 반환할 경우 <see langword="false"/> 반환합니다 (즉, 성공 여부입니다)
        /// </returns>
        public static bool ResourceElementLoop<T>(Func<T, bool> action) where T : IResourceElement
        {
            bool suc = false;
            ResourcePackLoop(x =>
            {
                try
                {
                    bool result = action.Invoke((T)x.resourceElements[typeof(T)]);
                    suc = suc || result;

                    return result;
                }
                catch (Exception e)
                {
                    ExceptionLog(e, x.path, typeof(T).Name);
                    return false;
                }
            });

            return suc;
        }

        public static void ExceptionLog(Exception e, string path) => ExceptionLog(e, path, Debug.NameOfCallingClass());

        public static void ExceptionLog(Exception e, string path, string? typeName)
        {
            Debug.LogException(e);
            Debug.ForceLogError(LanguageLoader.TryGetText("resource_manager.throw").Replace("{type}", typeName).Replace("{path}", path), nameof(ResourceManager));
        }

        public static void GarbageRemoval()
        {
            NotMainThreadException.Exception();

            for (int i = 0; i < garbages.Count; i++)
            {
                Object? resource = garbages[i];
                if (resource != null)
                    Object.DestroyImmediate(resource, true);
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
                    Object.DestroyImmediate(sprite, true);
                }
            }

            for (int i = 0; i < allLoadedResources.Count; i++)
            {
                Object? resource = allLoadedResources[i];
                if (resource != null)
                {
                    Debug.Log(resource);
                    Object.DestroyImmediate(resource, true);
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
            ResourcePackLoop(x =>
            {
                nameSpaces = nameSpaces.Union(x.nameSpaces);
                return false;
            });

            return nameSpaces.ToArray();
        }
    }
}
