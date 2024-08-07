#nullable enable
using System.Reflection;
using System;
using System.Collections.Generic;

namespace RuniEngine
{
    public static class ReflectionManager
    {
        static ReflectionManager()
        {
            assemblys = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> result = new List<Type>();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Count; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
                for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                {
                    Type type = types[typesIndex];
                    result.Add(type);
                }
            }

            types = result.ToArray();
        }

        /// <summary>
        /// All loaded assemblys
        /// </summary>
        public static IReadOnlyList<Assembly> assemblys { get; }

        /// <summary>
        /// All loaded types
        /// </summary>
        public static IReadOnlyList<Type> types { get; }

        public static bool IsSubtypeOf<T>(this Type type) => type != typeof(T) && typeof(T).IsAssignableFrom(type);
        public static bool IsSubtypeOf(this Type type, Type surclass) => type != surclass && surclass.IsAssignableFrom(type);

        public static bool AttributeContains<T>(this MemberInfo element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this MemberInfo element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this Assembly element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this Assembly element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this ParameterInfo element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this ParameterInfo element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this Module element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this Module element, Type attribute) => element.GetCustomAttributes(attribute, false).Length > 0;
    }
}
