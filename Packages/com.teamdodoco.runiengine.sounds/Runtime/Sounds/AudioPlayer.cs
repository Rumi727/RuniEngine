#nullable enable
using RuniEngine.Booting;
using RuniEngine.Pooling;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using RuniEngine.Threading;
using System;
using System.Threading;
using UnityEngine;

using Random = UnityEngine.Random;

namespace RuniEngine.Sounds
{
    [RequireComponent(typeof(AudioSource)), RequireComponent(typeof(AudioLowPassFilter))]
    public sealed class AudioPlayer : SoundPlayerBase
    {
        public override string objectKey => "audio_player.prefab";



        AudioSource? _audioSource;
        public AudioSource? audioSource => _audioSource = this.GetComponentFieldSave(_audioSource);

        AudioLowPassFilter? _audioLowPassFilter;
        public AudioLowPassFilter? audioLowPassFilter => _audioLowPassFilter = this.GetComponentFieldSave(_audioLowPassFilter);



        public override SoundData? soundData => audioData;
        public override SoundMetaDataBase? soundMetaData => audioMetaData;

        public AudioData? audioData { get; private set; }
        public AudioMetaData? audioMetaData { get; private set; }



        public float[]? datas => audioMetaData?.datas;



        public int frequency => audioMetaData != null ? audioMetaData.frequency : 0;

        public int channels => audioMetaData != null ? audioMetaData.channels : 0;


        public override double time
        {
            get => sampleIndex / (frequency != 0 ? frequency : AudioLoader.systemFrequency);
            set => sampleIndex = value * (frequency != 0 ? frequency : AudioLoader.systemFrequency);
        }

        public double sampleIndex
        {
            get => Interlocked.CompareExchange(ref _sampleIndex, 0, 0);
            set
            {
                if (sampleIndex != value)
                {
                    Interlocked.Exchange(ref _sampleIndex, value);
                    Interlocked.Exchange(ref realSampleIndex, value);

                    Interlocked.Exchange(ref tempoAdjustmentIndex, 0);

                    TimeChangedEventInvoke();
                }
            }
        }
        double _sampleIndex;

        public override double length => audioMetaData != null ? audioMetaData.length : 0;

        public double spatialStereo
        {
            get => Interlocked.CompareExchange(ref _spatialStereo, 0, 0);
            set => Interlocked.Exchange(ref _spatialStereo, value);
        }
        double _spatialStereo;



        bool isDisposable = false;



        void Update()
        {
            if (audioSource == null || AudioLoader.audioListener == null || audioLowPassFilter == null)
                return;
            
            {
                float pitch = (float)realPitch * ((float)frequency / AudioLoader.systemFrequency) * ((float)channels / AudioLoader.systemChannels);
                if (pitch != 0)
                    audioSource.pitch = pitch;
                else
                    audioSource.pitch = 1;
            }

            audioSource.spatialBlend = spatial ? 1 : 0;

            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            //매 프레임 시간 보정
            if (!isPaused)
            {
                double index = Interlocked.CompareExchange(ref _sampleIndex, 0, 0);
                index += Kernel.deltaTimeDouble * frequency * realTempo;

                Interlocked.Exchange(ref _sampleIndex, index);
            }

            if (isPlaying && (!audioSource.enabled || !audioSource.isPlaying || audioSource.clip != null))
            {
                audioSource.enabled = true;
                audioSource.Stop();

                audioSource.clip = null;
                audioSource.Play();
            }

            if (spatial)
                spatialStereo = (Quaternion.Inverse(AudioLoader.audioListener.transform.rotation) * (transform.position - AudioLoader.audioListener.transform.position)).x / (transform.position - AudioLoader.audioListener.transform.position).magnitude;

            if (isDisposable && !loop && datas != null && (sampleIndex < 0 || sampleIndex >= datas.Length))
                Remove();
        }



        public override bool Refresh()
        {
            try
            {
                ThreadManager.Lock(ref onAudioFilterReadLock);

                AudioData? audioData = AudioLoader.SearchAudioData(key, nameSpace);
                if (audioData == null || audioData.audios.Length <= 0)
                    return false;

                AudioMetaData? audioMetaData = audioData.audios[Random.Range(0, audioData.audios.Length)];
                if (audioMetaData == null || audioMetaData.datas == null)
                    return false;

                this.audioData = audioData;
                this.audioMetaData = audioMetaData;
            }
            finally
            {
                ThreadManager.Unlock(ref onAudioFilterReadLock);
            }

            return true;
        }

        public override void Play()
        {
            if (!isActiveAndEnabled)
                return;

            Stop();
            if (!Refresh())
                return;

            Update();

            if (tempo < 0 && audioMetaData != null && audioMetaData.datas != null)
            {
                int value = audioMetaData.datas.Length / audioMetaData.channels;

                Interlocked.Exchange(ref _sampleIndex, value);
                Interlocked.Exchange(ref realSampleIndex, value);
            }

            if (audioSource != null)
            {
                audioSource.clip = null;
                audioSource.Play();
            }

            base.Play();
        }

