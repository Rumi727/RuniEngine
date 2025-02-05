#nullable enable
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
using System;
#endif

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundMetaDataBase : IDisposable
    {
        public SoundMetaDataBase(string path, double pitch, double tempo)
        {
            this.path = path;

            this.pitch = pitch;
            this.tempo = tempo;
        }

        [NotNullField] public virtual string path { get; set; } = "";

        public virtual double pitch { get; set; } = 1;
        public virtual double tempo { get; set; } = 1;

        public abstract void Dispose();

#if ENABLE_RUNI_ENGINE_RHYTHMS
        [NotNullField] public abstract BeatBPMPairList bpms { get; }
        public abstract double rhythmOffset { get; set; }
#endif
    }
}
