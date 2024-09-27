#nullable enable
using Cysharp.Threading.Tasks;
using NAudio.Wave;
using RuniEngine.Threading;
using System;

namespace RuniEngine.Resource.Sounds
{
    public sealed class RawAudioClipStreamLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipStreamLoader(WaveStream stream) : base(stream)
        {
            this.stream = stream;
            reader = stream.ToSampleProvider();
            
            buffer = new float[frequency / 10 * channels];
            reader.Read(buffer, 0, buffer.Length);
        }

        /// <summary>Thread-safe</summary>
        public override bool isLoading => true;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => false;

        /// <summary>Thread-safe</summary>
        public override bool isStream => true;



        void Read(long index)
        {
            try
            {
                ThreadTask.Lock(ref disposeLock);

                if (stream == null || reader == null)
                    return;

                if (stream.Position != index * 4)
                    stream.Position = index * 4;

                reader.Read(buffer, 0, buffer.Length);
            }
            finally
            {
                ThreadTask.Unlock(ref disposeLock);
            }
        }



        WaveStream? stream;
        ISampleProvider? reader;
        readonly float[] buffer;
        int disposeLock = 0;
        long position = 0;
        /// <summary>Thread-safe</summary>
        public override float GetSample(long index)
        {
            if (position.Distance(index) >= buffer.Length)
            {
                Read(index);
                position = index;
            }

            return buffer[(index - position).Clamp(0)];
        }

        /// <summary>Thread-safe</summary>
        public override void Dispose()
        {
            try
            {
                ThreadTask.Lock(ref disposeLock);

                stream?.Dispose();

                stream = null;
                reader = null;
            }
            finally
            {
                ThreadTask.Unlock(ref disposeLock);
            }
        }
    }
}
