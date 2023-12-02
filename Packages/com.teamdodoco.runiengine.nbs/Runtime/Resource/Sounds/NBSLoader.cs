#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RuniEngine.Json;
using RuniEngine.NBS;
using System.Linq;
using RuniEngine.Pooling;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        /// <summary>
        /// NBSData = allNBSs[nameSpace][key];
        /// </summary>
        static Dictionary<string, Dictionary<string, NBSData>> allNBSes = new();



        public const string name = "nbses";
        string IResourceElement.name => name;



        [Awaken]
        static void Awaken()
        {
            ResourceManager.ElementRegister(new NBSLoader());
            ObjectPoolingManager.ProjectData.prefabList.TryAdd("nbs_player.prefab", "Prefab/NBS Player");
        }

        public static NBSData? SearchNBSData(string path, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (allNBSes.TryGetValue(nameSpace, out var value) && value.TryGetValue(path, out NBSData value2))
                return value2;

            return null;
        }



        public static NBSFile? GetNBSFile(string path)
        {
            if (File.Exists(path))
                return NBSManager.ReadNBSFile(path);

            return null;
        }

        public static string[] GetSoundDataKeys(string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (Kernel.isPlaying && BootLoader.allLoaded)
            {
                if (allNBSes.ContainsKey(nameSpace))
                    return allNBSes[nameSpace].Keys.ToArray();
                else
                    return new string[0];
            }
            else
            {
                string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(path + ".json");
                if (nbsDatas != null)
                    return nbsDatas.Keys.ToArray();
                else
                    return new string[0];
            }
        }



        public async UniTask Load()
        {
            NotPlayModeException.Exception();
            Dictionary<string, Dictionary<string, NBSData>> tempAllNBSes = new();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(() => ResourceManager.ResourcePackLoop(Thread));
            else
                await ResourceManager.ResourcePackLoop(Thread);

            allNBSes = tempAllNBSes;

            async UniTask Thread(string nameSpacePath, string nameSpace)
            {
                string folderPath = Path.Combine(nameSpacePath, name);

                Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(folderPath + ".json");
                if (nbsDatas == null)
                    return;

                foreach (var nbsData in nbsDatas)
                {
                    if (nbsData.Value.nbses == null)
                        continue;

                    List<NBSMetaData> nbsMetaDatas = new List<NBSMetaData>();
                    for (int i = 0; i < nbsData.Value.nbses.Length; i++)
                    {
                        NBSMetaData? nbsMetaData = nbsData.Value.nbses[i];
                        string nbsPath = Path.Combine(folderPath, nbsMetaData.path);

                        if (!ResourceManager.FileExtensionExists(nbsPath, out nbsPath, ExtensionFilter.nbsFileFilter))
                            return;
                        
                        NBSFile? nbsFile = GetNBSFile(nbsPath);
                        if (!Kernel.isPlaying)
                            return;

                        if (nbsFile != null)
                            nbsMetaData = new NBSMetaData(nbsMetaData.path, nbsMetaData.pitch, nbsMetaData.tempo, nbsMetaData.stream, nbsFile);

                        if (nbsMetaData != null)
                            nbsMetaDatas.Add(nbsMetaData);
                    }

                    tempAllNBSes.TryAdd(nameSpace, new Dictionary<string, NBSData>());
                    tempAllNBSes[nameSpace].TryAdd(nbsData.Key, new NBSData(nbsData.Value.subtitle, nbsData.Value.isBGM, nbsMetaDatas.ToArray()));
                }

                await UniTask.CompletedTask;
            }
        }
    }
}
