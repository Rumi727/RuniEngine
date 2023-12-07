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
        public virtual double realTime { get => time / realTempo; set => time = value * realTempo; }

        public event Action timeChanged { add => _timeChanged += value; remove => _timeChanged -= value; }
        Action? _timeChanged;

        public abstract double length { get; }
        public virtual double realLength => length / realTempo;



        public bool isPlaying => Interlocked.Add(ref _isPlaying, 0) != 0;
        [NonSerialized] int _isPlaying = 0;

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

        
        public event Action looped { add => _looped += value; remove => _looped -= value; }
        [NonSerialized] Action? _looped;



        public float pitch
        {
            get => Interlocked.CompareExchange(ref _pitch, 0, 0).Clamp(0);
            set => Interlocked.Exchange(ref _pitch, value.Clamp(0));
        }
        [SerializeField, Range(0, 3)] float _pitch = 1;

        public virtual float realPitch => pitch * (soundMetaData != null ? soundMetaData.pitch : 1);

        public float tempo
        {
            get => Interlocked.CompareExchange(ref _tempo, 0, 0);
            set => Interlocked.Exchange(ref _tempo, value);
        }
        [SerializeField, Range(-3, 3)] float _tempo = 1;

        public virtual double realTempo => tempo * (soundMetaData != null ? soundMetaData.tempo : 1);



        public float volume
        {
            get => Interlocked.CompareExchange(ref _volume, 0, 0);
            set => Interlocked.Exchange(ref _volume, value);
        }
        [SerializeField, Range(0, 2)] float _volume = 1;



        public float panStereo
        {
            get => Interlocked.CompareExchange(ref _panStereo, 0, 0);
            set => Interlocked.Exchange(ref _panStereo, value);
        }
        [SerializeField, Range(-1, 1)] float _panStereo = 0;



        public bool spatial { get => _spatial; set => _spatial = value; }
        [SerializeField] bool _spatial = false;



        public float minDistance { get => _minDistance; set => _minDistance = value; }
        [SerializeField, Range(0, 32)] float _minDistance = 1;

        public float maxDistance { get => _maxDistance; set => _maxDistance = value; }
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
                ThreadManager.Lock(ref onAudioFilterReadEventLock);
                _onAudioFilterReadEvent += value;
                ThreadManager.Unlock(ref onAudioFilterReadEventLock);
            }
            remove
            {
                ThreadManager.Lock(ref onAudioFilterReadEventLock);
                _onAudioFilterReadEvent -= value;
                ThreadManager.Unlock(ref onAudioFilterReadEventLock);
            }
        }
        event OnAudioFilterReadAction? _onAudioFilterReadEvent;
        [NonSerialized] int onAudioFilterReadEventLock = 0;

        protected virtual void OnDisable() => Stop();

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _looped = null;
            _timeChanged = null;
            _onAudioFilterReadEvent = null;
        }



        protected virtual void OnAudioFilterRead(float[] data, int channels)
        {
            ThreadManager.Lock(ref onAudioFilterReadEventLock);

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
                ThreadManager.Unlock(ref onAudioFilterReadEventLock);
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

            key = "";
            nameSpace = "";

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
