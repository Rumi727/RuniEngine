#nullable enable
using RuniEngine.Editor.TypeDrawers;
using System;
using System.Collections;
using System.Reflection;

namespace RuniEngine.Editor.SerializedTypes
{
    public sealed class SerializedTypeListProperty : SerializedTypeProperty
    {
        internal SerializedTypeListProperty(int targetIndex, SerializedType serializedType, PropertyInfo propertyInfo, SerializedTypeProperty? parent = null) : base(serializedType, propertyInfo, parent)
        {
            this.targetIndex = targetIndex;

            if (parent == null)
                propertyPath = name;
            else
                propertyPath = parent.propertyPath + "[" + targetIndex + "]";
        }

        internal SerializedTypeListProperty(int targetIndex, SerializedType serializedType, FieldInfo fieldInfo, SerializedTypeProperty? parent = null) : base(serializedType, fieldInfo, parent)
        {
            this.targetIndex = targetIndex;

            if (parent == null)
                propertyPath = name;
            else
                propertyPath = parent.propertyPath + "[" + targetIndex + "]";
        }

        public override Type realPropertyType => EditorTool.GetListType(base.realPropertyType);

        public override string propertyPath { get; }

        public int targetIndex { get; }

        protected override object? InternalGetValue(object? targetObject) => ((IList?)base.InternalGetValue(targetObject))?[targetIndex];

        protected override void InternalSetValue(object? targetObject, object? value)
        {
            IList? list = (IList?)base.InternalGetValue(targetObject);
            if (list != null)
                list[targetIndex] = value;
        }
    }
}
