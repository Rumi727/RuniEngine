#nullable enable
using Newtonsoft.Json;
using RuniEngine.NBS;
using RuniEngine.Rhythms;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
        public NBSMetaData(string path, double pitch, double tempo, double bpmMultiplier, double rhythmOffsetTick, NBSFile? nbsFile) : base(path, pitch, tempo)
        {
            this.bpmMultiplier = bpmMultiplier;
            this.rhythmOffsetTick = rhythmOffsetTick;

            this.nbsFile = nbsFile;
            if (nbsFile != null)
                bpms = new BeatBPMPairList(nbsFile.tickTempo * 0.15f * bpmMultiplier);
        }

        [JsonIgnore] public int loopStartTick => nbsFile?.loopStartTick ?? 0;

        [JsonIgnore] public override BeatBPMPairList bpms { get; } = new BeatBPMPairList();
        public double bpmMultiplier { get; } = 1;
        public double rhythmOffsetTick { get; } = 0;

        [JsonIgnore] public override double rhythmOffset => rhythmOffsetTick / 20d;

        [JsonIgnore] public NBSFile? nbsFile { get; }
    }
}
