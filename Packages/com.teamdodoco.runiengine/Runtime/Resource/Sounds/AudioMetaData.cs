#nullable enable
using Newtonsoft.Json;
using RuniEngine.NBS;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
        public AudioMetaData(string path, double pitch, double tempo, bool stream, int loopStartIndex, int loopOffsetIndex, AudioClip audioClip) : base(path, pitch, tempo, stream)
        {
            this.loopStartIndex = loopStartIndex;
            this.loopOffsetIndex = loopOffsetIndex;

            this.audioClip = audioClip;
        }

        public int loopStartIndex { get; } = 0;
        public int loopOffsetIndex { get; } = 0;

        [JsonIgnore] public AudioClip audioClip { get; }
    }
}
