#nullable enable
using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeDictionaryValueProperty : SerializedTypeProperty
    {
        internal SerializedTypeDictionaryValueProperty(object targetKeyObject, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : base(serializedType, propertyInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        internal SerializedTypeDictionaryValueProperty(object targetKeyObject, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : base(serializedType, fieldInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        public override Type realPropertyType => EditorTool.GetDictionaryType(base.realPropertyType).value;

        public override string propertyPath { get; }

        public object targetKeyObject { get; }

        protected override object? InternalGetValue(object? targetObject)
        {
            object? value = base.InternalGetValue(targetObject);
            if (value == null)
                return null;

            if (value is IDictionary dictionary)
                return dictionary[targetKeyObject];

            return null;
        }

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            object? value2 = base.InternalGetValue(targetObject);
            if (value2 == null)
                return;

            if (value2 is IDictionary dictionary)
                dictionary[targetKeyObject] = value;
        }
    }
}
