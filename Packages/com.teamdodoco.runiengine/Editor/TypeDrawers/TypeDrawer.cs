#nullable enable
using RuniEngine.Editor.SerializedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.TypeDrawers
{
    [InitializeOnLoad]
    public abstract class TypeDrawer
    {
        static TypeDrawer()
        {
            List<Type> typeDrawers = new List<Type>(ReflectionManager.types.Count);

            for (int i = 0; i < ReflectionManager.types.Count; i++)
            {
                Type type = ReflectionManager.types[i];
                if (!type.IsSubtypeOf<TypeDrawer>() || !type.AttributeContains<CustomTypeDrawerAttribute>())
                    continue;

                typeDrawers.Add(type);
            }
            
            _typeDrawers = typeDrawers.OrderByDescending(x => GetHierarchy(x.GetCustomAttribute<CustomTypeDrawerAttribute>().targetType).Count()).ToArray();
        }

        protected TypeDrawer(SerializedTypeProperty property) => this.property = property;

        /// <summary>Sorted Type hierarchy</summary>
        public static IReadOnlyList<Type> typeDrawers => _typeDrawers;
        private static readonly Type[] _typeDrawers;

        public SerializedTypeProperty property { get; }

        public virtual float GetPropertyHeight() => EditorGUIUtility.singleLineHeight;

        public void OnGUI(Rect position, string? label) => OnGUI(position, label != null ? new GUIContent(label) : null);
        public virtual void OnGUI(Rect position, GUIContent? label) => EditorGUI.LabelField(position, new GUIContent(label), new GUIContent("No GUI Implemented"));

        static IEnumerable<Type> GetHierarchy(Type type)
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
