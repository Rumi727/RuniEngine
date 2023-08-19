#nullable enable
using System.Collections.Specialized;
using System;

namespace RuniEngine
{
    public static class TypeUtility
    {
        static readonly HybridDictionary defaultValueMap = new HybridDictionary();

        public static object? GetDefaultValue(this Type type)
        {
            if (!type.IsValueType)
                return null;

            if (defaultValueMap.Contains(type))
                return defaultValueMap[type];

            object defaultValue = Activator.CreateInstance(type);
            defaultValueMap[type] = defaultValue;

            return defaultValue;
        }

        public static object GetDefaultValueNotNull(this Type type)
        {
            if (defaultValueMap.Contains(type))
                return defaultValueMap[type];

            object defaultValue = Activator.CreateInstance(type);
            defaultValueMap[type] = defaultValue;

            return defaultValue;
        }
    }
}