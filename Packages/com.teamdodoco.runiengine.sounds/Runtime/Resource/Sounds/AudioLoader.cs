using Cysharp.Threading.Tasks;
using NAudio.Vorbis;
using NAudio.Wave;
using OggVorbis;
using RuniEngine.Booting;
using RuniEngine.Jsons;
using RuniEngine.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioLoader : IResourceElement
    {
        public const string soundsNameSpace = "runi-sounds";

        public static int bufferLength { get => Interlocked.Add(ref _bufferLength, 0); }
        static int _bufferLength;

        public static int systemFrequency { get => Interlocked.Add(ref _systemFrequency, 0); }
        static int _systemFrequency = 48000;

        public static int systemChannels { get => Interlocked.Add(ref _systemChannels, 0); }
        static int _systemChannels = 2;

        public static AudioListener? audioListener
        {
            get
            {
                if (_audioListener == null || !_audioListener.isActiveAndEnabled)
                    _audioListener = Object.FindFirstObjectByType<AudioListener>();

                return _audioListener;
            }
        }
        static AudioListener? _audioListener;



        public static ConcurrentBag<RawAudioClipLoaderBase> loaders { get; } = new();



        public bool isLoaded { get; private set; } = false;
        public ResourcePack? resourcePack { get; set; } = null;



        /// <summary>
        /// AudioData = allAudios[nameSpace][key];
        /// </summary>
        Dictionary<string, Dictionary<string, AudioData>> allAudios = new();



        public const string name = "audios";
        string IResourceElement.name => name;



#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            OnAudioConfigurationChanged(false);
            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        }
#endif



#if UNITY_EDITOR
        [Awaken]
        static void Awaken() => AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
