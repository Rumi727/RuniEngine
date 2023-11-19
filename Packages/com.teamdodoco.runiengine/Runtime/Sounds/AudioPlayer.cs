#nullable enable
using RuniEngine.Resource.Sounds;
using RuniEngine.Threading;
using System;
using System.Threading;
using UnityEngine;

using Random = UnityEngine.Random;

namespace RuniEngine.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : SoundPlayerBase
    {
        readonly AudioSource? _audioSource;
        public AudioSource? audioSource => this.GetComponentFieldSave(_audioSource);



        public override SoundData? soundData => audioData;
        public override SoundMetaDataBase? soundMetaData => audioMetaData;

        public AudioData? audioData { get; private set; }
        public AudioMetaData? audioMetaData { get; private set; }



        public float[]? datas => audioMetaData?.datas;



        public int frequency => audioMetaData != null ? audioMetaData.frequency : 0;

        public int channels => audioMetaData != null ? audioMetaData.channels : 0;


        public override double time
        {
            get => currentSampleIndex / (frequency != 0 ? frequency : AudioLoader.systemFrequency);
            set => currentSampleIndex = value * (frequency != 0 ? frequency : AudioLoader.systemFrequency);
        }

        public double currentSampleIndex
        {
            get => Interlocked.CompareExchange(ref _currentSampleIndex, 0, 0);
            set
            {
                if (currentSampleIndex != value)
                {
                    Interlocked.Exchange(ref _currentSampleIndex, value);
                    TimeChangedEventInvoke();
                }
            }
        }
        double _currentSampleIndex;

        public override double length => audioMetaData != null ? audioMetaData.length : 0;



        void Update()
        {
            if (audioSource == null)
                return;
            
            {
                float pitch = (float)realPitch * ((float)frequency / AudioLoader.systemFrequency) * ((float)channels / AudioLoader.systemChannels);
                if (pitch != 0)
                    audioSource.pitch = pitch;
                else
                    audioSource.pitch = 1;
            }

            audioSource.volume = 1;
            //audioSource.panStereo = panStereo;
            audioSource.spatialBlend = spatial ? 1 : 0;

            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            if (isPlaying && (!audioSource.enabled || !audioSource.isPlaying || audioSource.clip != null))
            {
                audioSource.enabled = true;
                audioSource.Stop();

                audioSource.clip = null;
                audioSource.Play();
            }
        }



        public override void Refresh()
        {
            try
            {
                ThreadManager.Lock(ref onAudioFilterReadLock);

                AudioData? audioData = AudioLoader.SearchAudioData(key, nameSpace);
                if (audioData == null || audioData.audios.Length <= 0)
                    return;

                AudioMetaData? audioMetaData = audioData.audios[Random.Range(0, audioData.audios.Length)];
                if (audioMetaData == null || audioMetaData.datas == null)
                    return;

                this.audioData = audioData;
                this.audioMetaData = audioMetaData;
            }
            finally
            {
                ThreadManager.Unlock(ref onAudioFilterReadLock);
            }
        }

        public override void Play()
        {
            if (!isActiveAndEnabled)
                return;

            Stop();
            Refresh();

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

                Interlocked.Exchange(ref _currentSampleIndex, 0);
            }
            finally
            {
                ThreadManager.Unlock(ref onAudioFilterReadLock);
            }
        }



        [NonSerialized] int onAudioFilterReadLock = 0;
        protected override void OnAudioFilterRead(float[] data, int channels)
        {
            try
            {
                ThreadManager.Lock(ref onAudioFilterReadLock);
                
                if (isPlaying && !isPaused && realTempo != 0 && audioMetaData != null && datas != null)
                {
                    double currentIndex = currentSampleIndex;
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
                                index = (int)((currentIndex * audioChannels) + i);
                            else
                                index = (int)((currentIndex * audioChannels) - i);

                            float[] value = GetAudioSample(datas, index, channels, audioChannels, loop, loopStartIndex * audioChannels, loopOffsetIndex * audioChannels, spatial, panStereo);
                            for (int j = 0; j < channels; j++)
                                data[i + j] = value[j] * volume;
                        }
                    }

                    {
                        double value = data.Length / audioChannels * (tempo / (pitch != 0 ? pitch : 1));
                        currentIndex += value;

                        if (value == value.Floor())
                            currentIndex = currentIndex.Round();

                        if (loop)
                        {
                            bool isLooped = false;

                            while (tempo > 0 && currentIndex >= samplesLength)
                            {
                                currentIndex -= samplesLength - 1 - (loopStartIndex + loopOffsetIndex);
                                isLooped = true;
                            }

                            while (tempo < 0 && currentIndex < loopStartIndex + loopOffsetIndex)
                            {
                                currentIndex += samplesLength - 1 - (loopStartIndex + loopOffsetIndex);
                                isLooped = true;
                            }

                            if (isLooped)
                                LoopedEventInvoke();
                        }
                    }

                    Interlocked.Exchange(ref _currentSampleIndex, currentIndex);
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
        public static float[] GetAudioSample(float[] samples, int index, int channels, int audioChannels, bool loop, int loopStartIndex, int loopOffsetIndex, bool spatial, float panStereo)
        {
            float[] data = new float[channels];

            //현재 재생중인 오디오 채널이 2 보다 크다면 변환 없이 재생
            if (audioChannels > 2)
            {
                for (int i = 0; i < channels; i++)
                    data[i] = GetSample(channels);
            }
            else if (audioChannels == 2) //현재 재생중인 오디오 채널이 2일때
            {
                //현재 시스템 채널이 1 보다 크다면 스테레오로 재생
                if (channels > 2)
                {
                    for (int i = 0; i < channels; i++)
                        data[i] = GetSample(channels);
                }
                else if (channels == 2)
                {
                    if (!spatial)
                    {
                        float left = GetSample(0);
                        float right = GetSample(1);
                        float leftStereo = -panStereo.Clamp(-1, 0);
                        float rightStereo = panStereo.Clamp(0, 1);

                        data[0] = (left + 0f.Lerp(right, leftStereo)) * (1 - rightStereo) * 1f.Lerp(0.5f, panStereo.Abs());
                        data[1] = (right + 0f.Lerp(left, rightStereo)) * (1 - leftStereo) * 1f.Lerp(0.5f, panStereo.Abs());
                    }
                    else
                    {
                        float left = GetSample(0);
                        float right = GetSample(1);

                        data[0] = left;
                        data[1] = right;
                    }
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

            return data;
        }
    }
}
