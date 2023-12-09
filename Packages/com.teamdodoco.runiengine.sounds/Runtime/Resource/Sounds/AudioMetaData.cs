#nullable enable
using Newtonsoft.Json;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
        public AudioMetaData(string path, float pitch, float tempo, bool stream, int loopStartIndex, AudioClip? audioClip) : base(path, pitch, tempo, stream)
        {
            this.loopStartIndex = loopStartIndex;
            this.audioClip = audioClip;

            if (audioClip != null)
            {
                frequency = audioClip.frequency;
                channels = audioClip.channels;

                length = audioClip.length;
                samples = audioClip.samples;
            }
        }

        public int loopStartIndex { get; } = 0;

        [JsonIgnore] public AudioClip? audioClip { get; }

        [JsonIgnore] public int frequency { get; }
        [JsonIgnore] public int channels { get; }

        [JsonIgnore] public float length { get; }
        [JsonIgnore] public int samples { get; }
    }
}
