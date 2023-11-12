#nullable enable

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundMetaDataBase
    {
        public SoundMetaDataBase(string path, float pitch, float tempo, bool stream, float loopStartTime)
        {
            this.path = path;

            this.pitch = pitch;
            this.tempo = tempo;

            this.stream = stream;
            this.loopStartTime = loopStartTime;
        }

        public virtual string path { get; } = "";

        public virtual float pitch { get; } = 1;
        public virtual float tempo { get; } = 1;

        public virtual bool stream { get; } = false;
        public virtual float loopStartTime { get; } = 0;
    }
}
