#nullable enable
using Cysharp.Threading.Tasks;
using NAudio.Wave;
using RuniEngine.Threading;
using System;
using System.Threading;

namespace RuniEngine.Resource.Sounds
{
    public sealed class RawAudioClipBackgroundLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipBackgroundLoader(WaveStream stream) : base(stream)
        {
            this.stream = stream;
            reader = stream.ToSampleProvider();

            loadedDatas = Array.Empty<float>();
            buffer = new float[frequency * channels];

            long datasLength = samples * channels;
            int bufferLength = buffer.Length;

            if (datasLength < bufferLength)
            {
                loadedDatas = new float[datasLength];
                _isLoaded = 1;
            }
            else
                loadedDatas = buffer;

            Read(0);
        }

        /// <summary>Thread-safe</summary>
        public override bool isLoading => Interlocked.Add(ref _isLoading, 0) != 0;
        int _isLoading;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => Interlocked.Add(ref _isLoaded, 0) != 0;
        int _isLoaded;

        /// <summary>Thread-safe</summary>
        public override bool isStream => false;

        /// <summary>Thread-safe</summary>
        public bool isDisposed => Interlocked.Add(ref _isDisposed, 0) != 0;
        int _isDisposed;



        int Read(long position)
        {
            if (reader == null)
                return -1;

            long datasLength = samples * channels;
            int bufferLength = buffer.Length;
            int readLength = (int)(bufferLength - (position + bufferLength - datasLength).Clamp(0));

            reader.Read(buffer, 0, bufferLength);

            try
            {
                ThreadTask.Lock(ref getSampleLock);
                Array.Copy(buffer, 0, loadedDatas, position, readLength);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }

            return readLength;
        }


        int unloadCount = 0;
        int disposeCount = 0;
        void BackgroundLoadThread(long position)
        {
            if (stream != null)
            {
                long datasLength = samples * channels;
                int bufferLength = buffer.Length;

                stream.Position = position * 4;

                int readLength = 0;
                for (long i = 0; i < datasLength; i += readLength)
                {
                    readLength = Read(position);
                    if (readLength < 0)
                        break;

                    position += readLength;

                    if (position >= datasLength)
                    {
                        position = 0;
                        stream.Position = position;
                    }

                    if (Interlocked.Add(ref unloadCount, 0) > 0)
                    {
                        try
                        {
                            ThreadTask.Lock(ref getSampleLock);
                            InternalUnload();
                        }
                        finally
                        {
                            ThreadTask.Unlock(ref getSampleLock);
                        }

                        return;
                    }

                    if (Interlocked.Add(ref disposeCount, 0) > 0)
                    {
                        try
                        {
                            ThreadTask.Lock(ref getSampleLock);
                            InternalDispose();
                        }
                        finally
                        {
                            ThreadTask.Unlock(ref getSampleLock);
                        }

                        return;
                    }
                }
            }

            ThreadTask.Lock(ref getSampleLock);

            Interlocked.Exchange(ref _isLoading, 0);
            Interlocked.Exchange(ref _isLoaded, 1);

            if (unloadCount > 0)
                InternalUnload();
            if (disposeCount > 0)
                InternalDispose();

            ThreadTask.Unlock(ref getSampleLock);
        }



        WaveStream? stream;
        ISampleProvider? reader;
        float[] loadedDatas;
        float[] buffer;
        int getSampleLock = 0;
        /// <summary>Thread-safe</summary>
        public override float GetSample(long index)
        {
            if (index < 0 || index >= samples * channels)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            try
            {
                ThreadTask.Lock(ref getSampleLock);

                if (!isLoading && !isLoaded && !isDisposed)
                {
                    Interlocked.Exchange(ref _isLoading, 1);
                    loadedDatas = new float[samples * channels];
                    
                    UniTask.RunOnThreadPool(() => BackgroundLoadThread(index));
                }
                
                return loadedDatas[index];
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
        }

        /// <summary>Thread-safe</summary>
        public override void Unload()
        {
            try
            {
                ThreadTask.Lock(ref getSampleLock);

                if (isLoading)
                    Interlocked.Increment(ref unloadCount);
                else
                    InternalUnload();
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
        }

        void InternalUnload()
        {
            Interlocked.Exchange(ref _isLoading, 0);
            Interlocked.Exchange(ref _isLoaded, 0);

            loadedDatas = Array.Empty<float>();
        }

        /// <summary>Thread-safe</summary>
        public override void Dispose()
        {
            try
            {
                ThreadTask.Lock(ref getSampleLock);

                if (isLoading)
                    Interlocked.Increment(ref disposeCount);
                else
                    InternalDispose();
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
        }

        void InternalDispose()
        {
            Interlocked.Exchange(ref _isDisposed, 1);

            Interlocked.Exchange(ref _isLoading, 0);
            Interlocked.Exchange(ref _isLoaded, 0);

            loadedDatas = Array.Empty<float>();
            buffer = Array.Empty<float>();

            stream?.Dispose();

            stream = null;
            reader = null;
        }
    }
}
