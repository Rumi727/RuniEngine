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
        public RhythmWatch(IRhythmable? rhythmable, bool isGlobal = true) : this(rhythmable, null, 0, isGlobal) => _isCustomBPM = false;
        public RhythmWatch(BeatBPMPairList? bpms, double offset, bool isGlobal = true) : this(null, bpms, offset, isGlobal) => _isCustomBPM = true;
        public RhythmWatch(IRhythmable? rhythmable, BeatBPMPairList? bpms, double offset, bool isGlobal = true)
        {
            this.isGlobal = isGlobal;

            _rhythmable = rhythmable;
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



        public IRhythmable? rhythmable
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _rhythmable;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _rhythmable = value;
            }
        }
        IRhythmable? _rhythmable;

        public BeatBPMPairList? bpms
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (isCustomBPM)
                    return _bpms;
                else
                    return rhythmable?.bpms;
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
                    return rhythmable?.rhythmOffset ?? 0;
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

                return (rhythmable?.time - offset) ?? 0;
            }
            set
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (rhythmable != null)
                    rhythmable.time = value + offset;
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

            _rhythmable = null;
            _bpms = null;

            isGlobal = false;

            _instances.Remove(this);
            isDisposed = true;
        }
    }
}
