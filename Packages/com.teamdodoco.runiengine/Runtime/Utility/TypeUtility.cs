#nullable enable
using System;
using System.Collections.Generic;

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

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType) => IsAssignableToGenericType(givenType, genericType, out _);
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType, out Type? resultType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    resultType = it;
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                resultType = givenType;
                return true;
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                resultType = null;
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType, out resultType);
        }

        public static IEnumerable<Type> GetHierarchy(this Type type)
        {
            if (type == typeof(object))
                yield break;

            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}