#nullable enable
using NAudio.Wave;
using System;

namespace RuniEngine.Resource.Sounds
{
    public abstract class RawAudioClipLoaderBase : IDisposable
    {
        protected RawAudioClipLoaderBase(WaveStream stream) : this(stream.WaveFormat.SampleRate, stream.WaveFormat.Channels, stream.Length / 4 / stream.WaveFormat.Channels) { }

        protected RawAudioClipLoaderBase(int frequency, int channels, long samples)
        {
            ResourceManager.RegisterManagedResource(this);

            this.frequency = frequency;
            this.channels = channels;
            this.samples = samples;

            length = (float)samples / frequency;
            arrayLength = samples * channels;
        }

        public abstract bool isLoading { get; }
        public abstract bool isLoaded { get; }
        public abstract bool isStream { get; }

        /// <summary>
        /// 샘플 레이트
        /// <para></para>
        /// Thread-safe
        /// </summary>
        public int frequency { get; }
        /// <summary>
        /// 채널 수
        /// <para></para>
        /// Thread-safe
        /// </summary>
        public int channels { get; }

        /// <summary>
        /// 샘플 단위 길이
        /// <para></para>
        /// Thread-safe
        /// </summary>
        public long samples { get; }
        /// <summary>
        /// 초 단위 길이
        /// <para></para>
        /// Thread-safe
        /// </summary>
        public float length { get; }

        /// <summary>배열의 길이<br/>Thread-safe</summary>
        public long arrayLength { get; }

        public abstract float GetSample(long index);
        [Obsolete("Legacy Code", true)] public virtual void SetStreamPosition(long index) { }

        [Obsolete("Legacy Code", true)] public virtual void Playing() { }
        [Obsolete("Legacy Code", true)] public virtual void Stopping() { }

        public virtual void Unload() { }
        public virtual void Dispose() { }
    }
}
