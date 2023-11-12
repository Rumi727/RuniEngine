#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using RuniEngine.Json;
using System.Linq;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        /// <summary>
        /// AudioData = allAudios[nameSpace][key];
        /// </summary>
        static Dictionary<string, Dictionary<string, AudioData>> allAudios = new();



        public const string name = "audios";
        string IResourceElement.name => name;



        [Awaken]
        static void Awaken() => ResourceManager.ElementRegister(new AudioLoader());



        public static AudioData? SearchAudioData(string path, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (allAudios.TryGetValue(nameSpace, out var value) && value.TryGetValue(path, out AudioData value2))
                return value2;

            return null;
        }



        public static async UniTask<AudioClip?> GetAudio(string path, AudioType type, bool stream = false, HideFlags hideFlags = HideFlags.DontSave)
        {
#if !((UNITY_STANDALONE_LINUX && !UNITY_EDITOR) || UNITY_EDITOR_LINUX)
            NotMainThreadException.Exception();

            if (File.Exists(path))
            {
                using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path.UrlPathPrefix(), type);
                DownloadHandlerAudioClip downloadHandlerAudioClip = (DownloadHandlerAudioClip)www.downloadHandler;
                downloadHandlerAudioClip.streamAudio = stream;

                await www.SendWebRequest();

                if (!Kernel.isPlaying)
                    return null;

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError(www.error);

                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = Path.GetFileNameWithoutExtension(path);
                audioClip.hideFlags = hideFlags;

                ResourceManager.allLoadedResources.Add(audioClip);
                return audioClip;
            }
#endif
            return null;
        }



        public async UniTask Load()
        {
            NotPlayModeException.Exception();
            Dictionary<string, Dictionary<string, AudioData>> tempAllAudios = new();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(() => ResourceManager.ResourcePackLoop(Thread));
            else
                await ResourceManager.ResourcePackLoop(Thread);

            foreach (var item2 in from item in allAudios from item2 in item.Value select item2)
            {
                for (int i = 0; i < item2.Value.audios.Length; i++)
                    ResourceManager.garbages.Add(item2.Value.audios[i].audioClip);
            }

            allAudios = tempAllAudios;

            async UniTask Thread(string nameSpacePath, string nameSpace)
            {
                string folderPath = Path.Combine(nameSpacePath, name);

                Dictionary<string, AudioData>? audioDatas = JsonManager.JsonRead<Dictionary<string, AudioData>>(folderPath + ".json");
                if (audioDatas == null)
                    return;

                foreach (var audioData in audioDatas)
                {
                    if (audioData.Value.audios == null)
                        continue;

                    List<AudioMetaData> audioMetaDatas = new List<AudioMetaData>();
                    for (int i = 0; i < audioData.Value.audios.Length; i++)
                    {
                        AudioMetaData? audioMetaData = audioData.Value.audios[i];
                        string audioPath = Path.Combine(folderPath, audioMetaData.path);

                        if (!ResourceManager.FileExtensionExists(audioPath, out audioPath, ExtensionFilter.musicFileFilter))
                            continue;

                        AudioType audioType;
                        switch (Path.GetExtension(audioPath))
                        {
                            case ".ogg":
                                audioType = AudioType.OGGVORBIS;
                                break;
                            case ".mp3":
                                audioType = AudioType.MPEG;
                                break;
                            case ".mp2":
                                audioType = AudioType.MPEG;
                                break;
                            case ".wav":
                                audioType = AudioType.WAV;
                                break;
                            case ".aiff":
                                audioType = AudioType.AIFF;
                                break;
                            case ".xm":
                                audioType = AudioType.XM;
                                break;
                            case ".mod":
                                audioType = AudioType.MOD;
                                break;
                            case ".it":
                                audioType = AudioType.IT;
                                break;
                            case ".vag":
                                audioType = AudioType.VAG;
                                break;
                            case ".xma":
                                audioType = AudioType.XMA;
                                break;
                            case ".s3m":
                                audioType = AudioType.S3M;
                                break;
                            default:
                                continue;
                        }

                        AudioClip? audioClip = await await ThreadDispatcher.Execute(() => GetAudio(audioPath, audioType, audioMetaData.stream));
                        if (!Kernel.isPlaying)
                            return;

                        if (audioClip != null)
                            audioMetaData = new AudioMetaData(audioMetaData.path, audioMetaData.pitch, audioMetaData.tempo, audioMetaData.stream, audioMetaData.loopStartTime, audioClip);

                        if (audioMetaData != null)
                            audioMetaDatas.Add(audioMetaData);
                    }

                    tempAllAudios.TryAdd(nameSpace, new Dictionary<string, AudioData>());
                    tempAllAudios[nameSpace].TryAdd(audioData.Key, new AudioData(audioData.Value.subtitle, audioData.Value.isBGM, audioMetaDatas.ToArray()));
                }
            }
        }
    }
}
