#nullable enable
using RuniEngine.Pooling;
using RuniEngine.Resource.Sounds;
using RuniEngine.Threading;
using System;
using System.Threading;
using UnityEngine;

namespace RuniEngine.Sounds
{
    public abstract class SoundPlayerBase : ObjectPoolingBase
    {
        public string key { get => _key; set => _key = value; }
        [SerializeField] string _key = "";

        public string nameSpace { get => _nameSpace; set => _nameSpace = value; }
        [SerializeField] string _nameSpace = "";

        public NameSpacePathPair nameSpacePathPair
        {
            get => new NameSpacePathPair(nameSpace, key);
            set
            {
                key = value.path;
                nameSpace = value.nameSpace;
            }
        }



        public abstract SoundData? soundData { get; }
        public abstract SoundMetaDataBase? soundMetaData { get; }



        public abstract double time { get; set; }
        public virtual double realTime { get => time / tempo; set => time = value * tempo; }

        public event Action timeChanged { add => _timeChanged += value; remove => _timeChanged -= value; }
        Action? _timeChanged;

        public abstract double length { get; }
        public virtual double realLength => length / tempo;



        public bool isPlaying => Interlocked.Add(ref _isPlaying, 0) != 0;
        [NonSerialized] int _isPlaying = 0;

        public bool isPaused
        {
            get
            {
                ThreadTask.Lock(ref isPausedLock);
                bool result = _isPaused;
                ThreadTask.Unlock(ref isPausedLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref isPausedLock);
                _isPaused = value;
                ThreadTask.Unlock(ref isPausedLock);
            }
        }
        [SerializeField] bool _isPaused = false;
        int isPausedLock;



        public bool loop
        {
            get
            {
                ThreadTask.Lock(ref loopLock);
                bool result = _loop;
                ThreadTask.Unlock(ref loopLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref loopLock);
                _loop = value;
                ThreadTask.Unlock(ref loopLock);
            }
        }
        [SerializeField] bool _loop = false;
        int loopLock;

        public void LoopLock() => ThreadTask.Lock(ref loopLock);
        public void LoopUnlock() => ThreadTask.Unlock(ref loopLock);


        public event Action looped { add => _looped += value; remove => _looped -= value; }
        [NonSerialized] Action? _looped;



        public virtual double pitch
        {
            get
            {
                ThreadTask.Lock(ref pitchLock);
                double result = _pitch;
                ThreadTask.Unlock(ref pitchLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref pitchLock);
                _pitch = value;
                ThreadTask.Unlock(ref pitchLock);
            }
        }
        [SerializeField, Range(0, 3)] double _pitch = 1;
        int pitchLock;

        public void PitchLock() => ThreadTask.Lock(ref pitchLock);
        public void PitchUnlock() => ThreadTask.Unlock(ref pitchLock);

        public virtual double metaDataPitch => soundMetaData != null ? soundMetaData.pitch : 1;
        public virtual double realPitch => pitch * metaDataPitch;

        public double tempo
        {
            get
            {
                ThreadTask.Lock(ref tempoLock);
                double result = _tempo;
                ThreadTask.Unlock(ref tempoLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref tempoLock);
                _tempo = value;
                ThreadTask.Unlock(ref tempoLock);
            }
        }
        [SerializeField, Range(-3, 3)] double _tempo = 1;
        int tempoLock;

        public void TempoLock() => ThreadTask.Lock(ref tempoLock);
        public void TempoUnlock() => ThreadTask.Unlock(ref tempoLock);

        public virtual double metaDataTempo => soundMetaData != null ? soundMetaData.tempo : 1;
        public virtual double realTempo => tempo * metaDataTempo;



        /// <summary>
        /// 이 프로퍼티는 에디터에서만 사용되며 런타임에 영향가지 않습니다
        /// <para></para>
        /// 피치와 템포를 서로 같은 값으로 고정합니다
        /// </summary>
        public bool pitchFixed { get => _pitchFixed; set => _pitchFixed = value; }
        [SerializeField] bool _pitchFixed = false;


