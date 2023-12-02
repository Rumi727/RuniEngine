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
using System.Threading;
using OggVorbis;
using RuniEngine.Pooling;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;

        public static int systemFrequency { get => Interlocked.Add(ref _systemFrequency, 0); }
        static int _systemFrequency = 4800;

        public static int systemChannels { get => Interlocked.Add(ref _systemChannels, 0); }
        static int _systemChannels = 2;

        public static AudioListener? audioListener
        {
            get
            {
                if (_audioListener == null || _audioListener.isActiveAndEnabled)
                    _audioListener = Object.FindFirstObjectByType<AudioListener>();

                return _audioListener;
            }
        }

        static AudioListener? _audioListener;



        /// <summary>
        /// AudioData = allAudios[nameSpace][key];
        /// </summary>
        static Dictionary<string, Dictionary<string, AudioData>> allAudios = new();



        public const string name = "audios";
        string IResourceElement.name => name;



        [Awaken]
        static void Awaken()
        {
            ResourceManager.ElementRegister(new AudioLoader());
            Interlocked.Exchange(ref _systemFrequency, AudioSettings.outputSampleRate);
            
            switch (AudioSettings.speakerMode)
            {
                case AudioSpeakerMode.Mono:
                    Interlocked.Exchange(ref _systemChannels, 1);
                    break;
                case AudioSpeakerMode.Stereo:
                    Interlocked.Exchange(ref _systemChannels, 2);
                    break;
                case AudioSpeakerMode.Quad:
                    Interlocked.Exchange(ref _systemChannels, 4);
                    break;
                case AudioSpeakerMode.Surround:
                    Interlocked.Exchange(ref _systemChannels, 5);
                    break;
                case AudioSpeakerMode.Mode5point1:
                    Interlocked.Exchange(ref _systemChannels, 6);
                    break;
                case AudioSpeakerMode.Mode7point1:
                    Interlocked.Exchange(ref _systemChannels, 8);
                    break;
                case AudioSpeakerMode.Prologic:
                    Interlocked.Exchange(ref _systemChannels, 2);
                    break;
            }
        }

        [Starten]
        static void Starten() => ObjectPoolingManager.ProjectData.prefabList.TryAdd("audio_player.prefab", "Prefab/Audio Player");

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
            if (File.Exists(path))
            {
                if (type == AudioType.OGGVORBIS && !stream)
                    return await VorbisPlugin.ToAudioClipAsync(await File.ReadAllBytesAsync(path), Path.GetFileNameWithoutExtension(path));

                NotMainThreadException.Exception();

                using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path.UrlPathPrefix(), type);
                DownloadHandlerAudioClip downloadHandlerAudioClip = (DownloadHandlerAudioClip)www.downloadHandler;
                downloadHandlerAudioClip.streamAudio = stream;

                await www.SendWebRequest();

                if (!Kernel.isPlaying)
                    return null;

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    return null;
                }

                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = Path.GetFileNameWithoutExtension(path);
                audioClip.hideFlags = hideFlags;

                ResourceManager.allLoadedResources.Add(audioClip);
                return audioClip;
            }
#endif
            return null;
        }



        public static string[] GetSoundDataKeys(string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (Kernel.isPlaying && BootLoader.allLoaded)
            {
                if (allAudios.ContainsKey(nameSpace))
                    return allAudios[nameSpace].Keys.ToArray();
                else
                    return new string[0];
            }
            else
            {
                string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                Dictionary<string, AudioData>? audioDatas = JsonManager.JsonRead<Dictionary<string, AudioData>>(path + ".json");
                if (audioDatas != null)
                    return audioDatas.Keys.ToArray();
                else
                    return new string[0];
            }
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

                List<UniTask> tasks = new List<UniTask>();
                foreach (var audioData in audioDatas)
                {
                    tasks.Add(Task());

                    //병렬 로드
                    async UniTask Task()
                    {
                        if (audioData.Value.audios == null)
                            return;

                        List<UniTask> tasks2 = new List<UniTask>();
                        List<AudioMetaData> audioMetaDatas = new List<AudioMetaData>();

                        for (int i = 0; i < audioData.Value.audios.Length; i++)
                        {
                            tasks2.Add(Task2());

                            //병렬 로드 2
                            async UniTask Task2()
                            {
                                AudioMetaData? audioMetaData = audioData.Value.audios[i];
                                string audioPath = Path.Combine(folderPath, audioMetaData.path);

                                if (!ResourceManager.FileExtensionExists(audioPath, out audioPath, ExtensionFilter.musicFileFilter))
                                    return;

                                AudioType audioType = Path.GetExtension(audioPath).ToLower() switch
                                {
                                    ".ogg" => AudioType.OGGVORBIS,
                                    ".mp3" => AudioType.MPEG,
                                    ".mp2" => AudioType.MPEG,
                                    ".wav" => AudioType.WAV,
                                    ".aiff" => AudioType.AIFF,
                                    ".xm" => AudioType.XM,
                                    ".mod" => AudioType.MOD,
                                    ".it" => AudioType.IT,
                                    ".vag" => AudioType.VAG,
                                    ".xma" => AudioType.XMA,
                                    ".s3m" => AudioType.S3M,
                                    _ => AudioType.UNKNOWN,
                                };

                                AudioClip? audioClip = await await ThreadDispatcher.Execute(() => GetAudio(audioPath, audioType, audioMetaData.stream));
                                if (!Kernel.isPlaying)
                                    return;

                                if (audioClip != null)
                                    audioMetaData = new AudioMetaData(audioMetaData.path, audioMetaData.pitch, audioMetaData.tempo, audioMetaData.stream, audioMetaData.loopStartIndex, audioMetaData.loopOffsetIndex, audioClip);

                                if (audioMetaData != null)
                                    audioMetaDatas.Add(audioMetaData);
                            }
                        }

                        await UniTask.WhenAll(tasks2);

                        tempAllAudios.TryAdd(nameSpace, new Dictionary<string, AudioData>());
                        tempAllAudios[nameSpace].TryAdd(audioData.Key, new AudioData(audioData.Value.subtitle, audioData.Value.isBGM, audioMetaDatas.ToArray()));
                    }
                }

                await UniTask.WhenAll(tasks);
            }
        }
    }
}
