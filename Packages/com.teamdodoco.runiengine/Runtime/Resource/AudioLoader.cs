#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using RuniEngine.Json;
using RuniEngine.NBS;
using System.Xml.Linq;
using System;
using System.Threading;
using System.Linq;

namespace RuniEngine.Resource
{
    public sealed class AudioLoader : IResourceElement
    {
        public static bool isLoaded { get; private set; } = false;



        /// <summary>
        /// SoundData = allSounds[nameSpace][key];
        /// </summary>
        static Dictionary<string, Dictionary<string, SoundData>> allSounds = new();



        public const string name = "sounds";
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
            Dictionary<string, Dictionary<string, SoundData>> tempAllSounds = new();

            if (ThreadManager.isMainThread)
                await UniTask.RunOnThreadPool(() => ResourceManager.ResourcePackLoop(Thread));
            else
                await ResourceManager.ResourcePackLoop(Thread);

            foreach (var item2 in from item in allSounds from item2 in item.Value select item2)
            {
                for (int i = 0; i < item2.Value.sounds.Length; i++)
                    ResourceManager.garbages.Add(item2.Value.sounds[i].audioClip);
            }

            allSounds = tempAllSounds;

            static async UniTask Thread(string nameSpacePath, string nameSpace)
            {
                string folderPath = Path.Combine(nameSpacePath, name);

                Dictionary<string, SoundData>? soundDatas = JsonManager.JsonRead<Dictionary<string, SoundData>>(folderPath + ".json");
                if (soundDatas == null)
                    return;

                foreach (var soundData in soundDatas)
                {
                    if (soundData.Value.sounds == null)
                        continue;

                    List<SoundMetaData> soundMetaDatas = new List<SoundMetaData>();
                    for (int i = 0; i < soundData.Value.sounds.Length; i++)
                    {
                        SoundMetaData? soundMetaData = soundData.Value.sounds[i];
                        string audioPath = Path.Combine(folderPath, soundMetaData.path);

                        AudioClip? audioClip = await await ThreadDispatcher.Execute(() => GetAudio(audioPath, false, soundMetaData.stream));
                        if (!Kernel.isPlaying)
                            return;

                        if (audioClip != null)
                            soundMetaData = new SoundMetaData(soundMetaData.path, soundMetaData.pitch, soundMetaData.tempo, soundMetaData.stream, soundMetaData.loopStartTime, audioClip);

                        if (soundMetaData != null)
                            soundMetaDatas.Add(soundMetaData);
                    }

                    allSounds.TryAdd(nameSpace, new Dictionary<string, SoundData>());
                    allSounds[nameSpace].TryAdd(soundData.Key, new SoundData(soundData.Value.subtitle, soundData.Value.isBGM, soundMetaDatas.ToArray()));
                }
            }
        }
    }
}
