#nullable enable
using System;

namespace RuniEngine.APIBridge
{
    public abstract class APIBridge
    {
        public static Type type { get; } = typeof(object);

        protected APIBridge(object instance) => this.instance = instance;

        public object instance { get; }

        public abstract object CreateInstance();

        public override bool Equals(object obj) => instance.Equals(obj);
        public override int GetHashCode() => instance.GetHashCode();
        public override string ToString() => instance.ToString();
    }
}
