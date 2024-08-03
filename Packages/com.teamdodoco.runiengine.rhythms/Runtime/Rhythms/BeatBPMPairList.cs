#nullable enable
using System.Collections.Generic;

namespace RuniEngine.Rhythms
{
    public class BeatBPMPairList : BeatValuePairList<BeatBPMPairList.BPM>
    {
        public struct BPM
        {
            public double bpm;
            public double timeSignatures;

            public BPM(double bpm, double timeSignatures = 4)
            {
                this.bpm = bpm;
                this.timeSignatures = timeSignatures;
            }
        }

        public struct ItemInfo
        {
            public BPM bpm;
            public double beat;
            public double time;

            public ItemInfo(BPM bpm, double beat, double time)
            {
                this.bpm = bpm;
                this.beat = beat;
                this.time = time;
            }
        }

        public BeatBPMPairList() : this(new BPM(60)) { }
        public BeatBPMPairList(BPM defaultValue) : base(defaultValue) => BPMCalculate();
        public BeatBPMPairList(IEnumerable<BeatValuePair<BPM>> collection) : base(new(60), collection) => BPMCalculate();
        public BeatBPMPairList(BPM defaultValue, IEnumerable<BeatValuePair<BPM>> collection) : base(defaultValue, collection) => BPMCalculate();

        public override BeatValuePair<BPM> this[int index]
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
        public override BPM GetValue(double currentBeat, out double beat)
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
        public override void Add(BeatValuePair<BPM> item)
        {
            if (item.beat > double.MinValue)
                base.Add(item);
            else
                base.Add(new BeatValuePair<BPM>(0, item.value, item.confused));

            BPMCalculate();
        }

        public override void Clear()
        {
            base.Clear();
            BPMCalculate();
        }

        public override void CopyTo(BeatValuePair<BPM>[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
            BPMCalculate();
        }

        public override void Insert(int index, BeatValuePair<BPM> item)
        {
            base.Insert(index, item);
            BPMCalculate();
        }

        public override bool Remove(BeatValuePair<BPM> item)
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

        protected readonly BeatValuePairList<ItemInfo> cachedBPMDataListSecond = new(new ItemInfo(new BPM(60, 4), 0, 0));
        protected readonly BeatValuePairList<ItemInfo> cachedBPMDataListBeat = new(new ItemInfo(new BPM(60, 4), 0, 0));
        public virtual void BPMCalculate()
        {
            cachedBPMDataListSecond.defaultValue = new ItemInfo(defaultValue, 0, 0);
            cachedBPMDataListBeat.defaultValue = new ItemInfo(defaultValue, 0, 0);

            cachedBPMDataListSecond.Clear();
            cachedBPMDataListBeat.Clear();

            double time = 0;
            for (int i = 0; i < Count; i++)
            {
                BeatValuePair<BPM> pair = this[i];
                double beat = pair.beat;
                if (beat <= double.MinValue)
                    beat = 0;

                if (i > 0)
                    time += (beat - this[i - 1].beat) * (60d / this[i - 1].value.bpm);

                cachedBPMDataListSecond.Add(time, new ItemInfo(pair.value, beat, time));
                cachedBPMDataListBeat.Add(beat, new ItemInfo(pair.value, beat, time));
            }
        }

        public virtual double SecondToBeat(double second)
        {
            ItemInfo info = cachedBPMDataListSecond.GetValue(second);
            return info.beat + ((second - info.time) * (info.bpm.bpm / 60d));
        }

        public virtual double BeatToSecond(double beat)
        {
            ItemInfo info = cachedBPMDataListBeat.GetValue(beat);
            return info.time + ((beat - info.beat) * (60d / info.bpm.bpm));
        }

        public virtual ItemInfo GetInfoUsingSecond(double second) => cachedBPMDataListSecond.GetValue(second);
        public virtual ItemInfo GetInfoUsingBeat(double beat) => cachedBPMDataListBeat.GetValue(beat);
    }
}
