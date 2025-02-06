using System.Runtime.CompilerServices;
using System;

namespace RuniEngine.Spans
{
    public static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanSplitter<T> Split<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T> => new SpanSplitter<T>(source, separator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanSplitter<T> Split<T>(this Span<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T> => new SpanSplitter<T>(source, separator);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanAnySplitter<T> SplitAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T> => new SpanAnySplitter<T>(source, separator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanAnySplitter<T> SplitAny<T>(this Span<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T> => new SpanAnySplitter<T>(source, separator);
    }
}