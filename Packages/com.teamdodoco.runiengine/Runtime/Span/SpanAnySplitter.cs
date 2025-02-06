using System;
using System.Runtime.CompilerServices;

namespace RuniEngine.Spans
{
    public readonly ref struct SpanAnySplitter<T> where T : IEquatable<T>
    {
        readonly ReadOnlySpan<T> _source;
        readonly ReadOnlySpan<T> _separator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanAnySplitter(ReadOnlySpan<T> source, ReadOnlySpan<T> separator)
        {
            if (separator.Length == 0)
                throw new ArgumentException("Requires non-empty value", nameof(separator));

            _source = source;
            _separator = separator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(_source, _separator);

        public ref struct Enumerator
        {
            int _nextStartIndex;

            readonly ReadOnlySpan<T> _source;
            readonly ReadOnlySpan<T> _separator;

            SpanSplitValue<T> _current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> separator)
            {
                if (separator.Length == 0)
                    throw new ArgumentException("Requires non-empty value", nameof(separator));

                _nextStartIndex = 0;

                _source = source;
                _separator = separator;

                _current = new SpanSplitValue<T>();
            }

            public bool MoveNext()
            {
                if (_nextStartIndex > _source.Length)
                    return false;

                ReadOnlySpan<T> nextSource = _source.Slice(_nextStartIndex);

                int foundIndex = nextSource.IndexOfAny(_separator);
                int length = foundIndex >= 0 ? foundIndex : nextSource.Length;

                _current = new SpanSplitValue<T>(_source, _nextStartIndex, length);
                _nextStartIndex += _current.Length + 1;

                return true;
            }

            public readonly SpanSplitValue<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _current;
            }
        }
    }
}
