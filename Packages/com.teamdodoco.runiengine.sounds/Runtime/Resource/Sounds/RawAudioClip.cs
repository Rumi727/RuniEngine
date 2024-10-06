using NAudio.Wave;
using System;
using System.Threading;
using UnityEngine;

namespace RuniEngine.Resource.Sounds
{
    /// <summary>
    /// 원시 오디오 클립입니다
    /// <br/><br/>
    /// 이 클래스에 넘겨준 <see cref="WaveStream"/> 인스턴스는 넘겨준 시점부터 이 클래스가 관리하기에 분명한 목적이 있지 않는 이상 <see cref="WaveStream"/> 인스턴스는 건들지 마세요
    /// </summary>
    public sealed class RawAudioClip : IDisposable//, IReadOnlyList<float>
    {
        /// <summary>
        /// 제 생각이 맞다면 WaveStream 인스턴스를 다른 코드가 영향을 주지 않는다고 가정하면 Thread-safe 입니다
        /// <br/>
        /// 생성자이기에 인자값으로 받는 WaveStream 인스턴스를 제외한 다른 모든건 이 클래스 내부 변수라 완전히 계산이 끝나기 전에는 인스턴스가 할당되지 않을 것으로 생각해요
        /// <br/>
        /// 그래서 다른 클래스가 이 내부 변수를 먼저 읽는건 불가능 하다고 생각합니다 (리플랙션은 Thread-safe 처리 자채가 의미가 없으니 예외)
        /// <br/>
        /// 아니라면 확실하게 Thread-Safe 처리 해야지 뭐.. 어려운것도 아니니
        /// </summary>
        /// <param name="stream"></param>
        public RawAudioClip(WaveStream stream, RawAudioLoadType loadType)
        {
            loader = loadType switch
            {
                RawAudioLoadType.instant => new RawAudioClipInstLoader(stream),
                RawAudioLoadType.background => new RawAudioClipBackgroundLoader(stream),
                RawAudioLoadType.stream => new RawAudioClipStreamLoader(stream),
                _ => new RawAudioClipInstLoader(stream),
            };

            ResourceManager.RegisterManagedResource(this);
        }

        public RawAudioClip(RawAudioClipLoaderBase loader)
        {
            this.loader = loader;
            ResourceManager.RegisterManagedResource(this);
        }

        /// <summary>유니티 오디오 클립은 수동으로 해제시켜주세요!</summary>
        public RawAudioClip(AudioClip audioClip)
        {
            //NotMainThreadException.Exception();
            loader = new RawAudioClipUnityLoader(audioClip);

            ResourceManager.RegisterManagedResource(this);
        }



        public float this[int index] => GetSample(index);
        public float this[long index] => GetSample(index);




        public RawAudioClipLoaderBase loader { get; }



        /// <summary>
        /// 오디오 데이터 (유니티의 오디오 클립으로 로드하거나 즉시 로드하는 오디오 클립만 지원합니다)
        /// <br/><br/>
        /// Thread-safe
        /// </summary>
        /*public IReadOnlyList<float> datas
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (loadType == RawAudioLoadType.immediately)
                    return _datas;
                else
                    throw new InvalidOperationException("Streaming audio cannot load data into an array immediately. Instead, use the GetSample method.");
            }
        }
        readonly float[] _datas = Array.Empty<float>();*/



        /// <summary>샘플 레이트</summary>
        public int frequency => loader.frequency;

        /// <summary>채널 수</summary>
        public int channels => loader.channels;

        /// <summary>샘플 단위 길이<br/>실제 메모리에 있는 데이터 배열의 길이는 <c>samples * channels</c> 입니다</summary>
        public long samples => loader.samples;

        /// <summary>초 단위 길이</summary>
        public float length => loader.length;

        /// <summary>배열의 길이</summary>
        public long arrayLength => loader.arrayLength;



        /// <summary>Thread-safe</summary>
        public RawAudioLoadType loadType { get; }



        public bool isDisposed => Interlocked.Add(ref _isDisposed, 0) != 0;
        int _isDisposed = 0;



        public float GetSample(long index)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            return loader.GetSample(index);
        }

        [Obsolete("Legacy Code", true)] public void SetStreamPosition(long index) => loader.SetStreamPosition(index);

        [Obsolete("Legacy Code", true)] public void Playing() => loader.Playing();
        [Obsolete("Legacy Code", true)] public void Stopping() => loader.Stopping();

        public void Dispose()
        {
            Interlocked.Exchange(ref _isDisposed, 1);
            loader.Dispose();
        }



        /*int IReadOnlyCollection<float>.Count => (int)(samples * channels);

        public IEnumerator<float> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        sealed class Enumerator : IEnumerator<float>
        {
            public Enumerator(RawAudioClip rawAudioClip) => this.rawAudioClip = rawAudioClip;

            readonly RawAudioClip rawAudioClip;
            int position = -1;

            float IEnumerator<float>.Current => rawAudioClip[position];
            object IEnumerator.Current => rawAudioClip[position];

            bool IEnumerator.MoveNext()
            {
                if (position >= rawAudioClip.length)
                {
                    position = -1;
                    return false;
                }

                position++;
                return position < rawAudioClip.length;
            }

            void IEnumerator.Reset() => position = -1;

            void IDisposable.Dispose() { }
        }*/
    }
}
