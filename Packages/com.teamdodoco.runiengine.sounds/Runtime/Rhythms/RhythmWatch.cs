#nullable enable
using RuniEngine.Sounds;
using System;
using System.Collections.Generic;

namespace RuniEngine.Rhythms
{
    public class RhythmWatch : IDisposable
    {
        public static IReadOnlyList<RhythmWatch> instances => _instances;
        static readonly List<RhythmWatch> _instances = new List<RhythmWatch>();

        public static RhythmWatch? instance { get; private set; }

        public RhythmWatch(bool isGlobal = true) : this(null, null, 0, isGlobal) => _isCustomBPM = false;
        public RhythmWatch(SoundPlayerBase? soundPlayer, bool isGlobal = true) : this(soundPlayer, null, 0, isGlobal) => _isCustomBPM = false;
        public RhythmWatch(BeatBPMPairList? bpms, double offset, bool isGlobal = true) : this(null, bpms, offset, isGlobal) => _isCustomBPM = true;
        public RhythmWatch(SoundPlayerBase? soundPlayer, BeatBPMPairList? bpms, double offset, bool isGlobal = true)
        {
            this.isGlobal = isGlobal;

            _soundPlayer = soundPlayer;
            _bpms = bpms;
            _offset = offset;

            _isCustomBPM = true;

            _instances.Add(this);
        }

        

        public bool isGlobal
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return instance == this;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (value)
                    instance = this;
                else if (instance == this)
                    instance = null;
            }
        }



        public SoundPlayerBase? soundPlayer
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _soundPlayer;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _soundPlayer = value;
            }
        }
        SoundPlayerBase? _soundPlayer;

        public BeatBPMPairList? bpms
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (isCustomBPM)
                    return _bpms;
                else
                    return soundPlayer != null ? soundPlayer.soundMetaData?.bpms : null;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _bpms = value;
                _isCustomBPM = value != null;
            }
        }
        BeatBPMPairList? _bpms;

        public double offset
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (isCustomBPM)
                    return _offset;
                else
                    return soundPlayer != null ? (soundPlayer.soundMetaData?.rhythmOffset ?? 0) : 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _offset = value;
                _isCustomBPM = double.IsNormal(offset);
            }
        }
        double _offset;

        public bool isCustomBPM
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _isCustomBPM;
            }
        }
        bool _isCustomBPM = true;



        public double currentTime
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return soundPlayer != null ? soundPlayer.time - offset : 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (soundPlayer != null)
                    soundPlayer.time = value + offset;
            }
        }

        public double currentBeat
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return bpms?.SecondToBeat(currentTime) ?? 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                currentTime = bpms?.BeatToSecond(value) ?? 0;
            }
        }



        public bool isDisposed { get; private set; }

        public void Dispose()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            _soundPlayer = null;
            _bpms = null;

            isGlobal = false;

            _instances.Remove(this);
            isDisposed = true;
        }
    }
}
