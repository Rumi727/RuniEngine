using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeListProperty : SerializedTypeProperty
    {
        internal SerializedTypeListProperty(Type listType, int targetIndex, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : this(listType, targetIndex, serializedType, propertyInfo, null, parent) { }

        internal SerializedTypeListProperty(Type listType, int targetIndex, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : this(listType, targetIndex, serializedType, null, fieldInfo, parent) { }

        SerializedTypeListProperty(Type listType, int targetIndex, SerializedType serializedType, PropertyInfo? propertyInfo, FieldInfo? fieldInfo, SerializedTypeProperty? parent = null) : base(listType, serializedType, propertyInfo, fieldInfo, parent)
        {
            this.targetIndex = targetIndex;

            if (parent == null)
                propertyPath = "[" + targetIndex + "]";
            else
                propertyPath = parent.propertyPath + "[" + targetIndex + "]";
        }

        public override string propertyPath { get; }

        public override bool canRead => true;
        public override bool canWrite => true;

        public override bool isArray => true;

        public int targetIndex { get; set; }

        protected internal override object? InternalGetValue(object? targetObject)
        {
            object? value = parent?.InternalGetValue(targetObject);
            if (value == null)
                return null;

            if (value is IList list)
                return list[targetIndex];
            else if (value is IDictionary dictionary)
            {
                object? key = null;

                int index = 0;
                foreach (var item in dictionary.Keys)
                {
                    if (targetIndex == index)
                        key = item;

                    index++;
                }

                if (key != null)
                    return dictionary[key];
            }

            return null;
        }

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            object? value2 = parent?.InternalGetValue(targetObject);
            if (value2 == null)
                return;

            if (value2 is IList list)
                list[targetIndex] = value;
            else if (value2 is IDictionary dictionary)
            {
                object? key = null;

                int index = 0;
                foreach (var item in dictionary.Keys)
                {
                    if (targetIndex == index)
                        key = item;

                    index++;
                }

                if (key != null)
                    dictionary[key] = value;
            }
        }
    }
}
