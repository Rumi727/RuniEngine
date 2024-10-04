#nullable enable
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
using UnityEngine;
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

        [SerializeField] public virtual string path { get; set; } = "";

        [SerializeField] public virtual double pitch { get; set; } = 1;
        [SerializeField] public virtual double tempo { get; set; } = 1;

#if ENABLE_RUNI_ENGINE_RHYTHMS
        [SerializeField] public abstract BeatBPMPairList bpms { get; }
        [SerializeField] public abstract double rhythmOffset { get; set; }
#endif
    }
}
