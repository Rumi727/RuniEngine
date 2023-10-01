#nullable enable
namespace RuniEngine.Resource
{
    public class SoundData
    {
        public SoundData(string subtitle, bool isBGM, params SoundMetaData[] sounds)
        {
            this.subtitle = subtitle;
            this.isBGM = isBGM;
            this.sounds = sounds;
        }

        public string subtitle { get; } = "";
        public bool isBGM { get; } = false;
        public SoundMetaData[] sounds { get; } = new SoundMetaData[0];
    }
}