        public float volume
        {
            get
            {
                ThreadTask.Lock(ref volumeLock);
                float result = _volume;
                ThreadTask.Unlock(ref volumeLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref volumeLock);
                _volume = value;
                ThreadTask.Unlock(ref volumeLock);
            }
        }
        [SerializeField, Range(0, 2)] float _volume = 1;
        int volumeLock;

        public void VolumeLock() => ThreadTask.Lock(ref volumeLock);
        public void VolumeUnlock() => ThreadTask.Unlock(ref volumeLock);



        public float panStereo
        {
            get
            {
                ThreadTask.Lock(ref panStereoLock);
                float result = _panStereo;
                ThreadTask.Unlock(ref panStereoLock);

                return result;
            }
            set
            {
                ThreadTask.Lock(ref panStereoLock);
                _panStereo = value;
                ThreadTask.Unlock(ref panStereoLock);
            }
        }
        [SerializeField, Range(-1, 1)] float _panStereo = 0;
        int panStereoLock;

        public void PanStereoLock() => ThreadTask.Lock(ref panStereoLock);
        public void PanStereoUnlock() => ThreadTask.Unlock(ref panStereoLock);




        public virtual bool spatial { get => _spatial; set => _spatial = value; }
        [SerializeField] bool _spatial = false;



        public virtual float minDistance { get => _minDistance; set => _minDistance = value; }
        [SerializeField, Range(0, 32)] float _minDistance = 1;

        public virtual float maxDistance { get => _maxDistance; set => _maxDistance = value; }
        [SerializeField, Range(0, 32)] float _maxDistance = 16;



        /// <summary>
        /// <para>Thread-Safe</para>
        /// <para>​</para>
        /// <para>Do not add more methods to this event from inside this event method like this. This causes deadlock</para>
        /// <code>
        /// onAudioFilterReadEvent += () =>
        /// {
        ///     onAudioFilterReadEvent += () => { };
        /// };
        /// </code>
        /// </summary>
        public event OnAudioFilterReadAction onAudioFilterReadEvent
        {
            add
            {
                ThreadTask.Lock(ref onAudioFilterReadEventLock);
                _onAudioFilterReadEvent += value;
                ThreadTask.Unlock(ref onAudioFilterReadEventLock);
            }
            remove
            {
                ThreadTask.Lock(ref onAudioFilterReadEventLock);
                _onAudioFilterReadEvent -= value;
                ThreadTask.Unlock(ref onAudioFilterReadEventLock);
            }
        }
        event OnAudioFilterReadAction? _onAudioFilterReadEvent;
        [NonSerialized] int onAudioFilterReadEventLock = 0;

        protected virtual void OnDisable() => Stop();

        protected void OnDestroy()
        {
            _looped = null;
            _timeChanged = null;
            _onAudioFilterReadEvent = null;
        }



        protected virtual void OnAudioFilterRead(float[] data, int channels)
        {
            ThreadTask.Lock(ref onAudioFilterReadEventLock);

            try
            {
                _onAudioFilterReadEvent?.Invoke(ref data, channels);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                ThreadTask.Unlock(ref onAudioFilterReadEventLock);
            }
        }



        protected void TimeChangedEventInvoke() => _timeChanged?.Invoke();
        protected void LoopedEventInvoke() => _looped?.Invoke();



        public abstract bool Refresh();

        public virtual void Play() => Interlocked.Exchange(ref _isPlaying, 1);

        public virtual void Stop() => Interlocked.Exchange(ref _isPlaying, 0);



        public override void Remove()
        {
            base.Remove();
            Stop();

            time = 0;
            loop = false;

            pitch = 1;
            tempo = 1;

            volume = 1;

            minDistance = 0;
            maxDistance = 16;

            panStereo = 0;



            spatial = false;



            _looped = null;
            _timeChanged = null;
            _onAudioFilterReadEvent = null;
        }
    }
}