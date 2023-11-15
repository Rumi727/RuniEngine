#nullable enable

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundMetaDataBase
    {
        public SoundMetaDataBase(string path, double pitch, double tempo, bool stream, double loopStartTime, double loopOffsetTime)
        {
            this.path = path;

            this.pitch = pitch;
            this.tempo = tempo;

            this.stream = stream;

            this.loopStartTime = loopStartTime;
            this.loopOffsetTime = loopOffsetTime;
        }

        public virtual string path { get; } = "";

        public virtual double pitch { get; } = 1;
        public virtual double tempo { get; } = 1;

        public virtual bool stream { get; } = false;

        public virtual double loopStartTime { get; } = 0;
        public virtual double loopOffsetTime { get; } = 0;
    }
}
