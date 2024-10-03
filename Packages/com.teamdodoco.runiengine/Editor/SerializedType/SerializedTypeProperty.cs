#nullable enable
using RuniEngine.Editor.TypeDrawers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public class SerializedTypeProperty
    {
        protected internal SerializedTypeProperty(SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null)
        {
            this.serializedType = serializedType;
            this.propertyInfo = propertyInfo;
            this.parent = parent;

            Type propertyType = realPropertyType;
            if (this.IsNullableValueType())
                propertyType = propertyType.GenericTypeArguments[0];

            this.propertyType = propertyType;

            isUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(propertyType);

            isArray = typeof(IList).IsAssignableFrom(propertyType);
            isInArray = parent != null && (parent.isArray || parent.isInArray);

            if (parent == null)
                propertyPath = name;
            else
            {
                propertyPath = parent.propertyPath + "." + name;
                depth = parent.depth + 1;
            }

            typeDrawer = GetTypeDrawer(propertyType) ?? new ObjectTypeDrawer(this);
        }

        protected internal SerializedTypeProperty(SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null)
        {
            this.serializedType = serializedType;
            this.fieldInfo = fieldInfo;
            this.parent = parent;

            Type propertyType = realPropertyType;
            if (this.IsNullableValueType())
                propertyType = propertyType.GenericTypeArguments[0];

            this.propertyType = propertyType;

            isUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(propertyType);

            isArray = typeof(IList).IsAssignableFrom(propertyType);
            isInArray = parent != null && (parent.isArray || parent.isInArray);

            if (parent == null)
                propertyPath = name;
            else
            {
                propertyPath = parent.propertyPath + "." + name;
                depth = parent.depth + 1;
            }

            typeDrawer = GetTypeDrawer(propertyType) ?? new ObjectTypeDrawer(this);
        }

        TypeDrawer? GetTypeDrawer(Type propertyType)
        {
            for (int i = 0; i < TypeDrawer.typeDrawers.Count; i++)
            {
                Type type = TypeDrawer.typeDrawers[i];
                CustomTypeDrawerAttribute attribute = type.GetCustomAttribute<CustomTypeDrawerAttribute>();

                if (attribute.targetType.IsAssignableFrom(propertyType))
                    return (TypeDrawer)Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[] { this }, null);
            }

            return null;
        }

        public PropertyInfo? propertyInfo { get; }
        public FieldInfo? fieldInfo { get; }

        public SerializedType serializedType { get; }
        public TypeDrawer typeDrawer { get; }

        public SerializedTypeProperty? parent { get; }

        public Type declaringType => propertyInfo?.DeclaringType ?? fieldInfo?.DeclaringType ?? typeof(object);
        public Type propertyType { get; }

        public virtual Type realPropertyType => propertyInfo?.PropertyType ?? fieldInfo?.FieldType ?? typeof(object);

        public virtual string propertyPath { get; } = string.Empty;
        public int depth { get; } = 0;

        public string name => propertyInfo?.Name ?? fieldInfo?.Name ?? string.Empty;

        public bool canRead => ((parent?.canRead ?? true) || parent.isArray) && ((propertyInfo != null && propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null) || fieldInfo != null);
        public bool canWrite => ((parent?.canWrite ?? true) || parent.isArray) && ((propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetSetMethod(false) != null) || (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral));

        public bool isStatic => propertyInfo?.GetAccessors(true)[0].IsStatic ?? fieldInfo?.IsStatic ?? false;

        public bool isValid => propertyInfo != null || fieldInfo != null;

        public bool isUnityObject { get; }

        public bool isArray { get; } = false;
        public bool isInArray { get; } = false;

        public bool isExpanded { get; set; } = false;

        public bool AttributeContains<T>() => AttributeContains(typeof(T));
        public bool AttributeContains(Type attribute)
        {
            if (propertyInfo != null)
                return propertyInfo.AttributeContains(attribute);
            else if (fieldInfo != null)
                return fieldInfo.AttributeContains(attribute);

            return false;
        }

        public T GetCustomAttribute<T>(bool inherit = true) where T : Attribute => (T)GetCustomAttribute(typeof(T), inherit);
        public Attribute GetCustomAttribute(Type attribute, bool inherit = true)
        {
            if (propertyInfo != null)
                return propertyInfo.GetCustomAttribute(attribute, inherit);
            else if (fieldInfo != null)
                return fieldInfo.GetCustomAttribute(attribute, inherit);

            throw new NullReferenceException();
        }

        public IEnumerable<T> GetCustomAttributes<T>(bool inherit = true) where T : Attribute
        {
            if (propertyInfo != null)
                return propertyInfo.GetCustomAttributes<T>();
            else if (fieldInfo != null)
                return fieldInfo.GetCustomAttributes<T>();

            return (IEnumerable<T>)Array.Empty<Attribute>();
        }

        public Attribute[] GetCustomAttributes() => GetCustomAttributes(true);

        public Attribute[] GetCustomAttributes(Type type) => GetCustomAttributes(type, true);
        public Attribute[] GetCustomAttributes(bool inherit) => GetCustomAttributes(typeof(Attribute), inherit);

        public Attribute[] GetCustomAttributes(Type type, bool inherit)
        {
            if (propertyInfo != null)
                return Attribute.GetCustomAttributes(propertyInfo, type, inherit);
            else if (fieldInfo != null)
                return Attribute.GetCustomAttributes(fieldInfo, type, inherit);

            return Array.Empty<Attribute>();
        }

        public bool isMixed
        {
            get
            {
                if (serializedType.targetObjects.Length > 0)
                    return false;

                object? firstValue = GetValue(0);
                for (int i = 1; i < serializedType.targetObjects.Length; i++)
                {
                    if (firstValue != GetValue(i))
                        return true;
                }

                return false;
            }
        }



        public bool IsListValueMixed(int index)
        {
            if (!isArray)
                throw new InvalidCastException();

            if (serializedType.targetObjects.Length > 0)
                return false;

            IList? list = (IList?)GetValue();

            object? firstValue = list?[0];
            for (int i = 1; i < serializedType.targetObjects.Length; i++)
            {
                if (firstValue != list?[i])
                    return true;
            }

            return false;
        }



        public IEnumerable<object?> GetValues()
        {
            for (int i = 0; i < serializedType.targetObjects.Length; i++)
                yield return GetValue(i);
        }

        public IEnumerable<object> GetNotNullValues()
        {
            for (int i = 0; i < serializedType.targetObjects.Length; i++)
            {
                object? obj = GetValue(i);
                if (obj != null)
                    yield return obj;
            }
        }



        public object? GetValue() => InternalGetValue(serializedType.targetObject);
        public object? GetValue(int index) => InternalGetValue(serializedType.targetObjects[index]);

        protected virtual object? InternalGetValue(object? targetObject) => propertyInfo?.GetValue(targetObject) ?? fieldInfo?.GetValue(targetObject);

        public void SetValue(object? value)
        {
            for (int i = 0; i < serializedType.targetObjects.Length; i++)
                InternalSetValue(serializedType.targetObjects[i], value);
        }

        protected virtual void InternalSetValue(object? targetObject, object? value)
        {
            propertyInfo?.SetValue(targetObject, value);
            fieldInfo?.SetValue(targetObject, value);

            parent?.SetValue(targetObject);
        }
    }
}