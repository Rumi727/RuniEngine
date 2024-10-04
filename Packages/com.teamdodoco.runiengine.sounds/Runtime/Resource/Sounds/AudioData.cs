#nullable enable
using Newtonsoft.Json;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioData : SoundData
    {
        public AudioData(string subtitle, bool isBGM, params AudioMetaData[]? audios) : base(subtitle, isBGM)
        {
            audios ??= new AudioMetaData[0];

            sounds = audios;
            this.audios = audios;
        }

        [JsonIgnore] public override SoundMetaDataBase[] sounds { get; }
        [NotNullField] public AudioMetaData[] audios { get; }
    }
}
