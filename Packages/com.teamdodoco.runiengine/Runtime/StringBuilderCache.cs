#nullable enable
using System.Collections.Concurrent;
using System.Text;

namespace RuniEngine
{
    public static class StringBuilderCache
    {
        static readonly ConcurrentBag<StringBuilder> cachedStringBuilders = new ConcurrentBag<StringBuilder>();

        public static StringBuilder Acquire()
        {
            if (cachedStringBuilders.TryTake(out StringBuilder? result))
            {
                if (result != null)
                    return result;
            }

            return new StringBuilder();
        }

        public static string Release(StringBuilder builder)
        {
            string result = builder.ToString();

            builder.Clear();
            cachedStringBuilders.Add(builder);

            return result;
        }

        public static void Clear() => cachedStringBuilders.Clear();
    }
}
