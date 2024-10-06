using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeDictionaryKeyProperty : SerializedTypeProperty
    {
        internal SerializedTypeDictionaryKeyProperty(Type listType, object targetKeyObject, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : this(listType, targetKeyObject, serializedType, propertyInfo, null, parent) { }

        internal SerializedTypeDictionaryKeyProperty(Type listType, object targetKeyObject, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : this(listType, targetKeyObject, serializedType, null, fieldInfo, parent) { }

        SerializedTypeDictionaryKeyProperty(Type propertyType, object targetKeyObject, SerializedType serializedType, PropertyInfo? propertyInfo, FieldInfo? fieldInfo, SerializedTypeProperty? parent = null) : base(propertyType, serializedType, propertyInfo, fieldInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        public override string propertyPath { get; }

        public override bool isNullable => false;

        public object targetKeyObject { get; private set; }

        protected internal override object? InternalGetValue(object? targetObject) => targetKeyObject;

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            object? obj = parent?.InternalGetValue(targetObject);
            if (obj == null)
                return;

            if (obj is IDictionary dictionary && !dictionary.Contains(value))
            {
                dictionary.RenameKey(targetKeyObject, value);
                targetKeyObject = value ?? throw new ArgumentNullException(nameof(targetObject));
            }
        }
    }
}
