#nullable enable
using RuniEngine.Rhythms;

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundMetaDataBase
    {
        public SoundMetaDataBase(string path, double pitch, double tempo)
        {
            this.path = path;

            this.pitch = pitch;
            this.tempo = tempo;
        }

        public virtual string path { get; } = "";

        public virtual double pitch { get; } = 1;
        public virtual double tempo { get; } = 1;

        public abstract BeatBPMPairList bpms { get; }
        public abstract double rhythmOffset { get; }
    }
}
