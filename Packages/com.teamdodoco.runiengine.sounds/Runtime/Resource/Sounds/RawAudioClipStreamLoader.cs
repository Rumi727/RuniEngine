#nullable enable
using NAudio.Wave;
using RuniEngine.Threading;
using System.Diagnostics;
using System.Threading;

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



        int loop = 0;
        Stopwatch stopwatch = new Stopwatch();
        void Read(long index)
        {
            try
            {
                ThreadTask.Lock(ref disposeLock);

                if (stream == null || reader == null)
                    return;

                if (stream.Position != index * 4)
                    stream.Position = index * 4;

                loop = 0;

                Thread thread = new Thread(() =>
                {
                    //이새끼 시간 빠르게 바꾸면 멈출때 있음
                    reader.Read(buffer, 0, buffer.Length);
                    Interlocked.Exchange(ref loop, 1);
                })
                { IsBackground = false };

                thread.Start();

                stopwatch.Restart();
                while (Interlocked.Add(ref loop, 0) == 0)
                {
                    if (stopwatch.ElapsedMilliseconds >= 100)
                    {
                        thread.Abort();
                        Debug.LogError("Stream Audio Read Deadlock Detected!!!", nameof(RawAudioClipStreamLoader));

                        break;
                    }
                }
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
            base.Dispose();

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
