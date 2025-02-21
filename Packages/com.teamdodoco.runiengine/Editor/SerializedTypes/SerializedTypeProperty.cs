#nullable enable
using RuniEngine.Editor.TypeDrawers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public class SerializedTypeProperty
    {
        internal SerializedTypeProperty(SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : this(propertyInfo.PropertyType, serializedType, propertyInfo, null, parent) { }

        internal SerializedTypeProperty(SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : this(fieldInfo.FieldType, serializedType, null, fieldInfo, parent) { }

        protected SerializedTypeProperty(Type propertyType, SerializedType serializedType, PropertyInfo? propertyInfo, FieldInfo? fieldInfo, SerializedTypeProperty? parent)
        {
            this.serializedType = serializedType;
            this.propertyInfo = propertyInfo;
            this.fieldInfo = fieldInfo;
            this.parent = parent;

            realPropertyType = propertyType;

            if (this.IsNullableValueType())
                propertyType = propertyType.GenericTypeArguments[0];

            this.propertyType = propertyType;

            isUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(propertyType);

            isArray = typeof(IList).IsAssignableFrom(propertyType) || typeof(IDictionary).IsAssignableFrom(propertyType);
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
            TypeDrawer? result = null;

            for (int i = 0; i < TypeDrawer.attributeDrawers.Count && result == null; i++)
            {
                Type type = TypeDrawer.typeDrawers[i];
                CustomTypeDrawerAttribute attribute = type.GetCustomAttribute<CustomTypeDrawerAttribute>();

                if (AttributeContains(attribute.targetType))
                {
                    result = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[] { this }, null) as TypeDrawer;
                    break;
                }
            }

            for (int i = 0; i < TypeDrawer.typeDrawers.Count && result == null; i++)
            {
                Type type = TypeDrawer.typeDrawers[i];
                CustomTypeDrawerAttribute attribute = type.GetCustomAttribute<CustomTypeDrawerAttribute>();

                if ((attribute.targetType.IsGenericTypeDefinition && type.IsAssignableToGenericType(attribute.targetType)) || attribute.targetType.IsAssignableFrom(propertyType))
                {
                    result = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[] { this }, null) as TypeDrawer;
                    break;
                }
            }

            return result;
        }

        public PropertyInfo? propertyInfo { get; }
        public FieldInfo? fieldInfo { get; }

        public SerializedType serializedType { get; }

        /// <summary>호출 스크립트가 Drawer에 전달하고 싶은 데이터를 담는 프로퍼티입니다</summary>
        public Dictionary<string, object?> metaData => serializedType.metaData;


        public TypeDrawer typeDrawer { get; }

        public SerializedTypeProperty? parent { get; }

        public Type declaringType => propertyInfo?.DeclaringType ?? fieldInfo?.DeclaringType ?? typeof(object);
        public Type propertyType { get; }

        public Type realPropertyType { get; }

        public virtual string propertyPath { get; } = string.Empty;
        public int depth { get; } = 0;

        public string name => propertyInfo?.Name ?? fieldInfo?.Name ?? string.Empty;

        public virtual bool canRead => ((parent?.canRead ?? true) || isArray) && ((propertyInfo != null && propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null) || fieldInfo != null);
        public virtual bool canWrite => ((parent?.canWrite ?? true) || isArray) && ((propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetSetMethod(false) != null) || (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral));

        public bool isStatic => propertyInfo?.GetAccessors(true)[0].IsStatic ?? fieldInfo?.IsStatic ?? false;

        public bool isValid => propertyInfo != null || fieldInfo != null;

        public bool isUnityObject { get; }

        public virtual bool isArray { get; } = false;
        public bool isInArray { get; } = false;

        /// <summary>false일 경우, 프로퍼티 타입이 Nullable 타입이여도 null 버튼을 표시하지 않게함</summary>
        public virtual bool isNullable => true;

        public bool isMixed
        {
            get
            {
                if (serializedType.targetObjects.Length <= 1)
                    return false;

                object? firstValue = GetValue(0);
                for (int i = 1; i < serializedType.targetObjects.Length; i++)
                {
                    object? value = GetValue(i);
                    if (firstValue == null && value == null)
                        continue;
                    else if (firstValue == null)
                        return true;
                    else if (value == null)
                        return true;

                    if (!firstValue.Equals(value))
                        return true;
                }

                return false;
            }
        }

        public bool isNullMixed
        {
            get
            {
                if (serializedType.targetObjects.Length <= 1)
                    return false;

                object? firstValue = GetValue(0);
                for (int i = 1; i < serializedType.targetObjects.Length; i++)
                {
                    object? value = GetValue(i);
                    if (firstValue == null && value == null)
                        continue;
                    else if (firstValue == null)
                        return true;
                    else if (value == null)
                        return true;
                }

                return false;
            }
        }

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

        public T? GetCustomAttribute<T>(bool inherit = true) where T : Attribute => (T?)GetCustomAttribute(typeof(T), inherit);
        public Attribute? GetCustomAttribute(Type attribute, bool inherit = true)
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
                return propertyInfo.GetCustomAttributes<T>(inherit);
            else if (fieldInfo != null)
                return fieldInfo.GetCustomAttributes<T>(inherit);

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



        /// <summary>선택한 오브젝트 중에서 null 값이 아닌 아닌 프로퍼티 중 첫번째 프로퍼티를 가져옵니다</summary>
        public object? GetValue() => GetNotNullValues().FirstOrDefault();
        public object? GetValue(int index) => InternalGetValue(serializedType.targetObjects[index]);

        protected internal virtual object? InternalGetValue(object? targetObject) => propertyInfo?.GetValue(targetObject) ?? fieldInfo?.GetValue(targetObject);

        /// <summary>선택된 인스턴스 중에서 Null 값인 인스턴스만 Null이 아닌 값으로 설정합니다<br/><see cref="canRead"/>가 false일 경우 무조건 인스턴스를 새로 만듭니다</summary>
        public void SetNonNullValue()
        {
            for (int i = 0; i < serializedType.targetObjects.Length; i++)
            {
                object? targetObject = serializedType.targetObjects[i];
                if (!canRead || InternalGetValue(targetObject) == null)
                    InternalSetValue(targetObject, typeDrawer.CreateInstance());
            }
        }

        public void SetValue(object? value)
        {
            for (int i = 0; i < serializedType.targetObjects.Length; i++)
                InternalSetValue(serializedType.targetObjects[i], value);
        }
        public void SetValue(int index, object? value) => InternalSetValue(serializedType.targetObjects[index], value);

        protected virtual void InternalSetValue(object? targetObject, object? value)
        {
            /*
             * 구조체는 밑의 코드가 없으면 값이 변경되지 않음
             * 또한, 같은 값만 바꿔줘야함 (전체를 바꾸게 되면 리스트 값이 손상됨)
             */

            int? index = parent?.GetValues().IndexOf(targetObject);

            propertyInfo?.SetValue(targetObject, value);
            fieldInfo?.SetValue(targetObject, value);

            if (index != null && index >= 0)
                parent?.SetValue(index.Value, targetObject);
        }
    }
}
