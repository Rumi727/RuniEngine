using Newtonsoft.Json;
using System;
using UnityEngine;

namespace RuniEngine.Rhythms
{
    [Serializable]
    public struct BeatValuePairAni<T> : IBeatValuePairAni<T>
    {
        public BeatValuePairAni(double beat, T value, double length, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;

            this.easingFunction = easingFunction;
            curve = null;
        }

        public BeatValuePairAni(double beat, T value, double length, AnimationCurve curve)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;

            easingFunction = EasingFunction.Ease.Curve;
            this.curve = curve;
        }



        [JsonIgnore] public readonly Type type => typeof(T);



        [FieldName("gui.beat")] public double beat;
        [FieldName("gui.value")] public T? value;

        [FieldName("gui.length")] public double length;

        [FieldName("gui.ease")] public EasingFunction.Ease easingFunction;
        [FieldName("gui.curve")] public AnimationCurve? curve;



        double IBeatValuePair.beat { readonly get => beat; set => beat = value; }

        T? IBeatValuePair<T>.value { readonly get => value; set => this.@value = value; }
        object? IBeatValuePair.value { readonly get => value; set => this.@value = (T?)value; }

        double IBeatValuePairAni.length { readonly get => length; set => length = value; }

        EasingFunction.Ease IBeatValuePairAni.easingFunction { readonly get => easingFunction; set => easingFunction = value; }
        AnimationCurve? IBeatValuePairAni.curve { readonly get => curve; set => curve = value; }

        //bool IBeatValuePair.confused { readonly get => confused; set => confused = value; }



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
