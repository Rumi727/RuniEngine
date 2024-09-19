#nullable enable
using Cysharp.Threading.Tasks;
using NAudio.Wave;
using RuniEngine.Threading;
using System;

namespace RuniEngine.Resource.Sounds
{
    [Obsolete("진짜 잘 작동하고 아마 이론상으로도 제대로 작동되야할 코드인데 stream.Position을 자주도 아니라 tq 약 1초마다 바꿔도 wlfkf나서 때려침 스벌\n차라리 유니티 기본 클립 스트리밍 해서 쓰세요 tlqkf...")]
    public sealed class RawAudioClipStreamLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipStreamLoader(WaveStream stream) : base(stream)
        {
            this.stream = stream;
            reader = stream.ToSampleProvider();

            cacheBuffer = new float[frequency * channels];
            buffer = new float[cacheBuffer.Length];

            reader.Read(buffer, 0, cacheBuffer.Length);
        }

        /// <summary>Thread-safe</summary>
        public override bool isLoading => true;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => false;

        /// <summary>Thread-safe</summary>
        public override bool isStream => true;



        long readPosition = 0;
        void Read(long index)
        {
            try
            {
                ThreadTask.Lock(ref disposeLock);

                if (stream == null || reader == null)
                    return;

                stream.Position = index * 4;
                reader.Read(cacheBuffer, 0, cacheBuffer.Length);
            }
            finally
            {
                ThreadTask.Unlock(ref disposeLock);
            }

            try
            {
                ThreadTask.Lock(ref getSampleLock);

                readPosition = index;
                Array.Copy(cacheBuffer, buffer, buffer.Length);
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
        }



        WaveStream? stream;
        ISampleProvider? reader;
        readonly float[] cacheBuffer;
        readonly float[] buffer;
        int disposeLock = 0;
        int getSampleLock = 0;
        long position = 0;
        /// <summary>Thread-safe</summary>
        public override float GetSample(long index)
        {
            try
            {
                ThreadTask.Lock(ref getSampleLock);

                if (position.Distance(index) >= buffer.Length - (buffer.Length * 0.125f))
                {
                    position = index;

                    //인덱스 복사 안하면 밑의 index -= readPosition 코드 때매 버그남
                    long threadIndex = index;
                    UniTask.RunOnThreadPool(() => Read(threadIndex));
                }

                index -= readPosition;
                if (index >= 0 && index < buffer.Length)
                    return buffer[index];

                return 0;
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
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
