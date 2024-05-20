#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RuniEngine.Jsons;
using RuniEngine.NBS;
using System.Linq;
using System;

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



        public static NBSFile? GetNBSFile(string path)
        {
            if (File.Exists(path))
                return NBSManager.ReadNBSFile(path);

            return null;
        }

        public static string[] GetNBSDataKeys(string nameSpace = "")
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
                string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(path + ".json");
                if (nbsDatas != null)
                    return nbsDatas.Keys.ToArray();
                else
                    return new string[0];
            }

            return result;
        }



        public async UniTask Load()
        {
            if (resourcePack == null)
                return;

            Dictionary<string, Dictionary<string, NBSData>> tempAllNBSes = new();

            if (ThreadTask.isMainThread)
                await UniTask.RunOnThreadPool(Thread);
            else
                await Thread();

            allNBSes = tempAllNBSes;
            isLoaded = true;

            async UniTask Thread()
            {
                for (int i = 0; i < resourcePack.nameSpaces.Count; i++)
                {
                    string nameSpace = resourcePack.nameSpaces[i];
                    string folderPath = Path.Combine(resourcePack.path, ResourceManager.rootName, nameSpace, name);

                    Dictionary<string, NBSData>? nbsDatas = JsonManager.JsonRead<Dictionary<string, NBSData>>(folderPath + ".json");
                    if (nbsDatas == null)
                        continue;

                    foreach (var nbsData in nbsDatas)
                    {
                        if (nbsData.Value.nbses == null)
                            continue;

                        List<NBSMetaData> nbsMetaDatas = new List<NBSMetaData>();
                        for (int j = 0; j < nbsData.Value.nbses.Length; j++)
                        {
                            NBSMetaData? nbsMetaData = nbsData.Value.nbses[j];
                            string nbsPath = Path.Combine(folderPath, nbsMetaData.path);

                            if (!ResourceManager.FileExtensionExists(nbsPath, out nbsPath, ExtensionFilter.nbsFileFilter))
                                return;

                            NBSFile? nbsFile = GetNBSFile(nbsPath);
                            if (nbsFile != null)
                                nbsMetaData = new NBSMetaData(nbsMetaData.path, nbsMetaData.pitch, nbsMetaData.tempo, nbsMetaData.stream, nbsFile);

                            if (nbsMetaData != null)
                                nbsMetaDatas.Add(nbsMetaData);
                        }

                        tempAllNBSes.TryAdd(nameSpace, new Dictionary<string, NBSData>());
                        tempAllNBSes[nameSpace].TryAdd(nbsData.Key, new NBSData(nbsData.Value.subtitle, nbsData.Value.isBGM, nbsMetaDatas.ToArray()));
                    }
                }

                await UniTask.CompletedTask;
            }
        }

        public async UniTask Unload()
        {
            allNBSes = new();
            isLoaded = false;

            await UniTask.CompletedTask;
        }
    }
}
