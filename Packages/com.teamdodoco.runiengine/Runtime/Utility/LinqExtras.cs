using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtras
    {
        public static bool Contains<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Intersect(second).Any();
        public static bool Contains<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) => first.Intersect(second, comparer).Any();

        public static bool ContainsAll<T>(this IEnumerable<T> first, IEnumerable<T> second) => !second.Except(first).Any();
        public static bool ContainsAll<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) => !second.Except(first, comparer).Any();
    }
}
