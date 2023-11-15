#nullable enable
using Newtonsoft.Json;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class AudioMetaData : SoundMetaDataBase
    {
        public AudioMetaData(string path, double pitch, double tempo, bool stream, double loopStartTime, double loopOffsetTime, AudioClip audioClip) : base(path, pitch, tempo, stream, loopOffsetTime, loopStartTime) => this.audioClip = audioClip;

        [JsonIgnore] public AudioClip audioClip { get; }
    }
}
