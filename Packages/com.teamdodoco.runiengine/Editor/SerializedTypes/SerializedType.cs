#nullable enable
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.SerializedTypes
{
    /// <summary>
    /// 모든 타입을 에디터 GUI에 표시할 수 있도록 직렬화합니다. (단, 원시 타입이 아니여야합니다)
    /// <br/>
    /// 프로퍼티 또한 지원하지만, <see cref="SerializeField"/> 어트리뷰트가 붙어있어야합니다.
    /// </summary>
    public sealed class SerializedType
    {
        public SerializedType(Type targetType, bool isStatic) : this(targetType, isStatic, Array.Empty<object>()) { }
        public SerializedType(Type targetType, bool isStatic, object targetObject) : this(targetType, isStatic, new object[] { targetObject }) { }
        public SerializedType(Type targetType, bool isStatic, object?[] targetObjects) : this(targetType, null, isStatic, targetObjects) { }

        internal SerializedType(Type targetType, SerializedTypeProperty? parentProperty, bool isStatic, object?[] targetObjects)
        {
            if (!targetType.IsChildrenIncluded())
                throw new ArgumentException(nameof(targetType));

            this.isStatic = isStatic;

            this.targetType = targetType;
            this.targetObjects = targetObjects;

            PropertyInfo[] propertyInfos = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            FieldInfo[] fieldInfos = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            List<SerializedTypeProperty> properties = new List<SerializedTypeProperty>(propertyInfos.Length + fieldInfos.Length);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo memberInfo = propertyInfos[i];
                if (memberInfo.AttributeContains<JsonIgnoreAttribute>() || memberInfo.AttributeContains<NonSerializedAttribute>())
                    continue;

                if (!memberInfo.GetAccessors(true)[0].IsPublic && !memberInfo.AttributeContains<SerializeField>())
                    continue;

                SerializedTypeProperty property = new SerializedTypeProperty(this, memberInfo, parentProperty);
                properties.Add(property);
            }

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo memberInfo = fieldInfos[i];
                if (memberInfo.AttributeContains<JsonIgnoreAttribute>() || memberInfo.AttributeContains<NonSerializedAttribute>())
                    continue;

                if (!memberInfo.IsPublic && !memberInfo.AttributeContains<SerializeField>())
                    continue;

                SerializedTypeProperty property = new SerializedTypeProperty(this, memberInfo, parentProperty);
                properties.Add(property);
            }

            this.properties = properties.ToArray();
            this.parentProperty = parentProperty;
        }

        public Type targetType { get; }

        public bool isStatic { get; set; }

        public object? targetObject => targetObjects.Length > 0 ? targetObjects[0] : null;
        public object?[] targetObjects
        {
            get => _targetObjects;
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    object? obj = value[i];
                    if (obj != null && !targetType.IsAssignableFrom(obj.GetType()))
                        throw new InvalidCastException();
                }

                _targetObjects = value;
            }
        }
        object?[] _targetObjects = Array.Empty<object>();

        public bool isEditingMultipleObjects => targetObjects.Length > 1;

        public SerializedTypeProperty[] properties { get; }
        public SerializedTypeProperty? parentProperty { get; }



        /// <summary>호출 스크립트가 Drawer에 전달하고 싶은 데이터를 담는 프로퍼티입니다</summary>
        public Dictionary<string, object?> metaData { get; set; } = new Dictionary<string, object?>();



        public SerializedTypeProperty? GetProperty(string propertyName) => properties.FirstOrDefault(x => x.name == propertyName);



        public float GetPropertyHeight() => (properties.Where(x => (isStatic && x.isStatic) || (!isStatic && !x.isStatic)).Sum(x => x.typeDrawer.GetPropertyHeight() + 3) - 3).Clamp(0);

        public void DrawGUILayout()
        {
            Rect position = EditorGUILayout.GetControlRect(false, GetPropertyHeight());
            DrawGUI(position);
        }

        public void DrawGUI(Rect position)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                SerializedTypeProperty property = properties[i];

                if ((isStatic && property.isStatic) || (!isStatic && !property.isStatic))
                {
                    position.height = property.GetPropertyHeight();

                    property.DrawGUI(position, property.name);
                    position.y += position.height + 3;
                }
            }
        }
    }
}
