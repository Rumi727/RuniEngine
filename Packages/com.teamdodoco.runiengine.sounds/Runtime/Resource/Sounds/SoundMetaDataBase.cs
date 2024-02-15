#nullable enable

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundMetaDataBase
    {
        public SoundMetaDataBase(string path, double pitch, double tempo, bool stream)
        {
            this.path = path;

            this.pitch = pitch;
            this.tempo = tempo;

            this.stream = stream;
        }

        public virtual string path { get; } = "";

        public virtual double pitch { get; } = 1;
        public virtual double tempo { get; } = 1;

        public virtual bool stream { get; } = false;
    }
}