#endif

        static void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            Interlocked.Exchange(ref _systemFrequency, AudioSettings.outputSampleRate);

            AudioSettings.GetDSPBufferSize(out int bufferLength, out _);

            int driverChannels = 1;
            switch (AudioSettings.driverCapabilities)
            {
                case AudioSpeakerMode.Mono:
                    driverChannels = 1;
                    break;
                case AudioSpeakerMode.Stereo:
                    driverChannels = 2;
                    break;
                case AudioSpeakerMode.Quad:
                    driverChannels = 4;
                    break;
                case AudioSpeakerMode.Surround:
                    driverChannels = 5;
                    break;
                case AudioSpeakerMode.Mode5point1:
                    driverChannels = 6;
                    break;
                case AudioSpeakerMode.Mode7point1:
                    driverChannels = 8;
                    break;
                case AudioSpeakerMode.Prologic:
                    driverChannels = 2;
                    break;
            }

            int systemChannels = 1;
            switch (AudioSettings.speakerMode)
            {
                case AudioSpeakerMode.Mono:
                    systemChannels = 1;
                    break;
                case AudioSpeakerMode.Stereo:
                    systemChannels = 2;
                    break;
                case AudioSpeakerMode.Quad:
                    systemChannels = 4;
                    break;
                case AudioSpeakerMode.Surround:
                    systemChannels = 5;
                    break;
                case AudioSpeakerMode.Mode5point1:
                    systemChannels = 6;
                    break;
                case AudioSpeakerMode.Mode7point1:
                    systemChannels = 8;
                    break;
                case AudioSpeakerMode.Prologic:
                    systemChannels = 2;
                    break;
            }

            Interlocked.Exchange(ref _systemChannels, systemChannels.Min(driverChannels));
            Interlocked.Exchange(ref _bufferLength, bufferLength * _systemChannels);
        }

        public static AudioData? SearchAudioData(string path, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            AudioData? result = null;
            ResourceManager.ResourceElementLoop<AudioLoader>(x =>
            {
                if (x.allAudios.TryGetValue(nameSpace, out var value) && value.TryGetValue(path, out AudioData value2))
                {
                    result = value2;
                    return true;
                }

                return false;
            });

            return result;
        }



        public static RawAudioClip? GetRawAudio(string path, RawAudioLoadType loadType)
        {
            if (!File.Exists(path))
                return null;

            WaveStream stream;
            if (path.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
                stream = new VorbisWaveReader(path);
            else
                stream = new AudioFileReader(path);

            try
            {
                return new RawAudioClip(stream, loadType);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            /*if (path.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
            {
                using VorbisReader reader = new VorbisReader(path);

                try
                {
                    int channels = reader.Channels;
                    int frequency = reader.SampleRate;
                    long samples = reader.TotalSamples;

                    float[] buffer = new float[frequency * channels];
                    float[] datas = new float[samples * channels];

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Debug.Log("start : " + Path.GetFileName(path));

                    int readSampleLength;
                    while ((readSampleLength = reader.ReadSamples(buffer, 0, buffer.Length)) > 0)
                    {
                        long curPosition = (reader.SamplePosition * channels) - readSampleLength;
                        for (int i = 0; i < readSampleLength; i++)
                            datas[curPosition + i] = buffer[i];
                    }

                    Debug.Log("end : " + Path.GetFileName(path));
                    Debug.Log(stopwatch.Elapsed);
                    stopwatch.Stop();

                    return new RawAudioClip(datas, frequency, channels);
                }
                catch (Exception e)
                {
                    Debug.Log(path);
                    Debug.LogException(e);
                }
            }
            else
            {
                int channels;
                int frequency;
                long samples;

                float[] buffer;
                float[] datas;

                WaveStream byteReader = new AudioFileReader(path);
                ISampleProvider reader = byteReader.ToSampleProvider();

                try
                {
                    channels = byteReader.WaveFormat.Channels;
                    frequency = byteReader.WaveFormat.SampleRate;
                    samples = byteReader.Length;

                    buffer = new float[channels * frequency];
                    List<float> dataLists = new List<float>((int)samples.Clamp(0, int.MaxValue));

                    int position = 0;
                    int readSampleLength;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while ((readSampleLength = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataLists.AddRange(buffer);
                        position += readSampleLength;
                    }

                    samples = position;
                    datas = dataLists.ToArray();

                    stopwatch.Stop();

                    return new RawAudioClip(datas, frequency, channels);
                }
                catch (Exception e)
                {
                    Debug.Log(path);
                    Debug.LogException(e);
                }
                finally
                {
                    byteReader.Dispose();
                }
            }*/

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

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    return null;
                }

                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = Path.GetFileNameWithoutExtension(path);
                audioClip.hideFlags = hideFlags;

                ResourceManager.RegisterManagedResource(audioClip);
                return audioClip;
            }
#endif
            return null;
        }



        public static string[] GetSoundDataKeys(string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            string[] result = Array.Empty<string>();
            if (!ResourceManager.ResourceElementLoop<AudioLoader>(x =>
            {
                if (!x.isLoaded)
                    return false;

                if (x.allAudios.ContainsKey(nameSpace))
                {
                    result = x.allAudios[nameSpace].Keys.ToArray();
                    return true;
                }
                else
                    return false;
            }))
            {
                string path = Path.Combine(Kernel.streamingAssetsPath, ResourceManager.rootName, nameSpace, name);
                Dictionary<string, AudioData>? audioDatas = JsonManager.JsonRead<Dictionary<string, AudioData>>(path + ".json");
                if (audioDatas != null)
                    result = audioDatas.Keys.ToArray();
            }

            return result;
        }



        public UniTask Load() => Load(null);
        public async UniTask Load(IProgress<float>? progress)
        {
            if (resourcePack == null)
                return;

            await UniTask.SwitchToThreadPool();

            ConcurrentDictionary<string, RawAudioClip>? pathAudios = new();
            ConcurrentDictionary<string, Dictionary<string, AudioData>> tempAllAudios = new();

            long progressValue = 0;
            long maxProgress = 0;

            //진정한 병렬 로드
            Parallel.ForEach(resourcePack.nameSpaces, nameSpace =>
            {
                string folderPath = Path.Combine(resourcePack.path, ResourceManager.rootName, nameSpace, name);
                if (!Directory.Exists(folderPath))
                    return;

                Dictionary<string, AudioData>? audioDatas = JsonManager.JsonRead<Dictionary<string, AudioData>>(folderPath + ".json");
                if (audioDatas == null)
                    return;

                tempAllAudios.TryAdd(nameSpace, audioDatas);

                string[] files = DirectoryUtility.GetFiles(folderPath, ExtensionFilter.musicFileFilter, SearchOption.AllDirectories);
                Interlocked.Add(ref maxProgress, files.Length);

                Parallel.ForEach(files, audioPath =>
                {
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

                    AudioFileMetaData? metaData = JsonManager.JsonRead<AudioFileMetaData>(audioPath + ".json");
                    RawAudioLoadType loadType = RawAudioLoadType.instant;
                    if (metaData != null)
                        loadType = metaData.Value.loadType;

                    RawAudioClip? rawAudioClip = null;
                    if (audioType == AudioType.OGGVORBIS || audioType == AudioType.MPEG || audioType == AudioType.AIFF)
                        rawAudioClip = GetRawAudio(audioPath, loadType);
                    /*else

                    var awaiter = ThreadDispatcher.Execute(Load).GetAwaiter();
                    while (!awaiter.IsCompleted)
                        Thread.Yield();

                    var awaiter2 = awaiter.GetResult().GetAwaiter();
                    while (!awaiter2.IsCompleted)
                        Thread.Yield();

                    async UniTask<RawAudioClip?> Load()
                    {
                        AudioClip? audioClip = await GetAudio(audioPat
                    h, audioType);
                        if (audioClip != null)
                        {
                            rawAudioClip = new RawAudioClip(audioClip);
                            Object.DestroyImmediate(audioClip);
                        }

                        return rawAudioClip;
                    }*/

                    if (rawAudioClip != null)
                        pathAudios.TryAdd(PathUtility.GetPathWithExtension(PathUtility.GetRelativePath(folderPath, audioPath)), rawAudioClip);

                    progress?.Report((float)Interlocked.Add(ref progressValue, 1) / Interlocked.Read(ref maxProgress));
                });
            });

            progress?.Report(1);

            //오디오 파일들을 오디오 데이터로 변환하는 후처리
            Dictionary<string, Dictionary<string, AudioData>> resultAllAudios = new();
            foreach (var audioDatas in tempAllAudios)
            {
                string nameSpace = audioDatas.Key;
                string folderPath = Path.Combine(resourcePack.path, ResourceManager.rootName, nameSpace, name);

                foreach (var audioData in audioDatas.Value)
                {
                    List<AudioMetaData> audioMetaDatas = new List<AudioMetaData>();
                    for (int i = 0; i < audioData.Value.audios.Length; i++)
                    {
                        AudioMetaData? audioMetaData = audioData.Value.audios[i];
                        if (!pathAudios.TryGetValue(audioMetaData.path, out RawAudioClip rawAudioClip))
                            continue;

#if ENABLE_RUNI_ENGINE_RHYTHMS
                        audioMetaData = new AudioMetaData(audioMetaData.path, audioMetaData.pitch, audioMetaData.tempo, audioMetaData.loopStartIndex, audioMetaData.loopOffsetIndex, audioMetaData.bpms, audioMetaData.rhythmOffsetIndex, rawAudioClip);
#else
                        audioMetaData = new AudioMetaData(audioMetaData.path, audioMetaData.pitch, audioMetaData.tempo, audioMetaData.loopStartIndex, audioMetaData.loopOffsetIndex, rawAudioClip);
#endif

                        if (audioMetaData != null)
                            audioMetaDatas.Add(audioMetaData);
                    }

                    resultAllAudios.TryAdd(nameSpace, new Dictionary<string, AudioData>());
                    resultAllAudios[nameSpace].TryAdd(audioData.Key, new AudioData(audioData.Value.subtitle, audioData.Value.isBGM, audioMetaDatas.ToArray()));
                };
            };

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Initialization);

            allAudios = resultAllAudios;
            isLoaded = true;
        }

        public async UniTask Unload()
        {
            allAudios = new();
            isLoaded = false;

            await UniTask.CompletedTask;
        }
    }
}
