#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Jsons;
using RuniEngine.NBS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSLoader : IResourceElement
    {
        public const string nbsNameSpace = "runi-nbs";

        public bool isLoaded { get; private set; } = false;
        public ResourcePack? resourcePack { get; set; } = null;



        /// <summary>
        /// NBSData = allNBSs[nameSpace][key];
        /// </summary>
        Dictionary<string, Dictionary<string, NBSData>> allNBSes = new();



        public const string name = "nbses";
        string IResourceElement.name => name;



        public static NBSData? SearchNBSData(string path, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            NBSData? result = null;
            ResourceManager.ResourceElementLoop<NBSLoader>(x =>
            {
                if (x.allNBSes.TryGetValue(nameSpace, out var value) && value.TryGetValue(path, out NBSData value2))
                {
                    result = value2;
                    return true;
                }

                return false;
            });

            return result;
        }



        public static NBSFile? GetNBSFile(IOHandler ioHandler)
        {
            if (ioHandler.FileExists())
            {
                using System.IO.Stream stream = ioHandler.OpenRead();
                return NBSReader.ReadNBSFile(stream);
            }

            return null;
        }

        public static string[] GetNBSDataKeys(IOHandler ioHandler, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<NBSLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                if (x.allNBSes.ContainsKey(nameSpace))
                {
                    result = x.allNBSes[nameSpace].Keys.ToArray();
                    return true;
                }
                else
                    return false;
            }))
            {
                string path = PathUtility.Combine(ResourceManager.rootName, nameSpace, name);
                Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(ioHandler.CreateChild(path + ".json"));
                if (nbsDatas != null)
                    return nbsDatas.Keys.ToArray();
                else
                    return new string[0];
            }

            return result;
        }



        public UniTask Load() => Load(null);
        public async UniTask Load(IProgress<float>? progress)
        {
            if (resourcePack == null)
                return;

            await UniTask.SwitchToThreadPool();

            foreach (var item in allNBSes.SelectMany(x => x.Value).Select(x => x.Value))
                ResourceManager.RegisterGarbageResource(item);

            ConcurrentDictionary<string, NBSFile>? pathNBSes = new();
            ConcurrentDictionary<string, Dictionary<string, NBSData>> tempAllNBSes = new();

            long progressValue = 0;
            long maxProgress = 0;

            //진정한 병렬 로드
            Parallel.ForEach(resourcePack.nameSpaces, nameSpace =>
            {
                string folderPath = PathUtility.Combine(nameSpace, name);
                IOHandler root = resourcePack.ioHandler.CreateChild(folderPath);
                if (!root.DirectoryExists())
                    return;

                Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(root.AddExtension(".json"));
                if (nbsDatas == null)
                    return;

                tempAllNBSes.TryAdd(nameSpace, nbsDatas);

                string[] files = root.GetAllFiles(ExtensionFilter.nbsFileFilter).ToArray();
                Interlocked.Add(ref maxProgress, files.Length);

                Parallel.ForEach(files, nbsPath =>
                {
                    IOHandler nbsHandler = root.CreateChild(nbsPath);

                    NBSFile? nbsFile = GetNBSFile(nbsHandler);
                    if (nbsFile != null)
                        pathNBSes.TryAdd(PathUtility.GetPathWithoutExtension(nbsPath), nbsFile);

                    progress?.Report((float)Interlocked.Add(ref progressValue, 1) / Interlocked.Read(ref maxProgress));
                });
            });

            progress?.Report(1);

            //오디오 파일들을 오디오 데이터로 변환하는 후처리
            Dictionary<string, Dictionary<string, NBSData>> resultAllNBSes = new();
            foreach (var nbsDatas in tempAllNBSes)
            {
                string nameSpace = nbsDatas.Key;

                foreach (var nbsData in nbsDatas.Value)
                {
                    List<NBSMetaData> nbsMetaDatas = new List<NBSMetaData>();
                    for (int i = 0; i < nbsData.Value.nbses.Length; i++)
                    {
                        NBSMetaData? nbsMetaData = nbsData.Value.nbses[i];
                        if (!pathNBSes.TryGetValue(nbsMetaData.path, out NBSFile nbsFile))
                            continue;

#if ENABLE_RUNI_ENGINE_RHYTHMS
                        nbsMetaData = new NBSMetaData(nbsMetaData.path, nbsMetaData.pitch, nbsMetaData.tempo, nbsMetaData.bpmMultiplier, nbsMetaData.rhythmOffsetTick, nbsFile);
#else
                        nbsMetaData = new NBSMetaData(nbsMetaData.path, nbsMetaData.pitch, nbsMetaData.tempo, nbsFile);
#endif

                        if (nbsMetaData != null)
                            nbsMetaDatas.Add(nbsMetaData);
                    }

                    resultAllNBSes.TryAdd(nameSpace, new Dictionary<string, NBSData>());
                    resultAllNBSes[nameSpace].TryAdd(nbsData.Key, new NBSData(nbsData.Value.subtitle, nbsData.Value.isBGM, nbsMetaDatas.ToArray()));
                };
            };

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            allNBSes = resultAllNBSes;
            isLoaded = true;
        }

        public async UniTask Unload()
        {
            foreach (var item in allNBSes.SelectMany(x => x.Value).Select(x => x.Value))
            {
                item.Dispose();
                await UniTask.Yield();
            }

            allNBSes = new();
            isLoaded = false;

            await UniTask.CompletedTask;
        }
    }
}
