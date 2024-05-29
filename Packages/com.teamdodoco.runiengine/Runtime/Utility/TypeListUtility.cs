#nullable enable
using System.Collections.Generic;

namespace RuniEngine
{
    public static class TypeListUtility
    {
        public static TypeList<TSource> ToTypeList<TSource>(this IEnumerable<TSource> source) => new TypeList<TSource>(source);
    }
}
