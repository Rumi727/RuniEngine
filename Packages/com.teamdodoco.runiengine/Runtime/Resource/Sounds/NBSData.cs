#nullable enable
namespace RuniEngine.Resource.Sounds
{
    public sealed class NBSData : SoundData
    {
        public NBSData(string subtitle, bool isBGM, params NBSMetaData[]? nbses) : base(subtitle, isBGM)
        {
            nbses ??= new NBSMetaData[0];

            sounds = nbses;
            this.nbses = nbses;
        }

        public override SoundMetaDataBase[] sounds { get; }
        public NBSMetaData[] nbses { get; }
    }
}