        public override void Stop()
        {
            base.Stop();

            try
            {
                ThreadManager.Lock(ref onAudioFilterReadLock);

                if (audioSource != null)
                    audioSource.Stop();

                audioData = null;
                audioMetaData = null;

                spatialStereo = 0;

                Interlocked.Exchange(ref _sampleIndex, 0);
                Interlocked.Exchange(ref realSampleIndex, 0);
            }
            finally
            {
                ThreadManager.Unlock(ref onAudioFilterReadLock);
            }
        }



        [NonSerialized] int onAudioFilterReadLock = 0;
        [NonSerialized] float[] tempData = new float[0];
        [NonSerialized] double tempoAdjustmentIndex = 0;
        [NonSerialized] double realSampleIndex = 0;
        protected override void OnAudioFilterRead(float[] data, int channels)
        {
            try
            {
                ThreadManager.Lock(ref onAudioFilterReadLock);

                if (tempData.Length != AudioLoader.systemChannels)
                    tempData = new float[AudioLoader.systemChannels];
                
                if (isPlaying && !isPaused && realTempo != 0 && audioMetaData != null && datas != null)
                {
                    double sampleIndex = this.sampleIndex;
                    double realSampleIndex = Interlocked.CompareExchange(ref this.realSampleIndex, 0, 0);
                    double tempo = realTempo;
                    double pitch = realPitch;
                    float volume = (float)this.volume;
                    bool loop = this.loop;

                    int loopStartIndex = audioMetaData.loopStartIndex;
                    int loopOffsetIndex = audioMetaData.loopOffsetIndex;

                    int audioChannels = this.channels;
                    int samplesLength = datas.Length / audioChannels;
                    
                    if (pitch > 0)
                    {
                        for (int i = 0; i < data.Length; i += channels)
                        {
                            int index;
                            if (tempo > 0)
                                index = (int)((realSampleIndex * audioChannels) + i);
                            else
                                index = (int)((realSampleIndex * audioChannels) - i);

                            GetAudioSample(ref tempData, datas, index, channels, audioChannels, loop, loopStartIndex * audioChannels, loopOffsetIndex * audioChannels, spatial, panStereo, spatialStereo);
                            for (int j = 0; j < channels; j++)
                                data[i + j] += tempData[j] * volume;
                        }
                    }

                    //시간 조정
                    {
                        {
                            double value = data.Length / audioChannels;
                            double pitchDivideTempo = tempo / (pitch != 0 ? pitch : 1);

                            //sampleIndex += value * pitchDivideTempo;
                            realSampleIndex += value * tempo.Sign();

                            //템포
                            tempoAdjustmentIndex += value;

                            double condition = 2048 * (pitch != 0 ? pitch : 1);
                            while (tempoAdjustmentIndex >= condition)
                            {
                                realSampleIndex -= value * (1 - pitchDivideTempo.Abs()) * (condition / value) * tempo.Sign();
                                sampleIndex = realSampleIndex;

                                tempoAdjustmentIndex -= condition;
                            }

                            if (value == value.Floor())
                            {
                                sampleIndex = sampleIndex.Round();
                                realSampleIndex = realSampleIndex.Round();
                            }
                        }

                        if (loop)
                        {
                            bool isLooped = false;

                            while (tempo >= 0 && realSampleIndex >= samplesLength)
                            {
                                double value = samplesLength - 1 - (loopStartIndex + loopOffsetIndex);

                                sampleIndex -= value;
                                realSampleIndex -= value;

                                isLooped = true;
                            }

                            while (tempo < 0 && realSampleIndex < loopStartIndex + loopOffsetIndex)
                            {
                                double value = samplesLength - 1 - (loopStartIndex + loopOffsetIndex);

                                sampleIndex += value;
                                realSampleIndex += value;

                                isLooped = true;
                            }

                            if (isLooped)
                                LoopedEventInvoke();
                        }
                    }

                    Interlocked.Exchange(ref _sampleIndex, sampleIndex);
                    Interlocked.Exchange(ref this.realSampleIndex, realSampleIndex);
                }
            }
            finally
            {
                ThreadManager.Unlock(ref onAudioFilterReadLock);
            }

            base.OnAudioFilterRead(data, channels);
        }

