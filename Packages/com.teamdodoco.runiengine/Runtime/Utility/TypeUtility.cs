#nullable enable
using System;

namespace RuniEngine
{
    public static class TypeUtility
    {
        public static object? GetDefaultValue(this Type type)
        {
            if (!type.IsValueType)
                return null;

            return Activator.CreateInstance(type);
        }

        public static object GetDefaultValueNotNull(this Type type)
        {
            if (type == typeof(string))
                return string.Empty;

            return Activator.CreateInstance(type);
        }
    }
}