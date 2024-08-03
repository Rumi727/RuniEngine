#nullable enable
using Newtonsoft.Json;
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
#if ENABLE_RUNI_ENGINE_RHYTHMS
        public AudioMetaData(string path, double pitch, double tempo, int loopStartIndex, int loopOffsetIndex, BeatBPMPairList? bpms, int rhythmOffsetIndex, AudioClip? audioClip) : base(path, pitch, tempo)
#else
        public AudioMetaData(string path, double pitch, double tempo, int loopStartIndex, int loopOffsetIndex, AudioClip? audioClip) : base(path, pitch, tempo)
#endif
        {
            this.loopStartIndex = loopStartIndex;
            this.loopOffsetIndex = loopOffsetIndex;

#if ENABLE_RUNI_ENGINE_RHYTHMS
            this.bpms = bpms ?? new BeatBPMPairList();
            this.rhythmOffsetIndex = rhythmOffsetIndex;
#endif

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

#if ENABLE_RUNI_ENGINE_RHYTHMS
        public override BeatBPMPairList bpms { get; } = new BeatBPMPairList();
        public int rhythmOffsetIndex { get; } = 0;

        [JsonIgnore] public override double rhythmOffset => (double)rhythmOffsetIndex / frequency / tempo;
#endif

        [JsonIgnore] public float[]? datas { get; }

        [JsonIgnore] public int frequency { get; }
        [JsonIgnore] public int channels { get; }

        [JsonIgnore] public float length { get; }
        [JsonIgnore] public int samples { get; }
    }
}
