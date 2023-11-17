#nullable enable
using Newtonsoft.Json;
using RuniEngine.NBS;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
        public NBSMetaData(string path, double pitch, double tempo, bool stream, int loopOffsetTick, NBSFile? nbsFile) : base(path, pitch, tempo, stream)
        {
            this.loopOffsetTick = loopOffsetTick;
            this.nbsFile = nbsFile;
        }

        public int loopStartTick => nbsFile?.loopStartTick ?? 0;
        public int loopOffsetTick { get; }

        [JsonIgnore] public NBSFile? nbsFile { get; }
    }
}
