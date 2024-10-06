using Cysharp.Threading.Tasks;
using NAudio.Wave;
using RuniEngine.Threading;
using System;
using System.Threading;

namespace RuniEngine.Resource.Sounds
{
    [Obsolete("분명 복잡성으로 인해 스레드에 안전하지 않을겁니다... 애초에 더럽게 느려요...ㅋㅋㅋ", true)]
    public sealed class RawAudioClipLegacyBackgroundLoader : RawAudioClipLoaderBase
    {
        public RawAudioClipLegacyBackgroundLoader(WaveStream stream) : base(stream)
        {
            this.stream = stream;
            reader = stream.ToSampleProvider();

            //streamLoadedDatas = new float?[samples * channels];
            buffer = new float[frequency * channels]; //1초 버퍼

            long readSampleLength = reader.Read(buffer, 0, buffer.Length);
            currentPosition = readSampleLength.Min(samples * channels);
        }

        /// <summary>Thread-safe</summary>
        public override bool isLoading => Interlocked.Add(ref _isLoading, 0) != 0;
        int _isLoading = 0;

        /// <summary>Thread-safe</summary>
        public override bool isLoaded => Interlocked.Add(ref _isLoaded, 0) != 0;
        int _isLoaded = 0;

        /// <summary>Thread-safe</summary>
        public override bool isStream => false;

        /// <summary>Thread-safe</summary>
        public bool isDisposed => Interlocked.Add(ref _isDisposed, 0) != 0;
        int _isDisposed = 0;



        int isBackgroundLoadStop = 0;
        void BackgroundLoadThread()
        {
            Interlocked.Exchange(ref _isLoading, 1);

            while (true)
            {
                {
                    ThreadTask.Lock(ref streamLock);
                    if (stream == null || reader == null)
                        continue;

                    if (isBackgroundLoadStop != 0)
                    {
                        Interlocked.Exchange(ref _isLoading, 0);
                        Interlocked.Exchange(ref isBackgroundLoadStop, 0);

                        return;
                    }

                    ThreadTask.Unlock(ref streamLock);
                }

                int readSampleLength;
                long position = Interlocked.Read(ref currentPosition);


                long datasLength;
                try
                {
                    ThreadTask.Lock(ref loadedDatasLock);
                    if (isBackgroundLoadStop != 0)
                    {
                        Interlocked.Exchange(ref _isLoading, 0);
                        Interlocked.Exchange(ref isBackgroundLoadStop, 0);

                        return;
                    }

                    datasLength = loadedDatas.LongLength;

                    //중간에 읽는 위치가 바뀔 수 있기 때문에, 이미 불러온 스트림은 스킵하는 과정
                    {
                        bool isLoaded = true;
                        for (int i = 0; i < datasLength; i++)
                        {
                            if (!float.IsFinite(loadedDatas[position]))
                            {
                                isLoaded = false;
                                break;
                            }

                            position++;
                            while (position >= datasLength)
                                position = 0;
                        }

                        //이 과정에서 불러오지 않은 스트림이 발견되지 않은 경우 (null 값이 존재하지 않는 경우) 로드된 상태로 간주
                        if (isLoaded)
                        {
                            Interlocked.Exchange(ref _isLoaded, 1);
                            return;
                        }
                    }
                }
                finally
                {
                    ThreadTask.Unlock(ref loadedDatasLock);
                }

                int bufferLength = buffer.Length;

                //현재 읽고 있는 스트림 위치랑 읽어야하는 스트림 위치가 서로 다를경우 이를 맞춰줌 (보통 AudioPlayer 클래스에 의해 발생)
                if (stream.Position != position * 4)
                    stream.Position = position * 4;

                readSampleLength = reader.Read(buffer, 0, bufferLength);
                readSampleLength -= (int)(position + readSampleLength - datasLength).Clamp(0);

                if (readSampleLength <= 0)
                    continue;

                //읽은 버퍼를 스트림 데이터에 동기화
                try
                {
                    ThreadTask.Lock(ref getSampleLock);
                    if (isBackgroundLoadStop != 0)
                    {
                        Interlocked.Exchange(ref _isLoading, 0);
                        Interlocked.Exchange(ref isBackgroundLoadStop, 0);

                        return;
                    }

                    for (int i = 0; i < readSampleLength; i++)
                        loadedDatas[position + i] = buffer[i];
                }
                catch { }
                finally
                {
                    ThreadTask.Unlock(ref getSampleLock);
                }

                Interlocked.Add(ref position, readSampleLength);
            }
        }



        WaveStream? stream;
        int streamLock = 0;
        ISampleProvider? reader;
        float[] loadedDatas = Array.Empty<float>();
        int loadedDatasLock = 0;
        readonly float[] buffer = Array.Empty<float>();
        long currentPosition = 0;
        int getSampleLock = 0;
        /// <summary>Thread-safe</summary>
        public override float GetSample(long index)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            long datasLength = samples * channels;
            if (index < 0 || index >= datasLength)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!isLoading)
            {
                Interlocked.Exchange(ref currentPosition, index);
                Interlocked.Exchange(ref _isLoading, 1);

                loadedDatas = new float[datasLength];
                Array.Fill(loadedDatas, float.NaN);

                UniTask.RunOnThreadPool(BackgroundLoadThread);
            }

            try
            {
                ThreadTask.Lock(ref getSampleLock);

                float data = loadedDatas[index];
                return float.IsFinite(data) ? data : 0;
            }
            finally
            {
                ThreadTask.Unlock(ref getSampleLock);
            }
        }

        [Obsolete("Legacy Code", true)]
        public override void SetStreamPosition(long index)
        {
            if (!isLoaded)
                Interlocked.Exchange(ref currentPosition, index.Clamp(0, loadedDatas.LongLength - 1));
        }

        public override void Dispose()
        {
            Interlocked.Exchange(ref _isDisposed, 1);

            try
            {
                ThreadTask.Lock(ref loadedDatasLock);
                ThreadTask.Lock(ref getSampleLock);

                loadedDatas = Array.Empty<float>();

                ThreadTask.Lock(ref streamLock);

                stream?.Dispose();
                stream = null;
                reader = null;

                Interlocked.Exchange(ref isBackgroundLoadStop, 1);
            }
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
            }
            finally
            {
                ThreadTask.Unlock(ref streamLock);
                ThreadTask.Unlock(ref getSampleLock);
                ThreadTask.Unlock(ref loadedDatasLock);
            }
        }
    }
}
