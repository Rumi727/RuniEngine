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
            get
            {
                ThreadManager.Lock(ref isPausedLock);
                bool result = _isPaused;
                ThreadManager.Unlock(ref isPausedLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref isPausedLock);
                _isPaused = value;
                ThreadManager.Unlock(ref isPausedLock);
            }
        }
        [SerializeField] bool _isPaused = false;
        int isPausedLock;



        public bool loop
        {
            get
            {
                ThreadManager.Lock(ref loopLock);
                bool result = _loop;
                ThreadManager.Unlock(ref loopLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref loopLock);
                _loop = value;
                ThreadManager.Unlock(ref loopLock);
            }
        }
        [SerializeField] bool _loop = false;
        int loopLock;

        public void LoopLock() => ThreadManager.Lock(ref loopLock);
        public void LoopUnlock() => ThreadManager.Unlock(ref loopLock);


        public event Action looped { add => _looped += value; remove => _looped -= value; }
        [NonSerialized] Action? _looped;



        public double pitch
        {
            get
            {
                ThreadManager.Lock(ref pitchLock);
                double result = _pitch;
                ThreadManager.Unlock(ref pitchLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref pitchLock);
                _pitch = value;
                ThreadManager.Unlock(ref pitchLock);
            }
        }
        [SerializeField, Range(0, 3)] double _pitch = 1;
        int pitchLock;

        public void PitchLock() => ThreadManager.Lock(ref pitchLock);
        public void PitchUnlock() => ThreadManager.Unlock(ref pitchLock);

        public virtual double realPitch => pitch * (soundMetaData != null ? soundMetaData.pitch : 1);

        public double tempo
        {
            get
            {
                ThreadManager.Lock(ref tempoLock);
                double result = _tempo;
                ThreadManager.Unlock(ref tempoLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref tempoLock);
                _tempo = value;
                ThreadManager.Unlock(ref tempoLock);
            }
        }
        [SerializeField, Range(-3, 3)] double _tempo = 1;
        int tempoLock;

        public void TempoLock() => ThreadManager.Lock(ref tempoLock);
        public void TempoUnlock() => ThreadManager.Unlock(ref tempoLock);

        public virtual double realTempo => tempo * (soundMetaData != null ? soundMetaData.tempo : 1);



        public double volume
        {
            get
            {
                ThreadManager.Lock(ref volumeLock);
                double result = _volume;
                ThreadManager.Unlock(ref volumeLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref volumeLock);
                _volume = value;
                ThreadManager.Unlock(ref volumeLock);
            }
        }
        [SerializeField, Range(0, 2)] double _volume = 1;
        int volumeLock;

        public void VolumeLock() => ThreadManager.Lock(ref volumeLock);
        public void VolumeUnlock() => ThreadManager.Unlock(ref volumeLock);



        public double panStereo
        {
            get
            {
                ThreadManager.Lock(ref panStereoLock);
                double result = _panStereo;
                ThreadManager.Unlock(ref panStereoLock);

                return result;
            }
            set
            {
                ThreadManager.Lock(ref panStereoLock);
                panStereo = value;
                ThreadManager.Unlock(ref panStereoLock);
            }
        }
        [SerializeField, Range(-1, 1)] double _panStereo = 0;
        int panStereoLock;

        public void PanStereoLock() => ThreadManager.Lock(ref panStereoLock);
        public void PanStereoUnlock() => ThreadManager.Unlock(ref panStereoLock);




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