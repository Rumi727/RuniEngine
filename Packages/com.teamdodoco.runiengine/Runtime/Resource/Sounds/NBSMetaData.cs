#nullable enable
using Newtonsoft.Json;
using RuniEngine.NBS;

namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSMetaData : SoundMetaDataBase
    {
        public NBSMetaData(string path, double pitch, double tempo, bool stream, double loopStartTime, double loopOffsetTime, NBSFile nbsFile) : base(path, pitch, tempo, stream, loopStartTime, loopOffsetTime) => this.nbsFile = nbsFile;

        [JsonIgnore] public override double loopStartTime => nbsFile.loopStartTick / 20d;

        [JsonIgnore] public NBSFile nbsFile { get; }
    }
}
