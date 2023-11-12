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

namespace RuniEngine.Resource
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



        public static async UniTask<AudioClip?> GetAudio(string path, bool pathExtensionUse = false, bool stream = false, HideFlags hideFlags = HideFlags.DontSave)
        {
#if !((UNITY_STANDALONE_LINUX && !UNITY_EDITOR) || UNITY_EDITOR_LINUX)
            NotMainThreadException.Exception();
            if (pathExtensionUse)
                path = PathUtility.GetPathWithExtension(path);


            AudioClip? audioClip = await GetFile(".ogg", AudioType.OGGVORBIS);
            if (audioClip == null)
                audioClip = await GetFile(".mp3", AudioType.MPEG);
            if (audioClip == null)
                audioClip = await GetFile(".mp2", AudioType.MPEG);
            if (audioClip == null)
                audioClip = await GetFile(".wav", AudioType.WAV);
            if (audioClip == null)
                audioClip = await GetFile(".aif", AudioType.AIFF);
            if (audioClip == null)
                audioClip = await GetFile(".xm", AudioType.XM);
            if (audioClip == null)
                audioClip = await GetFile(".mod", AudioType.MOD);
            if (audioClip == null)
                audioClip = await GetFile(".it", AudioType.IT);
            if (audioClip == null)
                audioClip = await GetFile(".vag", AudioType.VAG);
            if (audioClip == null)
                audioClip = await GetFile(".xma", AudioType.XMA);
            if (audioClip == null)
                audioClip = await GetFile(".s3m", AudioType.S3M);

            return audioClip;

            async UniTask<AudioClip?> GetFile(string extension, AudioType type)
            {
                if (File.Exists(path + extension))
                {
                    using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip((path + extension).UrlPathPrefix(), type);
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

                return null;
            }
#else
            return null;
#endif
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

            static async UniTask Thread(string nameSpacePath, string nameSpace)
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

                        AudioClip? audioClip = await await ThreadDispatcher.Execute(() => GetAudio(audioPath, false, audioMetaData.stream));
                        if (!Kernel.isPlaying)
                            return;

                        if (audioClip != null)
                            audioMetaData = new AudioMetaData(audioMetaData.path, audioMetaData.pitch, audioMetaData.tempo, audioMetaData.stream, audioMetaData.loopStartTime, audioClip);

                        if (audioMetaData != null)
                            audioMetaDatas.Add(audioMetaData);
                    }

                    allAudios.TryAdd(nameSpace, new Dictionary<string, AudioData>());
                    allAudios[nameSpace].TryAdd(audioData.Key, new AudioData(audioData.Value.subtitle, audioData.Value.isBGM, audioMetaDatas.ToArray()));
                }
            }
        }
    }
}
