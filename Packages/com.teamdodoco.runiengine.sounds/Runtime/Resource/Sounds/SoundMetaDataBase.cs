#nullable enable
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif

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

#if ENABLE_RUNI_ENGINE_RHYTHMS
        public abstract BeatBPMPairList bpms { get; }
        public abstract double rhythmOffset { get; }
#endif
    }
}
