#nullable enable
using Cysharp.Threading.Tasks;
using NAudio.Wave;
using RuniEngine.Threading;
using System;
using System.Threading;

namespace RuniEngine.Resource.Sounds
{
    [Obsolete("미완성 및 개발 포기했으며, 매우매우매우 불안정합니다!!! 단지 보존을 위해 있을 뿐입니다", true)]
    public sealed class RawAudioClipLegacyStreamLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipLegacyStreamLoader(WaveStream stream) : base(stream)
        {
            this.stream = stream;
            reader = stream.ToSampleProvider();

            isSmall = samples * channels <= frequency * channels;

            if (isSmall)
            {
                buffer = Array.Empty<float>();
                calculatedBuffer = Array.Empty<float>();
                readingBuffer = new float[samples * channels];
            }
            else
            {
                buffer = new float[frequency * channels];
                calculatedBuffer = new float[buffer.Length];
                readingBuffer = new float[buffer.Length];

                ReadSample(0);
            }
        }

        /// <summary>Thread-safe</summary>
        public override bool isLoading => !isSmall;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => isSmall;

        /// <summary>Thread-safe</summary>
        public override bool isStream => !isSmall;



        object? playingObject;
        int playingObjectLock = 0;

        readonly bool isSmall = false;

        int streamLock = 0;
        WaveStream? stream;
        ISampleProvider? reader;
        readonly float[] buffer;
        readonly float[] calculatedBuffer;
        readonly float[] readingBuffer;
        long currentReadingIndex = 0;
        int readingBufferLock = 0;
        void ReadSample(long index)
        {
            index -= buffer.Length / 2;

            try
            {
                ThreadTask.Lock(ref streamLock);

                if (stream == null || reader == null)
                    return;

                long datasLength = samples * channels;
                int bufferLength = buffer.Length;

                int startOffset = (int)(-index).Clamp(0);
                int endOffset = (int)(index + bufferLength - datasLength);
                index += startOffset + index;
                //index = index.Clamp(0);

                if (startOffset > 0)
                {
                    stream.Position = (datasLength - startOffset) * 4;
                    reader.Read(buffer, 0, startOffset);

                    for (int i = 0; i < startOffset; i++)
                        calculatedBuffer[i] = buffer[i];
                }

                if (index >= 0 && index < datasLength - endOffset)
                {
                    //현재 읽고 있는 스트림 위치랑 읽어야하는 스트림 위치가 서로 다를경우 이를 맞춰줌
                    if (stream.Position != index * 4)
                        stream.Position = index * 4;

                    reader.Read(buffer, 0, bufferLength - endOffset);
                    for (int i = 0; i < bufferLength - endOffset; i++)
                        calculatedBuffer[startOffset + i] = buffer[i];
                }

                if (endOffset > 0)
                {
                    stream.Position = 0;
                    reader.Read(buffer, 0, endOffset);

                    for (int i = 0; i < endOffset; i++)
                        calculatedBuffer[startOffset + bufferLength - endOffset + i] = buffer[i];
                }
            }
            finally
            {
                ThreadTask.Unlock(ref streamLock);
            }

            try
            {
                ThreadTask.Lock(ref readingBufferLock);

                Array.Copy(readingBuffer, calculatedBuffer, calculatedBuffer.Length);
                Interlocked.Exchange(ref currentReadingIndex, index);
            }
            finally
            {
                ThreadTask.Unlock(ref readingBufferLock);
            }
        }



        /// <summary>Thread-safe</summary>
        public override float GetSample(long index)
        {
            if (index < 0 || index >= samples * channels)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (isSmall)
                return readingBuffer[index];

            if (!CheckPlayingObject(playingObject))
                return 0;

            long currentReadingIndex = Interlocked.Read(ref this.currentReadingIndex);
            index -= currentReadingIndex;

            Debug.Log(currentReadingIndex);

            if (index < buffer.Length * 0.25f || index >= buffer.Length * 0.75f)
                UniTask.RunOnThreadPool(() => ReadSample(index));

            if (index < 0 || index >= readingBuffer.Length)
                return 0;

            try
            {
                ThreadTask.Lock(ref readingBufferLock);
                return readingBuffer[index];
            }
            finally
            {
                ThreadTask.Unlock(ref readingBufferLock);
            }
        }

        bool CheckPlayingObject(object? playingObject)
        {
            if (isSmall)
                return true;

            if (playingObject == null)
                return false;

            try
            {
                ThreadTask.Lock(ref playingObjectLock);

                this.playingObject ??= playingObject;

                return this.playingObject == playingObject;
            }
            finally
            {
                ThreadTask.Unlock(ref playingObjectLock);
            }
        }

        [Obsolete("Legacy Code", true)]
        public override void SetStreamPosition(long index)
        {
            if (isSmall)
                return;

            UniTask.RunOnThreadPool(() => ReadSample(index));
        }

        public override void Dispose()
        {
            try
            {
                ThreadTask.Lock(ref streamLock);

                stream?.Dispose();

                stream = null;
                reader = null;
            }
            finally
            {
                ThreadTask.Unlock(ref streamLock);
            }
        }
    }
}
