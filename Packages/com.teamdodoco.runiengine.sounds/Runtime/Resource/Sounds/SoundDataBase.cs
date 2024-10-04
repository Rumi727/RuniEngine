#nullable enable
using Newtonsoft.Json;

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundData
    {
        public SoundData(string subtitle, bool isBGM)
        {
            this.subtitle = subtitle;
            this.isBGM = isBGM;
        }

        [NotNullField] public string subtitle { get; set; } = "";
        public bool isBGM { get; set; } = false;

        [JsonIgnore, NotNullField] public abstract SoundMetaDataBase[] sounds { get; }
    }
}
