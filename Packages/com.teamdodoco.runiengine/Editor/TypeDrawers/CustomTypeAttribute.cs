#nullable enable
using System;

namespace RuniEngine.Editor.TypeDrawers
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class CustomTypeDrawerAttribute : Attribute
    {
        public CustomTypeDrawerAttribute(Type targetType) => this.targetType = targetType;

        public Type targetType { get; }
    }
}