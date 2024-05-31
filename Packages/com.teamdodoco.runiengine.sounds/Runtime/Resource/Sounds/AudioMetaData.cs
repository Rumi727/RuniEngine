#nullable enable
using Newtonsoft.Json;
using RuniEngine.Rhythms;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
        public AudioMetaData(string path, double pitch, double tempo, int loopStartIndex, int loopOffsetIndex, BeatBPMPairList? bpms, int rhythmOffsetIndex, AudioClip? audioClip) : base(path, pitch, tempo)
        {
            this.loopStartIndex = loopStartIndex;
            this.loopOffsetIndex = loopOffsetIndex;

            this.bpms = bpms ?? new BeatBPMPairList();
            this.rhythmOffsetIndex = rhythmOffsetIndex;

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
                samples = audioClip.samples;
            }
        }

        public int loopStartIndex { get; } = 0;
        public int loopOffsetIndex { get; } = 0;

        public override BeatBPMPairList bpms { get; } = new BeatBPMPairList();
        public int rhythmOffsetIndex { get; } = 0;

        [JsonIgnore] public override double rhythmOffset => (double)rhythmOffsetIndex / frequency / tempo;

        [JsonIgnore] public float[]? datas { get; }

        [JsonIgnore] public int frequency { get; }
        [JsonIgnore] public int channels { get; }

        [JsonIgnore] public float length { get; }
        [JsonIgnore] public int samples { get; }
    }
}
