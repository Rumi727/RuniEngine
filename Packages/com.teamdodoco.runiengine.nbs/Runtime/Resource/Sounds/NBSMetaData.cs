#nullable enable
using Newtonsoft.Json;
using RuniEngine.NBS;
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
#if ENABLE_RUNI_ENGINE_RHYTHMS
        public NBSMetaData(string path, double pitch, double tempo, double bpmMultiplier, double rhythmOffsetTick, NBSFile? nbsFile) : base(path, pitch, tempo)
#else
        public NBSMetaData(string path, double pitch, double tempo, NBSFile? nbsFile) : base(path, pitch, tempo)
#endif
        {
#if ENABLE_RUNI_ENGINE_RHYTHMS
            this.bpmMultiplier = bpmMultiplier;
            this.rhythmOffsetTick = rhythmOffsetTick;
#endif
            this.nbsFile = nbsFile;

#if ENABLE_RUNI_ENGINE_RHYTHMS
            if (nbsFile != null)
                bpms = new BeatBPMPairList(new BPM(nbsFile.tickTempo * 0.15f * bpmMultiplier, 4));
#endif
        }

        [JsonIgnore] public int loopStartTick => nbsFile?.loopStartTick ?? 0;

#if ENABLE_RUNI_ENGINE_RHYTHMS
        [JsonIgnore] public override BeatBPMPairList bpms { get; } = new BeatBPMPairList();
        public double bpmMultiplier { get; set; } = 1;
        public double rhythmOffsetTick { get; set; } = 0;

        [JsonIgnore]
        public override double rhythmOffset
        {
            get => rhythmOffsetTick / 20d;
            set => rhythmOffsetTick = value * 20;
        }
#endif

        [JsonIgnore] public NBSFile? nbsFile { get; }
    }
}
