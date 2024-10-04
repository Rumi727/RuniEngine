#nullable enable
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

        [NotNullField] public abstract SoundMetaDataBase[] sounds { get; }
    }
}