        /// <summary>
        /// 채널 개수에 영향 받지 않는 원시 인덱스를 인자로 전달해야합니다
        /// </summary>
        public static void GetAudioSample(ref float[] data, float[] samples, int index, int channels, int audioChannels, bool loop, int loopStartIndex, int loopOffsetIndex, bool spatial, double panStereo, double spatialStereo)
        {
            //현재 재생중인 오디오 채널이 2 보다 크다면 변환 없이 재생
            if (audioChannels > 2)
            {
                for (int i = 0; i < channels; i++)
                    data[i] = GetSample(channels);
            }
            else if (audioChannels == 2) //현재 재생중인 오디오 채널이 2일때
            {
                //현재 시스템 채널이 1 보다 크다면 스테레오로 재생
                if (channels >= 2)
                {
                    for (int i = 0; i < channels; i++)
                        data[i] = GetSample(i);
                }
                else //현재 시스템 채널이 1 이하라면 모노로 재생
                {
                    float left = GetSample(0);
                    float right = GetSample(1);

                    data[0] = (left + right) * 0.66666666666666f;
                }
            }
            else if (audioChannels < 2) //현재 재생중인 오디오의 채널이 2 보다 작다면 변환 없이 재생
            {
                /* 
                 * 오디오 품질이 구려진다
                 * 왜 그런지는 알 것 같은데 해결 방법을 ㅁ?ㄹ
                 * 애초에 지금 템포도 구현을 잘못해놔서 이상해지지만 구현을 제대로 하기에는 너무 어렵다
                 * 나중에 오디오 재생 다시 설계해봐야할 듯?
                 * 그 나중에가 언제가 될진 모르겠지만...
                 */

                for (int i = 0; i < channels; i++)
                    data[i] = GetSample(0) / channels;
            }

            if (channels >= 2)
            {
                float left = data[0];
                float right = data[1];

                if (!spatial)
                {
                    float leftStereo = (float)-panStereo.Clamp(-1, 0);
                    float rightStereo = (float)panStereo.Clamp(0, 1);

                    data[0] = (left + 0f.Lerp(right, leftStereo)) * (1 - rightStereo) * 1f.Lerp(0.5f, (float)panStereo.Abs());
                    data[1] = (right + 0f.Lerp(left, rightStereo)) * (1 - leftStereo) * 1f.Lerp(0.5f, (float)panStereo.Abs());
                }
                else
                {
                    float leftStereo = (float)-spatialStereo.Clamp(-1, 0);
                    float rightStereo = (float)spatialStereo.Clamp(0, 1);

                    data[0] = (left + 0f.Lerp(right, leftStereo)) * (1 - rightStereo) * 1f.Lerp(0.5f, (float)spatialStereo.Abs());
                    data[1] = (right + 0f.Lerp(left, rightStereo)) * (1 - leftStereo) * 1f.Lerp(0.5f, (float)spatialStereo.Abs());
                }
            }

            float GetSample(int i)
            {
                int sampleIndex = index + i;
                if (loop)
                    sampleIndex = sampleIndex.Repeat(samples.Length - 1);
                
                if (sampleIndex >= 0 && sampleIndex < samples.Length)
                {
                    float sample = samples[sampleIndex];

                    //루프
                    if (loop)
                    {
                        int rawLoopOffsetSampleIndex = sampleIndex - (samples.Length - 1 - loopOffsetIndex);
                        int loopOffsetSampleIndex = loopStartIndex + rawLoopOffsetSampleIndex.Repeat(samples.Length - 1 - loopStartIndex);
                        
                        if (rawLoopOffsetSampleIndex >= 0 && loopOffsetSampleIndex >= 0 && loopOffsetSampleIndex < samples.Length)
                            sample += samples[loopOffsetSampleIndex];
                    }

                    return sample;
                }

                return 0;
            }

            return;
        }

        public static AudioPlayer? PlayAudio(string key, string nameSpace = "", double volume = 1, bool loop = false, double pitch = 1, double tempo = 1, double panStereo = 0, Transform? parent = null) => InternalPlayAudio(key, nameSpace, volume, loop, pitch, tempo, panStereo, parent, false, Vector3.zero, 0, 16);

        public static AudioPlayer? PlayAudio(string key, string nameSpace, double volume, bool loop, double pitch, double tempo, double panStereo, Transform? parent, Vector3 position, float minDistance = 0, float maxDistance = 16) => InternalPlayAudio(key, nameSpace, volume, loop, pitch, tempo, panStereo, parent, true, position, minDistance, maxDistance);

        static AudioPlayer? InternalPlayAudio(string key, string nameSpace, double volume, bool loop, double pitch, double tempo, double panStereo, Transform? parent, bool spatial, Vector3 position, float minDistance, float maxDistance)
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();
            ResourceDataNotLoadedException.Exception();

            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            AudioPlayer? audioPlayer = (AudioPlayer?)ObjectPoolingManager.ObjectCreate("audio_player.prefab", parent).monoBehaviour;
            if (audioPlayer == null)
                return null;

            audioPlayer.key = key;
            audioPlayer.nameSpace = nameSpace;

            audioPlayer.volume = volume;
            audioPlayer.loop = loop;
            audioPlayer.pitch = pitch;
            audioPlayer.tempo = tempo;

            audioPlayer.panStereo = panStereo;
            audioPlayer.spatial = spatial;

            audioPlayer.minDistance = minDistance;
            audioPlayer.maxDistance = maxDistance;

            audioPlayer.transform.localPosition = position;

            audioPlayer.isDisposable = true;
            audioPlayer.Play();

            return audioPlayer;
        }
    }
}
