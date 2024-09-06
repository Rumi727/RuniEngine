#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class RawAudioClip
    {
        /// <summary>샘플 데이터</summary>
        public IReadOnlyList<float> datas { get; }

        /// <summary>샘플 레이트</summary>
        public int frequency { get; }
        /// <summary>채널 수</summary>
        public int channels { get; }

        /// <summary>샘플 단위 길이</summary>
        public long samples { get; }
        /// <summary>초 단위 길이</summary>
        public float length { get; }

        public RawAudioClip(float[] datas, int frequency, int channels)
        {
            this.datas = datas;

            this.frequency = frequency;
            this.channels = channels;

            samples = datas.Length / channels;
            length = (float)samples / frequency;
        }

        public RawAudioClip(AudioClip audioClip)
        {
            frequency = audioClip.frequency;
            channels = audioClip.channels;

            samples = audioClip.samples;
            length = audioClip.length;

            float[] datas = new float[samples * channels];
            audioClip.GetData(datas, 0);

            this.datas = datas;
        }

        public AudioClip ToAudioClip(string name)
        {
            AudioClip clip = AudioClip.Create(name, datas.Count, channels, frequency, false);
            clip.SetData(datas.ToArray(), 0);

            return clip;
        }
    }
}
