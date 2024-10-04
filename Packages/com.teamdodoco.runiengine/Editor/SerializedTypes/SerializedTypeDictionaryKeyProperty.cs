#nullable enable
using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeDictionaryKeyProperty : SerializedTypeProperty
    {
        internal SerializedTypeDictionaryKeyProperty(object targetKeyObject, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : base(serializedType, propertyInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        internal SerializedTypeDictionaryKeyProperty(object targetKeyObject, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : base(serializedType, fieldInfo, parent)
        {
            this.targetKeyObject = targetKeyObject;

            if (parent == null)
                propertyPath = "[" + targetKeyObject + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetKeyObject + "]";
        }

        public override Type realPropertyType => EditorTool.GetDictionaryType(base.realPropertyType).key;

        public override string propertyPath { get; }

        public override bool isNullable => false;

        public object targetKeyObject { get; private set; }

        protected override object? InternalGetValue(object? targetObject) => targetKeyObject;

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            object? obj = base.InternalGetValue(targetObject);
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
