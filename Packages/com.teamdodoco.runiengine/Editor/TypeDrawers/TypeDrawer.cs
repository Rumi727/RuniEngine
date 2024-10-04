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
            List<Type> attributeDrawers = new List<Type>(ReflectionManager.types.Count);

            for (int i = 0; i < ReflectionManager.types.Count; i++)
            {
                Type type = ReflectionManager.types[i];
                if (!type.IsSubtypeOf<TypeDrawer>() || !type.AttributeContains<CustomTypeDrawerAttribute>())
                    continue;

                CustomTypeDrawerAttribute attribute = type.GetCustomAttribute<CustomTypeDrawerAttribute>();
                if (typeof(Attribute).IsAssignableFrom(attribute.targetType))
                    attributeDrawers.Add(type);
                else
                    typeDrawers.Add(type);
            }
            
            _typeDrawers = typeDrawers.OrderByDescending(x => GetHierarchy(x.GetCustomAttribute<CustomTypeDrawerAttribute>().targetType).Count()).ToArray();
        }

        protected TypeDrawer(SerializedTypeProperty property) => this.property = property;

        /// <summary>Sorted Type hierarchy</summary>
        public static IReadOnlyList<Type> typeDrawers => _typeDrawers;
        private static readonly Type[] _typeDrawers;

        public static IReadOnlyList<Type> attributeDrawers => _attributeDrawers;
        private static readonly Type[] _attributeDrawers;

        public SerializedTypeProperty property { get; }

        public virtual bool canEditMultipleObjects => true;

        public virtual float GetPropertyHeight() => EditorGUIUtility.singleLineHeight;

        public void OnGUI(Rect position, string? label) => OnGUI(position, label != null ? new GUIContent(label) : null);
        public void OnGUI(Rect position, GUIContent? label)
        {
            if (!canEditMultipleObjects && property.serializedType.targetObjects.Length > 1)
            {
                EditorGUI.LabelField(position, new GUIContent($"{label} ({property.propertyType})"), new GUIContent(EditorTool.TryGetText("serialized_type.edit_multiple_objects")));
                return;
            }

            InternalOnGUI(position, label);
        }

        protected virtual void InternalOnGUI(Rect position, GUIContent? label) => EditorGUI.LabelField(position, new GUIContent(label), new GUIContent("No GUI Implemented"));
    }
}
