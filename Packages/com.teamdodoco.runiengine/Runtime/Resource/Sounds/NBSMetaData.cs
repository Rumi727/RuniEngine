#nullable enable
using RuniEngine.NBS;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
        public NBSMetaData(string path, float pitch, float tempo, bool stream, float loopStartTime, NBSFile nbsFile) : base(path, pitch, tempo, stream, loopStartTime) => this.nbsFile = nbsFile;

        public NBSFile nbsFile { get; }
    }
}
