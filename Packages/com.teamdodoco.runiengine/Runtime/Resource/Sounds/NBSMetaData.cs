#nullable enable
using RuniEngine.NBS;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
        public NBSMetaData(string path, double pitch, double tempo, bool stream, double loopStartTime, double loopOffsetTime, NBSFile nbsFile) : base(path, pitch, tempo, stream, loopStartTime, loopOffsetTime) => this.nbsFile = nbsFile;

        public NBSFile nbsFile { get; }
    }
}
