
namespace RuniEngine.Resource.Sounds
{
    public struct AudioFileMetaData
    {
        public AudioFileMetaData(RawAudioLoadType loadType) => this.loadType = loadType;
        public RawAudioLoadType loadType;
    }
}
