#nullable enable
using System.Collections.Generic;

namespace RuniEngine.Rhythms
{
    public class BeatBPMPairList : BeatValuePairList<double>
    {
        public struct BPMInfo
        {
            public double bpm;
            public double beat;
            public double time;

            public BPMInfo(double bpm, double beat, double time)
            {
                this.bpm = bpm;
                this.beat = beat;
                this.time = time;
            }
        }

        public BeatBPMPairList() : this(60) { }
        public BeatBPMPairList(double defaultValue) : base(defaultValue) => BPMCalculate();
        public BeatBPMPairList(IEnumerable<BeatValuePair<double>> collection) : base(60, collection) => BPMCalculate();
        public BeatBPMPairList(double defaultValue, IEnumerable<BeatValuePair<double>> collection) : base(defaultValue, collection) => BPMCalculate();

        public override BeatValuePair<double> this[int index]
        {
            get => base[index]; set
            {
                base[index] = value;
                BPMCalculate();
            }
        }

        /// <summary>
        /// 비트 값이 <see cref="double.MinValue"/>랑 같거나 작을경우 비트를 0으로 취급합니다 (초기값 이슈)
        /// </summary>
        /// <param name="item"></param>
        public override double GetValue(double currentBeat, out double beat)
        {
            if (currentBeat > double.MinValue)
                return base.GetValue(currentBeat, out beat);
            else
                return base.GetValue(0, out beat);
        }

        /// <summary>
        /// 비트 값이 <see cref="double.MinValue"/>랑 같거나 작을경우 비트를 0으로 취급합니다 (초기값 이슈)
        /// </summary>
        /// <param name="item"></param>
        public override int GetValueIndexBinarySearch(double beat)
        {
            if (beat > double.MinValue)
                return base.GetValueIndexBinarySearch(beat);
            else
                return base.GetValueIndexBinarySearch(0);
        }

        /// <summary>
        /// 비트 값이 <see cref="double.MinValue"/>랑 같거나 작을경우 비트를 0으로 취급합니다 (초기값 이슈)
        /// </summary>
        /// <param name="item"></param>
        public override void Add(BeatValuePair<double> item)
        {
            if (item.beat > double.MinValue)
                base.Add(item);
            else
                base.Add(new BeatValuePair<double>(0, item.value, item.confused));

            BPMCalculate();
        }

        public override void Clear()
        {
            base.Clear();
            BPMCalculate();
        }

        public override void CopyTo(BeatValuePair<double>[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
            BPMCalculate();
        }

        public override void Insert(int index, BeatValuePair<double> item)
        {
            base.Insert(index, item);
            BPMCalculate();
        }

        public override bool Remove(BeatValuePair<double> item)
        {
            bool result = base.Remove(item);
            BPMCalculate();

            return result;
        }

        public override void RemoveAt(int index)
        {   
            base.RemoveAt(index);
            BPMCalculate();
        }

        protected readonly BeatValuePairList<BPMInfo> cachedBPMDataListSecond = new(new BPMInfo(60, 0, 0));
        protected readonly BeatValuePairList<BPMInfo> cachedBPMDataListBeat = new(new BPMInfo(60, 0, 0));
        public virtual void BPMCalculate()
        {
            cachedBPMDataListSecond.defaultValue = new BPMInfo(defaultValue, 0, 0);
            cachedBPMDataListBeat.defaultValue = new BPMInfo(defaultValue, 0, 0);

            cachedBPMDataListSecond.Clear();
            cachedBPMDataListBeat.Clear();

            double time = 0;
            for (int i = 0; i < Count; i++)
            {
                BeatValuePair<double> pair = this[i];
                double beat = pair.beat;
                if (beat <= double.MinValue)
                    beat = 0;

                if (i > 0)
                {
                    time += (beat - this[i - 1].beat) * (60d / this[i - 1].value);
                }

                cachedBPMDataListSecond.Add(time, new BPMInfo(pair.value, beat, time));
                cachedBPMDataListBeat.Add(beat, new BPMInfo(pair.value, beat, time));
            }
        }

        public virtual double SecondToBeat(double second)
        {
            BPMInfo info = cachedBPMDataListSecond.GetValue(second);
            return info.beat + ((second - info.time) * (info.bpm / 60d));
        }

        public virtual double BeatToSecond(double beat)
        {
            BPMInfo info = cachedBPMDataListBeat.GetValue(beat);
            return info.time + ((beat - info.beat) * (60d / info.bpm));
        }

        public virtual BPMInfo GetBPMInfoUsingSecond(double second) => cachedBPMDataListSecond.GetValue(second);
        public virtual BPMInfo GetBPMInfoUsingBeat(double beat) => cachedBPMDataListBeat.GetValue(beat);
    }
}
