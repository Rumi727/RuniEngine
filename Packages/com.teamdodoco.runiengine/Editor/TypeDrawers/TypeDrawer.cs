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

            _typeDrawers = typeDrawers.OrderByDescending(x => x.GetCustomAttribute<CustomTypeDrawerAttribute>().targetType.GetHierarchy().Count()).ToArray();
            _attributeDrawers = attributeDrawers.ToArray();
        }

        protected TypeDrawer(SerializedTypeProperty property) => this.property = property;

        /// <summary>Sorted Type hierarchy</summary>
        public static IReadOnlyList<Type> typeDrawers => _typeDrawers;
        private static readonly Type[] _typeDrawers;

        public static IReadOnlyList<Type> attributeDrawers => _attributeDrawers;
        private static readonly Type[] _attributeDrawers;

        public SerializedTypeProperty property { get; }

        public virtual bool canEditMultipleObjects => true;

        public float GetPropertyHeight()
        {
            try
            {
                return InternalGetPropertyHeight();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return EditorGUIUtility.singleLineHeight;
            }
        }

        protected virtual float InternalGetPropertyHeight() => EditorGUIUtility.singleLineHeight;

        public void OnGUI(Rect position, string? label) => OnGUI(position, label != null ? new GUIContent(label) : null);
        public void OnGUI(Rect position, GUIContent? label)
        {
            FieldNameAttribute? fieldName = property.GetCustomAttribute<FieldNameAttribute>();
            if (fieldName != null)
                label = new GUIContent(EditorTool.TryGetText(fieldName.name));

            if (!canEditMultipleObjects && property.serializedType.targetObjects.Length > 1)
            {
                EditorGUI.LabelField(position, new GUIContent($"{label} ({property.propertyType})"), new GUIContent(EditorTool.TryGetText("serialized_type.edit_multiple_objects")));
                return;
            }

            try
            {
                InternalOnGUI(position, label);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected virtual void InternalOnGUI(Rect position, GUIContent? label) => EditorGUI.LabelField(position, new GUIContent($"{label?.text} ({property.propertyType.Name})"), new GUIContent("No GUI Implemented"));

        public virtual object? CreateInstance()
        {
            if (property.propertyType == typeof(string))
                return string.Empty;

            try
            {
                object value;
                if (property.IsNullableValueType())
                    value = Activator.CreateInstance(property.realPropertyType, Activator.CreateInstance(property.propertyType));
                else
                    value = Activator.CreateInstance(property.propertyType);

                return value;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

            return null;
        }
    }
}
