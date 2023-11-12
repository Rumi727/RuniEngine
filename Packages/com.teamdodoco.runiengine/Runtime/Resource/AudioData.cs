#nullable enable
namespace RuniEngine.Resource
{
    public sealed class AudioData : SoundData
    {
        public AudioData(string subtitle, bool isBGM, params AudioMetaData[] audios) : base(subtitle, isBGM)
        {
            sounds = audios;
            this.audios = audios;
        }

        public override SoundMetaDataBase[] sounds { get; }
        public AudioMetaData[] audios { get; }
    }
}
