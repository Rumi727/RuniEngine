#nullable enable
using System;
using System.Reflection;

namespace RuniEngine.Editor.TypeDrawers
{
    public sealed class ObjectTypeDrawer : TypeDrawer
    {
        public ObjectTypeDrawer(Type type) : base(type) { }

        public ObjectTypeDrawer(object? instance) : base(instance) { }

        public override Type targetType => typeof(object);

        protected override void Recalculate(Type? type, object? instance)
        {
            if (type == null)
                return;
            else if (targetType.IsAssignableFrom(type))
                throw new InvalidOperationException();

            BindingFlags bindingFlags = BindingFlags.Public;
            if (instance == null)
                bindingFlags |= BindingFlags.Static;
            else
                bindingFlags |= BindingFlags.Instance;

            /*PropertyInfo propertyInfo = type.GetProperties(bindingFlags);
            if (propertyInfo.)*/
        }
    }
}
