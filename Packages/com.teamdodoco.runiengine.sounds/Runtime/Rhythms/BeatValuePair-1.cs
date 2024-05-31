#nullable enable
using Newtonsoft.Json;
using System;

namespace RuniEngine.Rhythms
{
    public struct BeatValuePair<T> : IBeatValuePair<T>
    {
        public BeatValuePair(double beat, T value, bool confused = false)
        {
            this.beat = beat;
            this.value = value;

            this.confused = confused;
        }



        [JsonIgnore] public readonly Type type => typeof(T);



        public double beat;
        public T? value;

        public bool confused;



        double IBeatValuePair.beat { readonly get => beat; set => beat = value; }

        T? IBeatValuePair<T>.value { readonly get => value; set => this.value = value; }
        object? IBeatValuePair.value { readonly get => value; set => this.value = (T?)value; }

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
