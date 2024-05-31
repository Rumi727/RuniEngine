#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Rhythms
{
    public abstract class BeatValuePairAniList<TValue, TPair> : BeatValuePairList<TValue, TPair>, IBeatValuePairAniList where TPair : IBeatValuePairAni<TValue>, new()
    {
        public BeatValuePairAniList(TValue defaultValue) : base(defaultValue) { }
        public BeatValuePairAniList(TValue defaultValue, IEnumerable<TPair> collection) : base(defaultValue, collection) { }



        public override TValue? GetValue(double currentBeat, out double beat)
        {
            TValue? value;
            if (Count <= 0)
            {
                beat = 0;
                value = defaultValue;
            }
            else
            {
                int index = GetValueIndexBinarySearch(currentBeat).Clamp(0);
                TPair beatValuePair = this[index];
                beat = beatValuePair.beat;

                if (beatValuePair.length == 0)
                    value = beatValuePair.value;
                else
                {
                    TPair previousBeatValuePair;
                    if (index <= 0)
                        previousBeatValuePair = Last();
                    else
                        previousBeatValuePair = this[index - 1];

                    double t = ((currentBeat - beatValuePair.beat) / beatValuePair.length).Clamp01();
                    if (!double.IsNormal(t))
                        t = 0;

                    if (beatValuePair.easingFunction == EasingFunction.Ease.Curve && beatValuePair.curve != null)
                        t = beatValuePair.curve.Evaluate((float)t);

                    value = ValueCalculate(t, beatValuePair.easingFunction, previousBeatValuePair.value, beatValuePair.value);
                }
            }

            return value;
        }

        public abstract TValue? ValueCalculate(double t, EasingFunction.Ease easingFunction, TValue? previousValue, TValue? value);



        public void Add(double beat = double.MinValue, double length = 0, bool confused = false) => Add(new TPair() { beat = beat, length = length, value = defaultValue, confused = confused });
        public void Add(double beat, double length, TValue? value, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear, bool confused = false) => Add(new TPair() { beat = beat, length = length, value = value, easingFunction = easingFunction, confused = confused });
        public void Add(double beat, double length, TValue? value, AnimationCurve curve, bool confused = false) => Add(new TPair() { beat = beat, length = length, value = value, easingFunction = EasingFunction.Ease.Curve, curve = curve, confused = confused });
    }
}
