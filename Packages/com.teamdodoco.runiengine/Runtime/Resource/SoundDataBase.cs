#nullable enable
namespace RuniEngine.Resource
{
    public abstract class SoundData
    {
        public SoundData(string subtitle, bool isBGM)
        {
            this.subtitle = subtitle;
            this.isBGM = isBGM;
        }

        public string subtitle { get; } = "";
        public bool isBGM { get; } = false;

        public abstract SoundMetaDataBase[] sounds { get; }
    }
}
