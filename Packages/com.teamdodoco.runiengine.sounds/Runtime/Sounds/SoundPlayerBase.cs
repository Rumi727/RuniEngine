#nullable enable
#if ENABLE_RUNI_ENGINE_POOLING
using RuniEngine.Pooling;
#endif
using RuniEngine.Resource.Sounds;
#if ENABLE_RUNI_ENGINE_RHYTHMS
using RuniEngine.Rhythms;
#endif
using RuniEngine.Threading;
using System;
using System.Threading;
using UnityEngine;

namespace RuniEngine.Sounds
{
    public abstract class SoundPlayerBase :
#if ENABLE_RUNI_ENGINE_POOLING
        ObjectPoolingBase
#else
        MonoBehaviour
#endif
#if ENABLE_RUNI_ENGINE_RHYTHMS
        , IRhythmable
#endif
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
        public virtual double realTime { get => time / tempo.Abs(); set => time = value * tempo.Abs(); }

        public event Action timeChanged { add => _timeChanged += value; remove => _timeChanged -= value; }
        Action? _timeChanged;

        public abstract double length { get; }
        public virtual double realLength => length / tempo.Abs();



        public bool isPlaying => Interlocked.Add(ref _isPlaying, 0) != 0;
        [HideInInspector, NonSerialized] int _isPlaying = 0;

        public bool isPaused
        {
            get => Interlocked.Add(ref _isPaused, 0) != 0;
            set => Interlocked.Exchange(ref _isPaused, value ? 1 : 0);
        }
        [SerializeField] int _isPaused = 0;



        public bool loop
        {
            get => Interlocked.Add(ref _loop, 0) != 0;
            set => Interlocked.Exchange(ref _loop, value ? 1 : 0);
        }
        [SerializeField] int _loop = 0;
        [HideInInspector, NonSerialized] int loopLock;

        public void LoopLock() => ThreadTask.Lock(ref loopLock);
        public void LoopUnlock() => ThreadTask.Unlock(ref loopLock);


        public event Action looped { add => _looped += value; remove => _looped -= value; }
        [HideInInspector, NonSerialized] Action? _looped;



        public virtual double pitch
        {
            get => Interlocked.CompareExchange(ref _pitch, 0, 0);
            set => Interlocked.Exchange(ref _pitch, value);
        }
        [HideInInspector, SerializeField, Range(0, 3)] double _pitch = 1;

        public virtual double metaDataPitch => soundMetaData != null ? soundMetaData.pitch : 1;
        public virtual double realPitch => pitch * metaDataPitch;

        public double tempo
        {
            get => Interlocked.CompareExchange(ref _tempo, 0, 0);
            set => Interlocked.Exchange(ref _tempo, value);
        }
        [HideInInspector, SerializeField, Range(-3, 3)] double _tempo = 1;

        public virtual double metaDataTempo => soundMetaData != null ? soundMetaData.tempo : 1;
        public virtual double realTempo => tempo * metaDataTempo;



        /// <summary>
        /// 이 프로퍼티는 에디터에서만 사용되며 런타임에 영향가지 않습니다
        /// <para></para>
        /// 피치와 템포를 서로 같은 비율로 고정합니다
        /// </summary>
        public bool pitchFixed
        {
            get => _pitchFixed;
            set
            {
                _pitchFixed = value;

                double tempo = this.tempo;
                double pitch = this.pitch;

                if (tempo == pitch)
                {
                    _pitchTempoRatio = 1;
                    _tempoPitchRatio = 1;
                }

                if (tempo != 0)
                {
                    if (pitch != 0)
                        _pitchTempoRatio = pitch / tempo;
                    else
                        _pitchTempoRatio = 1;
                }
                else
                    _pitchTempoRatio = 0;

                if (pitch != 0)
                {
                    if (tempo != 0)
                        _tempoPitchRatio = tempo / pitch;
                    else
                        _tempoPitchRatio = 1;
                }
                else
                    _tempoPitchRatio = 0;
            }
        }
        [HideInInspector, SerializeField] bool _pitchFixed = false;

        /// <summary>
        /// 이 프로퍼티는 에디터에서만 사용되며 런타임에 영향가지 않습니다
        /// <para></para>
        /// 피치와 템포의 비율입니다 피치를 고정할 때 사용됩니다 (pitchFixed 프로퍼티의 값이 바뀔 때만 변경됩니다)
        /// </summary>
        public double pitchTempoRatio => _pitchTempoRatio;
        [HideInInspector, SerializeField] double _pitchTempoRatio = 1;

        /// <summary>
        /// 이 프로퍼티는 에디터에서만 사용되며 런타임에 영향가지 않습니다
        /// <para></para>
        /// 템포와 피치의 비율입니다 템포를 고정할 때 사용됩니다 (pitchFixed 프로퍼티의 값이 바뀔 때만 변경됩니다)
        /// </summary>
        public double tempoPitchRatio => _tempoPitchRatio;
        [HideInInspector, SerializeField] double _tempoPitchRatio = 1;



        public float volume
        {
            get => Interlocked.CompareExchange(ref _volume, 0, 0);
            set => Interlocked.Exchange(ref _volume, value);
        }
        [HideInInspector, SerializeField, Range(0, 2)] float _volume = 1;



        public float panStereo
        {
            get => Interlocked.CompareExchange(ref _panStereo, 0, 0);
            set => Interlocked.Exchange(ref _panStereo, value);
        }
        [HideInInspector, SerializeField, Range(-1, 1)] float _panStereo = 0;



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
        [HideInInspector, NonSerialized] int onAudioFilterReadEventLock = 0;



#if ENABLE_RUNI_ENGINE_RHYTHMS
        double IRhythmable.rhythmOffset => soundMetaData?.rhythmOffset ?? 0;
        BeatBPMPairList? IRhythmable.bpms => soundMetaData?.bpms;
#endif



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



#if ENABLE_RUNI_ENGINE_POOLING
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
#endif
    }
}