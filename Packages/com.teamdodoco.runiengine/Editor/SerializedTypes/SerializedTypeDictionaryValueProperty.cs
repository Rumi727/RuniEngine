using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeDictionaryValueProperty : SerializedTypeProperty
    {
        internal SerializedTypeDictionaryValueProperty(Type listType, object targetKeyObject, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : this(listType, targetKeyObject, serializedType, propertyInfo, null, parent) { }

        internal SerializedTypeDictionaryValueProperty(Type listType, object targetKeyObject, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : this(listType, targetKeyObject, serializedType, null, fieldInfo, parent) { }

        SerializedTypeDictionaryValueProperty(Type propertyType, object targetKeyObject, SerializedType serializedType, PropertyInfo? propertyInfo, FieldInfo? fieldInfo, SerializedTypeProperty? parent = null) : base(propertyType, serializedType, propertyInfo, fieldInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        public override string propertyPath { get; }

        public object targetKeyObject { get; }

        protected internal override object? InternalGetValue(object? targetObject)
        {
            object? value = parent?.InternalGetValue(targetObject);
            if (value == null)
                return null;

            if (value is IDictionary dictionary)
                return dictionary[targetKeyObject];

            return null;
        }

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            object? value2 = parent?.InternalGetValue(targetObject);
            if (value2 == null)
                return;

            if (value2 is IDictionary dictionary)
                dictionary[targetKeyObject] = value;
        }
    }
}
