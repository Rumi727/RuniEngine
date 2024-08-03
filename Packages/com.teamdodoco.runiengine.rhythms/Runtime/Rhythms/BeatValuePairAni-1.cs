#nullable enable
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace RuniEngine.Rhythms
{
    public struct BeatValuePairAni<T> : IBeatValuePairAni<T>
    {
        public BeatValuePairAni(double beat, T value, double length, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear, bool confused = false)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;

            this.easingFunction = easingFunction;
            curve = null;

            this.confused = confused;
        }

        public BeatValuePairAni(double beat, T value, double length, AnimationCurve curve, bool confused = false)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;

            easingFunction = EasingFunction.Ease.Curve;
            this.curve = curve;

            this.confused = confused;
        }



        [JsonIgnore] public readonly Type type => typeof(T);



        public double beat;
        public T? value;

        public double length;

        public EasingFunction.Ease easingFunction;
        public AnimationCurve? curve;

        public bool confused;



        double IBeatValuePair.beat { readonly get => beat; set => beat = value; }

        T? IBeatValuePair<T>.value { readonly get => value; set => this.value = value; }
        object? IBeatValuePair.value { readonly get => value; set => this.value = (T?)value; }

        double IBeatValuePairAni.length { readonly get => length; set => length = value; }

        EasingFunction.Ease IBeatValuePairAni.easingFunction { readonly get => easingFunction; set => easingFunction = value; }
        AnimationCurve? IBeatValuePairAni.curve { readonly get => curve; set => curve = value; }

        bool IBeatValuePair.confused { readonly get => confused; set => confused = value; }



        public readonly int CompareTo(object? value)
        {
            if (value == null)
                return 1;

            if (value is not IBeatValuePair num)
                throw new ArgumentException();

            return beat.CompareTo(num.beat);
        }

        public readonly int CompareTo(IBeatValuePair value) => beat.CompareTo(value.beat);

        public readonly int CompareTo(double value) => beat.CompareTo(value);
    }
}
