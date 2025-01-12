#nullable enable
using RuniEngine.Threading;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    public sealed class RawAudioClipUnityLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipUnityLoader(AudioClip audioClip) : base(audioClip.frequency, audioClip.channels, audioClip.samples)
        {
            NotMainThreadException.Exception();

            float[] datas = new float[samples * channels];
            audioClip.GetData(datas, 0);

            this.datas = datas;
        }

        readonly float[] datas;

        /// <summary>Thread-safe</summary>
        public override bool isLoading => false;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => true;

        /// <summary>Thread-safe</summary>
        public override bool isStream => false;

        /// <summary>Thread-safe</summary>
        public override float GetSample(long index) => datas[index];
    }
}
