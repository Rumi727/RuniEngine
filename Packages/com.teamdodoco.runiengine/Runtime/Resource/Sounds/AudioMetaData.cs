#nullable enable
using Newtonsoft.Json;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
        public AudioMetaData(string path, double pitch, double tempo, bool stream, int loopStartIndex, int loopOffsetIndex, AudioClip? audioClip) : base(path, pitch, tempo, stream)
        {
            this.loopStartIndex = loopStartIndex;
            this.loopOffsetIndex = loopOffsetIndex;

            this.audioClip = audioClip;
            if (audioClip != null)
            {
                if (audioClip.loadType == AudioClipLoadType.DecompressOnLoad)
                {
                    datas = new float[audioClip.samples * audioClip.channels];
                    audioClip.GetData(datas, 0);
                }

                frequency = audioClip.frequency;
                channels = audioClip.channels;

                length = audioClip.length;
            }
        }

        public int loopStartIndex { get; } = 0;
        public int loopOffsetIndex { get; } = 0;

        [JsonIgnore] public AudioClip? audioClip { get; }

        [JsonIgnore] public float[]? datas { get; }

        [JsonIgnore] public int frequency { get; }
        [JsonIgnore] public int channels { get; }

        [JsonIgnore] public float length { get; }
    }
}
