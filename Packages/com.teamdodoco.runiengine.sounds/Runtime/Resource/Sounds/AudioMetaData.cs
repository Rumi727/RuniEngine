#nullable enable
using Newtonsoft.Json;
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
#if ENABLE_RUNI_ENGINE_RHYTHMS
        public AudioMetaData(string path, double pitch, double tempo, int loopStartIndex, int loopOffsetIndex, BeatBPMPairList? bpms, int rhythmOffsetIndex, RawAudioClip? rawAudioClip) : base(path, pitch, tempo)
#else
        public AudioMetaData(string path, double pitch, double tempo, int loopStartIndex, int loopOffsetIndex, RawAudioClip? rawAudioClip) : base(path, pitch, tempo)
#endif
        {
            this.loopStartIndex = loopStartIndex;
            this.loopOffsetIndex = loopOffsetIndex;

#if ENABLE_RUNI_ENGINE_RHYTHMS
            this.bpms = bpms ?? new BeatBPMPairList();
            this.rhythmOffsetIndex = rhythmOffsetIndex;
#endif

            this.rawAudioClip = rawAudioClip;
        }

        public int loopStartIndex { get; set; } = 0;
        public int loopOffsetIndex { get; set; } = 0;

#if ENABLE_RUNI_ENGINE_RHYTHMS
        public override BeatBPMPairList bpms { get; } = new BeatBPMPairList();
        public int rhythmOffsetIndex { get; set; } = 0;

        [JsonIgnore]
        public override double rhythmOffset
        {
            get => (double)rhythmOffsetIndex / frequency / tempo;
            set => rhythmOffsetIndex = (int)(value * frequency * tempo);
        }
#endif

        [JsonIgnore] public RawAudioClip? rawAudioClip { get; }

        [JsonIgnore] public int frequency => rawAudioClip?.frequency ?? 0;
        [JsonIgnore] public int channels => rawAudioClip?.channels ?? 0;

        [JsonIgnore] public float length => rawAudioClip?.length ?? 0;
        [JsonIgnore] public long samples => rawAudioClip?.samples ?? 0;

        public bool isDisposed => rawAudioClip?.isDisposed ?? true;

        public override void Dispose() => rawAudioClip?.Dispose();
    }
}
