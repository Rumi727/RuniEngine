#nullable enable
namespace RuniEngine.Rhythms
{
    public interface IRhythmable
    {
        public double time { get; set; }
        public double rhythmOffset { get; }

        public BeatBPMPairList? bpms { get; }
    }
}
