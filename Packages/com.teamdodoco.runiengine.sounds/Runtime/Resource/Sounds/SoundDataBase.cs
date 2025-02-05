#nullable enable
using Newtonsoft.Json;
using System;

namespace RuniEngine.Resource.Sounds
{
    public abstract class SoundData : IDisposable
    {
        public SoundData(string subtitle, bool isBGM)
        {
            this.subtitle = subtitle;
            this.isBGM = isBGM;
        }

        [NotNullField] public string subtitle { get; set; } = "";
        public bool isBGM { get; set; } = false;

        [JsonIgnore, NotNullField] public abstract SoundMetaDataBase[] sounds { get; }

        public void Dispose()
        {
            for (int i = 0; i < sounds.Length; i++)
                sounds[i].Dispose();
        }
    }
}
