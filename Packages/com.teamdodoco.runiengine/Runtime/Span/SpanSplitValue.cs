using System;

namespace RuniEngine.Spans
{
    public readonly ref struct SpanSplitValue<T> where T : IEquatable<T>
    {
        public SpanSplitValue(ReadOnlySpan<T> source, int startIndex, int length)
        {
            Source = source;

            StartIndex = startIndex;
            Length = length;
        }

        public ReadOnlySpan<T> Source { get; }

        public int StartIndex { get; }
        public int Length { get; }

        public ReadOnlySpan<T> AsSpan() => Source.Slice(StartIndex, Length);

        public static implicit operator ReadOnlySpan<T>(SpanSplitValue<T> value) => value.AsSpan();
    }
}
